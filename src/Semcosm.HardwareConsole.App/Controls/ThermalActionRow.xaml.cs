using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class ThermalActionRow : UserControl
{
    public static readonly DependencyProperty StageLabelProperty =
        DependencyProperty.Register(nameof(StageLabel), typeof(string), typeof(ThermalActionRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TriggerTextProperty =
        DependencyProperty.Register(nameof(TriggerText), typeof(string), typeof(ThermalActionRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(ThermalActionRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(ThermalActionRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TargetValueProperty =
        DependencyProperty.Register(nameof(TargetValue), typeof(string), typeof(ThermalActionRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(ThermalActionRow), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty ShowConfirmationProperty =
        DependencyProperty.Register(nameof(ShowConfirmation), typeof(bool), typeof(ThermalActionRow), new PropertyMetadata(false));

    public static readonly DependencyProperty ConfirmationTextProperty =
        DependencyProperty.Register(nameof(ConfirmationText), typeof(string), typeof(ThermalActionRow), new PropertyMetadata(string.Empty));

    public ThermalActionRow()
    {
        InitializeComponent();
    }

    public string StageLabel
    {
        get => (string)GetValue(StageLabelProperty);
        set => SetValue(StageLabelProperty, value);
    }

    public string TriggerText
    {
        get => (string)GetValue(TriggerTextProperty);
        set => SetValue(TriggerTextProperty, value);
    }

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string TargetValue
    {
        get => (string)GetValue(TargetValueProperty);
        set => SetValue(TargetValueProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }

    public bool ShowConfirmation
    {
        get => (bool)GetValue(ShowConfirmationProperty);
        set => SetValue(ShowConfirmationProperty, value);
    }

    public string ConfirmationText
    {
        get => (string)GetValue(ConfirmationTextProperty);
        set => SetValue(ConfirmationTextProperty, value);
    }
}
