using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;
using QuickWinstall.Config;

namespace QuickWinstall.Main
{
    public partial class MainForm : Form
    {
        #region Fields

        private UIValues _uiValues;
        private ThemeManager _themeManager;
        private LangManager _langManager;
        private StatusManager _statusManager;
        private ToolTipManager _toolTipManager;
        private IconManager _iconManager;
        private ConfigValues _configValues;
        private SettingsManager _settingsManager;
        private XMLGenerator _xmlGenerator;

        private GeneralConfig _generalConfig;
        private LangRegConfig _langRegConfig;

        private Panel _pnlGeneralConfig = null!;
        private Panel _pnlLangRegConfig = null!;

        private bool _hasUnsavedChanges = false;
        private bool _isLoadingConfig = false; // Flag to prevent status updates during config loading

        #endregion

        #region Constructor

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

            // Use GeneralConfig from ConfigValues (not a separate instance!)
            _generalConfig = _configValues.General;

            // Initialize LangRegConfig
            _langRegConfig = new LangRegConfig();

            InitializeComponent();
            InitializeForm();
        }

        #endregion

        #region Initialization

        private void InitializeForm()
        {
            // Attach status label to status manager
            _statusManager.AttachStatusLabel(lblStatus);

            // Set initial status
            _statusManager.SetReady();
            _statusManager.SetStatus(_langManager.GetString("mainForm.status.ready"), StatusType.Success);

            // Load last configuration without triggering unsaved status
            _isLoadingConfig = true;
            try
            {
                // Load last configuration data model
                _configValues.LoadLastConfig();
                
                // Update UI controls from loaded config in each section
                _generalConfig.LoadConfigIntoUI();
                _langRegConfig.LoadConfigIntoUI();
            }
            finally
            {
                // Always reset the flag, even if an error occurs
                _isLoadingConfig = false;
            }

            // Handle section expansion based on setting
            // Sections default to expanded, so we need to collapse them if setting is false
            if (_settingsManager.ExpandAllSectionsAtStartup)
            {
                // Keep them expanded (default state)
                _generalConfig.Expand();
                _langRegConfig.Expand();
            }
            else
            {
                // Collapse sections if setting is false
                _generalConfig.Collapse();
                _langRegConfig.Collapse();
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

        #endregion

        #region Button Event Handlers

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
                _statusManager.SetStatus(_langManager.GetString("mainForm.status.configurationCleared"), StatusType.Success);
            }
        }

        private void BtnPreset_Click(object sender, EventArgs e)
        {
            // TODO: Open PresetsForm
            MessageBox.Show("Presets form not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            // Close the form, which will trigger OnFormClosing with the proper check
            this.Close();
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
                // Save last config if autosave is enabled
                _configValues.SaveLastConfig();

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
                    _langManager.GetString("dialog.failed.message", _langManager.GetString("mainForm.status.failedToGenerateFile")),
                    _langManager.GetString("dialog.failed.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _statusManager.SetError(_langManager.GetString("mainForm.status.failedToGenerateFile"));
            }
        }

        private void BtnGeneralConfigToggle_Click(object sender, EventArgs e)
        {
            _generalConfig.ToggleSection();
        }

        private void BtnExpandAll_Click(object sender, EventArgs e)
        {
            // Expand all sections
            _generalConfig.Expand();
            _langRegConfig.Expand();
        }

        private void BtnCollapseAll_Click(object sender, EventArgs e)
        {
            // Collapse all sections
            _generalConfig.Collapse();
            _langRegConfig.Collapse();
        }

        #endregion

        #region Helper Methods

        private void ClearForm()
        {
            // Clear GeneralConfig using the GeneralConfig class
            _generalConfig.ClearControls();
            
            // Clear LangRegConfig
            _langRegConfig.ClearControls();
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            // Skip status update if we're loading config
            if (_isLoadingConfig)
            {
                return;
            }

            _hasUnsavedChanges = true;
            _statusManager.SetStatus(_langManager.GetString("mainForm.status.unsavedChanges"), StatusType.Warning);

            // Update ConfigValues
            UpdateConfigValues();
        }

        private void UpdateConfigValues()
        {
            // Update GeneralConfig from controls
            _generalConfig.UpdateFromControls();
            
            // Update LangRegConfig from controls
            _langRegConfig.UpdateFromControls();
            
            // No need to sync - _generalConfig IS _configValues.General (same instance)
        }

        private void RepositionSections()
        {
            // Check if panels are initialized
            if (_pnlGeneralConfig == null ||
                _pnlLangRegConfig == null)
                return;

            // Reposition
            _pnlLangRegConfig.Location = new Point(0, _pnlGeneralConfig.Bottom);
        }

        #endregion

        #region Form Events

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

        #endregion
    }
}
