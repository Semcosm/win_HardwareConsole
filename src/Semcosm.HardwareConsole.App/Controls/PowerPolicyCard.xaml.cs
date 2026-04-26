using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class PowerPolicyCard : UserControl
{
    public static readonly DependencyProperty PolicyIdProperty =
        DependencyProperty.Register(nameof(PolicyId), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(PowerPolicyCard), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty ScopeTextProperty =
        DependencyProperty.Register(nameof(ScopeText), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PlanTextProperty =
        DependencyProperty.Register(nameof(PlanText), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TriggerSummaryProperty =
        DependencyProperty.Register(nameof(TriggerSummary), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ActionCountTextProperty =
        DependencyProperty.Register(nameof(ActionCountText), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty AcBehaviorTextProperty =
        DependencyProperty.Register(nameof(AcBehaviorText), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DcBehaviorTextProperty =
        DependencyProperty.Register(nameof(DcBehaviorText), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ControlSummaryProperty =
        DependencyProperty.Register(nameof(ControlSummary), typeof(string), typeof(PowerPolicyCard), new PropertyMetadata(string.Empty));

    public event RoutedEventHandler? PreviewRequested;

    public PowerPolicyCard()
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

    public string PlanText
    {
        get => (string)GetValue(PlanTextProperty);
        set => SetValue(PlanTextProperty, value);
    }

    public string TriggerSummary
    {
        get => (string)GetValue(TriggerSummaryProperty);
        set => SetValue(TriggerSummaryProperty, value);
    }

    public string ActionCountText
    {
        get => (string)GetValue(ActionCountTextProperty);
        set => SetValue(ActionCountTextProperty, value);
    }

    public string AcBehaviorText
    {
        get => (string)GetValue(AcBehaviorTextProperty);
        set => SetValue(AcBehaviorTextProperty, value);
    }

    public string DcBehaviorText
    {
        get => (string)GetValue(DcBehaviorTextProperty);
        set => SetValue(DcBehaviorTextProperty, value);
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
