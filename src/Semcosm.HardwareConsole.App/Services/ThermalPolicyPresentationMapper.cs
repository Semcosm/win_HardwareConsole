using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class ThermalPolicyPresentationMapper
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public ThermalPolicyPresentationMapper(IHardwareInventoryService hardwareInventoryService)
    {
        _sensors = hardwareInventoryService
            .GetSensors()
            .ToDictionary(sensor => sensor.Id);

        _controls = hardwareInventoryService
            .GetControls()
            .ToDictionary(control => control.Id);
    }

    public ThermalPolicyCardModel MapPolicyCard(ThermalPolicyDescriptor policy)
    {
        var controlNames = policy.Actions
            .Select(action => GetControlName(action.ControlId))
            .Distinct()
            .ToArray();

        return new ThermalPolicyCardModel
        {
            Id = policy.Id,
            DisplayName = policy.DisplayName,
            Description = policy.Description,
            RiskLevel = policy.RiskLevel,
            ScopeText = $"{policy.Scope} thermal chain",
            SensorSummary = policy.InputSensorIds.Count == 0
                ? "Triggers: none"
                : $"Triggers: {string.Join(" · ", policy.InputSensorIds.Select(GetSensorName))}",
            ActionCountText = FormatCount(policy.Actions.Count, "stage"),
            TimingText = $"Poll {policy.PollIntervalSeconds:0.#}s · Cooldown {policy.CooldownSeconds:0.#}s",
            ControlSummary = controlNames.Length == 0
                ? "Targets: none"
                : $"Targets: {string.Join(" · ", controlNames)}"
        };
    }

    public ThermalActionRowModel MapAction(ThermalThresholdActionDescriptor action)
    {
        return new ThermalActionRowModel
        {
            StageLabel = action.StageLabel,
            TriggerText = $"{GetSensorName(action.TriggerSensorId)} ≥ {action.TriggerThreshold:0.#}°C",
            DisplayName = GetControlName(action.ControlId),
            Subtitle = $"{GetControlKindText(action.ControlId)} · {action.ControlId}",
            TargetValue = action.TargetValue.FormattedValue,
            RiskLevel = MapRiskLevel(action.RiskLevel),
            RequiresConfirmation = action.RequiresConfirmation,
            ConfirmationText = action.RequiresConfirmation
                ? "Confirmation Required"
                : string.Empty
        };
    }

    public ThermalPolicyPreviewModel MapPreview(ThermalPolicyPreview preview)
    {
        if (preview.Policy is null)
        {
            return ThermalPolicyPreviewModel.CreateEmpty();
        }

        return new ThermalPolicyPreviewModel
        {
            Title = preview.Policy.DisplayName,
            Description = preview.Policy.Description,
            ScopeText = $"{preview.Policy.Scope} thermal chain",
            StatusText = preview.Success ? "Preview Ready" : "Preview Blocked",
            FailureCodeText = GetFailureCodeText(preview.FailureCode),
            InputSensorsSummary = preview.RequiredSensorIds.Count == 0
                ? "Required Sensors: none"
                : $"Required Sensors: {string.Join(" · ", preview.RequiredSensorIds.Select(GetSensorName))}",
            WouldSetControlsSummary = preview.WouldSetControlIds.Count == 0
                ? "Would Set Controls: none"
                : $"Would Set Controls: {string.Join(" · ", preview.WouldSetControlIds.Select(GetControlName))}",
            TimingSummary = $"Timing: Poll {preview.Policy.PollIntervalSeconds:0.#}s · Cooldown {preview.Policy.CooldownSeconds:0.#}s",
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

    private string GetControlKindText(string controlId)
    {
        if (!_controls.TryGetValue(controlId, out var control))
        {
            return "Unknown";
        }

        return control.Kind switch
        {
            ControlKind.Toggle => "Toggle",
            ControlKind.Mode => "Mode",
            ControlKind.Range => "Range",
            ControlKind.Curve => "Curve",
            ControlKind.Fan => "Fan",
            ControlKind.FanCurve => "Fan Curve",
            _ => "Unknown"
        };
    }

    private static HardwareRiskLevel MapRiskLevel(ControlRiskLevel riskLevel)
    {
        return riskLevel switch
        {
            ControlRiskLevel.SafeControl => HardwareRiskLevel.SafeControl,
            ControlRiskLevel.HardwareWrite => HardwareRiskLevel.HardwareWrite,
            ControlRiskLevel.KernelDriverRequired => HardwareRiskLevel.KernelDriverRequired,
            ControlRiskLevel.Experimental => HardwareRiskLevel.Experimental,
            _ => HardwareRiskLevel.ReadOnly
        };
    }

    private static string GetFailureCodeText(ThermalPolicyFailureCode failureCode)
    {
        return failureCode switch
        {
            ThermalPolicyFailureCode.None => "No failure",
            ThermalPolicyFailureCode.InvalidPolicy => "Invalid Policy",
            ThermalPolicyFailureCode.MissingRequiredSensor => "Missing Required Sensor",
            ThermalPolicyFailureCode.UnsupportedControl => "Unsupported Control",
            ThermalPolicyFailureCode.RuntimeError => "Runtime Error",
            _ => "Unknown"
        };
    }

    private static string FormatCount(int count, string singular)
    {
        return count == 1 ? $"1 {singular}" : $"{count} {singular}s";
    }
}
