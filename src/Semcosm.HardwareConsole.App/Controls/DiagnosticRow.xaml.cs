using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class DiagnosticRow : UserControl
{
    public static readonly DependencyProperty SeverityTextProperty =
        DependencyProperty.Register(nameof(SeverityText), typeof(string), typeof(DiagnosticRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.Register(nameof(SourceText), typeof(string), typeof(DiagnosticRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CodeProperty =
        DependencyProperty.Register(nameof(Code), typeof(string), typeof(DiagnosticRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(string), typeof(DiagnosticRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RelatedIdProperty =
        DependencyProperty.Register(nameof(RelatedId), typeof(string), typeof(DiagnosticRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TimestampTextProperty =
        DependencyProperty.Register(nameof(TimestampText), typeof(string), typeof(DiagnosticRow), new PropertyMetadata(string.Empty));

    public DiagnosticRow()
    {
        InitializeComponent();
    }

    public string SeverityText
    {
        get => (string)GetValue(SeverityTextProperty);
        set => SetValue(SeverityTextProperty, value);
    }

    public string SourceText
    {
        get => (string)GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string RelatedId
    {
        get => (string)GetValue(RelatedIdProperty);
        set => SetValue(RelatedIdProperty, value);
    }

    public string TimestampText
    {
        get => (string)GetValue(TimestampTextProperty);
        set => SetValue(TimestampTextProperty, value);
    }
}
