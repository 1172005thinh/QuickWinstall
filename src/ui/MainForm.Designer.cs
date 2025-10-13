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
            this.Icon = IconManager.SetFormIcon(this, IconManager.Icons.Preset);
            #endregion

            #region Activity Panel
            activityPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(mainFormConfig.Padding),
            };
            #endregion

            #region Control Panel
            controlPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Size = new Size(mainFormConfig.ControlPanelWidth, mainFormConfig.ControlPanelHeight),
                Padding = new Padding(mainFormConfig.Padding),
            };

            // Settings Button
            settingsBtn = new Button
            {
                Text = LangManager.GetString("MainForm_SettingsButton", "Settings"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(
                    mainFormConfig.Padding + mainFormConfig.ControlPanelButtonWidth / 2,
                    mainFormConfig.Padding + mainFormConfig.ControlPanelButtonHeight / 2),
                Icon = IconManager.GetIcon(IconManager.Icons.Settings, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize)),
                TextAlign = ContentAlignment.MiddleCenter
            };
            settingsBtn.Click += SettingsBtn_Click;

            // Clear Button
            clearBtn = new Button
            {
                Text = LangManager.GetString("MainForm_ClearButton", "Clear"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(
                    settingsBtn.Right + mainFormConfig.Spacing + mainFormConfig.ControlPanelButtonWidth / 2,
                    mainFormConfig.Padding + mainFormConfig.ControlPanelButtonHeight / 2),
                Icon = IconManager.GetIcon(IconManager.Icons.Clear, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize)),
                TextAlign = ContentAlignment.MiddleCenter
            };
            clearBtn.Click += ClearBtn_Click;

            // Preset Button
            presetsBtn = new Button
            {
                Text = LangManager.GetString("MainForm_PresetsButton", "Preset"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(
                    clearBtn.Right + mainFormConfig.Spacing + mainFormConfig.ControlPanelButtonWidth / 2,
                    mainFormConfig.Padding + mainFormConfig.ControlPanelButtonHeight / 2),
                Icon = IconManager.GetIcon(IconManager.Icons.Preset, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize)),
                TextAlign = ContentAlignment.MiddleCenter
            };
            presetsBtn.Click += PresetsBtn_Click;

            // Cancel Button
            cancelBtn = new Button
            {
                Text = LangManager.GetString("MainForm_CancelButton", "Cancel"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(
                    controlPanel.Width - mainFormConfig.Padding - mainFormConfig.ControlPanelButtonWidth / 2,
                    mainFormConfig.Padding + mainFormConfig.ControlPanelButtonHeight / 2),
                Icon = IconManager.GetIcon(IconManager.Icons.Cancel, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize)),
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false
            };

            // Generate Button
            genBtn = new Button
            {
                Text = LangManager.GetString("MainForm_GenerateButton", "Generate"),
                Size = new Size(mainFormConfig.ControlPanelButtonWidth, mainFormConfig.ControlPanelButtonHeight),
                Location = new Point(
                    cancelBtn.Left - mainFormConfig.Spacing - mainFormConfig.ControlPanelButtonWidth / 2,
                    mainFormConfig.Padding + mainFormConfig.ControlPanelButtonHeight / 2),
                Icon = IconManager.GetIcon(IconManager.Icons.Generate, new Size(mainFormConfig.IconSize, mainFormConfig.IconSize)),
                TextAlign = ContentAlignment.MiddleCenter
            };
            genBtn.Click += GenBtn_Click;

            // Add buttons to control panel
            controlPanel.Controls.AddRange(new Control[] {
                settingsBtn,
                clearBtn,
                presetBtn,
                genBtn,
                cancelBtn
            });
            #endregion

            #region Status Strip
            statusStrip = new StatusStrip
            {
                Dock = DockStyle.Bottom,
                Size = new Size(mainFormConfig.StatusStripWidth, mainFormConfig.StatusStripHeight),
            };

            // Status Prefix Label
            statusPrefixLabel = new ToolStripStatusLabel
            {
                Text = LangManager.GetString("MainForm_StatusPrefixLabel", "Status:"),
                Size = new Size(mainFormConfig.StatusStripWidth * 20 / 100, mainFormConfig.StatusStripHeight),
                Spring = false,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            // Status Label
            statusLabel = new ToolStripStatusLabel
            {
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