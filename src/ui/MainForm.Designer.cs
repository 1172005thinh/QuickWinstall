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
        private Button btnToggleAll;
        private Button btnToggleLock;
        private Button btnModeToggle;
        
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
            this.picLogo.Cursor = Cursors.Hand;
            this.picLogo.Click += new EventHandler(this.picLogo_Click);
            tooltips.SetToolTip(this.picLogo, "tooltips.mainForm.logoClick", lang.GetString("tooltips.mainForm.logoClick"));
            
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
            this.lblBannerTitle.Size = new Size(ui.GlobalLabelWidth * 3, ui.GlobalLabelHeight);
            this.lblBannerTitle.Text = lang.GetString("mainForm.banner.title");
            this.lblBannerTitle.Font = theme.GetFont("header");
            this.lblBannerTitle.ForeColor = theme.GetFontColor("header");
            this.lblBannerTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Toggle All Button (Expand/Collapse)
            this.btnToggleAll = _themeManager.CreateRoundedButton();
            this.btnToggleAll.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnToggleAll.Image = iconMgr.GetIconAsImage("all_collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            this.btnToggleAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnToggleAll.Click += new EventHandler(this.BtnToggleAll_Click);
            tooltips.SetToolTip(this.btnToggleAll, "tooltips.mainForm.collapseAll");

            // Toggle Lock Button (Lock/Unlock All Sections)
            this.btnToggleLock = _themeManager.CreateRoundedButton();
            this.btnToggleLock.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnToggleLock.Image = iconMgr.GetIconAsImage("lock", theme.IsDarkTheme, ui.GlobalIconSize);
            this.btnToggleLock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnToggleLock.Click += new EventHandler(this.BtnToggleLock_Click);
            tooltips.SetToolTip(this.btnToggleLock, "tooltips.mainForm.lockAll");

            // Mode Toggle Button (New Installation / Upgrade Only)
            this.btnModeToggle = _themeManager.CreateRoundedButton();
            this.btnModeToggle.Size = new Size((int)(ui.GlobalBtnWidth * 1.2), ui.GlobalBtnHeight);
            this.btnModeToggle.Text = lang.GetString("mainForm.buttons.upgradeOnly");
            this.btnModeToggle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnModeToggle.Click += new EventHandler(this.BtnModeToggle_Click);
            tooltips.SetToolTip(this.btnModeToggle, "tooltips.mainForm.upgradeOnly");

            // Position buttons from right
            this.btnToggleAll.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnBox, (ui.BannerHeight - ui.GlobalBtnBox) / 2);
            this.btnToggleLock.Location = new Point(btnToggleAll.Left - ui.GlobalSpacingX - ui.GlobalBtnBox, (ui.BannerHeight - ui.GlobalBtnBox) / 2);
            this.btnModeToggle.Location = new Point(btnToggleLock.Left - ui.GlobalSpacingX - (int)(ui.GlobalBtnWidth * 1.2), (ui.BannerHeight - ui.GlobalBtnHeight) / 2);

            // Config Section Panel
            this.pnlConfigSection = new Panel();
            this.pnlConfigSection.Location = new Point(0, pnlBanner.Bottom);
            this.pnlConfigSection.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - ui.BannerHeight - ui.ControlPanelHeight - ui.StatusBarHeight);
            this.pnlConfigSection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.pnlConfigSection.AutoScroll = true;

            // Initialize sections in REVERSE order (bottom to top) with DockStyle.Top
            // This ensures they stack correctly: General at top, App at bottom
            
            // Initialize App Config Section (add first, appears at bottom)
            _pnlAppConfig = _appConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton
            );
            _pnlAppConfig.Dock = DockStyle.Top;

            // Initialize Personal Config Section
            _pnlPersonalConfig = _personalConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton
            );
            _pnlPersonalConfig.Dock = DockStyle.Top;
            
            // Initialize OOBE Config Section
            _pnlOOBEConfig = _oobeConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton,
                this.CheckAndUpdateLockUnlockButton
            );
            _pnlOOBEConfig.Dock = DockStyle.Top;

            // Initialize User Account Config Section
            _pnlUserAccConfig = _userAccConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton,
                this.CheckAndUpdateLockUnlockButton
            );
            _pnlUserAccConfig.Dock = DockStyle.Top;

            // Initialize Disk & Partition Config Section
            _pnlDiskPartConfig = _diskPartConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton,
                this.CheckAndUpdateLockUnlockButton
            );
            _pnlDiskPartConfig.Dock = DockStyle.Top;

            // Initialize Bypass Config Section
            _pnlBypassConfig = _bypassConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton,
                this.CheckAndUpdateLockUnlockButton
            );
            _pnlBypassConfig.Dock = DockStyle.Top;

            // Initialize Language & Region Config Section
            _pnlLangRegConfig = _langRegConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton,
                this.CheckAndUpdateLockUnlockButton
            );
            _pnlLangRegConfig.Dock = DockStyle.Top;

            // Initialize General Config Section (add last, appears at top)
            _pnlGeneralConfig = _generalConfig.InitializeUI(
                this.pnlConfigSection,
                this.OnConfigChanged,
                _themeManager.CreateRoundedButton,
                this.CheckAndUpdateExpandCollapseButton,
                this.CheckAndUpdateLockUnlockButton,
                this
            );
            _pnlGeneralConfig.Dock = DockStyle.Top;
            

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

            // Generate Button
            this.btnGenerate = _themeManager.CreateRoundedButton();
            this.btnGenerate.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnGenerate.Text = lang.GetString("mainForm.buttons.generate");
            this.btnGenerate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnGenerate.Click += new EventHandler(this.BtnGenerate_Click);
            tooltips.SetToolTip(this.btnGenerate, "tooltips.mainForm.generate");

            // Position buttons from right
            this.btnCancel.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnWidth, btnY);
            this.btnGenerate.Location = new Point(btnCancel.Left - ui.GlobalSpacingX - ui.GlobalBtnWidth, btnY);

            // Add buttons to Control Panel (moved to bottom)

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

            // Add all child controls (centralized)
            // Control panel buttons
            this.pnlControlPanel.Controls.Add(this.btnSettings);
            this.pnlControlPanel.Controls.Add(this.btnClear);
            this.pnlControlPanel.Controls.Add(this.btnPreset);
            this.pnlControlPanel.Controls.Add(this.btnGenerate);
            this.pnlControlPanel.Controls.Add(this.btnCancel);

            // Config section panels (respect reverse stacking order)
            this.pnlConfigSection.Controls.Add(_pnlAppConfig);
            this.pnlConfigSection.Controls.Add(_pnlPersonalConfig);
            this.pnlConfigSection.Controls.Add(_pnlOOBEConfig);
            this.pnlConfigSection.Controls.Add(_pnlUserAccConfig);
            this.pnlConfigSection.Controls.Add(_pnlDiskPartConfig);
            this.pnlConfigSection.Controls.Add(_pnlBypassConfig);
            this.pnlConfigSection.Controls.Add(_pnlLangRegConfig);
            this.pnlConfigSection.Controls.Add(_pnlGeneralConfig);

            // Banner children
            this.pnlBanner.Controls.Add(this.picLogo);
            this.pnlBanner.Controls.Add(this.btnToggleAll);
            this.pnlBanner.Controls.Add(this.btnToggleLock);
            this.pnlBanner.Controls.Add(this.btnModeToggle);
            this.pnlBanner.Controls.Add(this.lblBannerTitle);

            // Add root panels to the form
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

