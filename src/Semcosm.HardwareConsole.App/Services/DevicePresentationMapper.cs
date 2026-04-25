using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class DevicePresentationMapper
{
    public DeviceCardModel MapDeviceCard(
        DeviceDescriptor device,
        IReadOnlyList<HardwareCapability> capabilities,
        IReadOnlyList<PluginDescriptor> plugins,
        IReadOnlyList<SensorRowModel> sensors,
        IReadOnlyList<ControlRowModel> controls)
    {
        var capabilityNames = capabilities
            .Select(capability => capability.DisplayName)
            .ToArray();

        var pluginNames = GetPluginSources(device, plugins);

        return new DeviceCardModel
        {
            Id = device.Id,
            DisplayName = device.DisplayName,
            Vendor = device.Vendor,
            Model = device.Model,
            CapabilityCountText = FormatCount(capabilities.Count, "capability"),
            SensorCountText = FormatCount(sensors.Count, "sensor"),
            ControlCountText = FormatCount(controls.Count, "control"),
            PluginSourceSummary = pluginNames.Count == 0
                ? "No plugin source mapped yet"
                : string.Join(" · ", pluginNames),
            Capabilities = capabilityNames,
            PluginSources = pluginNames,
            Sensors = sensors,
            Controls = controls
        };
    }

    public SensorRowModel MapSensorRow(SensorDescriptor sensor, SensorValue? currentValue)
    {
        return new SensorRowModel
        {
            Id = sensor.Id,
            DisplayName = sensor.DisplayName,
            Subtitle = $"{GetSensorKindText(sensor.Kind)} · {sensor.Id}",
            CurrentValue = currentValue?.FormattedValue ?? "No snapshot value",
            UnitText = string.IsNullOrWhiteSpace(sensor.Unit) ? "No unit" : sensor.Unit,
            QualityText = currentValue?.Quality.ToString() ?? "Unknown"
        };
    }

    public ControlRowModel MapControlRow(
        ControlDescriptor control,
        string deviceDisplayName,
        ProfileDescriptor? activeProfile,
        ProfileControlActionDescriptor? activeAction)
    {
        return new ControlRowModel
        {
            Id = control.Id,
            DisplayName = control.DisplayName,
            Subtitle = $"{deviceDisplayName} · {GetControlKindText(control.Kind)} · {control.Id}",
            TargetValueText = activeAction?.TargetValue.FormattedValue ?? "No active profile target",
            UnitText = ResolveControlUnit(control, activeAction),
            SourceText = activeProfile is null
                ? "Mock runtime target unavailable because no profile is active."
                : $"Mock runtime target from active profile: {activeProfile.DisplayName}",
            RiskLevel = MapRiskLevel(control.RiskLevel)
        };
    }

    private static IReadOnlyList<string> GetPluginSources(
        DeviceDescriptor device,
        IReadOnlyList<PluginDescriptor> plugins)
    {
        var matches = new List<string>();

        foreach (var plugin in plugins)
        {
            if (OwnsDevice(device, plugin))
            {
                matches.Add(plugin.DisplayName);
            }
        }

        return matches;
    }

    private static bool OwnsDevice(DeviceDescriptor device, PluginDescriptor plugin)
    {
        return plugin.ProvidedDeviceIds.Contains(device.Id);
    }

    private static string ResolveControlUnit(
        ControlDescriptor control,
        ProfileControlActionDescriptor? activeAction)
    {
        if (activeAction is not null && !string.IsNullOrWhiteSpace(activeAction.TargetValue.Unit))
        {
            return activeAction.TargetValue.Unit;
        }

        return control.Kind switch
        {
            ControlKind.Toggle => "toggle",
            ControlKind.Mode => "mode",
            ControlKind.Curve => "curve",
            ControlKind.Fan => "%",
            ControlKind.FanCurve => "curve",
            ControlKind.Range => "profile-defined",
            _ => "unknown"
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

    private static string GetSensorKindText(SensorKind sensorKind)
    {
        return sensorKind switch
        {
            SensorKind.Temperature => "Temperature",
            SensorKind.Power => "Power",
            SensorKind.Clock => "Clock",
            SensorKind.FanSpeed => "Fan Speed",
            SensorKind.State => "State",
            SensorKind.Text => "Text",
            _ => "Unknown"
        };
    }

    private static string GetControlKindText(ControlKind controlKind)
    {
        return controlKind switch
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

    private static string FormatCount(int count, string singular)
    {
        return count == 1 ? $"1 {singular}" : $"{count} {singular}s";
    }
}
