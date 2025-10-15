using QuickWinstall.Lib;

namespace QuickWinstall
{
    public partial class SettingsForm : Form, ILangRefreshable
    {
        // State variables
        private bool _isRefreshingLang = false;

        #region SettingsForm
        public SettingsForm()
        {
            InitializeComponent();

            LoadCurrentSettings();

            LangHelper.RegisterForm(this);
            RefreshLang();

            ThemeManager.SetForm(this);
        }        

        #region LoadCurrentSettings
        private void LoadCurrentSettings()
        {
            // Load configurations
            var config = Config.LoadFromAppFolder();
            var defaults = Defaults.LoadFromAppFolder();
            var settings = SettingsManager.LoadSettings();
            var globalConfig = config.Global;
            var settingsFormConfig = config.SettingsForm;
            var langSettings = defaults.LangSettings;
            var themeSettings = defaults.ThemeSettings;

            // Load current lang
            langCombo.Items.Clear();
            foreach (var lang in LangManager.GetAvailableLangs())
            {
                langCombo.Items.Add(lang);
                if (lang.Code == langSettings.Lang)
                {
                    langCombo.SelectedItem = lang;
                }
            }

            // Set current land
            for (int i = 0; i < langCombo.Items.Count; i++)
            {
                if (((LangManager.LangItem)langCombo.Items[i]).Code == langSettings.Lang)
                {
                    langCombo.SelectedIndex = i;
                    break;
                }
            }

            // Load current theme
            themeCombo.Items.Clear();
            foreach (var theme in ThemeManager.GetAvailableThemes())
            {
                themeCombo.Items.Add(theme);
                if (theme.Name == themeSettings.Theme)
                {
                    themeCombo.SelectedItem = theme;
                }
            }

            // Set current theme
            for (int i = 0; i < themeCombo.Items.Count; i++)
            {
                if (((ThemeManager.ThemeItem)themeCombo.Items[i]).Name == themeSettings.Theme)
                {
                    themeCombo.SelectedIndex = i;
                    break;
                }
            }

            // Load current save path
            savePathTextBox.Text = !string.IsNullOrWhiteSpace(settings.SavePath) ? settings.SavePath : string.Empty;
        }
        #endregion

        #region Browse Button Click
        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = LangManager.GetString("SettingsForm_BrowseDialog_Description", "Select Folder to Save XML Files");
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    savePathTextBox.Text = dialog.SelectedPath;
                }
            }
        }
        #endregion

        #region About Button Click
        private void aboutBtn_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Help Button Click
        private void helpBtn_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Save Button Click
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (_isRefreshingLang) return;

            // Save lang
            if (langCombo.SelectedItem is LangManager.LangItem selectedLang)
            {
                SettingsManager.UpdateSetting("lang", selectedLang.Code);
                LangManager.SetLang(selectedLang.Code);
            }

            // Save theme
            if (themeCombo.SelectedItem is ThemeManager.ThemeItem selectedTheme)
            {
                SettingsManager.UpdateSetting("theme", selectedTheme.Name);
                ThemeManager.SwitchTheme(selectedTheme.Name);
                ThemeManager.SetForm(this);
            }

            // Save path
            var savePath = savePathTextBox.Text.Trim();
            SettingsManager.UpdateSetting("savepath", savePath);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region Cancel Button Click
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region RefreshLang
        public void RefreshLang()
        {
            try
            {
                _isRefreshingLang = true;
                
                this.Text = LangManager.GetString("SettingsForm_Title", "Settings");

                if (langLabel != null) langLabel.Text = LangManager.GetString("SettingsForm_LanguageLabel", "Language:");
                if (themeLabel != null) themeLabel.Text = LangManager.GetString("SettingsForm_ThemeLabel", "Theme:");
                if (savePathLabel != null) savePathLabel.Text = LangManager.GetString("SettingsForm_SavePathLabel", "XML Save Path:");
                if (browseBtn != null) browseBtn.Text = LangManager.GetString("SettingsForm_BrowseButton", "Browse");

                if (aboutBtn != null) aboutBtn.Text = LangManager.GetString("SettingsForm_AboutButton", "About");
                if (helpBtn != null) helpBtn.Text = LangManager.GetString("SettingsForm_HelpButton", "Help");
                if (saveBtn != null) saveBtn.Text = LangManager.GetString("SettingsForm_SaveButton", "Save");
                if (cancelBtn != null) cancelBtn.Text = LangManager.GetString("SettingsForm_CancelButton", "Cancel");

                if (themeCombo != null) LangHelper.RefreshThemeComboBox(themeCombo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsForm: Error refreshing language: {ex.Message}");
            }
            finally
            {
                _isRefreshingLang = false;
            }
        }
        #endregion
    }
    #endregion
}
