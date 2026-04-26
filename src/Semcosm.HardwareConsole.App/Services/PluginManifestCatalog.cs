using System.Collections.Generic;
using System.Linq;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class PluginManifestCatalog
{
    private readonly PluginManifestLoader _loader;
    private readonly PluginManifestValidator _validator;
    private IReadOnlyList<PluginManifestValidationResult>? _validationResults;

    public PluginManifestCatalog(
        PluginManifestLoader loader,
        PluginManifestValidator validator)
    {
        _loader = loader;
        _validator = validator;
    }

    public IReadOnlyList<PluginManifestValidationResult> GetValidationResults()
    {
        EnsureLoaded();
        return _validationResults!;
    }

    public IReadOnlyList<PluginManifestDescriptor> GetValidManifests()
    {
        return GetValidationResults()
            .Where(result => result.IsValid && result.Manifest is not null)
            .Select(result => result.Manifest!)
            .ToArray();
    }

    public IReadOnlyList<PluginManifestValidationResult> Reload()
    {
        var loadResults = _loader.LoadManifests();
        _validationResults = _validator.Validate(loadResults);
        return _validationResults;
    }

    private void EnsureLoaded()
    {
        if (_validationResults is not null)
        {
            return;
        }

        Reload();
    }
}
