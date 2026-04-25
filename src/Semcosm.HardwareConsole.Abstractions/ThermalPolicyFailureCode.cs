namespace Semcosm.HardwareConsole.Abstractions;

public enum ThermalPolicyFailureCode
{
    None,
    InvalidPolicy,
    MissingRequiredSensor,
    UnsupportedControl,
    RuntimeError
}
