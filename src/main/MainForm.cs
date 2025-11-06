using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;
using QuickWinstall.Config;

namespace QuickWinstall.Main
{
    /// <summary>
    /// Installation mode for the application
    /// </summary>
    public enum InstallationMode
    {
        NewInstallation,
        UpgradeOnly
    }

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
        private bool _allSectionsExpanded = true; // Track expand/collapse all state
        private bool _allSectionsLocked = false; // Track lock/unlock all state
        private InstallationMode _currentMode = InstallationMode.NewInstallation; // Track current installation mode

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

            _generalConfig = _configValues.General;
            _langRegConfig = _configValues.LangReg;
            _bypassConfig = _configValues.Bypass;
            _oobeConfig = _configValues.OOBE;
            _diskPartConfig = _configValues.DiskPart;

            _userAccConfig = new UserAccConfig();
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
                _bypassConfig.LoadConfigIntoUI();
                _diskPartConfig.LoadConfigIntoUI();
                _userAccConfig.LoadConfigIntoUI();
                _oobeConfig.LoadConfigIntoUI();
                _personalConfig.LoadConfigIntoUI();
                _appConfig.LoadConfigIntoUI();
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
                _allSectionsExpanded = true;
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
                _allSectionsExpanded = false;

                // Update button icon to show expand state
                btnToggleAll.Image = _iconManager.GetIconAsImage("all_expand", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleAll, "tooltips.mainForm.expandAll");
            }
            
            // Handle section lock state based on setting
            if (_settingsManager.LockSectionsAtStartup)
            {
                _generalConfig.SetEnableState(false);
                _langRegConfig.SetEnableState(false);
                _bypassConfig.SetEnableState(false);
                _diskPartConfig.SetEnableState(false);
                //_userAccConfig.SetEnableState(false);
                _oobeConfig.SetEnableState(false);
                //_personalConfig.SetEnableState(false);
                //_appConfig.SetEnableState(false);

                _allSectionsLocked = true;
                btnToggleLock.Image = _iconManager.GetIconAsImage("unlock", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleLock, "tooltips.mainForm.unlockAll");
            }
            else
            {
                _generalConfig.SetEnableState(true);
                _langRegConfig.SetEnableState(true);
                _bypassConfig.SetEnableState(true);
                _diskPartConfig.SetEnableState(true);
                //_userAccConfig.SetEnableState(true);
                _oobeConfig.SetEnableState(true);
                //_personalConfig.SetEnableState(true);
                //_appConfig.SetEnableState(true);

                _allSectionsLocked = false;
                btnToggleLock.Image = _iconManager.GetIconAsImage("lock", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleLock, "tooltips.mainForm.lockAll");
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
            lblBannerTitle.Font = _themeManager.GetFont("header");
            lblBannerTitle.ForeColor = _themeManager.GetFontColor("header");
            
            // Update status strip colors
            lblStatusPrefix.Font = _themeManager.GetFont("normal");
            lblStatusPrefix.ForeColor = _themeManager.GetFontColor("normal");
            // lblStatus color depends on current status state
        }
        
        private void UpdateLanguageUI()
        {
            // Update banner title
            lblBannerTitle.Text = _langManager.GetString("mainForm.banner.title");
            _toolTipManager.SetToolTip(picLogo, "tooltips.mainForm.logoClick");
            
            
            // Update button texts (not icon buttons like btnExpandAll/btnCollapseAll)
            btnSettings.Text = _langManager.GetString("mainForm.buttons.settings");
            btnClear.Text = _langManager.GetString("mainForm.buttons.clear");
            btnPreset.Text = _langManager.GetString("mainForm.buttons.preset");
            btnCancel.Text = _langManager.GetString("mainForm.buttons.cancel");
            btnGenerate.Text = _langManager.GetString("mainForm.buttons.generate");
            
            // Update mode toggle button text and tooltip based on current mode
            if (_currentMode == InstallationMode.NewInstallation)
            {
                btnModeToggle.Text = _langManager.GetString("mainForm.buttons.newInstallation");
                _toolTipManager.SetToolTip(btnModeToggle, "tooltips.mainForm.newInstallation");
            }
            else
            {
                btnModeToggle.Text = _langManager.GetString("mainForm.buttons.upgradeOnly");
                _toolTipManager.SetToolTip(btnModeToggle, "tooltips.mainForm.upgradeOnly");
            }
            
            // Update icon button tooltips based on current state
            string toggleAllTooltip = _allSectionsExpanded ? "tooltips.mainForm.collapseAll" : "tooltips.mainForm.expandAll";
            _toolTipManager.SetToolTip(btnToggleAll, toggleAllTooltip);
            
            string toggleLockTooltip = _allSectionsLocked ? "tooltips.mainForm.unlockAll" : "tooltips.mainForm.lockAll";
            _toolTipManager.SetToolTip(btnToggleLock, toggleLockTooltip);
            
            // Update status strip texts
            lblStatusPrefix.Text = _langManager.GetString("mainForm.status.prefix");
            // Refresh current status with new language
            _statusManager.RefreshStatus();
        }

        public void RefreshUI()
        {
            // Set loading flag to prevent triggering unsaved changes during UI refresh
            _isLoadingConfig = true;
            
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
                _themeManager.ApplyButtonTheme(btnToggleAll);
                _themeManager.ApplyButtonTheme(btnToggleLock);
                _themeManager.ApplyButtonTheme(btnModeToggle);
                
                // Update icon buttons with new theme icons
                bool isDark = _themeManager.IsDarkTheme;
                string iconName = _allSectionsExpanded ? "all_collapse" : "all_expand";
                btnToggleAll.Image = _iconManager.GetIconAsImage(iconName, isDark, _uiValues.GlobalIconSize);
                
                string lockIconName = _allSectionsLocked ? "unlock" : "lock";
                btnToggleLock.Image = _iconManager.GetIconAsImage(lockIconName, isDark, _uiValues.GlobalIconSize);

                // Refresh all sections in REVERSE order (bottom to top) with DockStyle.Top
                // Save all values first
                _generalConfig?.UpdateFromControls();
                _langRegConfig?.UpdateFromControls();
                _bypassConfig?.UpdateFromControls();
                _diskPartConfig?.UpdateFromControls();
                _userAccConfig?.UpdateFromControls();
                _oobeConfig?.UpdateFromControls();
                _personalConfig?.UpdateFromControls();
                _appConfig?.UpdateFromControls();
                
                // Dispose all panels
                _pnlGeneralConfig?.Dispose();
                _pnlLangRegConfig?.Dispose();
                _pnlBypassConfig?.Dispose();
                _pnlDiskPartConfig?.Dispose();
                _pnlUserAccConfig?.Dispose();
                _pnlOOBEConfig?.Dispose();
                _pnlPersonalConfig?.Dispose();
                _pnlAppConfig?.Dispose();
                
                // Recreate in reverse order (App first, General last)
                if (_appConfig != null)
                {
                    _pnlAppConfig = _appConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        null
                    );
                    _pnlAppConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlAppConfig);
                    _appConfig.LoadConfigIntoUI();
                }
                
                if (_personalConfig != null)
                {
                    _pnlPersonalConfig = _personalConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        null
                    );
                    _pnlPersonalConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlPersonalConfig);
                    _personalConfig.LoadConfigIntoUI();
                }
                
                if (_oobeConfig != null)
                {
                    _pnlOOBEConfig = _oobeConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        CheckAndUpdateExpandCollapseButton,
                        CheckAndUpdateLockUnlockButton
                    );
                    _pnlOOBEConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlOOBEConfig);
                    _oobeConfig.LoadConfigIntoUI();
                    _oobeConfig.ValidateAllUIFields();
                }
                
                if (_userAccConfig != null)
                {
                    _pnlUserAccConfig = _userAccConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        null
                    );
                    _pnlUserAccConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlUserAccConfig);
                    _userAccConfig.LoadConfigIntoUI();
                }
                
                if (_diskPartConfig != null)
                {
                    _pnlDiskPartConfig = _diskPartConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        CheckAndUpdateExpandCollapseButton,
                        CheckAndUpdateLockUnlockButton
                    );
                    _pnlDiskPartConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlDiskPartConfig);
                    _diskPartConfig.LoadConfigIntoUI();
                }
                
                if (_bypassConfig != null)
                {
                    _pnlBypassConfig = _bypassConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        CheckAndUpdateExpandCollapseButton,
                        CheckAndUpdateLockUnlockButton
                    );
                    _pnlBypassConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlBypassConfig);
                    _bypassConfig.LoadConfigIntoUI();
                }
                
                if (_langRegConfig != null)
                {
                    _pnlLangRegConfig = _langRegConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        CheckAndUpdateExpandCollapseButton,
                        CheckAndUpdateLockUnlockButton
                    );
                    _pnlLangRegConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlLangRegConfig);
                    _langRegConfig.LoadConfigIntoUI();
                    _langRegConfig.ValidateAllUIFields();
                }
                
                if (_generalConfig != null)
                {
                    _pnlGeneralConfig = _generalConfig.InitializeUI(
                        pnlConfigSection,
                        (sender, e) => OnConfigChanged(sender!, e),
                        _themeManager.CreateRoundedButton,
                        CheckAndUpdateExpandCollapseButton,
                        CheckAndUpdateLockUnlockButton,
                        this
                    );
                    _pnlGeneralConfig.Dock = DockStyle.Top;
                    pnlConfigSection.Controls.Add(_pnlGeneralConfig);
                    _generalConfig.LoadConfigIntoUI();
                    _generalConfig.ValidateAllUIFields();
                }
                
                // Apply panel visibility based on current installation mode
                ApplyModeVisibility();
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
            // Update config values from UI controls before validation
            try
            {
                _generalConfig.UpdateFromControls();
                _langRegConfig.UpdateFromControls();
                _bypassConfig.UpdateFromControls();
                _diskPartConfig.UpdateFromControls();
                _userAccConfig.UpdateFromControls();
                _oobeConfig.UpdateFromControls();
                _personalConfig.UpdateFromControls();
                _appConfig.UpdateFromControls();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to update config from controls: {ex.Message}");
            }

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
            
            // Select template based on installation mode
            string templatePath = _currentMode == InstallationMode.UpgradeOnly 
                ? System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "lib", "template_upgrade.xml")
                : System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "lib", "template.xml");
            
            bool success = _xmlGenerator.GenerateXML(outputPath, _configValues.GetAllValues(), templatePath);

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
                    _langManager.GetString("dialogs.failed.message", _langManager.GetString("mainForm.status.failedToGenerateFile")),
                    _langManager.GetString("dialogs.failed.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _statusManager.SetError(_langManager.GetString("mainForm.status.failedToGenerateFile"));
            }
        }
        
        private void picLogo_Click(object sender, EventArgs e)
        {
            // Scroll to top of the ConfigSection panel
            pnlConfigSection.AutoScrollPosition = new Point(0, 0);
        }

        private void BtnToggleAll_Click(object sender, EventArgs e)
        {
            if (_allSectionsExpanded)
            {
                // Collapse all sections
                _generalConfig.Collapse();
                _langRegConfig.Collapse();
                _userAccConfig.Collapse();
                _oobeConfig.Collapse();
                _personalConfig.Collapse();
                _diskPartConfig.Collapse();
                _bypassConfig.Collapse();
                _appConfig.Collapse();
                
                _allSectionsExpanded = false;
                btnToggleAll.Image = _iconManager.GetIconAsImage("all_expand", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleAll, "tooltips.mainForm.expandAll");
            }
            else
            {
                // Expand all sections
                _generalConfig.Expand();
                _langRegConfig.Expand();
                _userAccConfig.Expand();
                _oobeConfig.Expand();
                _personalConfig.Expand();
                _diskPartConfig.Expand();
                _bypassConfig.Expand();
                _appConfig.Expand();
                
                _allSectionsExpanded = true;
                btnToggleAll.Image = _iconManager.GetIconAsImage("all_collapse", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleAll, "tooltips.mainForm.collapseAll");
            }
        }

        private void BtnToggleLock_Click(object sender, EventArgs e)
        {
            if (_allSectionsLocked)
            {
                // Unlock all sections (set Enable toggles to true)
                _generalConfig.SetEnableState(true);
                _langRegConfig.SetEnableState(true);
                _bypassConfig.SetEnableState(true);
                _diskPartConfig.SetEnableState(true);
                _oobeConfig.SetEnableState(true);
                
                _allSectionsLocked = false;
                btnToggleLock.Image = _iconManager.GetIconAsImage("lock", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleLock, "tooltips.mainForm.lockAll");
            }
            else
            {
                // Lock all sections (set Enable toggles to false)
                _generalConfig.SetEnableState(false);
                _langRegConfig.SetEnableState(false);
                _bypassConfig.SetEnableState(false);
                _diskPartConfig.SetEnableState(false);
                _oobeConfig.SetEnableState(false);
                
                _allSectionsLocked = true;
                btnToggleLock.Image = _iconManager.GetIconAsImage("unlock", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleLock, "tooltips.mainForm.unlockAll");
            }
        }

        private void BtnModeToggle_Click(object sender, EventArgs e)
        {
            // Toggle between NewInstallation and UpgradeOnly modes
            if (_currentMode == InstallationMode.NewInstallation)
            {
                // Switch to Upgrade Only mode
                _currentMode = InstallationMode.UpgradeOnly;
                btnModeToggle.Text = _langManager.GetString("mainForm.buttons.upgradeOnly");
                _toolTipManager.SetToolTip(btnModeToggle, "tooltips.mainForm.upgradeOnly");
            }
            else
            {
                // Switch to New Installation mode
                _currentMode = InstallationMode.NewInstallation;
                btnModeToggle.Text = _langManager.GetString("mainForm.buttons.newInstallation");
                _toolTipManager.SetToolTip(btnModeToggle, "tooltips.mainForm.newInstallation");
            }
            
            // Apply the visibility changes
            ApplyModeVisibility();
        }

        /// <summary>
        /// Applies panel visibility based on the current installation mode
        /// </summary>
        private void ApplyModeVisibility()
        {
            if (_currentMode == InstallationMode.UpgradeOnly)
            {
                // Upgrade Only mode - only show Bypass and OOBE panels
                _pnlGeneralConfig.Visible = false;
                _pnlLangRegConfig.Visible = false;
                _pnlUserAccConfig.Visible = false;
                _pnlPersonalConfig.Visible = false;
                _pnlDiskPartConfig.Visible = false;
                _pnlAppConfig.Visible = false;
                _pnlBypassConfig.Visible = true;
                _pnlOOBEConfig.Visible = true;
            }
            else
            {
                // New Installation mode - show all panels
                _pnlGeneralConfig.Visible = true;
                _pnlLangRegConfig.Visible = true;
                _pnlUserAccConfig.Visible = true;
                _pnlPersonalConfig.Visible = true;
                _pnlDiskPartConfig.Visible = true;
                _pnlAppConfig.Visible = true;
                _pnlBypassConfig.Visible = true;
                _pnlOOBEConfig.Visible = true;
            }
        }

        /// <summary>
        /// Checks if all sections are in the same expand/collapse state and updates the button accordingly
        /// </summary>
        public void CheckAndUpdateExpandCollapseButton()
        {
            // Check if all sections have the same expanded state
            bool allExpanded = _generalConfig.IsExpanded &&
                                _langRegConfig.IsExpanded &&
                                _userAccConfig.IsExpanded &&
                                _oobeConfig.IsExpanded &&
                                _personalConfig.IsExpanded &&
                                _diskPartConfig.IsExpanded &&
                                _bypassConfig.IsExpanded &&
                                _appConfig.IsExpanded;
                              
            bool allCollapsed = !_generalConfig.IsExpanded &&
                                !_langRegConfig.IsExpanded &&
                                !_userAccConfig.IsExpanded &&
                                !_oobeConfig.IsExpanded &&
                                !_personalConfig.IsExpanded &&
                                !_diskPartConfig.IsExpanded &&
                                !_bypassConfig.IsExpanded &&
                                !_appConfig.IsExpanded;
            
            if (allExpanded && !_allSectionsExpanded)
            {
                _allSectionsExpanded = true;
                btnToggleAll.Image = _iconManager.GetIconAsImage("all_collapse", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleAll, "tooltips.mainForm.collapseAll");
            }
            else if (allCollapsed && _allSectionsExpanded)
            {
                _allSectionsExpanded = false;
                btnToggleAll.Image = _iconManager.GetIconAsImage("all_expand", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleAll, "tooltips.mainForm.expandAll");
            }
        }

        /// <summary>
        /// Checks if all sections with Enable toggles are in the same lock/unlock state and updates the button accordingly
        /// </summary>
        public void CheckAndUpdateLockUnlockButton()
        {
            // Check if all sections with Enable toggles have the same state
            bool allUnlocked = _generalConfig.EnableGeneral &&
                                _langRegConfig.EnableLangReg &&
                                _bypassConfig.EnableBypass &&
                                _diskPartConfig.EnableDiskPart &&
                                _oobeConfig.EnableOOBE;
            bool allLocked = !_generalConfig.EnableGeneral &&
                                !_langRegConfig.EnableLangReg &&
                                !_bypassConfig.EnableBypass &&
                                !_diskPartConfig.EnableDiskPart &&
                                !_oobeConfig.EnableOOBE;

            if (allUnlocked && _allSectionsLocked)
            {
                _allSectionsLocked = false;
                btnToggleLock.Image = _iconManager.GetIconAsImage("lock", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleLock, "tooltips.mainForm.lockAll");
            }
            else if (allLocked && !_allSectionsLocked)
            {
                _allSectionsLocked = true;
                btnToggleLock.Image = _iconManager.GetIconAsImage("unlock", _themeManager.IsDarkTheme, _uiValues.GlobalIconSize);
                _toolTipManager.SetToolTip(btnToggleLock, "tooltips.mainForm.unlockAll");
            }
        }

        #endregion

        #region Helper Methods

        private void ClearForm()
        {
            // Only clear sections that are not locked (enabled)
            if (_generalConfig.EnableGeneral)
                _generalConfig.ClearControls();
            
            if (_langRegConfig.EnableLangReg)
                _langRegConfig.ClearControls();

            if (_bypassConfig.EnableBypass)
                _bypassConfig.ClearControls();
            
            if (_oobeConfig.EnableOOBE)
                _oobeConfig.ClearControls();

            if (_diskPartConfig.EnableDiskPart)
                _diskPartConfig.ClearControls();

            // These sections don't have Enable toggles, so always clear them
            _userAccConfig.ClearControls();
            _personalConfig.ClearControls();
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
            _generalConfig.UpdateFromControls();
            _langRegConfig.UpdateFromControls();
            _bypassConfig.UpdateFromControls();
            _diskPartConfig.UpdateFromControls();
            _oobeConfig.UpdateFromControls();
        }

        // NOTE: RepositionSections() is no longer needed with DockStyle.Top
        // Sections automatically stack and reposition when they expand/collapse

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
