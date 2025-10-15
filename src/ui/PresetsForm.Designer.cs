using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    #region PresetsForm
    partial class PresetsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel activityPanel;
        private Label presetsListLabel;
        private ListBox presetsListBox;
        private Label presetInfoLabel;
        private TextBox presetInfoTextBox;
        private Panel controlPresetsPanel;
        private Button addBtn;
        private Button removeBtn;
        private Button exportBtn;
        private Button importBtn;
        private Panel controlPanel;
        private Button applyBtn;
        private Button cancelBtn;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region InitializeComponent
        private void InitializeComponent()
        {
            // Load configurations
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var presetsFormConfig = config.PresetForm;

            #region Form Settings
            this.SuspendLayout();
            this.Text = LangManager.GetString("PresetsForm_Title", "Presets");
            this.Size = new Size(presetsFormConfig.FormWidth, presetsFormConfig.FormHeight);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = presetsFormConfig.Resizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = presetsFormConfig.Resizable;
            this.ShowInTaskbar = false;
            IconManager.SetFormIcon(this, IconManager.Icons.Preset);
            LangManager.Initialize();
            #endregion

            #region ActivityPanel
            activityPanel = new Panel()
            {
                Name = "activityPanel",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(globalConfig.Padding)
            };

            #region presetsListLabel
            presetsListLabel = new Label()
            {
                Name = "presetsListLabel",
                Text = LangManager.GetString("PresetsForm_PresetsListLabel", "Select a preset:"),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
            };
            ThemeManager.SetLabelStyle(presetsListLabel, ThemeManager.Type.Header);
            ToolTipManager.SetToolTip(presetsListLabel, LangManager.GetString("PresetsForm_PresetsListLabel_Tooltip", "Presets is a collection of predefined settings\n that can be applied to quickly\n configure the application."));
            #endregion

            #region presetsListBox
            presetsListBox = new ListBox()
            {
                Name = "presetsListBox",
                Size = new Size(presetsFormConfig.ListBoxWidth, presetsFormConfig.ListBoxHeight),
                Dock = DockStyle.Fill,
                SelectionMode = SelectionMode.One
            };
            ToolTipManager.SetToolTip(presetsListBox, LangManager.GetString("PresetsForm_ListBox_Tooltip", "Select a preset to apply."));
            presetsListBox.SelectedIndexChanged += PresetsListBox_SelectedIndexChanged;
            #endregion

            #region Preset Info Panel
            Panel infoPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, globalConfig.Spacing, 0, 0)
            };

            presetInfoLabel = new Label()
            {
                Name = "presetInfoLabel",
                Text = LangManager.GetString("PresetsForm_InfoLabel", "Info:"),
                Dock = DockStyle.Top,
                Height = presetsFormConfig.LabelHeight,
                TextAlign = ContentAlignment.BottomLeft,
            };
            ThemeManager.SetLabelStyle(presetInfoLabel, ThemeManager.Type.SubHeader);

            presetInfoTextBox = new TextBox()
            {
                Name = "presetInfoTextBox",
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };
            ThemeManager.SetTextBoxStyle(presetInfoTextBox, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(presetInfoTextBox, LangManager.GetString("PresetsForm_Info_Tooltip", "Preset information and description."));

            infoPanel.Controls.Add(presetInfoTextBox);
            infoPanel.Controls.Add(presetInfoLabel);
            #endregion

            #region Control Presets Panel
            controlPresetsPanel = new Panel()
            {
                Name = "controlPresetsPanel",
                Dock = DockStyle.Fill,
                Height = presetsFormConfig.ControlPanelHeight
            };
            int yPos = globalConfig.Padding;

            // Add Button
            addBtn = new Button()
            {
                Name = "addBtn",
                Text = LangManager.GetString("PresetsForm_AddButton", "Add"),
                Size = new Size(presetsFormConfig.ControlPanelButtonWidth, presetsFormConfig.ControlPanelButtonHeight),
                Location = new Point(applyBtn.Right + globalConfig.Spacing, yPos)
            };
            ThemeManager.SetButtonStyle(addBtn);
            IconManager.SetButtonIcon(addBtn, false, IconManager.Icons.Add, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(addBtn, LangManager.GetString("PresetsForm_AddButton_Tooltip", "Add a new preset."));
            addBtn.Click += AddBtn_Click;

            // Remove Button
            removeBtn = new Button()
            {
                Name = "removeBtn",
                Text = LangManager.GetString("PresetsForm_RemoveButton", "Remove"),
                Size = new Size(presetsFormConfig.ControlPanelButtonWidth, presetsFormConfig.ControlPanelButtonHeight),
                Location = new Point(addBtn.Right + globalConfig.Spacing, yPos),
                Enabled = false
            };
            ThemeManager.SetButtonStyle(removeBtn);
            IconManager.SetButtonIcon(removeBtn, false, IconManager.Icons.Remove, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(removeBtn, LangManager.GetString("PresetsForm_RemoveButton_Tooltip", "Remove the selected preset."));
            removeBtn.Click += RemoveBtn_Click;
            
            // Export Button
            exportBtn = new Button()
            {
                Name = "exportBtn",
                Text = LangManager.GetString("PresetsForm_ExportButton", "Export"),
                Size = new Size(presetsFormConfig.ControlPanelButtonWidth, presetsFormConfig.ControlPanelButtonHeight),
                Location = new Point(removeBtn.Right + globalConfig.Spacing, yPos),
                Enabled = false
            };
            ThemeManager.SetButtonStyle(exportBtn);
            IconManager.SetButtonIcon(exportBtn, false, IconManager.Icons.Generate, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(exportBtn, LangManager.GetString("PresetsForm_ExportButton_Tooltip", "Export the selected preset."));
            exportBtn.Click += ExportBtn_Click;

            // Import Button
            importBtn = new Button()
            {
                Name = "importBtn",
                Text = LangManager.GetString("PresetsForm_ImportButton", "Import"),
                Size = new Size(presetsFormConfig.ControlPanelButtonWidth, presetsFormConfig.ControlPanelButtonHeight),
                Location = new Point(exportBtn.Right + globalConfig.Spacing, yPos)
            };
            ThemeManager.SetButtonStyle(importBtn);
            IconManager.SetButtonIcon(importBtn, false, IconManager.Icons.Add, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(importBtn, LangManager.GetString("PresetsForm_ImportButton_Tooltip", "Import a preset from file."));
            importBtn.Click += ImportBtn_Click;

            // Add controls to controlPresetsPanel
            controlPresetsPanel.Controls.AddRange(new Control[] {
                addBtn,
                removeBtn,
                exportBtn,
                importBtn
            });
            #endregion

            #endregion

            #region Control Panel
            controlPanel = new Panel()
            {
                Name = "controlPanel",
                Dock = DockStyle.Fill,
                Height = presetsFormConfig.ControlPanelHeight
            };

            // Cancel Button
            cancelBtn = new Button()
            {
                Name = "cancelBtn",
                Text = LangManager.GetString("PresetsForm_CancelButton", "Cancel"),
                Size = new Size(presetsFormConfig.ControlPanelButtonWidth, presetsFormConfig.ControlPanelButtonHeight),
                Location = new Point(controlPanel.Width - presetsFormConfig.Padding, presetsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter
            };
            ThemeManager.SetButtonStyle(cancelBtn);
            IconManager.SetButtonIcon(cancelBtn, false, IconManager.Icons.Cancel, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(cancelBtn, LangManager.GetString("PresetsForm_CancelButton_Tooltip", "Cancel any changes and close this window."));
            cancelBtn.Click += CancelBtn_Click;

            // Apply Button
            applyBtn = new Button()
            {
                Name = "applyBtn",
                Text = LangManager.GetString("PresetsForm_ApplyButton", "Apply"),
                Size = new Size(presetsFormConfig.ControlPanelButtonWidth, presetsFormConfig.ControlPanelButtonHeight),
                Location = new Point(cancelBtn.Left - presetsFormConfig.Spacing, presetsFormConfig.Padding),
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false
            };
            ThemeManager.SetButtonStyle(applyBtn);
            IconManager.SetButtonIcon(applyBtn, false, IconManager.Icons.OK, new Size(globalConfig.IconSize, globalConfig.IconSize));
            ToolTipManager.SetToolTip(applyBtn, LangManager.GetString("PresetsForm_ApplyButton_Tooltip", "Apply the selected preset."));
            applyBtn.Click += ApplyBtn_Click;

            // Add controls to controlPanel
            controlPanel.Controls.AddRange(new Control[] {
                applyBtn,
                cancelBtn
            });
            #endregion

            this.ResumeLayout(false);
        }
        #endregion
    }
    #endregion
}
