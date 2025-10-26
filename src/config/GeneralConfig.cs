using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    public class GeneralConfig
    {
        public string WindowsEdition { get; set; } = "";
        public string ProductKey { get; set; } = "";
        public string CPUArchitecture { get; set; } = "";

        public void Clear()
        {
            WindowsEdition = "";
            ProductKey = "";
            CPUArchitecture = "";
        }

        public List<string> Validate()
        {
            List<string> errors = new List<string>();
            LangManager lang = LangManager.Instance;

            // Validate Windows Edition
            if (string.IsNullOrWhiteSpace(WindowsEdition))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("generalConfig.windowsEdition.label")));
            }

            // Validate Product Key
            if (!string.IsNullOrWhiteSpace(ProductKey))
            {
                if (!IsValidProductKey(ProductKey))
                {
                    errors.Add(lang.GetString("validation.invalidProductKey"));
                }
            }

            // Validate CPU Architecture
            if (string.IsNullOrWhiteSpace(CPUArchitecture))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("generalConfig.cpuArch.label")));
            }

            return errors;
        }

        private bool IsValidProductKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return true; // Empty is valid

            // Remove hyphens for validation
            string cleanKey = key.Replace("-", "");

            // Must be exactly 25 alphanumeric characters
            if (cleanKey.Length != 25)
                return false;

            // Must contain only alphanumeric characters
            return Regex.IsMatch(cleanKey, @"^[A-Z0-9]{25}$");
        }

        public Dictionary<string, string> GetValues()
        {
            return new Dictionary<string, string>
            {
                ["WindowsEdition"] = WindowsEdition,
                ["ProductKey"] = ProductKey,
                ["CPUArchitecture"] = CPUArchitecture
            };
        }

        public void SetValues(Dictionary<string, string> values)
        {
            if (values.ContainsKey("WindowsEdition"))
                WindowsEdition = values["WindowsEdition"];

            if (values.ContainsKey("ProductKey"))
                ProductKey = values["ProductKey"];

            if (values.ContainsKey("CPUArchitecture"))
                CPUArchitecture = values["CPUArchitecture"];
        }
    }
}
