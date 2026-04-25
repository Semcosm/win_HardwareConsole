using System;
using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

internal static class MockHardwareData
{
    private static readonly DateTimeOffset SnapshotTimestamp = DateTimeOffset.UtcNow;

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
        new("sensor.cpu.temperature", "CPU Package Temp", "device.cpu.intel-13900hx", SensorKind.Temperature, "C")
        {
            CapabilityTags = new[] { "fan.policy.input" }
        },
        new("sensor.cpu.package_power", "CPU Package Power", "device.cpu.intel-13900hx", SensorKind.Power, "W"),
        new("sensor.cpu.clock", "CPU Clock", "device.cpu.intel-13900hx", SensorKind.Clock, "GHz"),
        new("sensor.cpu.power_limits", "CPU Power Limits", "device.cpu.intel-13900hx", SensorKind.Text, string.Empty),
        new("sensor.cpu.status", "CPU Status", "device.cpu.intel-13900hx", SensorKind.State, string.Empty),
        new("sensor.gpu.temperature", "GPU Temp", "device.gpu.rtx4060-laptop", SensorKind.Temperature, "C")
        {
            CapabilityTags = new[] { "fan.policy.input" }
        },
        new("sensor.gpu.board_power", "GPU Board Power", "device.gpu.rtx4060-laptop", SensorKind.Power, "W"),
        new("sensor.gpu.clock", "GPU Clock", "device.gpu.rtx4060-laptop", SensorKind.Clock, "MHz"),
        new("sensor.gpu.power_limit", "GPU Power Limit", "device.gpu.rtx4060-laptop", SensorKind.Text, string.Empty),
        new("sensor.gpu.status", "GPU Status", "device.gpu.rtx4060-laptop", SensorKind.State, string.Empty),
        new("sensor.fan.mode", "Fan Mode", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.fan.state", "Fan State", "device.platform.mock-host", SensorKind.State, string.Empty),
        new("sensor.fan.cpu_rpm", "CPU Fan RPM", "device.platform.mock-host", SensorKind.FanSpeed, "RPM"),
        new("sensor.fan.gpu_rpm", "GPU Fan RPM", "device.platform.mock-host", SensorKind.FanSpeed, "RPM"),
        new("sensor.fan.response", "Fan Response", "device.platform.mock-host", SensorKind.Text, string.Empty),
        new("sensor.thermal.max_cpu_gpu", "Max(CPU, GPU)", "device.platform.mock-host", SensorKind.Temperature, "C")
        {
            CapabilityTags = new[] { "fan.policy.input" }
        },
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
        new("control.fan.cpu_pwm", "CPU Fan PWM", "device.platform.mock-host", ControlKind.Fan, ControlRiskLevel.HardwareWrite)
        {
            CapabilityTags = new[] { "fan.policy.output" }
        },
        new("control.fan.gpu_pwm", "GPU Fan PWM", "device.platform.mock-host", ControlKind.Fan, ControlRiskLevel.HardwareWrite)
        {
            CapabilityTags = new[] { "fan.policy.output" }
        },
        new("control.fan.curve", "Fan Curve", "device.platform.mock-host", ControlKind.FanCurve, ControlRiskLevel.HardwareWrite)
        {
            CapabilityTags = new[] { "fan.policy.output" }
        },
        new("control.platform.profile", "Platform Profile", "device.platform.mock-host", ControlKind.Mode, ControlRiskLevel.SafeControl)
    };

    public static IReadOnlyList<ProfileDescriptor> Profiles { get; } = new List<ProfileDescriptor>
    {
        new(
            "profile.silent",
            "Silent",
            "Noise-first operation with reduced boost behavior and a quiet fan curve.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.SafeControl,
            new[] { "cap.fan.control", "cap.power.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Silent", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Quiet Curve", ControlRiskLevel.HardwareWrite),
                TextAction("control.cpu.power_limits", "PL1 45W / PL2 70W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 60, "W", "60W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.power.battery", "policy.thermal.quiet" }),
        new(
            "profile.balanced",
            "Balanced",
            "Balanced thermal and performance behavior for daily workloads.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.SafeControl,
            new[] { "cap.power.policy", "cap.scheduler.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Balanced", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Balanced Curve", ControlRiskLevel.HardwareWrite),
                TextAction("control.cpu.power_limits", "PL1 70W / PL2 110W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 90, "W", "90W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.thermal.default" }),
        new(
            "profile.performance",
            "Performance",
            "Maximum sustained performance with aggressive cooling and foreground bias.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.SafeControl,
            new[] { "cap.cpu.telemetry", "cap.gpu.telemetry", "cap.fan.control", "cap.thermal.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Performance", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Performance Curve", ControlRiskLevel.HardwareWrite),
                TextAction("control.cpu.power_limits", "PL1 90W / PL2 140W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 115, "W", "115W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.thermal.default", "policy.scheduler.performance" }),
        new(
            "profile.turbo",
            "Turbo",
            "Short-burst maximum performance with elevated fan noise and hardware-write controls.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.HardwareWrite,
            new[] { "cap.cpu.telemetry", "cap.gpu.telemetry", "cap.fan.control", "cap.thermal.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Turbo", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Turbo Curve", ControlRiskLevel.HardwareWrite, requiresConfirmation: true),
                TextAction("control.cpu.power_limits", "PL1 110W / PL2 157W", ControlRiskLevel.HardwareWrite, requiresConfirmation: true),
                NumericAction("control.gpu.power_limit", 140, "W", "140W", ControlRiskLevel.HardwareWrite, requiresConfirmation: true)
            },
            new[] { "policy.thermal.turbo", "policy.scheduler.performance" }),
        new(
            "profile.compile",
            "Compile",
            "CPU-prioritized compile preset with stable GPU limits and scheduler tuning.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.SafeControl,
            new[] { "cap.cpu.telemetry", "cap.power.policy", "cap.scheduler.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Compile", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Build Curve", ControlRiskLevel.HardwareWrite),
                TextAction("control.cpu.power_limits", "PL1 100W / PL2 145W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 80, "W", "80W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.scheduler.compile", "policy.thermal.default" }),
        new(
            "profile.streaming",
            "Streaming",
            "Balanced foreground performance with quieter acoustics for capture and streaming.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.SafeControl,
            new[] { "cap.gpu.telemetry", "cap.scheduler.policy", "cap.power.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Streaming", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Streaming Curve", ControlRiskLevel.HardwareWrite),
                TextAction("control.cpu.power_limits", "PL1 75W / PL2 105W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 95, "W", "95W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.scheduler.streaming", "policy.thermal.default" }),
        new(
            "profile.battery",
            "Battery",
            "Low-power battery mode with conservative CPU, GPU and fan targets.",
            ProfileKind.BuiltIn,
            HardwareRiskLevel.SafeControl,
            new[] { "cap.power.policy", "cap.scheduler.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Battery", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Battery Saver Curve", ControlRiskLevel.HardwareWrite),
                TextAction("control.cpu.power_limits", "PL1 28W / PL2 45W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 45, "W", "45W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.power.battery" }),
        new(
            "profile.creator-custom",
            "Creator Custom",
            "User-defined mixed workload preset with quieter fans and sustained compile headroom.",
            ProfileKind.User,
            HardwareRiskLevel.HardwareWrite,
            new[] { "cap.fan.control", "cap.power.policy", "cap.scheduler.policy" },
            new[]
            {
                TextAction("control.platform.profile", "Creator Custom", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "Creator Curve", ControlRiskLevel.HardwareWrite, requiresConfirmation: true),
                TextAction("control.cpu.power_limits", "PL1 85W / PL2 125W", ControlRiskLevel.SafeControl),
                NumericAction("control.gpu.power_limit", 105, "W", "105W", ControlRiskLevel.SafeControl)
            },
            new[] { "policy.scheduler.compile" }),
        new(
            "profile.oem-gaming-plus",
            "OEM Gaming+",
            "Device-provided vendor preset with elevated thermal targets and confirmation-gated writes.",
            ProfileKind.DeviceProvided,
            HardwareRiskLevel.HardwareWrite,
            new[] { "cap.cpu.telemetry", "cap.gpu.telemetry", "cap.fan.control", "cap.thermal.policy" },
            new[]
            {
                TextAction("control.platform.profile", "OEM Gaming+", ControlRiskLevel.SafeControl),
                TextAction("control.fan.curve", "OEM Gaming Curve", ControlRiskLevel.HardwareWrite, requiresConfirmation: true),
                TextAction("control.cpu.power_limits", "PL1 105W / PL2 150W", ControlRiskLevel.HardwareWrite, requiresConfirmation: true),
                NumericAction("control.gpu.power_limit", 130, "W", "130W", ControlRiskLevel.HardwareWrite, requiresConfirmation: true)
            },
            new[] { "policy.thermal.turbo", "policy.scheduler.performance" })
    };

    public static IReadOnlyList<PolicyDescriptor> Policies { get; } = new List<PolicyDescriptor>
    {
        new("policy.fan.cpu", "CPU Fan Policy", "CPU fan PWM follows CPU package temperature through a mock fan curve.", new[] { "sensor.cpu.temperature" }, new[] { "cap.fan.control", "cap.cpu.telemetry" }),
        new("policy.fan.gpu", "GPU Fan Policy", "GPU fan PWM follows GPU temperature through a mock fan curve.", new[] { "sensor.gpu.temperature" }, new[] { "cap.fan.control", "cap.gpu.telemetry" }),
        new("policy.fan.platform", "Platform Fan Policy", "Platform fan strategy follows Max(CPU, GPU) through a shared mock curve.", new[] { "sensor.thermal.max_cpu_gpu" }, new[] { "cap.fan.control", "cap.thermal.policy" }),
        new("policy.thermal.default", "Default Thermal Chain", "Thermal limits and emergency handling chain.", new[] { "sensor.cpu.temperature", "sensor.gpu.temperature" }, new[] { "cap.thermal.policy" }),
        new("policy.thermal.quiet", "Quiet Thermal Chain", "Lower fan ramp strategy and conservative thermal targets.", new[] { "sensor.cpu.temperature", "sensor.gpu.temperature" }, new[] { "cap.thermal.policy", "cap.fan.control" }),
        new("policy.thermal.turbo", "Turbo Thermal Chain", "Elevated fan response and higher thermal limits for burst performance.", new[] { "sensor.cpu.temperature", "sensor.gpu.temperature" }, new[] { "cap.thermal.policy", "cap.fan.control" }),
        new("policy.scheduler.performance", "Performance Scheduler Policy", "Scheduler hints for high-performance foreground tasks.", new[] { "sensor.cpu.clock" }, new[] { "cap.scheduler.policy" }),
        new("policy.scheduler.compile", "Compile Scheduler Policy", "Favors sustained CPU throughput and stable background scheduling.", new[] { "sensor.cpu.package_power" }, new[] { "cap.scheduler.policy" }),
        new("policy.scheduler.streaming", "Streaming Scheduler Policy", "Reserves foreground responsiveness for capture and broadcast tasks.", new[] { "sensor.gpu.clock" }, new[] { "cap.scheduler.policy" }),
        new("policy.power.battery", "Battery Saver Policy", "Reduces power targets and discourages boost while on battery.", new[] { "sensor.power.state" }, new[] { "cap.power.policy" })
    };

    public static IReadOnlyList<FanCurvePolicyDescriptor> FanCurvePolicies { get; } = new List<FanCurvePolicyDescriptor>
    {
        new(
            "fan-policy.cpu",
            "policy.fan.cpu",
            "CPU",
            "CPU Fan Policy",
            "CPU package temperature drives the CPU fan PWM curve in mock mode.",
            "sensor.cpu.temperature",
            "control.fan.cpu_pwm",
            new[]
            {
                CurvePoint(50, 30),
                CurvePoint(70, 55),
                CurvePoint(90, 100)
            },
            3,
            2,
            8,
            HardwareRiskLevel.HardwareWrite),
        new(
            "fan-policy.gpu",
            "policy.fan.gpu",
            "GPU",
            "GPU Fan Policy",
            "GPU temperature drives the GPU fan PWM curve in mock mode.",
            "sensor.gpu.temperature",
            "control.fan.gpu_pwm",
            new[]
            {
                CurvePoint(48, 28),
                CurvePoint(68, 58),
                CurvePoint(88, 100)
            },
            3,
            2,
            7,
            HardwareRiskLevel.HardwareWrite),
        new(
            "fan-policy.platform",
            "policy.fan.platform",
            "Platform",
            "Platform Fan Policy",
            "Max(CPU, GPU) drives the shared platform fan curve in mock mode.",
            "sensor.thermal.max_cpu_gpu",
            "control.fan.curve",
            new[]
            {
                CurvePoint(45, 28),
                CurvePoint(65, 50),
                CurvePoint(80, 78),
                CurvePoint(92, 100)
            },
            4,
            1.5,
            10,
            HardwareRiskLevel.HardwareWrite)
    };

    public static IReadOnlyList<SensorValue> CurrentSensorValues { get; } = new List<SensorValue>
    {
        NumericSensor("sensor.cpu.temperature", 82, "°C", "82°C"),
        NumericSensor("sensor.cpu.package_power", 95, "W", "95W"),
        NumericSensor("sensor.cpu.clock", 4.8, "GHz", "4.8 GHz"),
        TextSensor("sensor.cpu.power_limits", "PL1 90W / PL2 140W"),
        TextSensor("sensor.cpu.status", "Performance"),
        NumericSensor("sensor.gpu.temperature", 76, "°C", "76°C"),
        NumericSensor("sensor.gpu.board_power", 105, "W", "105W"),
        NumericSensor("sensor.gpu.clock", 2370, "MHz", "2370 MHz"),
        TextSensor("sensor.gpu.power_limit", "Power Limit 115W"),
        TextSensor("sensor.gpu.status", "Boost"),
        TextSensor("sensor.fan.mode", "Curve controlled by Max(CPU, GPU)"),
        TextSensor("sensor.fan.state", "Auto"),
        NumericSensor("sensor.fan.cpu_rpm", 4200, "RPM", "CPU 4200 RPM"),
        NumericSensor("sensor.fan.gpu_rpm", 3900, "RPM", "GPU 3900 RPM"),
        TextSensor("sensor.fan.response", "Ramp-up 2s · Ramp-down 8s"),
        NumericSensor("sensor.thermal.max_cpu_gpu", 82, "°C", "82°C"),
        TextSensor("sensor.power.state", "Plugged in"),
        TextSensor("sensor.power.summary", "AC · High performance"),
        TextSensor("sensor.power.details", "Processor boost enabled · EcoQoS for background"),
        TextSensor("sensor.thermal.policy", "CPU target 92°C · GPU target 84°C"),
        TextSensor("sensor.thermal.summary", "No emergency action active"),
        TextSensor("sensor.thermal.state", "Normal")
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
            },
            new[]
            {
                "device.cpu.intel-13900hx",
                "device.platform.mock-host"
            },
            new[]
            {
                "sensor.cpu.package_power",
                "sensor.power.state",
                "sensor.power.summary",
                "sensor.power.details"
            },
            new[]
            {
                "control.cpu.power_limits",
                "control.platform.profile"
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
            },
            new[]
            {
                "device.gpu.rtx4060-laptop"
            },
            new[]
            {
                "sensor.gpu.temperature",
                "sensor.gpu.board_power",
                "sensor.gpu.clock",
                "sensor.gpu.power_limit",
                "sensor.gpu.status"
            },
            new[]
            {
                "control.gpu.power_limit"
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
            },
            new[]
            {
                "device.platform.mock-host"
            },
            new[]
            {
                "sensor.fan.mode",
                "sensor.fan.state",
                "sensor.fan.cpu_rpm",
                "sensor.fan.gpu_rpm",
                "sensor.fan.response",
                "sensor.thermal.max_cpu_gpu",
                "sensor.thermal.policy",
                "sensor.thermal.summary",
                "sensor.thermal.state"
            },
            new[]
            {
                "control.fan.cpu_pwm",
                "control.fan.gpu_pwm",
                "control.fan.curve"
            })
    };

    public static PluginState GetPluginState(string pluginId)
    {
        return pluginId switch
        {
            "semcosm.windows.power" => PluginState.Enabled,
            "semcosm.nvidia.nvapi" => PluginState.Mocked,
            "semcosm.mechrevo.gm6px0x" => PluginState.Mocked,
            _ => PluginState.Unknown
        };
    }

    public static ProfileDescriptor? GetProfile(string profileId)
    {
        foreach (var profile in Profiles)
        {
            if (string.Equals(profile.Id, profileId, StringComparison.OrdinalIgnoreCase))
            {
                return profile;
            }
        }

        return null;
    }

    private static SensorValue NumericSensor(
        string sensorId,
        double numericValue,
        string unit,
        string formattedValue,
        ValueQuality quality = ValueQuality.Good)
    {
        return new SensorValue(
            sensorId,
            numericValue,
            null,
            unit,
            formattedValue,
            SnapshotTimestamp,
            quality);
    }

    private static SensorValue TextSensor(
        string sensorId,
        string textValue,
        ValueQuality quality = ValueQuality.Good)
    {
        return new SensorValue(
            sensorId,
            null,
            textValue,
            string.Empty,
            textValue,
            SnapshotTimestamp,
            quality);
    }

    private static ProfileControlActionDescriptor NumericAction(
        string controlId,
        double numericValue,
        string unit,
        string formattedValue,
        ControlRiskLevel riskLevel,
        bool requiresConfirmation = false)
    {
        return new ProfileControlActionDescriptor(
            controlId,
            new ControlValue(
                controlId,
                numericValue,
                null,
                unit,
                formattedValue,
                SnapshotTimestamp,
                ValueQuality.Good),
            riskLevel,
            requiresConfirmation);
    }

    private static ProfileControlActionDescriptor TextAction(
        string controlId,
        string formattedValue,
        ControlRiskLevel riskLevel,
        bool requiresConfirmation = false)
    {
        return new ProfileControlActionDescriptor(
            controlId,
            new ControlValue(
                controlId,
                null,
                formattedValue,
                string.Empty,
                formattedValue,
                SnapshotTimestamp,
                ValueQuality.Good),
            riskLevel,
            requiresConfirmation);
    }

    private static FanCurvePoint CurvePoint(double inputValue, double outputPercent)
    {
        return new FanCurvePoint(inputValue, outputPercent);
    }
}
