using System.Collections.Generic;

namespace Semcosm.HardwareConsole.Abstractions;

public sealed record ProfileDescriptor(
    string Id,
    string DisplayName,
    string Description,
    ProfileKind Kind,
    HardwareRiskLevel RiskLevel,
    IReadOnlyList<string> CapabilityIds,
    IReadOnlyList<ProfileControlActionDescriptor> Actions,
    IReadOnlyList<string> PolicyIds);
