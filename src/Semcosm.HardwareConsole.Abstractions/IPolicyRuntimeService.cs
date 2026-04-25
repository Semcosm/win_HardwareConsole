using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IPolicyRuntimeService
{
    IReadOnlyList<FanCurvePolicyDescriptor> GetAvailableFanPolicies();
    PolicyRuntimePreview PreviewFanPolicy(FanCurvePolicyDescriptor policy);
}
