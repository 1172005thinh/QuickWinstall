using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuickWinstall.Sections;
using QuickWinstall.Lib;
using System.Runtime.CompilerServices;

namespace QuickWinstall
{
    public partial class MainForm : Form, ILanguageRefreshable
    {
        // Section controls
        private GeneralConfig generalConfig;
        private LangRegionConfig langRegionConfig;
        private BypassConfig bypassConfig;
        private DiskConfig diskConfig;
        private AccountConfig accountConfig;
        private OOBEConfig oobeConfig;
        private BitLockerConfig bitLockerConfig;
        private PersonalizeConfig personalizeConfig;
        private AppConfig appConfig;

        // State variables
        private bool _isValidating = false;
        private bool _isRefreshingLanguage = false;
        private bool _isInitializing = true;
        private bool _hasUnsavedChanges = false;

        public MainForm()
        {
            InitializaComponent();
            InitializeConfigSections();

            LoadAllSectionSettings();
            UpdateStatusLabel(LangManager.GetString("MainForm_Status_InitializedSuccessfully", "Initialized successfully."));

            LangHelper.RegisterForm(this);
            RefreshLanguage();

            ThemeManager.ApplyThemeToForm(this);

            this.Shown += (s, e) =>
            {
                activityPanel.VerticalScroll.Value = 0;
                activityPanel.HorizontalScroll.Value = 0;
            };

            _isInitializing = false;
        }

        #region Initialization Config Sections
        private void InitializeConfigSections()
        {
            var config = Config.LoadFromAppFolder();
            var defaults = Defaults.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            generalConfig = new GeneralConfig();
            langRegionConfig = new LangRegionConfig();
            bypassConfig = new BypassConfig();
            diskConfig = new DiskConfig();
            accountConfig = new AccountConfig();
            oobeConfig = new OOBEConfig();
            bitLockerConfig = new BitLockerConfig();
            personalizeConfig = new PersonalizeConfig();
            appConfig = new AppConfig();

            generalConfig.ValueChanged += OnConfigValueChanged;
            langRegionConfig.ValueChanged += OnConfigValueChanged;
            bypassConfig.ValueChanged += OnConfigValueChanged;
            diskConfig.ValueChanged += OnConfigValueChanged;
            accountConfig.ValueChanged += OnConfigValueChanged;
            oobeConfig.ValueChanged += OnConfigValueChanged;
            bitLockerConfig.ValueChanged += OnConfigValueChanged;
            personalizeConfig.ValueChanged += OnConfigValueChanged;
            appConfig.ValueChanged += OnConfigValueChanged;

            // Location and order of sections
            appConfig.Dock = DockStyle.Top;
            personalizeConfig.Dock = DockStyle.Top;
            bitLockerConfig.Dock = DockStyle.Top;
            oobeConfig.Dock = DockStyle.Top;
            accountConfig.Dock = DockStyle.Top;
            diskConfig.Dock = DockStyle.Top;
            bypassConfig.Dock = DockStyle.Top;
            langRegionConfig.Dock = DockStyle.Top;
            generalConfig.Dock = DockStyle.Top;

            // Add sections to activity panel
            activityPanel.Controls.Add(appConfig);
            activityPanel.Controls.Add(personalizeConfig);
            activityPanel.Controls.Add(bitLockerConfig);
            activityPanel.Controls.Add(oobeConfig);
            activityPanel.Controls.Add(accountConfig);
            activityPanel.Controls.Add(diskConfig);
            activityPanel.Controls.Add(bypassConfig);
            activityPanel.Controls.Add(langRegionConfig);
            activityPanel.Controls.Add(generalConfig);
        }
        #endregion

        #region OnConfigValueChanged
        private void OnConfigValueChanged(object sender, EventArgs e)
        {
            if (_isValidating || _isRefreshingLanguage) return;
            _hasUnsavedChanges = true;

            // Valiate all sections
            try
            {
                _isValidating = true;
                string validationError = ValidateAllSections();
                if (string.IsNullOrEmpty(validationError))
                {
                    UpdateStatusLabel(validationError);
                }
                else
                {
                    UpdateStatusLabel(LangManager.GetString("MainForm_Status_UnsavedChanges"));
                }
            }
            finally
            {
                _isValidating = false;
            }
        }
        #endregion

        #region ValidateAllSections
        private string ValidateAllSections()
        {
            string error;

            error = generalConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = langRegionConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = bypassConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = diskConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = accountConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = oobeConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = bitLockerConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = personalizeConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            error = appConfig.ValidateConfig();
            if (!string.IsNullOrEmpty(error)) return error;

            return null; // All sections are valid
        }
        #endregion

        #region UpdateStatusLabel
        private void UpdateStatusLabel(string status)
        {
            statusLabel.Text = status;

            // Apply status label color based on content
            if (string.IsNullOrEmpty(status))
            {
                ThemeManager.ApplyThemeToStatusLabel(statusLabel, ThemeManager.StatusLabelType.Normal);
            }
            else if (status.Contains(LangManager.GetString("MainForm_Status_Error"))
                    || status.Contains(LangManager.GetString("MainForm_Status_error"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Invalid"))
                    || status.Contains(LangManager.GetString("MainForm_Status_invalid"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Failed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_failed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Unexpected"))
                    || status.Contains(LangManager.GetString("MainForm_Status_unexpected"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Critical"))
                    || status.Contains(LangManager.GetString("MainForm_Status_critical"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Empty"))
                    || status.Contains(LangManager.GetString("MainForm_Status_empty")))
            {
                ThemeManager.ApplyThemeToStatusLabel(statusLabel, ThemeManager.StatusLabelType.Error);
            }
            else if (status.Contains(LangManager.GetString("MainForm_Status_Warning"))
                    || status.Contains(LangManager.GetString("MainForm_Status_warning"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Unsaved"))
                    || status.Contains(LangManager.GetString("MainForm_Status_unsaved")))
            {
                ThemeManager.ApplyThemeToStatusLabel(statusLabel, ThemeManager.StatusLabelType.Warning);
            }
            else if (status.Contains(LangManager.GetString("MainForm_Status_Success"))
                    || status.Contains(LangManager.GetString("MainForm_Status_success"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Ready"))
                    || status.Contains(LangManager.GetString("MainForm_Status_ready"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Completed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_completed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Saved"))
                    || status.Contains(LangManager.GetString("MainForm_Status_saved"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Applied"))
                    || status.Contains(LangManager.GetString("MainForm_Status_applied"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Cleared")
                    || status.Contains(LangManager.GetString("MainForm_Status_cleared")))
            {
                ThemeManager.ApplyThemeToStatusLabel(statusLabel, ThemeManager.StatusLabelType.Success);
            }
            else
            {
                ThemeManager.ApplyThemeToStatusLabel(statusLabel, ThemeManager.StatusLabelType.Normal);
            }
        }
        #endregion

        #region Settings Button Click
        private void settingsBtn_Click(object sender, EventArgs e)
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            try
            {
                using (var settingsForm = new SettingsForm())
                {
                    var result = settingsForm.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        UpdateStatusLabel(LangManager.GetString("MainForm_Status_SettingsSaved"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToOpenSettings"), ex.Message),
                    LangManager.GetString("MainForm_Status_Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
        }
        #endregion

        #region Clear Button Click
        private void clearBtn_Click(object sender, EventArgs e)
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            try
            {
                var confirmResult = MessageBox.Show(
                    LangManager.GetString("MainForm_Warning_ClearAllSettings", "Clear all settings?"),
                    LangManager.GetString("MainForm_Warning_Title", "Warning"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Warning, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );

                if (confirmResult == DialogResult.OK)
                {
                    _isValidating = true;

                    generalConfig.ResetToDefaults();
                    langRegionConfig.ResetToDefaults();
                    bypassConfig.ResetToDefaults();
                    diskConfig.ResetToDefaults();
                    accountConfig.ResetToDefaults();
                    oobeConfig.ResetToDefaults();
                    bitLockerConfig.ResetToDefaults();
                    personalizeConfig.ResetToDefaults();
                    appConfig.ResetToDefaults();
                    _hasUnsavedChanges = false;
                    UpdateStatusLabel(LangManager.GetString("MainForm_Status_AllSettingsCleared"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(UpdateStatusLabel(LangManager.GetString("MainForm_Error_FailedToClearSettings", "Failed to clear settings: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
        }
        #endregion

        #region Presets Button Click        
        private void presetsBtn_Click(object sender, EventArgs e)
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            try
            {
                using (var presetsForm = new PresetsForm())
                {
                    var result = presetsForm.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        UpdateStatusLabel(LangManager.GetString("MainForm_Status_PresetApplied"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(UpdateStatusLabel(LangManager.GetString("MainForm_Error_FailedToOpenPreset", "Failed to open preset: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
        }
        #endregion

        #region Generate Button Click
        private void generateBtn_Click(object sender, EventArgs e)
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            try
            {
                // Final validation before generating
                string validationError = ValidateAllSections();
                if (!string.IsNullOrEmpty(validationError))
                {
                    UpdateStatusLabel(validationError);
                    MessageBox.Show(
                        string.Format(LangManager.GetString("MainForm_Warning_PleaseFixError", "Please fix error: {0}"), validationError),
                        LangManager.GetString("MainForm_Warning_Title", "Warning"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Warning, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                    );
                    return;
                }

                // Check if save path is set
                var settings = SettingsManager.LoadSettings();
                if (string.IsNullOrEmpty(settings.XmlSavePath))
                {
                    MessageBox.Show(
                        string.Format(LangManager.GetString("MainForm_Error_NoSavePath", "No save path specified in settings: {0}"), validationError),
                        LangManager.GetString("MainForm_Error_Title", "Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                    );
                    return;
                }

                // Check if save path exists
                if (!Directory.Exists(settings.XmlSavePath))
                {
                    MessageBox.Show(
                        string.Format(LangManager.GetString("MainForm_Error_SavePathNotExist", "The specified save path does not exist: {0}"), validationError),
                        LangManager.GetString("MainForm_Error_Title", "Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                    );
                    return;
                }

                // Build full save path
                string savePath = Path.Combine(settings.XmlSavePath, "autounattend.xml");

                // Check if file exists and confirm overwrite
                if (File.Exists(savePath))
                {
                    var result = MessageBox.Show(
                        LangManager.GetString("MainForm_Warning_OverwriteFile", "The file autounattend.xml already exists. Overwrite?"),
                        LangManager.GetString("MainForm_Warning_Title", "Warning"),
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Warning, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                    );

                    if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                // Save current settings
                SaveAllSectionSettings();

                // Proceed with generation
                XMLGenerator.Generate(
                    generalConfig,
                    langRegionConfig,
                    bypassConfig,
                    diskConfig,
                    accountConfig,
                    oobeConfig,
                    bitLockerConfig,
                    personalizeConfig,
                    appConfig,
                    savePath
                );

                _hasUnsavedChanges = false;
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_GenerationCompleted", "Generation completed successfully."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Success_FileSaved", "File saved successfully to: {0}"), savePath),
                    LangManager.GetString("MainForm_Success_Title", "Success"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Success, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(UpdateStatusLabel(LangManager.GetString("MainForm_Error_GenerationFailed", "Generation failed: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
        }
        #endregion

        #region Cancel Button Click
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region OnFormClosing
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    LangManager.GetString("MainForm_Warning_UnsavedChanges", "Leave without saving?"),
                    LangManager.GetString("MainForm_Warning_Title", "Warning"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Warning, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnFormClosing(e);
        }
        #endregion

        #region SaveAllSectionSettings
        private void SaveAllSectionSettings()
        {
            try
            {
                var settings = SettingsManager.LoadSettings();
                settings.GeneralConfig = generalConfig.GetCurrentSettings(),
                settings.LangRegionConfig = langRegionConfig.GetCurrentSettings(),
                settings.BypassConfig = bypassConfig.GetCurrentSettings(),
                settings.DiskConfig = diskConfig.GetCurrentSettings(),
                settings.AccountConfig = accountConfig.GetCurrentSettings(),
                settings.OOBEConfig = oobeConfig.GetCurrentSettings(),
                settings.BitLockerConfig = bitLockerConfig.GetCurrentSettings(),
                settings.PersonalizeConfig = personalizeConfig.GetCurrentSettings(),
                settings.AppConfig = appConfig.GetCurrentSettings()
                SettingsManager.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToSaveSettings", "Failed to save settings: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
        }
        #endregion

        #region LoadAllSectionSettings
        private void LoadAllSectionSettings()
        {
            try
            {
                var settings = SettingsManager.LoadSettings();
                if (settings.GeneralConfig != null)
                    generalConfig.LoadSettings(settings.GeneralConfig);
                if (settings.LangRegionConfig != null)
                    langRegionConfig.LoadSettings(settings.LangRegionConfig);
                if (settings.BypassConfig != null)
                    bypassConfig.LoadSettings(settings.BypassConfig);
                if (settings.DiskConfig != null)
                    diskConfig.LoadSettings(settings.DiskConfig);
                if (settings.AccountConfig != null)
                    accountConfig.LoadSettings(settings.AccountConfig);
                if (settings.OOBEConfig != null)
                    oobeConfig.LoadSettings(settings.OOBEConfig);
                if (settings.BitLockerConfig != null)
                    bitLockerConfig.LoadSettings(settings.BitLockerConfig);
                if (settings.PersonalizeConfig != null)
                    personalizeConfig.LoadSettings(settings.PersonalizeConfig);
                if (settings.AppConfig != null)
                    appConfig.LoadSettings(settings.AppConfig);
                _hasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToLoadLastSettings", "Failed to load last settings: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
        }
        #endregion

        #region ApplyPreset
        private void ApplyPreset(PresetsManager.PresetData preset)
        {
            try
            {
                _isValidating = true;
                var name = preset.Name ?? "/noname";

                if (preset.GeneralConfig != null)
                    generalConfig.LoadSettings(preset.GeneralConfig);
                if (preset.LangRegionConfig != null)
                    langRegionConfig.LoadSettings(preset.LangRegionConfig);
                if (preset.BypassConfig != null)
                    bypassConfig.LoadSettings(preset.BypassConfig);
                if (preset.DiskConfig != null)
                    diskConfig.LoadSettings(preset.DiskConfig);
                if (preset.AccountConfig != null)
                    accountConfig.LoadSettings(preset.AccountConfig);
                if (preset.OOBEConfig != null)
                    oobeConfig.LoadSettings(preset.OOBEConfig);
                if (preset.BitLockerConfig != null)
                    bitLockerConfig.LoadSettings(preset.BitLockerConfig);
                if (preset.PersonalizeConfig != null)
                    personalizeConfig.LoadSettings(preset.PersonalizeConfig);
                if (preset.AppConfig != null)
                    appConfig.LoadSettings(preset.AppConfig);
                _hasUnsavedChanges = true;
                UpdateStatusLabel(string.Format(LangManager.GetString("MainForm_Status_PresetApplied", "Preset {0} applied.")), name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToApplyPreset", "Failed to apply preset: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
            finally
            {
                _isValidating = false;
            }
        }
        #endregion

        #region RefreshLang
        public void RefreshLanguage()
        {
            try
            {
                _isRefreshingLanguage = true;

                this.Text = LangManager.GetString("MainForm_Title", "QuickWinstall");

                if (settingsBtn != null)
                    settingsBtn.Text = LangManager.GetString("MainForm_SettingsButton", "Settings");
                if (clearBtn != null)
                    clearBtn.Text = LangManager.GetString("MainForm_ClearButton", "Clear");
                if (presetsBtn != null)
                    presetsBtn.Text = LangManager.GetString("MainForm_PresetsButton", "Preset");
                if (generateBtn != null)
                    generateBtn.Text = LangManager.GetString("MainForm_GenerateButton", "Generate");
                if (cancelBtn != null)
                    cancelBtn.Text = LangManager.GetString("MainForm_CancelButton", "Cancel");

                if (statusPrefixLabel != null)
                    statusPrefixLabel.Text = LangManager.GetString("MainForm_StatusPrefixLabel", "Status:");

                RefreshSections();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToRefreshLanguage", "Failed to refresh language: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.IconManager.GetIcon(IconManager.Icons.Error, new Size(mainFormConfig.IconSize * 2, mainFormConfig.IconSize * 2))
                );
            }
            finally
            {
                _isRefreshingLanguage = false;
            }
        }
        #endregion

        #region RefreshSections
        private void RefreshSections()
        {
            try
            {
                foreach (Control control in activityPanel.Controls)
                {
                    if (control is ILanguageRefreshable refreshable)
                    {
                        refreshable.RefreshLanguage();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainForm.RefreshSections error: {ex.Message}");
            }
        }
        #endregion
    }
}