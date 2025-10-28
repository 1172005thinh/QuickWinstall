using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        // UI Controls
        private Panel pnlBanner;
        private Panel pnlSettings;
        private Panel pnlControls;
        
        // Banner
        private Label lblBannerTitle;
        private Button btnResetToDefault;
        
        // Settings
        private Label lblLanguage;
        private ComboBox cmbLanguage;
        
        private Label lblTheme;
        private ComboBox cmbTheme;
        
        private Label lblSavePath;
        private TextBox txtSavePath;
        private Button btnBrowse;
        
        private Label lblAutoSave;
        private Panel toggleAutoSave;
        private Label lblAutoSaveState;
        
        private Label lblLoadLast;
        private Panel toggleLoadLast;
        private Label lblLoadLastState;
        
        // Control buttons
        private Button btnAbout;
        private Button btnHelp;
        private Button btnCancel;
        private Button btnSave;

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

            // Banner height matching MainForm pattern
            int bannerHeight = (int)(ui.BannerHeight * 0.6);
            
            // Form settings
            this.Text = "Settings";
            this.Width = ui.GetValue("forms.settingsForm.width", 600);
            this.Height = ui.GetValue("forms.settingsForm.height", 450);
            this.FormBorderStyle = ui.SettingsFormResizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            
            // Banner Panel - matching MainForm pattern without DockStyle.Top
            this.pnlBanner = new Panel();
            this.pnlBanner.Location = new Point(0, 0);
            this.pnlBanner.Size = new Size(this.ClientSize.Width, bannerHeight);
            this.pnlBanner.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            
            // Banner Title
            this.lblBannerTitle = new Label();
            this.lblBannerTitle.Text = lang.GetString("settingsForm.banner.title");
            this.lblBannerTitle.Font = theme.GetFont("header");
            this.lblBannerTitle.AutoSize = false;
            this.lblBannerTitle.Size = new Size(400, ui.GlobalLabelHeight);
            this.lblBannerTitle.Location = new Point(ui.GlobalTabX, (bannerHeight - ui.GlobalLabelHeight) / 2);
            this.lblBannerTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.pnlBanner.Controls.Add(this.lblBannerTitle);
            
            // Reset Button - positioned like CollapseAll button in MainForm
            this.btnResetToDefault = theme.CreateRoundedButton();
            this.btnResetToDefault.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnResetToDefault.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnBox, (bannerHeight - ui.GlobalBtnBox) / 2);
            this.btnResetToDefault.ImageAlign = ContentAlignment.MiddleCenter;
            this.btnResetToDefault.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnResetToDefault.Click += btnResetToDefault_Click;
            tooltips.SetToolTip(this.btnResetToDefault, "settingsForm.tooltips.reset");
            
            // Load icon for Reset button
            try
            {
                this.btnResetToDefault.Image = iconMgr.GetIconAsImage("reset", theme.IsDarkTheme, ui.GlobalIconSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load reset icon: {ex.Message}");
            }
            
            this.pnlBanner.Controls.Add(this.btnResetToDefault);
            
            this.Controls.Add(this.pnlBanner);
            
            // Settings Panel - positioned below banner like MainForm's config section
            this.pnlSettings = new Panel();
            this.pnlSettings.Location = new Point(0, pnlBanner.Bottom);
            this.pnlSettings.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - bannerHeight - 60);
            this.pnlSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.pnlSettings.AutoScroll = true;
            
            int currentY = ui.GlobalSpacingY;
            int labelX = ui.GlobalTabX;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int inputWidth = (int)(ui.GlobalInputWidth * 0.75);
            
            // Language
            this.lblLanguage = new Label();
            this.lblLanguage.Text = lang.GetString("settingsForm.language.label");
            this.lblLanguage.Font = theme.GetFont("normal");
            this.lblLanguage.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblLanguage.Location = new Point(labelX, currentY);
            this.lblLanguage.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(this.lblLanguage, "settingsForm.tooltips.language");
            this.pnlSettings.Controls.Add(this.lblLanguage);
            
            this.cmbLanguage = new ComboBox();
            this.cmbLanguage.Size = new Size(inputWidth, ui.GlobalInputHeight);
            this.cmbLanguage.Location = new Point(inputX, currentY);
            this.cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbLanguage.Font = theme.GetFont("normal");
            tooltips.SetToolTip(this.cmbLanguage, "settingsForm.tooltips.language");
            this.pnlSettings.Controls.Add(this.cmbLanguage);
            
            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;
            
            // Theme
            this.lblTheme = new Label();
            this.lblTheme.Text = lang.GetString("settingsForm.theme.label");
            this.lblTheme.Font = theme.GetFont("normal");
            this.lblTheme.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblTheme.Location = new Point(labelX, currentY);
            this.lblTheme.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(this.lblTheme, "settingsForm.tooltips.theme");
            this.pnlSettings.Controls.Add(this.lblTheme);
            
            this.cmbTheme = new ComboBox();
            this.cmbTheme.Size = new Size(inputWidth, ui.GlobalInputHeight);
            this.cmbTheme.Location = new Point(inputX, currentY);
            this.cmbTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbTheme.Font = theme.GetFont("normal");
            tooltips.SetToolTip(this.cmbTheme, "settingsForm.tooltips.theme");
            this.pnlSettings.Controls.Add(this.cmbTheme);
            
            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;
            
            // Save Path
            this.lblSavePath = new Label();
            this.lblSavePath.Text = lang.GetString("settingsForm.savePath.label");
            this.lblSavePath.Font = theme.GetFont("normal");
            this.lblSavePath.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblSavePath.Location = new Point(labelX, currentY);
            this.lblSavePath.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(this.lblSavePath, "settingsForm.tooltips.savePath");
            this.pnlSettings.Controls.Add(this.lblSavePath);
            
            int textBoxWidth = inputWidth - ui.GlobalBtnBox - ui.GlobalSpacingX / 2;
            this.txtSavePath = new TextBox();
            this.txtSavePath.Size = new Size(textBoxWidth, ui.GlobalInputHeight);
            this.txtSavePath.Location = new Point(inputX, currentY);
            this.txtSavePath.Font = theme.GetFont("normal");
            tooltips.SetToolTip(this.txtSavePath, "settingsForm.tooltips.savePath");
            this.pnlSettings.Controls.Add(this.txtSavePath);
            
            this.btnBrowse = theme.CreateRoundedButton();
            this.btnBrowse.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            this.btnBrowse.Location = new Point(inputX + textBoxWidth + ui.GlobalSpacingX / 2, currentY - (this.btnBrowse.Height - ui.GlobalInputHeight) / 2);
            this.btnBrowse.ImageAlign = ContentAlignment.MiddleCenter;
            this.btnBrowse.Click += btnBrowse_Click;
            tooltips.SetToolTip(this.btnBrowse, "settingsForm.tooltips.browse");
            this.pnlSettings.Controls.Add(this.btnBrowse);
            
            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;
            
            // Auto Save Last Config
            this.lblAutoSave = new Label();
            this.lblAutoSave.Text = lang.GetString("settingsForm.autoSave.label");
            this.lblAutoSave.Font = theme.GetFont("normal");
            this.lblAutoSave.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblAutoSave.Location = new Point(labelX, currentY);
            this.lblAutoSave.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(this.lblAutoSave, "settingsForm.tooltips.autoSave");
            this.pnlSettings.Controls.Add(this.lblAutoSave);
            
            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);
            this.toggleAutoSave = CreateToggleSwitch(new Point(inputX, currentY), toggleWidth, ui.GlobalInputHeight);
            this.toggleAutoSave.Click += ToggleSwitch_Click;
            tooltips.SetToolTip(this.toggleAutoSave, "settingsForm.tooltips.autoSave");
            this.pnlSettings.Controls.Add(this.toggleAutoSave);
            
            this.lblAutoSaveState = new Label();
            this.lblAutoSaveState.Text = "ON";
            this.lblAutoSaveState.Font = theme.GetFont("normal");
            this.lblAutoSaveState.AutoSize = true;
            this.lblAutoSaveState.Location = new Point(inputX + toggleWidth + ui.GlobalSpacingX, currentY);
            this.lblAutoSaveState.TextAlign = ContentAlignment.MiddleLeft;
            this.lblAutoSaveState.Top += (ui.GlobalInputHeight - this.lblAutoSaveState.Height) / 2;
            this.pnlSettings.Controls.Add(this.lblAutoSaveState);
            
            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;
            
            // Load Last Saved Config
            this.lblLoadLast = new Label();
            this.lblLoadLast.Text = lang.GetString("settingsForm.loadLast.label");
            this.lblLoadLast.Font = theme.GetFont("normal");
            this.lblLoadLast.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            this.lblLoadLast.Location = new Point(labelX, currentY);
            this.lblLoadLast.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(this.lblLoadLast, "settingsForm.tooltips.loadLast");
            this.pnlSettings.Controls.Add(this.lblLoadLast);
            
            this.toggleLoadLast = CreateToggleSwitch(new Point(inputX, currentY), toggleWidth, ui.GlobalInputHeight);
            this.toggleLoadLast.Click += ToggleSwitch_Click;
            tooltips.SetToolTip(this.toggleLoadLast, "settingsForm.tooltips.loadLast");
            this.pnlSettings.Controls.Add(this.toggleLoadLast);
            
            this.lblLoadLastState = new Label();
            this.lblLoadLastState.Text = "ON";
            this.lblLoadLastState.Font = theme.GetFont("normal");
            this.lblLoadLastState.AutoSize = true;
            this.lblLoadLastState.Location = new Point(inputX + toggleWidth + ui.GlobalSpacingX, currentY);
            this.lblLoadLastState.TextAlign = ContentAlignment.MiddleLeft;
            this.lblLoadLastState.Top += (ui.GlobalInputHeight - this.lblLoadLastState.Height) / 2;
            this.pnlSettings.Controls.Add(this.lblLoadLastState);
            
            this.Controls.Add(this.pnlSettings);
            
            // Control Panel - matching MainForm pattern
            this.pnlControls = new Panel();
            this.pnlControls.Location = new Point(0, this.ClientSize.Height - 60);
            this.pnlControls.Size = new Size(this.ClientSize.Width, 60);
            this.pnlControls.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            
            int btnY = (60 - ui.GlobalBtnHeight) / 2;
            
            // About Button
            this.btnAbout = theme.CreateRoundedButton();
            this.btnAbout.Text = lang.GetString("settingsForm.buttons.about");
            this.btnAbout.Font = theme.GetFont("normal");
            this.btnAbout.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnAbout.Location = new Point(ui.GlobalSpacingX, btnY);
            this.btnAbout.TextAlign = ContentAlignment.MiddleCenter;
            this.btnAbout.Click += btnAbout_Click;
            tooltips.SetToolTip(this.btnAbout, "settingsForm.tooltips.about");
            this.pnlControls.Controls.Add(this.btnAbout);
            
            // Help Button
            this.btnHelp = theme.CreateRoundedButton();
            this.btnHelp.Text = lang.GetString("settingsForm.buttons.help");
            this.btnHelp.Font = theme.GetFont("normal");
            this.btnHelp.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnHelp.Location = new Point(ui.GlobalSpacingX + ui.GlobalBtnWidth + ui.GlobalSpacingX, btnY);
            this.btnHelp.TextAlign = ContentAlignment.MiddleCenter;
            this.btnHelp.Click += btnHelp_Click;
            tooltips.SetToolTip(this.btnHelp, "settingsForm.tooltips.help");
            this.pnlControls.Controls.Add(this.btnHelp);
            
            // Cancel Button
            this.btnCancel = theme.CreateRoundedButton();
            this.btnCancel.Text = lang.GetString("settingsForm.buttons.cancel");
            this.btnCancel.Font = theme.GetFont("normal");
            this.btnCancel.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnCancel.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnWidth, btnY);
            this.btnCancel.TextAlign = ContentAlignment.MiddleCenter;
            this.btnCancel.Click += btnCancel_Click;
            tooltips.SetToolTip(this.btnCancel, "settingsForm.tooltips.cancel");
            this.pnlControls.Controls.Add(this.btnCancel);
            
            // Save Button
            this.btnSave = theme.CreateRoundedButton();
            this.btnSave.Text = lang.GetString("settingsForm.buttons.save");
            this.btnSave.Font = theme.GetFont("normal");
            this.btnSave.Size = new Size(ui.GlobalBtnWidth, ui.GlobalBtnHeight);
            this.btnSave.Location = new Point(this.ClientSize.Width - ui.GlobalTabX - ui.GlobalBtnWidth * 2 - ui.GlobalSpacingX, btnY);
            this.btnSave.TextAlign = ContentAlignment.MiddleCenter;
            this.btnSave.Click += btnSave_Click;
            tooltips.SetToolTip(this.btnSave, "settingsForm.tooltips.save");
            this.pnlControls.Controls.Add(this.btnSave);
            
            this.Controls.Add(this.pnlControls);
        }

        private Panel CreateToggleSwitch(Point location, int width, int height)
        {
            // Create the main toggle container with rounded corners
            Panel toggle = new Panel();
            toggle.Location = location;
            toggle.Size = new Size(width, height);
            toggle.BorderStyle = BorderStyle.None;
            toggle.Cursor = Cursors.Hand;
            toggle.Tag = true; // Default state is ON
            
            // Make the toggle track rounded
            System.Drawing.Drawing2D.GraphicsPath trackPath = new System.Drawing.Drawing2D.GraphicsPath();
            int cornerRadius = height / 2;
            trackPath.AddArc(0, 0, height, height, 90, 180);
            trackPath.AddLine(cornerRadius, 0, width - cornerRadius, 0);
            trackPath.AddArc(width - height, 0, height, height, 270, 180);
            trackPath.AddLine(width - cornerRadius, height, cornerRadius, height);
            toggle.Region = new Region(trackPath);
            
            // Create the toggle thumb (circular button)
            Panel toggleThumb = new Panel();
            int thumbSize = height - 6; // Slightly smaller than track height for padding
            toggleThumb.Size = new Size(thumbSize, thumbSize);
            toggleThumb.Location = new Point(width - thumbSize - 3, 3); // Position for ON state
            toggleThumb.BackColor = Color.White;
            toggleThumb.Tag = "thumb";
            
            // Make the thumb circular
            System.Drawing.Drawing2D.GraphicsPath thumbPath = new System.Drawing.Drawing2D.GraphicsPath();
            thumbPath.AddEllipse(0, 0, thumbSize, thumbSize);
            toggleThumb.Region = new Region(thumbPath);
            
            // Add shadow effect using Paint event
            toggle.Paint += (s, e) =>
            {
                // Draw inner shadow for depth effect
                using (System.Drawing.Drawing2D.GraphicsPath shadowPath = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int shadowRadius = height / 2;
                    shadowPath.AddArc(1, 1, height - 2, height - 2, 90, 180);
                    shadowPath.AddLine(shadowRadius, 1, width - shadowRadius, 1);
                    shadowPath.AddArc(width - height + 1, 1, height - 2, height - 2, 270, 180);
                    shadowPath.AddLine(width - shadowRadius, height - 1, shadowRadius, height - 1);
                    
                    using (Pen shadowPen = new Pen(Color.FromArgb(30, 0, 0, 0), 1))
                    {
                        e.Graphics.DrawPath(shadowPen, shadowPath);
                    }
                }
            };
            
            toggle.Controls.Add(toggleThumb);
            
            // Add click event to both toggle and thumb
            toggleThumb.Click += (s, e) => ToggleSwitch_Click(toggle, e);
            
            return toggle;
        }
    }
}
