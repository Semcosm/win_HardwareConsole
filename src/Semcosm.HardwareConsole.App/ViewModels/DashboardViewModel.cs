using System.Collections.ObjectModel;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.Abstractions.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class DashboardViewModel
{
    public ObservableCollection<MetricCardModel> SummaryCards { get; }
    public ObservableCollection<MetricCardModel> DetailCards { get; }

    public DashboardViewModel(IHardwareDataService hardwareDataService)
    {
        SummaryCards = new ObservableCollection<MetricCardModel>(
            hardwareDataService.GetDashboardSummaryCards());

        DetailCards = new ObservableCollection<MetricCardModel>(
            hardwareDataService.GetDashboardDetailCards());
    }
}
