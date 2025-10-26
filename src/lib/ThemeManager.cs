using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;

namespace QuickWinstall.Lib
{
    public class ThemeManager
    {
        private static ThemeManager? _instance;
        private JObject? _currentTheme;
        private string _currentThemeName = "Light";

        private ThemeManager()
        {
            LoadTheme("Light");
        }

        public static ThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ThemeManager();
                }
                return _instance;
            }
        }

        public void LoadTheme(string themeName)
        {
            try
            {
                string themeFile = themeName.ToLower() == "dark" ? "dark.json" : "light.json";
                string themePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "themes", themeFile);
                
                if (File.Exists(themePath))
                {
                    string jsonContent = File.ReadAllText(themePath);
                    _currentTheme = JObject.Parse(jsonContent);
                    _currentThemeName = themeName;
                }
                else
                {
                    Console.WriteLine($"Warning: Theme file not found: {themePath}. Using defaults.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load theme: {ex.Message}. Using defaults.");
            }
        }

        public Color GetColor(string colorKey)
        {
            try
            {
                string? colorHex = _currentTheme?["colors"]?[colorKey]?.ToString();
                if (!string.IsNullOrEmpty(colorHex))
                {
                    return ColorTranslator.FromHtml(colorHex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get color {colorKey}: {ex.Message}");
            }

            // Default colors
            return _currentThemeName.ToLower() == "dark" ? Color.FromArgb(30, 30, 30) : Color.FromArgb(225, 225, 225);
        }

        public Font GetFont(string fontType)
        {
            try
            {
                var fontConfig = _currentTheme?["fonts"]?[fontType];
                if (fontConfig != null)
                {
                    string family = fontConfig["family"]?.ToString() ?? "Segoe UI";
                    float size = fontConfig["size"]?.ToObject<float>() ?? 12f;
                    string styleStr = fontConfig["style"]?.ToString() ?? "Regular";
                    
                    FontStyle style = FontStyle.Regular;
                    if (styleStr.Contains("Bold", StringComparison.OrdinalIgnoreCase))
                        style = FontStyle.Bold;
                    else if (styleStr.Contains("Italic", StringComparison.OrdinalIgnoreCase))
                        style = FontStyle.Italic;

                    return new Font(family, size, style);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get font {fontType}: {ex.Message}");
            }

            // Default font
            return new Font("Segoe UI", 12f, FontStyle.Regular);
        }

        public Color GetFontColor(string fontType)
        {
            try
            {
                string? colorHex = _currentTheme?["fonts"]?[fontType]?["color"]?.ToString();
                if (!string.IsNullOrEmpty(colorHex))
                {
                    return ColorTranslator.FromHtml(colorHex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get font color {fontType}: {ex.Message}");
            }

            // Default font color
            return _currentThemeName.ToLower() == "dark" ? Color.White : Color.Black;
        }

        public string CurrentTheme => _currentThemeName;

        public bool IsDarkTheme => _currentThemeName.ToLower() == "dark";
    }
}
