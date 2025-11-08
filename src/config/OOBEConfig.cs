using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles OOBE Configuration section including UI, data model, and business logic
    /// </summary>
    public class OOBEConfig
    {
        #region Data Model

        public bool EnableOOBE { get; set; } = true;
        public bool SkipAll { get; set; } = true;
        public bool SkipEULA { get; set; } = true;
        public bool SkipLocalAccountCreation { get; set; } = true;
        public bool SkipOnlineAccountCreation { get; set; } = true;
        public bool SkipWirelessNetwork { get; set; } = true;
        public bool SkipMachineOOBE { get; set; } = true;
        public bool SkipUserOOBE { get; set; } = true;
        public string NetworkLocation { get; set; } = "";
        public string ProtectYourPC { get; set; } = "";

        #endregion

        #region UI Components

        private Panel pnlOOBEConfig = null!;
        private Button btnOOBEConfigToggle = null!;
        private Label lblOOBEConfigTitle = null!;
        private Panel pnlOOBEConfigSeparator = null!;
        private Panel pnlOOBEConfigContent = null!;
        
        // Enable OOBE Config
        private Label lblEnableOOBE = null!;
        private Panel toggleEnableOOBE = null!;
        
        // oobe All Checks
        private Label lblSkipAll = null!;
        private Panel toggleSkipAll = null!;

        // Separator after oobe All
        private Panel pnlOOBEConfigAllSeparator = null!;
        
        // Individual oobe checks
        private Label lblSkipEULA = null!;
        private Panel toggleSkipEULA = null!;
        
        private Label lblSkipLocalAccountCreation = null!;
        private Panel toggleSkipLocalAccountCreation = null!;
        
        private Label lblSkipOnlineAccountCreation = null!;
        private Panel toggleSkipOnlineAccountCreation = null!;
        
        private Label lblSkipWirelessNetwork = null!;
        private Panel toggleSkipWirelessNetwork = null!;
        
        private Label lblSkipMachineOOBE = null!;
        private Panel toggleSkipMachineOOBE = null!;
        
        private Label lblSkipUserOOBE = null!;
        private Panel toggleSkipUserOOBE = null!;

        // Line separator
        private Panel pnlOOBEConfigAllAfterSeparator = null!;

        // Network Location
        private Label lblNetworkLocation = null!;
        private ComboBox cmbNetworkLocation = null!;
        private StatusRing ringNetworkLocation = null!;

        // Protect Your PC
        private Label lblProtectYourPC = null!;
        private ComboBox cmbProtectYourPC = null!;
        private StatusRing ringProtectYourPC = null!;

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
            
            int contentHeight = ui.GetSectionValue("oobeConfig", "contentHeight", 570);
            
            pnlOOBEConfig = new Panel();
            pnlOOBEConfig.Location = new Point(0, 0);
            pnlOOBEConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlOOBEConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnOOBEConfigToggle = createRoundedButton();
            btnOOBEConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnOOBEConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnOOBEConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnOOBEConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnOOBEConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnOOBEConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.oobe"));

            lblOOBEConfigTitle = new Label();
            lblOOBEConfigTitle.Location = new Point(btnOOBEConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblOOBEConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblOOBEConfigTitle.Text = lang.GetString("mainForm.sections.oobe");
            lblOOBEConfigTitle.Font = theme.GetFont("subheader");
            lblOOBEConfigTitle.UseMnemonic = false;
            lblOOBEConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblOOBEConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblOOBEConfigTitle.Cursor = Cursors.Hand;
            lblOOBEConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblOOBEConfigTitle, "tooltips.oobeConfig.header", lang.GetString("tooltips.oobeConfig.header"));

            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);
            int doubleLabelWidth = ui.GlobalLabelWidth * 2;

            // Enable OOBE Config
            // Check if oobe should be locked at startup
            // Only set EnableOOBE from settings on first initialization
            if (_isFirstInitialization)
            {
                EnableOOBE = !SettingsManager.Instance.LockSectionsAtStartup;
                _isFirstInitialization = false;
            }
            toggleEnableOOBE = theme.CreateToggleSwitch(new Point(ui.GlobalTabX * 2 + ui.GlobalBtnBox, btnOOBEConfigToggle.Bottom + ui.GlobalSpacingY), toggleWidth, ui.GlobalInputHeight, EnableOOBE);
            toggleEnableOOBE.TabStop = false; // Skip this control in tab order
            toggleEnableOOBE.Click += (s, e) => OnToggleEnableOOBE();

            lblEnableOOBE = new Label();
            lblEnableOOBE.Location = new Point(toggleEnableOOBE.Right + ui.GlobalSpacingX, btnOOBEConfigToggle.Bottom + ui.GlobalSpacingY);
            lblEnableOOBE.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblEnableOOBE.Text = string.Format(lang.GetString("oobeConfig.enableOobe.label"), lang.GetString("mainForm.sections.oobe"));
            lblEnableOOBE.Font = theme.GetFont("normal");
            lblEnableOOBE.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableOOBE.Cursor = Cursors.Hand;
            lblEnableOOBE.Click += (s, e) => OnToggleEnableOOBE();
            tooltips.SetToolTip(lblEnableOOBE, "tooltips.oobeConfig.enableOobe");

            // Line Separator
            pnlOOBEConfigSeparator = new Panel();
            pnlOOBEConfigSeparator.Location = new Point(ui.GlobalTabX, toggleEnableOOBE.Bottom + ui.GlobalSpacingY * 2);
            pnlOOBEConfigSeparator.Size = new Size(pnlOOBEConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlOOBEConfigSeparator.BackColor = theme.GetColor("separator");

            // oobe Config Content Panel
            pnlOOBEConfigContent = new Panel();
            pnlOOBEConfigContent.Location = new Point(0, pnlOOBEConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlOOBEConfigContent.Size = new Size(pnlOOBEConfig.Width, contentHeight);
            pnlOOBEConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlOOBEConfigContent.AutoScroll = false;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int toggleX = pnlOOBEConfigContent.Width - toggleWidth - ui.GlobalSpacingX;
            int currentY = ui.GlobalSpacingY;

            // Skip All
            lblSkipAll = new Label();
            lblSkipAll.Location = new Point(labelX, currentY);
            lblSkipAll.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipAll.Text = lang.GetString("oobeConfig.skipAll.label");
            lblSkipAll.Font = theme.GetFont("normal");
            lblSkipAll.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipAll.Cursor = Cursors.Hand;
            lblSkipAll.Click += (s, e) => OnToggleSkipAll();
            tooltips.SetToolTip(lblSkipAll, "tooltips.oobeConfig.skipAll");

            toggleSkipAll = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipAll);
            toggleSkipAll.Click += (s, e) => OnToggleSkipAll();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Line Separator
            pnlOOBEConfigAllSeparator = new Panel();
            pnlOOBEConfigAllSeparator.Location = new Point(labelX, currentY);
            pnlOOBEConfigAllSeparator.Size = new Size(pnlOOBEConfigContent.Width - labelX - ui.GlobalTabX, 1);
            pnlOOBEConfigAllSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            labelX = ui.GlobalTabX * 3 + ui.GlobalBtnBox + ui.GlobalSpacingX;
            toggleX = pnlOOBEConfigContent.Width - toggleWidth - ui.GlobalSpacingX;

            // Skip EULA
            lblSkipEULA = new Label();
            lblSkipEULA.Location = new Point(labelX, currentY);
            lblSkipEULA.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipEULA.Text = lang.GetString("oobeConfig.skipEULA.label");
            lblSkipEULA.Font = theme.GetFont("normal");
            lblSkipEULA.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipEULA.Cursor = Cursors.Hand;
            lblSkipEULA.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblSkipEULA, "tooltips.oobeConfig.skipEULA");

            toggleSkipEULA = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipEULA);
            toggleSkipEULA.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Skip Local Account Creation
            lblSkipLocalAccountCreation = new Label();
            lblSkipLocalAccountCreation.Location = new Point(labelX, currentY);
            lblSkipLocalAccountCreation.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipLocalAccountCreation.Text = lang.GetString("oobeConfig.skipLocalAccountCreation.label");
            lblSkipLocalAccountCreation.Font = theme.GetFont("normal");
            lblSkipLocalAccountCreation.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipLocalAccountCreation.Cursor = Cursors.Hand;
            lblSkipLocalAccountCreation.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblSkipLocalAccountCreation, "tooltips.oobeConfig.skipLocalAccountCreation");

            toggleSkipLocalAccountCreation = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipLocalAccountCreation);
            toggleSkipLocalAccountCreation.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Skip Online Account Creation
            lblSkipOnlineAccountCreation = new Label();
            lblSkipOnlineAccountCreation.Location = new Point(labelX, currentY);
            lblSkipOnlineAccountCreation.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipOnlineAccountCreation.Text = lang.GetString("oobeConfig.skipOnlineAccountCreation.label");
            lblSkipOnlineAccountCreation.Font = theme.GetFont("normal");
            lblSkipOnlineAccountCreation.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipOnlineAccountCreation.Cursor = Cursors.Hand;
            lblSkipOnlineAccountCreation.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblSkipOnlineAccountCreation, "tooltips.oobeConfig.skipOnlineAccountCreation");

            toggleSkipOnlineAccountCreation = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipOnlineAccountCreation);
            toggleSkipOnlineAccountCreation.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Skip Wireless Network
            lblSkipWirelessNetwork = new Label();
            lblSkipWirelessNetwork.Location = new Point(labelX, currentY);
            lblSkipWirelessNetwork.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipWirelessNetwork.Text = lang.GetString("oobeConfig.skipWirelessNetwork.label");
            lblSkipWirelessNetwork.Font = theme.GetFont("normal");
            lblSkipWirelessNetwork.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipWirelessNetwork.Cursor = Cursors.Hand;
            lblSkipWirelessNetwork.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblSkipWirelessNetwork, "tooltips.oobeConfig.skipWirelessNetwork");

            toggleSkipWirelessNetwork = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipWirelessNetwork);
            toggleSkipWirelessNetwork.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Skip Machine OOBE
            lblSkipMachineOOBE = new Label();
            lblSkipMachineOOBE.Location = new Point(labelX, currentY);
            lblSkipMachineOOBE.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipMachineOOBE.Text = lang.GetString("oobeConfig.skipMachineOOBE.label");
            lblSkipMachineOOBE.Font = theme.GetFont("normal");
            lblSkipMachineOOBE.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipMachineOOBE.Cursor = Cursors.Hand;
            lblSkipMachineOOBE.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblSkipMachineOOBE, "tooltips.oobeConfig.skipMachineOOBE");

            toggleSkipMachineOOBE = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipMachineOOBE);
            toggleSkipMachineOOBE.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Skip User OOBE
            lblSkipUserOOBE = new Label();
            lblSkipUserOOBE.Location = new Point(labelX, currentY);
            lblSkipUserOOBE.Size = new Size(doubleLabelWidth, ui.GlobalLabelHeight);
            lblSkipUserOOBE.Text = lang.GetString("oobeConfig.skipUserOOBE.label");
            lblSkipUserOOBE.Font = theme.GetFont("normal");
            lblSkipUserOOBE.TextAlign = ContentAlignment.MiddleLeft;
            lblSkipUserOOBE.Cursor = Cursors.Hand;
            lblSkipUserOOBE.Click += (s, e) => OnToggleIndividualCheck();
            tooltips.SetToolTip(lblSkipUserOOBE, "tooltips.oobeConfig.skipUserOOBE");

            toggleSkipUserOOBE = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, SkipUserOOBE);
            toggleSkipUserOOBE.Click += (s, e) => OnToggleIndividualCheck();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;
            labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;

            // Line Separator
            pnlOOBEConfigAllAfterSeparator = new Panel();
            pnlOOBEConfigAllAfterSeparator.Location = new Point(labelX, currentY);
            pnlOOBEConfigAllAfterSeparator.Size = new Size(pnlOOBEConfigContent.Width - labelX - ui.GlobalTabX, 1);
            pnlOOBEConfigAllAfterSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            // Network Location
            lblNetworkLocation = new Label();
            lblNetworkLocation.Location = new Point(labelX, currentY);
            lblNetworkLocation.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblNetworkLocation.Text = lang.GetString("oobeConfig.networkLocation.label");
            lblNetworkLocation.Font = theme.GetFont("normal");
            lblNetworkLocation.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblNetworkLocation, "tooltips.oobeConfig.networkLocation");

            cmbNetworkLocation = new ComboBox();
            cmbNetworkLocation.Location = new Point(inputX, currentY);
            cmbNetworkLocation.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbNetworkLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNetworkLocation.BackColor = theme.GetColor("inputBackground");
            cmbNetworkLocation.ForeColor = theme.GetFontColor("inputForeground");
            cmbNetworkLocation.Items.AddRange(new string[]
            {
                lang.GetString("oobeConfig.networkLocation.option.selectOne"),
                lang.GetString("oobeConfig.networkLocation.option.home"),
                lang.GetString("oobeConfig.networkLocation.option.work"),
                lang.GetString("oobeConfig.networkLocation.option.public")
            });

            int statusRingExtra = 6;
            // Status ring for Network Location
            ringNetworkLocation = new StatusRing();
            ringNetworkLocation.Location = new Point(cmbNetworkLocation.Left - ui.GetValue("global.statusRing.borderWidth"), cmbNetworkLocation.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringNetworkLocation.Size = new Size(cmbNetworkLocation.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbNetworkLocation.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingExtra);
            ringNetworkLocation.Visible = false;

            cmbNetworkLocation.SelectedIndex = 0;
            cmbNetworkLocation.SelectedIndexChanged += onConfigChanged;
            cmbNetworkLocation.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    NetworkLocation = GetNetworkLocationValueFromIndex(cmbNetworkLocation.SelectedIndex);
                }
                ValidateNetworkLocation();
            };
            tooltips.SetToolTip(cmbNetworkLocation, "tooltips.oobeConfig.networkLocation");

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Protect Your PC
            lblProtectYourPC = new Label();
            lblProtectYourPC.Location = new Point(labelX, currentY);
            lblProtectYourPC.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblProtectYourPC.Text = lang.GetString("oobeConfig.protectYourPC.label");
            lblProtectYourPC.Font = theme.GetFont("normal");
            lblProtectYourPC.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblProtectYourPC, "tooltips.oobeConfig.protectYourPC");

            cmbProtectYourPC = new ComboBox();
            cmbProtectYourPC.Location = new Point(inputX, currentY);
            cmbProtectYourPC.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbProtectYourPC.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProtectYourPC.BackColor = theme.GetColor("inputBackground");
            cmbProtectYourPC.ForeColor = theme.GetFontColor("inputForeground");
            cmbProtectYourPC.Items.AddRange(new string[]
            {
                lang.GetString("oobeConfig.protectYourPC.option.selectOne"),
                lang.GetString("oobeConfig.protectYourPC.option.cloudTelemetry"),
                lang.GetString("oobeConfig.protectYourPC.option.privacyFocused")
            });

            // Status ring for Protect Your PC
            ringProtectYourPC = new StatusRing();
            ringProtectYourPC.Location = new Point(cmbProtectYourPC.Left - ui.GetValue("global.statusRing.borderWidth"), cmbProtectYourPC.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringProtectYourPC.Size = new Size(cmbProtectYourPC.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbProtectYourPC.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingExtra);
            ringProtectYourPC.Visible = false;

            cmbProtectYourPC.SelectedIndex = 0;
            cmbProtectYourPC.SelectedIndexChanged += onConfigChanged;
            cmbProtectYourPC.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    ProtectYourPC = GetProtectYourPCValueFromIndex(cmbProtectYourPC.SelectedIndex);
                }
                ValidateProtectYourPC();
            };
            tooltips.SetToolTip(cmbProtectYourPC, "tooltips.oobeConfig.protectYourPC");

            // Add all controls to content panel
            pnlOOBEConfigContent.Controls.Add(lblSkipAll);
            pnlOOBEConfigContent.Controls.Add(toggleSkipAll);
            pnlOOBEConfigContent.Controls.Add(pnlOOBEConfigAllSeparator);
            pnlOOBEConfigContent.Controls.Add(lblSkipEULA);
            pnlOOBEConfigContent.Controls.Add(toggleSkipEULA);
            pnlOOBEConfigContent.Controls.Add(lblSkipLocalAccountCreation);
            pnlOOBEConfigContent.Controls.Add(toggleSkipLocalAccountCreation);
            pnlOOBEConfigContent.Controls.Add(lblSkipOnlineAccountCreation);
            pnlOOBEConfigContent.Controls.Add(toggleSkipOnlineAccountCreation);
            pnlOOBEConfigContent.Controls.Add(lblSkipWirelessNetwork);
            pnlOOBEConfigContent.Controls.Add(toggleSkipWirelessNetwork);
            pnlOOBEConfigContent.Controls.Add(lblSkipMachineOOBE);
            pnlOOBEConfigContent.Controls.Add(toggleSkipMachineOOBE);
            pnlOOBEConfigContent.Controls.Add(lblSkipUserOOBE);
            pnlOOBEConfigContent.Controls.Add(toggleSkipUserOOBE);
            pnlOOBEConfigContent.Controls.Add(pnlOOBEConfigAllAfterSeparator);
            pnlOOBEConfigContent.Controls.Add(lblNetworkLocation);
            pnlOOBEConfigContent.Controls.Add(cmbNetworkLocation);
            pnlOOBEConfigContent.Controls.Add(ringNetworkLocation);
            pnlOOBEConfigContent.Controls.Add(lblProtectYourPC);
            pnlOOBEConfigContent.Controls.Add(cmbProtectYourPC);
            pnlOOBEConfigContent.Controls.Add(ringProtectYourPC);

            pnlOOBEConfig.Controls.Add(btnOOBEConfigToggle);
            pnlOOBEConfig.Controls.Add(lblOOBEConfigTitle);
            pnlOOBEConfig.Controls.Add(toggleEnableOOBE);
            pnlOOBEConfig.Controls.Add(lblEnableOOBE);
            pnlOOBEConfig.Controls.Add(pnlOOBEConfigSeparator);
            pnlOOBEConfig.Controls.Add(pnlOOBEConfigContent);

            // Apply current expansion state
            pnlOOBEConfigContent.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlOOBEConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            return pnlOOBEConfig;
        }

        public void LoadConfigIntoUI()
        {
            try
            {
                Console.WriteLine("OOBEConfig: Loading config into UI...");
                UpdateControlsFromModel();
                Console.WriteLine("OOBEConfig: Config loaded into UI successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OOBEConfig: Error loading config into UI: {ex.Message}");
            }
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlOOBEConfigContent.Visible = _isExpanded;
            pnlOOBEConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("oobeConfig", "contentHeight", 570);
                pnlOOBEConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnOOBEConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.oobe"));
            }
            else
            {
                pnlOOBEConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnOOBEConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.oobe"));
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnOOBEConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnOOBEConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
            theme.UpdateToggleSwitchState(toggleEnableOOBE, enabled);
            EnableOOBE = enabled;
            
            // Update dependent controls muted state
            UpdateControlsFromModel();
        }

        /// <summary>
        /// Handles the toggle for Enable OOBE Config
        /// </summary>
        private void OnToggleEnableOOBE()
        {
            if (_isLoading) return;

            ThemeManager theme = ThemeManager.Instance;
            
            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleEnableOOBE);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableOOBE, newState);
            
            EnableOOBE = newState;

            // Enable or disable all dependent toggles
            bool enableControls = newState;
            
            toggleSkipAll.Enabled = enableControls;
            toggleSkipEULA.Enabled = enableControls;
            toggleSkipLocalAccountCreation.Enabled = enableControls;
            toggleSkipOnlineAccountCreation.Enabled = enableControls;
            toggleSkipWirelessNetwork.Enabled = enableControls;
            toggleSkipMachineOOBE.Enabled = enableControls;
            toggleSkipUserOOBE.Enabled = enableControls;
            cmbNetworkLocation.Enabled = enableControls;
            cmbProtectYourPC.Enabled = enableControls;

            // Update visual appearance based on enabled state
            bool isMuted = !newState;

            // Restore focus to the toggle switch to prevent scroll jumping
            toggleEnableOOBE.Focus();
            
            // Update all toggle switches to muted or normal state
            theme.UpdateToggleSwitchMutedState(toggleSkipAll, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleSkipEULA, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleSkipLocalAccountCreation, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleSkipOnlineAccountCreation, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleSkipWirelessNetwork, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleSkipMachineOOBE, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleSkipUserOOBE, isMuted);
            
            // Update all labels to muted or normal state
            if (isMuted)
            {
                lblEnableOOBE.Font = theme.GetFont("muted");
                lblEnableOOBE.ForeColor = theme.GetFontColor("muted");
                
                lblSkipAll.Font = theme.GetFont("muted");
                lblSkipAll.ForeColor = theme.GetFontColor("muted");
                lblSkipAll.Cursor = Cursors.Default;
                lblSkipEULA.Font = theme.GetFont("muted");
                lblSkipEULA.ForeColor = theme.GetFontColor("muted");
                lblSkipEULA.Cursor = Cursors.Default;
                lblSkipLocalAccountCreation.Font = theme.GetFont("muted");
                lblSkipLocalAccountCreation.ForeColor = theme.GetFontColor("muted");
                lblSkipLocalAccountCreation.Cursor = Cursors.Default;
                lblSkipOnlineAccountCreation.Font = theme.GetFont("muted");
                lblSkipOnlineAccountCreation.ForeColor = theme.GetFontColor("muted");
                lblSkipOnlineAccountCreation.Cursor = Cursors.Default;
                lblSkipWirelessNetwork.Font = theme.GetFont("muted");
                lblSkipWirelessNetwork.ForeColor = theme.GetFontColor("muted");
                lblSkipWirelessNetwork.Cursor = Cursors.Default;
                lblSkipMachineOOBE.Font = theme.GetFont("muted");
                lblSkipMachineOOBE.ForeColor = theme.GetFontColor("muted");
                lblSkipMachineOOBE.Cursor = Cursors.Default;
                lblSkipUserOOBE.Font = theme.GetFont("muted");
                lblSkipUserOOBE.ForeColor = theme.GetFontColor("muted");
                lblSkipUserOOBE.Cursor = Cursors.Default;
                lblNetworkLocation.Font = theme.GetFont("muted");
                lblNetworkLocation.ForeColor = theme.GetFontColor("muted");
                cmbNetworkLocation.Font = theme.GetFont("muted");
                cmbNetworkLocation.ForeColor = theme.GetFontColor("muted");
                lblProtectYourPC.Font = theme.GetFont("muted");
                lblProtectYourPC.ForeColor = theme.GetFontColor("muted");
                cmbProtectYourPC.Font = theme.GetFont("muted");
                cmbProtectYourPC.ForeColor = theme.GetFontColor("muted");
            }
            else
            {
                lblEnableOOBE.Font = theme.GetFont("normal");
                lblEnableOOBE.ForeColor = theme.GetFontColor("normal");
                
                lblSkipAll.Font = theme.GetFont("normal");
                lblSkipAll.ForeColor = theme.GetFontColor("normal");
                lblSkipAll.Cursor = Cursors.Hand;
                lblSkipEULA.Font = theme.GetFont("normal");
                lblSkipEULA.ForeColor = theme.GetFontColor("normal");
                lblSkipEULA.Cursor = Cursors.Hand;
                lblSkipLocalAccountCreation.Font = theme.GetFont("normal");
                lblSkipLocalAccountCreation.ForeColor = theme.GetFontColor("normal");
                lblSkipLocalAccountCreation.Cursor = Cursors.Hand;
                lblSkipOnlineAccountCreation.Font = theme.GetFont("normal");
                lblSkipOnlineAccountCreation.ForeColor = theme.GetFontColor("normal");
                lblSkipOnlineAccountCreation.Cursor = Cursors.Hand;
                lblSkipWirelessNetwork.Font = theme.GetFont("normal");
                lblSkipWirelessNetwork.ForeColor = theme.GetFontColor("normal");
                lblSkipWirelessNetwork.Cursor = Cursors.Hand;
                lblSkipMachineOOBE.Font = theme.GetFont("normal");
                lblSkipMachineOOBE.ForeColor = theme.GetFontColor("normal");
                lblSkipMachineOOBE.Cursor = Cursors.Hand;
                lblSkipUserOOBE.Font = theme.GetFont("normal");
                lblSkipUserOOBE.ForeColor = theme.GetFontColor("normal");
                lblSkipUserOOBE.Cursor = Cursors.Hand;
                
                lblNetworkLocation.Font = theme.GetFont("normal");
                lblNetworkLocation.ForeColor = theme.GetFontColor("normal");
                cmbNetworkLocation.Font = theme.GetFont("normal");
                cmbNetworkLocation.ForeColor = theme.GetFontColor("inputForeground");

                lblProtectYourPC.Font = theme.GetFont("normal");
                lblProtectYourPC.ForeColor = theme.GetFontColor("normal");
                cmbProtectYourPC.Font = theme.GetFont("normal");
                cmbProtectYourPC.ForeColor = theme.GetFontColor("inputForeground");
            }

            // Notify MainForm to update lock/unlock button
            _onEnableToggle?.Invoke();
        }

        /// <summary>
        /// Handles the toggle for oobe All Checks
        /// </summary>
        private void OnToggleSkipAll()
        {
            if (_isLoading || !EnableOOBE) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleSkipAll);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleSkipAll, newState);

            SkipAll = newState;

            // If enabling, turn on all individual oobees
            if (newState)
            {
                theme.UpdateToggleSwitchState(toggleSkipEULA, true);
                theme.UpdateToggleSwitchState(toggleSkipLocalAccountCreation, true);
                theme.UpdateToggleSwitchState(toggleSkipOnlineAccountCreation, true);
                theme.UpdateToggleSwitchState(toggleSkipWirelessNetwork, true);
                theme.UpdateToggleSwitchState(toggleSkipMachineOOBE, true);
                theme.UpdateToggleSwitchState(toggleSkipUserOOBE, true);

                SkipEULA = true;
                SkipLocalAccountCreation = true;
                SkipOnlineAccountCreation = true;
                SkipWirelessNetwork = true;
                SkipMachineOOBE = true;
                SkipUserOOBE = true;
            }
            else
            {
                // If disabling, turn off all individual oobees
                theme.UpdateToggleSwitchState(toggleSkipEULA, false);
                theme.UpdateToggleSwitchState(toggleSkipLocalAccountCreation, false);
                theme.UpdateToggleSwitchState(toggleSkipOnlineAccountCreation, false);
                theme.UpdateToggleSwitchState(toggleSkipWirelessNetwork, false);
                theme.UpdateToggleSwitchState(toggleSkipMachineOOBE, false);
                theme.UpdateToggleSwitchState(toggleSkipUserOOBE, false);

                SkipEULA = false;
                SkipLocalAccountCreation = false;
                SkipOnlineAccountCreation = false;
                SkipWirelessNetwork = false;
                SkipMachineOOBE = false;
                SkipUserOOBE = false;
            }
            
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle for individual oobe checks
        /// </summary>
        private void OnToggleIndividualCheck()
        {
            if (_isLoading || !EnableOOBE) return;

            ThemeManager theme = ThemeManager.Instance;
            Panel? sender = null;
            
            // Get the sender from the active control or mouse position
            Control? activeControl = pnlOOBEConfigContent.GetChildAtPoint(
                pnlOOBEConfigContent.PointToClient(Cursor.Position));
            sender = activeControl as Panel;
            
            if (sender == null) return;
            
            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(sender);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(sender, newState);

            // Update data model
            if (ReferenceEquals(sender, toggleSkipEULA)) SkipEULA = newState;
            else if (ReferenceEquals(sender, toggleSkipLocalAccountCreation)) SkipLocalAccountCreation = newState;
            else if (ReferenceEquals(sender, toggleSkipOnlineAccountCreation)) SkipOnlineAccountCreation = newState;
            else if (ReferenceEquals(sender, toggleSkipWirelessNetwork)) SkipWirelessNetwork = newState;
            else if (ReferenceEquals(sender, toggleSkipMachineOOBE)) SkipMachineOOBE = newState;
            else if (ReferenceEquals(sender, toggleSkipUserOOBE)) SkipUserOOBE = newState;
            
            // Update oobe All toggle based on all individual toggles
            bool allEnabled = SkipEULA && SkipLocalAccountCreation && SkipOnlineAccountCreation && 
                            SkipWirelessNetwork && SkipMachineOOBE && SkipUserOOBE;
            
            if (allEnabled != SkipAll)
            {
                SkipAll = allEnabled;
                theme.UpdateToggleSwitchState(toggleSkipAll, allEnabled);
            }
            
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Data Model Operations

        public void UpdateFromControls()
        {
            if (_isLoading) return;
            
            // Check if UI is initialized
            if (toggleSkipAll == null ||
                toggleSkipEULA == null ||
                toggleSkipLocalAccountCreation == null ||
                toggleSkipOnlineAccountCreation == null ||
                toggleSkipWirelessNetwork == null ||
                toggleSkipMachineOOBE == null ||
                toggleSkipUserOOBE == null ||
                cmbNetworkLocation == null ||
                cmbProtectYourPC == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }
            
            ThemeManager theme = ThemeManager.Instance;
            
            // EnableOOBE should never be saved - it's a safety lock that always resets to true
            // EnableOOBE = theme.GetToggleSwitchState(toggleEnableOOBE);
            
            SkipAll = theme.GetToggleSwitchState(toggleSkipAll);
            SkipEULA = theme.GetToggleSwitchState(toggleSkipEULA);
            SkipLocalAccountCreation = theme.GetToggleSwitchState(toggleSkipLocalAccountCreation);
            SkipOnlineAccountCreation = theme.GetToggleSwitchState(toggleSkipOnlineAccountCreation);
            SkipWirelessNetwork = theme.GetToggleSwitchState(toggleSkipWirelessNetwork);
            SkipMachineOOBE = theme.GetToggleSwitchState(toggleSkipMachineOOBE);
            SkipUserOOBE = theme.GetToggleSwitchState(toggleSkipUserOOBE);
            
            // Update NetworkLocation from ComboBox using helper method
            NetworkLocation = GetNetworkLocationValueFromIndex(cmbNetworkLocation?.SelectedIndex ?? 0);
            
            // Update ProtectYourPC from ComboBox using helper method
            ProtectYourPC = GetProtectYourPCValueFromIndex(cmbProtectYourPC?.SelectedIndex ?? 0);
        }

        public void ClearControls()
        {
            _isLoading = true;
            try
            {
                ThemeManager theme = ThemeManager.Instance;

                EnableOOBE = true;
                
                // Reset all other toggles to off state
                theme.UpdateToggleSwitchState(toggleSkipAll, false);
                theme.UpdateToggleSwitchState(toggleSkipEULA, false);
                theme.UpdateToggleSwitchState(toggleSkipLocalAccountCreation, false);
                theme.UpdateToggleSwitchState(toggleSkipOnlineAccountCreation, false);
                theme.UpdateToggleSwitchState(toggleSkipWirelessNetwork, false);
                theme.UpdateToggleSwitchState(toggleSkipMachineOOBE, false);
                theme.UpdateToggleSwitchState(toggleSkipUserOOBE, false);
                
                // Enable all dependent toggles (since EnableOOBE is true)
                toggleSkipAll.Enabled = true;
                toggleSkipEULA.Enabled = true;
                toggleSkipLocalAccountCreation.Enabled = true;
                toggleSkipOnlineAccountCreation.Enabled = true;
                toggleSkipWirelessNetwork.Enabled = true;
                toggleSkipMachineOOBE.Enabled = true;
                toggleSkipUserOOBE.Enabled = true;
                cmbNetworkLocation.Enabled = true;
                cmbProtectYourPC.Enabled = true;
                
                // Apply normal visual state (not muted)
                theme.UpdateToggleSwitchMutedState(toggleSkipAll, false);
                theme.UpdateToggleSwitchMutedState(toggleSkipEULA, false);
                theme.UpdateToggleSwitchMutedState(toggleSkipLocalAccountCreation, false);
                theme.UpdateToggleSwitchMutedState(toggleSkipOnlineAccountCreation, false);
                theme.UpdateToggleSwitchMutedState(toggleSkipWirelessNetwork, false);
                theme.UpdateToggleSwitchMutedState(toggleSkipMachineOOBE, false);
                theme.UpdateToggleSwitchMutedState(toggleSkipUserOOBE, false);
                
                // Update all labels to normal state
                lblSkipAll.Font = theme.GetFont("normal");
                lblSkipAll.ForeColor = theme.GetFontColor("normal");
                lblSkipAll.Cursor = Cursors.Hand;
                lblSkipEULA.Font = theme.GetFont("normal");
                lblSkipEULA.ForeColor = theme.GetFontColor("normal");
                lblSkipEULA.Cursor = Cursors.Hand;
                lblSkipLocalAccountCreation.Font = theme.GetFont("normal");
                lblSkipLocalAccountCreation.ForeColor = theme.GetFontColor("normal");
                lblSkipLocalAccountCreation.Cursor = Cursors.Hand;
                lblSkipOnlineAccountCreation.Font = theme.GetFont("normal");
                lblSkipOnlineAccountCreation.ForeColor = theme.GetFontColor("normal");
                lblSkipOnlineAccountCreation.Cursor = Cursors.Hand;
                lblSkipWirelessNetwork.Font = theme.GetFont("normal");
                lblSkipWirelessNetwork.ForeColor = theme.GetFontColor("normal");
                lblSkipWirelessNetwork.Cursor = Cursors.Hand;
                lblSkipMachineOOBE.Font = theme.GetFont("normal");
                lblSkipMachineOOBE.ForeColor = theme.GetFontColor("normal");
                lblSkipMachineOOBE.Cursor = Cursors.Hand;
                lblSkipUserOOBE.Font = theme.GetFont("normal");
                lblSkipUserOOBE.ForeColor = theme.GetFontColor("normal");
                lblSkipUserOOBE.Cursor = Cursors.Hand;

                lblNetworkLocation.Font = theme.GetFont("normal");
                lblNetworkLocation.ForeColor = theme.GetFontColor("normal");
                cmbNetworkLocation.Font = theme.GetFont("normal");
                cmbNetworkLocation.ForeColor = theme.GetFontColor("inputForeground");

                lblProtectYourPC.Font = theme.GetFont("normal");
                lblProtectYourPC.ForeColor = theme.GetFontColor("normal");
                cmbProtectYourPC.Font = theme.GetFont("normal");
                cmbProtectYourPC.ForeColor = theme.GetFontColor("inputForeground");

                // Reset ComboBoxes to default selection
                if (cmbNetworkLocation != null) cmbNetworkLocation.SelectedIndex = 0;
                if (cmbProtectYourPC != null) cmbProtectYourPC.SelectedIndex = 0;
                
                // Reset ComboBox labels to normal state
                if (lblNetworkLocation != null)
                {
                    lblNetworkLocation.Font = theme.GetFont("normal");
                    lblNetworkLocation.ForeColor = theme.GetFontColor("normal");
                }
                if (lblProtectYourPC != null)
                {
                    lblProtectYourPC.Font = theme.GetFont("normal");
                    lblProtectYourPC.ForeColor = theme.GetFontColor("normal");
                }
                
                // Reset data model (except EnableOOBE which stays true)
                SkipAll = false;
                SkipEULA = false;
                SkipLocalAccountCreation = false;
                SkipOnlineAccountCreation = false;
                SkipWirelessNetwork = false;
                SkipMachineOOBE = false;
                SkipUserOOBE = false;
                NetworkLocation = "";
                ProtectYourPC = "";
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
            if (toggleSkipAll == null ||
                toggleSkipEULA == null ||
                toggleSkipLocalAccountCreation == null ||
                toggleSkipOnlineAccountCreation == null ||
                toggleSkipWirelessNetwork == null ||
                toggleSkipMachineOOBE == null ||
                toggleSkipUserOOBE == null ||
                cmbNetworkLocation == null ||
                cmbProtectYourPC == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: SkipAll={SkipAll}, SkipEULA={SkipEULA}, SkipLocalAccountCreation={SkipLocalAccountCreation}, SkipOnlineAccountCreation={SkipOnlineAccountCreation}, SkipWirelessNetwork={SkipWirelessNetwork}, SkipMachineOOBE={SkipMachineOOBE}, SkipUserOOBE={SkipUserOOBE}, NetworkLocation={NetworkLocation}, ProtectYourPC={ProtectYourPC}");

            try
            {
                ThemeManager theme = ThemeManager.Instance;
                
                // Update all toggle states
                theme.UpdateToggleSwitchState(toggleEnableOOBE, EnableOOBE);
                theme.UpdateToggleSwitchState(toggleSkipAll, SkipAll);
                theme.UpdateToggleSwitchState(toggleSkipEULA, SkipEULA);
                theme.UpdateToggleSwitchState(toggleSkipLocalAccountCreation, SkipLocalAccountCreation);
                theme.UpdateToggleSwitchState(toggleSkipOnlineAccountCreation, SkipOnlineAccountCreation);
                theme.UpdateToggleSwitchState(toggleSkipWirelessNetwork, SkipWirelessNetwork);
                theme.UpdateToggleSwitchState(toggleSkipMachineOOBE, SkipMachineOOBE);
                theme.UpdateToggleSwitchState(toggleSkipUserOOBE, SkipUserOOBE);

                // Enable/disable dependent toggles based on EnableOOBE
                bool enableControls = EnableOOBE;
                
                toggleSkipAll.Enabled = enableControls;
                toggleSkipEULA.Enabled = enableControls;
                toggleSkipLocalAccountCreation.Enabled = enableControls;
                toggleSkipOnlineAccountCreation.Enabled = enableControls;
                toggleSkipWirelessNetwork.Enabled = enableControls;
                toggleSkipMachineOOBE.Enabled = enableControls;
                toggleSkipUserOOBE.Enabled = enableControls;
                cmbNetworkLocation.Enabled = enableControls;
                cmbProtectYourPC.Enabled = enableControls;
                
                // Update visual appearance based on enabled state
                bool isMuted = !EnableOOBE;
                
                // Update all toggle switches to muted or normal state
                theme.UpdateToggleSwitchMutedState(toggleSkipAll, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleSkipEULA, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleSkipLocalAccountCreation, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleSkipOnlineAccountCreation, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleSkipWirelessNetwork, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleSkipMachineOOBE, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleSkipUserOOBE, isMuted);
                
                // Update all labels to muted or normal state
                if (isMuted)
                {
                    lblEnableOOBE.Font = theme.GetFont("muted");
                    lblEnableOOBE.ForeColor = theme.GetFontColor("muted");

                    lblSkipAll.Font = theme.GetFont("muted");
                    lblSkipAll.ForeColor = theme.GetFontColor("muted");
                    lblSkipAll.Cursor = Cursors.Default;
                    lblSkipEULA.Font = theme.GetFont("muted");
                    lblSkipEULA.ForeColor = theme.GetFontColor("muted");
                    lblSkipEULA.Cursor = Cursors.Default;
                    lblSkipLocalAccountCreation.Font = theme.GetFont("muted");
                    lblSkipLocalAccountCreation.ForeColor = theme.GetFontColor("muted");
                    lblSkipLocalAccountCreation.Cursor = Cursors.Default;
                    lblSkipOnlineAccountCreation.Font = theme.GetFont("muted");
                    lblSkipOnlineAccountCreation.ForeColor = theme.GetFontColor("muted");
                    lblSkipOnlineAccountCreation.Cursor = Cursors.Default;
                    lblSkipWirelessNetwork.Font = theme.GetFont("muted");
                    lblSkipWirelessNetwork.ForeColor = theme.GetFontColor("muted");
                    lblSkipWirelessNetwork.Cursor = Cursors.Default;
                    lblSkipMachineOOBE.Font = theme.GetFont("muted");
                    lblSkipMachineOOBE.ForeColor = theme.GetFontColor("muted");
                    lblSkipMachineOOBE.Cursor = Cursors.Default;
                    lblSkipUserOOBE.Font = theme.GetFont("muted");
                    lblSkipUserOOBE.ForeColor = theme.GetFontColor("muted");
                    lblSkipUserOOBE.Cursor = Cursors.Default;

                    lblNetworkLocation.Font = theme.GetFont("muted");
                    lblNetworkLocation.ForeColor = theme.GetFontColor("muted");
                    cmbNetworkLocation.Font = theme.GetFont("muted");
                    cmbNetworkLocation.ForeColor = theme.GetFontColor("muted");

                    lblProtectYourPC.Font = theme.GetFont("muted");
                    lblProtectYourPC.ForeColor = theme.GetFontColor("muted");
                    cmbProtectYourPC.Font = theme.GetFont("muted");
                    cmbProtectYourPC.ForeColor = theme.GetFontColor("muted");
                }
                else
                {
                    lblEnableOOBE.Font = theme.GetFont("normal");
                    lblEnableOOBE.ForeColor = theme.GetFontColor("normal");

                    lblSkipAll.Font = theme.GetFont("normal");
                    lblSkipAll.ForeColor = theme.GetFontColor("normal");
                    lblSkipAll.Cursor = Cursors.Hand;
                    lblSkipEULA.Font = theme.GetFont("normal");
                    lblSkipEULA.ForeColor = theme.GetFontColor("normal");
                    lblSkipEULA.Cursor = Cursors.Hand;
                    lblSkipLocalAccountCreation.Font = theme.GetFont("normal");
                    lblSkipLocalAccountCreation.ForeColor = theme.GetFontColor("normal");
                    lblSkipLocalAccountCreation.Cursor = Cursors.Hand;
                    lblSkipOnlineAccountCreation.Font = theme.GetFont("normal");
                    lblSkipOnlineAccountCreation.ForeColor = theme.GetFontColor("normal");
                    lblSkipOnlineAccountCreation.Cursor = Cursors.Hand;
                    lblSkipWirelessNetwork.Font = theme.GetFont("normal");
                    lblSkipWirelessNetwork.ForeColor = theme.GetFontColor("normal");
                    lblSkipWirelessNetwork.Cursor = Cursors.Hand;
                    lblSkipMachineOOBE.Font = theme.GetFont("normal");
                    lblSkipMachineOOBE.ForeColor = theme.GetFontColor("normal");
                    lblSkipMachineOOBE.Cursor = Cursors.Hand;
                    lblSkipUserOOBE.Font = theme.GetFont("normal");
                    lblSkipUserOOBE.ForeColor = theme.GetFontColor("normal");
                    lblSkipUserOOBE.Cursor = Cursors.Hand;

                    lblNetworkLocation.Font = theme.GetFont("normal");
                    lblNetworkLocation.ForeColor = theme.GetFontColor("normal");
                    cmbNetworkLocation.Font = theme.GetFont("normal");
                    cmbNetworkLocation.ForeColor = theme.GetFontColor("inputForeground");

                    lblProtectYourPC.Font = theme.GetFont("normal");
                    lblProtectYourPC.ForeColor = theme.GetFontColor("normal");
                    cmbProtectYourPC.Font = theme.GetFont("normal");
                    cmbProtectYourPC.ForeColor = theme.GetFontColor("inputForeground");
                }
                
                // Update ComboBox selections from model using helper methods
                // Note: Setting SelectedIndex will trigger SelectedIndexChanged event,
                // which will call validation methods automatically
                cmbNetworkLocation.SelectedIndex = GetIndexFromNetworkLocationValue(NetworkLocation);
                cmbProtectYourPC.SelectedIndex = GetIndexFromProtectYourPCValue(ProtectYourPC);
            
                // Validate after loading
                ValidateAllUIFields();

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
                // EnableOOBE should never be loaded from JSON - always stays true (safety feature)
                
                if (json.skipAll != null) SkipAll = ParseBool(json.skipAll);
                if (json.skipEULA != null) SkipEULA = ParseBool(json.skipEULA);
                if (json.skipLocalAccountCreation != null) SkipLocalAccountCreation = ParseBool(json.skipLocalAccountCreation);
                if (json.skipOnlineAccountCreation != null) SkipOnlineAccountCreation = ParseBool(json.skipOnlineAccountCreation);
                if (json.skipWirelessNetwork != null) SkipWirelessNetwork = ParseBool(json.skipWirelessNetwork);
                if (json.skipMachineOOBE != null) SkipMachineOOBE = ParseBool(json.skipMachineOOBE);
                if (json.skipUserOOBE != null) SkipUserOOBE = ParseBool(json.skipUserOOBE);
                
                // Load NetworkLocation and ProtectYourPC
                if (json.networkLocation != null) NetworkLocation = json.networkLocation.ToString();
                if (json.protectYourPC != null) ProtectYourPC = json.protectYourPC.ToString();
            }
            catch { }
        }

        #endregion

        #region Value Mapping Helpers

        /// <summary>
        /// Maps dropdown index to NetworkLocation value (Home, Work, Public)
        /// </summary>
        private string GetNetworkLocationValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "Home";
                case 2: return "Work";
                case 3: return "Public";
                default: return "";
            }
        }

        /// <summary>
        /// Maps NetworkLocation value to dropdown index
        /// </summary>
        private int GetIndexFromNetworkLocationValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value.ToUpper())
            {
                case "HOME": return 1;
                case "WORK": return 2;
                case "PUBLIC": return 3;
                default: return 0;
            }
        }

        /// <summary>
        /// Maps dropdown index to ProtectYourPC value (1 or 3)
        /// </summary>
        private string GetProtectYourPCValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "1"; // Express Settings (Cloud/Telemetry)
                case 2: return "3"; // Privacy Settings
                default: return "";
            }
        }

        /// <summary>
        /// Maps ProtectYourPC value to dropdown index
        /// </summary>
        private int GetIndexFromProtectYourPCValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value)
            {
                case "1": return 1; // Express Settings
                case "3": return 2; // Privacy Settings
                default: return 0;
            }
        }

        /// <summary>
        /// Helper method to parse boolean values from JSON (handles bool, int, string)
        /// </summary>
        private static bool ParseBool(dynamic value)
        {
            if (value == null) return false;
            
            try
            {
                // If it's already a bool, return it
                if (value is bool boolValue) return boolValue;
                
                // Convert to string and try parsing
                string strValue = value.ToString().ToLower().Trim();
                
                // Handle "true"/"false" strings
                if (strValue == "true") return true;
                if (strValue == "false") return false;
                
                // Handle numeric strings "1"/"0"
                if (int.TryParse(strValue, out int intValue))
                    return intValue != 0;
                
                // Handle decimal numbers
                if (double.TryParse(strValue, out double doubleValue))
                    return Math.Abs(doubleValue) > double.Epsilon;
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns oobe configuration values as a dictionary for XML generation
        /// </summary>
        public Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            // Return oobe values as strings for XML placeholders
            values["SkipEULA"] = SkipEULA ? "true" : "false";
            values["SkipLocalAccountCreation"] = SkipLocalAccountCreation ? "true" : "false";
            values["SkipOnlineAccountCreation"] = SkipOnlineAccountCreation ? "true" : "false";
            values["SkipWirelessNetwork"] = SkipWirelessNetwork ? "true" : "false";
            values["SkipMachineOOBE"] = SkipMachineOOBE ? "true" : "false";
            values["SkipUserOOBE"] = SkipUserOOBE ? "true" : "false";
            values["NetworkLocation"] = NetworkLocation ?? "";
            values["ProtectYourPC"] = ProtectYourPC ?? "";

            return values;
        }
        
        #endregion

        #region UI Validation Methods

        /// <summary>
        /// Validates all ComboBox selections and updates status rings
        /// </summary>
        public void ValidateAllUIFields()
        {
            ValidateNetworkLocation();
            ValidateProtectYourPC();
        }

        /// <summary>
        /// Validates NetworkLocation ComboBox selection and updates status ring
        /// </summary>
        private void ValidateNetworkLocation()
        {
            if (ringNetworkLocation == null || cmbNetworkLocation == null)
                return;

            if (cmbNetworkLocation.SelectedIndex > 0 && !string.IsNullOrWhiteSpace(cmbNetworkLocation.SelectedItem?.ToString()))
            {
                ringNetworkLocation.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringNetworkLocation.SetStatus(ValidationStatus.Invalid);
            }
        }

        /// <summary>
        /// Validates ProtectYourPC ComboBox selection and updates status ring
        /// </summary>
        private void ValidateProtectYourPC()
        {
            if (ringProtectYourPC == null || cmbProtectYourPC == null)
                return;

            if (cmbProtectYourPC.SelectedIndex > 0 && !string.IsNullOrWhiteSpace(cmbProtectYourPC.SelectedItem?.ToString()))
            {
                ringProtectYourPC.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringProtectYourPC.SetStatus(ValidationStatus.Invalid);
            }
        }

        /// <summary>
        /// Validates the OOBE configuration and returns list of validation errors
        /// </summary>
        public List<string> Validate()
        {
            List<string> errors = new List<string>();
            
            // Only validate if OOBE config is enabled
            if (!EnableOOBE)
                return errors;
            
            LangManager lang = LangManager.Instance;
            
            // Validate NetworkLocation
            if (string.IsNullOrWhiteSpace(NetworkLocation))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("oobeConfig.networkLocation.label")));
            }
            
            // Validate ProtectYourPC
            if (string.IsNullOrWhiteSpace(ProtectYourPC))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("oobeConfig.protectYourPC.label")));
            }
            
            return errors;
        }

        #endregion
    }
}
