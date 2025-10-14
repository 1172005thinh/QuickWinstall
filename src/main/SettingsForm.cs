using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    public partial class SettingsForm : Form, ILangRefreshable
    {
        public SettingsForm()
        {
            Initialize();
            LoadCurrentSettings();
            
            // Register for automatic lang refresh
            LangHelper.RegisterForm(this);
            
            // Apply current theme
            ThemeManager.SetForm(this);
        }

        private void LoadCurrentSettings()
        {
            // Load current settings
            var settings = SettingsManager.LoadSettings();

            // Load lang options
            langCombo.Items.Clear();
            var availableLanguages = LangManager.GetAvailableLangs();
            foreach (var lang in availableLanguages)
            {
                langCombo.Items.Add(new LanguageItem(lang, LangManager.GetLangDisplayName(lang)));
            }

            // Set current lang from settings
            var currentLang = settings.Language;
            for (int i = 0; i < langCombo.Items.Count; i++)
            {
                if (((LanguageItem)langCombo.Items[i]).Code == currentLang)
                {
                    langCombo.SelectedIndex = i;
                    break;
                }
            }

            // Load theme options
            themeCombo.Items.AddRange(new string[] { 
                LangManager.GetString("SettingsForm_ThemeLight"), 
                LangManager.GetString("SettingsForm_ThemeDark")
            });
            
            // Set current theme selection
            string currentTheme = settings.Theme;
            if (currentTheme == "Dark")
                themeCombo.SelectedItem = LangManager.GetString("SettingsForm_ThemeDark");
            else
                themeCombo.SelectedItem = LangManager.GetString("SettingsForm_ThemeLight");

            // Load XML save path from settings
            defaultLocationTextBox.Text = !string.IsNullOrEmpty(settings.XmlSavePath) 
                ? settings.XmlSavePath 
                : Application.StartupPath;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = LangManager.GetString("SettingsForm_BrowseFolderDialog_Description", "Select folder to save autounattend.xml");
                folderDialog.SelectedPath = defaultLocationTextBox.Text;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    defaultLocationTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            using (var helpForm = new HelpForm())
            {
                helpForm.ShowDialog(this);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = SettingsManager.LoadSettings();
                // Save lang setting
                if (langCombo.SelectedItem is LanguageItem selectedLang)
                {
                    if (settings.Lang != selectedLang.Code)
                    {
                        settings.Lang = selectedLang.Code;
                        LangManager.SetLang(selectedLang.Code);
                    }
                }

                // Save theme setting
                string selectedTheme = "Light"; // Default
                if (themeCombo.SelectedItem != null)
                {
                    string selectedThemeText = themeCombo.SelectedItem.ToString();
                    if (selectedThemeText == LangManager.GetString("SettingsForm_ThemeDark", "Dark"))
                        selectedTheme = "Dark";
                    else
                        selectedTheme = "Light";
                }
                
                if (settings.Theme != selectedTheme)
                {
                    settings.Theme = selectedTheme;
                    ThemeManager.SwitchTheme(selectedTheme);
                }

                // Save XML save path
                settings.XmlSavePath = defaultLocationTextBox.Text;

                // Save all settings
                if (SettingsManager.SaveSettings(settings))
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    var errorMessage = LangManager.GetString("MainForm_Error_FailedToSaveSettings", "Failed to save settings. Please try again.");
                    MessageBox.Show(errorMessage, 
                        LangManager.GetString("MainForm_Error_Title", "Error"),
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error saving settings: {ex.Message}";
                MessageBox.Show(errorMessage, 
                    LangManager.GetString("MainForm_Error_Title"), 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"SettingsForm: {errorMessage}");
                return;
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        internal class LanguageItem
        {
            public string Code { get; }
            public string DisplayName { get; }

            public LanguageItem(string code, string displayName)
            {
                Code = code;
                DisplayName = displayName;
            }

            public override string ToString() => DisplayName;
        }

        #region RefreshLang
        public void RefreshLang()
        {
            try
            {
                this.Text = LangManager.GetString("SettingsForm_Title", "Settings");

                if (langLabel != null) langLabel.Text = LangManager.GetString("SettingsForm_Language", "Language:");
                if (themeLabel != null) themeLabel.Text = LangManager.GetString("SettingsForm_Theme", "Theme:");
                if (defaultLocationLabel != null) defaultLocationLabel.Text = LangManager.GetString("SettingsForm_XMLPath", "XML Path:");

                if (aboutButton != null) aboutButton.Text = LangManager.GetString("SettingsForm_ButtonAbout", "About");
                if (helpButton != null) helpButton.Text = LangManager.GetString("SettingsForm_ButtonHelp", "Help");
                if (saveButton != null) saveButton.Text = LangManager.GetString("SettingsForm_ButtonSave", "Save");
                if (cancelButton != null) cancelButton.Text = LangManager.GetString("SettingsForm_ButtonCancel", "Cancel");

                // Refresh theme ComboBox
                if (themeCombo != null)
                {
                    LangHelper.RefreshThemeComboBox(themeCombo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsForm.RefreshLanguage error: {ex.Message}");
            }
        }
        #endregion
    }
}
