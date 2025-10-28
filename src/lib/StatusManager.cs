using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuickWinstall.Lib
{
    public enum StatusType
    {
        Normal,
        Error,
        Warning,
        Success
    }

    public class StatusManager
    {
        private static StatusManager? _instance;
        private ToolStripStatusLabel? _statusLabel;
        private ThemeManager _themeManager;
        private LangManager _langManager;
        
        // Track current status for refresh capability
        private string _currentStatusKey = "mainForm.status.ready";
        private StatusType _currentStatusType = StatusType.Normal;

        private StatusManager()
        {
            _themeManager = ThemeManager.Instance;
            _langManager = LangManager.Instance;
        }

        public static StatusManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StatusManager();
                }
                return _instance;
            }
        }

        public void AttachStatusLabel(ToolStripStatusLabel statusLabel)
        {
            _statusLabel = statusLabel;
        }

        public void SetStatus(string message, StatusType statusType = StatusType.Normal)
        {
            if (_statusLabel == null)
            {
                Console.WriteLine($"Warning: Status label not attached. Message: {message}");
                return;
            }

            try
            {
                _statusLabel.Text = message;

                // Set font and color based on status type
                switch (statusType)
                {
                    case StatusType.Error:
                        _statusLabel.Font = _themeManager.GetFont("error");
                        _statusLabel.ForeColor = _themeManager.GetFontColor("error");
                        break;
                    case StatusType.Warning:
                        _statusLabel.Font = _themeManager.GetFont("warning");
                        _statusLabel.ForeColor = _themeManager.GetFontColor("warning");
                        break;
                    case StatusType.Success:
                        _statusLabel.Font = _themeManager.GetFont("success");
                        _statusLabel.ForeColor = _themeManager.GetFontColor("success");
                        break;
                    case StatusType.Normal:
                    default:
                        _statusLabel.Font = _themeManager.GetFont("normal");
                        _statusLabel.ForeColor = _themeManager.GetFontColor("normal");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to set status: {ex.Message}");
            }
        }

        public void SetStatusFromKey(string messageKey, StatusType statusType = StatusType.Normal, params object[] args)
        {
            _currentStatusKey = messageKey;
            _currentStatusType = statusType;
            string message = _langManager.GetString(messageKey, args);
            SetStatus(message, statusType);
        }
        
        /// <summary>
        /// Refresh the current status with updated language/theme
        /// </summary>
        public void RefreshStatus()
        {
            if (_statusLabel != null)
            {
                SetStatusFromKey(_currentStatusKey, _currentStatusType);
            }
        }

        public void SetReady()
        {
            SetStatusFromKey("mainForm.status.ready", StatusType.Normal);
        }

        public void SetLoading()
        {
            SetStatusFromKey("mainForm.status.loading", StatusType.Normal);
        }

        public void SetGenerating()
        {
            SetStatusFromKey("mainForm.status.generating", StatusType.Normal);
        }

        public void SetSuccess()
        {
            SetStatusFromKey("mainForm.status.success", StatusType.Success);
        }

        public void SetError(string errorMessage = "")
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                SetStatusFromKey("mainForm.status.error", StatusType.Error);
            }
            else
            {
                SetStatus(errorMessage, StatusType.Error);
            }
        }
    }
}
