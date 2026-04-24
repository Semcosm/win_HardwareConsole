using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class PluginCard : UserControl
{
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(PluginCard), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty VendorProperty =
        DependencyProperty.Register(nameof(Vendor), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty VersionProperty =
        DependencyProperty.Register(nameof(Version), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CapabilitySummaryProperty =
        DependencyProperty.Register(nameof(CapabilitySummary), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty MatchedDeviceSummaryProperty =
        DependencyProperty.Register(nameof(MatchedDeviceSummary), typeof(string), typeof(PluginCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CapabilitiesProperty =
        DependencyProperty.Register(nameof(Capabilities), typeof(IEnumerable<string>), typeof(PluginCard), new PropertyMetadata(null));

    public PluginCard()
    {
        InitializeComponent();
    }

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    public string State
    {
        get => (string)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }

    public string Vendor
    {
        get => (string)GetValue(VendorProperty);
        set => SetValue(VendorProperty, value);
    }

    public string Version
    {
        get => (string)GetValue(VersionProperty);
        set => SetValue(VersionProperty, value);
    }

    public string CapabilitySummary
    {
        get => (string)GetValue(CapabilitySummaryProperty);
        set => SetValue(CapabilitySummaryProperty, value);
    }

    public string MatchedDeviceSummary
    {
        get => (string)GetValue(MatchedDeviceSummaryProperty);
        set => SetValue(MatchedDeviceSummaryProperty, value);
    }

    public IEnumerable<string>? Capabilities
    {
        get => (IEnumerable<string>?)GetValue(CapabilitiesProperty);
        set => SetValue(CapabilitiesProperty, value);
    }
}
