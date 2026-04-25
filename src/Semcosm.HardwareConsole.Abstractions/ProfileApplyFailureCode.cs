namespace Semcosm.HardwareConsole.Abstractions;

public enum ProfileApplyFailureCode
{
    None,
    UnknownProfile,
    ConfirmationRequired,
    BlockedByPolicy,
    BlockedByPermission,
    BlockedByPlugin,
    UnsupportedControl,
    PartialFailure,
    RuntimeError
}
