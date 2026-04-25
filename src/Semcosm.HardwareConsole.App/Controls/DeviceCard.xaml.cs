using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class DeviceCard : UserControl
{
    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty VendorProperty =
        DependencyProperty.Register(nameof(Vendor), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register(nameof(Model), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CapabilityCountTextProperty =
        DependencyProperty.Register(nameof(CapabilityCountText), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SensorCountTextProperty =
        DependencyProperty.Register(nameof(SensorCountText), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ControlCountTextProperty =
        DependencyProperty.Register(nameof(ControlCountText), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PluginSourceSummaryProperty =
        DependencyProperty.Register(nameof(PluginSourceSummary), typeof(string), typeof(DeviceCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CapabilitiesProperty =
        DependencyProperty.Register(nameof(Capabilities), typeof(IEnumerable<string>), typeof(DeviceCard), new PropertyMetadata(null));

    public static readonly DependencyProperty PluginSourcesProperty =
        DependencyProperty.Register(nameof(PluginSources), typeof(IEnumerable<string>), typeof(DeviceCard), new PropertyMetadata(null));

    public static readonly DependencyProperty SensorsProperty =
        DependencyProperty.Register(nameof(Sensors), typeof(IEnumerable<SensorRowModel>), typeof(DeviceCard), new PropertyMetadata(null));

    public static readonly DependencyProperty ControlsProperty =
        DependencyProperty.Register(nameof(Controls), typeof(IEnumerable<ControlRowModel>), typeof(DeviceCard), new PropertyMetadata(null));

    public DeviceCard()
    {
        InitializeComponent();
    }

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string Vendor
    {
        get => (string)GetValue(VendorProperty);
        set => SetValue(VendorProperty, value);
    }

    public string Model
    {
        get => (string)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    public string CapabilityCountText
    {
        get => (string)GetValue(CapabilityCountTextProperty);
        set => SetValue(CapabilityCountTextProperty, value);
    }

    public string SensorCountText
    {
        get => (string)GetValue(SensorCountTextProperty);
        set => SetValue(SensorCountTextProperty, value);
    }

    public string ControlCountText
    {
        get => (string)GetValue(ControlCountTextProperty);
        set => SetValue(ControlCountTextProperty, value);
    }

    public string PluginSourceSummary
    {
        get => (string)GetValue(PluginSourceSummaryProperty);
        set => SetValue(PluginSourceSummaryProperty, value);
    }

    public IEnumerable<string>? Capabilities
    {
        get => (IEnumerable<string>?)GetValue(CapabilitiesProperty);
        set => SetValue(CapabilitiesProperty, value);
    }

    public IEnumerable<string>? PluginSources
    {
        get => (IEnumerable<string>?)GetValue(PluginSourcesProperty);
        set => SetValue(PluginSourcesProperty, value);
    }

    public IEnumerable<SensorRowModel>? Sensors
    {
        get => (IEnumerable<SensorRowModel>?)GetValue(SensorsProperty);
        set => SetValue(SensorsProperty, value);
    }

    public IEnumerable<ControlRowModel>? Controls
    {
        get => (IEnumerable<ControlRowModel>?)GetValue(ControlsProperty);
        set => SetValue(ControlsProperty, value);
    }
}
