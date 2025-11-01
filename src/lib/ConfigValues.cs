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
        
        // Additional configuration sections will be added later
        // public DiskPartConfig DiskPart { get; set; }
        // public UserAccConfig UserAcc { get; set; }
        // public OOBEConfig OOBE { get; set; }
        // public PersonalConfig Personal { get; set; }
        // public AppConfig App { get; set; }

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

                    // Restore Enable states after clearing data models
                    General.EnableGeneral = generalEnabled;
                    LangReg.EnableLangReg = langRegEnabled;
                    Bypass.EnableBypass = bypassEnabled;

                    // Clear other sections when implemented
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
            // Validate other sections when implemented

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

            // Add other sections when implemented

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
                        ["bypassRAMCheck"] = Bypass.BypassRAM,
                        ["bypassSecureBoot"] = Bypass.BypassSecureBoot,
                        ["bypassCPU"] = Bypass.BypassCPU,
                        ["bypassStorage"] = Bypass.BypassStorage,
                        ["bypassDisk"] = Bypass.BypassDisk
                    }
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

                // Load other sections when implemented
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
