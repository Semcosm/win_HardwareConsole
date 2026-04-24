using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class RiskBadge : UserControl
{
    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(
            nameof(RiskLevel),
            typeof(HardwareRiskLevel),
            typeof(RiskBadge),
            new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public RiskBadge()
    {
        InitializeComponent();
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }

    public string Text => RiskLevel switch
    {
        HardwareRiskLevel.ReadOnly => "Read Only",
        HardwareRiskLevel.SafeControl => "Safe Control",
        HardwareRiskLevel.HardwareWrite => "Hardware Write",
        HardwareRiskLevel.KernelDriverRequired => "Kernel Driver",
        HardwareRiskLevel.Experimental => "Experimental",
        _ => "Unknown"
    };

    public Brush BadgeBackground => new SolidColorBrush(RiskLevel switch
    {
        HardwareRiskLevel.ReadOnly => ColorHelper.FromArgb(32, 90, 160, 255),
        HardwareRiskLevel.SafeControl => ColorHelper.FromArgb(40, 38, 171, 108),
        HardwareRiskLevel.HardwareWrite => ColorHelper.FromArgb(40, 255, 176, 32),
        HardwareRiskLevel.KernelDriverRequired => ColorHelper.FromArgb(44, 232, 72, 85),
        HardwareRiskLevel.Experimental => ColorHelper.FromArgb(40, 184, 118, 255),
        _ => ColorHelper.FromArgb(24, 255, 255, 255)
    });
}
