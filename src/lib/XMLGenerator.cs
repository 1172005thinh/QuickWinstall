using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace QuickWinstall.Lib
{
    public class XMLGenerator
    {
        private static XMLGenerator? _instance;
        private string _templatePath;

        private XMLGenerator()
        {
            _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "lib", "template.xml");
        }

        public static XMLGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new XMLGenerator();
                }
                return _instance;
            }
        }

        public bool GenerateXML(string outputPath, Dictionary<string, string> values)
        {
            try
            {
                // Read template
                if (!File.Exists(_templatePath))
                {
                    Console.WriteLine($"Error: Template file not found: {_templatePath}");
                    return false;
                }

                string template = File.ReadAllText(_templatePath);

                // Replace placeholders
                string result = ReplacePlaceholders(template, values);

                // Ensure output directory exists
                string? directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write output
                File.WriteAllText(outputPath, result);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to generate XML: {ex.Message}");
                return false;
            }
        }

        private string ReplacePlaceholders(string template, Dictionary<string, string> values)
        {
            string result = template;

            foreach (var kvp in values)
            {
                string placeholder = $"{{{{{kvp.Key}}}}}";
                string value = kvp.Value ?? "";

                // Handle empty product key - remove the entire ProductKey element
                if (kvp.Key == "ProductKey" && string.IsNullOrWhiteSpace(value))
                {
                    // Remove the ProductKey element entirely
                    result = Regex.Replace(result, 
                        @"<ProductKey>.*?</ProductKey>", 
                        "", 
                        RegexOptions.Singleline);
                }
                else
                {
                    result = result.Replace(placeholder, value);
                }
            }

            return result;
        }

        public bool ValidateTemplate()
        {
            try
            {
                if (!File.Exists(_templatePath))
                {
                    Console.WriteLine($"Error: Template file not found: {_templatePath}");
                    return false;
                }

                // Basic validation - check if file is readable and contains XML declaration
                string content = File.ReadAllText(_templatePath);
                return content.Contains("<?xml") && content.Contains("<unattend");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Failed to validate template: {ex.Message}");
                return false;
            }
        }
    }
}
