using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    #region SettingsForm
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel activityPanel;
        private Panel controlPanel;
        private Label langLabel;
        private ComboBox langCombo;
        private Label themeLabel;
        private ComboBox themeCombo;
        private Label savePathLabel;
        private TextBox savePathTextBox;
        private Button browseBtn;
        private Button aboutBtn;
        private Button helpBtn;
        private Button saveBtn;
        private Button cancelBtn;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region InitializationComponent
        private void InitializeComponent()
        {
            // Load configurations
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var settingsFormConfig = config.SettingsForm;

            #region Form Settings
            this.SuspendLayout();
            this.Text = LangManager.GetString("SettingsForm_Title", "Settings");
            this.Size = new Size(settingsFormConfig.FormWidth, settingsFormConfig.FormHeight);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = settingsFormConfig.Resizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = settingsFormConfig.Resizable;
            this.ShowInTaskbar = false;
            IconManager.SetFormIcon(this, IconManager.Icons.Settings);
            LangManager.Initialize();
            #endregion

            #region ActivityPanel
            activityPanel = new Panel
            {
                Name = "activityPanel",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(settingsFormConfig.Padding)
            };

            #region Language
            langLabel = new Label
            {
                Name = "languageLabel",
                Text = LangManager.GetString("SettingsForm_LanguageLabel", "Language:"),
                Size = new Size(settingsFormConfig.LabelWidth, settingsFormConfig.LabelHeight),
                Location = new Point(settingsFormConfig.Padding, settingsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleLeft
            };
            ThemeManager.SetLabelStyle(langLabel, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(langLabel, LangManager.GetString("SettingsForm_LanguageLabel_Tooltip", "Application language DOES NOT affect Windows UI Language."));

            langCombo = new ComboBox
            {
                Name = "languageCombo",
                Size = new Size(settingsFormConfig.ComboboxWidth, settingsFormConfig.ComboboxHeight),
                Location = new Point(langLabel.Right + settingsFormConfig.Spacing, settingsFormConfig.Padding),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            ThemeManager.SetComboBoxStyle(langCombo, ThemeManager.Type.Normal);

            langCombo.Items.AddRange(new string[]
            {
                LangManager.GetString("SettingsForm_English", "English"),
                LangManager.GetString("SettingsForm_Vietnamese", "Tiếng Việt")
            });
            #endregion

            #region Theme
            themeLabel = new Label
            {
                Name = "themeLabel",
                Text = LangManager.GetString("SettingsForm_ThemeLabel", "Theme:"),
                Size = new Size(settingsFormConfig.LabelWidth, settingsFormConfig.LabelHeight),
                Location = new Point(settingsFormConfig.Padding, langLabel.Bottom + settingsFormConfig.Spacing),
                TextAlign = ContentAlignment.MiddleLeft
            };
            ThemeManager.SetLabelStyle(themeLabel, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(themeLabel, LangManager.GetString("SettingsForm_ThemeLabel_Tooltip", "Select application theme."));

            themeCombo = new ComboBox
            {
                Name = "themeCombo",
                Size = new Size(settingsFormConfig.ComboboxWidth, settingsFormConfig.ComboboxHeight),
                Location = new Point(themeLabel.Right + settingsFormConfig.Spacing, langLabel.Bottom + settingsFormConfig.Spacing),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            ThemeManager.SetComboBoxStyle(themeCombo, ThemeManager.Type.Normal);

            themeCombo.Items.AddRange(new string[]
            {
                LangManager.GetString("SettingsForm_ThemeLight", "Light"),
                LangManager.GetString("SettingsForm_ThemeDark", "Dark")
            });
            #endregion

            #region Save Path
            savePathLabel = new Label
            {
                Name = "savePathLabel",
                Text = LangManager.GetString("SettingsForm_SavePathLabel", "XML Save Path:"),
                Size = new Size(settingsFormConfig.LabelWidth, settingsFormConfig.LabelHeight),
                Location = new Point(settingsFormConfig.Padding, themeLabel.Bottom + settingsFormConfig.Spacing),
                TextAlign = ContentAlignment.MiddleLeft
            };
            ThemeManager.SetLabelStyle(savePathLabel, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(savePathLabel, LangManager.GetString("SettingsForm_SavePathLabel_Tooltip", "Select save path for autounattend.xml file."));

            savePathTextBox = new TextBox
            {
                Name = "savePathTextBox",
                Size = new Size(settingsFormConfig.TextboxWidth, settingsFormConfig.TextboxHeight),
                Location = new Point(savePathLabel.Right + settingsFormConfig.Spacing, themeLabel.Bottom + settingsFormConfig.Spacing)
            };
            ThemeManager.SetTextBoxStyle(savePathTextBox, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(savePathTextBox, LangManager.GetString("SettingsForm_SavePathTextBox_Tooltip", "Enter the XML save path."));

            browseBtn = new Button
            {
                Name = "browseBtn",
                Text = LangManager.GetString("SettingsForm_BrowseButton", "Browse"),
                Size = new Size(settingsFormConfig.ButtonBox, settingsFormConfig.ButtonBox),
                Location = new Point(savePathTextBox.Right + settingsFormConfig.Spacing, themeLabel.Bottom + settingsFormConfig.Spacing),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(browseBtn);
            IconManager.SetButtonIcon(browseBtn, false, IconManager.Icons.Browse, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(browseBtn, LangManager.GetString("SettingsForm_BrowseButton_Tooltip", "Browse for folder"));
            browseBtn.Click += BrowseButton_Click;
            #endregion
            
            #endregion

            #region Control Panel
            controlPanel = new Panel
            {
                Name = "controlPanel",
                Dock = DockStyle.Bottom,
                Size = new Size(settingsFormConfig.ControlPanelWidth, settingsFormConfig.ControlPanelHeight),
                Padding = new Padding(settingsFormConfig.Padding),
            };

            // About Button
            aboutBtn = new Button
            {
                Name = "aboutBtn",
                Text = LangManager.GetString("SettingsForm_AboutButton", "About"),
                Size = new Size(settingsFormConfig.ControlPanelButtonWidth, settingsFormConfig.ControlPanelButtonHeight),
                Location = new Point(settingsFormConfig.Padding, settingsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(aboutBtn);
            ToolTipManager.SetToolTip(aboutBtn, LangManager.GetString("SettingsForm_AboutButton_Tooltip", "About QuickWinstall"));
            IconManager.SetButtonIcon(aboutBtn, false, IconManager.Icons.Info, new Size(globalConfig.IconSize, globalConfig.IconSize));
            aboutBtn.Click += aboutBtn_Click;

            // Help Button
            helpBtn = new Button
            {
                Name = "helpBtn",
                Text = LangManager.GetString("SettingsForm_HelpButton", "Help"),
                Size = new Size(settingsFormConfig.ControlPanelButtonWidth, settingsFormConfig.ControlPanelButtonHeight),
                Location = new Point(aboutBtn.Right + settingsFormConfig.Spacing, settingsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(helpBtn);
            ToolTipManager.SetToolTip(helpBtn, LangManager.GetString("SettingsForm_HelpButton_Tooltip", "Open help documentation"));
            IconManager.SetButtonIcon(helpBtn, false, IconManager.Icons.Help, new Size(globalConfig.IconSize, globalConfig.IconSize));
            helpBtn.Click += helpBtn_Click;

            // Cancel Button
            cancelBtn = new Button
            {
                Name = "cancelBtn",
                Text = LangManager.GetString("SettingsForm_CancelButton", "Cancel"),
                Size = new Size(settingsFormConfig.ControlPanelButtonWidth, settingsFormConfig.ControlPanelButtonHeight),
                Location = new Point(controlPanel.Width - settingsFormConfig.Padding, settingsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            ThemeManager.SetButtonStyle(cancelBtn);
            ToolTipManager.SetToolTip(cancelBtn, LangManager.GetString("SettingsForm_CancelButton_Tooltip", "Cancel changes"));
            IconManager.SetButtonIcon(cancelBtn, false, IconManager.Icons.Cancel, new Size(globalConfig.IconSize, globalConfig.IconSize));
            cancelBtn.Click += cancelBtn_Click;

            // Save Button
            saveBtn = new Button
            {
                Name = "saveBtn",
                Text = LangManager.GetString("SettingsForm_SaveButton", "Save"),
                Size = new Size(settingsFormConfig.ControlPanelButtonWidth, settingsFormConfig.ControlPanelButtonHeight),
                Location = new Point(cancelBtn.Left - settingsFormConfig.Spacing, settingsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            ThemeManager.SetButtonStyle(saveBtn);
            ToolTipManager.SetToolTip(saveBtn, LangManager.GetString("SettingsForm_SaveButton_Tooltip", "Save settings"));
            IconManager.SetButtonIcon(saveBtn, false, IconManager.Icons.Save, new Size(globalConfig.IconSize, globalConfig.IconSize));
            saveBtn.Click += saveBtn_Click;

            // Add buttons to control panel
            controlPanel.Controls.AddRange(new Control[] {
                aboutBtn,
                helpBtn,
                saveBtn,
                cancelBtn
            });
            #endregion

            this.ResumeLayout(false);
        }
        #endregion
    }
    #endregion
}