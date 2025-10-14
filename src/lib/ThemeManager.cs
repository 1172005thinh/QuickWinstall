using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;
using QuickWinstall;

namespace QuickWinstall.Lib
{
    #region ThemeData
    public class ThemeData
    {

    }
    #endregion

    #region Themes
    public static class ThemeManager
    {
        private static ThemeData _currentTheme;
        //private static ThemeConfig _configTheme;
        private static bool _isInitialized = false;
        private static string _currentThemeName = "Light";

        #region Initialize
        public static void Initialize()
        {
            if (!_isInitialized)
            {
                try
                {
                    var settings = SettingsManager.LoadSettings();
                    LoadTheme(settings.Theme);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load theme: {ex.Message}");
                    LoadFallbackTheme();
                }
                _isInitialized = true;
            }
        }
        #endregion

        #region LoadTheme
        public static void LoadTheme(string themeName)
        {
            try
            {
                string themePath = Path.Combine(Application.StartupPath, "res", "themes", $"{themeName.ToLower()}.json");
                if (File.Exists(themePath))
                {
                    string json = File.ReadAllText(themePath);
                    _currentTheme = JsonSerializer.Deserialize<ThemeData>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    _currentThemeName = themeName;

                    // Save theme preference
                    var settings = SettingsManager.LoadSettings();
                    settings.Theme = themeName;
                    SettingsManager.SaveSettings(settings);
                }
                else
                {
                    LoadFallbackTheme();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load theme {themeName}: {ex.Message}");
                LoadFallbackTheme();
            }
        }
        #endregion

        #region LoadFallbackTheme
        private static void LoadFallbackTheme()
        {
            _currentTheme = new ThemeData
            {

            };
            _currentThemeName = "Light";
        }
        #endregion

        public static string CurrentThemeName => _currentThemeName;

        #region Hex2Color
        private static Color Hex2Color(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Color.Black;
            try
            {
                // Use ColorTranslator which properly handles hex colors as opaque
                return ColorTranslator.FromHtml(hex);
            }
            catch
            {
                // Fallback: manual parsing with explicit alpha = 255 (opaque)
                if (hex.StartsWith("#")) hex = hex.Substring(1);
                int rgb = Convert.ToInt32(hex, 16);
                return Color.FromArgb(255, (rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
            }
        }
        #endregion

        #region SwitchTheme
        public static void SwitchTheme(string themeName)
        {
            LoadTheme(themeName);
            // Force refresh all cached colors and fonts
            _isInitialized = false;
            Initialize();

            // Apply theme to all open forms
            ApplyThemeToAllForms();
        }
        #endregion

        #region ApplyThemeToAllForms
        public static void ApplyThemeToAllForms()
        {
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    ApplyThemeToForm(form);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply theme to all forms: {ex.Message}");
            }
        }
        #endregion

        #region ApplyThemeToForm
        public static void ApplyThemeToForm(Form form)
        {
            if (form == null) return;

            try
            {
                //form.BackColor = BackgroundColor;
                //form.ForeColor = TextColor;

                ApplyThemeToControls(form.Controls);

                form.Invalidate(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply theme to form {form.Name}: {ex.Message}");
            }
        }
        #endregion

        #region ApplyThemeToControls
        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                try
                {
                    // Apply theme based on control type
                    switch (control)
                    {
                        case Panel panel:
                            break;
                        case Label label when label.Name.Contains("header") || label.Name.Contains("Header"):
                            break;
                        case Label label when label.Name.Contains("warning") || label.Name.Contains("Warning"):
                            break;
                        case Label label when label.Name.Contains("info") || label.Name.Contains("Info"):
                            break;
                        case Label label when label.Name.Contains("error") || label.Name.Contains("Error"):
                            break;
                        case LinkLabel linkLabel:
                            break;
                        case Label label:
                            break;
                        case Button button:
                            break;
                        case TextBox textBox:
                            break;
                        case ComboBox comboBox:
                            break;
                        case CheckBox checkBox:
                            break;
                        case UserControl userControl:
                            break;
                    }

                    // Recursively apply to child controls
                    if (control.HasChildren)
                    {
                        ApplyThemeToControls(control.Controls);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to apply theme to control {control.Name}: {ex.Message}");
                }
            }
        }
        #endregion

        #region GetAvailableThemes
        public static string[] GetAvailableThemes()
        {
            return new string[] { "Light", "Dark" };
        }
        #endregion

        #region ParseColor
        public static Color ParseColor(string colorString, Color fallback)
        {
            try
            {
                return ColorTranslator.FromHtml(colorString);
            }
            catch
            {
                return fallback;
            }
        }
        #endregion

        #region RefreshTheme
        public static void RefreshTheme()
        {
            _isInitialized = false;
            Initialize();
        }
        #endregion
    }
    #endregion
}