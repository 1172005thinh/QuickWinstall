using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles General Configuration section including UI, data model, and business logic
    /// </summary>
    public class GeneralConfig
    {
        #region Data Model

        public bool EnableGeneral { get; set; } = true;
        public string WindowsEdition { get; set; } = "";
        public string ProductKey { get; set; } = "";
        public string CPUArchitecture { get; set; } = "";

        #endregion

        #region UI Components

        private Panel pnlGeneralConfig = null!;
        private Button btnGeneralConfigToggle = null!;
        private Label lblGeneralConfigTitle = null!;
        private Panel pnlGeneralConfigSeparator = null!;
        private Panel pnlGeneralConfigContent = null!;

        private Label lblEnableGeneral = null!;
        private Panel toggleEnableGeneral = null!;

        // Windows Edition
        private Label lblWindowsEdition = null!;
        private ComboBox cmbWindowsEdition = null!;
        private StatusRing ringWindowsEdition = null!;

        // Product Key
        private Label lblProductKey = null!;
        private TextBox txtProductKey1 = null!;
        private Label lblHyphen1 = null!;
        private TextBox txtProductKey2 = null!;
        private Label lblHyphen2 = null!;
        private TextBox txtProductKey3 = null!;
        private Label lblHyphen3 = null!;
        private TextBox txtProductKey4 = null!;
        private Label lblHyphen4 = null!;
        private TextBox txtProductKey5 = null!;
        private StatusRing ringProductKey = null!;

        // CPU Architecture
        private Label lblCPUArch = null!;
        private ComboBox cmbCPUArch = null!;
        private StatusRing ringCPUArch = null!;

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

        /// <summary>
        /// Initializes the General Config section UI and returns the main panel
        /// </summary>
        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged, 
            Func<Button> createRoundedButton, Action? onSectionToggle, Action? onEnableToggle, Control parentForm)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            // Store callback
            _onSectionToggle = onSectionToggle;
            _onEnableToggle = onEnableToggle;
            _onConfigChanged = onConfigChanged;

            int contentHeight = ui.GetSectionValue("generalConfig", "contentHeight", 230);
            
            // General Config Panel
            pnlGeneralConfig = new Panel();
            pnlGeneralConfig.Location = new Point(0, 0);
            pnlGeneralConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlGeneralConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // General Config Toggle Button
            btnGeneralConfigToggle = createRoundedButton();
            btnGeneralConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnGeneralConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnGeneralConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnGeneralConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnGeneralConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnGeneralConfigToggle, _isExpanded ? "tooltips.section.expand" : "tooltips.section.collapse", lang.GetString("mainForm.sections.general"));

            // General Config Title
            lblGeneralConfigTitle = new Label();
            lblGeneralConfigTitle.Location = new Point(btnGeneralConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblGeneralConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblGeneralConfigTitle.Text = lang.GetString("mainForm.sections.general");
            lblGeneralConfigTitle.Font = theme.GetFont("subheader");
            lblGeneralConfigTitle.UseMnemonic = false;
            lblGeneralConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblGeneralConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblGeneralConfigTitle.Cursor = Cursors.Hand;
            lblGeneralConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblGeneralConfigTitle, "tooltips.generalConfig.header", lang.GetString("tooltips.generalConfig.header"));

            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);

            // Enable General Config
            // Only set EnableGeneral from settings on first initialization
            if (_isFirstInitialization)
            {
                EnableGeneral = !SettingsManager.Instance.LockSectionsAtStartup;
                _isFirstInitialization = false;
            }
            toggleEnableGeneral = theme.CreateToggleSwitch(new Point(ui.GlobalTabX * 2 + ui.GlobalBtnBox, btnGeneralConfigToggle.Bottom + ui.GlobalSpacingY), toggleWidth, ui.GlobalInputHeight, EnableGeneral);
            toggleEnableGeneral.TabStop = false; // Skip this control in tab order
            toggleEnableGeneral.Click += (s, e) => OnToggleEnableGeneral();

            lblEnableGeneral = new Label();
            lblEnableGeneral.Location = new Point(toggleEnableGeneral.Right + ui.GlobalSpacingX, btnGeneralConfigToggle.Bottom + ui.GlobalSpacingY);
            lblEnableGeneral.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblEnableGeneral.Text = string.Format(lang.GetString("generalConfig.enableGeneral.label"), lang.GetString("mainForm.sections.general"));
            lblEnableGeneral.Font = theme.GetFont("normal");
            lblEnableGeneral.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableGeneral.Cursor = Cursors.Hand;
            lblEnableGeneral.Click += (s, e) => OnToggleEnableGeneral();
            tooltips.SetToolTip(lblEnableGeneral, "tooltips.generalConfig.enableGeneral");

            // Line Separator
            pnlGeneralConfigSeparator = new Panel();
            pnlGeneralConfigSeparator.Location = new Point(ui.GlobalTabX, toggleEnableGeneral.Bottom + ui.GlobalSpacingY * 2);
            pnlGeneralConfigSeparator.Size = new Size(pnlGeneralConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlGeneralConfigSeparator.BackColor = theme.GetColor("separator");

            // General Config Content Panel
            pnlGeneralConfigContent = new Panel();
            pnlGeneralConfigContent.Location = new Point(0, pnlGeneralConfigSeparator.Bottom + ui.GlobalSpacingY / 2);
            pnlGeneralConfigContent.Size = new Size(pnlGeneralConfig.Width, contentHeight);
            pnlGeneralConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlGeneralConfigContent.AutoScroll = false;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int currentY = ui.GlobalSpacingY;

            // Windows Edition
            lblWindowsEdition = new Label();
            lblWindowsEdition.Location = new Point(labelX, currentY);
            lblWindowsEdition.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblWindowsEdition.Text = lang.GetString("generalConfig.windowsEdition.label");
            lblWindowsEdition.Font = theme.GetFont("normal");
            lblWindowsEdition.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblWindowsEdition, "tooltips.generalConfig.windowsEdition");

            cmbWindowsEdition = new ComboBox();
            cmbWindowsEdition.Location = new Point(inputX, currentY);
            cmbWindowsEdition.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbWindowsEdition.FlatStyle = FlatStyle.Flat;
            cmbWindowsEdition.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWindowsEdition.BackColor = theme.GetColor("inputBackground");
            cmbWindowsEdition.ForeColor = theme.GetFontColor("inputForeground");
            cmbWindowsEdition.Items.AddRange(new object[] {
                lang.GetString("generalConfig.windowsEdition.options.selectOne"),
                lang.GetString("generalConfig.windowsEdition.options.home"),
                lang.GetString("generalConfig.windowsEdition.options.pro"),
                lang.GetString("generalConfig.windowsEdition.options.education"),
                lang.GetString("generalConfig.windowsEdition.options.enterprise")
            });

            // Status ring for Windows Edition (create BEFORE setting SelectedIndex)
            int statusRingBorderExtra = 6;
            ringWindowsEdition = new StatusRing();
            ringWindowsEdition.Location = new Point(cmbWindowsEdition.Left - ui.GetValue("global.statusRing.borderWidth"), cmbWindowsEdition.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringWindowsEdition.Size = new Size(cmbWindowsEdition.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbWindowsEdition.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringWindowsEdition.Visible = false;
            
            cmbWindowsEdition.SelectedIndex = 0;
            cmbWindowsEdition.SelectedIndexChanged += onConfigChanged;
            cmbWindowsEdition.SelectedIndexChanged += (s, e) => ValidateWindowsEdition();
            tooltips.SetToolTip(cmbWindowsEdition, "tooltips.generalConfig.windowsEdition");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Product Key
            lblProductKey = new Label();
            lblProductKey.Location = new Point(labelX, currentY);
            lblProductKey.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblProductKey.Text = lang.GetString("generalConfig.productKey.label");
            lblProductKey.Font = theme.GetFont("normal");
            lblProductKey.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblProductKey, "tooltips.generalConfig.productKey");

            int pkWidth = (ui.GlobalInputWidth - 4 * ui.GlobalSpacingX) / 5;
            int pkX = inputX;

            // Status ring for Product Key (create BEFORE textboxes so event handlers can access it)
            ringProductKey = new StatusRing();
            ringProductKey.Location = new Point(inputX - ui.GetValue("global.statusRing.borderWidth"), currentY - ui.GetValue("global.statusRing.borderWidth"));
            ringProductKey.Size = new Size(ui.GlobalInputWidth + 2 * ui.GetValue("global.statusRing.borderWidth"), ui.GlobalInputHeight + 2 * ui.GetValue("global.statusRing.borderWidth"));
            ringProductKey.Visible = false;

            txtProductKey1 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight, parentForm);
            txtProductKey1.BackColor = theme.GetColor("inputBackground");
            txtProductKey1.Location = new Point(pkX, currentY);
            txtProductKey1.Tag = 1;
            txtProductKey1.TextChanged += (s, e) => ValidateProductKey();
            tooltips.SetToolTip(txtProductKey1, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen1 = new Label();
            lblHyphen1.Location = new Point(pkX, currentY);
            lblHyphen1.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen1.Text = "-";
            lblHyphen1.Font = theme.GetFont("normal");
            lblHyphen1.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey2 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight, parentForm);
            txtProductKey2.BackColor = theme.GetColor("inputBackground");
            txtProductKey2.Location = new Point(pkX, currentY);
            txtProductKey2.Tag = 2;
            txtProductKey2.TextChanged += (s, e) => ValidateProductKey();
            tooltips.SetToolTip(txtProductKey2, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen2 = new Label();
            lblHyphen2.Location = new Point(pkX, currentY);
            lblHyphen2.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen2.Text = "-";
            lblHyphen2.Font = theme.GetFont("normal");
            lblHyphen2.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey3 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight, parentForm);
            txtProductKey3.BackColor = theme.GetColor("inputBackground");
            txtProductKey3.Location = new Point(pkX, currentY);
            txtProductKey3.Tag = 3;
            txtProductKey3.Font = theme.GetFont("normal");
            txtProductKey3.TextChanged += (s, e) => ValidateProductKey();
            tooltips.SetToolTip(txtProductKey3, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen3 = new Label();
            lblHyphen3.Location = new Point(pkX, currentY);
            lblHyphen3.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen3.Text = "-";
            lblHyphen3.Font = theme.GetFont("normal");
            lblHyphen3.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey4 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight, parentForm);
            txtProductKey4.BackColor = theme.GetColor("inputBackground");
            txtProductKey4.Location = new Point(pkX, currentY);
            txtProductKey4.Tag = 4;
            txtProductKey4.TextChanged += (s, e) => ValidateProductKey();
            tooltips.SetToolTip(txtProductKey4, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen4 = new Label();
            lblHyphen4.Location = new Point(pkX, currentY);
            lblHyphen4.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen4.Text = "-";
            lblHyphen4.Font = theme.GetFont("normal");
            lblHyphen4.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey5 = CreateProductKeyTextBox(pkWidth, ui.GlobalInputHeight, parentForm);
            txtProductKey5.BackColor = theme.GetColor("inputBackground");
            txtProductKey5.Location = new Point(pkX, currentY);
            txtProductKey5.Tag = 5;
            txtProductKey5.TextChanged += (s, e) => ValidateProductKey();
            tooltips.SetToolTip(txtProductKey5, "tooltips.generalConfig.productKey");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // CPU Architecture
            lblCPUArch = new Label();
            lblCPUArch.Location = new Point(labelX, currentY);
            lblCPUArch.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblCPUArch.Text = lang.GetString("generalConfig.cpuArch.label");
            lblCPUArch.Font = theme.GetFont("normal");
            lblCPUArch.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblCPUArch, "tooltips.generalConfig.cpuArch");

            cmbCPUArch = new ComboBox();
            cmbCPUArch.Location = new Point(inputX, currentY);
            cmbCPUArch.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbCPUArch.FlatStyle = FlatStyle.Flat;
            cmbCPUArch.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCPUArch.BackColor = theme.GetColor("inputBackground");
            cmbCPUArch.ForeColor = theme.GetFontColor("inputForeground");
            cmbCPUArch.Items.AddRange(new object[] {
                lang.GetString("generalConfig.cpuArch.options.selectOne"),
                lang.GetString("generalConfig.cpuArch.options.x64"),
                lang.GetString("generalConfig.cpuArch.options.arm64")
            });
            
            // Status ring for CPU Architecture (create BEFORE setting SelectedIndex)
            ringCPUArch = new StatusRing();
            ringCPUArch.Location = new Point(cmbCPUArch.Left - ui.GetValue("global.statusRing.borderWidth"), cmbCPUArch.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringCPUArch.Size = new Size(cmbCPUArch.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbCPUArch.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringCPUArch.Visible = false;
            
            cmbCPUArch.SelectedIndex = 0;
            cmbCPUArch.SelectedIndexChanged += onConfigChanged;
            cmbCPUArch.SelectedIndexChanged += (s, e) => ValidateCPUArch();
            tooltips.SetToolTip(cmbCPUArch, "tooltips.generalConfig.cpuArch");

            // Add controls to General Config Content
            pnlGeneralConfigContent.Controls.Add(lblWindowsEdition);
            pnlGeneralConfigContent.Controls.Add(cmbWindowsEdition);
            pnlGeneralConfigContent.Controls.Add(lblProductKey);
            pnlGeneralConfigContent.Controls.Add(txtProductKey1);
            pnlGeneralConfigContent.Controls.Add(lblHyphen1);
            pnlGeneralConfigContent.Controls.Add(txtProductKey2);
            pnlGeneralConfigContent.Controls.Add(lblHyphen2);
            pnlGeneralConfigContent.Controls.Add(txtProductKey3);
            pnlGeneralConfigContent.Controls.Add(lblHyphen3);
            pnlGeneralConfigContent.Controls.Add(txtProductKey4);
            pnlGeneralConfigContent.Controls.Add(lblHyphen4);
            pnlGeneralConfigContent.Controls.Add(txtProductKey5);
            pnlGeneralConfigContent.Controls.Add(lblCPUArch);
            pnlGeneralConfigContent.Controls.Add(cmbCPUArch);

            // Add status rings
            pnlGeneralConfigContent.Controls.Add(ringWindowsEdition);
            pnlGeneralConfigContent.Controls.Add(ringProductKey);
            pnlGeneralConfigContent.Controls.Add(ringCPUArch);

            // Add controls to General Config Panel
            pnlGeneralConfig.Controls.Add(btnGeneralConfigToggle);
            pnlGeneralConfig.Controls.Add(lblGeneralConfigTitle);
            pnlGeneralConfig.Controls.Add(lblEnableGeneral);
            pnlGeneralConfig.Controls.Add(toggleEnableGeneral);
            pnlGeneralConfig.Controls.Add(pnlGeneralConfigSeparator);
            pnlGeneralConfig.Controls.Add(pnlGeneralConfigContent);

            // Apply current expansion state
            pnlGeneralConfigContent.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlGeneralConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            // Initialize placeholders
            InitializePlaceholders();

            // Note: Config will be loaded after InitializeForm() calls LoadLastConfig()
            // Then MainForm will call LoadConfigIntoUI() on this section

            return pnlGeneralConfig;
        }

        /// <summary>
        /// Loads the data model values into UI controls
        /// Called by MainForm after ConfigValues.LoadLastConfig() has loaded the data
        /// </summary>
        public void LoadConfigIntoUI()
        {
            try
            {
                Console.WriteLine("GeneralConfig: Loading config into UI...");
                UpdateControlsFromModel();
                Console.WriteLine("GeneralConfig: Config loaded into UI successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GeneralConfig: Error loading config into UI: {ex.Message}");
            }
        }

        #endregion

        #region UI Interactions

        /// <summary>
        /// Toggles the visibility of the section content
        /// </summary>
        public void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlGeneralConfigContent.Visible = _isExpanded;
            pnlGeneralConfigSeparator.Visible = _isExpanded;

            // Update panel height based on state
            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("generalConfig", "contentHeight", 230);
                pnlGeneralConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnGeneralConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.general"));
            }
            else
            {
                pnlGeneralConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnGeneralConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.general"));
            }

            // Update button icon
            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnGeneralConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnGeneralConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

            // Notify parent to reposition sections
            _onSectionToggle?.Invoke();
        }

        /// <summary>
        /// Expands the section
        /// </summary>
        public void Expand()
        {
            if (!_isExpanded)
                ToggleSection();
        }

        /// <summary>
        /// Collapses the section
        /// </summary>
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
            theme.UpdateToggleSwitchState(toggleEnableGeneral, enabled);
            EnableGeneral = enabled;

            // Update dependent controls muted state
            UpdateControlsFromModel();
        }
        
        /// <summary>
        /// Handles the toggle for Enable General Configurations
        /// </summary>
        private void OnToggleEnableGeneral()
        {
            if (_isLoading) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle state
            bool currentState = theme.GetToggleSwitchState(toggleEnableGeneral);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableGeneral, newState);

            EnableGeneral = newState;

            // Restore focus to the toggle switch to prevent scroll jumping
            toggleEnableGeneral.Focus();

            // Enable or disable all dependent controls
            bool enableControls = newState;
            
            cmbWindowsEdition.Enabled = enableControls;
            txtProductKey1.Enabled = enableControls;
            txtProductKey2.Enabled = enableControls;
            txtProductKey3.Enabled = enableControls;
            txtProductKey4.Enabled = enableControls;
            txtProductKey5.Enabled = enableControls;
            cmbCPUArch.Enabled = enableControls;

            // Update font styles based on enabled state
            if (enableControls)
            {
                // Normal state
                lblEnableGeneral.Font = theme.GetFont("normal");
                lblEnableGeneral.ForeColor = theme.GetFontColor("normal");
                
                lblWindowsEdition.Font = theme.GetFont("normal");
                lblWindowsEdition.ForeColor = theme.GetFontColor("normal");
                cmbWindowsEdition.Font = theme.GetFont("normal");
                cmbWindowsEdition.ForeColor = theme.GetFontColor("inputForeground");
                
                lblProductKey.Font = theme.GetFont("normal");
                lblProductKey.ForeColor = theme.GetFontColor("normal");
                
                // For product key textboxes, keep placeholder color if they have placeholder text
                txtProductKey1.Font = theme.GetFont("normal");
                txtProductKey1.ForeColor = (txtProductKey1.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                txtProductKey2.Font = theme.GetFont("normal");
                txtProductKey2.ForeColor = (txtProductKey2.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                txtProductKey3.Font = theme.GetFont("normal");
                txtProductKey3.ForeColor = (txtProductKey3.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                txtProductKey4.Font = theme.GetFont("normal");
                txtProductKey4.ForeColor = (txtProductKey4.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                txtProductKey5.Font = theme.GetFont("normal");
                txtProductKey5.ForeColor = (txtProductKey5.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                
                lblHyphen1.Font = theme.GetFont("normal");
                lblHyphen1.ForeColor = theme.GetFontColor("normal");
                lblHyphen2.Font = theme.GetFont("normal");
                lblHyphen2.ForeColor = theme.GetFontColor("normal");
                lblHyphen3.Font = theme.GetFont("normal");
                lblHyphen3.ForeColor = theme.GetFontColor("normal");
                lblHyphen4.Font = theme.GetFont("normal");
                lblHyphen4.ForeColor = theme.GetFontColor("normal");
                
                lblCPUArch.Font = theme.GetFont("normal");
                lblCPUArch.ForeColor = theme.GetFontColor("normal");
                cmbCPUArch.Font = theme.GetFont("normal");
                cmbCPUArch.ForeColor = theme.GetFontColor("inputForeground");
            }
            else
            {
                // Muted state
                lblEnableGeneral.Font = theme.GetFont("muted");
                lblEnableGeneral.ForeColor = theme.GetFontColor("muted");
                
                lblWindowsEdition.Font = theme.GetFont("muted");
                lblWindowsEdition.ForeColor = theme.GetFontColor("muted");
                cmbWindowsEdition.Font = theme.GetFont("muted");
                cmbWindowsEdition.ForeColor = theme.GetFontColor("muted");
                
                lblProductKey.Font = theme.GetFont("muted");
                lblProductKey.ForeColor = theme.GetFontColor("muted");
                
                // For product key textboxes, keep placeholder color if they have placeholder text
                txtProductKey1.Font = theme.GetFont("muted");
                txtProductKey1.ForeColor = (txtProductKey1.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                txtProductKey2.Font = theme.GetFont("muted");
                txtProductKey2.ForeColor = (txtProductKey2.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                txtProductKey3.Font = theme.GetFont("muted");
                txtProductKey3.ForeColor = (txtProductKey3.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                txtProductKey4.Font = theme.GetFont("muted");
                txtProductKey4.ForeColor = (txtProductKey4.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                txtProductKey5.Font = theme.GetFont("muted");
                txtProductKey5.ForeColor = (txtProductKey5.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                
                lblHyphen1.Font = theme.GetFont("muted");
                lblHyphen1.ForeColor = theme.GetFontColor("muted");
                lblHyphen2.Font = theme.GetFont("muted");
                lblHyphen2.ForeColor = theme.GetFontColor("muted");
                lblHyphen3.Font = theme.GetFont("muted");
                lblHyphen3.ForeColor = theme.GetFontColor("muted");
                lblHyphen4.Font = theme.GetFont("muted");
                lblHyphen4.ForeColor = theme.GetFontColor("muted");
                
                lblCPUArch.Font = theme.GetFont("muted");
                lblCPUArch.ForeColor = theme.GetFontColor("muted");
                cmbCPUArch.Font = theme.GetFont("muted");
                cmbCPUArch.ForeColor = theme.GetFontColor("muted");
            }

            // Notify MainForm to update lock/unlock button
            _onEnableToggle?.Invoke();
        }

        /// <summary>
        /// Initializes placeholder text for all Product Key textboxes
        /// </summary>
        private void InitializePlaceholders()
        {
            SetProductKeyPlaceholder(txtProductKey1);
            SetProductKeyPlaceholder(txtProductKey2);
            SetProductKeyPlaceholder(txtProductKey3);
            SetProductKeyPlaceholder(txtProductKey4);
            SetProductKeyPlaceholder(txtProductKey5);
        }

        /// <summary>
        /// Sets placeholder text for a Product Key textbox
        /// </summary>
        private void SetProductKeyPlaceholder(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = LangManager.Instance.GetString("generalConfig.productKey.placeholder");
                textBox.ForeColor = ThemeManager.Instance.GetFontColor("placeholder");
            }
        }

        #endregion

        #region Data Operations

        /// <summary>
        /// Updates the data model from UI controls
        /// </summary>
        public void UpdateFromControls()
        {
            // Skip if we're loading config to prevent overwriting values
            if (_isLoading)
            {
                return;
            }

            // Check if UI is initialized
            if (cmbWindowsEdition == null ||
                txtProductKey1 == null ||
                cmbCPUArch == null)
            {
                Console.WriteLine("UpdateFromControls: UI controls not initialized yet!");
                return;
            }

            // Update WindowsEdition using mapping
            WindowsEdition = GetWindowsEditionValueFromIndex(cmbWindowsEdition.SelectedIndex);

            // Update ProductKey
            string pk1 = txtProductKey1.Text.Trim();
            string pk2 = txtProductKey2.Text.Trim();
            string pk3 = txtProductKey3.Text.Trim();
            string pk4 = txtProductKey4.Text.Trim();
            string pk5 = txtProductKey5.Text.Trim();

            // Check if placeholders
            if (pk1 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk1 = "";
            if (pk2 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk2 = "";
            if (pk3 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk3 = "";
            if (pk4 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk4 = "";
            if (pk5 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk5 = "";

            if (string.IsNullOrEmpty(pk1) && string.IsNullOrEmpty(pk2) && 
                string.IsNullOrEmpty(pk3) && string.IsNullOrEmpty(pk4) && string.IsNullOrEmpty(pk5))
            {
                ProductKey = "";
            }
            else
            {
                ProductKey = $"{pk1}-{pk2}-{pk3}-{pk4}-{pk5}";
            }

            // Update CPUArchitecture using mapping
            CPUArchitecture = GetCPUArchValueFromIndex(cmbCPUArch.SelectedIndex);
        }

        /// <summary>
        /// Clears all UI controls and resets to default state
        /// </summary>
        public void ClearControls()
        {
            _isLoading = true;

            try
            {
                // Check if UI is initialized
                if (cmbWindowsEdition == null ||
                    txtProductKey1 == null ||
                    cmbCPUArch == null)
                    return;

                // Try to load empty state from src/config/empty.json
                string emptyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "config", "empty.json");
                if (File.Exists(emptyPath))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(emptyPath);
                        JObject empty = JObject.Parse(jsonContent);

                        if (empty["general"] is JObject generalSection)
                        {
                            // Use SetValues(dynamic) to populate the data model from JSON
                            SetValues(generalSection);

                            // Update UI from the model
                            UpdateControlsFromModel();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"GeneralConfig.ClearControls: failed to read/parse empty.json: {ex.Message}");
                        // fall through to hardcoded clear as fallback
                    }
                }

                // Fallback: Clear controls manually
                cmbWindowsEdition.SelectedIndex = 0;

                // Clear and restore placeholders for Product Key textboxes
                txtProductKey1.Clear();
                SetProductKeyPlaceholder(txtProductKey1);
                txtProductKey2.Clear();
                SetProductKeyPlaceholder(txtProductKey2);
                txtProductKey3.Clear();
                SetProductKeyPlaceholder(txtProductKey3);
                txtProductKey4.Clear();
                SetProductKeyPlaceholder(txtProductKey4);
                txtProductKey5.Clear();
                SetProductKeyPlaceholder(txtProductKey5);

                cmbCPUArch.SelectedIndex = 0;
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Updates UI controls from the data model
        /// </summary>
        public void UpdateControlsFromModel()
        {
            // Set flag to prevent event handlers from firing during loading
            _isLoading = true;
            
            // Check if UI is initialized
            if (cmbWindowsEdition == null || txtProductKey1 == null || cmbCPUArch == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: WindowsEdition={WindowsEdition}, ProductKey={ProductKey}, CPUArchitecture={CPUArchitecture}");

            try
            {
                LangManager lang = LangManager.Instance;
                ThemeManager theme = ThemeManager.Instance;

                // Update Windows Edition combo box using mapping
                cmbWindowsEdition.SelectedIndex = GetIndexFromWindowsEditionValue(WindowsEdition);

                // Update Product Key textboxes
                if (!string.IsNullOrWhiteSpace(ProductKey))
                {
                    string[] segments = ProductKey.Split('-');
                    if (segments.Length == 5)
                    {
                        SetProductKeySegment(txtProductKey1, segments[0], theme);
                        SetProductKeySegment(txtProductKey2, segments[1], theme);
                        SetProductKeySegment(txtProductKey3, segments[2], theme);
                        SetProductKeySegment(txtProductKey4, segments[3], theme);
                        SetProductKeySegment(txtProductKey5, segments[4], theme);
                    }
                }
                else
                {
                    SetProductKeyPlaceholder(txtProductKey1);
                    SetProductKeyPlaceholder(txtProductKey2);
                    SetProductKeyPlaceholder(txtProductKey3);
                    SetProductKeyPlaceholder(txtProductKey4);
                    SetProductKeyPlaceholder(txtProductKey5);
                }

                // Update CPU Architecture combo box using mapping
                cmbCPUArch.SelectedIndex = GetIndexFromCPUArchValue(CPUArchitecture);

                // Enable/disable controls based on EnableGeneral state
                bool enableControls = EnableGeneral;
                cmbWindowsEdition.Enabled = enableControls;
                txtProductKey1.Enabled = enableControls;
                txtProductKey2.Enabled = enableControls;
                txtProductKey3.Enabled = enableControls;
                txtProductKey4.Enabled = enableControls;
                txtProductKey5.Enabled = enableControls;
                cmbCPUArch.Enabled = enableControls;

                // Update font styles based on enabled state
                if (enableControls)
                {
                    // Normal state
                    lblEnableGeneral.Font = theme.GetFont("normal");
                    lblEnableGeneral.ForeColor = theme.GetFontColor("normal");
                    lblWindowsEdition.Font = theme.GetFont("normal");
                    lblWindowsEdition.ForeColor = theme.GetFontColor("normal");
                    cmbWindowsEdition.Font = theme.GetFont("normal");
                    cmbWindowsEdition.ForeColor = theme.GetFontColor("inputForeground");
                    
                    lblProductKey.Font = theme.GetFont("normal");
                    lblProductKey.ForeColor = theme.GetFontColor("normal");
                    
                    // For product key textboxes, keep placeholder color if they have placeholder text
                    txtProductKey1.Font = theme.GetFont("normal");
                    txtProductKey1.ForeColor = (txtProductKey1.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                    txtProductKey2.Font = theme.GetFont("normal");
                    txtProductKey2.ForeColor = (txtProductKey2.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                    txtProductKey3.Font = theme.GetFont("normal");
                    txtProductKey3.ForeColor = (txtProductKey3.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                    txtProductKey4.Font = theme.GetFont("normal");
                    txtProductKey4.ForeColor = (txtProductKey4.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                    txtProductKey5.Font = theme.GetFont("normal");
                    txtProductKey5.ForeColor = (txtProductKey5.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("inputForeground");
                    
                    lblHyphen1.Font = theme.GetFont("normal");
                    lblHyphen1.ForeColor = theme.GetFontColor("normal");
                    lblHyphen2.Font = theme.GetFont("normal");
                    lblHyphen2.ForeColor = theme.GetFontColor("normal");
                    lblHyphen3.Font = theme.GetFont("normal");
                    lblHyphen3.ForeColor = theme.GetFontColor("normal");
                    lblHyphen4.Font = theme.GetFont("normal");
                    lblHyphen4.ForeColor = theme.GetFontColor("normal");
                    
                    lblCPUArch.Font = theme.GetFont("normal");
                    lblCPUArch.ForeColor = theme.GetFontColor("normal");
                    cmbCPUArch.Font = theme.GetFont("normal");
                    cmbCPUArch.ForeColor = theme.GetFontColor("inputForeground");
                }
                else
                {
                    // Muted state
                    lblEnableGeneral.Font = theme.GetFont("muted");
                    lblEnableGeneral.ForeColor = theme.GetFontColor("muted");
                    lblWindowsEdition.Font = theme.GetFont("muted");
                    lblWindowsEdition.ForeColor = theme.GetFontColor("muted");
                    cmbWindowsEdition.Font = theme.GetFont("muted");
                    cmbWindowsEdition.ForeColor = theme.GetFontColor("muted");
                    
                    lblProductKey.Font = theme.GetFont("muted");
                    lblProductKey.ForeColor = theme.GetFontColor("muted");
                    
                    // For product key textboxes, keep placeholder color if they have placeholder text
                    txtProductKey1.Font = theme.GetFont("muted");
                    txtProductKey1.ForeColor = (txtProductKey1.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                    txtProductKey2.Font = theme.GetFont("muted");
                    txtProductKey2.ForeColor = (txtProductKey2.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                    txtProductKey3.Font = theme.GetFont("muted");
                    txtProductKey3.ForeColor = (txtProductKey3.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                    txtProductKey4.Font = theme.GetFont("muted");
                    txtProductKey4.ForeColor = (txtProductKey4.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                    txtProductKey5.Font = theme.GetFont("muted");
                    txtProductKey5.ForeColor = (txtProductKey5.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) ? theme.GetFontColor("placeholder") : theme.GetFontColor("muted");
                    
                    lblHyphen1.Font = theme.GetFont("muted");
                    lblHyphen1.ForeColor = theme.GetFontColor("muted");
                    lblHyphen2.Font = theme.GetFont("muted");
                    lblHyphen2.ForeColor = theme.GetFontColor("muted");
                    lblHyphen3.Font = theme.GetFont("muted");
                    lblHyphen3.ForeColor = theme.GetFontColor("muted");
                    lblHyphen4.Font = theme.GetFont("muted");
                    lblHyphen4.ForeColor = theme.GetFontColor("muted");
                    
                    lblCPUArch.Font = theme.GetFont("muted");
                    lblCPUArch.ForeColor = theme.GetFontColor("muted");
                    cmbCPUArch.Font = theme.GetFont("muted");
                    cmbCPUArch.ForeColor = theme.GetFontColor("muted");
                }
            }
            finally
            {
                // Always reset the flag, even if an error occurs
                _isLoading = false;
                
                // Trigger validation after loading config
                ValidateWindowsEdition();
                ValidateProductKey();
                ValidateCPUArch();
            }
        }

        /// <summary>
        /// Helper to set a product key segment in a textbox
        /// </summary>
        private void SetProductKeySegment(TextBox textBox, string value, ThemeManager theme)
        {
            if (!string.IsNullOrWhiteSpace(value) && value != LangManager.Instance.GetString("generalConfig.productKey.placeholder"))
            {
                textBox.Text = value;
                textBox.ForeColor = theme.GetFontColor("normal");
            }
            else
            {
                SetProductKeyPlaceholder(textBox);
            }
        }

        /// <summary>
        /// Clears the data model
        /// </summary>
        public void Clear()
        {
            WindowsEdition = "";
            ProductKey = "";
            CPUArchitecture = "";
        }

        /// <summary>
        /// Gets the configuration values as a dictionary
        /// </summary>
        public Dictionary<string, string> GetValues()
        {
            return new Dictionary<string, string>
            {
                ["WindowsEdition"] = WindowsEdition,
                ["ProductKey"] = ProductKey,
                ["CPUArchitecture"] = CPUArchitecture
            };
        }

        /// <summary>
        /// Sets configuration values from JSON
        /// </summary>
        public void SetValues(dynamic json)
        {
            if (json.windowsEdition != null)
                WindowsEdition = json.windowsEdition;
            if (json.productKey != null)
                ProductKey = json.productKey;
            if (json.cpuArchitecture != null)
                CPUArchitecture = json.cpuArchitecture;
        }

        #region UI Validation Methods
        
        /// <summary>
        /// Validates all UI fields and updates their status rings
        /// Call this after theme/language changes to refresh validation state
        /// </summary>
        public void ValidateAllUIFields()
        {
            ValidateWindowsEdition();
            ValidateProductKey();
            ValidateCPUArch();
            
            // Refresh placeholder colors with current theme
            RefreshPlaceholders();
        }
        
        /// <summary>
        /// Refreshes placeholder text colors to match current theme
        /// </summary>
        private void RefreshPlaceholders()
        {
            SetProductKeyPlaceholder(txtProductKey1);
            SetProductKeyPlaceholder(txtProductKey2);
            SetProductKeyPlaceholder(txtProductKey3);
            SetProductKeyPlaceholder(txtProductKey4);
            SetProductKeyPlaceholder(txtProductKey5);
        }

        /// <summary>
        /// Validates Windows Edition field and updates its status ring
        /// </summary>
        private void ValidateWindowsEdition()
        {
            if (ringWindowsEdition == null || cmbWindowsEdition == null)
                return;

            if (cmbWindowsEdition.SelectedIndex > 0 && !string.IsNullOrWhiteSpace(cmbWindowsEdition.SelectedItem?.ToString()))
            {
                ringWindowsEdition.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringWindowsEdition.SetStatus(ValidationStatus.Invalid);
            }
        }

        /// <summary>
        /// Validates Product Key field and updates its status ring
        /// </summary>
        private void ValidateProductKey()
        {
            if (ringProductKey == null)
                return;

            string pk1 = txtProductKey1?.Text?.Trim() ?? "";
            string pk2 = txtProductKey2?.Text?.Trim() ?? "";
            string pk3 = txtProductKey3?.Text?.Trim() ?? "";
            string pk4 = txtProductKey4?.Text?.Trim() ?? "";
            string pk5 = txtProductKey5?.Text?.Trim() ?? "";

            // Remove placeholders
            if (pk1 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk1 = "";
            if (pk2 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk2 = "";
            if (pk3 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk3 = "";
            if (pk4 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk4 = "";
            if (pk5 == LangManager.Instance.GetString("generalConfig.productKey.placeholder")) pk5 = "";

            // If all empty, it's valid (optional field)
            if (string.IsNullOrEmpty(pk1) && string.IsNullOrEmpty(pk2) &&
                string.IsNullOrEmpty(pk3) && string.IsNullOrEmpty(pk4) && string.IsNullOrEmpty(pk5))
            {
                ringProductKey.SetStatus(ValidationStatus.Valid);
                return;
            }

            // Check if any segment contains non-alphanumeric characters
            if (ContainsNonAlphanumeric(pk1) || ContainsNonAlphanumeric(pk2) || 
                ContainsNonAlphanumeric(pk3) || ContainsNonAlphanumeric(pk4) || ContainsNonAlphanumeric(pk5))
            {
                ringProductKey.SetStatus(ValidationStatus.Invalid);
                return;
            }

            // All filled - validate format
            string fullKey = $"{pk1}-{pk2}-{pk3}-{pk4}-{pk5}";
            if (IsValidProductKey(fullKey))
            {
                ringProductKey.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringProductKey.SetStatus(ValidationStatus.Invalid);
            }
        }

        /// <summary>
        /// Checks if a string contains any non-alphanumeric characters
        /// </summary>
        private bool ContainsNonAlphanumeric(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            foreach (char c in text)
            {
                if (!char.IsLetterOrDigit(c))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Validates CPU Architecture field and updates its status ring
        /// </summary>
        private void ValidateCPUArch()
        {
            if (ringCPUArch == null || cmbCPUArch == null)
                return;

            if (cmbCPUArch.SelectedIndex > 0 && !string.IsNullOrWhiteSpace(cmbCPUArch.SelectedItem?.ToString()))
            {
                ringCPUArch.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringCPUArch.SetStatus(ValidationStatus.Invalid);
            }
        }

        #endregion

        #endregion

        #region Value Mapping Helpers

        /// <summary>
        /// Maps dropdown index to Windows Edition value
        /// </summary>
        private string GetWindowsEditionValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "Windows 11 Home";
                case 2: return "Windows 11 Pro";
                case 3: return "Windows 11 Education";
                case 4: return "Windows 11 Enterprise";
                default: return ""; // Index 0 or invalid
            }
        }

        /// <summary>
        /// Maps Windows Edition value to dropdown index
        /// </summary>
        private int GetIndexFromWindowsEditionValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value)
            {
                case "Windows 11 Home": return 1;
                case "Windows 11 Pro": return 2;
                case "Windows 11 Education": return 3;
                case "Windows 11 Enterprise": return 4;
                default: return 0; // Unknown value
            }
        }

        /// <summary>
        /// Maps dropdown index to CPU Architecture value (amd64, arm64)
        /// </summary>
        private string GetCPUArchValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "amd64"; // x64
                case 2: return "arm64"; // ARM64
                default: return ""; // Index 0 or invalid
            }
        }

        /// <summary>
        /// Maps CPU Architecture value to dropdown index
        /// </summary>
        private int GetIndexFromCPUArchValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value)
            {
                case "amd64": return 1;
                case "arm64": return 2;
                default: return 0; // Unknown value
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates the configuration and returns a list of errors
        /// </summary>
        public List<string> Validate()
        {
            List<string> errors = new List<string>();
            LangManager lang = LangManager.Instance;

            // Validate Windows Edition
            if (string.IsNullOrWhiteSpace(WindowsEdition))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("generalConfig.windowsEdition.label")));
            }

            // Validate Product Key
            if (!string.IsNullOrWhiteSpace(ProductKey))
            {
                if (!IsValidProductKey(ProductKey))
                {
                    errors.Add(lang.GetString("validation.invalidProductKey"));
                }
            }

            // Validate CPU Architecture
            if (string.IsNullOrWhiteSpace(CPUArchitecture))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("generalConfig.cpuArch.label")));
            }

            return errors;
        }

        /// <summary>
        /// Validates Product Key format
        /// </summary>
        private bool IsValidProductKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return true; // Empty is valid

            // Remove hyphens for validation
            string cleanKey = key.Replace("-", "");

            // Must be exactly 25 alphanumeric characters
            if (cleanKey.Length != 25)
                return false;

            // Must contain only alphanumeric characters
            return Regex.IsMatch(cleanKey, @"^[A-Z0-9]{25}$");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a Product Key textbox with proper formatting and navigation behavior
        /// </summary>
        public static TextBox CreateProductKeyTextBox(int width, int height, Control parentForm)
        {
            ThemeManager theme = ThemeManager.Instance;
            UIValues ui = UIValues.Instance;
            
            TextBox txt = new TextBox();
            txt.Size = new Size(width, height);
            txt.MaxLength = ui.GetValue("global.productKey.segmentLength", 5);
            txt.CharacterCasing = CharacterCasing.Upper;
            txt.TextAlign = HorizontalAlignment.Center;
            txt.BorderStyle = BorderStyle.Fixed3D;
            txt.Font = theme.GetFont("normal");
            txt.Multiline = false;
            
            // Handle placeholder
            txt.ForeColor = theme.GetFontColor("placeholder");
            txt.Text = LangManager.Instance.GetString("generalConfig.productKey.placeholder");
            
            txt.Enter += (sender, e) =>
            {
                if (txt.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder") && txt.ForeColor == theme.GetFontColor("placeholder"))
                {
                    txt.Text = "";
                    txt.ForeColor = theme.GetFontColor("normal");
                }
            };
            
            txt.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.ForeColor = theme.GetFontColor("placeholder");
                    txt.Text = LangManager.Instance.GetString("generalConfig.productKey.placeholder");
                }
            };
            
            // Only allow alphanumeric characters (A-Z, 0-9)
            txt.KeyPress += (sender, e) =>
            {
                // Allow control characters (Backspace, etc.)
                if (char.IsControl(e.KeyChar))
                {
                    return;
                }
                
                // Only allow letters and digits
                if (!char.IsLetterOrDigit(e.KeyChar))
                {
                    e.Handled = true; // Block the character
                }
            };
            
            // Navigate to previous textbox on Backspace when empty
            txt.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Back)
                {
                    // If textbox is empty or only has placeholder, move to previous
                    if (string.IsNullOrWhiteSpace(txt.Text) || 
                        (txt.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder") && txt.ForeColor == theme.GetFontColor("placeholder")))
                    {
                        parentForm.SelectNextControl(txt, false, true, true, true);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }
            };
            
            // Auto-move to next textbox
            txt.TextChanged += (sender, e) =>
            {
                // Don't trigger for placeholder text
                if (txt.Text == LangManager.Instance.GetString("generalConfig.productKey.placeholder") && txt.ForeColor == theme.GetFontColor("placeholder"))
                {
                    return;
                }
                
                // Auto-focus to next textbox when max length reached
                if (!string.IsNullOrWhiteSpace(txt.Text) && txt.Text != LangManager.Instance.GetString("generalConfig.productKey.placeholder"))
                {
                    if (txt.Text.Length >= txt.MaxLength)
                    {
                        parentForm.SelectNextControl(txt, true, true, true, true);
                    }
                }
            };
            
            return txt;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Sets the configuration values from a dictionary
        /// </summary>
        public void SetValues(Dictionary<string, string> values)
        {
            if (values.ContainsKey("WindowsEdition"))
                WindowsEdition = values["WindowsEdition"];

            if (values.ContainsKey("ProductKey"))
                ProductKey = values["ProductKey"];

            if (values.ContainsKey("CPUArchitecture"))
                CPUArchitecture = values["CPUArchitecture"];
        }

        #endregion
    }
}
