using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class ProfilesPage : Page
{
    public ProfilesViewModel ViewModel { get; }

    public ProfilesPage()
    {
        ViewModel = App.GetService<ProfilesViewModel>();
        InitializeComponent();
    }

    private void ProfileCard_PreviewRequested(object sender, RoutedEventArgs e)
    {
        if (sender is ProfileCard card && !string.IsNullOrWhiteSpace(card.ProfileId))
        {
            ViewModel.PreviewProfile(card.ProfileId);
        }
    }

    private void ProfileCard_ApplyRequested(object sender, RoutedEventArgs e)
    {
        if (sender is ProfileCard card && !string.IsNullOrWhiteSpace(card.ProfileId))
        {
            ViewModel.ApplyProfile(card.ProfileId);
        }
    }
}
