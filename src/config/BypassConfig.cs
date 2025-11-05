using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Bypass Windows 11 Hardware Checks Configuration section including UI, data model, and business logic
    /// </summary>
    public class BypassConfig
    {
        #region Data Model

        public bool EnableBypass { get; set; } = true;
        public bool BypassAll { get; set; } = true;
        public bool BypassTPM { get; set; } = true;
        public bool BypassRAM { get; set; } = true;
        public bool BypassSecureBoot { get; set; } = true;
        public bool BypassCPU { get; set; } = true;
        public bool BypassStorage { get; set; } = true;
        public bool BypassDisk { get; set; } = true;

        #endregion

        #region UI Components

        private Panel pnlBypassConfig = null!;
        private Button btnBypassConfigToggle = null!;
        private Label lblBypassConfigTitle = null!;
        private Panel pnlBypassConfigSeparator = null!;
        private Panel pnlBypassConfigContent = null!;
        
        // Enable Bypass Hardware Check
        private Label lblEnableBypass = null!;
        private Panel toggleEnableBypass = null!;
        
        // Bypass All Checks
        private Label lblBypassAll = null!;
        private Panel toggleBypassAll = null!;

        // Separator after Bypass All
        private Panel pnlBypassConfigAllSeparator = null!;
        
        // Individual bypass checks
        private Label lblBypassTPM = null!;
        private Panel toggleBypassTPM = null!;
        
        private Label lblBypassRAM = null!;
        private Panel toggleBypassRAM = null!;
        
        private Label lblBypassSecureBoot = null!;
        private Panel toggleBypassSecureBoot = null!;
        
        private Label lblBypassCPU = null!;
        private Panel toggleBypassCPU = null!;
        
        private Label lblBypassStorage = null!;
        private Panel toggleBypassStorage = null!;
        
        private Label lblBypassDisk = null!;
        private Panel toggleBypassDisk = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false;
        private bool _isFirstInitialization = true;
        private Action? _onSectionToggle = null;
        private Action? _onEnableToggle = null;
        private EventHandler? _onConfigChanged = null;

        /// <summary>
        /// Gets whether the section is expanded
        /// </summary>
        public bool IsExpanded => _isExpanded;

        #endregion

        #region UI Initialization

        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged, 
            Func<Button> createRoundedButton, Action? onSectionToggle, Action? onEnableToggle)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            _onSectionToggle = onSectionToggle;
            _onEnableToggle = onEnableToggle;
            _onConfigChanged = onConfigChanged;
            
            int contentHeight = ui.GetSectionValue("bypassConfig", "contentHeight", 450);
            
            pnlBypassConfig = new Panel();
            pnlBypassConfig.Location = new Point(0, 0);
            pnlBypassConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlBypassConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnBypassConfigToggle = createRoundedButton();
            btnBypassConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnBypassConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnBypassConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnBypassConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnBypassConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnBypassConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.bypass"));

            lblBypassConfigTitle = new Label();
            lblBypassConfigTitle.Location = new Point(btnBypassConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblBypassConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblBypassConfigTitle.Text = lang.GetString("mainForm.sections.bypass");
            lblBypassConfigTitle.Font = theme.GetFont("subheader");
            lblBypassConfigTitle.UseMnemonic = false;
            lblBypassConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblBypassConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassConfigTitle.Cursor = Cursors.Hand;
            lblBypassConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblBypassConfigTitle, "tooltips.bypassConfig.header", lang.GetString("tooltips.bypassConfig.header"));

            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);
            int doubleLabelWidth = ui.GlobalLabelWidth * 2;

            // Enable Bypass Hardware Check
            // Only set EnableBypass from settings on first initialization
            if (_isFirstInitialization)
            {
                EnableBypass = !SettingsManager.Instance.LockSectionsAtStartup;
                _isFirstInitialization = false;
            }
            toggleEnableBypass = theme.CreateToggleSwitch(new Point(ui.GlobalTabX * 2 + ui.GlobalBtnBox, btnBypassConfigToggle.Bottom + ui.GlobalSpacingY), toggleWidth, ui.GlobalInputHeight, EnableBypass);
            toggleEnableBypass.TabStop = false; // Skip this control in tab order
            toggleEnableBypass.Click += (s, e) => OnToggleEnableBypass();

            lblEnableBypass = new Label();
            lblEnableBypass.Location = new Point(toggleEnableBypass.Right + ui.GlobalSpacingX, btnBypassConfigToggle.Bottom + ui.GlobalSpacingY);
            lblEnableBypass.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblEnableBypass.Text = string.Format(lang.GetString("bypassConfig.enableBypass.label"), lang.GetString("mainForm.sections.bypass"));
            lblEnableBypass.Font = theme.GetFont("normal");
            lblEnableBypass.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableBypass.Cursor = Cursors.Hand;
            lblEnableBypass.Click += (s, e) => OnToggleEnableBypass();
            tooltips.SetToolTip(lblEnableBypass, "tooltips.bypassConfig.enableBypass");

            // Line Separator
            pnlBypassConfigSeparator = new Panel();
            pnlBypassConfigSeparator.Location = new Point(ui.GlobalTabX, toggleEnableBypass.Bottom + ui.GlobalSpacingY * 2);
            pnlBypassConfigSeparator.Size = new Size(pnlBypassConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlBypassConfigSeparator.BackColor = theme.GetColor("separator");

            // Bypass Config Content Panel
            pnlBypassConfigContent = new Panel();
            pnlBypassConfigContent.Location = new Point(0, pnlBypassConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlBypassConfigContent.Size = new Size(pnlBypassConfig.Width, contentHeight);
            pnlBypassConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlBypassConfigContent.AutoScroll = false;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int toggleX = pnlBypassConfigContent.Width - toggleWidth - ui.GlobalSpacingX;
            int currentY = ui.GlobalSpacingY;

            // Bypass All Checks
            lblBypassAll = new Label();
            lblBypassAll.Location = new Point(labelX, currentY);
            lblBypassAll.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassAll.Text = lang.GetString("bypassConfig.bypassAll.label");
            lblBypassAll.Font = theme.GetFont("normal");
            lblBypassAll.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassAll.Cursor = Cursors.Hand;
            lblBypassAll.Click += (s, e) => OnToggleBypassAll();
            tooltips.SetToolTip(lblBypassAll, "tooltips.bypassConfig.bypassAll");

            toggleBypassAll = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassAll);
            toggleBypassAll.Click += (s, e) => OnToggleBypassAll();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Line Separator
            pnlBypassConfigAllSeparator = new Panel();
            pnlBypassConfigAllSeparator.Location = new Point(labelX, currentY);
            pnlBypassConfigAllSeparator.Size = new Size(pnlBypassConfigContent.Width - labelX - ui.GlobalTabX, 1);
            pnlBypassConfigAllSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            labelX = ui.GlobalTabX * 3 + ui.GlobalBtnBox + ui.GlobalSpacingX;
            toggleX = pnlBypassConfigContent.Width - toggleWidth - ui.GlobalSpacingX;

            // Bypass TPM Check
            lblBypassTPM = new Label();
            lblBypassTPM.Location = new Point(labelX, currentY);
            lblBypassTPM.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassTPM.Text = lang.GetString("bypassConfig.bypassTPM.label");
            lblBypassTPM.Font = theme.GetFont("normal");
            lblBypassTPM.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassTPM.Cursor = Cursors.Hand;
            lblBypassTPM.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblBypassTPM, "tooltips.bypassConfig.bypassTPM");

            toggleBypassTPM = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassTPM);
            toggleBypassTPM.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Bypass RAM Check
            lblBypassRAM = new Label();
            lblBypassRAM.Location = new Point(labelX, currentY);
            lblBypassRAM.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassRAM.Text = lang.GetString("bypassConfig.bypassRAM.label");
            lblBypassRAM.Font = theme.GetFont("normal");
            lblBypassRAM.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassRAM.Cursor = Cursors.Hand;
            lblBypassRAM.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblBypassRAM, "tooltips.bypassConfig.bypassRAM");

            toggleBypassRAM = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassRAM);
            toggleBypassRAM.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Bypass Secure Boot Check
            lblBypassSecureBoot = new Label();
            lblBypassSecureBoot.Location = new Point(labelX, currentY);
            lblBypassSecureBoot.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassSecureBoot.Text = lang.GetString("bypassConfig.bypassSecureBoot.label");
            lblBypassSecureBoot.Font = theme.GetFont("normal");
            lblBypassSecureBoot.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassSecureBoot.Cursor = Cursors.Hand;
            lblBypassSecureBoot.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblBypassSecureBoot, "tooltips.bypassConfig.bypassSecureBoot");

            toggleBypassSecureBoot = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassSecureBoot);
            toggleBypassSecureBoot.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Bypass CPU Check
            lblBypassCPU = new Label();
            lblBypassCPU.Location = new Point(labelX, currentY);
            lblBypassCPU.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassCPU.Text = lang.GetString("bypassConfig.bypassCPU.label");
            lblBypassCPU.Font = theme.GetFont("normal");
            lblBypassCPU.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassCPU.Cursor = Cursors.Hand;
            lblBypassCPU.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblBypassCPU, "tooltips.bypassConfig.bypassCPU");

            toggleBypassCPU = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassCPU);
            toggleBypassCPU.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Bypass Storage Check
            lblBypassStorage = new Label();
            lblBypassStorage.Location = new Point(labelX, currentY);
            lblBypassStorage.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassStorage.Text = lang.GetString("bypassConfig.bypassStorage.label");
            lblBypassStorage.Font = theme.GetFont("normal");
            lblBypassStorage.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassStorage.Cursor = Cursors.Hand;
            lblBypassStorage.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblBypassStorage, "tooltips.bypassConfig.bypassStorage");

            toggleBypassStorage = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassStorage);
            toggleBypassStorage.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Bypass Disk Check
            lblBypassDisk = new Label();
            lblBypassDisk.Location = new Point(labelX, currentY);
            lblBypassDisk.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblBypassDisk.Text = lang.GetString("bypassConfig.bypassDisk.label");
            lblBypassDisk.Font = theme.GetFont("normal");
            lblBypassDisk.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassDisk.Cursor = Cursors.Hand;
            lblBypassDisk.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblBypassDisk, "tooltips.bypassConfig.bypassDisk");

            toggleBypassDisk = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, BypassDisk);
            toggleBypassDisk.Click += (s, e) => OnToggleIndividualCheck();

            // Add all controls to content panel
            pnlBypassConfigContent.Controls.Add(lblBypassAll);
            pnlBypassConfigContent.Controls.Add(toggleBypassAll);
            pnlBypassConfigContent.Controls.Add(pnlBypassConfigAllSeparator);
            pnlBypassConfigContent.Controls.Add(lblBypassTPM);
            pnlBypassConfigContent.Controls.Add(toggleBypassTPM);
            pnlBypassConfigContent.Controls.Add(lblBypassRAM);
            pnlBypassConfigContent.Controls.Add(toggleBypassRAM);
            pnlBypassConfigContent.Controls.Add(lblBypassSecureBoot);
            pnlBypassConfigContent.Controls.Add(toggleBypassSecureBoot);
            pnlBypassConfigContent.Controls.Add(lblBypassCPU);
            pnlBypassConfigContent.Controls.Add(toggleBypassCPU);
            pnlBypassConfigContent.Controls.Add(lblBypassStorage);
            pnlBypassConfigContent.Controls.Add(toggleBypassStorage);
            pnlBypassConfigContent.Controls.Add(lblBypassDisk);
            pnlBypassConfigContent.Controls.Add(toggleBypassDisk);

            pnlBypassConfig.Controls.Add(btnBypassConfigToggle);
            pnlBypassConfig.Controls.Add(lblBypassConfigTitle);
            pnlBypassConfig.Controls.Add(toggleEnableBypass);
            pnlBypassConfig.Controls.Add(lblEnableBypass);
            pnlBypassConfig.Controls.Add(pnlBypassConfigSeparator);
            pnlBypassConfig.Controls.Add(pnlBypassConfigContent);

            // Apply current expansion state
            pnlBypassConfigContent.Visible = _isExpanded;
            pnlBypassConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlBypassConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            // Apply muted state if EnableBypass is locked at startup
            if (!EnableBypass)
            {
                UpdateControlsFromModel();
            }

            return pnlBypassConfig;
        }

        public void LoadConfigIntoUI()
        {
            try
            {
                Console.WriteLine("BypassConfig: Loading config into UI...");
                UpdateControlsFromModel();
                Console.WriteLine("BypassConfig: Config loaded into UI successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BypassConfig: Error loading config into UI: {ex.Message}");
            }
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlBypassConfigContent.Visible = _isExpanded;
            pnlBypassConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("bypassConfig", "contentHeight", 450);
                pnlBypassConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnBypassConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.bypass"));
            }
            else
            {
                pnlBypassConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnBypassConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.bypass"));
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnBypassConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnBypassConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

            _onSectionToggle?.Invoke();
        }

        public void Expand()
        {
            if (!_isExpanded)
                ToggleSection();
        }

        public void Collapse()
        {
            if (_isExpanded)
                ToggleSection();
        }

        /// <summary>
        /// Sets the Enable state for this section
        /// </summary>
        public void SetEnableState(bool enabled)
        {
            if (_isLoading) return;
            
            ThemeManager theme = ThemeManager.Instance;
            theme.UpdateToggleSwitchState(toggleEnableBypass, enabled);
            EnableBypass = enabled;
            
            // Update dependent controls muted state
            UpdateControlsFromModel();
        }

        /// <summary>
        /// Handles the toggle for Enable Bypass Hardware Check
        /// </summary>
        private void OnToggleEnableBypass()
        {
            if (_isLoading) return;

            ThemeManager theme = ThemeManager.Instance;
            
            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleEnableBypass);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableBypass, newState);
            
            EnableBypass = newState;

            // Enable or disable all dependent toggles
            bool enableControls = newState;
            
            toggleBypassAll.Enabled = enableControls;
            toggleBypassTPM.Enabled = enableControls;
            toggleBypassRAM.Enabled = enableControls;
            toggleBypassSecureBoot.Enabled = enableControls;
            toggleBypassCPU.Enabled = enableControls;
            toggleBypassStorage.Enabled = enableControls;
            toggleBypassDisk.Enabled = enableControls;

            // Update visual appearance based on enabled state
            bool isMuted = !newState;
            
            // Update all toggle switches to muted or normal state
            theme.UpdateToggleSwitchMutedState(toggleBypassAll, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleBypassTPM, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleBypassRAM, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleBypassSecureBoot, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleBypassCPU, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleBypassStorage, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleBypassDisk, isMuted);
            
            // Update all labels to muted or normal state
            if (isMuted)
            {
                lblEnableBypass.Font = theme.GetFont("muted");
                lblEnableBypass.ForeColor = theme.GetFontColor("muted");
                
                lblBypassAll.Font = theme.GetFont("muted");
                lblBypassAll.ForeColor = theme.GetFontColor("muted");
                lblBypassAll.Cursor = Cursors.Default;
                lblBypassTPM.Font = theme.GetFont("muted");
                lblBypassTPM.ForeColor = theme.GetFontColor("muted");
                lblBypassTPM.Cursor = Cursors.Default;
                lblBypassRAM.Font = theme.GetFont("muted");
                lblBypassRAM.ForeColor = theme.GetFontColor("muted");
                lblBypassRAM.Cursor = Cursors.Default;
                lblBypassSecureBoot.Font = theme.GetFont("muted");
                lblBypassSecureBoot.ForeColor = theme.GetFontColor("muted");
                lblBypassSecureBoot.Cursor = Cursors.Default;
                lblBypassCPU.Font = theme.GetFont("muted");
                lblBypassCPU.ForeColor = theme.GetFontColor("muted");
                lblBypassCPU.Cursor = Cursors.Default;
                lblBypassStorage.Font = theme.GetFont("muted");
                lblBypassStorage.ForeColor = theme.GetFontColor("muted");
                lblBypassStorage.Cursor = Cursors.Default;
                lblBypassDisk.Font = theme.GetFont("muted");
                lblBypassDisk.ForeColor = theme.GetFontColor("muted");
                lblBypassDisk.Cursor = Cursors.Default;
            }
            else
            {
                lblEnableBypass.Font = theme.GetFont("normal");
                lblEnableBypass.ForeColor = theme.GetFontColor("normal");
                
                lblBypassAll.Font = theme.GetFont("normal");
                lblBypassAll.ForeColor = theme.GetFontColor("normal");
                lblBypassAll.Cursor = Cursors.Hand;
                lblBypassTPM.Font = theme.GetFont("normal");
                lblBypassTPM.ForeColor = theme.GetFontColor("normal");
                lblBypassTPM.Cursor = Cursors.Hand;
                lblBypassRAM.Font = theme.GetFont("normal");
                lblBypassRAM.ForeColor = theme.GetFontColor("normal");
                lblBypassRAM.Cursor = Cursors.Hand;
                lblBypassSecureBoot.Font = theme.GetFont("normal");
                lblBypassSecureBoot.ForeColor = theme.GetFontColor("normal");
                lblBypassSecureBoot.Cursor = Cursors.Hand;
                lblBypassCPU.Font = theme.GetFont("normal");
                lblBypassCPU.ForeColor = theme.GetFontColor("normal");
                lblBypassCPU.Cursor = Cursors.Hand;
                lblBypassStorage.Font = theme.GetFont("normal");
                lblBypassStorage.ForeColor = theme.GetFontColor("normal");
                lblBypassStorage.Cursor = Cursors.Hand;
                lblBypassDisk.Font = theme.GetFont("normal");
                lblBypassDisk.ForeColor = theme.GetFontColor("normal");
                lblBypassDisk.Cursor = Cursors.Hand;
            }

            // Notify MainForm to update lock/unlock button
            _onEnableToggle?.Invoke();
        }

        /// <summary>
        /// Handles the toggle for Bypass All Checks
        /// </summary>
        private void OnToggleBypassAll()
        {
            if (_isLoading || !EnableBypass) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleBypassAll);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleBypassAll, newState);

            BypassAll = newState;

            // If enabling, turn on all individual bypasses
            if (newState)
            {
                theme.UpdateToggleSwitchState(toggleBypassTPM, true);
                theme.UpdateToggleSwitchState(toggleBypassRAM, true);
                theme.UpdateToggleSwitchState(toggleBypassSecureBoot, true);
                theme.UpdateToggleSwitchState(toggleBypassCPU, true);
                theme.UpdateToggleSwitchState(toggleBypassStorage, true);
                theme.UpdateToggleSwitchState(toggleBypassDisk, true);

                BypassTPM = true;
                BypassRAM = true;
                BypassSecureBoot = true;
                BypassCPU = true;
                BypassStorage = true;
                BypassDisk = true;
            }
            else
            {
                // If disabling, turn off all individual bypasses
                theme.UpdateToggleSwitchState(toggleBypassTPM, false);
                theme.UpdateToggleSwitchState(toggleBypassRAM, false);
                theme.UpdateToggleSwitchState(toggleBypassSecureBoot, false);
                theme.UpdateToggleSwitchState(toggleBypassCPU, false);
                theme.UpdateToggleSwitchState(toggleBypassStorage, false);
                theme.UpdateToggleSwitchState(toggleBypassDisk, false);

                BypassTPM = false;
                BypassRAM = false;
                BypassSecureBoot = false;
                BypassCPU = false;
                BypassStorage = false;
                BypassDisk = false;
            }
            
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle for individual bypass checks
        /// </summary>
        private void OnToggleIndividualCheck()
        {
            if (_isLoading || !EnableBypass) return;

            ThemeManager theme = ThemeManager.Instance;
            Panel? sender = null;
            
            // Get the sender from the active control or mouse position
            Control? activeControl = pnlBypassConfigContent.GetChildAtPoint(
                pnlBypassConfigContent.PointToClient(Cursor.Position));
            sender = activeControl as Panel;
            
            if (sender == null) return;
            
            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(sender);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(sender, newState);

            // Update data model
            if (ReferenceEquals(sender, toggleBypassTPM)) BypassTPM = newState;
            else if (ReferenceEquals(sender, toggleBypassRAM)) BypassRAM = newState;
            else if (ReferenceEquals(sender, toggleBypassSecureBoot)) BypassSecureBoot = newState;
            else if (ReferenceEquals(sender, toggleBypassCPU)) BypassCPU = newState;
            else if (ReferenceEquals(sender, toggleBypassStorage)) BypassStorage = newState;
            else if (ReferenceEquals(sender, toggleBypassDisk)) BypassDisk = newState;
            
            // Update Bypass All toggle based on all individual toggles
            bool allEnabled = BypassTPM && BypassRAM && BypassSecureBoot && 
                            BypassCPU && BypassStorage && BypassDisk;
            
            if (allEnabled != BypassAll)
            {
                BypassAll = allEnabled;
                theme.UpdateToggleSwitchState(toggleBypassAll, allEnabled);
            }
            
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Data Model Operations

        public void UpdateFromControls()
        {
            if (_isLoading) return;
            
            // Check if UI is initialized
            if (toggleBypassAll == null ||
                toggleBypassTPM == null ||
                toggleBypassRAM == null ||
                toggleBypassSecureBoot == null ||
                toggleBypassCPU == null ||
                toggleBypassStorage == null ||
                toggleBypassDisk == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }
            
            ThemeManager theme = ThemeManager.Instance;
            
            // EnableBypass should never be saved - it's a safety lock that always resets to true
            // EnableBypass = theme.GetToggleSwitchState(toggleEnableBypass);
            
            BypassAll = theme.GetToggleSwitchState(toggleBypassAll);
            BypassTPM = theme.GetToggleSwitchState(toggleBypassTPM);
            BypassRAM = theme.GetToggleSwitchState(toggleBypassRAM);
            BypassSecureBoot = theme.GetToggleSwitchState(toggleBypassSecureBoot);
            BypassCPU = theme.GetToggleSwitchState(toggleBypassCPU);
            BypassStorage = theme.GetToggleSwitchState(toggleBypassStorage);
            BypassDisk = theme.GetToggleSwitchState(toggleBypassDisk);
        }

        public void ClearControls()
        {
            _isLoading = true;
            try
            {
                ThemeManager theme = ThemeManager.Instance;

                EnableBypass = true;
                
                // Reset all other toggles to off state
                theme.UpdateToggleSwitchState(toggleBypassAll, false);
                theme.UpdateToggleSwitchState(toggleBypassTPM, false);
                theme.UpdateToggleSwitchState(toggleBypassRAM, false);
                theme.UpdateToggleSwitchState(toggleBypassSecureBoot, false);
                theme.UpdateToggleSwitchState(toggleBypassCPU, false);
                theme.UpdateToggleSwitchState(toggleBypassStorage, false);
                theme.UpdateToggleSwitchState(toggleBypassDisk, false);
                
                // Enable all dependent toggles (since EnableBypass is true)
                toggleBypassAll.Enabled = true;
                toggleBypassTPM.Enabled = true;
                toggleBypassRAM.Enabled = true;
                toggleBypassSecureBoot.Enabled = true;
                toggleBypassCPU.Enabled = true;
                toggleBypassStorage.Enabled = true;
                toggleBypassDisk.Enabled = true;
                
                // Apply normal visual state (not muted)
                theme.UpdateToggleSwitchMutedState(toggleBypassAll, false);
                theme.UpdateToggleSwitchMutedState(toggleBypassTPM, false);
                theme.UpdateToggleSwitchMutedState(toggleBypassRAM, false);
                theme.UpdateToggleSwitchMutedState(toggleBypassSecureBoot, false);
                theme.UpdateToggleSwitchMutedState(toggleBypassCPU, false);
                theme.UpdateToggleSwitchMutedState(toggleBypassStorage, false);
                theme.UpdateToggleSwitchMutedState(toggleBypassDisk, false);
                
                // Update all labels to normal state
                lblBypassAll.Font = theme.GetFont("normal");
                lblBypassAll.ForeColor = theme.GetFontColor("normal");
                lblBypassAll.Cursor = Cursors.Hand;
                lblBypassTPM.Font = theme.GetFont("normal");
                lblBypassTPM.ForeColor = theme.GetFontColor("normal");
                lblBypassTPM.Cursor = Cursors.Hand;
                lblBypassRAM.Font = theme.GetFont("normal");
                lblBypassRAM.ForeColor = theme.GetFontColor("normal");
                lblBypassRAM.Cursor = Cursors.Hand;
                lblBypassSecureBoot.Font = theme.GetFont("normal");
                lblBypassSecureBoot.ForeColor = theme.GetFontColor("normal");
                lblBypassSecureBoot.Cursor = Cursors.Hand;
                lblBypassCPU.Font = theme.GetFont("normal");
                lblBypassCPU.ForeColor = theme.GetFontColor("normal");
                lblBypassCPU.Cursor = Cursors.Hand;
                lblBypassStorage.Font = theme.GetFont("normal");
                lblBypassStorage.ForeColor = theme.GetFontColor("normal");
                lblBypassStorage.Cursor = Cursors.Hand;
                lblBypassDisk.Font = theme.GetFont("normal");
                lblBypassDisk.ForeColor = theme.GetFontColor("normal");
                lblBypassDisk.Cursor = Cursors.Hand;
                
                // Reset data model (except EnableBypass which stays true)
                BypassAll = false;
                BypassTPM = false;
                BypassRAM = false;
                BypassSecureBoot = false;
                BypassCPU = false;
                BypassStorage = false;
                BypassDisk = false;
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void UpdateControlsFromModel()
        {
            _isLoading = true;
            
            // Check if UI is initialized
            if (toggleBypassAll == null ||
                toggleBypassTPM == null ||
                toggleBypassRAM == null ||
                toggleBypassSecureBoot == null ||
                toggleBypassCPU == null ||
                toggleBypassStorage == null ||
                toggleBypassDisk == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: BypassAll={BypassAll}, BypassTPM={BypassTPM}, BypassRAM={BypassRAM}, BypassSecureBoot={BypassSecureBoot}, BypassCPU={BypassCPU}, BypassStorage={BypassStorage}, BypassDisk={BypassDisk}");

            try
            {
                ThemeManager theme = ThemeManager.Instance;
                
                // Update all toggle states
                theme.UpdateToggleSwitchState(toggleEnableBypass, EnableBypass);
                theme.UpdateToggleSwitchState(toggleBypassAll, BypassAll);
                theme.UpdateToggleSwitchState(toggleBypassTPM, BypassTPM);
                theme.UpdateToggleSwitchState(toggleBypassRAM, BypassRAM);
                theme.UpdateToggleSwitchState(toggleBypassSecureBoot, BypassSecureBoot);
                theme.UpdateToggleSwitchState(toggleBypassCPU, BypassCPU);
                theme.UpdateToggleSwitchState(toggleBypassStorage, BypassStorage);
                theme.UpdateToggleSwitchState(toggleBypassDisk, BypassDisk);

                // Enable/disable dependent toggles based on EnableBypass
                bool enableControls = EnableBypass;
                
                toggleBypassAll.Enabled = enableControls;
                toggleBypassTPM.Enabled = enableControls;
                toggleBypassRAM.Enabled = enableControls;
                toggleBypassSecureBoot.Enabled = enableControls;
                toggleBypassCPU.Enabled = enableControls;
                toggleBypassStorage.Enabled = enableControls;
                toggleBypassDisk.Enabled = enableControls;
                
                // Update visual appearance based on enabled state
                bool isMuted = !EnableBypass;
                
                // Update all toggle switches to muted or normal state
                theme.UpdateToggleSwitchMutedState(toggleBypassAll, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleBypassTPM, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleBypassRAM, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleBypassSecureBoot, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleBypassCPU, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleBypassStorage, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleBypassDisk, isMuted);
                
                // Update all labels to muted or normal state
                if (isMuted)
                {
                    lblEnableBypass.Font = theme.GetFont("muted");
                    lblEnableBypass.ForeColor = theme.GetFontColor("muted");
                    lblBypassAll.Font = theme.GetFont("muted");
                    lblBypassAll.ForeColor = theme.GetFontColor("muted");
                    lblBypassAll.Cursor = Cursors.Default;
                    lblBypassTPM.Font = theme.GetFont("muted");
                    lblBypassTPM.ForeColor = theme.GetFontColor("muted");
                    lblBypassTPM.Cursor = Cursors.Default;
                    lblBypassRAM.Font = theme.GetFont("muted");
                    lblBypassRAM.ForeColor = theme.GetFontColor("muted");
                    lblBypassRAM.Cursor = Cursors.Default;
                    lblBypassSecureBoot.Font = theme.GetFont("muted");
                    lblBypassSecureBoot.ForeColor = theme.GetFontColor("muted");
                    lblBypassSecureBoot.Cursor = Cursors.Default;
                    lblBypassCPU.Font = theme.GetFont("muted");
                    lblBypassCPU.ForeColor = theme.GetFontColor("muted");
                    lblBypassCPU.Cursor = Cursors.Default;
                    lblBypassStorage.Font = theme.GetFont("muted");
                    lblBypassStorage.ForeColor = theme.GetFontColor("muted");
                    lblBypassStorage.Cursor = Cursors.Default;
                    lblBypassDisk.Font = theme.GetFont("muted");
                    lblBypassDisk.ForeColor = theme.GetFontColor("muted");
                    lblBypassDisk.Cursor = Cursors.Default;
                }
                else
                {
                    lblEnableBypass.Font = theme.GetFont("normal");
                    lblEnableBypass.ForeColor = theme.GetFontColor("normal");
                    lblBypassAll.Font = theme.GetFont("normal");
                    lblBypassAll.ForeColor = theme.GetFontColor("normal");
                    lblBypassAll.Cursor = Cursors.Hand;
                    lblBypassTPM.Font = theme.GetFont("normal");
                    lblBypassTPM.ForeColor = theme.GetFontColor("normal");
                    lblBypassTPM.Cursor = Cursors.Hand;
                    lblBypassRAM.Font = theme.GetFont("normal");
                    lblBypassRAM.ForeColor = theme.GetFontColor("normal");
                    lblBypassRAM.Cursor = Cursors.Hand;
                    lblBypassSecureBoot.Font = theme.GetFont("normal");
                    lblBypassSecureBoot.ForeColor = theme.GetFontColor("normal");
                    lblBypassSecureBoot.Cursor = Cursors.Hand;
                    lblBypassCPU.Font = theme.GetFont("normal");
                    lblBypassCPU.ForeColor = theme.GetFontColor("normal");
                    lblBypassCPU.Cursor = Cursors.Hand;
                    lblBypassStorage.Font = theme.GetFont("normal");
                    lblBypassStorage.ForeColor = theme.GetFontColor("normal");
                    lblBypassStorage.Cursor = Cursors.Hand;
                    lblBypassDisk.Font = theme.GetFont("normal");
                    lblBypassDisk.ForeColor = theme.GetFontColor("normal");
                    lblBypassDisk.Cursor = Cursors.Hand;
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void SetValues(dynamic json)
        {
            if (json == null) return;

            try
            {
                // EnableBypass should never be loaded from JSON - always stays true (safety feature)
                // if (json.enableBypass != null) EnableBypass = (bool)json.enableBypass;
                
                if (json.bypassAll != null) BypassAll = (bool)json.bypassAll;
                if (json.bypassTPM != null) BypassTPM = (bool)json.bypassTPM;
                if (json.bypassRAM != null) BypassRAM = (bool)json.bypassRAM;
                if (json.bypassSecureBoot != null) BypassSecureBoot = (bool)json.bypassSecureBoot;
                if (json.bypassCPU != null) BypassCPU = (bool)json.bypassCPU;
                if (json.bypassStorage != null) BypassStorage = (bool)json.bypassStorage;
                if (json.bypassDisk != null) BypassDisk = (bool)json.bypassDisk;
            }
            catch { }
        }

        /// <summary>
        /// Returns bypass configuration values as a dictionary for XML generation
        /// </summary>
        public Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            
            // Return bypass values as strings for XML placeholders
            values["BypassTPM"] = BypassTPM ? "1" : "0";
            values["BypassRAM"] = BypassRAM ? "1" : "0";
            values["BypassSecureBoot"] = BypassSecureBoot ? "1" : "0";
            values["BypassCPU"] = BypassCPU ? "1" : "0";
            values["BypassStorage"] = BypassStorage ? "1" : "0";
            values["BypassDisk"] = BypassDisk ? "1" : "0";
            
            return values;
        }

        #endregion
    }
}
