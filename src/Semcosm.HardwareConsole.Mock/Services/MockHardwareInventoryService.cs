using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.Mock.Services;

public sealed class MockHardwareInventoryService : IHardwareInventoryService
{
    public IReadOnlyList<HardwareCapability> GetCapabilities() => MockHardwareData.Capabilities;

    public IReadOnlyList<DeviceDescriptor> GetDevices() => MockHardwareData.Devices;

    public IReadOnlyList<SensorDescriptor> GetSensors() => MockHardwareData.Sensors;

    public IReadOnlyList<ControlDescriptor> GetControls() => MockHardwareData.Controls;

    public IReadOnlyList<ProfileDescriptor> GetProfiles() => MockHardwareData.Profiles;

    public IReadOnlyList<PolicyDescriptor> GetPolicies() => MockHardwareData.Policies;
}
