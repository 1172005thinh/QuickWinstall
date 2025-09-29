# QuickWinstall - Autounattend Generator

![Version](https://img.shields.io/badge/version-0.1-blue)
![License](https://img.shields.io/badge/license-Free%20Open%20Source-green)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)

## 📖 Description

This C# application generates customized autounattend.xml files for unattended Windows installation. Features multi-language support, user-friendly GUI, and template-based XML generation.

## ✨ Features

- **🌐 Multi-language Support**: English and Vietnamese UI with full localization
- **🎨 Modern GUI**: Clean interface with expandable/collapsible sections  
- **⚙️ Comprehensive Configuration**:

  - General settings (PC name, Windows edition, CPU architecture, product key)
  - Language and region settings with locale synchronization
  - Multi-account management (up to 4 user accounts)
  - Windows 11 bypass options (TPM, RAM, SecureBoot, CPU, Storage, Disk)
  - BitLocker configuration
  - Placeholder for disk configuration
- **✅ Smart Validation**: Real-time input validation with visual feedback
- **🔧 Template-based Generation**: Creates standard autounattend.xml files
- **💾 Settings Management**: Persistent configuration with auto-save
- **🎯 Custom Dialogs**: Native-styled warning and information dialogs
- **📁 Icon System**: Extensible icon management with fallback support

## 🤖 Windows 11 LAZY Installation

1. If you haven't downloaded the latest Windows 11 ISO file, download it from the [official Microsoft Windows 11 download page](https://www.microsoft.com/en-us/software-download/windows11)
2. Extract your downloaded ISO file with any archive extraction tool (e.g., WinRAR) to a known folder
3. Copy the `autounattend.xml` file to your extracted Windows ISO folder
4. Copy all contents of the extracted ISO folder (including the `autounattend.xml` file) to a clean USB drive (minimum 8GB recommended)
5. Insert the USB drive into your target PC and boot to the Boot Device Manager (follow your manufacturer's instructions, as PC models and BIOS shortcuts vary)
6. Select the USB drive and enjoy a hassle-free Windows 11 installation

### IMPORTANT

- The project does not yet include disk configuration options; therefore, disk 0 will be completely wiped during Windows installation.
- PLEASE BACKUP ANY IMPORTANT FILES BEFORE INSTALLING.
- I DO NOT HOLD ANY RESPONSIBILITY FOR YOUR OWN DATA.

## 📁 Project Structure

```plaintext
QuickWinstall/
├── icons/                  # Application icons and graphics
│   ├── README.md           # Icon guidelines and list
│   └── [icon files]        # PNG icon files
├── lang/                   # Localization files
│   ├── README.md           # Localization guide
│   ├── en-US/              # English language pack
│   │   └── strings.json
│   └── vi-VN/              # Vietnamese language pack  
│       └── strings.json
├── .vscode/                # VS Code configuration
│   ├── launch.json         # Debug configuration
│   ├── tasks.json          # Build tasks
│   └── extensions.json     # Recommended extensions
├── MainForm.cs             # Main application form
├── AppSettings.cs          # Application settings management
├── LanguageManager.cs      # Localization manager
├── IconManager.cs          # Icon loading and caching
├── template.xml            # XML template file
├── QuickWinstall.csproj    # Project configuration
├── QuickWinstall.sln       # Solution file
└── README.md               # This file
```

## 🌐 Multi-language Support

The application supports:

- **English (en-US)**: Default language
- **Vietnamese (vi-VN)**: Full UTF-8 support

Language files are located in the `lang/` directory and can be easily extended for additional languages.

## 📋 System Requirements

- Windows 7 or later
- .NET 8.0 Runtime (for compiled executable)

## 🛠️ Development Setup

### Prerequisites

- .NET 8.0 SDK
- Visual Studio Code or Visual Studio 2022

### Building the Project

```bash
# Clone the repository
git clone https://github.com/1172005thinh/QuickWinstall.git
cd QuickWinstall

# Build the project
dotnet build

# Run the application
dotnet run

# Publish as single executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### VS Code Setup

The project includes VS Code configuration files:

- `.vscode/launch.json` - Debug configuration
- `.vscode/tasks.json` - Build tasks
- `.vscode/extensions.json` - Recommended extensions

Press `F5` to build and run with debugging, or use `Ctrl+Shift+P` and run "Tasks: Run Build Task".

## 🔗 References

- I used AI in development which might not fully credit somebody's work. If your work is here but not get credited, please contact me
- [Schneegans Unattend Generator](https://schneegans.de/windows/unattend-generator/)
- [Microsoft Documentation](https://docs.microsoft.com/en-us)

## 👨‍💻 Author

1172005thinh (QuickComp.)

- GitHub: [@1172005thinh](https://github.com/1172005thinh)
- Facebook: [@quickcomp.hungthinhnguyen](https://www.facebook.com/quickcomp.hungthinhnguyen)

## 📄 License

This project is free and open-source software

## 📜 Change Log

### v0.1 - Initial Release

- Initial release with core features and basic UI

## 🐞 Known Issues

- User Account managment section has UI bugs when removing not the last account. The field is red highlighted for no reason. Generation still works.
- User Account managment section has UX bugs when renaming Account Name. The Account Display Name glitches and does not update properly.
- Settings/Language does not show the current language on open. Currently it shows "English" always, although the language is switched.
- About Window is too small for the content.
- Vietnamese translation is incomplete and Google Translate-ish.
- Some UI elements may not be properly aligned or sized, such as Account Manager bottom padding, Account Password field is too small.
- Close button in Main Window does not trigger unsaved changes prompt if there are unsaved changes in dropdown lists.
