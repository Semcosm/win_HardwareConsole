using System;
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
    private readonly IDiagnosticsSink _diagnosticsSink;
    private readonly IReadOnlyList<SelectionOptionModel> _inputSensorOptions;
    private readonly IReadOnlyList<SelectionOptionModel> _outputControlOptions;
    private readonly FanPolicyPresentationMapper _presentationMapper;
    private readonly Dictionary<string, FanPolicyDraftModel> _draftsById;
    private readonly Dictionary<string, FanCurvePolicyDescriptor> _savedPoliciesById;
    private PolicyRuntimePreviewModel _preview = PolicyRuntimePreviewModel.CreateEmpty();
    private string? _previewedPolicyId;

    public event PropertyChangedEventHandler? PropertyChanged;

    public FansViewModel(
        IFanPolicyRuntimeService policyRuntimeService,
        FanPolicyPresentationMapper presentationMapper,
        IDiagnosticsSink diagnosticsSink)
    {
        _diagnosticsSink = diagnosticsSink;
        _presentationMapper = presentationMapper;
        _inputSensorOptions = _presentationMapper.BuildInputSensorOptions();
        _outputControlOptions = _presentationMapper.BuildOutputControlOptions();
        var availablePolicies = policyRuntimeService.GetAvailableFanPolicies();
        _savedPoliciesById = BuildPolicyMap(availablePolicies);
        _draftsById = BuildDraftMap(availablePolicies);

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

    private readonly IFanPolicyRuntimeService _policyRuntimeService;

    public ObservableCollection<FanPolicyEditorModel> Policies { get; }

    public ObservableCollection<FanCurvePointRowModel> PreviewCurvePoints { get; }

    public PolicyRuntimePreviewModel Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public void UpdateDraft(string policyId, string inputSensorId, string outputControlId)
    {
        if (!TryUpdateDraft(policyId, inputSensorId, outputControlId, out var changed) || !changed)
        {
            return;
        }

        RefreshPolicyModel(policyId);
    }

    public void PreviewPolicy(string policyId, string inputSensorId, string outputControlId)
    {
        if (!TryUpdateDraft(policyId, inputSensorId, outputControlId, out var changed))
        {
            Preview = PolicyRuntimePreviewModel.CreateEmpty();
            PreviewCurvePoints.Clear();
            return;
        }

        if (changed)
        {
            RefreshPolicyModel(policyId);
        }

        var previewPolicy = BuildPolicyFromDraft(policyId);
        var preview = _policyRuntimeService.PreviewFanPolicy(previewPolicy);
        ReportPreviewDiagnostic(preview);

        _previewedPolicyId = policyId;
        RebuildPreviewCurvePoints(previewPolicy.Points);
        Preview = _presentationMapper.MapPreview(preview);
    }

    public void ResetPolicy(string policyId)
    {
        if (!_savedPoliciesById.TryGetValue(policyId, out var savedPolicy)
            || !_draftsById.TryGetValue(policyId, out var draft))
        {
            return;
        }

        draft.SelectedInputSensorId = savedPolicy.InputSensorId;
        draft.SelectedOutputControlId = savedPolicy.OutputControlId;
        draft.IsDirty = false;

        RefreshPolicyModel(policyId);

        if (string.Equals(_previewedPolicyId, policyId))
        {
            PreviewPolicy(policyId, draft.SelectedInputSensorId, draft.SelectedOutputControlId);
        }
    }

    public void ApplyMockPolicy(string policyId)
    {
        if (!_draftsById.TryGetValue(policyId, out var draft)
            || !_savedPoliciesById.ContainsKey(policyId))
        {
            return;
        }

        var appliedPolicy = BuildPolicyFromDraft(policyId);
        _savedPoliciesById[policyId] = appliedPolicy;
        draft.SelectedInputSensorId = appliedPolicy.InputSensorId;
        draft.SelectedOutputControlId = appliedPolicy.OutputControlId;
        draft.IsDirty = false;

        RefreshPolicyModel(policyId);
        _previewedPolicyId = policyId;
        RebuildPreviewCurvePoints(appliedPolicy.Points);
        var preview = _policyRuntimeService.PreviewFanPolicy(appliedPolicy);
        ReportPreviewDiagnostic(preview);
        Preview = _presentationMapper.MapPreview(preview);
    }

    private FanPolicyEditorModel CreateEditorModel(FanCurvePolicyDescriptor policy)
    {
        return _presentationMapper.MapPolicyEditor(
            policy,
            _draftsById[policy.Id],
            _inputSensorOptions,
            _outputControlOptions);
    }

    private void RefreshPolicyModel(string policyId)
    {
        if (!_savedPoliciesById.TryGetValue(policyId, out var policy))
        {
            return;
        }

        for (var index = 0; index < Policies.Count; index++)
        {
            if (string.Equals(Policies[index].PolicyId, policyId))
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

    private static Dictionary<string, FanPolicyDraftModel> BuildDraftMap(
        IReadOnlyList<FanCurvePolicyDescriptor> policies)
    {
        var map = new Dictionary<string, FanPolicyDraftModel>();

        foreach (var policy in policies)
        {
            map[policy.Id] = new FanPolicyDraftModel
            {
                PolicyId = policy.Id,
                SelectedInputSensorId = policy.InputSensorId,
                SelectedOutputControlId = policy.OutputControlId,
                IsDirty = false
            };
        }

        return map;
    }

    private FanCurvePolicyDescriptor BuildPolicyFromDraft(string policyId)
    {
        var savedPolicy = _savedPoliciesById[policyId];
        var draft = _draftsById[policyId];

        return savedPolicy with
        {
            InputSensorId = string.IsNullOrWhiteSpace(draft.SelectedInputSensorId)
                ? savedPolicy.InputSensorId
                : draft.SelectedInputSensorId,
            OutputControlId = string.IsNullOrWhiteSpace(draft.SelectedOutputControlId)
                ? savedPolicy.OutputControlId
                : draft.SelectedOutputControlId
        };
    }

    private bool TryUpdateDraft(
        string policyId,
        string inputSensorId,
        string outputControlId,
        out bool changed)
    {
        if (!_savedPoliciesById.TryGetValue(policyId, out var savedPolicy)
            || !_draftsById.TryGetValue(policyId, out var draft))
        {
            changed = false;
            return false;
        }

        var nextInputSensorId = string.IsNullOrWhiteSpace(inputSensorId)
            ? savedPolicy.InputSensorId
            : inputSensorId;

        var nextOutputControlId = string.IsNullOrWhiteSpace(outputControlId)
            ? savedPolicy.OutputControlId
            : outputControlId;

        var nextIsDirty =
            !string.Equals(nextInputSensorId, savedPolicy.InputSensorId)
            || !string.Equals(nextOutputControlId, savedPolicy.OutputControlId);

        changed =
            !string.Equals(draft.SelectedInputSensorId, nextInputSensorId)
            || !string.Equals(draft.SelectedOutputControlId, nextOutputControlId)
            || draft.IsDirty != nextIsDirty;

        if (!changed)
        {
            return true;
        }

        draft.SelectedInputSensorId = nextInputSensorId;
        draft.SelectedOutputControlId = nextOutputControlId;
        draft.IsDirty = nextIsDirty;

        return true;
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

    private void ReportPreviewDiagnostic(PolicyRuntimePreview preview)
    {
        var severity = preview.Success
            ? DiagnosticSeverity.Info
            : DiagnosticSeverity.Warning;

        var message = preview.Message;
        if (preview.BlockedReasons.Count > 0)
        {
            message = $"{message} Blocked: {string.Join(" · ", preview.BlockedReasons)}";
        }

        _diagnosticsSink.Report(new DiagnosticRecord(
            severity,
            DiagnosticSource.Fans,
            $"fans.preview.{preview.FailureCode.ToString().ToLowerInvariant()}",
            message,
            preview.PolicyId,
            DateTimeOffset.UtcNow));
    }
}
