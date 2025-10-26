using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace QuickWinstall.Lib
{
    public class SettingsManager
    {
        private static SettingsManager? _instance;
        private JObject? _settings;
        private string _settingsPath;

        // Default settings
        private const string DEFAULT_THEME = "Light";
        private const string DEFAULT_LANGUAGE = "en-US";
        private const bool DEFAULT_SAVE_LAST_CONFIG = true;
        private const bool DEFAULT_LOAD_LAST_CONFIG = true;

        public string Theme { get; set; } = DEFAULT_THEME;
        public string Language { get; set; } = DEFAULT_LANGUAGE;
        public string SavePath { get; set; } = "";
        public bool SaveLastConfig { get; set; } = DEFAULT_SAVE_LAST_CONFIG;
        public bool LoadLastConfig { get; set; } = DEFAULT_LOAD_LAST_CONFIG;

        private SettingsManager()
        {
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "main", "settings.json");
            LoadSettings();
        }

        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsManager();
                }
                return _instance;
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string jsonContent = File.ReadAllText(_settingsPath);
                    _settings = JObject.Parse(jsonContent);

                    // Load settings
                    Theme = _settings["theme"]?.ToString() ?? DEFAULT_THEME;
                    Language = _settings["language"]?.ToString() ?? DEFAULT_LANGUAGE;
                    SavePath = _settings["savePath"]?.ToString() ?? "";
                    SaveLastConfig = _settings["saveLastConfig"]?.ToObject<bool>() ?? DEFAULT_SAVE_LAST_CONFIG;
                    LoadLastConfig = _settings["loadLastConfig"]?.ToObject<bool>() ?? DEFAULT_LOAD_LAST_CONFIG;

                    // Set default save path if empty
                    if (string.IsNullOrEmpty(SavePath))
                    {
                        SavePath = AppDomain.CurrentDomain.BaseDirectory;
                    }
                }
                else
                {
                    // Create default settings file
                    CreateDefaultSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load settings: {ex.Message}. Using defaults.");
                CreateDefaultSettings();
            }
        }

        private void CreateDefaultSettings()
        {
            try
            {
                Theme = DEFAULT_THEME;
                Language = DEFAULT_LANGUAGE;
                SavePath = AppDomain.CurrentDomain.BaseDirectory;
                SaveLastConfig = DEFAULT_SAVE_LAST_CONFIG;
                LoadLastConfig = DEFAULT_LOAD_LAST_CONFIG;

                SaveSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to create default settings: {ex.Message}");
            }
        }

        public void SaveSettings()
        {
            try
            {
                _settings = new JObject
                {
                    ["theme"] = Theme,
                    ["language"] = Language,
                    ["savePath"] = SavePath,
                    ["saveLastConfig"] = SaveLastConfig,
                    ["loadLastConfig"] = LoadLastConfig
                };

                // Ensure directory exists
                string? directory = Path.GetDirectoryName(_settingsPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_settingsPath, _settings.ToString(Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to save settings: {ex.Message}");
            }
        }

        public void ApplySettings()
        {
            // Apply theme
            ThemeManager.Instance.LoadTheme(Theme);

            // Apply language
            LangManager.Instance.LoadLanguage(Language);
        }
    }
}
