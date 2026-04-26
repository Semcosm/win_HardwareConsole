using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class PowerViewModel : INotifyPropertyChanged
{
    private readonly IDiagnosticsSink _diagnosticsSink;
    private readonly IReadOnlyDictionary<string, PowerPolicyDescriptor> _policiesById;
    private readonly PowerPolicyPresentationMapper _presentationMapper;
    private readonly IPowerPolicyRuntimeService _policyRuntimeService;
    private PowerPolicyPreviewModel _preview = PowerPolicyPreviewModel.CreateEmpty();

    public event PropertyChangedEventHandler? PropertyChanged;

    public PowerViewModel(
        IPowerPolicyRuntimeService policyRuntimeService,
        PowerPolicyPresentationMapper presentationMapper,
        IDiagnosticsSink diagnosticsSink)
    {
        _diagnosticsSink = diagnosticsSink;
        _policyRuntimeService = policyRuntimeService;
        _presentationMapper = presentationMapper;
        _policiesById = BuildPolicyMap(_policyRuntimeService.GetAvailablePowerPolicies());

        Policies = new ObservableCollection<PowerPolicyCardModel>();
        PreviewActions = new ObservableCollection<ProfileActionRowModel>();

        foreach (var policy in _policiesById.Values)
        {
            Policies.Add(_presentationMapper.MapPolicyCard(policy));
        }

        if (Policies.Count > 0)
        {
            PreviewPolicy(Policies[0].Id);
        }
    }

    public ObservableCollection<PowerPolicyCardModel> Policies { get; }

    public ObservableCollection<ProfileActionRowModel> PreviewActions { get; }

    public PowerPolicyPreviewModel Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public void PreviewPolicy(string policyId)
    {
        if (!_policiesById.TryGetValue(policyId, out var policy))
        {
            Preview = PowerPolicyPreviewModel.CreateEmpty();
            PreviewActions.Clear();
            return;
        }

        var preview = _policyRuntimeService.PreviewPowerPolicy(policy);
        ReportPreviewDiagnostic(preview);

        PreviewActions.Clear();
        foreach (var action in policy.Actions)
        {
            PreviewActions.Add(_presentationMapper.MapAction(action));
        }

        Preview = _presentationMapper.MapPreview(preview);
    }

    private void ReportPreviewDiagnostic(PowerPolicyPreview preview)
    {
        var message = preview.Message;
        if (preview.BlockedReasons.Count > 0)
        {
            message = $"{message} Blocked: {string.Join(" · ", preview.BlockedReasons)}";
        }

        _diagnosticsSink.Report(new DiagnosticRecord(
            GetSeverity(preview.FailureCode),
            DiagnosticSource.Power,
            $"power.preview.{preview.FailureCode.ToString().ToLowerInvariant()}",
            message,
            preview.PolicyId,
            DateTimeOffset.UtcNow));
    }

    private static IReadOnlyDictionary<string, PowerPolicyDescriptor> BuildPolicyMap(
        IReadOnlyList<PowerPolicyDescriptor> policies)
    {
        var map = new Dictionary<string, PowerPolicyDescriptor>();

        foreach (var policy in policies)
        {
            map[policy.Id] = policy;
        }

        return map;
    }

    private static DiagnosticSeverity GetSeverity(PolicyPreviewFailureCode failureCode)
    {
        return failureCode switch
        {
            PolicyPreviewFailureCode.None => DiagnosticSeverity.Info,
            PolicyPreviewFailureCode.RuntimeError => DiagnosticSeverity.Error,
            PolicyPreviewFailureCode.UnsupportedControl => DiagnosticSeverity.Error,
            _ => DiagnosticSeverity.Warning
        };
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
