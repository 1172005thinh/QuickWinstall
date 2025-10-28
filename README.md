# QuickWinstall Project - Automated Windows 11 Installation

QuickWinstall is a Windows Forms application designed to streamline and automate the installation process of Windows 11. It provides users with an intuitive interface to configure installation settings, manage language and region preferences, and handle user accounts efficiently.

## Implementation Notes

### Tooltip System

Tooltips have been implemented in SettingsForm using the ToolTipManager singleton:

- Tooltips are initialized in `SettingsForm.Designer.cs` during `InitializeComponent()` method
- All control tooltips use language keys (e.g., "settingsForm.tooltips.save")
- Language files (`res/langs/en-US.json`, `vi-VN.json`) contain tooltip translations
- **Known Issue**: Tooltips may not appear consistently. This might be due to:
  - Windows Forms tooltip display timing/threading issues
  - Control focus/hover state conflicts
  - ToolTipManager singleton lifecycle management
  - Need to investigate tooltip auto-popup delay settings

Contributors: If tooltips don't show on hover, check:

1. ToolTipManager is properly initialized before setting tooltips
2. Control handles are created before SetToolTip() is called
3. Consider adding explicit `AutoPopDelay`, `InitialDelay`, and `ReshowDelay` settings
4. Verify tooltip text is not empty (check language key resolution)
