# Icons Directory

This directory contains icon files for the QuickWinstall application.

## Icon Guidelines

- **Format**: PNG format preferred (supports transparency)
- **Size**: Multiple sizes supported, commonly 16x16, 24x24, 32x32, 48x48
- **Naming**: Use lowercase with descriptive names (e.g., `settings.png`, `about.png`)

## Icon List

The following icons are used in the application:

### Main Interface

- `settings.png` - Settings/Preferences icon
- `about.png` - About/Information icon  
- `generate.png` - Generate XML button icon
- `exit.png` - Exit application icon

### File Operations

- `new.png` - New file icon
- `open.png` - Open file icon
- `save.png` - Save file icon
- `folder.png` - Folder/Directory icon
- `file.png` - Generic file icon

### Features

- `language.png` - Language selection icon
- `computer.png` - PC/Computer related icon
- `user.png` - User account icon
- `key.png` - Product key icon
- `shield.png` - Security/Bypass features icon
- `windows.png` - Windows logo icon

## Usage

Icons are loaded through the `IconManager` class:

```csharp
// Load a 16x16 icon
var settingsIcon = IconManager.GetIcon(IconManager.Icons.Settings, new Size(16, 16));

// Load with default size
var aboutIcon = IconManager.GetIcon(IconManager.Icons.About);
```

## Fallback Behavior

If an icon file is not found, the `IconManager` will:

1. Try to load from the `icons/` directory
2. Try to load from embedded resources
3. Generate a placeholder icon with the first letter of the icon name

## Adding New Icons

1. Add the PNG file to this directory
2. Add the icon name constant to `IconManager.Icons` class
3. Update this README with the new icon description
