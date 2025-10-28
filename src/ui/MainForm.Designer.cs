using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuickWinstall.Lib;
using QuickWinstall.Config;

namespace QuickWinstall.Main
{
    partial class MainForm
    {
        #region Component Declarations

        private System.ComponentModel.IContainer components = null;

        // UI Components
        private Panel pnlBanner;
        private PictureBox picLogo;
        private Label lblBannerTitle;
        private Button btnExpandAll;
        private Button btnCollapseAll;
        
        private Panel pnlConfigSection;
        
        private Panel pnlControlPanel;
        private Button btnSettings;
        private Button btnClear;
        private Button btnPreset;
        private Button btnGenerate;
        private Button btnCancel;
        
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatusPrefix;
        private ToolStripStatusLabel lblStatus;

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            // Form settings
            this.SuspendLayout();
            this.Text = lang.GetString("mainForm.title");
            this.ClientSize = new Size(ui.GetValue("forms.mainForm.width", 800), ui.GetValue("forms.mainForm.height", 600));
            this.MinimumSize = new Size(ui.GetValue("forms.mainForm.minWidth", 600), ui.GetValue("forms.mainForm.minHeight", 400));
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = theme.GetFont("normal");
            
            // Try to load application icon
            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "icons", "app256.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load application icon: {ex.Message}");
            }

            // Banner Panel
            this.pnlBanner = new Panel();
            this.pnlBanner.Location = new Point(0, 0);
            this.pnlBanner.Size = new Size(this.ClientSize.Width, ui.BannerHeight);
            this.pnlBanner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Logo
            this.picLogo = new PictureBox();
            this.picLogo.Location = new Point(ui.GlobalTabX, (ui.BannerHeight - ui.GlobalBtnBox) / 2);
            this.picLogo.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.picLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            
            // Try to load logo
            try
            {
                this.picLogo.Image = iconMgr.GetIconAsImage("windows11256", theme.IsDarkTheme, ui.GlobalBtnBox);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load logo: {ex.Message}");
            }

            // Banner Title
            this.lblBannerTitle = new Label();
            this.lblBannerTitle.Location = new Point(picLogo.Right + ui.GlobalSpacingX, (ui.BannerHeight - ui.GlobalLabelHeight) / 2);
            this.lblBannerTitle.Size = new Size(400, ui.GlobalLabelHeight);
            this.lblBannerTitle.Text = lang.GetString("mainForm.banner.title");
            this.lblBannerTitle.Font = theme.GetFont("header");
            this.lblBannerTitle.ForeColor = theme.GetFontColor("header");
            this.lblBannerTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Expand All Button
            this.btnExpandAll = _themeManager.CreateRoundedButton();
            this.btnExpandAll.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnExpandAll.Image = iconMgr.GetIconAsImage("add", theme.IsDarkTheme, ui.GlobalIconSize);
            this.btnExpandAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnExpandAll.Click += new EventHandler(this.BtnExpandAll_Click);
            tooltips.SetToolTip(this.btnExpandAll, "tooltips.mainForm.expandAll");

            // Collapse All Button
            this.btnCollapseAll = _themeManager.CreateRoundedButton();
            this.btnCollapseAll.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnCollapseAll.Image = iconMgr.GetIconAsImage("remove", theme.IsDarkTheme, ui.GlobalIconSize);
            this.btnCollapseAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnCollapseAll.Click += new EventHandler(this.BtnCollapseAll_Click);
            tooltips.SetToolTip(this.btnCollapseAll, "tooltips.mainForm.collapseAll");

            // Position buttons from right
            this.btnCollapseAll.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnBox, (ui.BannerHeight - ui.GlobalBtnBox) / 2);
            this.btnExpandAll.Location = new Point(btnCollapseAll.Left - ui.GlobalSpacingX - ui.GlobalBtnBox, (ui.BannerHeight - ui.GlobalBtnBox) / 2);

            // Config Section Panel
            this.pnlConfigSection = new Panel();
            this.pnlConfigSection.Location = new Point(0, pnlBanner.Bottom);
            this.pnlConfigSection.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - ui.BannerHeight - ui.ControlPanelHeight - ui.StatusBarHeight);
            this.pnlConfigSection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.pnlConfigSection.AutoScroll = true;

            // Initialize General Config Section via GeneralConfig class
            _pnlGeneralConfig = _generalConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections,  // Pass reposition callback
                this  // Pass parent form for navigation
            );
            this.pnlConfigSection.Controls.Add(_pnlGeneralConfig);

            // Initialize Language & Region Config Section via LangRegConfig class
            _pnlLangRegConfig = _langRegConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections  // Pass reposition callback
            );
            _pnlLangRegConfig.Location = new Point(0, _pnlGeneralConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlLangRegConfig);

            // Initialize User Account Config Section
            _pnlUserAccConfig = _userAccConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections
            );
            _pnlUserAccConfig.Location = new Point(0, _pnlLangRegConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlUserAccConfig);

            // Initialize OOBE Config Section
            _pnlOOBEConfig = _oobeConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections
            );
            _pnlOOBEConfig.Location = new Point(0, _pnlUserAccConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlOOBEConfig);

            // Initialize Personal Config Section
            _pnlPersonalConfig = _personalConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections
            );
            _pnlPersonalConfig.Location = new Point(0, _pnlOOBEConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlPersonalConfig);

            // Initialize Disk & Partition Config Section
            _pnlDiskPartConfig = _diskPartConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections
            );
            _pnlDiskPartConfig.Location = new Point(0, _pnlPersonalConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlDiskPartConfig);

            // Initialize Bypass Config Section
            _pnlBypassConfig = _bypassConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections
            );
            _pnlBypassConfig.Location = new Point(0, _pnlDiskPartConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlBypassConfig);

            // Initialize App Config Section
            _pnlAppConfig = _appConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.RepositionSections
            );
            _pnlAppConfig.Location = new Point(0, _pnlBypassConfig.Bottom);
            this.pnlConfigSection.Controls.Add(_pnlAppConfig);

            // Control Panel
            this.pnlControlPanel = new Panel();
            this.pnlControlPanel.Location = new Point(0, this.ClientSize.Height - ui.ControlPanelHeight - ui.StatusBarHeight);
            this.pnlControlPanel.Size = new Size(this.ClientSize.Width, ui.ControlPanelHeight);
            this.pnlControlPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            int btnY = (ui.ControlPanelHeight - ui.GlobalBtnHeight) / 2;

            // Settings Button
            this.btnSettings = _themeManager.CreateRoundedButton();
            this.btnSettings.Location = new Point(ui.GlobalSpacingX, btnY);
            this.btnSettings.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnSettings.Text = lang.GetString("mainForm.buttons.settings");
            this.btnSettings.Click += new EventHandler(this.BtnSettings_Click);
            tooltips.SetToolTip(this.btnSettings, "tooltips.mainForm.settings");

            // Clear Button
            this.btnClear = _themeManager.CreateRoundedButton();
            this.btnClear.Location = new Point(btnSettings.Right + ui.GlobalSpacingX, btnY);
            this.btnClear.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnClear.Text = lang.GetString("mainForm.buttons.clear");
            this.btnClear.Click += new EventHandler(this.BtnClear_Click);
            tooltips.SetToolTip(this.btnClear, "tooltips.mainForm.clear");

                        // Preset Button
            this.btnPreset = _themeManager.CreateRoundedButton();
            this.btnPreset.Location = new Point(btnClear.Right + ui.GlobalSpacingX, btnY);
            this.btnPreset.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnPreset.Text = lang.GetString("mainForm.buttons.preset");
            this.btnPreset.Click += new EventHandler(this.BtnPreset_Click);
            tooltips.SetToolTip(this.btnPreset, "tooltips.mainForm.preset");

            // Cancel Button
            this.btnCancel = _themeManager.CreateRoundedButton();
            this.btnCancel.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnCancel.Text = lang.GetString("mainForm.buttons.cancel");
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);
            tooltips.SetToolTip(this.btnCancel, "tooltips.mainForm.cancel");

            this.btnGenerate = _themeManager.CreateRoundedButton();
            this.btnGenerate.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnGenerate.Text = lang.GetString("mainForm.buttons.generate");
            this.btnGenerate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnGenerate.Click += new EventHandler(this.BtnGenerate_Click);
            tooltips.SetToolTip(this.btnGenerate, "tooltips.mainForm.generate");

            // Position buttons from right
            this.btnCancel.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnWidth, btnY);
            this.btnGenerate.Location = new Point(btnCancel.Left - ui.GlobalSpacingX - ui.GlobalBtnWidth, btnY);

            // Add buttons to Control Panel
            this.pnlControlPanel.Controls.Add(this.btnSettings);
            this.pnlControlPanel.Controls.Add(this.btnClear);
            this.pnlControlPanel.Controls.Add(this.btnPreset);
            this.pnlControlPanel.Controls.Add(this.btnGenerate);
            this.pnlControlPanel.Controls.Add(this.btnCancel);

            // Status Strip
            this.statusStrip = new StatusStrip();
            this.statusStrip.Dock = DockStyle.Bottom;
            this.statusStrip.Height = ui.StatusBarHeight;

            this.lblStatusPrefix = new ToolStripStatusLabel();
            this.lblStatusPrefix.Text = lang.GetString("mainForm.status.prefix");
            this.lblStatusPrefix.Font = theme.GetFont("normal");
            this.lblStatusPrefix.ForeColor = theme.GetFontColor("normal");

            this.lblStatus = new ToolStripStatusLabel();
            this.lblStatus.Text = lang.GetString("mainForm.status.ready");
            this.lblStatus.Font = theme.GetFont("normal");
            this.lblStatus.ForeColor = theme.GetFontColor("success");
            this.lblStatus.Spring = true; // Fill remaining width
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            this.statusStrip.Items.Add(this.lblStatusPrefix);
            this.statusStrip.Items.Add(this.lblStatus);

            // Add all to Form
            this.pnlBanner.Controls.Add(this.picLogo);
            this.pnlBanner.Controls.Add(this.lblBannerTitle);
            this.pnlBanner.Controls.Add(this.btnExpandAll);
            this.pnlBanner.Controls.Add(this.btnCollapseAll);
            
            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.pnlConfigSection);
            this.Controls.Add(this.pnlControlPanel);
            this.Controls.Add(this.statusStrip);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Helper Methods

        #endregion
    }
}

