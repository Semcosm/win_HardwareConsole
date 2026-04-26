# Architecture

## Current Layering

```text
Semcosm.HardwareConsole.App
  -> Semcosm.HardwareConsole.Mock
    -> Semcosm.HardwareConsole.Abstractions
```

Responsibilities:

- `App`: WinUI shell, pages, view models, navigation, page composition
- `Abstractions`: shared hardware contracts, descriptors, values, and provider interfaces
- `Mock`: development-time implementations of inventory, plugin registry, and snapshot providers

Current profile shape:

```text
inventory
  -> ProfileDescriptor

runtime
  -> IProfileRuntimeService
    -> active profile
    -> profile preview
    -> mock apply result
```

This intentionally separates "what profiles exist" from "which profile is active and what it would do".

Current navigation shape:

```text
Navigation route providers
  -> composite route registry
  -> route content factories
    -> page factory
    -> navigation service
      -> MainWindow
```

This keeps page registration centralized and makes it possible to add plugin-provided routes later without putting more `switch` logic into the shell.

Current diagnostics shape:

```text
runtime surface
  -> IDiagnosticsSink
    -> DiagnosticsStore
      -> IDiagnosticsProvider
      -> IDiagnosticsSessionController
        -> DiagnosticsViewModel
          -> DiagnosticsPage
```

Current plugin-manifest shape:

```text
plugins/*/plugin.json
  -> PluginManifestLoader
    -> PluginManifestValidator
      -> PluginManifestCatalog
        -> manifest-backed plugin registry
          -> plugin diagnostics + plugins UI
```

Current policy-runtime shape:

```text
descriptor-driven page
  -> IFan/IPower/IScheduler/IThermalPolicyRuntimeService
    -> IPolicyValidator<TPolicy, TValidationResult>
      -> preview result
```

Current route types:

- `BuiltInNavigationRoute`: WinUI pages owned by the app shell
- future `PluginPage` routes: pages hosted from plugin-provided UI surfaces
- future `ExternalPanel` routes: embedded extension panels or out-of-process UI hosts

Current route-content boundary:

- `IRouteContentFactory` is the extension point that maps a `NavigationRoute` to shell content
- `BuiltInPageRouteContentFactory` currently handles `BuiltInNavigationRoute`
- higher `IRouteContentFactory.Priority` wins when multiple factories can create the same route
- plugin page hosting and external panel hosting are intentionally not implemented yet, but now have a clear seam instead of being hidden inside `PageFactory`

Current `NavigationService` scope:

- singleton, single-window shell service
- owns one active `Frame` through `Initialize(Frame frame)`
- must move to a per-window navigation context before multi-window shells, tray popups, or diagnostics windows are introduced

Current route registry behavior:

- built-in routes currently come from `BuiltInNavigationRouteProvider`
- `CompositeNavigationRouteRegistry` merges route providers into the shell-facing registry
- duplicate route tags are detected during merge; first provider wins and duplicates are ignored with a debug diagnostic
- the registry now exposes `RoutesChanged`
- `MainWindow` rebuilds its menu when route metadata changes
- plugin route contribution is not implemented yet, but the registry no longer requires plugins to mutate the built-in route source directly
- the composite registry now unsubscribes from provider events on `Dispose`, but dynamic provider attach/detach for hot-loaded plugins is still a later step

## Direction

The repository is being shaped so that UI code depends on abstractions and data providers rather than device-specific implementations.

Target flow:

```text
real hardware adapter or plugin host
  -> inventory / registry / snapshot provider
    -> profile runtime
    -> view model
      -> WinUI page
```

This keeps the UI stable when mock data is replaced with real hardware integration.

Current PR8 boundary:

- `Dashboard` now reads active profile state from `IProfileRuntimeService`
- `ProfilesPage` previews and applies profiles through the runtime service
- `ProfilePresentationMapper` keeps profile/runtime-to-UI mapping out of `ProfilesViewModel`
- high-risk profiles must be previewed and confirmed before mock apply is enabled
- `Apply Mock Profile` only updates mock runtime state today
- no real hardware write path is implemented yet

Current PR9 boundary:

- `DevicesPage` renders mock inventory from `IHardwareInventoryService`
- sensor rows use `ISensorSnapshotProvider`
- control rows use the active mock profile target as the target-value source
- plugin source hints now come from explicit mock provider ownership fields on `PluginDescriptor`
- no real hardware write path is implemented yet

Current PR10 boundary:

- `FansPage` renders mock fan policies from `IFanPolicyRuntimeService`
- `FanCurvePolicyDescriptor` binds sensors to controls through curve points and timing fields
- `MockFanPolicyValidator` now validates descriptor rules before preview is produced
- preview now returns structured policy-preview data including required sensors, would-set controls, blocked reasons and diagnostics
- fan policies reference the same sensor/control ids already exposed on `DevicesPage`
- no real hardware write path is implemented yet

Current PR13 boundary:

- `PowerPage` renders mock Windows power policies from `IPowerPolicyRuntimeService`
- `PowerPolicyDescriptor` expresses AC/DC plan behavior and would-set CPU/GPU budget controls
- power-policy preview now follows `validate -> preview` through shared validator contracts instead of inline runtime validation
- `SchedulerPage` renders mock process-rule scheduler policies from `ISchedulerPolicyRuntimeService`
- `SchedulerPolicyDescriptor` expresses foreground/background scheduling strategies through process rules and control targets
- scheduler-policy preview now follows `validate -> preview` through shared validator contracts instead of inline runtime validation
- both pages remain preview-only and do not write real Windows power or scheduler state
- both pages emit diagnostics through the same shared diagnostics sink used by the rest of the shell

Current PR11 boundary:

- `ThermalPage` renders mock thermal policy chains from `IThermalPolicyRuntimeService`
- `ThermalPolicyDescriptor` expresses staged threshold actions instead of real thermal-engine state
- `ThermalThresholdActionDescriptor` describes which control would change at which temperature wall
- `MockThermalPolicyValidator` now validates descriptor rules before preview is produced
- thermal preview returns structured required-sensor, would-set-control, blocked-reason and diagnostic data
- thermal chains reuse the same sensor/control inventory already exposed on `DevicesPage`
- no runtime thermal engine or real hardware write path is implemented yet

Current PR12 boundary:

- diagnostics contracts now live in `Semcosm.HardwareConsole.Abstractions`
- `DiagnosticsStore` is the current in-memory sink/provider pair for session diagnostics
- `DiagnosticsStore` now locks its in-memory record list before snapshot reads and writes
- `DiagnosticsPage` renders both latest-per-surface health cards and the session log
- route registry and navigation failures emit route diagnostics
- plugin registry state is snapshotted into diagnostics at app launch and can now be republished through `IPluginRegistry.PluginsChanged`
- profile apply results emit diagnostics through `ProfilesViewModel`
- fan and thermal preview flows emit diagnostics through their page view models
- `DiagnosticsViewModel` now detaches from the singleton diagnostics provider on disposal and uses the UI dispatcher before rebuilding observable collections
- diagnostics now support basic severity/source filtering and clearing the current in-memory session log through `IDiagnosticsSessionController`
- `DiagnosticSeverity` now reserves `Critical` for future hardware-fault and safety-stop paths
- `DiagnosticSource` now already reserves future areas such as `Devices`, `Power`, `Scheduler`, `Service`, `PolicyValidation`, and `HardwareAccess`
- diagnostics currently remain session-scoped; there is no persisted store or external log shipping yet

Current thermal validation boundary:

- validation contracts now live in `Semcosm.HardwareConsole.Abstractions`
- `IPolicyValidator<TPolicy, TValidationResult>` is the shared entry point for policy validation
- `PolicyValidationIssue` and `PolicyValidationSeverity` provide reusable issue semantics
- `MockThermalPolicyValidator` is the current mock implementation of the thermal-policy validator contract
- `InputSensorIds` must not be empty
- `PollIntervalSeconds` must be greater than `0`
- `CooldownSeconds` must be `>= 0`
- `Actions` must not be empty
- each action must reference a declared trigger sensor and an existing control
- trigger thresholds are constrained to a mock-safe `0..120°C` range
- high-risk actions must require confirmation
- thresholds for the same trigger sensor must be non-decreasing across the chain

Current fan validation boundary:

- `FanPolicyValidationResult` now brings fan policies into the same shared validation-result family as thermal, power, and scheduler
- `MockFanPolicyValidator` validates curve presence, input/output selection, temperature sensor kind, sorted curve points, `0..120°C` input range, and `0..100%` output range before preview is produced
- fan preview now follows the same `validate -> preview` shape as the other policy surfaces

Current power and scheduler validation boundary:

- `MockPowerPolicyValidator` and `MockSchedulerPolicyValidator` now follow the same abstraction-layer validator contract as thermal
- power policies validate AC/DC action presence, required sensors, supported controls, target compatibility, and confirmation gates before preview is produced
- scheduler policies validate rule presence, rule metadata, supported controls, target compatibility, and confirmation gates before preview is produced
- runtime preview services now consume validation results instead of duplicating inline validation logic

Current diagnostics session boundary:

- `IDiagnosticsProvider` is now read-only
- `IDiagnosticsSessionController` owns destructive session actions such as clearing the in-memory log
- this keeps page rendering, export/read-only tooling, and future service/plugin surfaces from implicitly receiving log-clear permissions

Current policy-preview convergence note:

- `PolicyRuntimePreview` and `ThermalPolicyPreview` intentionally remain separate because the page models differ
- both previews now carry the same core shape: success state, failure code, required sensors, would-set controls, blocked reasons, diagnostics, and message
- a later shared interface such as `IPolicyPreviewResult` is a likely cleanup step once more policy pages exist

Current diagnostics convergence note:

- diagnostics is intentionally orthogonal to local page-preview models
- fan, thermal, profile, route, and plugin surfaces still expose their own local result contracts to their pages
- `DiagnosticRecord` is the shared cross-cutting shape for global health and historical feedback
- the system-health card list is now configuration-driven instead of being hard-coded directly in the rebuild method
- power and scheduler previews now also emit through the shared diagnostics sink instead of inventing per-page global logging models
- diagnostics dispatch is still synchronous at the store boundary; view models currently marshal back to the UI thread themselves

Current PR15 boundary:

- plugin manifests now load from `plugins/*/plugin.json`
- `PluginManifestDescriptor`, declaration records, and validation results now live in `Semcosm.HardwareConsole.Abstractions`
- `PluginManifestLoader` parses JSON only; it does not execute plugin code
- `PluginManifestValidator` validates schema version, duplicate plugin ids, duplicate control ids, and confirmation requirements for high-risk manifests
- `ManifestBackedPluginRegistry` now maps valid manifests into `PluginDescriptor` for the existing plugin/device UI surfaces
- manifest load/validation outcomes emit diagnostics such as `plugins.manifest.loaded`, `plugins.manifest.invalid`, `plugins.manifest.duplicate_plugin_id`, `plugins.manifest.duplicate_control_id`, and `plugins.manifest.unsupported_schema_version`
- plugin route declarations are parsed but not executed or registered in the shell yet
- no plugin code execution or hardware write path is implemented yet
