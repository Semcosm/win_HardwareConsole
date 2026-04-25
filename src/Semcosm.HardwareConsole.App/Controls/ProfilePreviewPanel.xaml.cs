using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Controls;

public sealed partial class ProfilePreviewPanel : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SourceTextProperty =
        DependencyProperty.Register(nameof(SourceText), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RiskLevelProperty =
        DependencyProperty.Register(nameof(RiskLevel), typeof(HardwareRiskLevel), typeof(ProfilePreviewPanel), new PropertyMetadata(HardwareRiskLevel.ReadOnly));

    public static readonly DependencyProperty ShowConfirmationProperty =
        DependencyProperty.Register(nameof(ShowConfirmation), typeof(bool), typeof(ProfilePreviewPanel), new PropertyMetadata(false));

    public static readonly DependencyProperty ConfirmationTextProperty =
        DependencyProperty.Register(nameof(ConfirmationText), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ShowEmptyStateProperty =
        DependencyProperty.Register(
            nameof(ShowEmptyState),
            typeof(bool),
            typeof(ProfilePreviewPanel),
            new PropertyMetadata(true, OnShowEmptyStateChanged));

    public static readonly DependencyProperty EmptyTitleProperty =
        DependencyProperty.Register(nameof(EmptyTitle), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata("No Profile Preview"));

    public static readonly DependencyProperty EmptyDescriptionProperty =
        DependencyProperty.Register(nameof(EmptyDescription), typeof(string), typeof(ProfilePreviewPanel), new PropertyMetadata("Select Preview on any profile to inspect the control actions it would apply."));

    public static readonly DependencyProperty ActionsProperty =
        DependencyProperty.Register(nameof(Actions), typeof(IEnumerable<ProfileActionRowModel>), typeof(ProfilePreviewPanel), new PropertyMetadata(null));

    public ProfilePreviewPanel()
    {
        InitializeComponent();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public string SourceText
    {
        get => (string)GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    public HardwareRiskLevel RiskLevel
    {
        get => (HardwareRiskLevel)GetValue(RiskLevelProperty);
        set => SetValue(RiskLevelProperty, value);
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

    public bool ShowEmptyState
    {
        get => (bool)GetValue(ShowEmptyStateProperty);
        set => SetValue(ShowEmptyStateProperty, value);
    }

    public string EmptyTitle
    {
        get => (string)GetValue(EmptyTitleProperty);
        set => SetValue(EmptyTitleProperty, value);
    }

    public string EmptyDescription
    {
        get => (string)GetValue(EmptyDescriptionProperty);
        set => SetValue(EmptyDescriptionProperty, value);
    }

    public IEnumerable<ProfileActionRowModel>? Actions
    {
        get => (IEnumerable<ProfileActionRowModel>?)GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    public bool ShowContent => !ShowEmptyState;

    private static void OnShowEmptyStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProfilePreviewPanel panel)
        {
            panel.Bindings.Update();
        }
    }
}
