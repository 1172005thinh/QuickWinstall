using System;
using System.IO;
using System.Text.Json;

namespace QuickWinstall
{
    public class AppSettings
    {
        public string UILanguage { get; set; } = "en-US";
        public string DefaultFileLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        
        // General Settings
        public string PCName { get; set; } = "PC";
        public string SystemLocale { get; set; } = "en-US";
        public string UserLocale { get; set; } = "en-US";
        public bool SameAsSystemLocale { get; set; } = true;
        public string WindowsEdition { get; set; } = "Windows 11 Pro";
        public string CPUArchitecture { get; set; } = "Intel/AMD 64-bit";
        public string TimeZone { get; set; } = "Pacific Standard Time";
        public string ProductKey { get; set; } = "";

        // Account Settings
        public string Account1Name { get; set; } = "OEM";
        public string Account1DisplayName { get; set; } = "OEM";
        public bool Account1SameAsName { get; set; } = true;
        public string Account1Type { get; set; } = "Administrators";

        public string Account2Name { get; set; } = "User";
        public string Account2DisplayName { get; set; } = "User";
        public bool Account2SameAsName { get; set; } = true;
        public string Account2Type { get; set; } = "Users";

        // Bypass Settings
        public bool BypassAllChecks { get; set; } = true;
        public bool BypassTPM { get; set; } = true;
        public bool BypassRAM { get; set; } = true;
        public bool BypassSecureBoot { get; set; } = true;
        public bool BypassCPU { get; set; } = true;
        public bool BypassStorage { get; set; } = true;
        public bool BypassDisk { get; set; } = true;

        // BitLocker Settings
        public bool DisableBitLocker { get; set; } = true;

        private static string SettingsFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AutounattendGenerator", "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? GetDefault();
                }
            }
            catch
            {
                // If loading fails, return default settings
            }

            return GetDefault();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
                // Handle save errors silently
            }
        }

        public static AppSettings GetDefault()
        {
            return new AppSettings();
        }

        public AppSettings Clone()
        {
            var json = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }
    }
}