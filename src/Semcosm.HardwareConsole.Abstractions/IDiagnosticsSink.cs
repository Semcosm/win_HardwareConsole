namespace Semcosm.HardwareConsole.Abstractions;

public interface IDiagnosticsSink
{
    void Report(DiagnosticRecord record);
}
