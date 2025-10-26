using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace QuickWinstall.Lib
{
    public class UIValues
    {
        #region Singleton

        private static UIValues? _instance;

        public static UIValues Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIValues();
                }
                return _instance;
            }
        }

        private UIValues()
        {
            LoadUIConfig();
        }

        #endregion

        #region Fields

        private JObject? _uiConfig;

        #endregion

        #region Properties

        // Global spacing
        public int GlobalSpacingX { get; private set; } = 20;
        public int GlobalTabX { get; private set; } = 20;
        public int GlobalSpacingY { get; private set; } = 10;
        
        // Global buttons
        public int GlobalBtnWidth { get; private set; } = 100;
        public int GlobalBtnHeight { get; private set; } = 40;
        public int GlobalBtnBox { get; private set; } = 40;
        
        // Global labels
        public int GlobalLabelWidth { get; private set; } = 200;
        public int GlobalLabelHeight { get; private set; } = 30;
        
        // Global icons
        public int GlobalIconSize { get; private set; } = 16;
        
        // Global inputs
        public int GlobalInputWidth { get; private set; } = 400;
        public int GlobalInputHeight { get; private set; } = 30;
        
        // Global panels
        public int BannerHeight { get; private set; } = 100;
        public int ControlPanelHeight { get; private set; } = 60;
        public int StatusBarHeight { get; private set; } = 40;

        #endregion

        #region Methods

        private void LoadUIConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "ui", "ui.json");
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    _uiConfig = JObject.Parse(jsonContent);

                    // Load global spacing values
                    GlobalSpacingX = _uiConfig["global"]?["spacing"]?["spacingX"]?.ToObject<int>() ?? 20;
                    GlobalTabX = _uiConfig["global"]?["spacing"]?["tabX"]?.ToObject<int>() ?? 20;
                    GlobalSpacingY = _uiConfig["global"]?["spacing"]?["spacingY"]?.ToObject<int>() ?? 10;
                    
                    // Load global button values
                    GlobalBtnWidth = _uiConfig["global"]?["buttons"]?["width"]?.ToObject<int>() ?? 100;
                    GlobalBtnHeight = _uiConfig["global"]?["buttons"]?["height"]?.ToObject<int>() ?? 40;
                    GlobalBtnBox = _uiConfig["global"]?["buttons"]?["box"]?.ToObject<int>() ?? 40;
                    
                    // Load global label values
                    GlobalLabelWidth = _uiConfig["global"]?["labels"]?["width"]?.ToObject<int>() ?? 200;
                    GlobalLabelHeight = _uiConfig["global"]?["labels"]?["height"]?.ToObject<int>() ?? 30;
                    
                    // Load global icon values
                    GlobalIconSize = _uiConfig["global"]?["icons"]?["size"]?.ToObject<int>() ?? 16;
                    
                    // Load global input values
                    GlobalInputWidth = _uiConfig["global"]?["inputs"]?["width"]?.ToObject<int>() ?? 400;
                    GlobalInputHeight = _uiConfig["global"]?["inputs"]?["height"]?.ToObject<int>() ?? 30;
                    
                    // Load global panel values
                    BannerHeight = _uiConfig["global"]?["banner"]?["height"]?.ToObject<int>() ?? 100;
                    ControlPanelHeight = _uiConfig["global"]?["controlPanel"]?["height"]?.ToObject<int>() ?? 60;
                    StatusBarHeight = _uiConfig["global"]?["statusBar"]?["height"]?.ToObject<int>() ?? 40;
                }
            }
            catch (Exception ex)
            {
                // If loading fails, use default hardcoded values
                Console.WriteLine($"Warning: Failed to load UI config: {ex.Message}. Using default values.");
            }
        }

        public int GetSectionValue(string section, string key, int defaultValue = 0)
        {
            try
            {
                return _uiConfig?["sections"]?[section]?[key]?.ToObject<int>() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public int GetValue(string path, int defaultValue = 0)
        {
            try
            {
                string[] parts = path.Split('.');
                JToken? current = _uiConfig;
                
                foreach (string part in parts)
                {
                    current = current?[part];
                    if (current == null)
                        return defaultValue;
                }
                
                return current?.ToObject<int>() ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion
    }
}
