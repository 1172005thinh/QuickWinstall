using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace QuickWinstall
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            UpdateUIText();
        }

        private void UpdateUIText()
        {
            this.Text = LanguageManager.Instance.GetString("AboutForm_Title", "About");
            
            // Update labels with localized text if needed
            lblProjectTitle.Text = LanguageManager.Instance.GetString("AboutForm_ProjectTitle", 
                "Project: QuickWinstall - Autounattend Generator");
            lblVersion.Text = LanguageManager.Instance.GetString("AboutForm_Version", "Version:") + " 0.1";
            lblDescription.Text = LanguageManager.Instance.GetString("AboutForm_DescriptionInfo", 
                "For LAZYY~ Windows 11 Installation");
            lblAuthor.Text = LanguageManager.Instance.GetString("AboutForm_AuthorInfo", 
                "Original author: 1172005thinh (QuickComp.)");
            
            lblContributors.Text = LanguageManager.Instance.GetString("AboutForm_Contributors", "Contributors:");
            lblContributorsContent.Text = LanguageManager.Instance.GetString("AboutForm_ContributorsContent", "updating...");
            lblReferences.Text = LanguageManager.Instance.GetString("AboutForm_References", "References:");
            lblLicense.Text = LanguageManager.Instance.GetString("AboutForm_License", "License:");
            lblLicenseContent.Text = LanguageManager.Instance.GetString("AboutForm_LicenseContent", 
                "     This is a free, open-source project.");
            
            btnOK.Text = LanguageManager.Instance.GetString("Common_OK", "OK");
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender is LinkLabel linkLabel && linkLabel.Tag is string url)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Unable to open link: {url}\n\nError: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}