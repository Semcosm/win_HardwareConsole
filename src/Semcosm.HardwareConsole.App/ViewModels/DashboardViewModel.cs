using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class DashboardViewModel
{
    public ObservableCollection<MetricCardModel> SummaryCards { get; }
    public ObservableCollection<MetricCardModel> DetailCards { get; }

    public DashboardViewModel(
        IHardwareInventoryService hardwareInventoryService,
        ISensorSnapshotProvider sensorSnapshotProvider)
    {
        var devices = hardwareInventoryService
            .GetDevices()
            .ToDictionary(device => device.Id);

        var sensorValues = sensorSnapshotProvider
            .GetCurrentSensorValues()
            .ToDictionary(sensor => sensor.SensorId);

        SummaryCards = new ObservableCollection<MetricCardModel>(
            BuildSummaryCards(devices, sensorValues));

        DetailCards = new ObservableCollection<MetricCardModel>(
            BuildDetailCards(sensorValues));
    }

    private static IReadOnlyList<MetricCardModel> BuildSummaryCards(
        IReadOnlyDictionary<string, DeviceDescriptor> devices,
        IReadOnlyDictionary<string, SensorValue> sensorValues)
    {
        return new List<MetricCardModel>
        {
            new()
            {
                Title = "CPU",
                Subtitle = GetDeviceName(devices, "device.cpu.intel-13900hx", "CPU"),
                PrimaryValue = $"{GetSensorValue(sensorValues, "sensor.cpu.temperature")} · {GetSensorValue(sensorValues, "sensor.cpu.package_power")} · {GetSensorValue(sensorValues, "sensor.cpu.clock")}",
                SecondaryValue = GetSensorValue(sensorValues, "sensor.cpu.power_limits"),
                Status = GetSensorValue(sensorValues, "sensor.cpu.status")
            },
            new()
            {
                Title = "GPU",
                Subtitle = GetDeviceName(devices, "device.gpu.rtx4060-laptop", "GPU"),
                PrimaryValue = $"{GetSensorValue(sensorValues, "sensor.gpu.temperature")} · {GetSensorValue(sensorValues, "sensor.gpu.board_power")} · {GetSensorValue(sensorValues, "sensor.gpu.clock")}",
                SecondaryValue = GetSensorValue(sensorValues, "sensor.gpu.power_limit"),
                Status = GetSensorValue(sensorValues, "sensor.gpu.status")
            },
            new()
            {
                Title = "Active Profile",
                Subtitle = GetSensorValue(sensorValues, "sensor.profile.summary"),
                PrimaryValue = GetSensorValue(sensorValues, "sensor.profile.active"),
                SecondaryValue = GetSensorValue(sensorValues, "sensor.profile.details"),
                Status = GetSensorValue(sensorValues, "sensor.profile.state")
            }
        };
    }

    private static IReadOnlyList<MetricCardModel> BuildDetailCards(
        IReadOnlyDictionary<string, SensorValue> sensorValues)
    {
        return new List<MetricCardModel>
        {
            new()
            {
                Title = "Fans",
                Subtitle = GetSensorValue(sensorValues, "sensor.fan.mode"),
                PrimaryValue = $"{GetSensorValue(sensorValues, "sensor.fan.cpu_rpm")} · {GetSensorValue(sensorValues, "sensor.fan.gpu_rpm")}",
                SecondaryValue = GetSensorValue(sensorValues, "sensor.fan.response"),
                Status = GetSensorValue(sensorValues, "sensor.fan.state")
            },
            new()
            {
                Title = "Power",
                Subtitle = "Windows power state",
                PrimaryValue = GetSensorValue(sensorValues, "sensor.power.summary"),
                SecondaryValue = GetSensorValue(sensorValues, "sensor.power.details"),
                Status = GetSensorValue(sensorValues, "sensor.power.state")
            },
            new()
            {
                Title = "Thermal",
                Subtitle = "Thermal policy chain",
                PrimaryValue = GetSensorValue(sensorValues, "sensor.thermal.policy"),
                SecondaryValue = GetSensorValue(sensorValues, "sensor.thermal.summary"),
                Status = GetSensorValue(sensorValues, "sensor.thermal.state")
            }
        };
    }

    private static string GetDeviceName(
        IReadOnlyDictionary<string, DeviceDescriptor> devices,
        string deviceId,
        string fallback)
    {
        return devices.TryGetValue(deviceId, out var device)
            ? device.DisplayName
            : fallback;
    }

    private static string GetSensorValue(
        IReadOnlyDictionary<string, SensorValue> sensorValues,
        string sensorId,
        string fallback = "Unknown")
    {
        return sensorValues.TryGetValue(sensorId, out var value)
            ? value.FormattedValue
            : fallback;
    }
}
