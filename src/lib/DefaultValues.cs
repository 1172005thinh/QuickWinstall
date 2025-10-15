using System;
using System.IO;
using System.Text.Json;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    public record GeneralConfigDefaults(
        bool        Expanded,
        string      WindowsEdition,
        string[]    ProductKey,
        string      CPUArchitecture);

    public record LangRegionConfigDefaults(
        bool        Expanded,
        string      SystemLocale,
        string      UserLocale,
        bool        SameAsSystemLocale,
        string      WindowsUILanguage,
        string      KeyboardLayout,
        string      TimeZone);

    public record BypassConfigDefaults(
        bool        Expanded,
        bool        BypassAllChecks,
        bool        BypassTPMCheck,
        bool        BypassSecureBootCheck,
        bool        BypassCPUCheck,
        bool        BypassRAMCheck,
        bool        BypassDiskSpaceCheck,
        bool        BypassStorageCheck);

    public record PartitionDefaults(
        int         ID,
        string      Type,
        string      Name,
        int         SizeMB,
        bool        Active,
        bool        Format,
        string      Letter);

    public record DiskConfigDefaults(
        bool        Expanded,
        int         MinEntries,
        int         MaxEntries,
        bool        EnableDiskConfiguration,
        string      PartitionLayout,
        PartitionDefaults[] Partitions);

    public record AccountDefaults(
        int         ID,
        string      Type,
        string      Username,
        string      DisplayName,
        string      Password,
        bool        AutoLogin);

    public record AccountConfigDefaults(
        bool        Expanded,
        int         MinEntries,
        int         MaxEntries,
        bool        EnableLocalAccountCreation,
        AccountDefaults[] Accounts);

    public record BitLockerConfigDefaults(
        bool        Expanded,
        bool        DisableBitLocker);

    public record OOBEConfigDefaults(
        bool        Expanded,
        bool        SkipAndHideAll,
        bool        SkipEULAs,
        bool        SkipLocalAccount,
        bool        SkipOnlineAccount,
        bool        SkipWirelessSetup,
        bool        SkipMachineOOBE,
        bool        SkipUserOOBE);

    public record PersonalizeConfigDefaults(
        bool        Expanded,
        string      ComputerName);

    public record AppConfigDefaults(
        bool        Expanded);

    public record LangSettings(
        string Lang,
        string[] LangsAvailable);

    public record ThemeSettings(
        string Theme,
        string[] ThemesAvailable);

    public class Defaults
    {
        public GeneralConfigDefaults GeneralConfig { get; init; }
        public LangRegionConfigDefaults LangRegionConfig { get; init; }
        public BypassConfigDefaults BypassChecksConfig { get; init; }
        public DiskConfigDefaults DiskConfig { get; init; }
        public AccountConfigDefaults AccountConfig { get; init; }
        public OOBEConfigDefaults OOBEConfig { get; init; }
        public BitLockerConfigDefaults BitLockerConfig { get; init; }
        public PersonalizeConfigDefaults PersonalizeConfig { get; init; }
        public AppConfigDefaults AppConfig { get; init; }
        public LangSettings LangSettings { get; init; }
        public ThemeSettings ThemeSettings { get; init; }

        public static Defaults Load(string path)
        {
            if (!File.Exists(path)) 
                throw new FileNotFoundException(LangManager.GetString("DefaultsFileNotFound", "Defaults file not found."), path);
            
            var json = File.ReadAllText(path);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Defaults>(json, opts) 
                ?? throw new InvalidOperationException(LangManager.GetString("FailedToParseDefaults", "Failed to parse defaults."));
        }

        // Convenience: load from Application output directory (default.json must be copied to output)
        public static Defaults LoadFromAppFolder()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "src", "main", "default.json");
            return Load(path);
        }
    }
}