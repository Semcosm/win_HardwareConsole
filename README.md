# Semcosm Hardware Console

WinUI 3 based Windows hardware control console prototype.

The current repository focuses on establishing the app shell and the UI architecture for a future plugin-driven hardware control platform:

- `Dashboard` shows unified hardware state through bound view models instead of hard-coded UI.
- `Plugins` shows capability providers, risk level, matched devices, and extension metadata.
- Other pages already exist as placeholders for the next phase: `Performance`, `Fans`, `Power`, `Thermal`, `Scheduler`, `Devices`, `Profiles`, `Diagnostics`, `Settings`.

## Current Stack

- .NET 8
- WinUI 3
- Windows App SDK `1.8.260416003`
- Target framework: `net8.0-windows10.0.19041.0`

## Current Structure

```text
Semcosm.HardwareConsole.app/
  Models/
  Services/
  ViewModels/
  Views/
```

Current UI flow:

```text
MockHardwareService
  -> ViewModel
    -> WinUI Page
```

This is the intended direction for the real app as well:

```text
Hardware plugin / platform adapter
  -> service layer
    -> view model
      -> WinUI page
```

## Implemented Pages

### Dashboard

Backed by:

- `Models/MetricCardModel.cs`
- `Services/MockHardwareService.cs`
- `ViewModels/DashboardViewModel.cs`
- `Views/DashboardPage.xaml`

Shows mock summary and control-state cards for:

- CPU
- GPU
- Active Profile
- Fans
- Power
- Thermal

### Plugins

Backed by:

- `Models/PluginManifestModel.cs`
- `Services/MockHardwareService.cs`
- `ViewModels/PluginsViewModel.cs`
- `Views/PluginsPage.xaml`

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

## Why This Shape

The goal is to avoid tying the UI directly to a specific laptop brand, EC interface, or GPU vendor. Instead, device and control capabilities should come from plugins or adapters, while pages render from view models and capability metadata.

That keeps the UI stable when the backend evolves from mock data to real hardware access.

## Run

Open `Semcosm.HardwareConsole.app/Semcosm.HardwareConsole.app.slnx` in Visual Studio and run the WinUI project.

If your environment has the .NET SDK installed, you can also use:

```bash
dotnet build Semcosm.HardwareConsole.app/Semcosm.HardwareConsole.app.csproj
```

## Next Suggested Steps

- Rename the project namespace from `Semcosm.HardwareConsole.app` to `Semcosm.HardwareConsole.App`
- Build the `Profiles` page with the same `Model + Service + ViewModel + Binding` pattern
- Introduce real plugin manifest loading
- Split mock services behind interfaces
- Add reusable card controls and shared styles
