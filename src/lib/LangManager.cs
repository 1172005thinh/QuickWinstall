using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace QuickWinstall.Lib
{
    #region LangManager
    public static class LangManager
    {
        private static readonly string langPath = Path.Combine(AppContext.BaseDirectory, "res", "langs");
        private static readonly Dictionary<string, Dictionary<string, string>> _langCache = new Dictionary<string, Dictionary<string, string>>();
        private static string _currentLang = "en-US";
        private static bool _isInitialized = false;

        public static event EventHandler<LangChangedEventArgs> langChanged;

        #region LangChangedEventArgs
        public class LangChangedEventArgs : EventArgs
        {
            public string oldLang { get; }
            public string newLang { get; }
            public LangChangedEventArgs(string oldlang, string newlang)
            {
                oldLang = oldlang;
                newLang = newlang;
            }
        }
        #endregion

        #region Langs
        public static class Langs
        {
            public const string English = "en-US";
            public const string Vietnamese = "vi-VN";
        }
        #endregion

        #region LangDisplayNames
        public static readonly Dictionary<string, string> LangDisplayNames = new Dictionary<string, string>
        {
            [Langs.English] = "English",
            [Langs.Vietnamese] = "Tiếng Việt"
        };
        #endregion

        #region Initialize
        public static void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                // Load lang from settings first, fallback to default if not found
                var savedLang = SettingsManager.GetSetting("lang");
                if (!string.IsNullOrEmpty(savedLang))
                {
                    _currentLang = savedLang;
                }
                else
                {
                    var defaults = Defaults.LoadFromAppFolder();
                    _currentLang = defaults.LangConfig.Lang;
                }

                LoadLang(_currentLang);

                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine($"[LangManager] Initialized with lang: {_currentLang}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LangManager] Failed to initialize LangManager: {ex.Message}");
                _currentLang = Langs.English;
                _isInitialized = true;
            }
        }
        #endregion

        #region CurrentLang
        public static string CurrentLang
        {
            get
            {
                if (!_isInitialized) Initialize();
                return _currentLang;
            }
        }
        #endregion

        #region SetLang
        public static bool SetLang(string langCode)
        {
            if (string.IsNullOrEmpty(langCode) || langCode == _currentLang) return false;

            try
            {
                var oldLang = _currentLang;
                LoadLang(langCode);
                _currentLang = langCode;

                // Save to settings
                SettingsManager.UpdateSetting("lang", langCode);

                // Fire lang changed event to refresh UI
                langChanged?.Invoke(null, new LangChangedEventArgs(oldLang, langCode));
                System.Diagnostics.Debug.WriteLine($"[LangManager] lang changed to: {_currentLang}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LangManager] Failed to set lang: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region GetString
        public static string GetString(string key, string defaultValue = "")
        {
            Initialize();

            try
            {
                if (_langCache.ContainsKey(_currentLang) &&
                    _langCache[_currentLang].ContainsKey(key))
                {
                    return _langCache[_currentLang][key];
                }

                // Fallback to English if current lang doesn't have the key
                if (_currentLang != Langs.English)
                {
                    LoadLang(Langs.English);
                    if (_langCache.ContainsKey(Langs.English) &&
                        _langCache[Langs.English].ContainsKey(key))
                    {
                        return _langCache[Langs.English][key];
                    }
                }
                return string.IsNullOrEmpty(defaultValue) ? key : defaultValue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting localized string '{key}': {ex.Message}");
                return string.IsNullOrEmpty(defaultValue) ? key : defaultValue;
            }
        }
        #endregion

        #region LoadLang
        private static void LoadLang(string langCode)
        {
            if (_langCache.ContainsKey(langCode)) return;

            string langFilePath = Path.Combine(langPath, $"{langCode}.json");

            if (!File.Exists(langFilePath))
            {
                // Create basic fallback dictionary for missing lang files
                _langCache[langCode] = CreateFallbackStrings(langCode);
                System.Diagnostics.Debug.WriteLine($"Lang file not found: {langFilePath}, using fallback");
                return;
            }

            try
            {
                string json = File.ReadAllText(langFilePath);
                var strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _langCache[langCode] = strings ?? new Dictionary<string, string>();
                System.Diagnostics.Debug.WriteLine($"Loaded lang: {langCode} ({_langCache[langCode].Count} strings)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading lang file {langFilePath}: {ex.Message}");
                _langCache[langCode] = CreateFallbackStrings(langCode);
            }
        }
        #endregion

        #region CreateFallbackStrings
        private static Dictionary<string, string> CreateFallbackStrings(string langCode)
        {
            var fallback = new Dictionary<string, string>();

            if (langCode == Langs.English)
            {
                // English fallback strings
                fallback["AppName"] = "QuickWinstall";
                fallback["Settings"] = "Settings";
                fallback["About"] = "About";
                fallback["Help"] = "Help";
                fallback["Generate"] = "Generate";
                fallback["Cancel"] = "Cancel";
                fallback["Clear"] = "Clear";
                fallback["OK"] = "OK";
            }
            return fallback;
        }
        #endregion

        #region GetAvailableLangs
        public static string[] GetAvailableLangs()
        {
            try
            {
                var defaults = Defaults.LoadFromAppFolder();
                return defaults.LangConfig.LangsAvailable;
            }
            catch
            {
                return new[] { Langs.English, Langs.Vietnamese };
            }
        }
        #endregion

        #region GetLangDisplayName
        public static string GetLangDisplayName(string langCode)
        {
            return LangDisplayNames.ContainsKey(langCode)
                ? LangDisplayNames[langCode]
                : langCode;
        }
        #endregion

        #region LangExists
        public static bool LangExists(string langCode)
        {
            string langFile = Path.Combine(langPath, $"{langCode}.json");
            return File.Exists(langFile);
        }
        #endregion

        #region ClearCache
        public static void ClearCache()
        {
            _langCache.Clear();
            _isInitialized = false;
            System.Diagnostics.Debug.WriteLine("LangManager: Cache cleared");
        }
        #endregion

        #region GetCacheInfo
        public static string GetCacheInfo()
        {
            return $"langs cached: {_langCache.Count}, Current: {_currentLang}, Initialized: {_isInitialized}";
        }
        #endregion
    }
    #endregion

    #region ILangRefreshable
    public interface ILangRefreshable
    {
        void RefreshLang();
    }
    #endregion
}