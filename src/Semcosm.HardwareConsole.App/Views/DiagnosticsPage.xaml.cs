using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class DiagnosticsPage : Page
{
    public DiagnosticsViewModel ViewModel { get; }

    public DiagnosticsPage()
    {
        ViewModel = App.GetService<DiagnosticsViewModel>();
        InitializeComponent();
        Unloaded += DiagnosticsPage_Unloaded;
    }

    private void ClearSessionLogButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ClearDiagnostics();
    }

    private void DiagnosticsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Unloaded -= DiagnosticsPage_Unloaded;
    }
}
