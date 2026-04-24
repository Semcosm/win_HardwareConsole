using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.Abstractions.Models;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockHardwareService
{
    public IReadOnlyList<MetricCardModel> GetDashboardSummaryCards()
    {
        return new List<MetricCardModel>
        {
            new()
            {
                Title = "CPU",
                Subtitle = "Intel Core i9-13900HX",
                PrimaryValue = "82°C · 95W · 4.8 GHz",
                SecondaryValue = "PL1 90W / PL2 140W",
                Status = "Performance"
            },
            new()
            {
                Title = "GPU",
                Subtitle = "NVIDIA GeForce RTX 4060 Laptop",
                PrimaryValue = "76°C · 105W · 2370 MHz",
                SecondaryValue = "Power Limit 115W",
                Status = "Boost"
            },
            new()
            {
                Title = "Active Profile",
                Subtitle = "AC power · fan curve · scheduler boost",
                PrimaryValue = "Performance",
                SecondaryValue = "Hardware write disabled in mock mode",
                Status = "Mock"
            }
        };
    }

    public IReadOnlyList<MetricCardModel> GetDashboardDetailCards()
    {
        return new List<MetricCardModel>
        {
            new()
            {
                Title = "Fans",
                Subtitle = "Curve controlled by Max(CPU, GPU)",
                PrimaryValue = "CPU 4200 RPM · GPU 3900 RPM",
                SecondaryValue = "Ramp-up 2s · Ramp-down 8s",
                Status = "Auto"
            },
            new()
            {
                Title = "Power",
                Subtitle = "Windows power state",
                PrimaryValue = "AC · High performance",
                SecondaryValue = "Processor boost enabled · EcoQoS for background",
                Status = "Plugged in"
            },
            new()
            {
                Title = "Thermal",
                Subtitle = "Thermal policy chain",
                PrimaryValue = "CPU target 92°C · GPU target 84°C",
                SecondaryValue = "No emergency action active",
                Status = "Normal"
            }
        };
    }

    public IReadOnlyList<PluginDescriptor> GetInstalledPluginDescriptors()
    {
        return new List<PluginDescriptor>
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
    }

    public string GetInstalledPluginState(string pluginId)
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
