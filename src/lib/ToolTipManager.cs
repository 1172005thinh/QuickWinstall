using System;
using System.Windows.Forms;

namespace QuickWinstall.Lib
{
    public class ToolTipManager
    {
        private static ToolTipManager? _instance;
        private ToolTip _toolTip;
        private LangManager _langManager;

        private ToolTipManager()
        {
            _langManager = LangManager.Instance;
            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                ShowAlways = true
            };
        }

        public static ToolTipManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ToolTipManager();
                }
                return _instance;
            }
        }

        public void SetToolTip(Control control, string tooltipKey, params object[] args)
        {
            try
            {
                string tooltipText = _langManager.GetString(tooltipKey, args);
                _toolTip.SetToolTip(control, tooltipText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to set tooltip: {ex.Message}");
            }
        }

        public void RemoveToolTip(Control control)
        {
            try
            {
                _toolTip.SetToolTip(control, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to remove tooltip: {ex.Message}");
            }
        }

        public void UpdateAllToolTips()
        {
            // This method can be called when language changes to update all tooltips
            // Implementation would require tracking all controls with tooltips
        }
    }
}
