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
    public partial class MainForm : Form, ILangRefreshable
    {
        // Section controls
        private GeneralConfig generalConfig;
        //private LangRegionConfig langRegionConfig;
        //private BypassConfig bypassConfig;
        //private DiskConfig diskConfig;
        //private AccountConfig accountConfig;
        //private OOBEConfig oobeConfig;
        //private BitLockerConfig bitLockerConfig;
        //private PersonalizeConfig personalizeConfig;
        //private AppConfig appConfig;

        // State variables
        private bool _isValidating = false;
        private bool _isRefreshingLanguage = false;
        private bool _isInitializing = true;
        private bool _hasUnsavedChanges = false;

        #region MainForm
        public MainForm()
        {
            InitializeComponent();
            InitializeSections();

            LoadAllSectionConfigs();
            UpdateStatusLabel(LangManager.GetString("MainForm_Status_InitializedSuccessfully", "Initialized successfully."));

            LangHelper.RegisterForm(this);
            RefreshLang();

            ThemeManager.SetForm(this);

            this.Shown += (s, e) =>
            {
                activityPanel.VerticalScroll.Value = 0;
                activityPanel.HorizontalScroll.Value = 0;
            };

            _isInitializing = false;
        }

        #region InitializationSections
        private void InitializeSections()
        {
            var config = Config.LoadFromAppFolder();
            var defaults = Defaults.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            generalConfig = new GeneralConfig();
            //langRegionConfig = new LangRegionConfig();
            //bypassConfig = new BypassConfig();
            //diskConfig = new DiskConfig();
            //accountConfig = new AccountConfig();
            //oobeConfig = new OOBEConfig();
            //bitLockerConfig = new BitLockerConfig();
            //personalizeConfig = new PersonalizeConfig();
            //appConfig = new AppConfig();

            generalConfig.ValueChanged += OnConfigValueChanged;
            //langRegionConfig.ValueChanged += OnConfigValueChanged;
            //bypassConfig.ValueChanged += OnConfigValueChanged;
            //diskConfig.ValueChanged += OnConfigValueChanged;
            //accountConfig.ValueChanged += OnConfigValueChanged;
            //oobeConfig.ValueChanged += OnConfigValueChanged;
            //bitLockerConfig.ValueChanged += OnConfigValueChanged;
            //personalizeConfig.ValueChanged += OnConfigValueChanged;
            //appConfig.ValueChanged += OnConfigValueChanged;

            // Location and order of sections
            //appConfig.Dock = DockStyle.Top;
            //personalizeConfig.Dock = DockStyle.Top;
            //bitLockerConfig.Dock = DockStyle.Top;
            //oobeConfig.Dock = DockStyle.Top;
            //accountConfig.Dock = DockStyle.Top;
            //diskConfig.Dock = DockStyle.Top;
            //bypassConfig.Dock = DockStyle.Top;
            //langRegionConfig.Dock = DockStyle.Top;
            generalConfig.Dock = DockStyle.Top;

            // Add sections to activity panel
            //activityPanel.Controls.Add(appConfig);
            //activityPanel.Controls.Add(personalizeConfig);
            //activityPanel.Controls.Add(bitLockerConfig);
            //activityPanel.Controls.Add(oobeConfig);
            //activityPanel.Controls.Add(accountConfig);
            //activityPanel.Controls.Add(diskConfig);
            //activityPanel.Controls.Add(bypassConfig);
            //activityPanel.Controls.Add(langRegionConfig);
            activityPanel.Controls.Add(generalConfig);
        }
        #endregion

        #region OnConfigValueChanged
        private void OnConfigValueChanged(object sender, EventArgs e)
        {
            if (_isValidating || _isRefreshingLanguage || _isInitializing) return;
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
                    UpdateStatusLabel(LangManager.GetString("MainForm_Status_UnsavedChanges", "Has unsaved changes."));
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

            //error = langRegionConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = bypassConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = diskConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = accountConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = oobeConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = bitLockerConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = personalizeConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

            //error = appConfig.ValidateConfig();
            //if (!string.IsNullOrEmpty(error)) return error;

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
                ThemeManager.SetStatusLabelStyle(statusLabel, ThemeManager.Type.Normal);
            }
            else if (status.Contains(LangManager.GetString("MainForm_Status_Error", "Error"))
                    || status.Contains(LangManager.GetString("MainForm_Status_error", "error"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Invalid", "Invalid"))
                    || status.Contains(LangManager.GetString("MainForm_Status_invalid", "invalid"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Failed", "Failed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_failed", "failed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Unexpected", "Unexpected"))
                    || status.Contains(LangManager.GetString("MainForm_Status_unexpected", "unexpected"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Critical", "Critical"))
                    || status.Contains(LangManager.GetString("MainForm_Status_critical", "critical"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Empty", "Empty"))
                    || status.Contains(LangManager.GetString("MainForm_Status_empty", "empty")))
            {
                ThemeManager.SetStatusLabelStyle(statusLabel, ThemeManager.Type.Error);
            }
            else if (status.Contains(LangManager.GetString("MainForm_Status_Warning", "Warning"))
                    || status.Contains(LangManager.GetString("MainForm_Status_warning", "warning"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Unsaved", "Unsaved"))
                    || status.Contains(LangManager.GetString("MainForm_Status_unsaved", "unsaved")))
            {
                ThemeManager.SetStatusLabelStyle(statusLabel, ThemeManager.Type.Warning);
            }
            else if (status.Contains(LangManager.GetString("MainForm_Status_Success", "Success"))
                    || status.Contains(LangManager.GetString("MainForm_Status_success", "success"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Ready", "Ready"))
                    || status.Contains(LangManager.GetString("MainForm_Status_ready", "ready"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Completed", "Completed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_completed", "completed"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Saved", "Saved"))
                    || status.Contains(LangManager.GetString("MainForm_Status_saved", "saved"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Applied", "Applied"))
                    || status.Contains(LangManager.GetString("MainForm_Status_applied", "applied"))
                    || status.Contains(LangManager.GetString("MainForm_Status_Cleared", "Cleared"))
                    || status.Contains(LangManager.GetString("MainForm_Status_cleared", "cleared")))
            {
                ThemeManager.SetStatusLabelStyle(statusLabel, ThemeManager.Type.Success);
            }
            else
            {
                ThemeManager.SetStatusLabelStyle(statusLabel, ThemeManager.Type.Normal);
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
                        UpdateStatusLabel(LangManager.GetString("MainForm_Status_SettingsSaved", "Settings saved."));
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToOpenSettings", "Failed to open settings."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToOpenSettings", "Failed to open settings: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
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
                    LangManager.GetString("MainForm_Warning_ClearAllConfigs", "Clear all configurations?"),
                    LangManager.GetString("MainForm_Warning_Title", "Warning"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                );

                if (confirmResult == DialogResult.OK)
                {
                    _isValidating = true;

                    generalConfig.ResetToDefaults();
                    //langRegionConfig.ResetToDefaults();
                    //bypassConfig.ResetToDefaults();
                    //diskConfig.ResetToDefaults();
                    //accountConfig.ResetToDefaults();
                    //oobeConfig.ResetToDefaults();
                    //bitLockerConfig.ResetToDefaults();
                    //personalizeConfig.ResetToDefaults();
                    //appConfig.ResetToDefaults();
                    _hasUnsavedChanges = false;
                    UpdateStatusLabel(LangManager.GetString("MainForm_Status_AllConfigsCleared", "All configurations cleared."));
                }
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToClearConfigs", "Failed to clear configurations."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToClearConfigs", "Failed to clear configurations: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
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
                        UpdateStatusLabel(LangManager.GetString("MainForm_Status_PresetApplied", "Preset applied."));
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToOpenPreset", "Failed to open preset."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToOpenPreset", "Failed to open preset: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        #endregion

        #region Generate Button Click
        private void genBtn_Click(object sender, EventArgs e)
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
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Check if save path is set
                var settings = SettingsManager.LoadSettings();
                if (string.IsNullOrEmpty(settings.SavePath))
                {
                    UpdateStatusLabel(LangManager.GetString("MainForm_Status_NoSavePath", "No save path specified in settings."));
                    MessageBox.Show(
                        string.Format(LangManager.GetString("MainForm_Error_NoSavePath", "No save path specified in settings: {0}"), validationError),
                        LangManager.GetString("MainForm_Error_Title", "Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Check if save path exists
                if (!Directory.Exists(settings.SavePath))
                {
                    UpdateStatusLabel(LangManager.GetString("MainForm_Status_SavePathNotExist", "The specified save path does not exist."));
                    MessageBox.Show(
                        string.Format(LangManager.GetString("MainForm_Error_SavePathNotExist", "The specified save path does not exist: {0}"), validationError),
                        LangManager.GetString("MainForm_Error_Title", "Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Build full save path
                string savePath = Path.Combine(settings.SavePath, "autounattend.xml");

                // Check if file exists and confirm overwrite
                if (File.Exists(savePath))
                {
                    var result = MessageBox.Show(
                        LangManager.GetString("MainForm_Warning_OverwriteFile", "The file autounattend.xml already exists. Overwrite?"),
                        LangManager.GetString("MainForm_Warning_Title", "Warning"),
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.Cancel)
                    {
                        UpdateStatusLabel(LangManager.GetString("MainForm_Status_GenerationCancelled", "Generation cancelled."));
                        return;
                    }
                }

                // Save current settings
                SaveAllSectionConfigs();

                // Proceed with generation
                /*
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
                */

                _hasUnsavedChanges = false;
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_GenerationCompleted", "Generation completed successfully."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Info_FileSaved", "File saved to: {0}"), savePath),
                    LangManager.GetString("MainForm_Info_Title", "Success"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_GenerationFailed", "Generation failed."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_GenerationFailed", "Generation failed: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
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
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            if (_hasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    LangManager.GetString("MainForm_Warning_UnsavedChanges", "Leave without saving?"),
                    LangManager.GetString("MainForm_Warning_Title", "Warning"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
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

        #region SaveAllSectionConfigs
        private void SaveAllSectionConfigs()
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            try
            {
                var settings = SettingsManager.LoadSettings();
                settings.GeneralConfig = generalConfig.GetCurrentConfigs();
                //settings.LangRegionConfig = langRegionConfig.GetCurrentConfigs();
                //settings.BypassConfig = bypassConfig.GetCurrentConfigs();
                //settings.DiskConfig = diskConfig.GetCurrentConfigs();
                //settings.AccountConfig = accountConfig.GetCurrentConfigs();
                //settings.OOBEConfig = oobeConfig.GetCurrentConfigs();
                //settings.BitLockerConfig = bitLockerConfig.GetCurrentConfigs();
                //settings.PersonalizeConfig = personalizeConfig.GetCurrentConfigs();
                //settings.AppConfig = appConfig.GetCurrentConfigs();
                //SettingsManager.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToSaveConfigs", "Failed to save configurations."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToSaveConfigs", "Failed to save configurations: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        #endregion

        #region LoadAllSectionConfigs
        private void LoadAllSectionConfigs()
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            try
            {
                var settings = SettingsManager.LoadSettings();
                if (settings.GeneralConfig != null)
                    generalConfig.LoadConfigs(settings.GeneralConfig);
                //if (settings.LangRegionConfig != null)
                //langRegionConfig.LoadConfigs(settings.LangRegionConfig);
                //if (settings.BypassConfig != null)
                //bypassConfig.LoadConfigs(settings.BypassConfig);
                //if (settings.DiskConfig != null)
                //diskConfig.LoadConfigs(settings.DiskConfig);
                //if (settings.AccountConfig != null)
                //accountConfig.LoadConfigs(settings.AccountConfig);
                //if (settings.OOBEConfig != null)
                //oobeConfig.LoadConfigs(settings.OOBEConfig);
                //if (settings.BitLockerConfig != null)
                //bitLockerConfig.LoadConfigs(settings.BitLockerConfig);
                //if (settings.PersonalizeConfig != null)
                //personalizeConfig.LoadConfigs(settings.PersonalizeConfig);
                //if (settings.AppConfig != null)
                //appConfig.LoadConfigs(settings.AppConfig);
                _hasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToLoadLastConfigs", "Failed to load last configurations."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToLoadLastConfigs", "Failed to load last configurations: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        #endregion

        #region ApplyPreset
        private void ApplyPreset(string preset)
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;
            var presetData = PresetsManager.LoadPresetData(preset);
            var presetInfo = PresetsManager.LoadPresetInfo(preset);
            var name = presetInfo.Name ?? LangManager.GetString("PresetForm_Info_NoData", "");

            try
            {
                _isValidating = true;

                if (presetData.GeneralConfig != null)
                    generalConfig.LoadConfigs(presetData.GeneralConfig);
                //if (preset.LangRegionConfig != null)
                //langRegionConfig.LoadConfigs(preset.LangRegionConfig);
                //if (preset.BypassConfig != null)
                //bypassConfig.LoadConfigs(preset.BypassConfig);
                //if (preset.DiskConfig != null)
                //diskConfig.LoadConfigs(preset.DiskConfig);
                //if (preset.AccountConfig != null)
                //accountConfig.LoadConfigs(preset.AccountConfig);
                //if (preset.OOBEConfig != null)
                //oobeConfig.LoadConfigs(preset.OOBEConfig);
                //if (preset.BitLockerConfig != null)
                //bitLockerConfig.LoadConfigs(preset.BitLockerConfig);
                //if (preset.PersonalizeConfig != null)
                //personalizeConfig.LoadConfigs(preset.PersonalizeConfig);
                //if (preset.AppConfig != null)
                //appConfig.LoadConfigs(preset.AppConfig);
                _hasUnsavedChanges = true;
                UpdateStatusLabel(string.Format(LangManager.GetString("MainForm_Status_PresetApplied", "Preset {0} applied."), name));
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(string.Format(LangManager.GetString("MainForm_Status_FailedToApplyPreset", "Failed to apply preset: {0}"), name));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToApplyPreset", "Failed to apply preset {0}: {1}"), name, ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                _isValidating = false;
            }
        }
        #endregion

        #region RefreshLang
        public void RefreshLang()
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
                if (genBtn != null)
                    genBtn.Text = LangManager.GetString("MainForm_GenerateButton", "Generate");
                if (cancelBtn != null)
                    cancelBtn.Text = LangManager.GetString("MainForm_CancelButton", "Cancel");

                if (statusPrefixLabel != null)
                    statusPrefixLabel.Text = LangManager.GetString("MainForm_StatusPrefixLabel", "Status:");

                RefreshSections();
            }
            catch (Exception ex)
            {
                UpdateStatusLabel(LangManager.GetString("MainForm_Status_FailedToRefreshLanguage", "Failed to refresh language."));
                MessageBox.Show(
                    string.Format(LangManager.GetString("MainForm_Error_FailedToRefreshLanguage", "Failed to refresh language: {0}"), ex.Message),
                    LangManager.GetString("MainForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
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
                    if (control is ILangRefreshable refreshable)
                    {
                        refreshable.RefreshLang();
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
    #endregion
}