using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace QuickWinstall.Lib
{
    #region IconManager
    public static class IconManager
    {
        private static readonly string iconPath = Path.Combine(AppContext.BaseDirectory, "res", "icons");
        private static readonly Dictionary<string, Icon> _iconCache = new Dictionary<string, Icon>();
        private static readonly Dictionary<string, Bitmap> _bitmapCache = new Dictionary<string, Bitmap>();
        private static bool _isInitialized = false;

        #region Icons
        public static class Icons
        {
            public const string About = "about.ico";
            public const string About_dark = "about_dark.ico";
            public const string Add = "add.ico";
            public const string Add_dark = "add_dark.ico";
            public const string App = "app.ico";
            public const string App256 = "app256.ico";
            public const string Browse = "browse.ico";
            public const string Browse_dark = "browse_dark.ico";
            public const string Cancel = "cancel.ico";
            public const string Cancel_dark = "cancel_dark.ico";
            public const string Clear = "clear.ico";
            public const string Clear_dark = "clear_dark.ico";
            public const string Collapse = "collapse.ico";
            public const string Collapse_dark = "collapse_dark.ico";
            public const string Edit = "edit.ico";
            public const string Edit_dark = "edit_dark.ico";
            public const string Error = "error.ico";
            public const string Error256 = "error256.ico";
            public const string Expand = "expand.ico";
            public const string Expand_dark = "expand_dark.ico";
            public const string Facebook = "facebook.ico";
            public const string Facebook_dark = "facebook_dark.ico";
            public const string Generate = "gen.ico";
            public const string Generate_dark = "gen_dark.ico";
            public const string GitHub = "github.ico";
            public const string GitHub_dark = "github_dark.ico";
            public const string Help = "help.ico";
            public const string Help_dark = "help_dark.ico";
            public const string Hide = "hide.ico";
            public const string Hide_dark = "hide_dark.ico";
            public const string Import = "import.ico";
            public const string Import_dark = "import_dark.ico";
            public const string Info = "info.ico";
            public const string Info256 = "info256.ico";
            public const string No = "no.ico";
            public const string No_dark = "no_dark.ico";
            public const string OK = "ok.ico";
            public const string OK_dark = "ok_dark.ico";
            public const string Preset = "preset.ico";
            public const string Preset_dark = "preset_dark.ico";
            public const string Remove = "remove.ico";
            public const string Remove_dark = "remove_dark.ico";
            public const string Reset = "reset.ico";
            public const string Reset_dark = "reset_dark.ico";
            public const string Save = "save.ico";
            public const string Save_dark = "save_dark.ico";
            public const string Search = "search.ico";
            public const string Search_dark = "search_dark.ico";
            public const string Settings = "settings.ico";
            public const string Settings_dark = "settings_dark.ico";
            public const string Show = "show.ico";
            public const string Show_dark = "show_dark.ico";
            public const string Tips = "tips.ico";
            public const string Tips_dark = "tips_dark.ico";
            public const string URL = "url.ico";
            public const string URL_dark = "url_dark.ico";
            public const string Warning = "warning.ico";
            public const string Warning256 = "warning256.ico";
            public const string Windows11 = "windows11.ico";
            public const string xml = "xml.ico";
            public const string xml_dark = "xml_dark.ico";
            public const string YouTube = "youtube.ico";
            public const string YouTube_dark = "youtube_dark.ico";
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                if (Directory.Exists(iconPath))
                {
                    string[] iconFiles = Directory.GetFiles(iconPath, "*.ico");
                    foreach (string iconFile in iconFiles)
                    {
                        string fileName = Path.GetFileName(iconFile);
                        try
                        {
                            LoadIcon(fileName);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to load icon '{fileName}': {ex.Message}");
                        }
                    }
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IconManager initialization failed: {ex.Message}");
            }
        }
        #endregion

        #region LoadIcon
        private static Icon LoadIcon(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
                return null;

            if (_iconCache.TryGetValue(iconName, out Icon cachedIcon))
                return cachedIcon;

            try
            {
                string iconFilePath = Path.Combine(iconPath, iconName);
                if (File.Exists(iconFilePath))
                {
                    Icon icon = new Icon(iconFilePath);
                    _iconCache[iconName] = icon;
                    return icon;
                }

                System.Diagnostics.Debug.WriteLine($"Icon file not found: {iconFilePath}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon '{iconName}': {ex.Message}");
                return null;
            }
        }
        #endregion

        #region LoadIconAsBitmap
        private static Bitmap LoadIconAsBitmap(string iconName, Size? size = null)
        {
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;

            if (string.IsNullOrEmpty(iconName))
                return null;

            var targetSize = size ?? new Size(globalConfig.IconSize, globalConfig.IconSize);
            string cacheKey = $"{iconName}_{targetSize.Width}x{targetSize.Height}";

            // Check bitmap cache first
            if (_bitmapCache.TryGetValue(cacheKey, out Bitmap cachedBitmap))
                return cachedBitmap;

            try
            {
                var icon = LoadIcon(iconName);
                if (icon == null)
                    return null;

                Bitmap bitmap = new Bitmap(icon.ToBitmap(), targetSize);
                _bitmapCache[cacheKey] = bitmap;
                return bitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting icon '{iconName}' to bitmap: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region GetAppIcon
        public static Icon SetApp256Icon()
        {
            return LoadIcon(Icons.App256) ?? SystemIcons.Application;
        }

        public static Icon SetAppIcon()
        {
            return LoadIcon(Icons.App) ?? SystemIcons.Application;
        }
        #endregion

        #region SetFormIcon
        public static bool SetFormIcon(Form form, string iconName = null)
        {
            try
            {
                Icon icon = string.IsNullOrEmpty(iconName) ? SetAppIcon() : LoadIcon(iconName);
                if (icon != null)
                {
                    form.Icon = icon;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting form icon: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region SetButtonIcon
        public static bool SetButtonIcon(Button button, bool showText, string iconName, Size? size = null)
        {
            try
            {
                var bitmap = LoadIconAsBitmap(iconName, size);
                if (bitmap == null) return false;

                button.Image = bitmap;
                if (showText)
                {
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextAlign = ContentAlignment.MiddleRight;
                }
                else
                {
                    button.ImageAlign = ContentAlignment.MiddleCenter;
                    button.Text = string.Empty;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting button icon {iconName}: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region SetIcon
        public static Icon SetIcon(string iconName, Size? size = null)
        {
            return LoadIcon(iconName);
        }
        #endregion

        #region GetAvailableIcons
        public static List<string> GetAvailableIcons()
        {
            Initialize();
            return new List<string>(_iconCache.Keys);
        }
        #endregion

        #region GetIconStatus()
        public static Dictionary<string, bool> GetIconStatus()
        {
            Initialize();
            var status = new Dictionary<string, bool>();

            // Check all icons in the Icons class
            var iconFields = typeof(Icons).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in iconFields)
            {
                if (field.FieldType == typeof(string))
                {
                    string iconName = (string)field.GetValue(null);
                    status[iconName] = _iconCache.ContainsKey(iconName);
                }
            }

            return status;
        }
        #endregion

        #region IconExists
        public static bool IconExists(string iconName)
        {
            string iconFilePath = Path.Combine(iconPath, iconName);
            return File.Exists(iconFilePath);
        }
        #endregion

        #region GetIconPath
        public static string GetIconPath(string iconName)
        {
            return Path.Combine(iconPath, iconName);
        }
        #endregion

        #region PreloadCommonIcons
        public static void PreloadCommonIcons()
        {
            LoadIcon(Icons.Expand);
            LoadIcon(Icons.Collapse);
            LoadIcon(Icons.Add);
            LoadIcon(Icons.Remove);

            LoadIcon(Icons.Save);
            LoadIcon(Icons.Reset);
            LoadIcon(Icons.Preset);
            LoadIcon(Icons.Generate);
            LoadIcon(Icons.Cancel);
            LoadIcon(Icons.OK);

            LoadIcon(Icons.Settings);
            LoadIcon(Icons.About);
            LoadIcon(Icons.Help);

            LoadIcon(Icons.App);
            LoadIcon(Icons.App256);

            LoadIcon(Icons.Info);
            LoadIcon(Icons.Warning);
            LoadIcon(Icons.Error);

            System.Diagnostics.Debug.WriteLine("IconManager: Common icons preloaded");
        }
        #endregion

        #region ClearCache
        public static void ClearCache()
        {
            foreach (var icon in _iconCache.Values)
            {
                icon?.Dispose();
            }
            _iconCache.Clear();

            foreach (var bitmap in _bitmapCache.Values)
            {
                bitmap?.Dispose();
            }
            _bitmapCache.Clear();

            _isInitialized = false;
            System.Diagnostics.Debug.WriteLine("IconManager: Cache cleared");
        }
        #endregion

        #region GetMemoryInfo
        public static string GetMemoryInfo()
        {
            return $"Icons cached: {_iconCache.Count}, Bitmaps cached: {_bitmapCache.Count}, Initialized: {_isInitialized}";
        }
        #endregion

        #region ValidateIconFiles
        public static List<string> ValidateIconFiles()
        {
            var missingIcons = new List<string>();
            var iconFields = typeof(Icons).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            foreach (var field in iconFields)
            {
                if (field.FieldType == typeof(string))
                {
                    string iconName = (string)field.GetValue(null);
                    if (!IconExists(iconName))
                    {
                        missingIcons.Add(iconName);
                    }
                }
            }

            return missingIcons;
        }
        #endregion

        #region InitializeAndValidate
        public static bool InitializeAndValidate()
        {
            Initialize();
            PreloadCommonIcons();

            var missingIcons = ValidateIconFiles();
            if (missingIcons.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"IconManager: Warning - Missing icons: {string.Join(", ", missingIcons)}");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("IconManager: All icons validated successfully");
            return true;
        }
        #endregion
    }
    #endregion
}
