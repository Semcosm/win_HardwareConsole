using System.Collections.Generic;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Models;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class ProfilePresentationMapper
{
    private readonly IReadOnlyDictionary<string, ControlDescriptor> _controls;
    private readonly IReadOnlyDictionary<string, DeviceDescriptor> _devices;

    public ProfilePresentationMapper(IHardwareInventoryService hardwareInventoryService)
    {
        _controls = BuildControlMap(hardwareInventoryService.GetControls());
        _devices = BuildDeviceMap(hardwareInventoryService.GetDevices());
    }

    public ProfileCardModel MapProfileCard(
        ProfileDescriptor profile,
        string? activeProfileId,
        bool isApplyEnabled)
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
            ConfirmationText = requiresConfirmation ? "Preview + confirmation required" : string.Empty,
            IsApplyEnabled = isApplyEnabled,
            IsActive = string.Equals(profile.Id, activeProfileId)
        };
    }

    public ProfileActionRowModel MapAction(ProfileControlActionDescriptor action)
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
            ConfirmationText = action.RequiresConfirmation ? "Confirmation-gated action" : string.Empty
        };
    }

    public ProfilePreviewModel MapPreview(ProfilePreview preview)
    {
        return MapPreview(preview.Profile, preview.RequiresConfirmation, preview.Message);
    }

    public ProfilePreviewModel MapApplyResult(
        ProfileDescriptor? profile,
        ProfileApplyResult result)
    {
        if (profile is null)
        {
            return new ProfilePreviewModel
            {
                ShowEmptyState = true,
                EmptyTitle = "Profile unavailable",
                EmptyDescription = result.Message
            };
        }

        return MapPreview(profile, RequiresConfirmation(profile), result.Message);
    }

    public bool RequiresConfirmation(ProfileDescriptor profile)
    {
        if (profile.RiskLevel is HardwareRiskLevel.HardwareWrite
            or HardwareRiskLevel.KernelDriverRequired
            or HardwareRiskLevel.Experimental)
        {
            return true;
        }

        foreach (var action in profile.Actions)
        {
            if (action.RequiresConfirmation)
            {
                return true;
            }
        }

        return false;
    }

    public string GetConfirmationBannerText(ProfileDescriptor profile)
    {
        return $"'{profile.DisplayName}' is still mock-only, but it includes hardware-write or confirmation-gated actions. Review the preview first, then confirm before applying mock runtime state.";
    }

    public string GetConfirmationCheckText(ProfileDescriptor profile)
    {
        return $"I understand '{profile.DisplayName}' only updates mock runtime state right now and does not write real hardware yet.";
    }

    private static ProfilePreviewModel MapPreview(
        ProfileDescriptor? profile,
        bool requiresConfirmation,
        string message)
    {
        if (profile is null)
        {
            return ProfilePreviewModel.CreateEmpty();
        }

        return new ProfilePreviewModel
        {
            ShowEmptyState = false,
            Title = profile.DisplayName,
            Description = profile.Description,
            StatusText = message,
            SourceText = GetProfileKindText(profile.Kind),
            RiskLevel = profile.RiskLevel,
            RequiresConfirmation = requiresConfirmation,
            ConfirmationText = requiresConfirmation ? "Confirmation required" : string.Empty
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
}
