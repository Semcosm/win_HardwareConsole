using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Semcosm.HardwareConsole.App.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class ThermalPage : Page
{
    public ThermalViewModel ViewModel { get; }

    public ThermalPage()
    {
        ViewModel = App.GetService<ThermalViewModel>();
        InitializeComponent();
    }

    private void ThermalPolicyCard_PreviewRequested(object sender, RoutedEventArgs e)
    {
        if (sender is ThermalPolicyCard card && !string.IsNullOrWhiteSpace(card.PolicyId))
        {
            ViewModel.PreviewPolicy(card.PolicyId);
        }
    }
}
