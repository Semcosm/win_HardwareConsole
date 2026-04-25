using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class ProfileCard : UserControl
{
    public static readonly DependencyProperty ProfileIdProperty =
        DependencyProperty.Register(nameof(ProfileId), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DisplayNameProperty =
        DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(ProfileCard), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.Register(nameof(SourceText), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CapabilityCountTextProperty =
        DependencyProperty.Register(nameof(CapabilityCountText), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ActionCountTextProperty =
        DependencyProperty.Register(nameof(ActionCountText), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PolicyCountTextProperty =
        DependencyProperty.Register(nameof(PolicyCountText), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ShowConfirmationProperty =
        DependencyProperty.Register(nameof(ShowConfirmation), typeof(bool), typeof(ProfileCard), new PropertyMetadata(false));

    public static readonly DependencyProperty ConfirmationTextProperty =
        DependencyProperty.Register(nameof(ConfirmationText), typeof(string), typeof(ProfileCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ProfileCard), new PropertyMetadata(false));

    public static readonly DependencyProperty IsApplyEnabledProperty =
        DependencyProperty.Register(nameof(IsApplyEnabled), typeof(bool), typeof(ProfileCard), new PropertyMetadata(true));

    public event RoutedEventHandler? PreviewRequested;
    public event RoutedEventHandler? ApplyRequested;

    public ProfileCard()
    {
        InitializeComponent();
    }

    public string ProfileId
    {
        get => (string)GetValue(ProfileIdProperty);
        set => SetValue(ProfileIdProperty, value);
    }

    public string DisplayName
    {
        get => (string)GetValue(DisplayNameProperty);
        set => SetValue(DisplayNameProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
    }

    public string SourceText
    {
        get => (string)GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    public string CapabilityCountText
    {
        get => (string)GetValue(CapabilityCountTextProperty);
        set => SetValue(CapabilityCountTextProperty, value);
    }

    public string ActionCountText
    {
        get => (string)GetValue(ActionCountTextProperty);
        set => SetValue(ActionCountTextProperty, value);
    }

    public string PolicyCountText
    {
        get => (string)GetValue(PolicyCountTextProperty);
        set => SetValue(PolicyCountTextProperty, value);
    }

    public bool ShowConfirmation
    {
        get => (bool)GetValue(ShowConfirmationProperty);
        set => SetValue(ShowConfirmationProperty, value);
    }

    public string ConfirmationText
    {
        get => (string)GetValue(ConfirmationTextProperty);
        set => SetValue(ConfirmationTextProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsApplyEnabled
    {
        get => (bool)GetValue(IsApplyEnabledProperty);
        set => SetValue(IsApplyEnabledProperty, value);
    }

    private void PreviewButton_Click(object sender, RoutedEventArgs e)
    {
        PreviewRequested?.Invoke(this, e);
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        ApplyRequested?.Invoke(this, e);
    }
}
