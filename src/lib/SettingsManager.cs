using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

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
            // Basic settings
            [JsonPropertyName("lang")]
            public string Lang { get; set; } = "en-US";

            [JsonPropertyName("theme")]
            public string Theme { get; set; } = "Light";

            [JsonPropertyName("xmlSavePath")]
            public string XmlSavePath { get; set; } = "";

            [JsonPropertyName("lastModified")]
            public DateTime LastModified { get; set; } = DateTime.Now;

            // GeneralConfig settings
            [JsonPropertyName("generalConfig")]
            public GeneralConfigSettings GeneralConfig { get; set; } = new GeneralConfigSettings();

            [JsonPropertyName("langRegionConfig")]
            public LangRegionConfigSettings LangRegionConfig { get; set; } = new LangRegionConfigSettings();

            [JsonPropertyName("bypassConfig")]
            public BypassConfigSettings BypassConfig { get; set; } = new BypassConfigSettings();

            [JsonPropertyName("diskConfig")]
            public DiskConfigSettings DiskConfig { get; set; } = new DiskConfigSettings();

            [JsonPropertyName("accountConfig")]
            public AccountConfigSettings AccountConfig { get; set; } = new AccountConfigSettings();

            [JsonPropertyName("oobeConfig")]
            public OOBEConfigSettings OOBEConfig { get; set; } = new OOBEConfigSettings();

            [JsonPropertyName("bitLockerConfig")]
            public BitLockerConfigSettings BitLockerConfig { get; set; } = new BitLockerConfigSettings();
            [JsonPropertyName("personalizeConfig")]
            public PersonalizeConfigSettings PersonalizeConfig { get; set; } = new PersonalizeConfigSettings();
            [JsonPropertyName("appConfig")]
            public AppConfigSettings AppConfig { get; set; } = new AppConfigSettings();
        }
        #endregion

        #region GeneralConfigSettings
        public class GeneralConfigSettings
        {
            [JsonPropertyName("windowsEdition")]
            public string WindowsEdition { get; set; } = "";

            [JsonPropertyName("productKey")]
            public string[] ProductKey { get; set; } = new string[5] { "", "", "", "", "" };

            [JsonPropertyName("cpuArchitecture")]
            public string CPUArchitecture { get; set; } = "";
        }
        #endregion

        #region LangRegionConfigSettings
        /// LangRegionConfig settings structure
        public class LangRegionConfigSettings
        {
            [JsonPropertyName("systemLocale")]
            public string SystemLocale { get; set; } = "";

            [JsonPropertyName("userLocale")]
            public string UserLocale { get; set; } = "";

            [JsonPropertyName("sameAsSystemLocale")]
            public bool SameAsSystemLocale { get; set; } = true;

            [JsonPropertyName("windowsUILanguage")]
            public string WindowsUILanguage { get; set; } = "";

            [JsonPropertyName("keyboardLayout")]
            public string KeyboardLayout { get; set; } = "";

            [JsonPropertyName("timeZone")]
            public string TimeZone { get; set; } = "";
        }
        #endregion

        #region BypassConfigSettings
        public class BypassConfigSettings
        {
            [JsonPropertyName("bypassAll")]
            public bool BypassAll { get; set; } = true;

            [JsonPropertyName("bypassTPM")]
            public bool BypassTPM { get; set; } = true;

            [JsonPropertyName("bypassSecureBoot")]
            public bool BypassSecureBoot { get; set; } = true;

            [JsonPropertyName("bypassRAM")]
            public bool BypassRAM { get; set; } = true;

            [JsonPropertyName("bypassCPU")]
            public bool BypassCPU { get; set; } = true;

            [JsonPropertyName("bypassDiskSpace")]
            public bool BypassDiskSpace { get; set; } = true;

            [JsonPropertyName("bypassStorage")]
            public bool BypassStorage { get; set; } = true;
        }
        #endregion

        #region DiskConfigSettings
        public class DiskConfigSettings
        {
            [JsonPropertyName("enableDiskConfiguration")]
            public bool EnableDiskConfiguration { get; set; } = true;

            [JsonPropertyName("partitionLayout")]
            public string PartitionLayout { get; set; } = "";

            [JsonPropertyName("partitions")]
            public List<PartitionData> Partitions { get; set; } = new List<PartitionData>();
        }

        public class PartitionData
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; } = "";

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("sizeMB")]
            public int SizeMB { get; set; }

            [JsonPropertyName("active")]
            public bool Active { get; set; }

            [JsonPropertyName("format")]
            public bool Format { get; set; }

            [JsonPropertyName("letter")]
            public string Letter { get; set; } = "";
        }
        #endregion

        #region AccountConfigSettings
        public class AccountConfigSettings
        {
            [JsonPropertyName("enableLocalAccountCreation")]
            public bool EnableLocalAccountCreation { get; set; } = true;

            [JsonPropertyName("accounts")]
            public List<AccountData> Accounts { get; set; } = new List<AccountData>();
        }

        public class AccountData
        {
            [JsonPropertyName("id")]
            public int ID { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; } = "";

            [JsonPropertyName("username")]
            public string Username { get; set; } = "";

            [JsonPropertyName("displayName")]
            public string DisplayName { get; set; } = "";

            [JsonPropertyName("password")]
            public string Password { get; set; } = "";

            [JsonPropertyName("autoLogin")]
            public bool AutoLogin { get; set; }
        }
        #endregion

        #region OOBEConfigSettings
        public class OOBEConfigSettings
        {
            [JsonPropertyName("skipAndHideAll")]
            public bool SkipAndHideAll { get; set; } = true;

            [JsonPropertyName("skipEULAs")]
            public bool SkipEULAs { get; set; } = true;

            [JsonPropertyName("skipLocalAccount")]
            public bool SkipLocalAccount { get; set; } = true;

            [JsonPropertyName("skipOnlineAccount")]
            public bool SkipOnlineAccount { get; set; } = true;

            [JsonPropertyName("skipWirelessSetup")]
            public bool SkipWirelessSetup { get; set; } = true;

            [JsonPropertyName("skipMachineOOBE")]
            public bool SkipMachineOOBE { get; set; } = true;

            [JsonPropertyName("skipUserOOBE")]
            public bool SkipUserOOBE { get; set; } = true;
        }
        #endregion

        #region BitLockerConfigSettings
        public class BitLockerConfigSettings
        {
            [JsonPropertyName("disableBitLocker")]
            public bool DisableBitLocker { get; set; } = false;
        }
        #endregion

        #region PersonalizeConfigSettings
        public class PersonalizeConfigSettings
        {
            [JsonPropertyName("computerName")]
            public string ComputerName { get; set; } = "";
        }
        #endregion

        #region AppConfigSettings
        public class AppConfigSettings
        {

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

                    _cachedSettings = settings ?? CreateDefaultSettings();
                }
                else
                {
                    _cachedSettings = CreateDefaultSettings();
                    SaveSettings(_cachedSettings);
                }
                System.Diagnostics.Debug.WriteLine($"SettingsManager: Loaded settings - Language: {_cachedSettings.Lang}, Theme: {_cachedSettings.Theme}");
                return _cachedSettings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsManager: Error loading settings: {ex.Message}");
                _cachedSettings = CreateDefaultSettings();
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
                    case "xmlsavepath":
                        settings.XmlSavePath = value?.ToString() ?? "";
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
                    "xmlsavepath" => settings.XmlSavePath,
                    _ => ""
                };
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region CreateDefaultSettings
        private static AppSettings CreateDefaultSettings()
        {
            return new AppSettings
            {
                Lang = "en-US",
                Theme = "Light",
                XmlSavePath = Application.StartupPath
            };
        }
        #endregion

        #region ClearCache
        public static void ClearCache()
        {
            _cachedSettings = null;
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