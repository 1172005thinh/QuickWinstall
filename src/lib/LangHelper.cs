using System;
using System.Text.RegularExpressions;

namespace QuickWinstall.Lib
{
    public static class LangHelper
    {
        /// <summary>
        /// Formats a string with named parameters.
        /// Example: FormatString("Hello {name}, you have {count} messages", new { name = "John", count = 5 })
        /// Returns: "Hello John, you have 5 messages"
        /// </summary>
        public static string FormatString(string template, object parameters)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            if (parameters == null)
                return template;

            try
            {
                string result = template;
                var properties = parameters.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    string placeholder = $"{{{prop.Name}}}";
                    string value = prop.GetValue(parameters)?.ToString() ?? "";
                    result = result.Replace(placeholder, value);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to format string: {ex.Message}");
                return template;
            }
        }

        /// <summary>
        /// Formats a string with indexed parameters.
        /// Example: FormatString("Hello {0}, you have {1} messages", "John", 5)
        /// Returns: "Hello John, you have 5 messages"
        /// </summary>
        public static string FormatString(string template, params object[] args)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            if (args == null || args.Length == 0)
                return template;

            try
            {
                return string.Format(template, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to format string: {ex.Message}");
                return template;
            }
        }

        /// <summary>
        /// Escapes special characters in a string for display.
        /// </summary>
        public static string EscapeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace("\\n", Environment.NewLine)
                .Replace("\\t", "\t")
                .Replace("\\r", "\r");
        }

        /// <summary>
        /// Validates if a language key is in valid format (e.g., "mainForm.title").
        /// </summary>
        public static bool IsValidLanguageKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            // Language key format: section.subsection.key (can have multiple levels)
            return Regex.IsMatch(key, @"^[a-zA-Z][a-zA-Z0-9]*(\.[a-zA-Z][a-zA-Z0-9]*)+$");
        }

        /// <summary>
        /// Parses a language code (e.g., "en-US") into language and region.
        /// </summary>
        public static (string language, string region) ParseLanguageCode(string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
                return ("en", "US");

            string[] parts = langCode.Split('-');
            string language = parts[0];
            string region = parts.Length > 1 ? parts[1] : "";

            return (language, region);
        }
    }
}
