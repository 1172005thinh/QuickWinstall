using System.Windows.Forms;

namespace QuickWinstall.Lib
{
    #region ToolTipManager
    public class ToolTipManager
    {
        private static ToolTip _toolTipInstance;
        private ToolTipManager() { }
        public static ToolTip Instance
        {
            
            get
            {
                var config = Config.LoadFromAppFolder();
                var globalConfig = config.Global;

                if (_toolTipInstance == null)
                {
                    _toolTipInstance = new ToolTip
                    {
                        AutoPopDelay = globalConfig.AutoPopDelay,
                        InitialDelay = globalConfig.InitialDelay,
                        ReshowDelay = globalConfig.ReshowDelay,
                        ShowAlways = true
                    };
                }
                return _toolTipInstance;
            }
        }
        public static void SetToolTip(Control control, string text)
        {
            Instance.SetToolTip(control, text);
        }
    }
    #endregion
}