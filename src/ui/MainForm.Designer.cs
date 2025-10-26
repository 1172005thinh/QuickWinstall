using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Main
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // UI Components
        private Panel pnlBanner;
        private PictureBox picLogo;
        private Label lblBannerTitle;
        private Button btnExpandAll;
        private Button btnCollapseAll;
        
        private Panel pnlConfigSection;
        private Panel pnlGeneralConfig;
        private Button btnGeneralConfigToggle;
        private Label lblGeneralConfigTitle;
        private Panel pnlGeneralConfigSeparator;
        private Panel pnlGeneralConfigContent;
        
        // GeneralConfig controls
        private Label lblWindowsEdition;
        private ComboBox cmbWindowsEdition;
        private Label lblProductKey;
        private TextBox txtProductKey1;
        private Label lblHyphen1;
        private TextBox txtProductKey2;
        private Label lblHyphen2;
        private TextBox txtProductKey3;
        private Label lblHyphen3;
        private TextBox txtProductKey4;
        private Label lblHyphen4;
        private TextBox txtProductKey5;
        private Label lblCPUArch;
        private ComboBox cmbCPUArch;
        
        private Panel pnlControlPanel;
        private Button btnSettings;
        private Button btnClear;
        private Button btnPreset;
        private Button btnGenerate;
        private Button btnCancel;
        
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatusPrefix;
        private ToolStripStatusLabel lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

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
            this.ClientSize = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
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
                this.picLogo.Image = iconMgr.GetIconAsImage("windows11", theme.IsDarkTheme, ui.GlobalBtnBox);
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
            this.btnExpandAll = CreateRoundedButton();
            this.btnExpandAll.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnExpandAll.Text = "+";
            this.btnExpandAll.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            this.btnExpandAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnExpandAll.Click += new EventHandler(this.BtnExpandAll_Click);
            tooltips.SetToolTip(this.btnExpandAll, "tooltips.mainForm.expandAll");

            // Collapse All Button
            this.btnCollapseAll = CreateRoundedButton();
            this.btnCollapseAll.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnCollapseAll.Text = "−";
            this.btnCollapseAll.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
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

            // General Config Panel
            int contentHeight = ui.GetSectionValue("generalConfig", "contentHeight", 200);
            this.pnlGeneralConfig = new Panel();
            this.pnlGeneralConfig.Location = new Point(0, 0);
            this.pnlGeneralConfig.Size = new Size(pnlConfigSection.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            this.pnlGeneralConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // General Config Toggle Button
            this.btnGeneralConfigToggle = CreateRoundedButton();
            this.btnGeneralConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            this.btnGeneralConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnGeneralConfigToggle.Text = "−";
            this.btnGeneralConfigToggle.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            this.btnGeneralConfigToggle.Click += new EventHandler(this.BtnGeneralConfigToggle_Click);
            tooltips.SetToolTip(this.btnGeneralConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.general"));

            // General Config Title
            this.lblGeneralConfigTitle = new Label();
            this.lblGeneralConfigTitle.Location = new Point(btnGeneralConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            this.lblGeneralConfigTitle.Size = new Size(300, ui.GlobalLabelHeight);
            this.lblGeneralConfigTitle.Text = lang.GetString("mainForm.sections.general");
            this.lblGeneralConfigTitle.Font = theme.GetFont("subheader");
            this.lblGeneralConfigTitle.ForeColor = theme.GetFontColor("subheader");
            this.lblGeneralConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblGeneralConfigTitle.Cursor = Cursors.Hand;
            this.lblGeneralConfigTitle.Click += new EventHandler(this.BtnGeneralConfigToggle_Click);

            // Line Separator
            this.pnlGeneralConfigSeparator = new Panel();
            this.pnlGeneralConfigSeparator.Location = new Point(ui.GlobalTabX, btnGeneralConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            this.pnlGeneralConfigSeparator.Size = new Size(pnlGeneralConfig.Width - 2 * ui.GlobalTabX, 1);
            this.pnlGeneralConfigSeparator.BackColor = Color.Black;

            // General Config Content Panel
            this.pnlGeneralConfigContent = new Panel();
            this.pnlGeneralConfigContent.Location = new Point(0, pnlGeneralConfigSeparator.Bottom + ui.GlobalSpacingY / 2);
            this.pnlGeneralConfigContent.Size = new Size(pnlGeneralConfig.Width, contentHeight);
            this.pnlGeneralConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int currentY = ui.GlobalSpacingY;

            // Windows Edition
            this.lblWindowsEdition = new Label();
            this.lblWindowsEdition.Location = new Point(labelX, currentY);
            this.lblWindowsEdition.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblWindowsEdition.Text = lang.GetString("generalConfig.windowsEdition.label");
            this.lblWindowsEdition.Font = theme.GetFont("normal");
            this.lblWindowsEdition.TextAlign = ContentAlignment.MiddleLeft;

            this.cmbWindowsEdition = new ComboBox();
            this.cmbWindowsEdition.Location = new Point(inputX, currentY);
            this.cmbWindowsEdition.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            this.cmbWindowsEdition.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbWindowsEdition.Items.AddRange(new object[] {
                lang.GetString("generalConfig.windowsEdition.options.selectOne"),
                lang.GetString("generalConfig.windowsEdition.options.home"),
                lang.GetString("generalConfig.windowsEdition.options.pro"),
                lang.GetString("generalConfig.windowsEdition.options.education"),
                lang.GetString("generalConfig.windowsEdition.options.enterprise")
            });
            this.cmbWindowsEdition.SelectedIndex = 0;
            this.cmbWindowsEdition.SelectedIndexChanged += new EventHandler(this.OnConfigChanged);
            tooltips.SetToolTip(this.cmbWindowsEdition, "tooltips.generalConfig.windowsEdition");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Product Key
            this.lblProductKey = new Label();
            this.lblProductKey.Location = new Point(labelX, currentY);
            this.lblProductKey.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblProductKey.Text = lang.GetString("generalConfig.productKey.label");
            this.lblProductKey.Font = theme.GetFont("normal");
            this.lblProductKey.TextAlign = ContentAlignment.MiddleLeft;

            int pkWidth = (ui.GlobalInputWidth - 4 * ui.GlobalSpacingX) / 5;
            int pkX = inputX;

            this.txtProductKey1 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            this.txtProductKey1.Location = new Point(pkX, currentY);
            this.txtProductKey1.Tag = 1; // For auto-focus logic
            tooltips.SetToolTip(this.txtProductKey1, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            this.lblHyphen1 = new Label();
            this.lblHyphen1.Location = new Point(pkX, currentY);
            this.lblHyphen1.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            this.lblHyphen1.Text = "-";
            this.lblHyphen1.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            this.txtProductKey2 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            this.txtProductKey2.Location = new Point(pkX, currentY);
            this.txtProductKey2.Tag = 2;
            tooltips.SetToolTip(this.txtProductKey2, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            this.lblHyphen2 = new Label();
            this.lblHyphen2.Location = new Point(pkX, currentY);
            this.lblHyphen2.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            this.lblHyphen2.Text = "-";
            this.lblHyphen2.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            this.txtProductKey3 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            this.txtProductKey3.Location = new Point(pkX, currentY);
            this.txtProductKey3.Tag = 3;
            tooltips.SetToolTip(this.txtProductKey3, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            this.lblHyphen3 = new Label();
            this.lblHyphen3.Location = new Point(pkX, currentY);
            this.lblHyphen3.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            this.lblHyphen3.Text = "-";
            this.lblHyphen3.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            this.txtProductKey4 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            this.txtProductKey4.Location = new Point(pkX, currentY);
            this.txtProductKey4.Tag = 4;
            tooltips.SetToolTip(this.txtProductKey4, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            this.lblHyphen4 = new Label();
            this.lblHyphen4.Location = new Point(pkX, currentY);
            this.lblHyphen4.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            this.lblHyphen4.Text = "-";
            this.lblHyphen4.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            this.txtProductKey5 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            this.txtProductKey5.Location = new Point(pkX, currentY);
            this.txtProductKey5.Tag = 5;
            tooltips.SetToolTip(this.txtProductKey5, "tooltips.generalConfig.productKey");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // CPU Architecture
            this.lblCPUArch = new Label();
            this.lblCPUArch.Location = new Point(labelX, currentY);
            this.lblCPUArch.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblCPUArch.Text = lang.GetString("generalConfig.cpuArch.label");
            this.lblCPUArch.Font = theme.GetFont("normal");
            this.lblCPUArch.TextAlign = ContentAlignment.MiddleLeft;

            this.cmbCPUArch = new ComboBox();
            this.cmbCPUArch.Location = new Point(inputX, currentY);
            this.cmbCPUArch.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            this.cmbCPUArch.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCPUArch.Items.AddRange(new object[] {
                lang.GetString("generalConfig.cpuArch.options.selectOne"),
                lang.GetString("generalConfig.cpuArch.options.x64"),
                lang.GetString("generalConfig.cpuArch.options.arm64")
            });
            this.cmbCPUArch.SelectedIndex = 0;
            this.cmbCPUArch.SelectedIndexChanged += new EventHandler(this.OnConfigChanged);
            tooltips.SetToolTip(this.cmbCPUArch, "tooltips.generalConfig.cpuArch");

            // Add controls to General Config Content
            this.pnlGeneralConfigContent.Controls.Add(this.lblWindowsEdition);
            this.pnlGeneralConfigContent.Controls.Add(this.cmbWindowsEdition);
            this.pnlGeneralConfigContent.Controls.Add(this.lblProductKey);
            this.pnlGeneralConfigContent.Controls.Add(this.txtProductKey1);
            this.pnlGeneralConfigContent.Controls.Add(this.lblHyphen1);
            this.pnlGeneralConfigContent.Controls.Add(this.txtProductKey2);
            this.pnlGeneralConfigContent.Controls.Add(this.lblHyphen2);
            this.pnlGeneralConfigContent.Controls.Add(this.txtProductKey3);
            this.pnlGeneralConfigContent.Controls.Add(this.lblHyphen3);
            this.pnlGeneralConfigContent.Controls.Add(this.txtProductKey4);
            this.pnlGeneralConfigContent.Controls.Add(this.lblHyphen4);
            this.pnlGeneralConfigContent.Controls.Add(this.txtProductKey5);
            this.pnlGeneralConfigContent.Controls.Add(this.lblCPUArch);
            this.pnlGeneralConfigContent.Controls.Add(this.cmbCPUArch);

            // Add controls to General Config Panel
            this.pnlGeneralConfig.Controls.Add(this.btnGeneralConfigToggle);
            this.pnlGeneralConfig.Controls.Add(this.lblGeneralConfigTitle);
            this.pnlGeneralConfig.Controls.Add(this.pnlGeneralConfigSeparator);
            this.pnlGeneralConfig.Controls.Add(this.pnlGeneralConfigContent);

            // Add General Config to Config Section
            this.pnlConfigSection.Controls.Add(this.pnlGeneralConfig);

            // Control Panel
            this.pnlControlPanel = new Panel();
            this.pnlControlPanel.Location = new Point(0, this.ClientSize.Height - ui.ControlPanelHeight - ui.StatusBarHeight);
            this.pnlControlPanel.Size = new Size(this.ClientSize.Width, ui.ControlPanelHeight);
            this.pnlControlPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            int btnY = (ui.ControlPanelHeight - ui.GlobalBtnHeight) / 2;

            this.btnSettings = CreateRoundedButton();
            this.btnSettings.Location = new Point(ui.GlobalSpacingX, btnY);
            this.btnSettings.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnSettings.Text = lang.GetString("mainForm.buttons.settings");
            this.btnSettings.Click += new EventHandler(this.BtnSettings_Click);
            tooltips.SetToolTip(this.btnSettings, "tooltips.mainForm.settings");

            this.btnClear = CreateRoundedButton();
            this.btnClear.Location = new Point(btnSettings.Right + ui.GlobalSpacingX, btnY);
            this.btnClear.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnClear.Text = lang.GetString("mainForm.buttons.clear");
            this.btnClear.Click += new EventHandler(this.BtnClear_Click);
            tooltips.SetToolTip(this.btnClear, "tooltips.mainForm.clear");

            this.btnPreset = CreateRoundedButton();
            this.btnPreset.Location = new Point(btnClear.Right + ui.GlobalSpacingX, btnY);
            this.btnPreset.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnPreset.Text = lang.GetString("mainForm.buttons.preset");
            this.btnPreset.Click += new EventHandler(this.BtnPreset_Click);
            tooltips.SetToolTip(this.btnPreset, "tooltips.mainForm.preset");

            this.btnCancel = CreateRoundedButton();
            this.btnCancel.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnCancel.Text = lang.GetString("mainForm.buttons.cancel");
            this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);
            tooltips.SetToolTip(this.btnCancel, "tooltips.mainForm.cancel");

            this.btnGenerate = CreateRoundedButton();
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
            this.statusStrip.Location = new Point(0, this.ClientSize.Height - ui.StatusBarHeight);
            this.statusStrip.Size = new Size(this.ClientSize.Width, ui.StatusBarHeight);
            this.statusStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.lblStatusPrefix = new ToolStripStatusLabel();
            this.lblStatusPrefix.Text = lang.GetString("mainForm.status.prefix");
            this.lblStatusPrefix.Font = theme.GetFont("normal");

            this.lblStatus = new ToolStripStatusLabel();
            this.lblStatus.Text = lang.GetString("mainForm.status.ready");
            this.lblStatus.Font = theme.GetFont("normal");
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

        private Button CreateRoundedButton()
        {
            Button btn = new Button();
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = SystemColors.Control;
            btn.Cursor = Cursors.Hand;
            
            // Create rounded region
            btn.Paint += (sender, e) =>
            {
                GraphicsPath path = new GraphicsPath();
                int radius = 8;
                Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                btn.Region = new Region(path);
            };
            
            return btn;
        }

        private TextBox CreateProductKeyTextBox(int width, int height)
        {
            TextBox txt = new TextBox();
            txt.Size = new Size(width, height);
            txt.MaxLength = 5;
            txt.CharacterCasing = CharacterCasing.Upper;
            txt.TextAlign = HorizontalAlignment.Center;
            txt.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            
            // Handle placeholder
            txt.ForeColor = Color.Gray;
            txt.Text = "XXXXX";
            
            txt.Enter += (sender, e) =>
            {
                if (txt.Text == "XXXXX" && txt.ForeColor == Color.Gray)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };
            
            txt.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.ForeColor = Color.Gray;
                    txt.Text = "XXXXX";
                }
            };
            
            // Auto-move to next textbox
            txt.TextChanged += (sender, e) =>
            {
                if (txt.Text.Length == txt.MaxLength && txt.Text != "XXXXX")
                {
                    int currentTag = (int)(txt.Tag ?? 0);
                    if (currentTag < 5)
                    {
                        // Move to next ProductKey textbox
                        Control nextControl = FindProductKeyTextBox(currentTag + 1);
                        if (nextControl != null)
                        {
                            nextControl.Focus();
                        }
                    }
                }
                
                // Call OnConfigChanged
                this.OnConfigChanged(sender, e);
            };
            
            return txt;
        }

        private Control FindProductKeyTextBox(int tag)
        {
            switch (tag)
            {
                case 1: return txtProductKey1;
                case 2: return txtProductKey2;
                case 3: return txtProductKey3;
                case 4: return txtProductKey4;
                case 5: return txtProductKey5;
                default: return null;
            }
        }
    }
}
