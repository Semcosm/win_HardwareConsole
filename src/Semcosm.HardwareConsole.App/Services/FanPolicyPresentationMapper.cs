using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class FanPolicyPresentationMapper
{
    private const string FanPolicyInputTag = "fan.policy.input";
    private const string FanPolicyOutputTag = "fan.policy.output";

    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, PolicyDescriptor> _policies;
    private readonly IReadOnlyDictionary<string, SensorDescriptor> _sensors;

    public FanPolicyPresentationMapper(IHardwareInventoryService hardwareInventoryService)
    {
        _sensors = hardwareInventoryService
            .GetSensors()
            .ToDictionary(sensor => sensor.Id);

        _controls = hardwareInventoryService
            .GetControls()
            .ToDictionary(control => control.Id);

        _policies = hardwareInventoryService
            .GetPolicies()
            .ToDictionary(policy => policy.Id);
    }

    public IReadOnlyList<SelectionOptionModel> BuildInputSensorOptions()
    {
        return _sensors.Values
            .Where(sensor => sensor.Kind == SensorKind.Temperature
                && sensor.CapabilityTags.Contains(FanPolicyInputTag))
            .OrderBy(sensor => sensor.DisplayName)
            .Select(sensor => new SelectionOptionModel
            {
                Id = sensor.Id,
                DisplayName = sensor.DisplayName
            })
            .ToArray();
    }

    public IReadOnlyList<SelectionOptionModel> BuildOutputControlOptions()
    {
        return _controls.Values
            .Where(control =>
                (control.Kind == ControlKind.Fan || control.Kind == ControlKind.FanCurve)
                && control.CapabilityTags.Contains(FanPolicyOutputTag))
            .OrderBy(control => control.Kind == ControlKind.FanCurve ? 1 : 0)
            .ThenBy(control => control.DisplayName)
            .Select(control => new SelectionOptionModel
            {
                Id = control.Id,
                DisplayName = control.DisplayName
            })
            .ToArray();
    }

    public FanPolicyEditorModel MapPolicyEditor(
        FanCurvePolicyDescriptor policy,
        FanPolicyDraftModel draft,
        IReadOnlyList<SelectionOptionModel> inputSensorOptions,
        IReadOnlyList<SelectionOptionModel> outputControlOptions)
    {
        var policyText = _policies.TryGetValue(policy.PolicyId, out var policyDescriptor)
            ? policyDescriptor.DisplayName
            : policy.PolicyId;

        return new FanPolicyEditorModel
        {
            PolicyId = policy.Id,
            DisplayName = policy.DisplayName,
            Description = policy.Description,
            ScopeText = $"{policy.Scope} fan policy",
            PolicyText = $"Rule: {policyText}",
            RiskLevel = policy.RiskLevel,
            SelectedInputSensorId = draft.SelectedInputSensorId,
            SelectedOutputControlId = draft.SelectedOutputControlId,
            InputSensorOptions = inputSensorOptions,
            OutputControlOptions = outputControlOptions,
            CurvePoints = policy.Points.Select(MapCurvePoint).ToArray(),
            HysteresisText = $"{policy.HysteresisDegrees:0.#}°C",
            RampUpText = $"{policy.RampUpSeconds:0.#}s",
            RampDownText = $"{policy.RampDownSeconds:0.#}s",
            IsDirty = draft.IsDirty,
            DraftStateText = draft.IsDirty
                ? "Draft changes pending"
                : "Saved mock policy",
            CanReset = draft.IsDirty,
            CanApplyMockPolicy = draft.IsDirty
        };
    }

    public PolicyRuntimePreviewModel MapPreview(PolicyRuntimePreview preview)
    {
        if (preview.Policy is null)
        {
            return PolicyRuntimePreviewModel.CreateEmpty();
        }

        return new PolicyRuntimePreviewModel
        {
            Title = preview.Policy.DisplayName,
            Description = preview.Policy.Description,
            ScopeText = $"{preview.Policy.Scope} fan policy",
            StatusText = preview.Success ? "Preview Ready" : "Preview Blocked",
            FailureCodeText = GetFailureCodeText(preview.FailureCode),
            InputSensorText = $"Input Sensor: {GetSensorName(preview.Policy.InputSensorId)}",
            OutputControlText = $"Output Control: {GetControlName(preview.Policy.OutputControlId)}",
            WouldSetSummary = preview.WouldSetControlIds.Count == 0
                ? "Would Set Controls: none"
                : $"Would Set Controls: {string.Join(" · ", preview.WouldSetControlIds.Select(GetControlName))}",
            RequiredSensorsSummary = preview.RequiredSensorIds.Count == 0
                ? "Required Sensors: none"
                : $"Required Sensors: {string.Join(" · ", preview.RequiredSensorIds.Select(GetSensorName))}",
            BlockedReasonsSummary = preview.BlockedReasons.Count == 0
                ? "Blocked Reasons: none"
                : $"Blocked Reasons: {string.Join(" · ", preview.BlockedReasons)}",
            DiagnosticsSummary = preview.Diagnostics.Count == 0
                ? "Diagnostics: none"
                : $"Diagnostics: {string.Join(" · ", preview.Diagnostics)}",
            TimingSummary = $"Timing: Hysteresis {preview.Policy.HysteresisDegrees:0.#}°C · Ramp-up {preview.Policy.RampUpSeconds:0.#}s · Ramp-down {preview.Policy.RampDownSeconds:0.#}s",
            Message = preview.Message,
            RiskLevel = preview.Policy.RiskLevel
        };
    }

    public FanCurvePointRowModel MapCurvePoint(FanCurvePoint point)
    {
        return new FanCurvePointRowModel
        {
            InputText = $"{point.InputValue:0.#}°C",
            OutputText = $"{point.OutputPercent:0.#}%"
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
}
