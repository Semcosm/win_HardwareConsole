using System.Collections.ObjectModel;
using Semcosm.HardwareConsole.Abstractions.Models;
using Semcosm.HardwareConsole.Mock.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

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
