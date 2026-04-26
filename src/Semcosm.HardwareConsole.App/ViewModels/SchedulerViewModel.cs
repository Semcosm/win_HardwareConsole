using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class SchedulerViewModel : INotifyPropertyChanged
{
    private readonly IDiagnosticsSink _diagnosticsSink;
    private readonly IReadOnlyDictionary<string, SchedulerPolicyDescriptor> _policiesById;
    private readonly SchedulerPolicyPresentationMapper _presentationMapper;
    private readonly ISchedulerPolicyRuntimeService _policyRuntimeService;
    private SchedulerPolicyPreviewModel _preview = SchedulerPolicyPreviewModel.CreateEmpty();

    public event PropertyChangedEventHandler? PropertyChanged;

    public SchedulerViewModel(
        ISchedulerPolicyRuntimeService policyRuntimeService,
        SchedulerPolicyPresentationMapper presentationMapper,
        IDiagnosticsSink diagnosticsSink)
    {
        _diagnosticsSink = diagnosticsSink;
        _policyRuntimeService = policyRuntimeService;
        _presentationMapper = presentationMapper;
        _policiesById = BuildPolicyMap(_policyRuntimeService.GetAvailableSchedulerPolicies());

        Policies = new ObservableCollection<SchedulerPolicyCardModel>();
        PreviewRules = new ObservableCollection<SchedulerRuleRowModel>();

        foreach (var policy in _policiesById.Values)
        {
            Policies.Add(_presentationMapper.MapPolicyCard(policy));
        }

        if (Policies.Count > 0)
        {
            PreviewPolicy(Policies[0].Id);
        }
    }

    public ObservableCollection<SchedulerPolicyCardModel> Policies { get; }

    public ObservableCollection<SchedulerRuleRowModel> PreviewRules { get; }

    public SchedulerPolicyPreviewModel Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public void PreviewPolicy(string policyId)
    {
        if (!_policiesById.TryGetValue(policyId, out var policy))
        {
            Preview = SchedulerPolicyPreviewModel.CreateEmpty();
            PreviewRules.Clear();
            return;
        }

        var preview = _policyRuntimeService.PreviewSchedulerPolicy(policy);
        ReportPreviewDiagnostic(preview);

        PreviewRules.Clear();
        foreach (var rule in policy.Rules)
        {
            PreviewRules.Add(_presentationMapper.MapRule(rule));
        }

        Preview = _presentationMapper.MapPreview(preview);
    }

    private void ReportPreviewDiagnostic(SchedulerPolicyPreview preview)
    {
        var message = preview.Message;
        if (preview.BlockedReasons.Count > 0)
        {
            message = $"{message} Blocked: {string.Join(" · ", preview.BlockedReasons)}";
        }

        _diagnosticsSink.Report(new DiagnosticRecord(
            GetSeverity(preview.FailureCode),
            DiagnosticSource.Scheduler,
            $"scheduler.preview.{preview.FailureCode.ToString().ToLowerInvariant()}",
            message,
            preview.PolicyId,
            DateTimeOffset.UtcNow));
    }

    private static IReadOnlyDictionary<string, SchedulerPolicyDescriptor> BuildPolicyMap(
        IReadOnlyList<SchedulerPolicyDescriptor> policies)
    {
        var map = new Dictionary<string, SchedulerPolicyDescriptor>();

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
