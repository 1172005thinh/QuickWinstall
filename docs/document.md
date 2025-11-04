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
│       ├── light.json
│       └── windowsXP.json
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

- Defined already in `res/themes/dark.json`

Windows XP:

- Has Windows XP style fonts and colors, defined in `res/themes/windowsXP.json`

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

- Content:
  - Labels and Input Fields for each configuration parameter:
    - System Locale:
      - Label:
        - Text: System Locale
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, lineSeparator.Bottom + globalSpacingY)
        - ThemeColor: Depends on theme (light/dark)
        - Text Align: Middle Left
        - Tooltip: Select the System Locale
      - Dropdown:
        - Options: Select one, English (United States), Vietnamese (Vietnam)
        - Value mapping:
          - Select one -> (empty string)
          - English (United States) -> en-US
          - Vietnamese (Vietnam) -> vi-VN
        - Position (x, y): (label.Right + globalSpacingX, lineSeparator.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
        - On change: If "Same as System Locale" is checked, set User Locale dropdown value to the same as System Locale dropdown.
        - Validation: Must not be empty. If empty -> message: System Locale must not be empty. Watchout for System Local != User Locale && checkbox checked case as user can manually change in json file. If so, uncheck the checkbox automatically.
        - Warning: If "Same as System Locale" is unchecked and User Locale is not the same as System Locale (not empty) -> warning ring to both dropdowns with message: Different System Locale and User Locale may cause issues during Windows Installation.
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring
    - User Locale:
      - Label:
        - Text: User Locale
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, systemLocaleDropdown.Bottom + globalSpacingY)
        - ThemeColor: Depends on theme (light/dark)
        - Text Align: Middle Left
        - Tooltip: Select the User Locale
      - Dropdown:
        - Options: Select one, English (United States), Vietnamese (Vietnam)
        - Value mapping:
          - Select one -> (empty string)
          - English (United States) -> en-US
          - Vietnamese (Vietnam) -> vi-VN
        - Position (x, y): (label.Right + globalSpacingX, systemLocaleDropdown.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
        - On change: If "Same as System Locale" is checked, set its value to the same as System Locale dropdown.
        - Validation: Disabled if "Same as System Locale" is checked. If enabled, must not be empty. If empty -> message: User Locale must not be empty. Watchout for System Local != User Locale && checkbox checked case as user can manually change in json file. If so, uncheck the checkbox automatically.
        - Warning: If "Same as System Locale" is unchecked and User Locale is not the same as System Locale (not empty) -> warning ring to both dropdowns with message: Different System Locale and User Locale may cause issues during Windows Installation.
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring
    - Same as System Locale:
      - Toggle checkbox:
        - Label:
          - Text: Same as System Locale
          - Font: Normal Font
          - Width: globalInputWidth
          - Height: globalInputHeight
          - ThemeColor: Depends on theme (light/dark)
          - Position (x, y): (checkbox.Right + globalSpacingX, userLocaleDropdown.Bottom + globalSpacingY)
          - Text Align: Middle Left
          - Tooltip: Enable or disable using the same locale as the system locale for User Locale
        - Checkbox:
          - Position (x, y): (labelUserLocale.Right + globalSpacingX, userLocaleDropdown.Bottom + globalSpacingY)
          - Size: globalCheckboxSize x globalCheckboxSize (new keys -> add to ui.json and UIValues.cs)
          - On check: Disable User Locale dropdown and set its value to the same as System Locale. Update userLocaleDropdown state accordingly. Update if System Locale changes.
          - On uncheck: Enable User Locale dropdown.
          - Default Value: Checked
          - Tooltip: Enable or disable using the same locale as the system locale for User Locale
      - Validation Rules:
        - If checked: User Locale must be the same as System Locale.
        - If unchecked: User Locale must not be empty.
    - Windows UI Language:
      - Label:
        - Text: Windows UI Language
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, sameAsSystemLocaleCheckbox.Bottom + globalSpacingY)
        - ThemeColor: Depends on theme (light/dark)
        - Text Align: Middle Left
        - Tooltip: Select the Windows UI Language
      - Dropdown:
        - Options: Select one, English (United States), Vietnamese (Vietnam)
        - Value mapping:
          - Select one -> (empty string)
          - English (United States) -> en-US
          - Vietnamese (Vietnam) -> vi-VN
        - Position (x, y): (label.Right + globalSpacingX, sameAsSystemLocaleCheckbox.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
        - Validation: Must not be empty. If empty -> message: Windows UI Language must not be empty.
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring
    - Keyboard Layout:
      - Label:
        - Text: Keyboard Layout
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, windowsUILanguageDropdown.Bottom + globalSpacingY)
        - ThemeColor: Depends on theme (light/dark)
        - Text Align: Middle Left
        - Tooltip: Select the Keyboard Layout
      - Dropdown:
        - Options: Select one, US, Vietnamese
        - Value mapping:
          - Select one -> (empty string)
          - US -> 0409:00000409
          - Vietnamese -> 00000409:00000409 (Vietnamese Keyboard Layout uses US layout with additional Vietnamese input method)
        - Position (x, y): (label.Right + globalSpacingX, windowsUILanguageDropdown.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
        - Validation: Must not be empty. If empty -> message: Keyboard Layout must not be empty.
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring
    - Time Zone:
      - Label:
        - Text: Time Zone
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, keyboardLayoutDropdown.Bottom + globalSpacingY)
        - ThemeColor: Depends on theme (light/dark)
        - Text Align: Middle Left
        - Tooltip: Select the Time Zone
      - Dropdown:
        - Options: Select one, (list of time zones)
        - Value mapping:
          - Select one -> (empty string)
          - (list of time zones) -> (corresponding time zone IDs)
        - Position (x, y): (label.Right + globalSpacingX, keyboardLayoutDropdown.Bottom + globalSpacingY)
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Font: Normal Font
        - Text Align: Middle Left
        - Validation: Must not be empty. If empty -> message: Time Zone must not be empty.
      - Status Ring:
        - Width: globalInputWidth
        - Height: globalInputHeight
        - Color: Glowing effect -> parse from ThemeManager (No Color, Error Color, Warning Color)
        - Position: same as the dropdown, the dropdown is on top of the status ring

### BypassConfig

- Content:
  - Label and Input Field for each configuration parameter:
    - Bypass Hardware Check:
      - Label:
        - Text: Enable Bypass Windows 11 Hardware Check
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, lineSeparator.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Enable to make hardware check bypass options available
      - Toggle switch:
        - Position (x, y): (label.Right + globalSpacingX, lineSeparator.Bottom + globalSpacingY)
        - Height: globalInputHeight
        - Width: globalInputWidth * 0.15
        - On toggle: Enable (true) or Disable (false) hardware check bypass
        - On update: If disabled, disable all depenedent toggles (All, TPM, RAM, Secure Boot, CPU,...). If enabled, enable all dependents (just for available, not change their states).
        - Default Value: True
    - Bypass All Checks:
      - Label:
        - Text: Bypass All Checks
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, enableBypassWindows11HardwareCheckToggle.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Enable or disable bypassing all hardware checks during installation
      - Toggle switch:
        - Position (x, y): (label.Right + globalSpacingX, enableBypassWindows11HardwareCheckToggle.Bottom + globalSpacingY)
        - Height: globalInputHeight
        - Width: globalInputWidth * 0.15
        - On toggle: Enable (true) or Disable (false) all hardware check bypasses
        - On update: If enabled, assign True to all individual bypass toggles (TPM, RAM, Secure Boot, CPU,...). If disabled, do not change individual toggles states. If all individual toggles are enabled, enable this toggle automatically; else, disable this toggle automatically.
        - Default Value: True
    - Bypass TPM Check:
      - Label:
        - Text: Bypass TPM Check
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, bypassAllChecksToggle.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Enable or disable bypassing TPM check during installation
      - Toggle switch:
        - Position (x, y): (label.Right + globalSpacingX, bypassAllChecksToggle.Bottom + globalSpacingY)
        - Height: globalInputHeight
        - Width: globalInputWidth * 0.15
        - On toggle: Enable (true) or Disable (false) TPM check bypass
        - Default Value: True
    - Bypass RAM Check:
      - Label:
        - Text: Bypass RAM Check
        - Font: Normal Font
        - Width: globalLabelWidth
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX * 2 + globalBtnBox, bypassTPMCheckToggle.Bottom + globalSpacingY)
        - Text Align: Middle Left
        - Tooltip: Enable or disable bypassing RAM check during installation
      - Toggle switch:
        - Position (x, y): (label.Right + globalSpacingX, bypassTPMCheckToggle.Bottom + globalSpacingY)
        - Height: globalInputHeight
        - Width: globalInputWidth * 0.15
        - On toggle: Enable (true) or Disable (false) RAM check bypass
        - Default Value: True
    - ... (similar structure for Bypass Secure Boot Check, Bypass CPU Check, Storage Check, Disk Check)
    - Validation Rules:
      - None (all toggles are optional)
    - Value mapping to XML:
      - Bypass All checks = True -> Set all individual bypass values to 1 in XML
      - Bypass All checks = False -> Set all individual bypass values to 0 in XML
      - Look for {{BypassTPMCheck}}, {{BypassRAMCheck}}, {{BypassSecureBootCheck}}, {{BypassCPUCheck}}, {{BypassStorageCheck}}, {{BypassDiskCheck}} keys in the XML template for individual bypass values mapping.

### DiskPartConfig

Later...

### UserAccConfig

Later...

### OOBEConfig

- Content:
  - Label and Input Field for each configuration parameter:
    - Skip All
    - Skip EULA screen
    - Skip Local Account Creation screen
    - Skip Online Account Creation screen
    - Skip Wireless Network screen
    - Skip Machine OOBE settings screen
    - Skip User OOBE settings screen
  - Validation Rules:
    - None (all toggles are optional)
  - Structure are similar to BypassConfig section.

### SettingsForm

The settings form of the application
See the illustration for better preference:
![SettingsForm Illustration](pngs/SettingsForm.png)
![SettingsForm on MainForm](pngs/SettingsForm_onMainForm.png)

Properties:

- Form frame:
  - Form Title: Settings
  - Width: 400px
  - Height: 300px
  - Resizable: No
  - Minimize/Maximize/Close Buttons: 0/0/1
  - Icon: res/icons/settings.ico / res/icons/settings_dark.ico
  - Background Color: Depends on theme (light/dark)
  - Title Font: Normal Font
- Content:
  - Banner Panel:
    - Width: same as Form width
    - Height: 40px
    - Background Color: Depends on theme (light/dark)
    - Position: Top of the form
    - Elements:
      - Label:
        - Text: Application Settings
        - Font: Header Font
        - Text Align: Middle Left
        - Width: auto (fit to text)
        - Height: globalLabelHeight
        - Position (x, y): (globalTabX, BannerHeight/2 - LabelHeight/2)
      - Button:
        - Reset to Default Button:
          - Text: none (icon only)
          - Icon: res/icons/reset.ico / res/icons/reset_dark.ico
          - Size: globalBtnBox x globalBtnBox
          - Image align: Middle Center
          - Position (x, y): (Form.Width - globalTabX - globalBtnBox, BannerHeight/2 - globalBtnBox/2)
          - On click: Prompt confirmation dialog before resetting -> Confirmed, reset all settings to default values (hardcoded)
          - Tooltip: Reset all settings to default values
          - Confirmation Dialog:
            - Title: Warning
            - Icon: Warning Icon
            - Font: Normal Font
            - Position: Center of the SettingsForm
            - Message: Reset all settings to default values?\n This action cannot be undone.
            - Buttons: OK/Cancel
            - Focus Default Button: Cancel
            - If OK clicked: Reset all settings to default values, return to SettingsForm
            - If Cancel clicked: Return to SettingsForm
  - Settings Panel:
    - Width: same as Form width
    - Height: dynamic (depends on content)
    - Background Color: Depends on theme (light/dark)
    - Position: Below the Banner Panel
    - Elements:
      - Language Selection:
        - Label:
          - Text: Language
          - Font: Normal Font
          - Width: globalLabelWidth
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX, globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
          - Tooltip: Application’s Language settings DOES NOT AFFECT Windows UI Language
        - Dropdown:
          - Options: English, Tiếng Việt
          - Value mapping:
            - English -> en-US (Default value)
            - Tiếng Việt -> vi-VN
          - Position (x, y): (label.Right + globalSpacingX, globalSpacingY)
          - Width: globalInputWidth * 0.75
          - Height: globalInputHeight
          - Font: Normal Font
          - Background Color: Depends on theme (light/dark)
          - Foreground Color: Depends on theme (light/dark)
          - Text Align: Middle Left
          - On change: Update application language after Save button is clicked
          - Tooltip: Application’s Language settings DOES NOT AFFECT Windows UI Language
          - Auto-correct: If the settings.json is manually modified to an unsupported language code, reset to default (en-US)
      - Theme Selection:
        - Label:
          - Text: Theme
          - Font: Normal Font
          - Width: globalLabelWidth
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX, languageDropdown.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
          - Tooltip: Select application theme
        - Dropdown:
          - Options: Light, Dark
          - Value mapping:
            - Light -> Light (Default value)
            - Dark -> Dark
          - Position (x, y): (label.Right + globalSpacingX, languageDropdown.Bottom + globalSpacingY)
          - Width: globalInputWidth * 0.75
          - Height: globalInputHeight
          - Font: Normal Font
          - Text Align: Middle Left
          - Background Color: Depends on theme (light/dark)
          - Foreground Color: Depends on theme (light/dark)
          - On change: Update application theme after Save button is clicked
          - Tooltip: Select application theme
          - Auto-correct: If the settings.json is manually modified to an unsupported theme code, reset to default (Light)
      - XML Save Path:
        - Label:
          - Text: XML Save Path
          - Font: Normal Font
          - Width: globalLabelWidth
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX, themeDropdown.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
          - Tooltip: Set the default save path for generated autounattend.xml file
        - TextBox:
          - Position (x, y): (label.Right + globalSpacingX, themeDropdown.Bottom + globalSpacingY)
          - Width: globalInputWidth * 0.75 - globalBtnBox - globalSpacingX
          - Height: globalInputHeight
          - Font: Normal Font
          - Placeholder Text: e.g., C:\Users\Username\Desktop
          - Placeholder Font: Placeholder Font
          - Background Color: Depends on theme (light/dark)
          - Foreground Color: Depends on theme (light/dark)
          - Default value: Application Directory
          - Text Align: Middle Left
          - On change: Update save path after Save button is clicked
          - Tooltip: Set the default save path for generated autounattend.xml file
        - Button:
          - Browse Button:
            - Text: none (icon only)
            - Icon: res/icons/browse.ico / res/icons/browse_dark.ico
            - Font: Normal Font
            - Text Align: Middle Center
            - Size: globalBtnBox x globalBtnBox
            - Background Color: Depends on theme (light/dark)
            - Position (x, y): (textBox.Right + globalSpacingX / 2, themeDropdown.Bottom + globalSpacingY)
            - On click: Open Folder Browser Dialog to select a folder, update the TextBox with selected folder path
            - Tooltip: Browse for a folder to set as XML Save Path
            - Folder Browser Dialog:
              - Description: XML saved at:
              - Root Folder: Application Directory
              - File Explorer Title: Select XML Save Path
      - Auto Save Last Configuration:
        - Label:
          - Text: Auto Save Last Config
          - Font: Normal Font
          - Width: globalLabelWidth
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX, xmlSavePathTextBox.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Tooltip: Automatically save the current configuration when exiting the application to Presets/lastConfig.json.
          - Text Align: Middle Left
        - Toggle Switch:
          - Position (x, y): (label.Right + globalSpacingX, xmlSavePathTextBox.Bottom + globalSpacingY)
          - Width: globalInputWidth * 0.25
          - Height: globalInputHeight
          - Font: Normal Font
          - On change: Update auto save last configuration setting after Save button is clicked
          - Tooltip: Automatically save the current configuration when exiting the application to Presets/lastConfig.json.
          - States: ON/OFF (Default: ON)
          - ON:
            - Background Color: #2f5597 (light) / #8faadc (dark)
            - Toggle Color: #ffffff (light) / #e1e1e1 (dark)
            - Value: true
          - OFF:
            - Background Color: #afabab (light and dark)
            - Toggle Color: #ffffff (light) / #e1e1e1 (dark)
            - Value: false
          - Auto-correct: If the settings.json is manually modified to an invalid value, reset to default (true)
        - Label:
          - Text: ON / OFF
          - Font: Normal Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (toggleSwitch.Right + globalSpacingX, xmlSavePathTextBox.Bottom + globalSpacingY)
          - On Change: Update text based on toggle state
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
      - Load Last Saved Configuration:
        - Label:
          - Text: Load Last Saved Config
          - Font: Normal Font
          - Width: globalLabelWidth
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX, autoSaveToggle.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Tooltip: Automatically load the last saved configuration from Presets/lastConfig.json when opening the application.
          - Text Align: Middle Left
        - Toggle Switch:
          - Position (x, y): (label.Right + globalSpacingX, autoSaveToggle.Bottom + globalSpacingY)
          - Width: globalInputWidth * 0.25
          - Height: globalInputHeight
          - Font: Normal Font
          - On change: Update load last saved configuration setting after Save button is clicked
          - Tooltip: Automatically load the last saved configuration from Presets/lastConfig.json when opening the application.
          - States: ON/OFF (Default: ON)
          - ON:
            - Background Color: #2f5597 (light) / #8faadc (dark)
            - Toggle Color: #ffffff (light) / #e1e1e1 (dark)
            - Value: true
          - OFF:
            - Background Color: #afabab (light and dark)
            - Toggle Color: #ffffff (light) / #e1e1e1 (dark)
            - Value: false
          - Auto-correct: If the settings.json is manually modified to an invalid value, reset to default (true)
        - Label:
          - Text: ON / OFF
          - Font: Normal Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - FontColor: Depends on theme (light/dark)
          - On Change: Update text based on toggle state
          - Position (x, y): (toggleSwitch.Right + globalSpacingX, autoSaveToggle.Bottom + globalSpacingY)
          - Text Align: Middle Left
    - Control Panel:
      - Width: same as Form width
      - Height: 60px
      - Background Color: Depends on theme (light/dark)
      - Position: Bottom of the form
      - Elements:
        - About Button:
          - Text: About
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Background Color: Depends on theme (light/dark)
          - FontColor: Depends on theme (light/dark)
          - Position (x, y): (globalSpacingX, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Open AboutForm -> not implemented yet
          - Tooltip: About QuickWinstall
        - Help Button:
          - Text: Help
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Background Color: Depends on theme (light/dark)
          - FontColor: Depends on theme (light/dark)
          - Position (x, y): (aboutBtn.Right + globalSpacingX, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Open HelpForm -> not implemented yet
          - Tooltip: Open Help documentation
        - Cancel Button:
          - Text: Cancel
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Background Color: Depends on theme (light/dark)
          - FontColor: Depends on theme (light/dark)
          - Position (x, y): (Form.Width - globalTabX - globalBtnWidth, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Discard changes and close SettingsForm
          - Tooltip: Discard changes and close Settings
        - Save Button:
          - Text: Save
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Background Color: Depends on theme (light/dark)
          - FontColor: Depends on theme (light/dark)
          - Position (x, y): (cancelBtn.Left - globalSpacingX - globalBtnWidth, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Save all settings and close SettingsForm -> return to MainForm -> if theme or language changed, apply changes to all forms
          - Tooltip: Save all settings and close Settings

### AboutForm

The about form of the application
See the illustration for better preference:
![AboutForm Illustration](pngs/AboutForm.png)

Properties:

- Form frame:
  - Form Title: About QuickWinstall
  - Width: 580px <-- ui.json
  - Height: 600px <-- ui.json
  - Resizable: No
  - Minimize/Maximize/Close Buttons: 0/0/1
  - Icon: res/icons/about.ico / res/icons/about_dark.ico
  - Background Color: Depends on theme (light/dark)
  - Title Font: Normal Font
- Content:
  - Banner Panel:
    - Width: same as Form width
    - Height: 80px
    - Background Color: Depends on theme (light/dark)
    - Position: Top of the form
    - Elements:
      - Icon:
        - Source: res/icons/app256.ico
        - Size: 128x128px
        - Position (x, y): (globalTabX, BannerHeight/2 - 128/2)
      - Label Project:
        - Text: QuickWinstall Project <-- lang package
        - Font: Header Font
        - Text Align: Middle Left
        - Width: auto (fit to text)
        - Height: globalLabelHeight
        - Position (x, y): (icon.Right + globalSpacingX, BannerHeight/2 - globalLabelHeight - globalSpacingY/2)
      - Label Brief:
        - Text: A free, open-source autounattend generator for Windows 11 Installation. <-- lang package
        - Font: Normal Font
        - Text Align: Middle Left
        - Width: auto (fit to text)
        - Height: globalLabelHeight * 2
        - Position (x, y): (icon.Right + globalSpacingX, BannerHeight/2 + globalSpacingY/2)
      - Image:
        - Source: res/images/bg_Windows11.png
        - Size: fit to Banner Panel height, width auto
        - Position (x, y): (Form.Width - Image.Width, BannerHeight/2 - Image.Height/2)
    - About Panel:
      - Width: same as Form width
      - Height: dynamic (depends on content)
      - Background Color: Depends on theme (light/dark)
      - Position: Below the Banner Panel
      - Elements:
        - Image:
          - Source: res/images/bg_AboutForm.png
          - Size: fit to About Panel height, width auto
          - Position (x, y): (0, 0)
        - Label Original Author:
          - Text: Original Author: <-- lang package
          - Font: Subheader Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX x 8, globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Label Original Author Name Bullet:
          - Text: * <-- lang package
          - Font: Normal Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX x 9, labelOriginalAuthor.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Label Original Author Name:
          - Text: 1172005thinh (QuickComp.) <-- lang package
          - Font: Normal Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (labelOriginalAuthorBullet.Right + globalSpacingX, labelOriginalAuthor.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Label Follow Me:
          - Text: Follow me now! <-- lang package
          - Font: Normal Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX x 9, labelOriginalAuthorNameBullet.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Icon Youtube:
          - Source: res/icons/youtube.ico / res/icons/youtube_dark.ico
          - Size: globalBtnBox x 2 x globalBtnBox x 2
          - Position (x, y): (globalTabX x 10, labelFollowMe.Bottom + globalSpacingY)
          - On hover: Change cursor to Hand
          - On click: Open URL `https://www.youtube.com/@quickcompstore/`
          - Tooltip: `https://www.youtube.com/@quickcompstore/`
        - Icon GitHub:
          - Source: res/icons/github.ico / res/icons/github_dark.ico
          - Size: globalBtnBox x 2 x globalBtnBox x 2
          - Position (x, y): (youtubeIcon.Right + globalSpacingX, labelFollowMe.Bottom + globalSpacingY)
          - On hover: Change cursor to Hand
          - On click: Open URL `https://github.com/1172005thinh/`
          - Tooltip: `https://github.com/1172005thinh/`
        - Icon Facebook:
          - Source: res/icons/facebook.ico / res/icons/facebook_dark.ico
          - Size: globalBtnBox x 2 x globalBtnBox x 2
          - Position (x, y): (githubIcon.Right + globalSpacingX, labelFollowMe.Bottom + globalSpacingY)
          - On hover: Change cursor to Hand
          - On click: Open URL `https://www.facebook.com/quickcomp.hungthinhnguyen/`
          - Tooltip: `https://www.facebook.com/quickcomp.hungthinhnguyen/`
        - Label Contributors:
          - Text: Contributors: <-- lang package
          - Font: Subheader Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX x 8, labelOriginalAuthorName.Bottom + globalSpacingY * 2)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Contributor Panel:
          - Width: globalLabelWidth
          - Height: dynamic (depends on number of contributors)
          - Background Color: Depends on theme (light/dark)
          - Position: Below the Contributors Label
          - Contributors should have a list to show most significant contributors only (max 5)
          - Elements:
            - Label Contributor 1 Bullet:
              - Text: * <-- lang package
              - Font: Normal Font
              - Width: auto (fit to text)
              - Height: globalLabelHeight
              - Position (x, y): (globalTabX x 9, labelContributors.Bottom + globalSpacingY)
              - FontColor: Depends on theme (light/dark)
              - Text Align: Middle Left
            - Label Contributor 1 Name:
              - Text: 1172005thinh (QuickComp.) <-- lang package
              - Font: Normal Font
              - Width: auto (fit to text)
              - Height: globalLabelHeight
              - Position (x, y): (labelContributor1Bullet.Right + globalSpacingX, labelContributors.Bottom + globalSpacingY)
              - FontColor: Depends on theme (light/dark)
              - Text Align: Middle Left
            - ... (similar structure for Contributor 2, 3, 4, 5), but only show if the list has that many contributors
        - Label Contributor Submit Request:
          - Text: Contributors are welcome! Feel free to submit requests on + [GitHub](https://github.com/1172005thinh/quickwinstall/) <-- lang package for GitHub part
          - Font: Normal Font + Link Font for GitHub part
          - Width: globalLabelWidth
          - Height: globalLabelHeight * 2
          - Position (x, y): (globalTabX x 9, labelContributor1Name.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Label References:
          - Text: References: <-- lang package
          - Font: Subheader Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX x 8, labelContributorSubmitRequest.Bottom + globalSpacingY * 2)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Reference Panel:
          - Width: globalLabelWidth
          - Height: dynamic (depends on number of references)
          - Background Color: Depends on theme (light/dark)
          - Position: Below the References Label
          - Elements:
            - Label Reference 1 Bullet:
              - Text: * <-- lang package
              - Font: Normal Font
              - Width: auto (fit to text)
              - Height: globalLabelHeight
              - Position (x, y): (globalTabX x 9, labelReferences.Bottom + globalSpacingY)
              - FontColor: Depends on theme (light/dark)
              - Text Align: Middle Left
            - Label Reference 1 Name:
              - Text: [Schneegans Unattend Generator](https://schneegans.de/windows/unattend-generator/) <-- lang package for Schneegans Unattend Generator part
              - Font: Normal Font
              - Width: auto (fit to text)
              - Height: globalLabelHeight
              - Position (x, y): (labelReference1Bullet.Right + globalSpacingX, labelReferences.Bottom + globalSpacingY)
              - FontColor: Depends on theme (light/dark)
              - Text Align: Middle Left
            - ... (similar structure for Reference 2, 3, 4, 5), but only show if the list has that many references
        - Label License:
          - Text: License: <-- lang package
          - Font: Subheader Font
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX x 8, labelReferences.Bottom + globalSpacingY * 2)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
        - Label License Name:
          - Text: Free, open-source software licensed under the MIT License. <-- lang package
          - Font: Normal Font
          - Width: globalLabelWidth
          - Height: globalLabelHeight * 2
          - Position (x, y): (globalTabX x 9, labelLicense.Bottom + globalSpacingY)
          - FontColor: Depends on theme (light/dark)
          - Text Align: Middle Left
    - Control Panel:
      - Width: same as Form width
      - Height: 60px
      - Background Color: Depends on theme (light/dark)
      - Position: Bottom of the form
      - Elements:
        - Label Version:
          - Text: Version: vX.X.X
          - Font: Normal Font
          - Text Align: Middle Left
          - Width: auto (fit to text)
          - Height: globalLabelHeight
          - Position (x, y): (globalTabX, ControlPanel.Height/2 - globalLabelHeight/2)
          - FontColor: Depends on theme (light/dark)
        - Close Button:
          - Text: Close
          - Font: Normal Font
          - Text Align: Middle Center
          - Size: globalBtnWidth x globalBtnHeight
          - Background Color: Depends on theme (light/dark)
          - FontColor: Depends on theme (light/dark)
          - Position (x, y): (Form.Width - globalTabX - globalBtnWidth, ControlPanel.Height/2 - globalBtnHeight/2)
          - On click: Close AboutForm
          - Tooltip: Close About QuickWinstall

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
