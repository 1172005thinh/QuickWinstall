using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
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

        public FontStyle GetFontStyle(string fontType)
        {
            try
            {
                string? style = _currentTheme?["fonts"]?[fontType]?["style"]?.ToString();
                if (!string.IsNullOrEmpty(style))
                {
                    return style.Contains("Bold", StringComparison.OrdinalIgnoreCase) ? FontStyle.Bold :
                           style.Contains("Italic", StringComparison.OrdinalIgnoreCase) ? FontStyle.Italic :
                           FontStyle.Regular;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to get font style {fontType}: {ex.Message}");
            }

            // Default font style
            return FontStyle.Regular;
        }

        public string CurrentTheme => _currentThemeName;

        public bool IsDarkTheme => _currentThemeName.ToLower() == "dark";

        public Button CreateRoundedButton()
        {
            Button btn = new Button();
            UIValues ui = UIValues.Instance;

            // Use Flat style but disable default rectangular border so we can draw a rounded one
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = ui.GlobalBtnBorderWidth;
            btn.BackColor = GetColor("buttonBackground");
            // keep border color available from theme for our custom drawing
            btn.FlatAppearance.BorderColor = GetColor("buttonBorder");
            btn.Cursor = Cursors.Hand;
            btn.Padding = new Padding(0);

            // Hover effect (update background and request repaint so custom border can update if needed)
            btn.MouseEnter += (sender, e) =>
            {
                btn.BackColor = GetColor("buttonBackgroundHover");
                btn.Invalidate();
            };

            btn.MouseLeave += (sender, e) =>
            {
                btn.BackColor = GetColor("buttonBackground");
                btn.Invalidate();
            };

            // Custom painting: set a rounded region and draw a rounded border so corners are smooth
            btn.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = ui.GlobalBtnBorderRadius;
                    // shrink rectangle by 1px so the border is drawn inside the control bounds and not clipped
                    Rectangle rect = new Rectangle(0, 0, Math.Max(0, btn.Width - 1), Math.Max(0, btn.Height - 1));

                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();

                    // Clip control to rounded region so default background is rounded
                    btn.Region = new Region(path);

                    // Draw custom border inside the rounded region using theme color
                    using (Pen pen = new Pen(GetColor("buttonBorder"), ui.GlobalBtnBorderWidth))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            return btn;
        }

        public void ApplyButtonTheme(Button btn)
        {
            if (btn == null) return;

            UIValues ui = UIValues.Instance;
            btn.BackColor = GetColor("buttonBackground");
            btn.FlatAppearance.BorderColor = GetColor("buttonBorder");
            btn.ForeColor = GetFontColor("normal");
            btn.Invalidate();
        }

        /// <summary>
        /// Creates a toggle switch control with rounded corners and smooth animation
        /// </summary>
        /// <param name="location">Location of the toggle switch</param>
        /// <param name="width">Width of the toggle switch</param>
        /// <param name="height">Height of the toggle switch</param>
        /// <param name="initialState">Initial state (true = ON, false = OFF)</param>
        /// <returns>Panel containing the toggle switch</returns>
        public Panel CreateToggleSwitch(Point location, int width, int height, bool initialState = true)
        {
            // Create the main toggle container with rounded corners
            Panel toggle = new Panel();
            toggle.Location = location;
            toggle.Size = new Size(width, height);
            toggle.BorderStyle = BorderStyle.None;
            toggle.Cursor = Cursors.Hand;
            toggle.Tag = initialState; // Default state
            
            // Make the toggle track rounded
            GraphicsPath trackPath = new GraphicsPath();
            int cornerRadius = height / 2;
            trackPath.AddArc(0, 0, height, height, 90, 180);
            trackPath.AddLine(cornerRadius, 0, width - cornerRadius, 0);
            trackPath.AddArc(width - height, 0, height, height, 270, 180);
            trackPath.AddLine(width - cornerRadius, height, cornerRadius, height);
            toggle.Region = new Region(trackPath);
            
            // Set initial background color based on state
            toggle.BackColor = initialState ? GetColor("toggleOnBackground") : GetColor("toggleOffBackground");
            
            // Create the toggle thumb (circular button)
            Panel toggleThumb = new Panel();
            int thumbSize = height - 6; // Slightly smaller than track height for padding
            toggleThumb.Size = new Size(thumbSize, thumbSize);
            toggleThumb.Location = initialState ? 
                new Point(width - thumbSize - 3, 3) : // ON state (right)
                new Point(3, 3); // OFF state (left)
            toggleThumb.BackColor = GetColor("toggleThumb");
            toggleThumb.Tag = "thumb";
            
            // Make the thumb circular
            GraphicsPath thumbPath = new GraphicsPath();
            thumbPath.AddEllipse(0, 0, thumbSize, thumbSize);
            toggleThumb.Region = new Region(thumbPath);
            
            // Add shadow effect using Paint event
            toggle.Paint += (s, e) =>
            {
                // Draw inner shadow for depth effect
                using (GraphicsPath shadowPath = new GraphicsPath())
                {
                    int shadowRadius = height / 2;
                    shadowPath.AddArc(1, 1, height - 2, height - 2, 90, 180);
                    shadowPath.AddLine(shadowRadius, 1, width - shadowRadius, 1);
                    shadowPath.AddArc(width - height + 1, 1, height - 2, height - 2, 270, 180);
                    shadowPath.AddLine(width - shadowRadius, height - 1, shadowRadius, height - 1);
                    
                    using (Pen shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                    {
                        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        e.Graphics.DrawPath(shadowPen, shadowPath);
                    }
                }
            };
            
            // Forward thumb click to the parent toggle's Click event so both parts act the same
            toggleThumb.Click += (s, e) =>
            {
                try
                {
                    // Invoke protected OnClick on the parent toggle to raise its Click event handlers
                    MethodInfo? mi = typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
                    mi?.Invoke(toggle, new object[] { e });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to forward thumb click: {ex.Message}");
                    // Fallback: attempt to raise Click via event invocation if available
                    try { toggle.PerformLayout(); } catch { }
                }
            };

            toggle.Controls.Add(toggleThumb);
            
            return toggle;
        }

        /// <summary>
        /// Updates the visual state of a toggle switch
        /// </summary>
        /// <param name="toggle">The toggle switch panel</param>
        /// <param name="state">The new state (true = ON, false = OFF)</param>
        public void UpdateToggleSwitchState(Panel toggle, bool state)
        {
            if (toggle == null) return;

            toggle.Tag = state;
            
            // Update background color
            toggle.BackColor = state ? GetColor("toggleOnBackground") : GetColor("toggleOffBackground");
            
            // Find and move the thumb
            foreach (Control ctrl in toggle.Controls)
            {
                if (ctrl.Tag?.ToString() == "thumb" && ctrl is Panel thumb)
                {
                    int thumbSize = thumb.Height;
                    int newX = state ? toggle.Width - thumbSize - 3 : 3;
                    thumb.Location = new Point(newX, 3);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the current state of a toggle switch
        /// </summary>
        /// <param name="toggle">The toggle switch panel</param>
        /// <returns>True if ON, false if OFF</returns>
        public bool GetToggleSwitchState(Panel toggle)
        {
            if (toggle?.Tag is bool state)
                return state;
            return false;
        }
    }
}
