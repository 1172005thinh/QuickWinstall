using System;
using System.Collections.Generic;
using QuickWinstall.Config;

namespace QuickWinstall.Lib
{
    public class ConfigValues
    {
        private static ConfigValues? _instance;

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

        private ConfigValues()
        {
            General = new GeneralConfig();
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

        public void Clear()
        {
            General.Clear();
            // Clear other sections when implemented
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
    }
}
