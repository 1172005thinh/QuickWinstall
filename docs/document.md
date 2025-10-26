# QuickWinstall Project Documentation

## Introduction

This project aims to simplify the Windows 11 installation process through automation and customization.
The application generates an unattended answer file (autounattend.xml) based on user pre-configurations.
Pre-configurations:

- General Configurations --> Config basic Windows installation parameters, such as Windows Edition, Product Key, CPU Architecture.
- Language & Region Configurations --> Set up System Locale, User Locale, Windows UI Language, Keyboard Layout, Time Zone.
- Bypass Windows 11 Hardware Checks --> Option to bypass TPM, Secure Boot, RAM, and Storage requirements.
- Disk & Partition Configurations --> Define disk partitioning schemes, format options, and drive letter assignments.
- User Account Configurations --> Create local or Microsoft user accounts, set passwords, and configure auto-login.
- OOBE Configurations --> Customize Out-Of-Box Experience settings, such as skipping Cortana, OneDrive setup, and privacy settings.
- Personal Configurations --> Configure personalize settings, set up themes, wallpapers, and user preferences.
- App Configurations --> Specify applications to be installed, uninstalled, including default apps and optional features.

## Documents for Development

### Project's Structure

``` plaintext
QuickWinstall/
├── .gitignore
├── .vscode/
│   ├── extensions.json
│   ├── launch.json
│   └── tasks.json
├── res/
│   ├── icons/
│   │   ├── pngs/
│   │   │   └──   **.png
│   │   └── *.ico
│   ├── images/
│   │   └── *.png
│   ├── langs/
│   │   ├── en-US.json
│   │   └── vi-VN.json
│   ├── presets/
│   │   ├── common.json
│   │   ├── developer.json
│   │   └── lastConfig.json
│   └── themes/
│       ├── dark.json
│       └── light.json
├── src/
│   ├── lib/
│   │   ├── ConfigValues.cs
│   │   ├── IconManager.cs
│   │   ├── LangHelper.cs
│   │   ├── LangManager.cs
│   │   ├── PresetsManager.cs
│   │   ├── SettingsManager.cs
│   │   ├── StatusManager.cs
│   │   ├── ThemeManager.cs
│   │   ├── ToolTipManager.cs
│   │   ├── UIValues.cs
│   │   ├── XMLGenerator.cs
│   │   └── template.xml
│   ├── main/
│   │   ├── AboutForm.cs
│   │   ├── HelpForm.cs
│   │   ├── MainForm.cs
│   │   ├── NewPresetForm.cs
│   │   ├── PresetsForm.cs
│   │   ├── SettingsForm.cs
│   │   └── Program.cs
│   ├── ui/
│   │   ├── AboutForm.Designer.cs
│   │   ├── HelpForm.Designer.cs
│   │   ├── MainForm.Designer.cs
│   │   ├── NewPresetForm.Designer.cs
│   │   ├── PresetsForm.Designer.cs
│   │   └── SettingsForm.Designer.cs
│   └── config/
│       ├── AppConfig.cs
│       ├── BypassConfig.cs
│       ├── DiskPartConfig.cs
│       ├── GeneralConfig.cs
│       ├── LangRegConfig.cs
│       ├── OOBEConfig.cs
│       ├── PersonalConfig.cs
│       ├── UserAccConfig.cs
│       └── empty.json
├── tools/
│   ├── findIcons.py
│   ├── findLangKeys.py
│   ├── png2ico.py
│   └── sortLangKeys.py
├── docs/
│   ├── document.md
│   └── prompt.md
├── README.md
├── QuickWinstall.sln
└── QuickWinstall.csproj
```

### Theme, Font & UI Elements

Light:

- Normal Font: Segoe UI, 12pt, Regular, #000000
- Header Font: Segoe UI, 16pt, Bold, #000000
- SubHeader Font: Segoe UI, 12pt, Bold, #000000
- Placeholder Font: Segoe UI, 12pt, Regular, #e1e1e1
- Error Font: Segoe UI, 12pt, Regular, #ff0000
- Warning Font: Segoe UI, 12pt, Regular, #ffa500
- Success Font: Segoe UI, 12pt, Regular, #008000
- ToolTip Font: Segoe UI, 10pt, Italic, #bbbbbb

Dark:

- Not defined yet

UI Elements:

- globalSpacingX: 20px
- globalTabX: 20px
- globalSpacingY: 10px
- globalBtnWidth: 100px
- globalBtnHeight: 40px
- globalBtnBox: 40px
- globalLabelWidth: 200px
- globalLabelHeight: 30px
- globalIconSize: 16x16px
- globalInputWidth: 400px
- globalInputHeight: 30px

### MainForm

The main form of the application.
See the illustration for better preference:
![MainForm Illustration](pngs/MainForm.png)
See the remainings in `docs/pngs/`.

Properties:

- Form frame:
  - Form Title: QuickWinstall
  - Width: min/default/max - 600/800/fullscreen -> remember last size
  - Height: min/default/max - 400/600/fullscreen -> remember last size
  - Resizable: Yes
  - Minimize/Maximize/Close Buttons: 1/1/1
  - Icon: res/icons/app256.ico
  - Background Color: Depends on theme (light/dark) (light: #e1e1e1, dark: not defined yet)
  - Title Font: Normal Font
- Activity Area:
  - Banner Panel:
    - Width: same as Form width
    - Height: 100px (fixed on scaling)
    - Background Color: Depends on theme (light/dark) (light: #e1e1e1, dark: not defined yet)
    - Position: Top of the form
    - Elements:
      - Logo:
        - Image Source: res/icons/windows11.ico
        - Size: globalBtnBox x globalBtnBox
        - Image align: Middle Center
        - Posisition (x, y): (globalTabX, BannerHeight/2 - LogoHeight/2)
        - On click: Scroll to top of the ConfigSection Panel
      - Label:
        - Text: Automated Windows 11 Installation
        - Font: Header Font
        - Text Align: Middle Left
        - Width: auto (fit to text)
        - Height: globalLabelHeight
        - Position (x, y): (Logo.Right + globalSpacingX, BannerHeight/2 - LabelHeight/2)
      - Buttons:
        - Collapse Button:
          - Icon: res/icons/collapse.ico
          - Size: globalIconSize x globalIconSize
          - Image align: Middle Center
          - Position (x, y): (Form.Width - globalTabX - globalBtnBox, BannerHeight/2 - globalBtnBox/2)
          - On click: Collapse the ConfigSection Panel by assigning the expanded state to false
          - Tooltip: Collapse the Configuration Panel (parse from ToolTipManager)
        - Expand Button:
          - Icon: res/icons/expand.ico
          - Size: globalIconSize x globalIconSize
          - Image align: Middle Center
          - Position (x, y): (collapseBtn.Left - globalSpacingX - globalBtnBox, BannerHeight/2 - globalBtnBox/2)
          - On click: Expand the ConfigSection Panel by assigning the expanded state to true
          - Tooltip: Expand the Configuration Panel
  - ConfigSection Panel:
    - Width: same as Form width
    - Height: dynamic (depends on content)
    - Background Color: Depends on theme (light/dark) (light: #e1e1e1, dark: not defined yet)
    - Position: Below the Banner Panel
    - Elements:
      - GeneralConfig Section:
        - Width: same as ConfigSection Panel width
        - Height: dynamic (depends on content)
        - Position: Top of the ConfigSection Panel
        - Elements:
          - Button:
            - Expand/Collapse Button:
              - Icon: res/icons/collapse.ico / res/icons/expand.ico
              - Image align: Middle Center
              - Size: globalBtnBox x globalBtnBox
              - Position (x, y): (globalTabX, globalSpacingY)
              - On click: Toggle the expanded state of the GeneralConfig Section
              - Tooltip: Expand/Collapse General Configuration Section
          - Label:
            - Text: General Configurations
            - Font: SubHeader Font
            - Text Align: Middle Left
            - Width: auto (fit to text)
            - Height: globalLabelHeight
            - Position (x, y): (expandCollapseBtn.Right + globalSpacingX, globalSpacingY)
            - On click: Expand/Collapse GeneralConfig Section
          - Line:
            - Width: ConfigSection Panel.Width - 2 * globalTabX
            - Height: 1px
            - Color: #000000
          - Content: described later in "GeneralConfig" sub-section.
      - LangRegConfigSection:
        - Width: same as ConfigSection Panel width
        - Height: dynamic (depends on content)
        - Position: Below the GeneralConfig Section
        - Elements:
          - Button:
            - Expand/Collapse Button:
              - Icon: res/icons/collapse.ico / res/icons/expand.ico
              - Image align: Middle Center
              - Size: globalBtnBox x globalBtnBox
              - Position (x, y): (globalTabX, generalConfigSection.Bottom + globalSpacingY)
              - On click: Toggle the expanded state of the LangRegConfig Section
              - Tooltip: Expand/Collapse Language & Region Configuration Section
          - Label:
            - Text: Language & Region Configurations
            - Font: SubHeader Font
            - Text Align: Middle Left
            - Width: auto (fit to text)
            - Height: globalLabelHeight
            - Position (x, y): (expandCollapseBtn.Right + globalSpacingX, generalConfigSection.Bottom + globalSpacingY)
            - On click: Expand/Collapse LangRegConfig Section
          - Line:
            - Width: ConfigSection Panel.Width - 2 * globalTabX
            - Height: 1px
            - Color: #000000
            - Position (x, y): (globalTabX, expandCollapseBtn.Bottom + globalSpacingY / 2)
          - Content: described later in "LangRegConfig" sub-section.
      - ... (similar structure for BypassConfig, DiskPartConfig, UserAccConfig, OOBEConfig, PersonalConfig sections)
  - Control Panel:
    - Width: same as Form width
    - Height: 60px (fixed on scaling)
    - Background Color: Depends on theme (light/dark) (light: #e1e1e1, dark: not defined yet)
    - Position: Bottom of the form, below ConfigSection Panel and above Status Strip
    - Elements:
      - Buttons:
        - Settings Button:
          - Text: Settings
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Position (x, y): (globalSpacingX, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Open SettingsForm
          - Tooltip: Open Application's settings
        - Clear Button:
          - Text: Clear
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Position (x, y): (settingsBtn.Right + globalSpacingX, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Clear all configurations to empty values (specified in `src/config/empty.json`)
          - Tooltip: Clear all configurations
          - Confirmation Dialog:
            - Title: Warning
            - Icon: Warning Icon
            - Font: Normal Font
            - Message: Clear all configurations?\n This action cannot be undone.
            - Buttons: OK/Cancel
            - Focus Default Button: Cancel
            - If OK clicked: Clear all configurations
            - If Cancel clicked: Return to MainForm
        - Preset Button:
          - Text: Preset
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Position (x, y): (clearBtn.Right + globalSpacingX, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Open PresetsForm
          - Tooltip: Load/Save a preset of current configurations
        - Cancel Button:
          - Text: Cancel
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Position (x, y): (Form.Width - globalTabX - globalBtnWidth, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Check for unsaved changes, if any, prompt the Unsaved Warning Dialog -> YES -> Close the application; NO -> return to MainForm; else, close the application directly.
          - Tooltip: Exit QuickWinstall
          - Unsaved Warning Dialog:
            - Title: Warning
            - Icon: Warning Icon
            - Font: Normal Font
            - Message: Leave without saving?\n This action cannot be undone.
            - Buttons: OK/Cancel
            - Focus Default Button: Cancel
            - If OK clicked: Close the application
            - If Cancel clicked: Return to MainForm
            - If there are no unsaved changes: Close the application directly
        - Generate Button:
          - Text: Generate
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Position (x, y): (cancelBtn.Left - globalSpacingX - globalBtnWidth, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Validate all configurations -> if valid, Check if the save path is accessible; else, prompt Validation Error Dialog -> if accessible, Check if a file named autounattend.xml already exists in the save path; else, prompt Empty Save Path Error Dialog -> if exists, prompt Overwrite Confirmation Dialog -> if confirmed, Generate autounattend.xml file using XMLGenerator; else, return to MainForm -> if successful, Show Success Dialog; else, Show Generate Error Dialog
          - Tooltip: Generate autounattend.xml file
          - Validation Error Dialog:
            - Title: Error
            - Icon: Error Icon
            - Font: Normal Font
            - Message: Please fix this validation errors:\n - {The first error}
            - Buttons: OK
            - Focus Default Button: OK
            - If OK clicked: Return to MainForm
          - Empty Save Path Error Dialog:
            - Title: Error
            - Icon: Error Icon
            - Font: Normal Font
            - Message: The save path is empty or inaccessible.\n Please set a valid save path in Settings.
            - Buttons: OK
            - Focus Default Button: OK
            - If OK clicked: Return to MainForm and open SettingsForm
          - Overwrite Confirmation Dialog:
            - Title: Warning
            - Icon: Warning Icon
            - Font: Normal Font
            - Message: Overwrite existing autounattend.xml?\n This action cannot be undone.
            - Buttons: OK/Cancel
            - Focus Default Button: OK
            - If OK clicked: Generate autounattend.xml file
            - If Cancel clicked: Return to MainForm
          - Success Dialog:
            - Title: Information
            - Icon: Information Icon
            - Font: Normal Font
            - Message: Done! File saved at:\n {save path}\autounattend.xml
            - Buttons: OK
            - Focus Default Button: OK
            - If OK clicked: Return to MainForm
    - Status Strip:
      - Width: same as Form width
      - Height: 40px (fixed on scaling)
      - Background Color: Depends on theme (light/dark) (light: #e1e1e1, dark: not defined yet)
      - Position: Bottom of the form, below Control Panel
      - Elements:
        - Prefix Label:
          - Text: Status:
          - Font: Normal Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalSpacingX, StatusStrip.Height/2 - LabelHeight/2)
          - On update: Static text
          - Text align: Middle Left
        - Status Message Label:
          - Text: -> parse from StatusManager
          - Font: -> parse from StatusManager (Normal Font, Error Font, Warning Font, Success Font)
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (prefixLabel.Right + globalTabX, StatusStrip.Height/2 - LabelHeight/2)
          - On update: Update text and font based on StatusManager
          - Text align: Middle Left

#### GeneralConfig

- Content:
  - Labels and Input Fields for each configuration parameter:
    - Windows Edition:
      - Label:
        - Text: Windows Edition
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, lineSeparator.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Select the Windows edition to install
      - Dropdown:
        - Options: Select one, Windows 11 Home, Windows 11 Pro, Windows 11 Education, Windows 11 Enterprise
        - Value mapping:
          - Select one -> (empty string)
          - Windows 11 Home -> Windows 11 Home
          - Windows 11 Pro -> Windows 11 Pro
          - Windows 11 Education -> Windows 11 Education
          - Windows 11 Enterprise -> Windows 11 Enterprise
        - Position (x, y): (label.Right + globalSpacingX, lineSeparator.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring
    - Product Key:
      - Label:
        - Text: Product Key
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, windowsEditionDropdown.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Enter Product Key to activate Windows.\n Leave it blank to skip activation, or to activate with embedded key.
      - TextBox[]:
        - TextBox1:
          - Max Length: 5
          - Position (x, y): (label.Right + globalSpacingX, windowsEditionDropdown.Bottom + globalSpacingY)
          - Width: (globalInputWidth - 4 * globalSpacingX) / 5
          - Height: globalInputHeight
          - Placeholder Text: XXXXX
          - Placeholder Font: Placeholder Font
          - Enter Font: Normal Font
          - Input Type: Uppercase Alphanumeric only
          - Auto Move Focus: Yes, to next TextBox when 5 characters are entered
        - Label (Hyphen1):
          - Text: -
          - Font: Normal Font
          - Width: globalSpacingX
          - Height: globalInputHeight
          - Position (x, y): (textbox1.Right, windowsEditionDropdown.Bottom + globalSpacingY)
          - Text Align: Middle Center
        - TextBox2:
          - Max Length: 5
          - Position (x, y): (LabelHyphen1.Right, windowsEditionDropdown.Bottom + globalSpacingY)
          - Width: (globalInputWidth - 4 * globalSpacingX) / 5
          - Height: globalInputHeight
          - Placeholder Text: XXXXX
          - Placeholder Font: Placeholder Font
          - Enter Font: Normal Font
          - Input Type: Uppercase Alphanumeric only
          - Auto Move Focus: Yes, to next TextBox when 5 characters are entered
        - ... (similar structure for TextBox3, TextBox4, TextBox5 with Hyphen labels in between)
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the first TextBox, the TextBoxes are on top of the status ring
      - Note: If all TextBoxes are empty, the Product Key is considered as empty string.
    - CPU Architecture:
      - Label:
        - Text: CPU Architecture
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, productKeyTextBoxes.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Select the CPU architecture for installation.
      - Dropdown:
        - Options: Select one, Intel/AMD x64, Windows ARM64
        - Value mapping:
          - Select one -> (empty string)
          - Intel/AMD x64 -> x64
          - Windows ARM64 -> ARM64
        - Position (x, y): (label.Right + globalSpacingX, productKeyTextBoxes.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring
- Validation Rules:
  - Windows Edition: Must not be empty.
  - Product Key: Must be either empty or a valid 25-character key in the format XXXXX-XXXXX-XXXXX-XXXXX-XXXXX.
  - CPU Architecture: Must not be empty.

### LangRegConfig

Further updates will be added later.

### Lib

#### ConfigValues.cs

This class contains all configuration values in each section

#### IconManager.cs

This class manages all icons used in the application.
Provide functions to load icons from `res/icons/` folder and retrieve them by name.
If an icon is not found, it returns a default placeholder icon (e.g., `res/icons/error.ico`)
There are two themes supported: light and dark icon sets. (except for app, app256, windows11, error, error256, warning, waning256, info, info256 icons)
Light set icons: stored without any suffix, e.g., `expand.ico`
Dark set icons: stored with `_dark` suffix, e.g., `expand_dark.ico`
By default, if theme is not set or light set is selected, light set icons are used.

#### LangHelper.cs

This class contains helper functions for language management.
Helper functions to parse and format strings with parameters.

#### LangManager.cs

This class manages language loading and switching.
There are many language packages used in the application, stored in `res/langs/` folder in JSON format.
Currently, two languages are supported: English (en-US) and Vietnamese (vi-VN).
The langManager loads the language files and provides functions to get strings based on the current language setting.
If a string key is not found in the current language, it falls back to English.
Even if language files are missing or corrupted, the application will still run using default (hardcoded) English strings.

#### PresetsManager.cs

This class manages loading and saving configuration presets.
Presets are stored in `res/presets/` folder in JSON format.
There are three built-in presets:

- common.json: for general users
- developer.json: for developers
- many more: for specific use cases

Specially, lastConfig.json is used to store the last used configuration when the application is closed, and load it when the application is opened again.
All presets are supposed to be added, removed (not modified yet), and imported, exported by users.

#### SettingsManager.cs

This class manages application settings, such as theme and language preferences.
Store application's settings in `src/main/settings.json` file in JSON format.
If the settings file is missing or corrupted, the application will create a new one with default settings (which are hardcoded).
Default settings:

- Theme: Light
- Language: en-US
- Save Path: Application Directory
- Save last configuration: true

#### StatusManager.cs

This class manages status messages displayed in the Status Strip of the MainForm.

#### ThemeManager.cs

This class manages theme settings (light/dark) and provides color values based on the current theme.  

#### ToolTipManager.cs

This class manages tooltips for various tooltip properties in the application.

#### UIValues.cs

This class contains global UI values, such as dimensions, bool states, and other properties used throughout the application.
These values are stored in `src/ui/ui.json` file in JSON format.
This class load this ui.json file and provide functions to get these values by name.
Here are some common UI values:

-globalSpacingX: 20px
-globalSpacingY: 10px
-globalBtnWidth: 100px
...

#### XMLGenerator.cs

This class generates the autounattend.xml file based on the current configurations.

#### template.xml

This is the XML template used by XMLGenerator to create the autounattend.xml file.
This template contains placeholders for various configuration parameters that will be replaced with actual values during the generation process.
For example:

```plaintext
<ProductKey>{{ProductKey}}</ProductKey>

will be replaced with the actual product key value provided by the user.

<ProductKey>ABCDE-FGHIJ-KLMNO-PQRST-UVWXY</ProductKey>
```

## Changelog

### v0.1 - Initial Release (2024)

**Initial Implementation:**
- Complete application structure with all core library classes
- MainForm UI with GeneralConfig section
- Theme management (light/dark)
- Multi-language support (English/Vietnamese)
- XML generation with template system
- Configuration validation
- Product key format validation

**UI Fixes and Enhancements:**

1. **Icon Loading** - Fixed IconManager to properly load application icon (app256.ico) and Windows 11 logo (windows11.png)

2. **Expand/Collapse All Buttons** - Added global expand/collapse buttons in Banner Panel for all configuration sections

3. **Section Header Alignment** - Centered section headers vertically relative to toggle buttons, made headers clickable

4. **Tooltips** - Implemented comprehensive tooltip system with ToolTipManager for all interactive controls

5. **Button Styling** - Created rounded button corners with `CreateRoundedButton()` helper method, removed white borders using `FlatStyle.Flat` with `BorderSize=0`

6. **Application Icon** - Set Form.Icon property to display application icon in window title bar and taskbar

7. **ui.json Restructure** - Changed from flat structure to hierarchical format with `global` and `sections` organization for better maintainability

8. **UIValues Section Support** - Added `GetSectionValue()` and path-based `GetValue()` methods to support sectioned configuration

9. **Line Separators** - Added 1px black separator panels between section headers and content for visual clarity

10. **Content Panel Height** - Fixed content panel height to use configurable value from ui.json instead of hardcoded 250px

11. **Status Bar Improvements** - Set `lblStatus.Spring = true` to fill remaining window width, implemented "Unsaved Configurations" warning when changes are made

12. **Product Key TextBox** - Enhanced with:
    - Centered text alignment
    - Placeholder text "XXXXX" with gray color
    - Auto-clear placeholder on focus
    - Auto-restore placeholder when empty
    - Auto-move to next textbox when reaching max length (5 characters)

**Technical Improvements:**
- Added `System.Drawing.Drawing2D` for GraphicsPath rounded button regions
- Helper methods: `CreateRoundedButton()`, `CreateProductKeyTextBox()`, `FindProductKeyTextBox()`
- Event handlers: `BtnExpandAll_Click()`, `BtnCollapseAll_Click()`
- Enhanced `OnConfigChanged()` to update status bar with unsaved changes warning
- Proper use of `ToolTipManager.SetToolTip()` for all controls with language-based tooltip keys

**Language File Updates:**
- Added `mainForm.status.unsavedChanges` key for English and Vietnamese
- Complete tooltip section with keys for all buttons and controls
- Section-specific tooltips with formatting support (e.g., `{0}` for section name)
