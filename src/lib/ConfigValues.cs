using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using QuickWinstall.Config;

namespace QuickWinstall.Lib
{
    public class ConfigValues
    {
        #region Singleton

        private static ConfigValues? _instance;

        private ConfigValues()
        {
            General = new GeneralConfig();
            LangReg = new LangRegConfig();
            Bypass = new BypassConfig();
            DiskPart = new DiskPartConfig();
            UserAcc = new UserAccConfig();
            OOBE = new OOBEConfig();
            Personal = new PersonalConfig();
            App = new AppConfig();
            _emptyConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "config", "empty.json");
        }

        public static ConfigValues Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigValues();
                }
                return _instance;
            }
        }

        #endregion

        #region Fields

        private readonly string _emptyConfigPath;

        #endregion

        #region Properties

        // Configuration sections
        public GeneralConfig General { get; set; }
        public LangRegConfig LangReg { get; set; }
        public BypassConfig Bypass { get; set; }
        public DiskPartConfig DiskPart { get; set; }
        public UserAccConfig UserAcc { get; set; }
        public OOBEConfig OOBE { get; set; }
        public PersonalConfig Personal { get; set; }
        public AppConfig App { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all configuration values by loading from empty.json
        /// </summary>
        public void Clear()
        {
            try
            {
                if (File.Exists(_emptyConfigPath))
                {
                    string jsonContent = File.ReadAllText(_emptyConfigPath);
                    JObject emptyConfig = JObject.Parse(jsonContent);

                    // Preserve Enable states before clearing
                    bool generalEnabled = General.EnableGeneral;
                    bool langRegEnabled = LangReg.EnableLangReg;
                    bool bypassEnabled = Bypass.EnableBypass;
                    bool diskPartEnabled = DiskPart.EnableDiskPart;
                    //bool userAccEnabled = UserAcc.EnableUserAcc;
                    bool oobeEnabled = OOBE.EnableOOBE;
                    //bool personalEnabled = Personal.EnablePersonal;
                    //bool appEnabled = App.EnableApp;

                    // Load empty values for General section
                    if (emptyConfig["general"] is JObject generalSection)
                    {
                        General.SetValues(generalSection);
                    }

                    // Load empty values for LangReg section
                    if (emptyConfig["langReg"] is JObject langRegSection)
                    {
                        LangReg.SetValues(langRegSection);
                    }

                    // Load empty values for Bypass section
                    if (emptyConfig["bypass"] is JObject bypassSection)
                    {
                        Bypass.SetValues(bypassSection);
                    }

                    // Load empty values for DiskPart section
                    if (emptyConfig["diskPart"] is JObject diskPartSection)
                    {
                        DiskPart.SetValues(diskPartSection);
                    }

                    // Load empty values for UserAcc section
                    // if (emptyConfig["userAcc"] is JObject userAccSection)
                    // {
                    //     UserAcc.SetValues(userAccSection);
                    // }

                    // Load empty values for OOBE section
                    if (emptyConfig["oobe"] is JObject oobeSection)
                    {
                        OOBE.SetValues(oobeSection);
                    }

                    // Load empty values for Personal section
                    // if (emptyConfig["personal"] is JObject personalSection)
                    // {
                    //     Personal.SetValues(personalSection);
                    // }

                    // Load empty values for App section
                    // if (emptyConfig["app"] is JObject appSection)
                    // {
                    //     App.SetValues(appSection);
                    // }

                    // Restore Enable states after clearing data models
                    General.EnableGeneral = generalEnabled;
                    LangReg.EnableLangReg = langRegEnabled;
                    Bypass.EnableBypass = bypassEnabled;
                    DiskPart.EnableDiskPart = diskPartEnabled;
                    //UserAcc.EnableUserAcc = userAccEnabled;
                    OOBE.EnableOOBE = oobeEnabled;
                    //Personal.EnablePersonal = personalEnabled;
                    //App.EnableApp = appEnabled;
                }
                else
                {
                    // Fallback to hardcoded clear if empty.json not found
                    Console.WriteLine($"Warning: empty.json not found at {_emptyConfigPath}. Using hardcoded empty values.");
                    General.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing config from empty.json: {ex.Message}. Using hardcoded empty values.");
                General.Clear();
            }
        }

        public List<string> Validate()
        {
            List<string> errors = new List<string>();
            
            errors.AddRange(General.Validate());
            errors.AddRange(LangReg.Validate());
            //errors.AddRange(Bypass.Validate());
            errors.AddRange(DiskPart.Validate());
            //errors.AddRange(UserAcc.Validate());
            errors.AddRange(OOBE.Validate());
            //errors.AddRange(Personal.Validate());
            //errors.AddRange(App.Validate());

            return errors;
        }

        public Dictionary<string, string> GetAllValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            // Get values from all sections
            foreach (var kvp in General.GetValues())
            {
                values[kvp.Key] = kvp.Value;
            }
            
            foreach (var kvp in LangReg.GetValues())
            {
                values[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in Bypass.GetValues())
            {
                values[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in DiskPart.GetValues())
            {
                values[kvp.Key] = kvp.Value;
            }

            //foreach (var kvp in UserAcc.GetValues())
            //{
            //    values[kvp.Key] = kvp.Value;
            //}

            foreach (var kvp in OOBE.GetValues())
            {
                values[kvp.Key] = kvp.Value;
            }

            //foreach (var kvp in Personal.GetValues())
            //{
            //    values[kvp.Key] = kvp.Value;
            //}

            //foreach (var kvp in App.GetValues())
            //{
            //    values[kvp.Key] = kvp.Value;
            //}

            return values;
        }

        /// <summary>
        /// Saves the current configuration to lastConfig.json if autosave is enabled
        /// </summary>
        public void SaveLastConfig()
        {
            try
            {
                // Check if autosave is enabled
                if (!SettingsManager.Instance.SaveLastConfig)
                {
                    Console.WriteLine("SaveLastConfig is disabled in settings.");
                    return;
                }

                string lastConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "presets", "lastConfig.json");
                Console.WriteLine($"Attempting to save config to: {lastConfigPath}");

                // Create directory if it doesn't exist
                string? directory = Path.GetDirectoryName(lastConfigPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine($"Created directory: {directory}");
                }

                // Update data model from UI controls
                try
                {
                    General.UpdateFromControls();
                    LangReg.UpdateFromControls();
                    Bypass.UpdateFromControls();
                    DiskPart.UpdateFromControls();
                    //UserAcc.UpdateFromControls();
                    OOBE.UpdateFromControls();
                    //Personal.UpdateFromControls();
                    //App.UpdateFromControls();
                    Console.WriteLine("Updated config from UI controls.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not update from controls: {ex.Message}");
                }

                // Build JSON structure
                JObject config = new JObject
                {
                    ["general"] = new JObject
                    {
                        ["windowsEdition"] = General.WindowsEdition ?? "",
                        ["productKey"] = General.ProductKey ?? "",
                        ["cpuArchitecture"] = General.CPUArchitecture ?? ""
                    },
                    ["langReg"] = new JObject
                    {
                        ["systemLocale"] = LangReg.SystemLocale ?? "",
                        ["userLocale"] = LangReg.UserLocale ?? "",
                        ["windowsUILanguage"] = LangReg.WindowsUILanguage ?? "",
                        ["keyboardLayout"] = LangReg.KeyboardLayout ?? "",
                        ["timeZone"] = LangReg.TimeZone ?? "",
                        ["sameAsSystemLocale"] = LangReg.SameAsSystemLocale
                    },
                    ["bypass"] = new JObject
                    {
                        ["bypassAll"] = Bypass.BypassAll,
                        ["bypassTPM"] = Bypass.BypassTPM,
                        ["bypassRAM"] = Bypass.BypassRAM,
                        ["bypassSecureBoot"] = Bypass.BypassSecureBoot,
                        ["bypassCPU"] = Bypass.BypassCPU,
                        ["bypassStorage"] = Bypass.BypassStorage,
                        ["bypassDisk"] = Bypass.BypassDisk
                    },
                    ["diskPart"] = new JObject
                    {
                        ["enableAutoDiskPart"] = DiskPart.EnableAutoDiskPart,
                        ["diskID"] = DiskPart.DiskID,
                        ["wipeDisk"] = DiskPart.WipeDisk,
                        ["partitionLayout"] = DiskPart.PartitionLayout,
                        ["useRemainingSpace"] = DiskPart.UseRemainingSpace,
                        ["installToPartitionID"] = DiskPart.InstallToPartitionID,
                        ["disableBitLocker"] = DiskPart.DisableBitLocker
                    },
                    ["oobe"] = new JObject
                        {
                            ["skipAll"] = OOBE.SkipAll,
                            ["skipEULA"] = OOBE.SkipEULA,
                            ["skipLocalAccountCreation"] = OOBE.SkipLocalAccountCreation,
                            ["skipOnlineAccountCreation"] = OOBE.SkipOnlineAccountCreation,
                            ["skipWirelessNetwork"] = OOBE.SkipWirelessNetwork,
                            ["skipMachineOOBE"] = OOBE.SkipMachineOOBE,
                            ["skipUserOOBE"] = OOBE.SkipUserOOBE,
                            ["networkLocation"] = OOBE.NetworkLocation,
                            ["protectYourPC"] = OOBE.ProtectYourPC
                        },
                        // Add other sections when implemented
                    };

                Console.WriteLine($"Config JSON: {config.ToString(Newtonsoft.Json.Formatting.None)}");

                // Write to file with pretty formatting
                File.WriteAllText(lastConfigPath, config.ToString(Newtonsoft.Json.Formatting.Indented));
                Console.WriteLine("Config saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving last config: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Loads the last configuration from lastConfig.json if enabled in settings
        /// </summary>
        public void LoadLastConfig()
        {
            try
            {
                // Check if load last config is enabled
                if (!SettingsManager.Instance.LoadLastConfig)
                {
                    Console.WriteLine("LoadLastConfig is disabled in settings.");
                    return;
                }

                string lastConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "presets", "lastConfig.json");
                
                Console.WriteLine($"Attempting to load config from: {lastConfigPath}");

                if (!File.Exists(lastConfigPath))
                {
                    Console.WriteLine("No last config file found to load.");
                    return;
                }

                string jsonContent = File.ReadAllText(lastConfigPath);
                Console.WriteLine($"Config file content: {jsonContent}");
                
                // Check if file is empty
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    Console.WriteLine("Config file is empty. Skipping load.");
                    return;
                }
                
                JObject config = JObject.Parse(jsonContent);

                // Load General section
                if (config["general"] is JObject generalSection)
                {
                    General.SetValues(generalSection);        
                    Console.WriteLine("GeneralConfig loaded successfully.");
                }

                // Load LangReg section
                if (config["langReg"] is JObject langRegSection)
                {
                    LangReg.SetValues(langRegSection);
                    Console.WriteLine("LangReg config loaded successfully.");
                }

                // Load Bypass section
                if (config["bypass"] is JObject bypassSection)
                {
                    Bypass.SetValues(bypassSection);
                    Console.WriteLine("BypassConfig loaded successfully.");
                }

                // Load DiskPart section
                if (config["diskPart"] is JObject diskPartSection)
                {
                    DiskPart.SetValues(diskPartSection);
                    Console.WriteLine("DiskPartConfig loaded successfully.");
                }

                // Load UserAcc section
                // if (config["userAcc"] is JObject userAccSection)
                // {
                //     UserAcc.SetValues(userAccSection);
                //     Console.WriteLine("UserAccConfig loaded successfully.");
                // }

                // Load OOBE section
                if (config["oobe"] is JObject oobeSection)
                {
                    OOBE.SetValues(oobeSection);
                    Console.WriteLine("OOBEConfig loaded successfully.");
                }

                // Load Personal section
                // if (config["personal"] is JObject personalSection)
                // {
                //     Personal.SetValues(personalSection);
                //     Console.WriteLine("PersonalConfig loaded successfully.");
                // }

                // Load App section
                // if (config["app"] is JObject appSection)
                // {
                //     App.SetValues(appSection);
                //     Console.WriteLine("AppConfig loaded successfully.");
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading last config: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        #endregion
    }
}
