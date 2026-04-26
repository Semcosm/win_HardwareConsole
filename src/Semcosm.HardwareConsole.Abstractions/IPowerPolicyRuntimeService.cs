using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IPowerPolicyRuntimeService
{
    IReadOnlyList<PowerPolicyDescriptor> GetAvailablePowerPolicies();
    PowerPolicyPreview PreviewPowerPolicy(PowerPolicyDescriptor policy);
}
