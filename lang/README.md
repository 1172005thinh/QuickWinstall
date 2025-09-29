# Language Packs Directory

This directory contains localization files for the QuickWinstall application.

## Structure

Each language has its own subdirectory named with the language code:

```plaintext
lang/
├── en-US/          # English (United States)
│   └── strings.json
├── vi-VN/          # Vietnamese (Vietnam)
│   └── strings.json
└── [more languages...]
```

## Supported Languages

- **en-US**: English (United States) - Default language
- **vi-VN**: Vietnamese (Vietnam) - Tiếng Việt

## Language File Format

Each `strings.json` file contains key-value pairs for localized strings:

```json
{
  "MainForm_Title": "QuickWinstall - Autounattend Generator",
  "MainForm_GeneralTab": "General",
  "ValidationError_PCNameRequired": "PC Name is required"
}
```

## Adding a New Language

1. Create a new directory with the language code (e.g., `fr-FR` for French)
2. Copy `en-US/strings.json` to the new directory
3. Translate all string values to the target language
4. Test the translation in the application

## Usage

The `LanguageManager` class handles loading and switching languages:

```csharp
// Load a specific language
LanguageManager.Instance.LoadLanguage("vi-VN");

// Get a localized string
string title = LanguageManager.Instance.GetString("MainForm_Title");

// Get available languages
var languages = LanguageManager.Instance.GetAvailableLanguages();
```

## String Key Conventions

- **MainForm_**: Main form controls and labels
- **AboutForm_**: About dialog strings
- **SettingsForm_**: Settings dialog strings  
- **ValidationError_**: Input validation messages
- **Success_**: Success messages
- **Error_**: Error messages
- **Common_**: Common UI elements (OK, Cancel, etc.)
- **WindowsEditions_**: Windows edition names
- **AccountTypes_**: User account type names

## Guidelines for Translators

1. Keep translations concise to fit in UI controls
2. Maintain consistent terminology throughout
3. Use appropriate formal/informal tone for the target culture
4. Test translations with longer text to ensure UI layout works
5. Preserve placeholder markers like `{0}`, `{1}` if present

## Character Encoding

All JSON files should be saved in UTF-8 encoding to support international characters.
