using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class CapabilityChip : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(CapabilityChip),
            new PropertyMetadata(string.Empty));

    public CapabilityChip()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
