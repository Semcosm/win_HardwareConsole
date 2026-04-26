using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class SchedulerRuleRow : UserControl
{
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(SchedulerRuleRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty MatchTextProperty =
        DependencyProperty.Register(nameof(MatchText), typeof(string), typeof(SchedulerRuleRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(SchedulerRuleRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ActionSummaryProperty =
        DependencyProperty.Register(nameof(ActionSummary), typeof(string), typeof(SchedulerRuleRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(SchedulerRuleRow), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public SchedulerRuleRow()
    {
        InitializeComponent();
    }

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string MatchText
    {
        get => (string)GetValue(MatchTextProperty);
        set => SetValue(MatchTextProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string ActionSummary
    {
        get => (string)GetValue(ActionSummaryProperty);
        set => SetValue(ActionSummaryProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }
}
