using System.Collections.Generic;
using Semcosm.HardwareConsole.app.Models;

namespace Semcosm.HardwareConsole.app.Services;

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

    public IReadOnlyList<PluginManifestModel> GetInstalledPlugins()
    {
        return new List<PluginManifestModel>
        {
            new()
            {
                Id = "semcosm.windows.power",
                DisplayName = "Windows Power Plugin",
                Vendor = "Semcosm",
                Version = "0.1.0",
                State = "Enabled",
                RiskLevel = PluginRiskLevel.SafeControl,
                Capabilities =
                {
                    "power.plan",
                    "power.ac_dc",
                    "scheduler.ecoqos",
                    "process.priority"
                },
                MatchedDevices =
                {
                    "Windows power subsystem"
                }
            },
            new()
            {
                Id = "semcosm.nvidia.nvapi",
                DisplayName = "NVIDIA NVAPI Plugin",
                Vendor = "Semcosm",
                Version = "0.1.0",
                State = "Mocked",
                RiskLevel = PluginRiskLevel.SafeControl,
                Capabilities =
                {
                    "gpu.sensor.temperature",
                    "gpu.sensor.power",
                    "gpu.performance_state",
                    "gpu.power_limit"
                },
                MatchedDevices =
                {
                    "NVIDIA GeForce RTX 4060 Laptop"
                }
            },
            new()
            {
                Id = "semcosm.mechrevo.gm6px0x",
                DisplayName = "Mechrevo GM6PX0X Platform Plugin",
                Vendor = "Semcosm",
                Version = "0.1.0",
                State = "Mocked",
                RiskLevel = PluginRiskLevel.HardwareWrite,
                Capabilities =
                {
                    "fan.cpu.rpm",
                    "fan.gpu.rpm",
                    "fan.cpu.pwm",
                    "fan.gpu.pwm",
                    "platform.performance_mode"
                },
                MatchedDevices =
                {
                    "MECHREVO Kuangshi16Pro GM6PX0X"
                }
            }
        };
    }
}
