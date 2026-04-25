namespace Semcosm.HardwareConsole.Abstractions;

public enum PolicyPreviewFailureCode
{
    None,
    InvalidPolicy,
    MissingRequiredSensor,
    UnsupportedControl,
    RuntimeError
}
