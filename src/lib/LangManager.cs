using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace QuickWinstall.Lib
{
    public class LangManager
    {
        private static LangManager? _instance;
        private JObject? _currentLanguage;
        private JObject? _fallbackLanguage; // English fallback
        private string _currentLangCode = "en-US";
        private Dictionary<string, string> _hardcodedStrings = new Dictionary<string, string>();

        private LangManager()
        {
            InitializeHardcodedStrings();
            LoadLanguage("en-US");
        }

        public static LangManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LangManager();
                }
                return _instance;
            }
        }

        private void InitializeHardcodedStrings()
        {
            // Hardcoded English strings as ultimate fallback
            _hardcodedStrings = new Dictionary<string, string>
            {
                ["app.name"] = "QuickWinstall",
                ["app.title"] = "Automated Windows 11 Installation",
                ["mainForm.title"] = "QuickWinstall",
                ["mainForm.banner.title"] = "Automated Windows 11 Installation",
                ["mainForm.sections.general"] = "General Configurations",
                ["mainForm.buttons.settings"] = "Settings",
                ["mainForm.buttons.clear"] = "Clear",
                ["mainForm.buttons.preset"] = "Preset",
                ["mainForm.buttons.cancel"] = "Cancel",
                ["mainForm.buttons.generate"] = "Generate",
                ["mainForm.status.prefix"] = "Status:",
                ["mainForm.status.ready"] = "Ready",
                ["generalConfig.windowsEdition.label"] = "Windows Edition",
                ["generalConfig.productKey.label"] = "Product Key",
                ["generalConfig.cpuArch.label"] = "CPU Architecture",
                ["dialogs.buttons.ok"] = "OK",
                ["dialogs.buttons.cancel"] = "Cancel"
            };
        }

        public void LoadLanguage(string langCode)
        {
            try
            {
                string langFile = $"{langCode}.json";
                string langPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "langs", langFile);

                if (File.Exists(langPath))
                {
                    string jsonContent = File.ReadAllText(langPath);
                    _currentLanguage = JObject.Parse(jsonContent);
                    _currentLangCode = langCode;

                    // Load English as fallback if not already loaded and current is not English
                    if (langCode != "en-US" && _fallbackLanguage == null)
                    {
                        string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "langs", "en-US.json");
                        if (File.Exists(fallbackPath))
                        {
                            string fallbackContent = File.ReadAllText(fallbackPath);
                            _fallbackLanguage = JObject.Parse(fallbackContent);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Language file not found: {langPath}. Using defaults.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load language: {ex.Message}. Using defaults.");
            }
        }

        public string GetString(string key, params object[] args)
        {
            string result = GetStringInternal(key);

            // Escape special characters BEFORE formatting with parameters
            // This prevents escape sequences in parameter values from being processed
            result = LangHelper.EscapeString(result);

            // Format with parameters if provided
            if (args != null && args.Length > 0)
            {
                result = LangHelper.FormatString(result, args);
            }

            return result;
        }

        private string GetStringInternal(string key)
        {
            try
            {
                // Try current language
                string? value = GetValueFromJson(_currentLanguage, key);
                if (!string.IsNullOrEmpty(value))
                    return value;

                // Try fallback language
                value = GetValueFromJson(_fallbackLanguage, key);
                if (!string.IsNullOrEmpty(value))
                    return value;

                // Try hardcoded strings
                if (_hardcodedStrings.ContainsKey(key))
                    return _hardcodedStrings[key];

                // Return key itself if not found
                Console.WriteLine($"Warning: Translation key not found: {key}");
                return $"[{key}]";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get string for key {key}: {ex.Message}");
                return $"[{key}]";
            }
        }

        private string? GetValueFromJson(JObject? json, string key)
        {
            if (json == null || string.IsNullOrEmpty(key))
                return null;

            try
            {
                // Split key by dots and navigate through JSON
                string[] parts = key.Split('.');
                JToken? current = json;

                foreach (string part in parts)
                {
                    current = current?[part];
                    if (current == null)
                        return null;
                }

                return current?.ToString();
            }
            catch
            {
                return null;
            }
        }

        public string CurrentLanguage => _currentLangCode;

        public List<string> GetAvailableLanguages()
        {
            List<string> languages = new List<string>();

            try
            {
                string langsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "langs");
                if (Directory.Exists(langsPath))
                {
                    foreach (string file in Directory.GetFiles(langsPath, "*.json"))
                    {
                        string langCode = Path.GetFileNameWithoutExtension(file);
                        languages.Add(langCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get available languages: {ex.Message}");
            }

            // Ensure at least en-US is available
            if (!languages.Contains("en-US"))
            {
                languages.Add("en-US");
            }

            return languages;
        }

        /// <summary>
        /// Gets a translated string from a specific language without switching the current language
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="langCode">The language code (e.g., "en-US", "vi-VN")</param>
        /// <returns>The translated string or null if not found</returns>
        public string? GetStringFromLanguage(string key, string langCode)
        {
            try
            {
                string langFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "langs", $"{langCode}.json");
                if (!File.Exists(langFilePath))
                    return null;

                string jsonContent = File.ReadAllText(langFilePath);
                JObject? langJson = JObject.Parse(jsonContent);
                return GetValueFromJson(langJson, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get string '{key}' from language '{langCode}': {ex.Message}");
                return null;
            }
        }
    }
}
