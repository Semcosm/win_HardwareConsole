using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class SchedulerPolicyPresentationMapper
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public SchedulerPolicyPresentationMapper(IHardwareInventoryService hardwareInventoryService)
    {
        _sensors = hardwareInventoryService
            .GetSensors()
            .ToDictionary(sensor => sensor.Id);

        _controls = hardwareInventoryService
            .GetControls()
            .ToDictionary(control => control.Id);
    }

    public SchedulerPolicyCardModel MapPolicyCard(SchedulerPolicyDescriptor policy)
    {
        var controlNames = policy.Rules
            .SelectMany(rule => rule.Actions)
            .Select(action => GetControlName(action.ControlId))
            .Distinct()
            .ToArray();

        return new SchedulerPolicyCardModel
        {
            Id = policy.Id,
            DisplayName = policy.DisplayName,
            Description = policy.Description,
            RiskLevel = policy.RiskLevel,
            ScopeText = $"{policy.Scope} scheduler policy",
            RuleCountText = FormatCount(policy.Rules.Count, "rule"),
            TriggerSummary = policy.InputSensorIds.Count == 0
                ? "Triggers: none"
                : $"Triggers: {string.Join(" · ", policy.InputSensorIds.Select(GetSensorName))}",
            ForegroundStrategyText = $"Foreground: {policy.ForegroundStrategy}",
            BackgroundStrategyText = $"Background: {policy.BackgroundStrategy}",
            ControlSummary = controlNames.Length == 0
                ? "Targets: none"
                : $"Targets: {string.Join(" · ", controlNames)}"
        };
    }

    public SchedulerRuleRowModel MapRule(SchedulerRuleDescriptor rule)
    {
        return new SchedulerRuleRowModel
        {
            DisplayName = rule.DisplayName,
            MatchText = $"Match: {rule.MatchText}",
            Description = rule.Description,
            ActionSummary = rule.Actions.Count == 0
                ? "No scheduler actions"
                : string.Join(" · ", rule.Actions.Select(action =>
                    $"{GetControlName(action.ControlId)} = {action.TargetValue.FormattedValue}")),
            RiskLevel = GetRuleRiskLevel(rule)
        };
    }

    public SchedulerPolicyPreviewModel MapPreview(SchedulerPolicyPreview preview)
    {
        if (preview.Policy is null)
        {
            return SchedulerPolicyPreviewModel.CreateEmpty();
        }

        return new SchedulerPolicyPreviewModel
        {
            Title = preview.Policy.DisplayName,
            Description = preview.Policy.Description,
            ScopeText = $"{preview.Policy.Scope} scheduler policy",
            StatusText = preview.Success ? "Preview Ready" : "Preview Blocked",
            FailureCodeText = GetFailureCodeText(preview.FailureCode),
            RequiredSensorsSummary = preview.RequiredSensorIds.Count == 0
                ? "Required Sensors: none"
                : $"Required Sensors: {string.Join(" · ", preview.RequiredSensorIds.Select(GetSensorName))}",
            WouldSetControlsSummary = preview.WouldSetControlIds.Count == 0
                ? "Would Set Controls: none"
                : $"Would Set Controls: {string.Join(" · ", preview.WouldSetControlIds.Select(GetControlName))}",
            ForegroundStrategyText = $"Foreground Strategy: {preview.Policy.ForegroundStrategy}",
            BackgroundStrategyText = $"Background Strategy: {preview.Policy.BackgroundStrategy}",
            BlockedReasonsSummary = preview.BlockedReasons.Count == 0
                ? "Blocked Reasons: none"
                : $"Blocked Reasons: {string.Join(" · ", preview.BlockedReasons)}",
            DiagnosticsSummary = preview.Diagnostics.Count == 0
                ? "Diagnostics: none"
                : $"Diagnostics: {string.Join(" · ", preview.Diagnostics)}",
            Message = preview.Message,
            RiskLevel = preview.Policy.RiskLevel
        };
    }

    private string GetSensorName(string sensorId)
    {
        return _sensors.TryGetValue(sensorId, out var sensor)
            ? sensor.DisplayName
            : sensorId;
    }

    private string GetControlName(string controlId)
    {
        return _controls.TryGetValue(controlId, out var control)
            ? control.DisplayName
            : controlId;
    }

    private static HardwareRiskLevel GetRuleRiskLevel(SchedulerRuleDescriptor rule)
    {
        if (rule.Actions.Any(action => action.RiskLevel == ControlRiskLevel.KernelDriverRequired))
        {
            return HardwareRiskLevel.KernelDriverRequired;
        }

        if (rule.Actions.Any(action => action.RiskLevel == ControlRiskLevel.HardwareWrite))
        {
            return HardwareRiskLevel.HardwareWrite;
        }

        if (rule.Actions.Any(action => action.RiskLevel == ControlRiskLevel.Experimental))
        {
            return HardwareRiskLevel.Experimental;
        }

        return rule.Actions.Any(action => action.RiskLevel == ControlRiskLevel.SafeControl)
            ? HardwareRiskLevel.SafeControl
            : HardwareRiskLevel.ReadOnly;
    }

    private static string GetFailureCodeText(PolicyPreviewFailureCode failureCode)
    {
        return failureCode switch
        {
            PolicyPreviewFailureCode.None => "No failure",
            PolicyPreviewFailureCode.InvalidPolicy => "Invalid Policy",
            PolicyPreviewFailureCode.MissingRequiredSensor => "Missing Required Sensor",
            PolicyPreviewFailureCode.UnsupportedControl => "Unsupported Control",
            PolicyPreviewFailureCode.RuntimeError => "Runtime Error",
            _ => "Unknown"
        };
    }

    private static string FormatCount(int count, string singular)
    {
        return count == 1 ? $"1 {singular}" : $"{count} {singular}s";
    }
}
