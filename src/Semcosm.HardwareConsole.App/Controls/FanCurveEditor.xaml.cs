using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class FanCurveEditor : UserControl
{
    public static readonly DependencyProperty PolicyIdProperty =
        DependencyProperty.Register(nameof(PolicyId), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ScopeTextProperty =
        DependencyProperty.Register(nameof(ScopeText), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PolicyTextProperty =
        DependencyProperty.Register(nameof(PolicyText), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(FanCurveEditor), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty InputSensorOptionsProperty =
        DependencyProperty.Register(nameof(InputSensorOptions), typeof(IEnumerable<SelectionOptionModel>), typeof(FanCurveEditor), new PropertyMetadata(null));

    public static readonly DependencyProperty OutputControlOptionsProperty =
        DependencyProperty.Register(nameof(OutputControlOptions), typeof(IEnumerable<SelectionOptionModel>), typeof(FanCurveEditor), new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedInputSensorIdProperty =
        DependencyProperty.Register(nameof(SelectedInputSensorId), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SelectedOutputControlIdProperty =
        DependencyProperty.Register(nameof(SelectedOutputControlId), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CurvePointsProperty =
        DependencyProperty.Register(nameof(CurvePoints), typeof(IEnumerable<FanCurvePointRowModel>), typeof(FanCurveEditor), new PropertyMetadata(null));

    public static readonly DependencyProperty HysteresisTextProperty =
        DependencyProperty.Register(nameof(HysteresisText), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RampUpTextProperty =
        DependencyProperty.Register(nameof(RampUpText), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RampDownTextProperty =
        DependencyProperty.Register(nameof(RampDownText), typeof(string), typeof(FanCurveEditor), new PropertyMetadata(string.Empty));

    public event RoutedEventHandler? PreviewRequested;

    public FanCurveEditor()
    {
        InitializeComponent();
    }

    public string PolicyId
    {
        get => (string)GetValue(PolicyIdProperty);
        set => SetValue(PolicyIdProperty, value);
    }

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string ScopeText
    {
        get => (string)GetValue(ScopeTextProperty);
        set => SetValue(ScopeTextProperty, value);
    }

    public string PolicyText
    {
        get => (string)GetValue(PolicyTextProperty);
        set => SetValue(PolicyTextProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }

    public IEnumerable<SelectionOptionModel>? InputSensorOptions
    {
        get => (IEnumerable<SelectionOptionModel>?)GetValue(InputSensorOptionsProperty);
        set => SetValue(InputSensorOptionsProperty, value);
    }

    public IEnumerable<SelectionOptionModel>? OutputControlOptions
    {
        get => (IEnumerable<SelectionOptionModel>?)GetValue(OutputControlOptionsProperty);
        set => SetValue(OutputControlOptionsProperty, value);
    }

    public string SelectedInputSensorId
    {
        get => (string)GetValue(SelectedInputSensorIdProperty);
        set => SetValue(SelectedInputSensorIdProperty, value);
    }

    public string SelectedOutputControlId
    {
        get => (string)GetValue(SelectedOutputControlIdProperty);
        set => SetValue(SelectedOutputControlIdProperty, value);
    }

    public IEnumerable<FanCurvePointRowModel>? CurvePoints
    {
        get => (IEnumerable<FanCurvePointRowModel>?)GetValue(CurvePointsProperty);
        set => SetValue(CurvePointsProperty, value);
    }

    public string HysteresisText
    {
        get => (string)GetValue(HysteresisTextProperty);
        set => SetValue(HysteresisTextProperty, value);
    }

    public string RampUpText
    {
        get => (string)GetValue(RampUpTextProperty);
        set => SetValue(RampUpTextProperty, value);
    }

    public string RampDownText
    {
        get => (string)GetValue(RampDownTextProperty);
        set => SetValue(RampDownTextProperty, value);
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        PreviewRequested?.Invoke(this, e);
    }
}
