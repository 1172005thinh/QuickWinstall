using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    public partial class AboutForm : Form
    {
        private readonly ThemeManager _themeManager;
        private readonly LangManager _langManager;
        private readonly IconManager _iconManager;
        private readonly UIValues _uiValues;

        // Version
        public string appVersion = "0.7.1.1";
        public bool _isSEdition = true;
        public string sEdition = "Get Ready for UserAccount Config";

        public AboutForm()
        {
            InitializeComponent();

            _themeManager = ThemeManager.Instance;
            _langManager = LangManager.Instance;
            _iconManager = IconManager.Instance;
            _uiValues = UIValues.Instance;

            ApplyTheme();
            ApplyLanguage();
        }

        private void ApplyTheme()
        {
            // Apply theme colors
            this.BackColor = _themeManager.GetColor("background");
            pnlBanner.BackColor = _themeManager.GetColor("background");
            pnlAbout.BackColor = _themeManager.GetColor("background");
            pnlControls.BackColor = _themeManager.GetColor("background");
            
            // Apply button theme
            _themeManager.ApplyButtonTheme(btnClose);
            
            // Update label colors and transparency
            lblProject.BackColor = Color.Transparent;
            lblProject.ForeColor = _themeManager.GetFontColor("header");
            lblBrief.BackColor = Color.Transparent;
            lblBrief.ForeColor = _themeManager.GetFontColor("normal");
            lblOriginalAuthor.BackColor = Color.Transparent;
            lblOriginalAuthor.ForeColor = _themeManager.GetFontColor("subheader");
            lblOriginalAuthorNameBullet.BackColor = Color.Transparent;
            lblOriginalAuthorNameBullet.ForeColor = _themeManager.GetFontColor("normal");
            lblOriginalAuthorName.BackColor = Color.Transparent;
            lblOriginalAuthorName.ForeColor = _themeManager.GetFontColor("normal");
            lblFollowMe.BackColor = Color.Transparent;
            lblFollowMe.ForeColor = _themeManager.GetFontColor("subheader");
            lblContributors.BackColor = Color.Transparent;
            lblContributors.ForeColor = _themeManager.GetFontColor("subheader");
            lblContributor1Bullet.BackColor = Color.Transparent;
            lblContributor1Bullet.ForeColor = _themeManager.GetFontColor("normal");
            lblContributor1Name.BackColor = Color.Transparent;
            lblContributor1Name.ForeColor = _themeManager.GetFontColor("normal");
            lblReferences.BackColor = Color.Transparent;
            lblReferences.ForeColor = _themeManager.GetFontColor("subheader");
            lblReference1Bullet.BackColor = Color.Transparent;
            lblReference1Bullet.ForeColor = _themeManager.GetFontColor("normal");
            lblReference2Bullet.BackColor = Color.Transparent;
            lblReference2Bullet.ForeColor = _themeManager.GetFontColor("normal");
            lblReference3Bullet.BackColor = Color.Transparent;
            lblReference3Bullet.ForeColor = _themeManager.GetFontColor("normal");
            lblLicense.BackColor = Color.Transparent;
            lblLicense.ForeColor = _themeManager.GetFontColor("subheader");
            lblLicenseName.BackColor = Color.Transparent;
            lblLicenseName.ForeColor = _themeManager.GetFontColor("normal");
            lblVersion.BackColor = Color.Transparent;
            lblVersion.ForeColor = _themeManager.GetFontColor("muted");
            
            // Apply link colors
            linkReference1.LinkColor = _themeManager.GetColor("link");
            linkReference1.ActiveLinkColor = _themeManager.GetColor("linkActive");
            linkReference1.VisitedLinkColor = _themeManager.GetColor("linkVisited");
            linkReference2.LinkColor = _themeManager.GetColor("link");
            linkReference2.ActiveLinkColor = _themeManager.GetColor("linkActive");
            linkReference2.VisitedLinkColor = _themeManager.GetColor("linkVisited");
            linkReference3.LinkColor = _themeManager.GetColor("link");
            linkReference3.ActiveLinkColor = _themeManager.GetColor("linkActive");
            linkReference3.VisitedLinkColor = _themeManager.GetColor("linkVisited");
            
            // Update icons based on theme
            bool isDark = _themeManager.IsDarkTheme;
            
            // Update social media icons
            if (picYoutube.Image != null) picYoutube.Image.Dispose();
            if (picGitHub.Image != null) picGitHub.Image.Dispose();
            if (picFacebook.Image != null) picFacebook.Image.Dispose();
            
            int iconSize = _uiValues.GlobalBtnBox;
            this.Icon = _iconManager.GetIcon("about", isDark && false);
            picYoutube.Image = _iconManager.GetIconAsImage("youtube", isDark, iconSize);
            picGitHub.Image = _iconManager.GetIconAsImage("github", isDark, iconSize);
            picFacebook.Image = _iconManager.GetIconAsImage("facebook", isDark, iconSize);
        }

        private void ApplyLanguage()
        {
            // Update form title
            this.Text = _langManager.GetString("aboutForm.title");
            
            // Banner labels
            lblProject.Text = _langManager.GetString("aboutForm.banner.project");
            lblBrief.Text = _langManager.GetString("aboutForm.banner.brief");
            
            // About panel labels
            lblOriginalAuthor.Text = _langManager.GetString("aboutForm.originalAuthor.label");
            lblOriginalAuthorName.Text = _langManager.GetString("aboutForm.originalAuthor.name");
            lblFollowMe.Text = _langManager.GetString("aboutForm.followMe.label");
            
            lblContributors.Text = _langManager.GetString("aboutForm.contributors.label");
            lblContributor1Name.Text = _langManager.GetString("aboutForm.contributors.contributor1");
            
            lblReferences.Text = _langManager.GetString("aboutForm.references.label");
            linkReference1.Text = _langManager.GetString("aboutForm.references.reference1");
            linkReference2.Text = _langManager.GetString("aboutForm.references.reference2");
            linkReference3.Text = _langManager.GetString("aboutForm.references.reference3");

            lblLicense.Text = _langManager.GetString("aboutForm.license.label");
            lblLicenseName.Text = _langManager.GetString("aboutForm.license.name");
            
            // Control panel
            lblVersion.Text = $"{_langManager.GetString("aboutForm.version.label")}" + $" v{appVersion}" + (_isSEdition ? $" ({sEdition})" : "");
            btnClose.Text = _langManager.GetString("aboutForm.buttons.close");
            
            // Tooltips
            ToolTipManager tooltips = ToolTipManager.Instance;
            tooltips.SetToolTip(picYoutube, "aboutForm.tooltips.youtube", "https://www.youtube.com/@quickcompstore/");
            tooltips.SetToolTip(picGitHub, "aboutForm.tooltips.github", "https://github.com/1172005thinh/");
            tooltips.SetToolTip(picFacebook, "aboutForm.tooltips.facebook", "https://www.facebook.com/quickcomp.hungthinhnguyen/");
            tooltips.SetToolTip(btnClose, "aboutForm.tooltips.close");
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void PicYoutube_Click(object? sender, EventArgs e)
        {
            OpenUrl("https://www.youtube.com/@quickcompstore/");
        }

        private void PicGitHub_Click(object? sender, EventArgs e)
        {
            OpenUrl("https://github.com/1172005thinh/");
        }

        private void PicFacebook_Click(object? sender, EventArgs e)
        {
            OpenUrl("https://www.facebook.com/quickcomp.hungthinhnguyen/");
        }

        private void LinkReference1_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://schneegans.de/windows/unattend-generator/");
        }

        private void LinkReference2_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_isSEdition)
            {
                OpenUrl("https://marisa0704.itch.io/brother-hais-pho-restaurant");
            }
            else
            {
                OpenUrl("https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/update-windows-settings-and-scripts-create-your-own-answer-file-sxs?view=windows-11");
            }
        }

        private void LinkReference3_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://learn.microsoft.com/en-us/windows/deployment/");
        }

        private void LinkReference4_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl("https://github.com/1172005thinh/quickwinstall/");
        }

        private void OpenUrl(string url)
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
                MessageBox.Show($"Could not open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
