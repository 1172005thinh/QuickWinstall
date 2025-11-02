using System;
using System.IO;
using System.Windows.Forms;
using QuickWinstall.Lib;
using QuickWinstall.Main;

namespace QuickWinstall
{
    public partial class SettingsForm : Form
    {
        private readonly SettingsManager _settingsManager;
        private readonly ThemeManager _themeManager;
        private readonly LangManager _langManager;
        private readonly IconManager _iconManager;

        // Store original values to detect changes
        private string _originalTheme;
        private string _originalLanguage;
        private string _originalSavePath;
        private bool _originalSaveLastConfig;
        private bool _originalLoadLastConfig;
        private bool _originalExpandAllSectionsAtStartup;

        public SettingsForm()
        {
            InitializeComponent();

            _settingsManager = SettingsManager.Instance;
            _themeManager = ThemeManager.Instance;
            _langManager = LangManager.Instance;
            _iconManager = IconManager.Instance;

            // Store original values
            _originalTheme = _settingsManager.Theme;
            _originalLanguage = _settingsManager.Language;
            _originalSavePath = _settingsManager.SavePath;
            _originalSaveLastConfig = _settingsManager.SaveLastConfig;
            _originalLoadLastConfig = _settingsManager.LoadLastConfig;
            _originalExpandAllSectionsAtStartup = _settingsManager.ExpandAllSectionsAtStartup;

            LoadCurrentSettings();
            ApplyTheme();
            ApplyLanguage();
        }

        private void LoadCurrentSettings()
        {
            // Language dropdown - dynamically load from res/langs/ folder using LangHelper
            cmbLanguage.Items.Clear();
            
            var availableLanguages = LangHelper.GetAvailableLanguageCodes();
            if (availableLanguages.Count > 0)
            {
                int selectedIndex = 0;
                
                for (int i = 0; i < availableLanguages.Count; i++)
                {
                    string langCode = availableLanguages[i];
                    string displayName = LangHelper.GetLanguageDisplayName(langCode);
                    cmbLanguage.Items.Add(displayName);
                    
                    // Track which index matches current language
                    if (langCode == _settingsManager.Language)
                    {
                        selectedIndex = i;
                    }
                }
                
                cmbLanguage.SelectedIndex = selectedIndex;
            }
            else
            {
                // Fallback to hardcoded languages if folder doesn't exist
                cmbLanguage.Items.Add("English");
                cmbLanguage.Items.Add("Tiếng Việt");
                cmbLanguage.SelectedIndex = _settingsManager.Language == "vi-VN" ? 1 : 0;
            }

            // Theme dropdown - use translated strings
            cmbTheme.Items.Clear();
            cmbTheme.Items.Add(_langManager.GetString("settingsForm.theme.options.light"));
            cmbTheme.Items.Add(_langManager.GetString("settingsForm.theme.options.dark"));
            
            cmbTheme.SelectedIndex = _settingsManager.Theme.Equals("Dark", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            // Save path
            txtSavePath.Text = _settingsManager.SavePath;

            // Auto save last config
            _themeManager.UpdateToggleSwitchState(toggleAutoSave, _settingsManager.SaveLastConfig);

            // Load last saved config
            _themeManager.UpdateToggleSwitchState(toggleLoadLast, _settingsManager.LoadLastConfig);
            
            // Expand all sections on startup
            _themeManager.UpdateToggleSwitchState(toggleExpandAll, _settingsManager.ExpandAllSectionsAtStartup);

            // Lock sections at startup
            _themeManager.UpdateToggleSwitchState(toggleLockSections, _settingsManager.LockSectionsAtStartup);
        }

        private void ApplyTheme()
        {
            // Set form background
            this.BackColor = _themeManager.GetColor("background");

            // Banner panel
            pnlBanner.BackColor = _themeManager.GetColor("background");
            lblBannerTitle.ForeColor = _themeManager.GetFontColor("header");

            // Settings panel
            pnlSettings.BackColor = _themeManager.GetColor("background");

            // Labels
            lblLanguage.ForeColor = _themeManager.GetFontColor("normal");
            lblTheme.ForeColor = _themeManager.GetFontColor("normal");
            lblSavePath.ForeColor = _themeManager.GetFontColor("normal");
            lblAutoSave.ForeColor = _themeManager.GetFontColor("normal");
            lblLoadLast.ForeColor = _themeManager.GetFontColor("normal");
            lblExpandAll.ForeColor = _themeManager.GetFontColor("normal");
            lblLockSections.ForeColor = _themeManager.GetFontColor("normal");

            // Dropdowns
            cmbLanguage.BackColor = _themeManager.GetColor("inputBackground");
            cmbLanguage.ForeColor = _themeManager.GetFontColor("normal");
            cmbTheme.BackColor = _themeManager.GetColor("inputBackground");
            cmbTheme.ForeColor = _themeManager.GetFontColor("normal");

            // TextBox
            txtSavePath.BackColor = _themeManager.GetColor("inputBackground");
            txtSavePath.ForeColor = _themeManager.GetFontColor("normal");

            // Buttons - apply theme to all buttons
            _themeManager.ApplyButtonTheme(btnResetToDefault);
            _themeManager.ApplyButtonTheme(btnBrowse);
            _themeManager.ApplyButtonTheme(btnAbout);
            _themeManager.ApplyButtonTheme(btnHelp);
            _themeManager.ApplyButtonTheme(btnCancel);
            _themeManager.ApplyButtonTheme(btnSave);

            // Control panel
            pnlControls.BackColor = _themeManager.GetColor("background");

            // Icons
            bool useDarkTheme = _settingsManager.Theme == "Dark";
            // I cannot alternate frame color so I decided to keep the icon consistent
            this.Icon = _iconManager.GetIcon("settings", useDarkTheme && false);
            btnResetToDefault.Image = _iconManager.GetIconAsImage("reset", useDarkTheme, UIValues.Instance.GlobalIconSize);
            btnBrowse.Image = _iconManager.GetIconAsImage("browse", useDarkTheme, UIValues.Instance.GlobalIconSize);

            // Toggle switches - update colors based on theme
            _themeManager.UpdateToggleSwitchState(toggleAutoSave, _themeManager.GetToggleSwitchState(toggleAutoSave));
            _themeManager.UpdateToggleSwitchState(toggleLoadLast, _themeManager.GetToggleSwitchState(toggleLoadLast));
            _themeManager.UpdateToggleSwitchState(toggleExpandAll, _themeManager.GetToggleSwitchState(toggleExpandAll));
            _themeManager.UpdateToggleSwitchState(toggleLockSections, _themeManager.GetToggleSwitchState(toggleLockSections));
        }

        private void ApplyLanguage()
        {
            // Form title
            this.Text = _langManager.GetString("settingsForm.title");

            // Banner
            lblBannerTitle.Text = _langManager.GetString("settingsForm.banner.title");

            // Labels
            lblLanguage.Text = _langManager.GetString("settingsForm.language.label");
            lblTheme.Text = _langManager.GetString("settingsForm.theme.label");
            lblSavePath.Text = _langManager.GetString("settingsForm.savePath.label");
            lblAutoSave.Text = _langManager.GetString("settingsForm.autoSave.label");
            lblLoadLast.Text = _langManager.GetString("settingsForm.loadLast.label");
            lblExpandAll.Text = _langManager.GetString("settingsForm.expandAll.label");
            lblLockSections.Text = _langManager.GetString("settingsForm.lockSections.label");

            // Update theme dropdown with translated options
            int currentThemeIndex = cmbTheme.SelectedIndex;
            cmbTheme.Items.Clear();
            cmbTheme.Items.Add(_langManager.GetString("settingsForm.theme.options.light"));
            cmbTheme.Items.Add(_langManager.GetString("settingsForm.theme.options.dark"));
            cmbTheme.SelectedIndex = currentThemeIndex >= 0 ? currentThemeIndex : 0;

            // Buttons
            btnAbout.Text = _langManager.GetString("settingsForm.buttons.about");
            btnHelp.Text = _langManager.GetString("settingsForm.buttons.help");
            btnCancel.Text = _langManager.GetString("settingsForm.buttons.cancel");
            btnSave.Text = _langManager.GetString("settingsForm.buttons.save");
        }

        private void btnResetToDefault_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                this,
                _langManager.GetString("dialogs.resetSettings.message"),
                _langManager.GetString("dialogs.resetSettings.title"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (result == DialogResult.OK)
            {
                // Reset to defaults
                cmbLanguage.SelectedIndex = 0; // English
                cmbTheme.SelectedIndex = 0; // Light
                txtSavePath.Text = AppDomain.CurrentDomain.BaseDirectory;
                _themeManager.UpdateToggleSwitchState(toggleAutoSave, true);
                _themeManager.UpdateToggleSwitchState(toggleLoadLast, true);
                _themeManager.UpdateToggleSwitchState(toggleExpandAll, true);
                _themeManager.UpdateToggleSwitchState(toggleLockSections, false);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowHiddenFiles = true;
                folderDialog.Description = _langManager.GetString("settingsForm.folderDialog.description");
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                folderDialog.SelectedPath = txtSavePath.Text;

                if (folderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    txtSavePath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Handle empty save path - use default application folder
            if (string.IsNullOrWhiteSpace(txtSavePath.Text))
            {
                txtSavePath.Text = AppDomain.CurrentDomain.BaseDirectory;
            }
            
            // Validate save path exists
            if (!Directory.Exists(txtSavePath.Text))
            {
                MessageBox.Show(
                    this,
                    _langManager.GetString("dialogs.invalidSavePath.message"),
                    _langManager.GetString("dialogs.invalidSavePath.title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                txtSavePath.Focus();
                return;
            }

            // Save settings
            _settingsManager.Language = LangHelper.GetLanguageCodeFromIndex(cmbLanguage.SelectedIndex);
            _settingsManager.Theme = cmbTheme.SelectedIndex == 1 ? "Dark" : "Light";
            _settingsManager.SavePath = txtSavePath.Text;
            _settingsManager.SaveLastConfig = _themeManager.GetToggleSwitchState(toggleAutoSave);
            _settingsManager.LoadLastConfig = _themeManager.GetToggleSwitchState(toggleLoadLast);
            _settingsManager.ExpandAllSectionsAtStartup = _themeManager.GetToggleSwitchState(toggleExpandAll);
            _settingsManager.LockSectionsAtStartup = _themeManager.GetToggleSwitchState(toggleLockSections);
            _settingsManager.SaveSettings();

            // Check if theme or language changed
            bool themeChanged = _originalTheme != _settingsManager.Theme;
            bool languageChanged = _originalLanguage != _settingsManager.Language;

            if (themeChanged || languageChanged)
            {
                _settingsManager.ApplySettings();
                
                // Notify parent form to refresh
                if (this.Owner is MainForm mainForm)
                {
                    mainForm.RefreshUI();
                }
            }
            
            // Update status message to indicate settings were saved
            if (this.Owner is MainForm ownerForm)
            {
                StatusManager.Instance.SetStatusFromKey("mainForm.status.settingsUpdated", StatusType.Success);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            // TODO: Open AboutForm when implemented
            MessageBox.Show(
                this,
                "AboutForm not implemented yet",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            // TODO: Open HelpForm when implemented
            MessageBox.Show(
                this,
                "HelpForm not implemented yet",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Handles the toggle for Enable General Configurations
        /// </summary>
        private void OnToggleAutoSave()
        {
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleAutoSave);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleAutoSave, newState);
        }

        private void OnToggleLoadLast()
        {
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleLoadLast);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleLoadLast, newState);
        }

        private void OnToggleExpandAll()
        {
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleExpandAll);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleExpandAll, newState);
        }

        private void OnToggleLockSections()
        {
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleLockSections);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleLockSections, newState);
        }

        public void FocusSavePathTextBox()
        {
            txtSavePath.Focus();
            txtSavePath.SelectAll();
        }
    }
}
