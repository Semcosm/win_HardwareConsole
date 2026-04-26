using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IFanPolicyRuntimeService
{
    IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies();
    PolicyRuntimePreview PreviewFanPolicy(FanCurvePolicyDescriptor policy);
}
