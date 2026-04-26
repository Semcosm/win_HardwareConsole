using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class PowerPage : Page
{
    public PowerViewModel ViewModel { get; }

    public PowerPage()
    {
        ViewModel = App.GetService<PowerViewModel>();
        InitializeComponent();
    }

    private void PowerPolicyCard_PreviewRequested(object sender, RoutedEventArgs e)
    {
        if (sender is PowerPolicyCard card && !string.IsNullOrWhiteSpace(card.PolicyId))
        {
            ViewModel.PreviewPolicy(card.PolicyId);
        }
    }
}
