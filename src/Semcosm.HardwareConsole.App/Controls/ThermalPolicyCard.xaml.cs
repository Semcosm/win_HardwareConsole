using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class ThermalPolicyCard : UserControl
{
    public static readonly DependencyProperty PolicyIdProperty =
        DependencyProperty.Register(nameof(PolicyId), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(ThermalPolicyCard), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty ScopeTextProperty =
        DependencyProperty.Register(nameof(ScopeText), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SensorSummaryProperty =
        DependencyProperty.Register(nameof(SensorSummary), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ActionCountTextProperty =
        DependencyProperty.Register(nameof(ActionCountText), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TimingTextProperty =
        DependencyProperty.Register(nameof(TimingText), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ControlSummaryProperty =
        DependencyProperty.Register(nameof(ControlSummary), typeof(string), typeof(ThermalPolicyCard), new PropertyMetadata(string.Empty));

    public event RoutedEventHandler? PreviewRequested;

    public ThermalPolicyCard()
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

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }

    public string ScopeText
    {
        get => (string)GetValue(ScopeTextProperty);
        set => SetValue(ScopeTextProperty, value);
    }

    public string SensorSummary
    {
        get => (string)GetValue(SensorSummaryProperty);
        set => SetValue(SensorSummaryProperty, value);
    }

    public string ActionCountText
    {
        get => (string)GetValue(ActionCountTextProperty);
        set => SetValue(ActionCountTextProperty, value);
    }

    public string TimingText
    {
        get => (string)GetValue(TimingTextProperty);
        set => SetValue(TimingTextProperty, value);
    }

    public string ControlSummary
    {
        get => (string)GetValue(ControlSummaryProperty);
        set => SetValue(ControlSummaryProperty, value);
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        PreviewRequested?.Invoke(this, e);
    }
}
