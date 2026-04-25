using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class FanPolicyPresentationMapper
{
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
                && (sensor.Id == "sensor.cpu.temperature"
                    || sensor.Id == "sensor.gpu.temperature"
                    || sensor.Id == "sensor.thermal.max_cpu_gpu"))
            .OrderBy(GetSensorSortKey)
            .Select(sensor => new SelectionOptionModel
            {
                Id = sensor.Id,
                DisplayName = GetSensorDisplayName(sensor)
            })
            .ToArray();
    }

    public IReadOnlyList<SelectionOptionModel> BuildOutputControlOptions()
    {
        return _controls.Values
            .Where(control => control.Id == "control.fan.cpu_pwm"
                || control.Id == "control.fan.gpu_pwm"
                || control.Id == "control.fan.curve")
            .OrderBy(GetControlSortKey)
            .Select(control => new SelectionOptionModel
            {
                Id = control.Id,
                DisplayName = control.DisplayName
            })
            .ToArray();
    }

    public FanPolicyEditorModel MapPolicyEditor(
        FanCurvePolicyDescriptor policy,
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
            SelectedInputSensorId = policy.InputSensorId,
            SelectedOutputControlId = policy.OutputControlId,
            InputSensorOptions = inputSensorOptions,
            OutputControlOptions = outputControlOptions,
            CurvePoints = policy.Points.Select(MapCurvePoint).ToArray(),
            HysteresisText = $"{policy.HysteresisDegrees:0.#}°C",
            RampUpText = $"{policy.RampUpSeconds:0.#}s",
            RampDownText = $"{policy.RampDownSeconds:0.#}s"
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
            InputSensorText = $"Input Sensor: {GetSensorName(preview.Policy.InputSensorId)}",
            OutputControlText = $"Output Control: {GetControlName(preview.Policy.OutputControlId)}",
            WouldSetSummary = preview.WouldSetSummary,
            TimingSummary = preview.TimingSummary,
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
            ? GetSensorDisplayName(sensor)
            : sensorId;
    }

    private string GetControlName(string controlId)
    {
        return _controls.TryGetValue(controlId, out var control)
            ? control.DisplayName
            : controlId;
    }

    private static string GetSensorDisplayName(SensorDescriptor sensor)
    {
        return sensor.Id switch
        {
            "sensor.cpu.temperature" => "CPU Package Temp",
            "sensor.gpu.temperature" => "GPU Temp",
            "sensor.thermal.max_cpu_gpu" => "Max(CPU, GPU)",
            _ => sensor.DisplayName
        };
    }

    private static int GetSensorSortKey(SensorDescriptor sensor)
    {
        return sensor.Id switch
        {
            "sensor.cpu.temperature" => 0,
            "sensor.gpu.temperature" => 1,
            "sensor.thermal.max_cpu_gpu" => 2,
            _ => 100
        };
    }

    private static int GetControlSortKey(ControlDescriptor control)
    {
        return control.Id switch
        {
            "control.fan.cpu_pwm" => 0,
            "control.fan.gpu_pwm" => 1,
            "control.fan.curve" => 2,
            _ => 100
        };
    }
}
