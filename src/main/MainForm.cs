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
        private UserAccConfig _userAccConfig;
        private OOBEConfig _oobeConfig;
        private PersonalConfig _personalConfig;
        private DiskPartConfig _diskPartConfig;
        private BypassConfig _bypassConfig;
        private AppConfig _appConfig;

        private Panel _pnlGeneralConfig = null!;
        private Panel _pnlLangRegConfig = null!;
        private Panel _pnlBypassConfig = null!;
        private Panel _pnlDiskPartConfig = null!;
        private Panel _pnlUserAccConfig = null!;
        private Panel _pnlOOBEConfig = null!;
        private Panel _pnlPersonalConfig = null!;
        private Panel _pnlAppConfig = null!;

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

            _langRegConfig = new LangRegConfig();
            _bypassConfig = new BypassConfig();
            _diskPartConfig = new DiskPartConfig();
            _userAccConfig = new UserAccConfig();
            _oobeConfig = new OOBEConfig();
            _personalConfig = new PersonalConfig();
            _appConfig = new AppConfig();

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
                _bypassConfig.Expand();
                _diskPartConfig.Expand();
                _userAccConfig.Expand();
                _oobeConfig.Expand();
                _personalConfig.Expand();
                _appConfig.Expand();
            }
            else
            {
                // Collapse sections if setting is false
                _generalConfig.Collapse();
                _langRegConfig.Collapse();
                _bypassConfig.Collapse();
                _diskPartConfig.Collapse();
                _userAccConfig.Collapse();
                _oobeConfig.Collapse();
                _personalConfig.Collapse();
                _appConfig.Collapse();
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
            
            // Update banner label colors
            lblBannerTitle.ForeColor = _themeManager.GetFontColor("header");
            
            // Update status strip colors
            lblStatusPrefix.ForeColor = _themeManager.GetFontColor("normal");
            // lblStatus color depends on current status state
        }
        
        private void UpdateLanguageUI()
        {
            // Update banner title
            lblBannerTitle.Text = _langManager.GetString("mainForm.banner.title");
            
            // Update button texts (not icon buttons like btnExpandAll/btnCollapseAll)
            btnSettings.Text = _langManager.GetString("mainForm.buttons.settings");
            btnClear.Text = _langManager.GetString("mainForm.buttons.clear");
            btnPreset.Text = _langManager.GetString("mainForm.buttons.preset");
            btnCancel.Text = _langManager.GetString("mainForm.buttons.cancel");
            btnGenerate.Text = _langManager.GetString("mainForm.buttons.generate");
            
            // Update status strip texts
            lblStatusPrefix.Text = _langManager.GetString("mainForm.status.prefix");
            // Refresh current status with new language
            _statusManager.RefreshStatus();
        }

        public void RefreshUI()
        {
            // Set loading flag to prevent triggering unsaved changes during UI refresh
            _isLoadingConfig = true;
            
            // Scroll to top first to prevent layout issues
            pnlConfigSection.AutoScrollPosition = new Point(0, 0);
            
            try
            {
                // Reapply theme
                ApplyTheme();
                
                // Update language-dependent UI elements
                UpdateLanguageUI();
                
                // Update all buttons with new theme
                _themeManager.ApplyButtonTheme(btnSettings);
                _themeManager.ApplyButtonTheme(btnClear);
                _themeManager.ApplyButtonTheme(btnPreset);
                _themeManager.ApplyButtonTheme(btnCancel);
                _themeManager.ApplyButtonTheme(btnGenerate);
                _themeManager.ApplyButtonTheme(btnExpandAll);
                _themeManager.ApplyButtonTheme(btnCollapseAll);
                
                // Update icon buttons with new theme icons
                bool isDark = _themeManager.IsDarkTheme;
                btnExpandAll.Image = _iconManager.GetIconAsImage("add", isDark, _uiValues.GlobalIconSize);
                btnCollapseAll.Image = _iconManager.GetIconAsImage("remove", isDark, _uiValues.GlobalIconSize);
                
                // Refresh all sections - recreate their UI with new theme
                if (_generalConfig != null)
                {
                    // Save current control values to model BEFORE disposing controls
                    _generalConfig.UpdateFromControls();
                    
                    // Clear and reinitialize the section with new theme
                    _pnlGeneralConfig?.Dispose();
                    _pnlGeneralConfig = _generalConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections,
                        this
                    );
                    pnlConfigSection.Controls.Add(_pnlGeneralConfig);
                    
                    // Reload data from model into the new UI controls
                    _generalConfig.LoadConfigIntoUI();
                    
                    // Re-validate UI fields to restore validation status rings with new theme
                    _generalConfig.ValidateAllUIFields();
                }
                
                if (_langRegConfig != null)
                {
                    // Save current control values to model BEFORE disposing controls
                    _langRegConfig.UpdateFromControls();
                    
                    _pnlLangRegConfig?.Dispose();
                    _pnlLangRegConfig = _langRegConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlLangRegConfig.Location = new Point(0, _pnlGeneralConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlLangRegConfig);
                    
                    // Reload data from model into the new UI controls
                    _langRegConfig.LoadConfigIntoUI();
                }
                
                if (_userAccConfig != null)
                {
                    _userAccConfig.UpdateFromControls();
                    _pnlUserAccConfig?.Dispose();
                    _pnlUserAccConfig = _userAccConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlUserAccConfig.Location = new Point(0, _pnlLangRegConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlUserAccConfig);
                    _userAccConfig.LoadConfigIntoUI();
                }
                
                if (_oobeConfig != null)
                {
                    _oobeConfig.UpdateFromControls();
                    _pnlOOBEConfig?.Dispose();
                    _pnlOOBEConfig = _oobeConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlOOBEConfig.Location = new Point(0, _pnlUserAccConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlOOBEConfig);
                    _oobeConfig.LoadConfigIntoUI();
                }
                
                if (_personalConfig != null)
                {
                    _personalConfig.UpdateFromControls();
                    _pnlPersonalConfig?.Dispose();
                    _pnlPersonalConfig = _personalConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlPersonalConfig.Location = new Point(0, _pnlOOBEConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlPersonalConfig);
                    _personalConfig.LoadConfigIntoUI();
                }
                
                if (_diskPartConfig != null)
                {
                    _diskPartConfig.UpdateFromControls();
                    _pnlDiskPartConfig?.Dispose();
                    _pnlDiskPartConfig = _diskPartConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlDiskPartConfig.Location = new Point(0, _pnlPersonalConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlDiskPartConfig);
                    _diskPartConfig.LoadConfigIntoUI();
                }
                
                if (_bypassConfig != null)
                {
                    _bypassConfig.UpdateFromControls();
                    _pnlBypassConfig?.Dispose();
                    _pnlBypassConfig = _bypassConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlBypassConfig.Location = new Point(0, _pnlDiskPartConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlBypassConfig);
                    _bypassConfig.LoadConfigIntoUI();
                }
                
                if (_appConfig != null)
                {
                    _appConfig.UpdateFromControls();
                    _pnlAppConfig?.Dispose();
                    _pnlAppConfig = _appConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        RepositionSections
                    );
                    _pnlAppConfig.Location = new Point(0, _pnlBypassConfig.Bottom);
                    pnlConfigSection.Controls.Add(_pnlAppConfig);
                    _appConfig.LoadConfigIntoUI();
                }
            }
            finally
            {
                // Always reset the loading flag
                _isLoadingConfig = false;
            }
        }

        #endregion

        #region Button Event Handlers

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                settingsForm.StartPosition = FormStartPosition.CenterParent;
                settingsForm.ShowDialog(this);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                this,
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
            MessageBox.Show(this, "Presets form not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    this,
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
                DialogResult result = MessageBox.Show(
                    this,
                    _langManager.GetString("dialogs.emptySavePath.message"),
                    _langManager.GetString("dialogs.emptySavePath.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _statusManager.SetError(_langManager.GetString("mainForm.status.invalidSavePath"));
                
                // Open SettingsForm and focus on save path textbox
                using (SettingsForm settingsForm = new SettingsForm())
                {
                    settingsForm.StartPosition = FormStartPosition.CenterParent;
                    settingsForm.ShowDialog(this);
                    settingsForm.FocusSavePathTextBox();
                }
                return;
            }

            string outputPath = System.IO.Path.Combine(savePath, "autounattend.xml");

            // Check if file exists
            if (System.IO.File.Exists(outputPath))
            {
                var result = MessageBox.Show(
                    this,
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
                    this,
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
                    this,
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
            // Scroll to top first for consistent experience
            pnlConfigSection.AutoScrollPosition = new Point(0, 0);
            
            // Expand all sections
            _generalConfig.Expand();
            _langRegConfig.Expand();
            _userAccConfig.Expand();
            _oobeConfig.Expand();
            _personalConfig.Expand();
            _diskPartConfig.Expand();
            _bypassConfig.Expand();
            _appConfig.Expand();
        }

        private void BtnCollapseAll_Click(object sender, EventArgs e)
        {
            // Scroll to top first for consistent experience
            pnlConfigSection.AutoScrollPosition = new Point(0, 0);
            
            // Collapse all sections
            _generalConfig.Collapse();
            _langRegConfig.Collapse();
            _userAccConfig.Collapse();
            _oobeConfig.Collapse();
            _personalConfig.Collapse();
            _diskPartConfig.Collapse();
            _bypassConfig.Collapse();
            _appConfig.Collapse();
        }

        #endregion

        #region Helper Methods

        private void ClearForm()
        {
            // Clear GeneralConfig using the GeneralConfig class
            _generalConfig.ClearControls();
            
            // Clear LangRegConfig
            _langRegConfig.ClearControls();
            
            // Clear all other sections
            _userAccConfig.ClearControls();
            _oobeConfig.ClearControls();
            _personalConfig.ClearControls();
            _diskPartConfig.ClearControls();
            _bypassConfig.ClearControls();
            _appConfig.ClearControls();
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
            if (_pnlGeneralConfig == null)
                return;

            // CRITICAL: Suspend layout to prevent flicker and scroll issues
            pnlConfigSection.SuspendLayout();
            
            // Always scroll to top first to prevent layout issues with AutoScrollPosition
            pnlConfigSection.AutoScrollPosition = new Point(0, 0);

            // Reposition all sections in order
            int currentY = 0;
            
            if (_pnlGeneralConfig != null)
            {
                _pnlGeneralConfig.Location = new Point(0, currentY);
                currentY = _pnlGeneralConfig.Bottom;
            }
            
            if (_pnlLangRegConfig != null)
            {
                _pnlLangRegConfig.Location = new Point(0, currentY);
                currentY = _pnlLangRegConfig.Bottom;
            }
            
            if (_pnlUserAccConfig != null)
            {
                _pnlUserAccConfig.Location = new Point(0, currentY);
                currentY = _pnlUserAccConfig.Bottom;
            }
            
            if (_pnlOOBEConfig != null)
            {
                _pnlOOBEConfig.Location = new Point(0, currentY);
                currentY = _pnlOOBEConfig.Bottom;
            }
            
            if (_pnlPersonalConfig != null)
            {
                _pnlPersonalConfig.Location = new Point(0, currentY);
                currentY = _pnlPersonalConfig.Bottom;
            }
            
            if (_pnlDiskPartConfig != null)
            {
                _pnlDiskPartConfig.Location = new Point(0, currentY);
                currentY = _pnlDiskPartConfig.Bottom;
            }
            
            if (_pnlBypassConfig != null)
            {
                _pnlBypassConfig.Location = new Point(0, currentY);
                currentY = _pnlBypassConfig.Bottom;
            }
            
            if (_pnlAppConfig != null)
            {
                _pnlAppConfig.Location = new Point(0, currentY);
                currentY = _pnlAppConfig.Bottom;
            }

            // Resume layout
            pnlConfigSection.ResumeLayout(true);
        }

        #endregion

        #region Form Events

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    this,
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
