using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

internal static class MockHardwareData
{
    public static IReadOnlyList<HardwareCapability> Capabilities { get; } = new List<HardwareCapability>
    {
        new("cap.cpu.telemetry", "Sensor", "CPU Telemetry", "CPU temperature, power and frequency telemetry."),
        new("cap.gpu.telemetry", "Sensor", "GPU Telemetry", "GPU temperature, power and clock telemetry."),
        new("cap.fan.control", "Control", "Fan Control", "Fan RPM monitoring and curve control surfaces."),
        new("cap.power.policy", "Policy", "Power Policy", "Windows power plan and AC/DC behavior integration."),
        new("cap.scheduler.policy", "Policy", "Scheduler Policy", "Process and scheduler policy hints."),
        new("cap.thermal.policy", "Policy", "Thermal Policy", "Thermal targets and protection strategy hooks.")
    };

    public static IReadOnlyList<DeviceDescriptor> Devices { get; } = new List<DeviceDescriptor>
    {
        new(
            "device.cpu.intel-13900hx",
            "Intel Core i9-13900HX",
            "Intel",
            "Core i9-13900HX",
            new[] { "cap.cpu.telemetry", "cap.power.policy" }),
        new(
            "device.gpu.rtx4060-laptop",
            "NVIDIA GeForce RTX 4060 Laptop",
            "NVIDIA",
            "GeForce RTX 4060 Laptop",
            new[] { "cap.gpu.telemetry", "cap.thermal.policy" }),
        new(
            "device.platform.mock-host",
            "Semcosm Reference Mock Host",
            "Semcosm",
            "Reference Mock Host",
            new[] { "cap.fan.control", "cap.power.policy", "cap.scheduler.policy", "cap.thermal.policy" })
    };

    public static IReadOnlyList<SensorDescriptor> Sensors { get; } = new List<SensorDescriptor>
    {
        new("sensor.cpu.temperature", "CPU Temperature", "device.cpu.intel-13900hx", SensorKind.Temperature, "C"),
        new("sensor.cpu.package_power", "CPU Package Power", "device.cpu.intel-13900hx", SensorKind.Power, "W"),
        new("sensor.cpu.clock", "CPU Clock", "device.cpu.intel-13900hx", SensorKind.Clock, "GHz"),
        new("sensor.cpu.power_limits", "CPU Power Limits", "device.cpu.intel-13900hx", SensorKind.Text, string.Empty),
        new("sensor.cpu.status", "CPU Status", "device.cpu.intel-13900hx", SensorKind.State, string.Empty),
        new("sensor.gpu.temperature", "GPU Temperature", "device.gpu.rtx4060-laptop", SensorKind.Temperature, "C"),
        new("sensor.gpu.board_power", "GPU Board Power", "device.gpu.rtx4060-laptop", SensorKind.Power, "W"),
        new("sensor.gpu.clock", "GPU Clock", "device.gpu.rtx4060-laptop", SensorKind.Clock, "MHz"),
        new("sensor.gpu.power_limit", "GPU Power Limit", "device.gpu.rtx4060-laptop", SensorKind.Text, string.Empty),
        new("sensor.gpu.status", "GPU Status", "device.gpu.rtx4060-laptop", SensorKind.State, string.Empty),
        new("sensor.profile.active", "Active Profile", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.profile.summary", "Profile Summary", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.profile.details", "Profile Details", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.profile.state", "Profile State", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.fan.mode", "Fan Mode", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.fan.state", "Fan State", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.fan.cpu_rpm", "CPU Fan RPM", "device.platform.mock-host", SensorKind.FanSpeed, "RPM"),
        new("sensor.fan.gpu_rpm", "GPU Fan RPM", "device.platform.mock-host", SensorKind.FanSpeed, "RPM"),
        new("sensor.fan.response", "Fan Response", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.power.state", "Power State", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.power.summary", "Power Summary", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.power.details", "Power Details", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.thermal.policy", "Thermal Policy", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.thermal.summary", "Thermal Summary", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.thermal.state", "Thermal State", "device.platform.mock-host", SensorKind.State, string.Empty)
    };

    public static IReadOnlyList<ControlDescriptor> Controls { get; } = new List<ControlDescriptor>
    {
        new("control.cpu.power_limits", "CPU Power Limits", "device.cpu.intel-13900hx", ControlKind.Range, ControlRiskLevel.SafeControl),
        new("control.gpu.power_limit", "GPU Power Limit", "device.gpu.rtx4060-laptop", ControlKind.Range, ControlRiskLevel.SafeControl),
        new("control.fan.curve", "Fan Curve", "device.platform.mock-host", ControlKind.Curve, ControlRiskLevel.HardwareWrite),
        new("control.platform.profile", "Platform Profile", "device.platform.mock-host", ControlKind.Mode, ControlRiskLevel.SafeControl)
    };

    public static IReadOnlyList<ProfileDescriptor> Profiles { get; } = new List<ProfileDescriptor>
    {
        new("profile.silent", "Silent", "Noise-first operation with reduced boost behavior.", new[] { "cap.fan.control", "cap.power.policy" }),
        new("profile.balanced", "Balanced", "Balanced thermal and performance behavior for daily workloads.", new[] { "cap.power.policy", "cap.scheduler.policy" }),
        new("profile.performance", "Performance", "Maximum sustained performance with aggressive cooling.", new[] { "cap.cpu.telemetry", "cap.gpu.telemetry", "cap.fan.control", "cap.thermal.policy" })
    };

    public static IReadOnlyList<PolicyDescriptor> Policies { get; } = new List<PolicyDescriptor>
    {
        new("policy.thermal.default", "Default Thermal Chain", "Thermal limits and emergency handling chain.", new[] { "sensor.cpu.temperature", "sensor.gpu.temperature" }, new[] { "cap.thermal.policy" }),
        new("policy.scheduler.performance", "Performance Scheduler Policy", "Scheduler hints for high-performance foreground tasks.", new[] { "sensor.profile.active" }, new[] { "cap.scheduler.policy" })
    };

    public static IReadOnlyList<SensorValue> CurrentSensorValues { get; } = new List<SensorValue>
    {
        new("sensor.cpu.temperature", "82°C"),
        new("sensor.cpu.package_power", "95W"),
        new("sensor.cpu.clock", "4.8 GHz"),
        new("sensor.cpu.power_limits", "PL1 90W / PL2 140W"),
        new("sensor.cpu.status", "Performance"),
        new("sensor.gpu.temperature", "76°C"),
        new("sensor.gpu.board_power", "105W"),
        new("sensor.gpu.clock", "2370 MHz"),
        new("sensor.gpu.power_limit", "Power Limit 115W"),
        new("sensor.gpu.status", "Boost"),
        new("sensor.profile.active", "Performance"),
        new("sensor.profile.summary", "AC power · fan curve · scheduler boost"),
        new("sensor.profile.details", "Hardware write disabled in mock mode"),
        new("sensor.profile.state", "Mock"),
        new("sensor.fan.mode", "Curve controlled by Max(CPU, GPU)"),
        new("sensor.fan.state", "Auto"),
        new("sensor.fan.cpu_rpm", "CPU 4200 RPM"),
        new("sensor.fan.gpu_rpm", "GPU 3900 RPM"),
        new("sensor.fan.response", "Ramp-up 2s · Ramp-down 8s"),
        new("sensor.power.state", "Plugged in"),
        new("sensor.power.summary", "AC · High performance"),
        new("sensor.power.details", "Processor boost enabled · EcoQoS for background"),
        new("sensor.thermal.policy", "CPU target 92°C · GPU target 84°C"),
        new("sensor.thermal.summary", "No emergency action active"),
        new("sensor.thermal.state", "Normal")
    };

    public static IReadOnlyList<PluginDescriptor> InstalledPlugins { get; } = new List<PluginDescriptor>
    {
        new(
            "semcosm.windows.power",
            "Windows Power Plugin",
            "Semcosm",
            "0.1.0",
            HardwareRiskLevel.SafeControl,
            new[]
            {
                "power.plan",
                "power.ac_dc",
                "scheduler.ecoqos",
                "process.priority"
            },
            new[]
            {
                "Windows power subsystem"
            }),
        new(
            "semcosm.nvidia.nvapi",
            "NVIDIA NVAPI Plugin",
            "Semcosm",
            "0.1.0",
            HardwareRiskLevel.SafeControl,
            new[]
            {
                "gpu.sensor.temperature",
                "gpu.sensor.power",
                "gpu.performance_state",
                "gpu.power_limit"
            },
            new[]
            {
                "NVIDIA GeForce RTX 4060 Laptop"
            }),
        new(
            "semcosm.mechrevo.gm6px0x",
            "Mechrevo GM6PX0X Platform Plugin",
            "Semcosm",
            "0.1.0",
            HardwareRiskLevel.HardwareWrite,
            new[]
            {
                "fan.cpu.rpm",
                "fan.gpu.rpm",
                "fan.cpu.pwm",
                "fan.gpu.pwm",
                "platform.performance_mode"
            },
            new[]
            {
                "MECHREVO Kuangshi16Pro GM6PX0X"
            })
    };

    public static string GetPluginState(string pluginId)
    {
        return pluginId switch
        {
            "semcosm.windows.power" => "Enabled",
            "semcosm.nvidia.nvapi" => "Mocked",
            "semcosm.mechrevo.gm6px0x" => "Mocked",
            _ => "Unknown"
        };
    }
}
