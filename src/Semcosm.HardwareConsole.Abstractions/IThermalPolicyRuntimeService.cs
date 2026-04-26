using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IThermalPolicyRuntimeService
{
    IReadOnlyList<ThermalPolicyDescriptor> GetAvailableThermalPolicies();
    ThermalPolicyPreview PreviewThermalPolicy(ThermalPolicyDescriptor policy);
}
