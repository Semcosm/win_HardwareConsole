using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;
using Semcosm.HardwareConsole.App.Services;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class ProfilesViewModel : INotifyPropertyChanged
{
    private readonly IProfileRuntimeService _profileRuntimeService;
    private readonly ProfilePresentationMapper _profilePresentationMapper;
    private ProfileCardModel? _activeProfile;
    private string _confirmationBannerText = string.Empty;
    private string _confirmationCheckText = string.Empty;
    private bool _isApplyConfirmed;
    private ProfilePreviewModel _preview = ProfilePreviewModel.CreateEmpty();
    private string? _previewProfileId;
    private bool _showConfirmationGate;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ProfilesViewModel(
        IProfileRuntimeService profileRuntimeService,
        ProfilePresentationMapper profilePresentationMapper)
    {
        _profileRuntimeService = profileRuntimeService;
        _profilePresentationMapper = profilePresentationMapper;

        RefreshProfiles();

        var activeProfile = _profileRuntimeService.GetActiveProfile();
        if (activeProfile is not null)
        {
            PreviewProfile(activeProfile.Id);
        }
    }

    public ObservableCollection<ProfileCardModel> BuiltInProfiles { get; } = new();
    public ObservableCollection<ProfileCardModel> CustomProfiles { get; } = new();
    public ObservableCollection<ProfileActionRowModel> PreviewActions { get; } = new();

    public ProfileCardModel? ActiveProfile
    {
        get => _activeProfile;
        private set => SetProperty(ref _activeProfile, value);
    }

    public ProfilePreviewModel Preview
    {
        get => _preview;
        private set => SetProperty(ref _preview, value);
    }

    public bool ShowConfirmationGate
    {
        get => _showConfirmationGate;
        private set => SetProperty(ref _showConfirmationGate, value);
    }

    public string ConfirmationBannerText
    {
        get => _confirmationBannerText;
        private set => SetProperty(ref _confirmationBannerText, value);
    }

    public string ConfirmationCheckText
    {
        get => _confirmationCheckText;
        private set => SetProperty(ref _confirmationCheckText, value);
    }

    public bool IsApplyConfirmed
    {
        get => _isApplyConfirmed;
        set
        {
            if (SetProperty(ref _isApplyConfirmed, value))
            {
                RefreshProfiles();
            }
        }
    }

    public void PreviewProfile(string profileId)
    {
        var preview = _profileRuntimeService.PreviewProfile(profileId);
        _previewProfileId = preview.Profile?.Id;
        SetApplyConfirmed(false, refreshProfiles: false);

        RebuildPreviewActions(preview.Actions);
        Preview = _profilePresentationMapper.MapPreview(preview);
        UpdateConfirmationState(preview.Profile);
        RefreshProfiles();
    }

    public void ApplyProfile(string profileId)
    {
        var profile = FindProfile(profileId);
        var requiresConfirmation = profile is not null && _profilePresentationMapper.RequiresConfirmation(profile);
        var useConfirmedMode = requiresConfirmation
            && IsApplyConfirmed
            && string.Equals(_previewProfileId, profileId);

        var applyResult = _profileRuntimeService.ApplyProfile(
            profileId,
            useConfirmedMode ? ProfileApplyMode.ActivateConfirmed : ProfileApplyMode.Activate);

        _previewProfileId = profileId;
        RefreshProfiles();

        var previewActions = applyResult.BlockedActions.Count > 0
            ? applyResult.BlockedActions
            : applyResult.WouldSetActions;

        RebuildPreviewActions(previewActions);
        Preview = _profilePresentationMapper.MapApplyResult(profile, applyResult);

        if (applyResult.Success)
        {
            SetApplyConfirmed(false, refreshProfiles: false);
            UpdateConfirmationState(null);
        }
        else if (applyResult.FailureCode == ProfileApplyFailureCode.ConfirmationRequired)
        {
            SetApplyConfirmed(false, refreshProfiles: false);
            UpdateConfirmationState(profile);
        }
        else
        {
            SetApplyConfirmed(false, refreshProfiles: false);
            UpdateConfirmationState(null);
        }

    }

    private void RefreshProfiles()
    {
        var activeProfile = _profileRuntimeService.GetActiveProfile();
        var availableProfiles = _profileRuntimeService.GetAvailableProfiles();

        RebuildCards(BuiltInProfiles, FilterProfilesByKind(availableProfiles, ProfileKind.BuiltIn), activeProfile?.Id);
        RebuildCards(CustomProfiles, FilterProfilesByOtherKinds(availableProfiles), activeProfile?.Id);
        ActiveProfile = activeProfile is null
            ? null
            : _profilePresentationMapper.MapProfileCard(activeProfile, activeProfile.Id, IsApplyEnabled(activeProfile));
    }

    private void RebuildCards(
        ObservableCollection<ProfileCardModel> target,
        IEnumerable<ProfileDescriptor> profiles,
        string? activeProfileId)
    {
        target.Clear();

        foreach (var profile in profiles)
        {
            target.Add(_profilePresentationMapper.MapProfileCard(profile, activeProfileId, IsApplyEnabled(profile)));
        }
    }

    private void RebuildPreviewActions(IReadOnlyList<ProfileControlActionDescriptor> actions)
    {
        PreviewActions.Clear();

        foreach (var action in actions)
        {
            PreviewActions.Add(_profilePresentationMapper.MapAction(action));
        }
    }

    private void UpdateConfirmationState(ProfileDescriptor? profile)
    {
        if (profile is not null && _profilePresentationMapper.RequiresConfirmation(profile))
        {
            ShowConfirmationGate = true;
            ConfirmationBannerText = _profilePresentationMapper.GetConfirmationBannerText(profile);
            ConfirmationCheckText = _profilePresentationMapper.GetConfirmationCheckText(profile);
            return;
        }

        ShowConfirmationGate = false;
        ConfirmationBannerText = string.Empty;
        ConfirmationCheckText = string.Empty;
    }

    private bool IsApplyEnabled(ProfileDescriptor profile)
    {
        if (!_profilePresentationMapper.RequiresConfirmation(profile))
        {
            return true;
        }

        return string.Equals(_previewProfileId, profile.Id) && IsApplyConfirmed;
    }

    private ProfileDescriptor? FindProfile(string profileId)
    {
        foreach (var profile in _profileRuntimeService.GetAvailableProfiles())
        {
            if (string.Equals(profile.Id, profileId))
            {
                return profile;
            }
        }

        return null;
    }

    private static IEnumerable<ProfileDescriptor> FilterProfilesByKind(
        IReadOnlyList<ProfileDescriptor> profiles,
        ProfileKind kind)
    {
        foreach (var profile in profiles)
        {
            if (profile.Kind == kind)
            {
                yield return profile;
            }
        }
    }

    private static IEnumerable<ProfileDescriptor> FilterProfilesByOtherKinds(
        IReadOnlyList<ProfileDescriptor> profiles)
    {
        foreach (var profile in profiles)
        {
            if (profile.Kind != ProfileKind.BuiltIn)
            {
                yield return profile;
            }
        }
    }

    private void SetApplyConfirmed(bool value, bool refreshProfiles)
    {
        if (SetProperty(ref _isApplyConfirmed, value, nameof(IsApplyConfirmed)) && refreshProfiles)
        {
            RefreshProfiles();
        }
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
