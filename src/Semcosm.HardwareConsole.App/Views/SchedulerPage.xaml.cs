using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class SchedulerPage : Page
{
    public SchedulerViewModel ViewModel { get; }

    public SchedulerPage()
    {
        ViewModel = App.GetService<SchedulerViewModel>();
        InitializeComponent();
    }

    private void SchedulerPolicyCard_PreviewRequested(object sender, RoutedEventArgs e)
    {
        if (sender is SchedulerPolicyCard card && !string.IsNullOrWhiteSpace(card.PolicyId))
        {
            ViewModel.PreviewPolicy(card.PolicyId);
        }
    }
}
