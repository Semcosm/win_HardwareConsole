using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public interface ISchedulerPolicyRuntimeService
{
    IReadOnlyList<SchedulerPolicyDescriptor> GetAvailableSchedulerPolicies();
    SchedulerPolicyPreview PreviewSchedulerPolicy(SchedulerPolicyDescriptor policy);
}
