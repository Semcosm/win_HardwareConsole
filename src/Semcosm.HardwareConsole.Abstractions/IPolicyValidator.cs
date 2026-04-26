namespace Semcosm.HardwareConsole.Abstractions;

public interface IPolicyValidator<in TPolicy, out TValidationResult>
{
    TValidationResult Validate(TPolicy policy);
}
