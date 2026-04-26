# Semcosm Hardware Console

WinUI 3 based Windows hardware control console prototype.

The current repository focuses on establishing the app shell and the UI architecture for a future plugin-driven hardware control platform.

This PR-level layout now follows PascalCase naming and a `src/`-based project structure:

```text
src/
  Semcosm.HardwareConsole.App/
  Semcosm.HardwareConsole.Abstractions/
  Semcosm.HardwareConsole.Mock/
```

Current app goals:

- `Dashboard` shows unified hardware state through bound view models instead of hard-coded UI.
- `Diagnostics` now provides a unified system-health and diagnostic-log surface across routes, plugins, profiles, fan policy preview, and thermal validation.
- `Devices` shows mock hardware inventory, capability ownership, sensors, controls and plugin source hints.
- `Fans` now provides a mock fan policy editor with runtime preview, without writing real hardware.
- `Power` now provides mock Windows power-plan and AC/DC budget preview surfaces without writing real hardware.
- `Scheduler` now provides mock process-rule scheduler preview surfaces for EcoQoS, efficiency mode and boost behavior without writing real hardware.
- `Thermal` now provides mock thermal policy-chain preview for staged temperature walls, without writing real hardware.
- `Plugins` shows capability providers, risk level, matched devices, and extension metadata.
- `Profiles` now drives a mock profile runtime with preview/apply flows and updates Dashboard active profile state without writing real hardware.
- Other pages already exist as placeholders for the next phase: `Performance`, `Settings`.

## Current Stack

- .NET 8
- WinUI 3
- Windows App SDK `1.8.260416003`
- Target framework: `net8.0-windows10.0.19041.0`

## Current Structure

```text
docs/
src/
  Semcosm.HardwareConsole.App/
    Models/
    ViewModels/
    Views/
  Semcosm.HardwareConsole.Abstractions/
    IHardwareInventoryService.cs
    IPluginRegistry.cs
    IProfileRuntimeService.cs
    ISensorSnapshotProvider.cs
    PluginDescriptor.cs
    DeviceDescriptor.cs
    SensorDescriptor.cs
    ControlDescriptor.cs
    SensorValue.cs
    ControlValue.cs
    HardwareCapability.cs
  Semcosm.HardwareConsole.Mock/
    Services/
```

Current UI flow:

```text
Mock provider
  -> profile runtime / sensor snapshot
  -> ViewModel
    -> WinUI page
```

Navigation flow:

```text
Navigation route providers
  -> composite route registry
  -> route content factories
    -> page factory
      -> navigation service
        -> MainWindow / NavigationView
```

Diagnostics flow:

```text
runtime surface
  -> IDiagnosticsSink
    -> diagnostics store
      -> IDiagnosticsProvider
        -> DiagnosticsViewModel
          -> DiagnosticsPage
```

Current route model:

- `BuiltInNavigationRoute` keeps `Type PageType` and a typed `Symbol` icon for app-owned WinUI pages
- base `NavigationRoute` now carries route kind and provider identity instead of assuming every route is a direct `Type`
- plugin-hosted and external-panel routes are reserved for the next stage

This is the intended direction for the real app as well:

```text
Hardware plugin / platform adapter
  -> abstraction-backed service layer
    -> view model
      -> WinUI page
```

## Implemented Pages

### Dashboard

Backed by:

- `src/Semcosm.HardwareConsole.App/Models/MetricCardModel.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IHardwareInventoryService.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IProfileRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Abstractions/ISensorSnapshotProvider.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockHardwareInventoryService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockProfileRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockSensorSnapshotProvider.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/DashboardViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/DashboardPage.xaml`

Shows mock summary and control-state cards for:

- CPU
- GPU
- Active Profile
- Fans
- Power
- Thermal

The `Active Profile` card now comes from `IProfileRuntimeService`, not from mock `sensor.profile.*` values.

### Plugins

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/PluginDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IPluginRegistry.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockPluginRegistry.cs`
- `src/Semcosm.HardwareConsole.App/Models/PluginManifestModel.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/PluginsViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/PluginsPage.xaml`

Shows mock installed plugins with:

- display name
- plugin id
- state
- risk level
- capabilities
- matched devices

Current mock plugins:

- `Windows Power Plugin`
- `NVIDIA NVAPI Plugin`
- `Mechrevo GM6PX0X Platform Plugin`

The current flow is:

```text
PluginDescriptor
  -> PluginManifestModel
    -> PluginsViewModel
      -> PluginsPage
```

Dashboard now follows a similar mapping pattern:

```text
DeviceDescriptor + SensorValue
  -> MetricCardModel
    -> DashboardViewModel
      -> DashboardPage
```

Active profile state now follows:

```text
ProfileDescriptor
  -> IProfileRuntimeService
    -> DashboardViewModel
      -> DashboardPage
```

### Devices

Backed by:

- `src/Semcosm.HardwareConsole.App/Services/DevicePresentationMapper.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/DevicesViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/DevicesPage.xaml`

Shows:

- mock devices grouped as inventory cards
- capability ownership per device
- sensor rows from the current mock snapshot provider
- control rows with risk level, unit, mock target value and owning device
- plugin source hints from explicit mock provider ownership, not string heuristics

Current control values on `Devices` are labeled as mock targets from the active profile runtime, not as real hardware runtime values.

### Fans

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/FanCurvePoint.cs`
- `src/Semcosm.HardwareConsole.Abstractions/FanCurvePolicyDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/PolicyRuntimePreview.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.App/Services/FanPolicyPresentationMapper.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/FansViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/FansPage.xaml`

Shows:

- CPU, GPU and platform fan policies
- selectable input sensors such as `CPU Package Temp`, `GPU Temp` and `Max(CPU, GPU)`
- selectable output controls such as `CPU Fan PWM`, `GPU Fan PWM` and `Fan Curve`
- curve points, hysteresis, ramp-up and ramp-down mock fields
- runtime preview that only shows would-set policy behavior

`Fans` currently previews mock policy behavior only. It does not write real fan controller state. The preview contract is now structured around required sensors, would-set controls, blocked reasons and diagnostics instead of relying on summary strings alone.

### Power

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/PowerPolicyActionDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/PowerPolicyDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/PowerPolicyPreview.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.App/Services/PowerPolicyPresentationMapper.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/PowerViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/PowerPage.xaml`

Shows:

- mock Windows power-plan behavior
- AC/DC transition summaries
- CPU PL1 / PL2 mock control targets
- GPU power-limit mock control targets
- preview-only action rows for power plan and budget changes

`Power` currently previews mock policy behavior only. It does not write real Windows power-plan or hardware power-limit state.

### Scheduler

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/SchedulerPolicyActionDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/SchedulerRuleDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/SchedulerPolicyDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/SchedulerPolicyPreview.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.App/Services/SchedulerPolicyPresentationMapper.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/SchedulerViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/SchedulerPage.xaml`

Shows:

- process-rule based mock scheduler policies
- foreground boost behavior
- background throttling behavior
- EcoQoS and efficiency-mode mock targets
- preview-only rule rows for process match and scheduler intent

`Scheduler` currently previews mock policy behavior only. It does not write real Windows scheduler or process-policy state.

### Thermal

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/ThermalThresholdActionDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/ThermalPolicyDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/ThermalPolicyPreview.cs`
- `src/Semcosm.HardwareConsole.Abstractions/ThermalPolicyFailureCode.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockPolicyRuntimeService.cs`
- `src/Semcosm.HardwareConsole.App/Services/ThermalPolicyPresentationMapper.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/ThermalViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/ThermalPage.xaml`

Shows:

- built-in mock thermal chains for `Default`, `Quiet`, and `Turbo` thermal policy behavior
- staged threshold actions such as fan escalation, CPU/GPU power reduction, profile fallback, and background `EcoQoS`
- preview-only thermal chain diagnostics, required sensors, would-set controls, and failure codes

`Thermal` currently previews mock policy-chain behavior only. It does not start a runtime thermal engine or write real hardware state.
Thermal preview now passes through a dedicated mock validation layer before runtime preview is produced, so invalid thresholds, missing actions, missing sensors/controls, and confirmation gaps surface as structured failure results instead of ad-hoc runtime checks.
That validation contract is now anchored in `Semcosm.HardwareConsole.Abstractions` through generic policy-validator interfaces and shared validation issue types, while `Mock` only provides the current implementation.

### Profiles

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/ProfileDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/ProfileControlActionDescriptor.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IProfileRuntimeService.cs`
- `src/Semcosm.HardwareConsole.Mock/Services/MockProfileRuntimeService.cs`
- `src/Semcosm.HardwareConsole.App/Services/ProfilePresentationMapper.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/ProfilesViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/ProfilesPage.xaml`

Shows:

- current active profile from runtime state
- built-in and custom/mock provided profiles
- preview of the controls a profile would target
- structured mock apply results for future diagnostics and failure handling
- preview + explicit confirmation gates for high-risk mock profiles before apply is enabled
- mock apply behavior that updates runtime state without writing real hardware

Current profile runtime shape:

```text
ProfileDescriptor
  -> ProfileControlActionDescriptor
    -> IProfileRuntimeService
      -> ProfilesViewModel
        -> ProfilesPage
```

### Diagnostics

Backed by:

- `src/Semcosm.HardwareConsole.Abstractions/DiagnosticSeverity.cs`
- `src/Semcosm.HardwareConsole.Abstractions/DiagnosticSource.cs`
- `src/Semcosm.HardwareConsole.Abstractions/DiagnosticRecord.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IDiagnosticsSink.cs`
- `src/Semcosm.HardwareConsole.Abstractions/IDiagnosticsProvider.cs`
- `src/Semcosm.HardwareConsole.App/Services/DiagnosticsStore.cs`
- `src/Semcosm.HardwareConsole.App/Services/PluginDiagnosticsReporter.cs`
- `src/Semcosm.HardwareConsole.App/ViewModels/DiagnosticsViewModel.cs`
- `src/Semcosm.HardwareConsole.App/Views/DiagnosticsPage.xaml`

Shows:

- system-health cards for `Routes`, `Plugins`, `Profiles`, `Fans`, and `Thermal`
- historical diagnostic log rows with severity, source, code, message, related id, and timestamp
- route diagnostics for duplicate tags, unresolved routes, and navigation failures
- plugin-state diagnostics for mocked, failed, blocked, or unsupported providers
- profile apply diagnostics, fan preview diagnostics, and thermal validation / preview diagnostics in one shared surface

Diagnostics is currently session-scoped and mock-driven. It does not collect hardware telemetry logs or real crash dumps.
The current diagnostics taxonomy already reserves additional sources such as `Devices`, `Power`, `Scheduler`, `Service`, `PolicyValidation`, and `HardwareAccess`, even though the first active health cards still focus on routes, plugins, profiles, fans, and thermal.

## Why This Shape

The goal is to avoid tying the UI directly to a specific laptop brand, EC interface, or GPU vendor. Instead, device and control capabilities should come from plugins or adapters, while pages render from view models and capability metadata.

That keeps the UI stable when the backend evolves from mock data to real hardware access. The same rule now applies to navigation: the shell reads built-in routes from a registry instead of hard-coding page switches in `MainWindow`.

Profiles now follow the same separation: inventory still describes what profiles exist, `IProfileRuntimeService` owns which profile is active and what a profile would do, and `ProfilePresentationMapper` maps profile/runtime contracts into the UI models used by the page.

Thermal policies now follow the same pattern as fan policies: the page binds descriptor-driven mock chains from `IPolicyRuntimeService`, while preview returns structured thermal-policy diagnostics instead of raw summary strings.

Power and scheduler policies now follow the same descriptor-driven pattern as fans and thermal: the pages bind mock policy descriptors from `IPolicyRuntimeService`, while preview returns structured would-set controls, required sensors, blocked reasons and diagnostics without performing real writes.

Diagnostics now follows the same pattern: runtime surfaces report `DiagnosticRecord` entries through `IDiagnosticsSink`, while the page only renders the current aggregate state and session log through `IDiagnosticsProvider`.
The diagnostics store now guards its in-memory record list with locking, while `DiagnosticsViewModel` marshals refresh work back onto the UI dispatcher before rebuilding observable collections.

## Run

Open `Semcosm.HardwareConsole.slnx` in Visual Studio and run the WinUI project.

If your environment has the .NET SDK installed, you can also use:

```bash
dotnet build Semcosm.HardwareConsole.slnx
```

## Navigation

Built-in navigation is now table-driven through:

- `src/Semcosm.HardwareConsole.App/Services/NavigationRoute.cs`
- `src/Semcosm.HardwareConsole.App/Services/NavigationRouteKind.cs`
- `src/Semcosm.HardwareConsole.App/Services/BuiltInNavigationRoute.cs`
- `src/Semcosm.HardwareConsole.App/Services/INavigationRouteProvider.cs`
- `src/Semcosm.HardwareConsole.App/Services/BuiltInNavigationRouteProvider.cs`
- `src/Semcosm.HardwareConsole.App/Services/INavigationRouteRegistry.cs`
- `src/Semcosm.HardwareConsole.App/Services/CompositeNavigationRouteRegistry.cs`
- `src/Semcosm.HardwareConsole.App/Services/IRouteContentFactory.cs`
- `src/Semcosm.HardwareConsole.App/Services/BuiltInPageRouteContentFactory.cs`
- `src/Semcosm.HardwareConsole.App/Services/PageFactory.cs`
- `src/Semcosm.HardwareConsole.App/Services/NavigationService.cs`

This keeps `MainWindow` free of page-type switches and makes room for a later plugin-provided route source. The shell now reads routes through a composite registry, so plugin routes can be added as separate providers instead of mutating the built-in route source. The registry still exposes `RoutesChanged` so the shell can rebuild menu items when route metadata changes.

`PageFactory` no longer hard-codes route-kind handling directly. It now delegates route resolution through `IRouteContentFactory`, with only the built-in page implementation registered today. Plugin page hosts and external panel hosts are deliberately left for later.

When multiple route-content factories can handle the same route, higher `Priority` wins. Current registrations only include the built-in implementation, but the precedence rule is now explicit.

`CompositeNavigationRouteRegistry` validates merged routes and ignores duplicate tags after the first provider wins, emitting a debug diagnostic so route collisions do not fail silently.

`NavigationService` is still a singleton single-window shell service. That is intentional for the current app shape, but it needs to move to a per-window navigation context before multi-window UI is introduced.

The composite registry unsubscribes from provider events on disposal. That is sufficient for the current singleton shell, but hot-loaded plugin providers are still a future lifecycle problem to solve explicitly.

## Publish Notes

`PublishTrimmed` is currently forced to `false`.

That is intentional while the project is still using WinUI, XAML, DI, and future plugin-loading paths that are sensitive to trimming. Release trimming should only come back after publish validation and plugin-loading tests exist.

## Profile Runtime Notes

`Profiles` currently uses a mock runtime only.

- `Preview` shows which controls a profile intends to target
- high-risk profiles must be previewed and explicitly confirmed before `Apply Mock Profile` is enabled
- `Apply Mock Profile` updates mock runtime state and Dashboard active profile state
- no real hardware write is performed yet

## Thermal Policy Notes

`Thermal` currently uses mock preview only.

- threshold actions describe what control changes would happen as temperature walls are crossed
- preview shows required sensors, would-set controls, blocked reasons, and diagnostics
- no runtime thermal engine or real hardware write path is implemented yet

## Diagnostics Notes

`Diagnostics` currently uses an in-memory session store only.

- route registry, navigation failures, plugin states, profile apply results, fan previews, power previews, scheduler previews, and thermal previews all emit `DiagnosticRecord`
- `IPluginRegistry` now exposes `PluginsChanged`, and the current reporter republishes plugin-state diagnostics when registry state changes
- the page shows both latest-per-surface health and the historical session log
- `DiagnosticsViewModel` detaches from the singleton diagnostics provider when the page unloads, so transient view models are not held alive by event subscriptions
- diagnostics severity now reserves `Critical` for future hardware-fault and safety-stop scenarios
- diagnostics are mock/runtime feedback only; there is no persistent log store yet

## Next Suggested Steps

- Introduce real plugin manifest loading
- Replace mock providers with real inventory, plugin registry and snapshot providers
- Introduce a real profile runtime that writes validated control changes instead of mock state updates
- Introduce a real thermal runtime that can evaluate staged thresholds against validated hardware data
- Add a plugin route provider that can contribute pages to the navigation registry
