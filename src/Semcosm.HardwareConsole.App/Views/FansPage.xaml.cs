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

    private void FanCurveEditor_DraftChanged(object sender, RoutedEventArgs e)
    {
        if (sender is FanCurveEditor editor && !string.IsNullOrWhiteSpace(editor.PolicyId))
        {
            ViewModel.UpdateDraft(
                editor.PolicyId,
                editor.SelectedInputSensorId,
                editor.SelectedOutputControlId);
        }
    }

    private void FanCurveEditor_ResetRequested(object sender, RoutedEventArgs e)
    {
        if (sender is FanCurveEditor editor && !string.IsNullOrWhiteSpace(editor.PolicyId))
        {
            ViewModel.ResetPolicy(editor.PolicyId);
        }
    }

    private void FanCurveEditor_ApplyRequested(object sender, RoutedEventArgs e)
    {
        if (sender is FanCurveEditor editor && !string.IsNullOrWhiteSpace(editor.PolicyId))
        {
            ViewModel.ApplyMockPolicy(editor.PolicyId);
        }
    }
}
