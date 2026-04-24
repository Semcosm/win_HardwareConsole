using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions.Models;

namespace Semcosm.HardwareConsole.Abstractions;

public interface IHardwareDataService
{
    IReadOnlyList<MetricCardModel> GetDashboardSummaryCards();
    IReadOnlyList<MetricCardModel> GetDashboardDetailCards();
    IReadOnlyList<PluginDescriptor> GetInstalledPluginDescriptors();
    string GetInstalledPluginState(string pluginId);
}
