using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace QuickWinstall.Lib
{
    public class IconManager
    {
        private static IconManager? _instance;
        private Dictionary<string, Icon> _iconCache = new Dictionary<string, Icon>();
        private string _iconsPath;
        private Icon? _defaultIcon;

        private IconManager()
        {
            _iconsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "icons");
            LoadDefaultIcon();
        }

        public static IconManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IconManager();
                }
                return _instance;
            }
        }

        private void LoadDefaultIcon()
        {
            try
            {
                string errorIconPath = Path.Combine(_iconsPath, "error256.ico");
                if (File.Exists(errorIconPath))
                {
                    _defaultIcon = new Icon(errorIconPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load default icon: {ex.Message}");
            }
        }

        public Icon GetIcon(string iconName, bool useDarkTheme = false)
        {
            try
            {
                // Determine icon name based on theme
                // Some icons like app, windows11, error, warning, info are theme-independent
                string[] themeIndependent = { "app", "app256", "windows11", "windows11256", "error", "error256", "warning", "warning256", "info", "info256" };
                bool isThemeIndependent = Array.Exists(themeIndependent, s => iconName.Equals(s, StringComparison.OrdinalIgnoreCase));

                string fileName = iconName;
                if (!isThemeIndependent && useDarkTheme)
                {
                    fileName = $"{iconName}_dark";
                }

                string cacheKey = fileName;

                // Check if icon is already cached
                if (_iconCache.ContainsKey(cacheKey))
                {
                    return _iconCache[cacheKey];
                }

                // Try to load the icon
                string iconPath = Path.Combine(_iconsPath, $"{fileName}.ico");
                if (File.Exists(iconPath))
                {
                    Icon icon = new Icon(iconPath);
                    _iconCache[cacheKey] = icon;
                    return icon;
                }
                else
                {
                    Console.WriteLine($"Warning: Icon not found: {iconPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load icon {iconName}: {ex.Message}");
            }

            // Return default icon if available
            return _defaultIcon ?? SystemIcons.Application;
        }

        public Image GetIconAsImage(string iconName, bool useDarkTheme = false, int size = 16)
        {
            try
            {
                Icon icon = GetIcon(iconName, useDarkTheme);
                return new Icon(icon, new Size(size, size)).ToBitmap();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to convert icon to image: {ex.Message}");
                return new Bitmap(size, size);
            }
        }

        public void ClearCache()
        {
            foreach (var icon in _iconCache.Values)
            {
                icon?.Dispose();
            }
            _iconCache.Clear();
        }
    }
}
