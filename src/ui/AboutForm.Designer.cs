using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Panels
        private Panel pnlBanner;
        private Panel pnlAbout;
        private Panel pnlControls;

        // Banner elements
        private PictureBox picAppIcon;
        private Label lblProject;
        private Label lblBrief;
        private PictureBox picBannerImage;

        // About panel elements
        //private PictureBox picAboutBg;
        private Label lblOriginalAuthor;
        private Label lblOriginalAuthorNameBullet;
        private Label lblOriginalAuthorName;
        private Label lblFollowMe;
        private PictureBox picYoutube;
        private PictureBox picGitHub;
        private PictureBox picFacebook;
        private Label lblContributors;
        private Label lblContributor1Bullet;
        private Label lblContributor1Name;
        private Label lblReferences;
        private Label lblReference1Bullet;
        private LinkLabel linkReference1;
        private Label lblReference2Bullet;
        private LinkLabel linkReference2;
        private Label lblReference3Bullet;
        private LinkLabel linkReference3;
        private Label lblReference4Bullet;
        private LinkLabel linkReference4;
        private Label lblLicense;
        private Label lblLicenseName;

        // Control panel elements
        private Label lblVersion;
        private Button btnClose;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;

            // Form settings
            this.Text = lang.GetString("aboutForm.title");
            this.Width = ui.GetValue("forms.aboutForm.width", 580);
            this.Height = ui.GetValue("forms.aboutForm.height", 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.BackColor = theme.GetColor("formBackground");

            // Banner Panel
            this.pnlBanner = new Panel();
            this.pnlBanner.Location = new Point(0, 0);
            this.pnlBanner.Size = new Size(this.ClientSize.Width, 100);
            this.pnlBanner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlBanner.BackColor = theme.GetColor("bannerBackground");

            // App Icon in Banner
            this.picAppIcon = new PictureBox();
            this.picAppIcon.Size = new Size(ui.GlobalIconSize * 5, ui.GlobalIconSize * 5);
            this.picAppIcon.Location = new Point(ui.GlobalTabX, (pnlBanner.Height - (ui.GlobalIconSize * 5)) / 2);
            this.picAppIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            try
            {
                string iconPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "res", "icons", "app256.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.picAppIcon.Image = new Icon(iconPath).ToBitmap();
                }
            }
            catch { }

            int extraHeight = 15;

            // Project Label
            this.lblProject = new Label();
            this.lblProject.Text = lang.GetString("aboutForm.banner.project");
            this.lblProject.Font = theme.GetFont("header");
            this.lblProject.AutoSize = true;
            this.lblProject.BackColor = Color.Transparent;
            this.lblProject.Location = new Point(picAppIcon.Right + ui.GlobalSpacingX, (pnlBanner.Height / 2) - ui.GlobalLabelHeight);
            this.lblProject.ForeColor = theme.GetFontColor("header");
            this.lblProject.TextAlign = ContentAlignment.MiddleLeft;

            // Brief Label
            this.lblBrief = new Label();
            this.lblBrief.Text = lang.GetString("aboutForm.banner.brief");
            this.lblBrief.Font = theme.GetFont("normal");
            this.lblBrief.AutoSize = false;
            this.lblBrief.BackColor = Color.Transparent;
            this.lblBrief.Size = new Size(ui.GlobalLabelWidth * 2 - 60, ui.GlobalLabelHeight * 2 - extraHeight);
            this.lblBrief.Location = new Point(picAppIcon.Right + ui.GlobalSpacingX, (pnlBanner.Height / 2));
            this.lblBrief.ForeColor = theme.GetFontColor("normal");
            this.lblBrief.TextAlign = ContentAlignment.MiddleLeft;

            // Banner Background Image
            this.picBannerImage = new PictureBox();
            this.picBannerImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.picBannerImage.Size = new Size(pnlBanner.Height, pnlBanner.Height);
            this.picBannerImage.Location = new Point(this.ClientSize.Width - pnlBanner.Height, 0);
            this.picBannerImage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            try
            {
                string imagePath;
                if (lang.CurrentLanguage == "pho-AnhHai")
                {
                    imagePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "res", "images", "bg_CauVang.png");
                }
                else
                {
                    imagePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "res", "images", "bg_Windows11.png");
                }

                if (System.IO.File.Exists(imagePath))
                {
                    this.picBannerImage.Image = Image.FromFile(imagePath);
                    this.picBannerImage.Visible = true;
                }
                else
                {
                    this.picBannerImage.Visible = false;
                }
            }
            catch { }

            // About Panel
            this.pnlAbout = new Panel();
            this.pnlAbout.Location = new Point(0, pnlBanner.Bottom);
            this.pnlAbout.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - pnlBanner.Height - 60);
            this.pnlAbout.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.pnlAbout.BackColor = theme.GetColor("formBackground");
            this.pnlAbout.AutoScroll = true;

            // About Panel Background Image
            /*
            this.picAboutBg = new PictureBox();
            this.picAboutBg.SizeMode = PictureBoxSizeMode.Zoom;
            this.picAboutBg.Dock = DockStyle.Left;
            this.picAboutBg.Width = pnlAbout.Width / 2;
            try
            {
                string imagePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "res", "images", "bg_AboutForm.png");
                if (System.IO.File.Exists(imagePath))
                {
                    this.picAboutBg.Image = Image.FromFile(imagePath);
                }
            }
            catch { }
            */

            int currentY = ui.GlobalSpacingY * 2;
            int labelX = ui.GlobalTabX * 1;
            int contentX = ui.GlobalTabX * 2;

            // Original Author Label
            this.lblOriginalAuthor = new Label();
            this.lblOriginalAuthor.Text = lang.GetString("aboutForm.originalAuthor.label");
            this.lblOriginalAuthor.Font = theme.GetFont("subheader");
            this.lblOriginalAuthor.BackColor = Color.Transparent;
            this.lblOriginalAuthor.AutoSize = true;
            this.lblOriginalAuthor.Location = new Point(labelX, currentY);
            this.lblOriginalAuthor.ForeColor = theme.GetFontColor("subheader");

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // Original Author Name Bullet
            this.lblOriginalAuthorNameBullet = new Label();
            this.lblOriginalAuthorNameBullet.Text = "•";
            this.lblOriginalAuthorNameBullet.Font = theme.GetFont("normal");
            this.lblOriginalAuthorNameBullet.BackColor = Color.Transparent;
            this.lblOriginalAuthorNameBullet.Size = new Size(15, ui.GlobalLabelHeight);
            this.lblOriginalAuthorNameBullet.Location = new Point(contentX, currentY);
            this.lblOriginalAuthorNameBullet.ForeColor = theme.GetFontColor("normal");

            // Original Author Name
            this.lblOriginalAuthorName = new Label();
            this.lblOriginalAuthorName.Text = lang.GetString("aboutForm.originalAuthor.name");
            this.lblOriginalAuthorName.Font = theme.GetFont("normal");
            this.lblOriginalAuthorName.BackColor = Color.Transparent;
            this.lblOriginalAuthorName.AutoSize = true;
            this.lblOriginalAuthorName.Location = new Point(lblOriginalAuthorNameBullet.Right + ui.GlobalSpacingX / 2, currentY);

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // Follow Me Label
            this.lblFollowMe = new Label();
            this.lblFollowMe.Text = lang.GetString("aboutForm.followMe.label");
            this.lblFollowMe.Font = theme.GetFont("subheader");
            this.lblFollowMe.BackColor = Color.Transparent;
            this.lblFollowMe.AutoSize = true;
            this.lblFollowMe.Location = new Point(labelX, currentY);
            this.lblFollowMe.ForeColor = theme.GetFontColor("normal");

            currentY += ui.GlobalBtnBox;
            int iconSize = ui.GlobalBtnBox;
            int iconX = ui.GlobalTabX * 3;

            // YouTube Icon
            this.picYoutube = new PictureBox();
            this.picYoutube.Size = new Size(iconSize, iconSize);
            this.picYoutube.Location = new Point(iconX, currentY);
            this.picYoutube.BackColor = Color.Transparent;
            this.picYoutube.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picYoutube.Cursor = Cursors.Hand;
            this.picYoutube.Click += PicYoutube_Click;

            // GitHub Icon
            this.picGitHub = new PictureBox();
            this.picGitHub.Size = new Size(iconSize, iconSize);
            this.picGitHub.Location = new Point(picYoutube.Right + ui.GlobalSpacingX * 2, currentY);
            this.picGitHub.BackColor = Color.Transparent;
            this.picGitHub.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picGitHub.Cursor = Cursors.Hand;
            this.picGitHub.Click += PicGitHub_Click;

            // Facebook Icon
            this.picFacebook = new PictureBox();
            this.picFacebook.Size = new Size(iconSize, iconSize);
            this.picFacebook.Location = new Point(picGitHub.Right + ui.GlobalSpacingX * 2, currentY);
            this.picFacebook.BackColor = Color.Transparent;
            this.picFacebook.SizeMode = PictureBoxSizeMode.StretchImage;
            this.picFacebook.Cursor = Cursors.Hand;
            this.picFacebook.Click += PicFacebook_Click;

            currentY += iconSize + ui.GlobalSpacingY;

            // Contributors Label
            this.lblContributors = new Label();
            this.lblContributors.Text = lang.GetString("aboutForm.contributors.label");
            this.lblContributors.Font = theme.GetFont("subheader");
            this.lblContributors.BackColor = Color.Transparent;
            this.lblContributors.AutoSize = true;
            this.lblContributors.Location = new Point(labelX, currentY);
            this.lblContributors.ForeColor = theme.GetFontColor("subheader");

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // Contributor 1 Bullet
            this.lblContributor1Bullet = new Label();
            this.lblContributor1Bullet.Text = "•";
            this.lblContributor1Bullet.Font = theme.GetFont("normal");
            this.lblContributor1Bullet.BackColor = Color.Transparent;
            this.lblContributor1Bullet.Size = new Size(15, ui.GlobalLabelHeight);
            this.lblContributor1Bullet.Location = new Point(contentX, currentY);
            this.lblContributor1Bullet.ForeColor = theme.GetFontColor("normal");

            // Contributor 1 Name
            this.lblContributor1Name = new Label();
            this.lblContributor1Name.Text = lang.GetString("aboutForm.contributors.contributor1");
            this.lblContributor1Name.Font = theme.GetFont("normal");
            this.lblContributor1Name.BackColor = Color.Transparent;
            this.lblContributor1Name.AutoSize = true;
            this.lblContributor1Name.Location = new Point(lblContributor1Bullet.Right + ui.GlobalSpacingX / 2, currentY);
            this.lblContributor1Name.ForeColor = theme.GetFontColor("normal");            

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // References Label
            this.lblReferences = new Label();
            this.lblReferences.Text = lang.GetString("aboutForm.references.label");
            this.lblReferences.Font = theme.GetFont("subheader");
            this.lblReferences.BackColor = Color.Transparent;
            this.lblReferences.AutoSize = true;
            this.lblReferences.Location = new Point(labelX, currentY);
            this.lblReferences.ForeColor = theme.GetFontColor("subheader");

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // Reference 1 Bullet
            this.lblReference1Bullet = new Label();
            this.lblReference1Bullet.Text = "•";
            this.lblReference1Bullet.Font = theme.GetFont("normal");
            this.lblReference1Bullet.BackColor = Color.Transparent;
            this.lblReference1Bullet.Size = new Size(15, ui.GlobalLabelHeight);
            this.lblReference1Bullet.Location = new Point(contentX, currentY);
            this.lblReference1Bullet.ForeColor = theme.GetFontColor("normal");

            // Reference 1 Link
            this.linkReference1 = new LinkLabel();
            this.linkReference1.Text = lang.GetString("aboutForm.references.reference1");
            this.linkReference1.Font = theme.GetFont("normal");
            this.linkReference1.BackColor = Color.Transparent;
            this.linkReference1.AutoSize = true;
            this.linkReference1.Location = new Point(lblReference1Bullet.Right + ui.GlobalSpacingX / 2, currentY);
            this.linkReference1.LinkColor = theme.GetColor("link");
            this.linkReference1.ActiveLinkColor = theme.GetColor("linkActive");
            this.linkReference1.VisitedLinkColor = theme.GetColor("linkVisited");
            this.linkReference1.LinkClicked += LinkReference1_LinkClicked;

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY / 2;

            // Reference 2 Bullet
            this.lblReference2Bullet = new Label();
            this.lblReference2Bullet.Text = "•";
            this.lblReference2Bullet.Font = theme.GetFont("normal");
            this.lblReference2Bullet.BackColor = Color.Transparent;
            this.lblReference2Bullet.Size = new Size(15, ui.GlobalLabelHeight);
            this.lblReference2Bullet.Location = new Point(contentX, currentY);
            this.lblReference2Bullet.ForeColor = theme.GetFontColor("normal");

            // Reference 2 Link
            this.linkReference2 = new LinkLabel();
            this.linkReference2.Text = lang.GetString("aboutForm.references.reference2");
            this.linkReference2.Font = theme.GetFont("normal");
            this.linkReference2.BackColor = Color.Transparent;
            this.linkReference2.AutoSize = true;
            this.linkReference2.Location = new Point(lblReference2Bullet.Right + ui.GlobalSpacingX / 2, currentY);
            this.linkReference2.LinkColor = theme.GetColor("link");
            this.linkReference2.ActiveLinkColor = theme.GetColor("linkActive");
            this.linkReference2.VisitedLinkColor = theme.GetColor("linkVisited");
            this.linkReference2.LinkClicked += LinkReference2_LinkClicked;

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY / 2;

            // Reference 3 Bullet
            this.lblReference3Bullet = new Label();
            this.lblReference3Bullet.Text = "•";
            this.lblReference3Bullet.Font = theme.GetFont("normal");
            this.lblReference3Bullet.BackColor = Color.Transparent;
            this.lblReference3Bullet.Size = new Size(15, ui.GlobalLabelHeight);
            this.lblReference3Bullet.Location = new Point(contentX, currentY);
            this.lblReference3Bullet.ForeColor = theme.GetFontColor("normal");

            // Reference 3 Link
            this.linkReference3 = new LinkLabel();
            this.linkReference3.Text = lang.GetString("aboutForm.references.reference3");
            this.linkReference3.Font = theme.GetFont("normal");
            this.linkReference3.BackColor = Color.Transparent;
            this.linkReference3.AutoSize = true;
            this.linkReference3.Location = new Point(lblReference3Bullet.Right + ui.GlobalSpacingX / 2, currentY);
            this.linkReference3.LinkColor = theme.GetColor("link");
            this.linkReference3.ActiveLinkColor = theme.GetColor("linkActive");
            this.linkReference3.VisitedLinkColor = theme.GetColor("linkVisited");
            this.linkReference3.LinkClicked += LinkReference3_LinkClicked;

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY / 2;

            // Reference 4 Bullet
            this.lblReference4Bullet = new Label();
            this.lblReference4Bullet.Text = "•";
            this.lblReference4Bullet.Font = theme.GetFont("normal");
            this.lblReference4Bullet.BackColor = Color.Transparent;
            this.lblReference4Bullet.Size = new Size(15, ui.GlobalLabelHeight);
            this.lblReference4Bullet.Location = new Point(contentX, currentY);
            this.lblReference4Bullet.ForeColor = theme.GetFontColor("normal");

            // Reference 4 Link
            this.linkReference4 = new LinkLabel();
            this.linkReference4.Text = lang.GetString("aboutForm.references.reference4");
            this.linkReference4.Font = theme.GetFont("normal");
            this.linkReference4.BackColor = Color.Transparent;
            this.linkReference4.AutoSize = true;
            this.linkReference4.Location = new Point(lblReference4Bullet.Right + ui.GlobalSpacingX / 2, currentY);
            this.linkReference4.LinkColor = theme.GetColor("link");
            this.linkReference4.ActiveLinkColor = theme.GetColor("linkActive");
            this.linkReference4.VisitedLinkColor = theme.GetColor("linkVisited");
            this.linkReference4.LinkClicked += LinkReference4_LinkClicked;

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // License Label
            this.lblLicense = new Label();
            this.lblLicense.Text = lang.GetString("aboutForm.license.label");
            this.lblLicense.Font = theme.GetFont("subheader");
            this.lblLicense.BackColor = Color.Transparent;
            this.lblLicense.AutoSize = true;
            this.lblLicense.Location = new Point(labelX, currentY);
            this.lblLicense.ForeColor = theme.GetFontColor("subheader");

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // License Name
            this.lblLicenseName = new Label();
            this.lblLicenseName.Text = lang.GetString("aboutForm.license.name");
            this.lblLicenseName.Font = theme.GetFont("normal");
            this.lblLicenseName.BackColor = Color.Transparent;
            this.lblLicenseName.AutoSize = true;
            this.lblLicenseName.Location = new Point(contentX, currentY);
            this.lblLicenseName.ForeColor = theme.GetFontColor("normal");

            // Controls Panel
            this.pnlControls = new Panel();
            this.pnlControls.Location = new Point(0, this.ClientSize.Height - 60);
            this.pnlControls.Size = new Size(this.ClientSize.Width, 60);
            this.pnlControls.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlControls.BackColor = theme.GetColor("formBackground");

            // Version Label
            this.lblVersion = new Label();
            this.lblVersion.Text = $"{lang.GetString("aboutForm.version.label")}" + $" v{appVersion}";
            this.lblVersion.Font = theme.GetFont("muted");
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = Color.Transparent;
            this.lblVersion.Location = new Point(ui.GlobalTabX, (pnlControls.Height - ui.GlobalLabelHeight + (ui.GlobalBtnHeight - ui.GlobalLabelHeight)) / 2);
            this.lblVersion.ForeColor = theme.GetFontColor("muted");

            // Close Button
            this.btnClose = theme.CreateRoundedButton();
            this.btnClose.Text = lang.GetString("aboutForm.buttons.close");
            this.btnClose.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnClose.Font = theme.GetFont("normal");
            this.btnClose.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnWidth, (pnlControls.Height - ui.GlobalBtnHeight) / 2);
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Click += BtnClose_Click;

            // Add all controls to About Panel
            pnlAbout.Controls.Add(lblOriginalAuthor);
            pnlAbout.Controls.Add(lblOriginalAuthorNameBullet);
            pnlAbout.Controls.Add(lblOriginalAuthorName);
            pnlAbout.Controls.Add(lblFollowMe);
            pnlAbout.Controls.Add(picYoutube);
            pnlAbout.Controls.Add(picGitHub);
            pnlAbout.Controls.Add(picFacebook);
            pnlAbout.Controls.Add(lblContributors);
            pnlAbout.Controls.Add(lblContributor1Bullet);
            pnlAbout.Controls.Add(lblContributor1Name);
            pnlAbout.Controls.Add(lblReferences);
            pnlAbout.Controls.Add(lblReference1Bullet);
            pnlAbout.Controls.Add(linkReference1);
            pnlAbout.Controls.Add(lblReference2Bullet);
            pnlAbout.Controls.Add(linkReference2);
            pnlAbout.Controls.Add(lblReference3Bullet);
            pnlAbout.Controls.Add(linkReference3);
            pnlAbout.Controls.Add(lblReference4Bullet);
            pnlAbout.Controls.Add(linkReference4);
            pnlAbout.Controls.Add(lblLicense);
            pnlAbout.Controls.Add(lblLicenseName);
            //pnlAbout.Controls.Add(picAboutBg);

            pnlControls.Controls.Add(lblVersion);
            pnlControls.Controls.Add(btnClose);

            pnlBanner.Controls.Add(picAppIcon);
            pnlBanner.Controls.Add(picBannerImage);
            pnlBanner.Controls.Add(lblProject);
            pnlBanner.Controls.Add(lblBrief);

            // Add all panels to form
            this.Controls.Add(pnlBanner);
            this.Controls.Add(pnlAbout);
            this.Controls.Add(pnlControls);
        }
    }
}
