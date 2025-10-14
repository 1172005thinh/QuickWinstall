using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel activityPanel;
        private Panel controlPanel;
        private Button settingsBtn;
        private Button clearBtn;
        private Button presetsBtn;
        private Button genBtn;
        private Button cancelBtn;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusPrefixLabel;
        private ToolStripStatusLabel statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Initialization
        private void InitializeComponent()
        {
            // Load configurations
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            #region Form Settings
            this.Text = LangManager.GetString("MainForm_FormTitle", "QuickWinstall");
            this.Size = new Size(mainFormConfig.FormWidth, mainFormConfig.FormHeight);
            this.MinimumSize = new Size(mainFormConfig.FormWidthMin, mainFormConfig.FormHeightMin);
            this.MaximumSize = new Size(mainFormConfig.FormWidthMax, mainFormConfig.FormHeightMax);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            IconManager.SetFormIcon(this, IconManager.Icons.Preset);
            #endregion

            #region Activity Panel
            activityPanel = new Panel
            {
                Name = "activityPanel",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(mainFormConfig.Padding),
            };
            #endregion

            #region Control Panel
            controlPanel = new Panel
            {
                Name = "controlPanel",
                Dock = DockStyle.Bottom,
                Size = new Size(mainFormConfig.ControlPanelWidth, mainFormConfig.ControlPanelHeight),
                Padding = new Padding(mainFormConfig.Padding),
            };

            // Settings Button
            settingsBtn = new Button
            {
                Name = "settingsBtn",
                Text = LangManager.GetString("MainForm_SettingsButton", "Settings"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(mainFormConfig.Padding, mainFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(settingsBtn);
            ToolTipManager.SetToolTip(settingsBtn, LangManager.GetString("MainForm_SettingsButton_Tooltip", "Open application settings."));
            IconManager.SetButtonIcon(settingsBtn, true, IconManager.Icons.Settings, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize));
            settingsBtn.Click += settingsBtn_Click;

            // Clear Button
            clearBtn = new Button
            {
                Name = "clearBtn",
                Text = LangManager.GetString("MainForm_ClearButton", "Clear"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(settingsBtn.Right + mainFormConfig.Spacing, mainFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(clearBtn);
            ToolTipManager.SetToolTip(clearBtn, LangManager.GetString("MainForm_ClearButton_Tooltip", "Clear all configurations."));
            IconManager.SetButtonIcon(clearBtn, true, IconManager.Icons.Clear, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize));
            clearBtn.Click += clearBtn_Click;

            // Preset Button
            presetsBtn = new Button
            {
                Name = "presetsBtn",
                Text = LangManager.GetString("MainForm_PresetsButton", "Preset"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(clearBtn.Right + mainFormConfig.Spacing, mainFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(presetsBtn);
            ToolTipManager.SetToolTip(presetsBtn, LangManager.GetString("MainForm_PresetsButton_Tooltip", "Load recommended settings."));
            IconManager.SetButtonIcon(presetsBtn, true, IconManager.Icons.Preset, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize));
            presetsBtn.Click += presetsBtn_Click;

            // Cancel Button
            cancelBtn = new Button
            {
                Name = "cancelBtn",
                Text = LangManager.GetString("MainForm_CancelButton", "Cancel"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(controlPanel.Width - mainFormConfig.Padding, mainFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false
            };
            ThemeManager.SetButtonStyle(cancelBtn);
            ToolTipManager.SetToolTip(cancelBtn, LangManager.GetString("MainForm_CancelButton_Tooltip", "Exit the application without saving."));
            IconManager.SetButtonIcon(cancelBtn, true, IconManager.Icons.Cancel, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize));
            cancelBtn.Click += cancelBtn_Click;

            // Generate Button
            genBtn = new Button
            {
                Name = "genBtn",
                Text = LangManager.GetString("MainForm_GenerateButton", "Generate"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(cancelBtn.Left - mainFormConfig.Spacing, mainFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(genBtn);
            ToolTipManager.SetToolTip(genBtn, LangManager.GetString("MainForm_GenerateButton_Tooltip", "Generate the autounattend.xml file."));
            IconManager.SetButtonIcon(genBtn, true, IconManager.Icons.Generate, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize));
            genBtn.Click += genBtn_Click;

            // Add buttons to control panel
            controlPanel.Controls.AddRange(new Control[] {
                settingsBtn,
                clearBtn,
                presetsBtn,
                genBtn,
                cancelBtn
            });
            #endregion

            #region Status Strip
            statusStrip = new StatusStrip
            {
                Name = "statusStrip",
                Dock = DockStyle.Bottom,
                Size = new Size(mainFormConfig.StatusStripWidth, mainFormConfig.StatusStripHeight),
            };

            // Status Prefix Label
            statusPrefixLabel = new ToolStripStatusLabel
            {
                Name = "statusPrefixLabel",
                Text = LangManager.GetString("MainForm_StatusPrefixLabel", "Status:"),
                Size = new Size(mainFormConfig.StatusStripWidth * 20 / 100, mainFormConfig.StatusStripHeight),
                Spring = false,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Status Label
            statusLabel = new ToolStripStatusLabel
            {
                Name = "statusLabel",
                Text = LangManager.GetString("MainForm_StatusLabel_Ready", "Language packages are missing or corrupted."),
                Size = new Size(mainFormConfig.StatusStripWidth * 80 / 100, mainFormConfig.StatusStripHeight),
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Add both labels to status strip
            statusStrip.Items.AddRange(new ToolStripItem[] {
                statusPrefixLabel,
                statusLabel
            });
            #endregion
        }
        #endregion
    }
}