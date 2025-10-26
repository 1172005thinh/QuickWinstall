using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickWinstall.Lib;
using QuickWinstall.Config;

namespace QuickWinstall.Main
{
    public partial class MainForm : Form
    {
        private UIValues _uiValues;
        private ThemeManager _themeManager;
        private LangManager _langManager;
        private StatusManager _statusManager;
        private ToolTipManager _toolTipManager;
        private IconManager _iconManager;
        private ConfigValues _configValues;
        private SettingsManager _settingsManager;
        private XMLGenerator _xmlGenerator;

        private bool _hasUnsavedChanges = false;
        private bool _isGeneralConfigExpanded = true;

        public MainForm()
        {
            // Initialize managers
            _uiValues = UIValues.Instance;
            _themeManager = ThemeManager.Instance;
            _langManager = LangManager.Instance;
            _statusManager = StatusManager.Instance;
            _toolTipManager = ToolTipManager.Instance;
            _iconManager = IconManager.Instance;
            _configValues = ConfigValues.Instance;
            _settingsManager = SettingsManager.Instance;
            _xmlGenerator = XMLGenerator.Instance;

            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Attach status label to status manager
            _statusManager.AttachStatusLabel(lblStatus);

            // Set initial status
            _statusManager.SetReady();

            // Load last configuration if enabled
            if (_settingsManager.SaveLastConfig)
            {
                // TODO: Load last config from lastConfig.json
            }

            // Apply theme to form
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            this.BackColor = _themeManager.GetColor("background");
            this.ForeColor = _themeManager.GetFontColor("normal");
            
            // Apply theme to panels
            pnlBanner.BackColor = _themeManager.GetColor("background");
            pnlConfigSection.BackColor = _themeManager.GetColor("background");
            pnlControlPanel.BackColor = _themeManager.GetColor("background");
            statusStrip.BackColor = _themeManager.GetColor("background");
        }

        // Event handlers
        private void BtnSettings_Click(object sender, EventArgs e)
        {
            // TODO: Open SettingsForm
            MessageBox.Show("Settings form not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                _langManager.GetString("dialogs.clear.message"),
                _langManager.GetString("dialogs.clear.title"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (result == DialogResult.OK)
            {
                _configValues.Clear();
                ClearForm();
                _hasUnsavedChanges = false;
                _statusManager.SetStatus("Configuration cleared", StatusType.Success);
            }
        }

        private void BtnPreset_Click(object sender, EventArgs e)
        {
            // TODO: Open PresetsForm
            MessageBox.Show("Presets form not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    _langManager.GetString("dialogs.unsaved.message"),
                    _langManager.GetString("dialogs.unsaved.title"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2
                );

                if (result == DialogResult.Cancel)
                    return;
            }

            Application.Exit();
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            // Validate configuration
            List<string> errors = _configValues.Validate();
            if (errors.Count > 0)
            {
                MessageBox.Show(
                    _langManager.GetString("dialogs.validation.message", errors[0]),
                    _langManager.GetString("dialogs.validation.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _statusManager.SetError(errors[0]);
                return;
            }

            // Check save path
            string savePath = _settingsManager.SavePath;
            if (string.IsNullOrEmpty(savePath) || !System.IO.Directory.Exists(savePath))
            {
                MessageBox.Show(
                    _langManager.GetString("dialogs.emptySavePath.message"),
                    _langManager.GetString("dialogs.emptySavePath.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            string outputPath = System.IO.Path.Combine(savePath, "autounattend.xml");

            // Check if file exists
            if (System.IO.File.Exists(outputPath))
            {
                var result = MessageBox.Show(
                    _langManager.GetString("dialogs.overwrite.message"),
                    _langManager.GetString("dialogs.overwrite.title"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1
                );

                if (result == DialogResult.Cancel)
                    return;
            }

            // Generate XML
            _statusManager.SetGenerating();
            bool success = _xmlGenerator.GenerateXML(outputPath, _configValues.GetAllValues());

            if (success)
            {
                MessageBox.Show(
                    _langManager.GetString("dialogs.success.message", savePath),
                    _langManager.GetString("dialogs.success.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                _statusManager.SetSuccess();
                _hasUnsavedChanges = false;
            }
            else
            {
                MessageBox.Show(
                    "Failed to generate autounattend.xml file.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _statusManager.SetError("Failed to generate file");
            }
        }

        private void BtnGeneralConfigToggle_Click(object sender, EventArgs e)
        {
            _isGeneralConfigExpanded = !_isGeneralConfigExpanded;
            pnlGeneralConfigContent.Visible = _isGeneralConfigExpanded;
            pnlGeneralConfigSeparator.Visible = _isGeneralConfigExpanded;
            btnGeneralConfigToggle.Text = _isGeneralConfigExpanded ? "−" : "+";
        }

        private void BtnExpandAll_Click(object sender, EventArgs e)
        {
            // Expand all sections
            _isGeneralConfigExpanded = true;
            pnlGeneralConfigContent.Visible = true;
            pnlGeneralConfigSeparator.Visible = true;
            btnGeneralConfigToggle.Text = "−";
        }

        private void BtnCollapseAll_Click(object sender, EventArgs e)
        {
            // Collapse all sections
            _isGeneralConfigExpanded = false;
            pnlGeneralConfigContent.Visible = false;
            pnlGeneralConfigSeparator.Visible = false;
            btnGeneralConfigToggle.Text = "+";
        }

        private void ClearForm()
        {
            // Clear GeneralConfig inputs
            cmbWindowsEdition.SelectedIndex = 0;
            txtProductKey1.Clear();
            txtProductKey2.Clear();
            txtProductKey3.Clear();
            txtProductKey4.Clear();
            txtProductKey5.Clear();
            cmbCPUArch.SelectedIndex = 0;
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            _hasUnsavedChanges = true;
            _statusManager.SetStatus(_langManager.GetString("mainForm.status.unsavedChanges"), StatusType.Warning);

            // Update ConfigValues
            UpdateConfigValues();
        }

        private void UpdateConfigValues()
        {
            // Update WindowsEdition
            if (cmbWindowsEdition.SelectedIndex > 0)
            {
                _configValues.General.WindowsEdition = cmbWindowsEdition.SelectedItem?.ToString() ?? "";
            }
            else
            {
                _configValues.General.WindowsEdition = "";
            }

            // Update ProductKey
            string pk1 = txtProductKey1.Text.Trim();
            string pk2 = txtProductKey2.Text.Trim();
            string pk3 = txtProductKey3.Text.Trim();
            string pk4 = txtProductKey4.Text.Trim();
            string pk5 = txtProductKey5.Text.Trim();

            if (string.IsNullOrEmpty(pk1) && string.IsNullOrEmpty(pk2) && 
                string.IsNullOrEmpty(pk3) && string.IsNullOrEmpty(pk4) && string.IsNullOrEmpty(pk5))
            {
                _configValues.General.ProductKey = "";
            }
            else
            {
                _configValues.General.ProductKey = $"{pk1}-{pk2}-{pk3}-{pk4}-{pk5}";
            }

            // Update CPUArchitecture
            if (cmbCPUArch.SelectedIndex > 0)
            {
                string selected = cmbCPUArch.SelectedItem?.ToString() ?? "";
                if (selected.Contains("x64"))
                    _configValues.General.CPUArchitecture = "amd64";
                else if (selected.Contains("ARM64"))
                    _configValues.General.CPUArchitecture = "arm64";
                else
                    _configValues.General.CPUArchitecture = "";
            }
            else
            {
                _configValues.General.CPUArchitecture = "";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    _langManager.GetString("dialogs.unsaved.message"),
                    _langManager.GetString("dialogs.unsaved.title"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2
                );

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            base.OnFormClosing(e);
        }
    }
}
