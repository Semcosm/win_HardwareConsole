using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.ViewModels;

public sealed class ProfilesViewModel : INotifyPropertyChanged
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, DeviceDescriptor> _devices;
    private readonly IProfileRuntimeService _profileRuntimeService;
    private ProfileCardModel? _activeProfile;
    private ProfilePreviewModel _preview = ProfilePreviewModel.CreateEmpty();

    public event PropertyChangedEventHandler? PropertyChanged;

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

    public ProfilesViewModel(
        IHardwareInventoryService hardwareInventoryService,
        IProfileRuntimeService profileRuntimeService)
    {
        _profileRuntimeService = profileRuntimeService;
        _controls = BuildControlMap(hardwareInventoryService.GetControls());
        _devices = BuildDeviceMap(hardwareInventoryService.GetDevices());

        RefreshProfiles();

        var activeProfile = _profileRuntimeService.GetActiveProfile();
        if (activeProfile is not null)
        {
            PreviewProfile(activeProfile.Id);
        }
    }

    public void PreviewProfile(string profileId)
    {
        var preview = _profileRuntimeService.PreviewProfile(profileId);
        UpdatePreview(preview.Profile, preview.Actions, preview.RequiresConfirmation, preview.Message);
    }

    public void ApplyProfile(string profileId)
    {
        var applyResult = _profileRuntimeService.ApplyProfile(profileId, ProfileApplyMode.Activate);

        RefreshProfiles();

        UpdatePreview(
            applyResult.ActiveProfile,
            applyResult.WouldSetActions,
            applyResult.RequiresConfirmation,
            applyResult.Message);
    }

    private void RefreshProfiles()
    {
        var activeProfile = _profileRuntimeService.GetActiveProfile();
        var availableProfiles = _profileRuntimeService.GetAvailableProfiles();

        RebuildCards(BuiltInProfiles, FilterProfilesByKind(availableProfiles, ProfileKind.BuiltIn), activeProfile?.Id);
        RebuildCards(CustomProfiles, FilterProfilesByOtherKinds(availableProfiles), activeProfile?.Id);
        ActiveProfile = activeProfile is null ? null : BuildProfileCard(activeProfile, activeProfile.Id);
    }

    private void UpdatePreview(
        ProfileDescriptor? profile,
        IReadOnlyList<ProfileControlActionDescriptor> actions,
        bool requiresConfirmation,
        string message)
    {
        PreviewActions.Clear();

        if (profile is null)
        {
            Preview = ProfilePreviewModel.CreateEmpty();
            return;
        }

        foreach (var action in actions)
        {
            PreviewActions.Add(BuildActionRow(action));
        }

        Preview = new ProfilePreviewModel
        {
            ShowEmptyState = false,
            Title = profile.DisplayName,
            Description = profile.Description,
            StatusText = message,
            SourceText = GetProfileKindText(profile.Kind),
            RiskLevel = profile.RiskLevel,
            RequiresConfirmation = requiresConfirmation,
            ConfirmationText = requiresConfirmation ? "Requires confirmation" : string.Empty
        };
    }

    private void RebuildCards(
        ObservableCollection<ProfileCardModel> target,
        IEnumerable<ProfileDescriptor> profiles,
        string? activeProfileId)
    {
        target.Clear();

        foreach (var profile in profiles)
        {
            target.Add(BuildProfileCard(profile, activeProfileId));
        }
    }

    private ProfileCardModel BuildProfileCard(ProfileDescriptor profile, string? activeProfileId)
    {
        var requiresConfirmation = RequiresConfirmation(profile);

        return new ProfileCardModel
        {
            Id = profile.Id,
            DisplayName = profile.DisplayName,
            Description = profile.Description,
            RiskLevel = profile.RiskLevel,
            SourceText = GetProfileKindText(profile.Kind),
            CapabilityCountText = FormatCount(profile.CapabilityIds.Count, "capability"),
            ActionCountText = FormatCount(profile.Actions.Count, "control target"),
            PolicyCountText = FormatCount(profile.PolicyIds.Count, "policy"),
            RequiresConfirmation = requiresConfirmation,
            ConfirmationText = requiresConfirmation ? "Requires confirmation" : string.Empty,
            IsActive = string.Equals(profile.Id, activeProfileId)
        };
    }

    private ProfileActionRowModel BuildActionRow(ProfileControlActionDescriptor action)
    {
        var controlDisplayName = _controls.TryGetValue(action.ControlId, out var control)
            ? control.DisplayName
            : action.ControlId;

        var deviceDisplayName = control is not null && _devices.TryGetValue(control.DeviceId, out var device)
            ? device.DisplayName
            : "Unknown device";

        return new ProfileActionRowModel
        {
            ControlId = action.ControlId,
            DisplayName = controlDisplayName,
            Subtitle = $"{deviceDisplayName} · {action.ControlId}",
            TargetValue = action.TargetValue.FormattedValue,
            RiskLevel = MapRiskLevel(action.RiskLevel),
            RequiresConfirmation = action.RequiresConfirmation,
            ConfirmationText = action.RequiresConfirmation ? "Confirm" : string.Empty
        };
    }

    private static IReadOnlyDictionary<string, ControlDescriptor> BuildControlMap(
        IReadOnlyList<ControlDescriptor> controls)
    {
        var map = new Dictionary<string, ControlDescriptor>();

        foreach (var control in controls)
        {
            map[control.Id] = control;
        }

        return map;
    }

    private static IReadOnlyDictionary<string, DeviceDescriptor> BuildDeviceMap(
        IReadOnlyList<DeviceDescriptor> devices)
    {
        var map = new Dictionary<string, DeviceDescriptor>();

        foreach (var device in devices)
        {
            map[device.Id] = device;
        }

        return map;
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

    private static bool RequiresConfirmation(ProfileDescriptor profile)
    {
        foreach (var action in profile.Actions)
        {
            if (action.RequiresConfirmation)
            {
                return true;
            }
        }

        return false;
    }

    private static string GetProfileKindText(ProfileKind kind)
    {
        return kind switch
        {
            ProfileKind.BuiltIn => "Built-in",
            ProfileKind.User => "User",
            ProfileKind.DeviceProvided => "Device Provided",
            ProfileKind.PluginProvided => "Plugin Provided",
            _ => "Unknown"
        };
    }

    private static HardwareRiskLevel MapRiskLevel(ControlRiskLevel riskLevel)
    {
        return riskLevel switch
        {
            ControlRiskLevel.SafeControl => HardwareRiskLevel.SafeControl,
            ControlRiskLevel.HardwareWrite => HardwareRiskLevel.HardwareWrite,
            ControlRiskLevel.KernelDriverRequired => HardwareRiskLevel.KernelDriverRequired,
            ControlRiskLevel.Experimental => HardwareRiskLevel.Experimental,
            _ => HardwareRiskLevel.ReadOnly
        };
    }

    private static string FormatCount(int count, string singular)
    {
        return count == 1 ? $"1 {singular}" : $"{count} {singular}s";
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
