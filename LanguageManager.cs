using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace QuickWinstall
{
    public class LanguageManager
    {
        private Dictionary<string, string> _strings;
        private string _currentLanguage;
        private static LanguageManager _instance;

        public static LanguageManager Instance => _instance ??= new LanguageManager();

        public string CurrentLanguage => _currentLanguage;

        private LanguageManager()
        {
            _strings = new Dictionary<string, string>();
            _currentLanguage = "en-US";
            LoadLanguage("en-US"); // Default language
        }

        public bool LoadLanguage(string languageCode)
        {
            try
            {
                string langPath = Path.Combine(Application.StartupPath ?? "", "lang", languageCode, "strings.json");
                
                // Fallback to development path if not found
                if (!File.Exists(langPath))
                {
                    langPath = Path.Combine(Directory.GetCurrentDirectory(), "lang", languageCode, "strings.json");
                }

                if (!File.Exists(langPath))
                {
                    // If language file doesn't exist, keep current language
                    return false;
                }

                string jsonContent = File.ReadAllText(langPath);
                var languageStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                if (languageStrings != null)
                {
                    _strings = languageStrings;
                    _currentLanguage = languageCode;
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Error loading language {languageCode}: {ex.Message}");
            }

            return false;
        }

        public string GetString(string key, string defaultValue = null)
        {
            if (_strings.TryGetValue(key, out string value))
            {
                return value;
            }

            // Return the key or default value if string not found
            return defaultValue ?? key;
        }

        public List<LanguageInfo> GetAvailableLanguages()
        {
            var languages = new List<LanguageInfo>();
            
            string langBasePath = Path.Combine(Application.StartupPath ?? "", "lang");
            
            // Fallback to development path
            if (!Directory.Exists(langBasePath))
            {
                langBasePath = Path.Combine(Directory.GetCurrentDirectory(), "lang");
            }

            if (Directory.Exists(langBasePath))
            {
                foreach (string langDir in Directory.GetDirectories(langBasePath))
                {
                    string langCode = Path.GetFileName(langDir);
                    string stringsFile = Path.Combine(langDir, "strings.json");
                    
                    if (File.Exists(stringsFile))
                    {
                        try
                        {
                            var culture = new CultureInfo(langCode);
                            languages.Add(new LanguageInfo
                            {
                                Code = langCode,
                                Name = culture.DisplayName,
                                NativeName = culture.NativeName
                            });
                        }
                        catch
                        {
                            // Skip invalid culture codes
                        }
                    }
                }
            }

            // Add default languages if directory doesn't exist
            if (languages.Count == 0)
            {
                languages.Add(new LanguageInfo { Code = "en-US", Name = "English (United States)", NativeName = "English (United States)" });
                languages.Add(new LanguageInfo { Code = "vi-VN", Name = "Vietnamese (Vietnam)", NativeName = "Tiếng Việt (Việt Nam)" });
            }

            return languages;
        }

        public void ApplyCultureToThread(string languageCode)
        {
            try
            {
                var culture = new CultureInfo(languageCode);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch
            {
                // Fallback to default culture if invalid
                var defaultCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = defaultCulture;
                Thread.CurrentThread.CurrentCulture = defaultCulture;
            }
        }
    }

    public class LanguageInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }

        public override string ToString()
        {
            return NativeName;
        }
    }
}