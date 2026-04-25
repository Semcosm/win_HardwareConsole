using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class SensorRow : UserControl
{
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(SensorRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(SensorRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CurrentValueProperty =
        DependencyProperty.Register(nameof(CurrentValue), typeof(string), typeof(SensorRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty UnitTextProperty =
        DependencyProperty.Register(nameof(UnitText), typeof(string), typeof(SensorRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty QualityTextProperty =
        DependencyProperty.Register(nameof(QualityText), typeof(string), typeof(SensorRow), new PropertyMetadata(string.Empty));

    public SensorRow()
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

    public string QualityText
    {
        get => (string)GetValue(QualityTextProperty);
        set => SetValue(QualityTextProperty, value);
    }
}
