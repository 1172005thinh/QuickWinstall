using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QuickWinstall
{
    public partial class SettingsForm : Form
    {
        public AppSettings Settings { get; private set; }
        private AppSettings _originalSettings;

        public SettingsForm(AppSettings settings)
        {
            InitializeComponent();
            _originalSettings = settings;
            Settings = settings.Clone();
            LoadSettings();
            UpdateUIText();
        }

        private void UpdateUIText()
        {
            this.Text = LanguageManager.Instance.GetString("SettingsForm_Title", "Settings");
            lblLanguage.Text = LanguageManager.Instance.GetString("SettingsForm_Language", "Language:");
            lblDefaultLocation.Text = LanguageManager.Instance.GetString("SettingsForm_DefaultLocation", "XML Default Location:");
            btnBrowse.Text = LanguageManager.Instance.GetString("SettingsForm_BrowseButton", "Browse...");
            btnAbout.Text = LanguageManager.Instance.GetString("AboutForm_Title", "About");
            btnSave.Text = LanguageManager.Instance.GetString("SettingsForm_SaveButton", "Save");
            btnCancel.Text = LanguageManager.Instance.GetString("SettingsForm_CancelButton", "Cancel");
        }

        private void LoadSettings()
        {
            // Set language
            var languages = LanguageManager.Instance.GetAvailableLanguages();
            var currentLanguage = languages.FirstOrDefault(l => l.Code == Settings.UILanguage);
            if (currentLanguage != null)
            {
                cmbLanguage.SelectedValue = currentLanguage.Code;
            }

            // Set default location
            txtDefaultLocation.Text = Settings.DefaultFileLocation;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = LanguageManager.Instance.GetString("SettingsForm_SelectFolder", "Select default folder for XML files:");
                dialog.SelectedPath = Settings.DefaultFileLocation;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtDefaultLocation.Text = dialog.SelectedPath;
                    Settings.DefaultFileLocation = dialog.SelectedPath;
                }
            }
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Save language setting
            if (cmbLanguage.SelectedValue != null)
            {
                Settings.UILanguage = cmbLanguage.SelectedValue.ToString();
            }

            // Settings.DefaultFileLocation is already updated in BtnBrowse_Click
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}