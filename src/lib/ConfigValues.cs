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
        
        // Additional configuration sections will be added later
        // public LangRegConfig LangReg { get; set; }
        // public BypassConfig Bypass { get; set; }
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

                    // Load empty values for General section
                    if (emptyConfig["general"] is JObject generalSection)
                    {
                        var emptyValues = new Dictionary<string, string>
                        {
                            ["WindowsEdition"] = generalSection["windowsEdition"]?.ToString() ?? "",
                            ["ProductKey"] = generalSection["productKey"]?.ToString() ?? "",
                            ["CPUArchitecture"] = generalSection["cpuArchitecture"]?.ToString() ?? ""
                        };
                        General.SetValues(emptyValues);
                    }
                    else
                    {
                        // Fallback to hardcoded clear
                        General.Clear();
                    }

                    // Clear UI controls
                    General.ClearControls();

                    // Clear other sections when implemented
                }
                else
                {
                    // Fallback to hardcoded clear if empty.json not found
                    Console.WriteLine($"Warning: empty.json not found at {_emptyConfigPath}. Using hardcoded empty values.");
                    General.Clear();
                    General.ClearControls();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing config from empty.json: {ex.Message}. Using hardcoded empty values.");
                General.Clear();
                General.ClearControls();
            }
        }

        public List<string> Validate()
        {
            List<string> errors = new List<string>();
            
            errors.AddRange(General.Validate());
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
                    var values = new Dictionary<string, string>
                    {
                        ["WindowsEdition"] = generalSection["windowsEdition"]?.ToString() ?? "",
                        ["ProductKey"] = generalSection["productKey"]?.ToString() ?? "",
                        ["CPUArchitecture"] = generalSection["cpuArchitecture"]?.ToString() ?? ""
                    };
                    
                    Console.WriteLine($"Loading values: WindowsEdition={values["WindowsEdition"]}, ProductKey={values["ProductKey"]}, CPUArchitecture={values["CPUArchitecture"]}");
                    
                    General.SetValues(values);
                    // Note: UI controls will be updated by each section after initialization
                    
                    Console.WriteLine("Config data model loaded successfully. UI update deferred to sections.");
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
