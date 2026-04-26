using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class DevelopmentPluginManifestRootProvider : IPluginManifestRootProvider
{
    public string? GetPluginsRoot()
    {
        var candidateRoots = EnumerateCandidateRoots(AppContext.BaseDirectory)
            .Concat(EnumerateCandidateRoots(Directory.GetCurrentDirectory()))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var candidate in candidateRoots)
        {
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> EnumerateCandidateRoots(string startPath)
    {
        for (var directory = new DirectoryInfo(startPath); directory is not null; directory = directory.Parent)
        {
            yield return Path.Combine(directory.FullName, "plugins");
        }
    }
}
