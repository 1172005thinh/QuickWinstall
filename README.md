# QuickWinstall

![Version](https://img.shields.io/badge/version-0.5.1-blue)
![License](https://img.shields.io/badge/license-Free%20Open%20Source-green)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![DEV](https://img.shields.io/badge/status-in_development-orange)

Automated Windows 11 installation helper — a small Windows Forms GUI that helps you build and apply unattended installation presets and settings for Windows 11.

QuickWinstall provides a friendly UI around common unattended-install tasks such as language/region, product key input, partitioning presets, user account setup and a handful of post-install options. It generates configuration data and helper artifacts to make unattended deployments easier.

---

## Features

- Visual editor for unattended-install configuration sections:
  - General: edition, product key, architecture
  - Language & Region: system locale, keyboard, locale settings
  - Bypass / compatibility toggles
  - Disk & Partition: partition presets and quick formatting (not fully implemented)
  - User Account: local account / admin options (not fully implemented)
  - OOBE: out-of-box experience options
  - Personalization: simple UI placeholders (not fully implemented)
  - App-level settings and presets (not fully implemented)
- Live preview and simple persistence of presets
- Theme and language (localization) support (English & Vietnamese included)
- Presets export and import (res/presets)
- Extensible config section pattern so new options can be added easily

---

## Project structure

Top-level files and important folders:

- `QuickWinstall.sln`, `QuickWinstall.csproj` — solution and main project
- `src/` — application sources
  - `main/` — main form wiring and top-level app logic
  - `ui/` — designer partial classes and UI-related resources
  - `config/` — modular config-section classes (GeneralConfig, LangRegConfig, ...)
  - `lib/` — shared helpers (ThemeManager, LangManager, IconManager, etc.)
  - `tools/` — support scripts (python helpers for icons/keys)
- `res/` — runtime resources
  - `langs/` — translation JSON files (en-US.json, vi-VN.json)
  - `themes/` — JSON theme files
  - `presets/` — preset/config files and lastConfig.json

Tests are located under `tools/tests/`.

---

## Requirements

- .NET (Windows) — project targets a modern .NET Framework/.NET 6+ depending on the `.csproj`. Check `QuickWinstall.csproj` for the target framework used by your branch.
- Windows OS (GUI) — designed for Windows desktop deployment and testing
- Visual Studio or `dotnet` SDK for building and running

Minimum recommended:

- Windows 10/11 for runtime testing
- .NET SDK matching the project `TargetFramework`

---

## Usage

As an end user:

1. In this repository, navigate to the `Actions` tab, open the latest workflow, and click download artifact `QuickWinstall-win` to get the latest build.
2. Extract the downloaded ZIP file.
3. Run `QuickWinstall.exe`.

As a developer (local run):

1. Clone the repository.
2. Open the solution in Visual Studio or use the `dotnet` CLI.

Build and run with the CLI (powershell):

```powershell
dotnet build QuickWinstall.sln
dotnet run --project QuickWinstall.csproj
```

Or open the solution in Visual Studio and run the project.

---

## How to contribute

- Fork the repository and create a feature branch.
- Make small, focused changes and add tests where possible (there are unit tests in `src/tests/`).
- Keep UI behavior backwards compatible when possible.
- Submit a pull request describing the change, why it was made, and how you tested it.

Developer notes:

- Config sections follow a repeatable pattern (see `src/config/LangRegConfig.cs` for a compact example). New sections should implement the same lifecycle methods (`InitializeUI`, `LoadConfigIntoUI`, `UpdateFromControls`, `ClearControls`, etc.).
- There are a PowerPoint presentation slides in `docs/` covering UI/UX concepts.

---

## Developer tools

- `tools/` contains Python utilities used during development (icon extraction, language key scanning). Use the included scripts to generate or update assets.
- Theme and language JSON files live under `res/` and can be edited to add new translations or color schemes.

---

## Author & License

Author: 1172005thinh (QuickComp.)
I am the original creator and maintainer of this project.
Contributors are welcome! Feel free to fork and submit pull requests.

License: MIT (or change as appropriate). Please include a `LICENSE` file if you change the licensing.

---

## References

- [Schneegans Unattend Generator](https://schneegans.de/windows/unattend-generator/) — project patterns and inspiration
- [Microsoft Documentation](https://docs.microsoft.com/en-us) — configuring Windows unattended installs
- [Microsoft Windows Deployment documentation](https://docs.microsoft.com/en-us/windows/deployment/) — imaging and deployment guidance

---

## Changelog

Only changes since v1.0 should be listed here. The changelog will start at v1.0 when that release is prepared.

- IN DEVELOPMENT

---

## Known issues & TODOs

1. Tooltips in `SettingsForm` may not appear consistently on hover. Possible causes: WinForms tooltip timing, control handle lifecycle, or ToolTipManager usage. Recommended follow-ups: ensure `SetToolTip()` is called after handle creation, tune `InitialDelay`/`AutoPopDelay`.
2. Some toggle/controls may appear pixelated on high-DPI displays. Consider enabling higher-DPI rendering, using anti-aliased drawing, or replacing custom-drawn controls with vector-based icons.
3. Improve unit test coverage for UI logic and theme switch handling.
4. Add more complete implementations for placeholder sections (UserAcc, OOBE, Personal, Disk & Partition, Bypass, App) and wire persistence for `lastConfig.json` autosave.
5. Add more themes and language translations.

---

NOTE: This README.md is created by AI based on project context and recent edits. It might not feel perfectly natural, or actually reflect the project state. I'm trying to improve it continuously. Thank you for understanding.
