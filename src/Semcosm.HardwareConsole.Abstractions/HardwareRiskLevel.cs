namespace Semcosm.HardwareConsole.Abstractions;

public enum HardwareRiskLevel
{
    ReadOnly,
    SafeControl,
    HardwareWrite,
    KernelDriverRequired,
    Experimental
}
