using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class SchedulerPolicyCard : UserControl
{
    public static readonly DependencyProperty PolicyIdProperty =
        DependencyProperty.Register(nameof(PolicyId), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(SchedulerPolicyCard), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty ScopeTextProperty =
        DependencyProperty.Register(nameof(ScopeText), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RuleCountTextProperty =
        DependencyProperty.Register(nameof(RuleCountText), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TriggerSummaryProperty =
        DependencyProperty.Register(nameof(TriggerSummary), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ForegroundStrategyTextProperty =
        DependencyProperty.Register(nameof(ForegroundStrategyText), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty BackgroundStrategyTextProperty =
        DependencyProperty.Register(nameof(BackgroundStrategyText), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ControlSummaryProperty =
        DependencyProperty.Register(nameof(ControlSummary), typeof(string), typeof(SchedulerPolicyCard), new PropertyMetadata(string.Empty));

    public event RoutedEventHandler? PreviewRequested;

    public SchedulerPolicyCard()
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

    public string RuleCountText
    {
        get => (string)GetValue(RuleCountTextProperty);
        set => SetValue(RuleCountTextProperty, value);
    }

    public string TriggerSummary
    {
        get => (string)GetValue(TriggerSummaryProperty);
        set => SetValue(TriggerSummaryProperty, value);
    }

    public string ForegroundStrategyText
    {
        get => (string)GetValue(ForegroundStrategyTextProperty);
        set => SetValue(ForegroundStrategyTextProperty, value);
    }

    public string BackgroundStrategyText
    {
        get => (string)GetValue(BackgroundStrategyTextProperty);
        set => SetValue(BackgroundStrategyTextProperty, value);
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
