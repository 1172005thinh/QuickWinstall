using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace QuickWinstall.Lib
{
    #region SettingsManager
    public static class SettingsManager
    {
        private static readonly string settingsPath = Path.Combine(AppContext.BaseDirectory, "src", "main", "settings.json");
        private static AppSettings _cachedSettings;

        #region AppSettings
        public class AppSettings
        {
            // App-level settings (unique to SettingsManager)
            [JsonPropertyName("lang")]
            public string Lang { get; set; } = "en-US";

            [JsonPropertyName("theme")]
            public string Theme { get; set; } = "Light";

            [JsonPropertyName("savePath")]
            public string SavePath { get; set; } = "";

            [JsonPropertyName("lastModified")]
            public DateTime LastModified { get; set; } = DateTime.Now;

            [JsonPropertyName("generalConfig")]
            public GeneralConfigDefaults GeneralConfig { get; set; }

            [JsonPropertyName("langRegionConfig")]
            public LangRegionConfigDefaults LangRegionConfig { get; set; }

            [JsonPropertyName("bypassConfig")]
            public BypassConfigDefaults BypassConfig { get; set; }

            [JsonPropertyName("diskConfig")]
            public DiskConfigDefaults DiskConfig { get; set; }

            [JsonPropertyName("accountConfig")]
            public AccountConfigDefaults AccountConfig { get; set; }

            [JsonPropertyName("oobeConfig")]
            public OOBEConfigDefaults OOBEConfig { get; set; }

            [JsonPropertyName("bitLockerConfig")]
            public BitLockerConfigDefaults BitLockerConfig { get; set; }

            [JsonPropertyName("personalizeConfig")]
            public PersonalizeConfigDefaults PersonalizeConfig { get; set; }

            [JsonPropertyName("appConfig")]
            public AppConfigDefaults AppConfig { get; set; }
        }
        #endregion

        #region LoadSettings
        public static AppSettings LoadSettings()
        {
            if (_cachedSettings != null)
                return _cachedSettings;

            try
            {
                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    });

                    _cachedSettings = settings ?? CreateDefaultConfigs();
                }
                else
                {
                    _cachedSettings = CreateDefaultConfigs();
                    SaveSettings(_cachedSettings);
                }
                System.Diagnostics.Debug.WriteLine($"SettingsManager: Loaded settings - Language: {_cachedSettings.Lang}, Theme: {_cachedSettings.Theme}");
                return _cachedSettings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsManager: Error loading settings: {ex.Message}");
                _cachedSettings = CreateDefaultConfigs();
                return _cachedSettings;
            }
        }
        #endregion

        #region SaveSettings
        public static bool SaveSettings(AppSettings settings)
        {
            try
            {
                settings.LastModified = DateTime.Now;

                // Ensure directory exists
                var directory = Path.GetDirectoryName(settingsPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                });

                File.WriteAllText(settingsPath, json);
                _cachedSettings = settings; // Update cache

                System.Diagnostics.Debug.WriteLine($"SettingsManager: Settings saved - Lang: {settings.Lang}, Theme: {settings.Theme}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsManager: Error saving settings: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region UpdateSetting
        public static bool UpdateSetting(string key, object value)
        {
            try
            {
                var settings = LoadSettings();

                switch (key.ToLowerInvariant())
                {
                    case "lang":
                        settings.Lang = value?.ToString() ?? "en-US";
                        break;
                    case "theme":
                        settings.Theme = value?.ToString() ?? "Light";
                        break;
                    case "savepath":
                        settings.SavePath = value?.ToString() ?? "";
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"SettingsManager: Unknown setting key: {key}");
                        return false;
                }

                return SaveSettings(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsManager: Error updating setting {key}: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region GetSetting
        public static string GetSetting(string key)
        {
            try
            {
                var settings = LoadSettings();

                return key.ToLowerInvariant() switch
                {
                    "lang" => settings.Lang,
                    "theme" => settings.Theme,
                    "savepath" => settings.SavePath,
                    _ => ""
                };
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region CreateDefaultConfigs
        private static AppSettings CreateDefaultConfigs()
        {
            return new AppSettings
            {
                Lang = "en-US",
                Theme = "Light",
                SavePath = Application.StartupPath,
                // Section configs with default values matching default.json
                GeneralConfig = new GeneralConfigDefaults(
                    Expanded: true,
                    WindowsEdition: "",
                    ProductKey: new[] { "" },
                    CPUArchitecture: ""
                ),
                LangRegionConfig = new LangRegionConfigDefaults(
                    Expanded: true,
                    SystemLocale: "",
                    UserLocale: "",
                    SameAsSystemLocale: false,
                    WindowsUILanguage: "",
                    KeyboardLayout: "",
                    TimeZone: ""
                ),
                BypassConfig = new BypassConfigDefaults(
                    Expanded: true,
                    BypassAllChecks: false,
                    BypassTPMCheck: false,
                    BypassSecureBootCheck: false,
                    BypassCPUCheck: false,
                    BypassRAMCheck: false,
                    BypassDiskSpaceCheck: false,
                    BypassStorageCheck: false
                ),
                DiskConfig = new DiskConfigDefaults(
                    Expanded: true,
                    MinEntries: 0,
                    MaxEntries: 10,
                    EnableDiskConfiguration: false,
                    PartitionLayout: "",
                    Partitions: Array.Empty<PartitionDefaults>()
                ),
                AccountConfig = new AccountConfigDefaults(
                    Expanded: true,
                    MinEntries: 0,
                    MaxEntries: 10,
                    EnableLocalAccountCreation: false,
                    Accounts: Array.Empty<AccountDefaults>()
                ),
                OOBEConfig = new OOBEConfigDefaults(
                    Expanded: true,
                    SkipAndHideAll: false,
                    SkipEULAs: false,
                    SkipLocalAccount: false,
                    SkipOnlineAccount: false,
                    SkipWirelessSetup: false,
                    SkipMachineOOBE: false,
                    SkipUserOOBE: false
                ),
                BitLockerConfig = new BitLockerConfigDefaults(
                    Expanded: true,
                    DisableBitLocker: false
                ),
                PersonalizeConfig = new PersonalizeConfigDefaults(
                    Expanded: true,
                    ComputerName: ""
                ),
                AppConfig = new AppConfigDefaults(
                    Expanded: true
                )
            };
        }
        #endregion

        #region ClearCache
        public static void ClearCache()
        {
            _cachedSettings = null;
            System.Diagnostics.Debug.WriteLine("SettingsManager: Cache cleared");
        }
        #endregion

        #region SettingsFileExists
        public static bool SettingsFileExists()
        {
            return File.Exists(settingsPath);
        }
        #endregion
    }
    #endregion
}
