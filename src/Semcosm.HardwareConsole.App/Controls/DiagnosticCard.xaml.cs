using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class DiagnosticCard : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(DiagnosticCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(DiagnosticCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DetailsTextProperty =
        DependencyProperty.Register(nameof(DetailsText), typeof(string), typeof(DiagnosticCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.Register(nameof(SourceText), typeof(string), typeof(DiagnosticCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SeverityTextProperty =
        DependencyProperty.Register(nameof(SeverityText), typeof(string), typeof(DiagnosticCard), new PropertyMetadata(string.Empty));

    public DiagnosticCard()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public string DetailsText
    {
        get => (string)GetValue(DetailsTextProperty);
        set => SetValue(DetailsTextProperty, value);
    }

    public string SourceText
    {
        get => (string)GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    public string SeverityText
    {
        get => (string)GetValue(SeverityTextProperty);
        set => SetValue(SeverityTextProperty, value);
    }
}
