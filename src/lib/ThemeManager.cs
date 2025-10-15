using System.Text.Json;

namespace QuickWinstall.Lib
{
    #region ThemeData
    public class ThemeData
    {
        public string SeparatorColor { get; set; }
    }
    #endregion

    #region Themes
    public static class ThemeManager
    {
        private static ThemeData _currentTheme;
        private static string _currentThemeName = LangManager.GetString("SettingsForm_Theme_Light", "Light");
        //private static ThemeConfig _configTheme;

        // State variables
        private static bool _isInitialized = false;

        #region ThemeItem
        public class ThemeItem
        {
            public string Name { get; set; }
            public override string ToString() => Name;
        }
        #endregion

        #region Type
        public enum Type
        {
            Normal,
            Success,
            Info,
            Warning,
            Error,
            Disabled,
            Header,
            WarningHeader,
            ErrorHeader,
            SubHeader,
            Link,
            LinkHover,
            LinkVisited,
            LinkDisabled
        }
        #endregion

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

        #region GetTheme
        public static Color SeparatorColor
        {
            get
            {
                Initialize();
                return Hex2Color(_currentTheme?.SeparatorColor ?? "#000000ff");
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
                SeparatorColor = "#000000ff"
            };
            _currentThemeName = LangManager.GetString("SettingsForm_Theme_Light", "Light");
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
            SetForms();
        }
        #endregion

        #region SetForms
        public static void SetForms()
        {
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    SetForm(form);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply theme to all forms: {ex.Message}");
            }
        }
        #endregion

        #region SetForm
        public static void SetForm(Form form)
        {
            if (form == null) return;

            try
            {
                //form.BackColor = BackgroundColor;
                //form.ForeColor = TextColor;

                SetControls(form.Controls);

                form.Invalidate(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to apply theme to form {form.Name}: {ex.Message}");
            }
        }
        #endregion

        #region SetControls
        private static void SetControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                try
                {
                    // Apply theme based on control type
                    switch (control)
                    {
                        case Button:
                            SetButtonStyle((Button)control, Type.Normal);
                            break;
                        case ComboBox:
                            SetComboBoxStyle((ComboBox)control, Type.Normal);
                            break;
                        case LinkLabel:
                            SetLinkStyle((LinkLabel)control, Type.Link);
                            break;
                        case Label:
                            SetLabelStyle((Label)control, Type.Normal);
                            break;
                        case Panel:
                            SetPanelStyle((Panel)control, Type.Normal);
                            break;
                        case TextBox:
                            SetTextBoxStyle((TextBox)control, Type.Normal);
                            break;
                    }
                    // Recursively apply to child controls
                    if (control.HasChildren)
                    {
                        SetControls(control.Controls);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to apply theme to control {control.Name}: {ex.Message}");
                }
            }
        }
        #endregion

        #region SetButtonStyle
        public static void SetButtonStyle(Button button, Type type = Type.Normal)
        {
            switch (type)
            {
                case Type.Normal:
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.FlatStyle = FlatStyle.Standard;
                    break;
                case Type.Disabled:
                    button.BackColor = SystemColors.ControlDark;
                    button.ForeColor = SystemColors.GrayText;
                    button.FlatStyle = FlatStyle.Flat;
                    break;
            }
        }
        #endregion

        #region SetComboBoxStyle
        public static void SetComboBoxStyle(ComboBox comboBox, Type type = Type.Normal)
        {
            switch (type)
            {
                case Type.Normal:
                    comboBox.BackColor = SystemColors.Window;
                    comboBox.ForeColor = SystemColors.WindowText;
                    comboBox.FlatStyle = FlatStyle.Standard;
                    break;
                case Type.Warning:
                    comboBox.BackColor = Color.LightYellow;
                    comboBox.ForeColor = SystemColors.WindowText;
                    comboBox.FlatStyle = FlatStyle.Standard;
                    break;
                case Type.Error:
                    comboBox.BackColor = Color.LightCoral;
                    comboBox.ForeColor = SystemColors.WindowText;
                    comboBox.FlatStyle = FlatStyle.Standard;
                    break;
                case Type.Disabled:
                    comboBox.BackColor = SystemColors.ControlDark;
                    comboBox.ForeColor = SystemColors.GrayText;
                    comboBox.FlatStyle = FlatStyle.Flat;
                    break;
            }
        }
        #endregion

        #region SetLabelStyle
        public static void SetLabelStyle(Label item, Type type = Type.Normal)
        {
            switch (type)
            {
                case Type.Normal:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = SystemColors.ControlText;
                    break;
                case Type.Info:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = Color.Blue;
                    break;
                case Type.Warning:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = Color.LightYellow;
                    break;
                case Type.Error:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = Color.LightCoral;
                    break;
                case Type.Disabled:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = SystemColors.GrayText;
                    break;
                case Type.Header:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = SystemColors.ControlText;
                    item.Font = new Font(item.Font, FontStyle.Bold);
                    break;
                case Type.WarningHeader:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = Color.LightYellow;
                    item.Font = new Font(item.Font, FontStyle.Bold);
                    break;
                case Type.ErrorHeader:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = Color.LightCoral;
                    item.Font = new Font(item.Font, FontStyle.Bold);
                    break;
                case Type.SubHeader:
                    item.BackColor = Color.Transparent;
                    item.ForeColor = SystemColors.ControlText;
                    item.Font = new Font(item.Font, FontStyle.Italic);
                    break;
            }
        }
        #endregion

        #region SetLinkStyle
        public static void SetLinkStyle(LinkLabel linkLabel, Type type = Type.Link)
        {
            switch (type)
            {
                case Type.Link:
                    linkLabel.LinkColor = Color.Blue;
                    linkLabel.ActiveLinkColor = Color.DarkBlue;
                    linkLabel.VisitedLinkColor = Color.Purple;
                    linkLabel.DisabledLinkColor = SystemColors.GrayText;
                    break;
                case Type.LinkHover:
                    linkLabel.LinkColor = Color.DarkBlue;
                    break;
                case Type.LinkVisited:
                    linkLabel.LinkColor = Color.Purple;
                    break;
                case Type.LinkDisabled:
                    linkLabel.LinkColor = SystemColors.GrayText;
                    break;
            }
        }
        #endregion

        #region SetPanelStyle
        public static void SetPanelStyle(Panel panel, Type type = Type.Normal)
        {
            switch (type)
            {
                case Type.Normal:
                    panel.BackColor = SystemColors.Control;
                    panel.ForeColor = SystemColors.ControlText;
                    break;
                case Type.Disabled:
                    panel.BackColor = SystemColors.ControlDark;
                    panel.ForeColor = SystemColors.GrayText;
                    break;
            }
        }
        #endregion

        #region SetStatusLabelStyle
        public static void SetStatusLabelStyle(ToolStripStatusLabel label, Type type = Type.Normal)
        {
            switch (type)
            {
                case Type.Normal:
                    label.BackColor = Color.Transparent;
                    label.ForeColor = SystemColors.ControlText;
                    break;
                case Type.Success:
                    label.BackColor = Color.Transparent;
                    label.ForeColor = Color.LightGreen;
                    break;
                case Type.Warning:
                    label.BackColor = Color.Transparent;
                    label.ForeColor = Color.LightYellow;
                    break;
                case Type.Error:
                    label.BackColor = Color.Transparent;
                    label.ForeColor = Color.LightCoral;
                    break;
            }
        }
        #endregion

        #region SetTextBoxStyle
        public static void SetTextBoxStyle(TextBox textBox, Type type = Type.Normal)
        {
            switch (type)
            {
                case Type.Normal:
                    textBox.BackColor = SystemColors.Window;
                    textBox.ForeColor = SystemColors.WindowText;
                    textBox.BorderStyle = BorderStyle.Fixed3D;
                    break;
                case Type.Warning:
                    textBox.BackColor = Color.LightYellow;
                    textBox.ForeColor = SystemColors.WindowText;
                    textBox.BorderStyle = BorderStyle.Fixed3D;
                    break;
                case Type.Error:
                    textBox.BackColor = Color.LightCoral;
                    textBox.ForeColor = SystemColors.WindowText;
                    textBox.BorderStyle = BorderStyle.Fixed3D;
                    break;
                case Type.Disabled:
                    textBox.BackColor = SystemColors.ControlDark;
                    textBox.ForeColor = SystemColors.GrayText;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;
            }
        }
        #endregion

        #region DrawSeparatorLine
        public static void DrawSeparatorLine(Graphics graphics, int width, int height)
        {
            using (Pen pen = new Pen(SeparatorColor, 1))
            {
                graphics.DrawLine(pen, 0, height - 1, width, height - 1);
            }
        }
        #endregion

        #region GetAvailableThemes
        public static ThemeItem[] GetAvailableThemes()
        {
            try
            {
                var defaults = Defaults.LoadFromAppFolder();
                var themes = defaults.ThemeSettings.ThemesAvailable;
                var items = new List<ThemeItem>();

                foreach (var theme in themes)
                {
                    items.Add(new ThemeItem
                    {
                        Name = theme
                    });
                }
                return items.ToArray();
            }
            catch
            {
                return new[]
                {
                    new ThemeItem
                    {
                        Name = LangManager.GetString("SettingsForm_Theme_Light", "Light")
                    },
                    new ThemeItem
                    {
                        Name = LangManager.GetString("SettingsForm_Theme_Dark", "Dark")
                    }
                };
            }
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