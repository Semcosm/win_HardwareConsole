using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class ControlRow : UserControl
{
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(ControlRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(ControlRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CurrentValueProperty =
        DependencyProperty.Register(nameof(CurrentValue), typeof(string), typeof(ControlRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty UnitTextProperty =
        DependencyProperty.Register(nameof(UnitText), typeof(string), typeof(ControlRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.Register(nameof(SourceText), typeof(string), typeof(ControlRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(ControlRow), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public ControlRow()
    {
        InitializeComponent();
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

    public string CurrentValue
    {
        get => (string)GetValue(CurrentValueProperty);
        set => SetValue(CurrentValueProperty, value);
    }

    public string UnitText
    {
        get => (string)GetValue(UnitTextProperty);
        set => SetValue(UnitTextProperty, value);
    }

    public string SourceText
    {
        get => (string)GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }
}
