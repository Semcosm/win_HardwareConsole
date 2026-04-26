using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class ThermalViewModel : INotifyPropertyChanged
{
    private readonly IDiagnosticsSink _diagnosticsSink;
    private readonly ThermalPolicyPresentationMapper _presentationMapper;
    private readonly Dictionary<string, ThermalPolicyDescriptor> _policiesById;
    private readonly IThermalPolicyRuntimeService _policyRuntimeService;
    private ThermalPolicyPreviewModel _preview = ThermalPolicyPreviewModel.CreateEmpty();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ThermalViewModel(
        IThermalPolicyRuntimeService policyRuntimeService,
        ThermalPolicyPresentationMapper presentationMapper,
        IDiagnosticsSink diagnosticsSink)
    {
        _diagnosticsSink = diagnosticsSink;
        _policyRuntimeService = policyRuntimeService;
        _presentationMapper = presentationMapper;
        _policiesById = BuildPolicyMap(_policyRuntimeService.GetAvailableThermalPolicies());

        Policies = new ObservableCollection<ThermalPolicyCardModel>();
        PreviewActions = new ObservableCollection<ThermalActionRowModel>();

        foreach (var policy in _policiesById.Values)
        {
            Policies.Add(_presentationMapper.MapPolicyCard(policy));
        }

        if (Policies.Count > 0)
        {
            PreviewPolicy(Policies[0].Id);
        }
    }

    public ObservableCollection<ThermalPolicyCardModel> Policies { get; }

    public ObservableCollection<ThermalActionRowModel> PreviewActions { get; }

    public ThermalPolicyPreviewModel Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public void PreviewPolicy(string policyId)
    {
        if (!_policiesById.TryGetValue(policyId, out var policy))
        {
            Preview = ThermalPolicyPreviewModel.CreateEmpty();
            PreviewActions.Clear();
            return;
        }

        var preview = _policyRuntimeService.PreviewThermalPolicy(policy);
        ReportPreviewDiagnostic(preview);

        PreviewActions.Clear();

        foreach (var action in policy.Actions)
        {
            PreviewActions.Add(_presentationMapper.MapAction(action));
        }

        Preview = _presentationMapper.MapPreview(preview);
    }

    private void ReportPreviewDiagnostic(ThermalPolicyPreview preview)
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
            DiagnosticSource.Thermal,
            $"thermal.preview.{preview.FailureCode.ToString().ToLowerInvariant()}",
            message,
            preview.PolicyId,
            DateTimeOffset.UtcNow));
    }

    private static Dictionary<string, ThermalPolicyDescriptor> BuildPolicyMap(
        IReadOnlyList<ThermalPolicyDescriptor> policies)
    {
        var map = new Dictionary<string, ThermalPolicyDescriptor>();

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
