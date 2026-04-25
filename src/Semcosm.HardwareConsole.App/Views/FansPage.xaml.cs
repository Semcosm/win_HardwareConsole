using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.Controls;
using Semcosm.HardwareConsole.App.ViewModels;

namespace Semcosm.HardwareConsole.App.Views;

public sealed partial class FansPage : Page
{
    public FansViewModel ViewModel { get; }

    public FansPage()
    {
        ViewModel = App.GetService<FansViewModel>();
        InitializeComponent();
    }

    private void FanCurveEditor_PreviewRequested(object sender, RoutedEventArgs e)
    {
        if (sender is FanCurveEditor editor && !string.IsNullOrWhiteSpace(editor.PolicyId))
        {
            ViewModel.PreviewPolicy(
                editor.PolicyId,
                editor.SelectedInputSensorId,
                editor.SelectedOutputControlId);
        }
    }
}
