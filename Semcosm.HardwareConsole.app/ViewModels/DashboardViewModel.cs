using System.Collections.ObjectModel;
using Semcosm.HardwareConsole.app.Models;
using Semcosm.HardwareConsole.app.Services;

namespace Semcosm.HardwareConsole.app.ViewModels;

public sealed class DashboardViewModel
{
    public ObservableCollection<MetricCardModel> SummaryCards { get; }
    public ObservableCollection<MetricCardModel> DetailCards { get; }

    public DashboardViewModel()
    {
        var mockHardwareService = new MockHardwareService();

        SummaryCards = new ObservableCollection<MetricCardModel>(
            mockHardwareService.GetDashboardSummaryCards());

        DetailCards = new ObservableCollection<MetricCardModel>(
            mockHardwareService.GetDashboardDetailCards());
    }
}
