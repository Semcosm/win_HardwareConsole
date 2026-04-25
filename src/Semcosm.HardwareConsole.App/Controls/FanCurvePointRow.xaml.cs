using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class FanCurvePointRow : UserControl
{
    public static readonly DependencyProperty InputTextProperty =
        DependencyProperty.Register(nameof(InputText), typeof(string), typeof(FanCurvePointRow), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty OutputTextProperty =
        DependencyProperty.Register(nameof(OutputText), typeof(string), typeof(FanCurvePointRow), new PropertyMetadata(string.Empty));

    public FanCurvePointRow()
    {
        InitializeComponent();
    }

    public string InputText
    {
        get => (string)GetValue(InputTextProperty);
        set => SetValue(InputTextProperty, value);
    }

    public string OutputText
    {
        get => (string)GetValue(OutputTextProperty);
        set => SetValue(OutputTextProperty, value);
    }
}
