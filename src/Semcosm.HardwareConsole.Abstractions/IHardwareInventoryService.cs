using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IHardwareInventoryService
{
    IReadOnlyList<HardwareCapability> GetCapabilities();
    IReadOnlyList<DeviceDescriptor> GetDevices();
    IReadOnlyList<SensorDescriptor> GetSensors();
    IReadOnlyList<ControlDescriptor> GetControls();
    IReadOnlyList<ProfileDescriptor> GetProfiles();
    IReadOnlyList<PolicyDescriptor> GetPolicies();
}
