using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace QuickWinstall
{
    public static class IconManager
    {
        private static readonly Dictionary<string, Image> _iconCache = new Dictionary<string, Image>();

        public static Image GetIcon(string iconName, Size size = default)
        {
            if (size == default)
                size = new Size(16, 16);

            string cacheKey = $"{iconName}_{size.Width}x{size.Height}";
            
            if (_iconCache.TryGetValue(cacheKey, out Image cachedIcon))
            {
                return cachedIcon;
            }

            Image icon = LoadIcon(iconName, size);
            if (icon != null)
            {
                _iconCache[cacheKey] = icon;
            }

            return icon;
        }

        private static Image LoadIcon(string iconName, Size size)
        {
            try
            {
                // Try to load from icons directory first
                string iconPath = Path.Combine(Application.StartupPath ?? "", "icons", $"{iconName}.png");
                
                // Fallback to development path
                if (!File.Exists(iconPath))
                {
                    iconPath = Path.Combine(Directory.GetCurrentDirectory(), "icons", $"{iconName}.png");
                }

                if (File.Exists(iconPath))
                {
                    var originalImage = Image.FromFile(iconPath);
                    if (originalImage.Size == size)
                    {
                        return originalImage;
                    }
                    else
                    {
                        return new Bitmap(originalImage, size);
                    }
                }

                // Try embedded resources
                return LoadEmbeddedIcon(iconName, size);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon {iconName}: {ex.Message}");
                return CreatePlaceholderIcon(iconName, size);
            }
        }

        private static Image LoadEmbeddedIcon(string iconName, Size size)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = $"QuickWinstall.icons.{iconName}.png";
                
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        var image = Image.FromStream(stream);
                        return new Bitmap(image, size);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading embedded icon {iconName}: {ex.Message}");
            }

            return null;
        }

        private static Image CreatePlaceholderIcon(string iconName, Size size)
        {
            var bitmap = new Bitmap(size.Width, size.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Create a simple placeholder icon
                graphics.Clear(Color.LightGray);
                graphics.DrawRectangle(Pens.DarkGray, 0, 0, size.Width - 1, size.Height - 1);
                
                // Draw first letter of icon name if possible
                if (!string.IsNullOrEmpty(iconName))
                {
                    string letter = iconName.Substring(0, 1).ToUpper();
                    using (var font = new Font("Segoe UI", Math.Max(6, size.Width / 3)))
                    using (var brush = new SolidBrush(Color.DarkGray))
                    {
                        var textSize = graphics.MeasureString(letter, font);
                        var x = (size.Width - textSize.Width) / 2;
                        var y = (size.Height - textSize.Height) / 2;
                        graphics.DrawString(letter, font, brush, x, y);
                    }
                }
            }
            return bitmap;
        }

        public static void ClearCache()
        {
            foreach (var icon in _iconCache.Values)
            {
                icon?.Dispose();
            }
            _iconCache.Clear();
        }

        // Common icon names
        public static class Icons
        {
            public const string Settings = "settings";
            public const string About = "about";
            public const string Generate = "generate";
            public const string Exit = "exit";
            public const string Open = "open";
            public const string Save = "save";
            public const string New = "new";
            public const string Folder = "folder";
            public const string File = "file";
            public const string Language = "language";
            public const string Computer = "computer";
            public const string User = "user";
            public const string Key = "key";
            public const string Shield = "shield";
            public const string Windows = "windows";
        }
    }
}