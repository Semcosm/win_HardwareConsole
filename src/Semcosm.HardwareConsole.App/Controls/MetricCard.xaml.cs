using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class MetricCard : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(MetricCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(MetricCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PrimaryValueProperty =
        DependencyProperty.Register(nameof(PrimaryValue), typeof(string), typeof(MetricCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SecondaryValueProperty =
        DependencyProperty.Register(nameof(SecondaryValue), typeof(string), typeof(MetricCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(string), typeof(MetricCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CardWidthProperty =
        DependencyProperty.Register(nameof(CardWidth), typeof(double), typeof(MetricCard), new PropertyMetadata(400d));

    public static readonly DependencyProperty MinCardHeightProperty =
        DependencyProperty.Register(nameof(MinCardHeight), typeof(double), typeof(MetricCard), new PropertyMetadata(132d));

    public static readonly DependencyProperty PrimaryFontSizeProperty =
        DependencyProperty.Register(nameof(PrimaryFontSize), typeof(double), typeof(MetricCard), new PropertyMetadata(24d));

    public MetricCard()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string PrimaryValue
    {
        get => (string)GetValue(PrimaryValueProperty);
        set => SetValue(PrimaryValueProperty, value);
    }

    public string SecondaryValue
    {
        get => (string)GetValue(SecondaryValueProperty);
        set => SetValue(SecondaryValueProperty, value);
    }

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public double CardWidth
    {
        get => (double)GetValue(CardWidthProperty);
        set => SetValue(CardWidthProperty, value);
    }

    public double MinCardHeight
    {
        get => (double)GetValue(MinCardHeightProperty);
        set => SetValue(MinCardHeightProperty, value);
    }

    public double PrimaryFontSize
    {
        get => (double)GetValue(PrimaryFontSizeProperty);
        set => SetValue(PrimaryFontSizeProperty, value);
    }
}
