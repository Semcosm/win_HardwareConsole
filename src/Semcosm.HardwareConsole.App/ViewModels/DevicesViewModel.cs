using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class DevicesViewModel
{
    public ObservableCollection<DeviceCardModel> Devices { get; }

    public DevicesViewModel(
        IHardwareInventoryService hardwareInventoryService,
        ISensorSnapshotProvider sensorSnapshotProvider,
        IPluginRegistry pluginRegistry,
        IProfileRuntimeService profileRuntimeService,
        DevicePresentationMapper devicePresentationMapper)
    {
        var capabilities = hardwareInventoryService.GetCapabilities();
        var devices = hardwareInventoryService.GetDevices();
        var sensors = hardwareInventoryService.GetSensors();
        var controls = hardwareInventoryService.GetControls();
        var sensorValues = sensorSnapshotProvider
            .GetCurrentSensorValues()
            .ToDictionary(sensor => sensor.SensorId);
        var installedPlugins = pluginRegistry.GetInstalledPlugins();
        var activeProfile = profileRuntimeService.GetActiveProfile();
        var activeActions = BuildActiveActionMap(activeProfile);

        Devices = new ObservableCollection<DeviceCardModel>(
            devices.Select(device =>
            {
                var deviceSensors = sensors
                    .Where(sensor => string.Equals(sensor.DeviceId, device.Id))
                    .Select(sensor => devicePresentationMapper.MapSensorRow(
                        sensor,
                        sensorValues.GetValueOrDefault(sensor.Id)))
                    .ToArray();

                var deviceControls = controls
                    .Where(control => string.Equals(control.DeviceId, device.Id))
                    .Select(control => devicePresentationMapper.MapControlRow(
                        control,
                        device.DisplayName,
                        activeProfile,
                        activeActions.GetValueOrDefault(control.Id)))
                    .ToArray();

                var deviceCapabilities = capabilities
                    .Where(capability => device.CapabilityIds.Contains(capability.Id))
                    .ToArray();

                return devicePresentationMapper.MapDeviceCard(
                    device,
                    deviceCapabilities,
                    installedPlugins,
                    deviceSensors,
                    deviceControls);
            }));
    }

    private static IReadOnlyDictionary<string, ProfileControlActionDescriptor> BuildActiveActionMap(
        ProfileDescriptor? activeProfile)
    {
        var map = new Dictionary<string, ProfileControlActionDescriptor>();

        if (activeProfile is null)
        {
            return map;
        }

        foreach (var action in activeProfile.Actions)
        {
            map[action.ControlId] = action;
        }

        return map;
    }
}
