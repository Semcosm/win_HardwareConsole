using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class FansViewModel : INotifyPropertyChanged
{
    private readonly IReadOnlyList<SelectionOptionModel> _inputSensorOptions;
    private readonly IReadOnlyList<SelectionOptionModel> _outputControlOptions;
    private readonly FanPolicyPresentationMapper _presentationMapper;
    private readonly Dictionary<string, FanCurvePolicyDescriptor> _policiesById;
    private PolicyRuntimePreviewModel _preview = PolicyRuntimePreviewModel.CreateEmpty();

    public event PropertyChangedEventHandler? PropertyChanged;

    public FansViewModel(
        IPolicyRuntimeService policyRuntimeService,
        FanPolicyPresentationMapper presentationMapper)
    {
        _presentationMapper = presentationMapper;
        _inputSensorOptions = _presentationMapper.BuildInputSensorOptions();
        _outputControlOptions = _presentationMapper.BuildOutputControlOptions();
        var availablePolicies = policyRuntimeService.GetAvailableFanPolicies();
        _policiesById = BuildPolicyMap(availablePolicies);

        Policies = new ObservableCollection<FanPolicyEditorModel>();
        PreviewCurvePoints = new ObservableCollection<FanCurvePointRowModel>();

        foreach (var policy in availablePolicies)
        {
            Policies.Add(CreateEditorModel(policy));
        }

        _policyRuntimeService = policyRuntimeService;

        if (Policies.Count > 0)
        {
            var firstPolicy = Policies[0];
            PreviewPolicy(firstPolicy.PolicyId, firstPolicy.SelectedInputSensorId, firstPolicy.SelectedOutputControlId);
        }
    }

    private readonly IPolicyRuntimeService _policyRuntimeService;

    public ObservableCollection<FanPolicyEditorModel> Policies { get; }

    public ObservableCollection<FanCurvePointRowModel> PreviewCurvePoints { get; }

    public PolicyRuntimePreviewModel Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public void PreviewPolicy(string policyId, string inputSensorId, string outputControlId)
    {
        if (!_policiesById.TryGetValue(policyId, out var basePolicy))
        {
            Preview = PolicyRuntimePreviewModel.CreateEmpty();
            PreviewCurvePoints.Clear();
            return;
        }

        var previewPolicy = basePolicy with
        {
            InputSensorId = string.IsNullOrWhiteSpace(inputSensorId) ? basePolicy.InputSensorId : inputSensorId,
            OutputControlId = string.IsNullOrWhiteSpace(outputControlId) ? basePolicy.OutputControlId : outputControlId
        };

        var preview = _policyRuntimeService.PreviewFanPolicy(previewPolicy);

        ReplacePolicyModel(previewPolicy);
        RebuildPreviewCurvePoints(previewPolicy.Points);
        Preview = _presentationMapper.MapPreview(preview);
    }

    private FanPolicyEditorModel CreateEditorModel(FanCurvePolicyDescriptor policy)
    {
        return _presentationMapper.MapPolicyEditor(
            policy,
            _inputSensorOptions,
            _outputControlOptions);
    }

    private void ReplacePolicyModel(FanCurvePolicyDescriptor policy)
    {
        _policiesById[policy.Id] = policy;

        for (var index = 0; index < Policies.Count; index++)
        {
            if (string.Equals(Policies[index].PolicyId, policy.Id))
            {
                Policies[index] = CreateEditorModel(policy);
                return;
            }
        }
    }

    private void RebuildPreviewCurvePoints(IReadOnlyList<FanCurvePoint> points)
    {
        PreviewCurvePoints.Clear();

        foreach (var point in points)
        {
            PreviewCurvePoints.Add(_presentationMapper.MapCurvePoint(point));
        }
    }

    private static Dictionary<string, FanCurvePolicyDescriptor> BuildPolicyMap(
        IReadOnlyList<FanCurvePolicyDescriptor> policies)
    {
        var map = new Dictionary<string, FanCurvePolicyDescriptor>();

        foreach (var policy in policies)
        {
            map[policy.Id] = policy;
        }

        return map;
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
