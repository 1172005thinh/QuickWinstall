using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Language & Region Configuration section including UI, data model, and business logic
    /// </summary>
    public class LangRegConfig
    {
        #region Data Model

        public bool EnableLangReg { get; set; } = true;
        public string SystemLocale { get; set; } = "";
        public string UserLocale { get; set; } = "";
        public string WindowsUILanguage { get; set; } = "";
        public string KeyboardLayout { get; set; } = "";
        public string TimeZone { get; set; } = "";
        public bool SameAsSystemLocale { get; set; } = true;

        #endregion

        #region UI Components

        private Panel pnlLangRegConfig = null!;
        private Button btnLangRegConfigToggle = null!;
        private Label lblLangRegConfigTitle = null!;
        private Panel pnlLangRegConfigSeparator = null!;
        private Panel pnlLangRegConfigContent = null!;

        // Enable Language & Region Config
        private Label lblEnableLangReg = null!;
        private Panel toggleEnableLangReg = null!;
        
        // System Locale
        private Label lblSystemLocale = null!;
        private ComboBox cmbSystemLocale = null!;
        private StatusRing ringSystemLocale = null!; // Combined ring for both System and User Locale
        
        // User Locale
        private Label lblUserLocale = null!;
        private ComboBox cmbUserLocale = null!;
        
        // Same as System Locale checkbox
        private Panel toggleSameAsSystemLocale = null!;
        private Label lblSameAsSystemLocale = null!;
        
        // Windows UI Language
        private Label lblWindowsUILanguage = null!;
        private ComboBox cmbWindowsUILanguage = null!;
        private StatusRing ringWindowsUILanguage = null!;
        
        // Keyboard Layout
        private Label lblKeyboardLayout = null!;
        private ComboBox cmbKeyboardLayout = null!;
        private StatusRing ringKeyboardLayout = null!;
        
        // Time Zone
        private Label lblTimeZone = null!;
        private ComboBox cmbTimeZone = null!;
        private StatusRing ringTimeZone = null!;

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
        /// Initializes the Language & Region Config section UI and returns the main panel
        /// </summary>
        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged,
            Func<Button> createRoundedButton, Action? onSectionToggle, Action? onEnableToggle)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            // Store callbacks
            _onSectionToggle = onSectionToggle;
            _onEnableToggle = onEnableToggle;
            _onConfigChanged = onConfigChanged;

            int contentHeight = ui.GetSectionValue("langRegConfig", "contentHeight", 390);

            // Language & Region Config Panel
            pnlLangRegConfig = new Panel();
            pnlLangRegConfig.Location = new Point(0, 0);
            pnlLangRegConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlLangRegConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Language & Region Config Toggle Button
            btnLangRegConfigToggle = createRoundedButton();
            btnLangRegConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnLangRegConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnLangRegConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnLangRegConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnLangRegConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnLangRegConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.langReg"));

            // Language & Region Config Title
            lblLangRegConfigTitle = new Label();
            lblLangRegConfigTitle.Location = new Point(btnLangRegConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblLangRegConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblLangRegConfigTitle.Text = lang.GetString("mainForm.sections.langReg");
            lblLangRegConfigTitle.Font = theme.GetFont("subheader");
            lblLangRegConfigTitle.UseMnemonic = false;
            lblLangRegConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblLangRegConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblLangRegConfigTitle.Cursor = Cursors.Hand;
            lblLangRegConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblLangRegConfigTitle, "tooltips.langRegConfig.header", lang.GetString("tooltips.langRegConfig.header"));
            
            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);

            // Enable Language & Region Config
            // Only set EnableLangReg from settings on first initialization
            if (_isFirstInitialization)
            {
                EnableLangReg = !SettingsManager.Instance.LockSectionsAtStartup;
                _isFirstInitialization = false;
            }
            toggleEnableLangReg = theme.CreateToggleSwitch(new Point(ui.GlobalTabX * 2 + ui.GlobalBtnBox, btnLangRegConfigToggle.Bottom + ui.GlobalSpacingY), toggleWidth, ui.GlobalInputHeight, EnableLangReg);
            toggleEnableLangReg.TabStop = false; // Skip this control in tab order
            toggleEnableLangReg.Click += (s, e) => OnToggleEnableLangReg();

            lblEnableLangReg = new Label();
            lblEnableLangReg.Location = new Point(toggleEnableLangReg.Right + ui.GlobalSpacingX, btnLangRegConfigToggle.Bottom + ui.GlobalSpacingY);
            lblEnableLangReg.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblEnableLangReg.Text = lang.GetString("langRegConfig.enableLangReg.label");
            lblEnableLangReg.Font = theme.GetFont("normal");
            lblEnableLangReg.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableLangReg.Cursor = Cursors.Hand;
            lblEnableLangReg.Click += (s, e) => OnToggleEnableLangReg();
            tooltips.SetToolTip(lblEnableLangReg, "tooltips.langRegConfig.enableLangReg");

            // Line Separator
            pnlLangRegConfigSeparator = new Panel();
            pnlLangRegConfigSeparator.Location = new Point(ui.GlobalTabX, toggleEnableLangReg.Bottom + ui.GlobalSpacingY * 2);
            pnlLangRegConfigSeparator.Size = new Size(pnlLangRegConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlLangRegConfigSeparator.BackColor = theme.GetColor("separator");

            // Language & Region Config Content Panel
            pnlLangRegConfigContent = new Panel();
            pnlLangRegConfigContent.Location = new Point(0, pnlLangRegConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlLangRegConfigContent.Size = new Size(pnlLangRegConfig.Width, contentHeight);
            pnlLangRegConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlLangRegConfigContent.AutoScroll = false;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int currentY = ui.GlobalSpacingY;

            // System Locale
            lblSystemLocale = new Label();
            lblSystemLocale.Location = new Point(labelX, currentY);
            lblSystemLocale.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblSystemLocale.Text = lang.GetString("langRegConfig.systemLocale.label");
            lblSystemLocale.Font = theme.GetFont("normal");
            lblSystemLocale.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblSystemLocale, "tooltips.langRegConfig.systemLocale");

            cmbSystemLocale = new ComboBox();
            cmbSystemLocale.Location = new Point(inputX, currentY);
            cmbSystemLocale.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbSystemLocale.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSystemLocale.BackColor = theme.GetColor("inputBackground");
            cmbSystemLocale.ForeColor = theme.GetFontColor("inputForeground");
            cmbSystemLocale.Items.AddRange(new object[] {
                lang.GetString("langRegConfig.systemLocale.options.selectOne"),
                lang.GetString("langRegConfig.systemLocale.options.en-US"),
                lang.GetString("langRegConfig.systemLocale.options.vi-VN"),
                lang.GetString("langRegConfig.systemLocale.options.zh-CN"),
                lang.GetString("langRegConfig.systemLocale.options.zh-TW"),
                lang.GetString("langRegConfig.systemLocale.options.ja-JP"),
                lang.GetString("langRegConfig.systemLocale.options.ko-KR"),
                lang.GetString("langRegConfig.systemLocale.options.de-DE"),
                lang.GetString("langRegConfig.systemLocale.options.fr-FR"),
                lang.GetString("langRegConfig.systemLocale.options.es-ES"),
                lang.GetString("langRegConfig.systemLocale.options.pt-BR")
            });

            // Status ring covering both System Locale and User Locale
            int statusRingBorderExtra = 6;
            // Calculate height to cover both dropdowns:
            // Height of first dropdown + spacing between them + height of second dropdown + 2 * border width
            int spacingBetweenControls = ui.GlobalSpacingY * 2;
            int combinedHeight = cmbSystemLocale.Height + spacingBetweenControls + cmbSystemLocale.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + 2 * statusRingBorderExtra + 1;
            
            ringSystemLocale = new StatusRing();
            ringSystemLocale.Location = new Point(cmbSystemLocale.Left - ui.GetValue("global.statusRing.borderWidth"), cmbSystemLocale.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringSystemLocale.Size = new Size(cmbSystemLocale.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), combinedHeight);
            ringSystemLocale.Visible = false;

            cmbSystemLocale.SelectedIndex = 0;
            cmbSystemLocale.SelectedIndexChanged += onConfigChanged;
            cmbSystemLocale.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    SystemLocale = GetLocaleValueFromIndex(cmbSystemLocale.SelectedIndex);
                    
                    // If toggle is ON, sync UserLocale
                    ThemeManager syncTheme = ThemeManager.Instance;
                    if (syncTheme.GetToggleSwitchState(toggleSameAsSystemLocale) && cmbSystemLocale.SelectedIndex > 0)
                    {
                        cmbUserLocale.SelectedIndex = cmbSystemLocale.SelectedIndex;
                    }
                }
                ValidateSystemLocale();
            };
            tooltips.SetToolTip(cmbSystemLocale, "tooltips.langRegConfig.systemLocale");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // User Locale
            lblUserLocale = new Label();
            lblUserLocale.Location = new Point(labelX, currentY);
            lblUserLocale.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblUserLocale.Text = lang.GetString("langRegConfig.userLocale.label");
            lblUserLocale.Font = theme.GetFont("normal");
            lblUserLocale.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblUserLocale, "tooltips.langRegConfig.userLocale");

            cmbUserLocale = new ComboBox();
            cmbUserLocale.Location = new Point(inputX, currentY);
            cmbUserLocale.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbUserLocale.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUserLocale.BackColor = theme.GetColor("inputBackground");
            cmbUserLocale.ForeColor = theme.GetFontColor("inputForeground");
            cmbUserLocale.Items.AddRange(new object[] {
                lang.GetString("langRegConfig.userLocale.options.selectOne"),
                lang.GetString("langRegConfig.userLocale.options.en-US"),
                lang.GetString("langRegConfig.userLocale.options.vi-VN"),
                lang.GetString("langRegConfig.userLocale.options.zh-CN"),
                lang.GetString("langRegConfig.userLocale.options.zh-TW"),
                lang.GetString("langRegConfig.userLocale.options.ja-JP"),
                lang.GetString("langRegConfig.userLocale.options.ko-KR"),
                lang.GetString("langRegConfig.userLocale.options.de-DE"),
                lang.GetString("langRegConfig.userLocale.options.fr-FR"),
                lang.GetString("langRegConfig.userLocale.options.es-ES"),
                lang.GetString("langRegConfig.userLocale.options.pt-BR")
            });

            cmbUserLocale.SelectedIndex = 0;
            cmbUserLocale.SelectedIndexChanged += onConfigChanged;
            cmbUserLocale.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    UserLocale = GetLocaleValueFromIndex(cmbUserLocale.SelectedIndex);
                }
                ValidateUserLocale();
            };
            tooltips.SetToolTip(cmbUserLocale, "tooltips.langRegConfig.userLocale");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Same as System Locale toggle switch
            toggleSameAsSystemLocale = theme.CreateToggleSwitch(new Point(inputX, currentY), toggleWidth, ui.GlobalInputHeight, true);
            toggleSameAsSystemLocale.Click += (s, e) => OnToggleSameAsSystemLocale();

            lblSameAsSystemLocale = new Label();
            lblSameAsSystemLocale.Location = new Point(toggleSameAsSystemLocale.Right + ui.GlobalSpacingX, currentY);
            lblSameAsSystemLocale.Size = new Size(ui.GlobalInputWidth - toggleWidth - ui.GlobalSpacingX, ui.GlobalLabelHeight);
            lblSameAsSystemLocale.Text = lang.GetString("langRegConfig.sameAsSystemLocale.label");
            // Initial color will be set to normal since toggle starts as true, then UpdateControlsFromModel will correct it
            lblSameAsSystemLocale.Font = theme.GetFont("normal");
            lblSameAsSystemLocale.ForeColor = theme.GetFontColor("normal");
            lblSameAsSystemLocale.TextAlign = ContentAlignment.MiddleLeft;
            lblSameAsSystemLocale.Cursor = Cursors.Hand;
            lblSameAsSystemLocale.Click += (s, e) => OnToggleSameAsSystemLocale();
            tooltips.SetToolTip(lblSameAsSystemLocale, "tooltips.langRegConfig.sameAsSystemLocale");
            pnlLangRegConfigContent.Controls.Add(lblSameAsSystemLocale);

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;

            // Windows UI Language
            lblWindowsUILanguage = new Label();
            lblWindowsUILanguage.Location = new Point(labelX, currentY);
            lblWindowsUILanguage.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblWindowsUILanguage.Text = lang.GetString("langRegConfig.windowsUILanguage.label");
            lblWindowsUILanguage.Font = theme.GetFont("normal");
            lblWindowsUILanguage.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblWindowsUILanguage, "tooltips.langRegConfig.windowsUILanguage");

            cmbWindowsUILanguage = new ComboBox();
            cmbWindowsUILanguage.Location = new Point(inputX, currentY);
            cmbWindowsUILanguage.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbWindowsUILanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWindowsUILanguage.BackColor = theme.GetColor("inputBackground");
            cmbWindowsUILanguage.ForeColor = theme.GetFontColor("inputForeground");
            cmbWindowsUILanguage.Items.AddRange(new object[] {
                lang.GetString("langRegConfig.windowsUILanguage.options.selectOne"),
                lang.GetString("langRegConfig.windowsUILanguage.options.en-US"),
                lang.GetString("langRegConfig.windowsUILanguage.options.vi-VN"),
                lang.GetString("langRegConfig.windowsUILanguage.options.zh-CN"),
                lang.GetString("langRegConfig.windowsUILanguage.options.zh-TW"),
                lang.GetString("langRegConfig.windowsUILanguage.options.ja-JP"),
                lang.GetString("langRegConfig.windowsUILanguage.options.ko-KR"),
                lang.GetString("langRegConfig.windowsUILanguage.options.de-DE"),
                lang.GetString("langRegConfig.windowsUILanguage.options.fr-FR"),
                lang.GetString("langRegConfig.windowsUILanguage.options.es-ES"),
                lang.GetString("langRegConfig.windowsUILanguage.options.pt-BR")
            });

            // Status ring for Windows UI Language
            ringWindowsUILanguage = new StatusRing();
            ringWindowsUILanguage.Location = new Point(cmbWindowsUILanguage.Left - ui.GetValue("global.statusRing.borderWidth"), cmbWindowsUILanguage.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringWindowsUILanguage.Size = new Size(cmbWindowsUILanguage.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbWindowsUILanguage.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringWindowsUILanguage.Visible = false;

            cmbWindowsUILanguage.SelectedIndex = 0;
            cmbWindowsUILanguage.SelectedIndexChanged += onConfigChanged;
            cmbWindowsUILanguage.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    WindowsUILanguage = GetLocaleValueFromIndex(cmbWindowsUILanguage.SelectedIndex);
                }
                ValidateWindowsUILanguage();
            };
            tooltips.SetToolTip(cmbWindowsUILanguage, "tooltips.langRegConfig.windowsUILanguage");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Keyboard Layout
            lblKeyboardLayout = new Label();
            lblKeyboardLayout.Location = new Point(labelX, currentY);
            lblKeyboardLayout.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblKeyboardLayout.Text = lang.GetString("langRegConfig.keyboardLayout.label");
            lblKeyboardLayout.Font = theme.GetFont("normal");
            lblKeyboardLayout.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblKeyboardLayout, "tooltips.langRegConfig.keyboardLayout");

            cmbKeyboardLayout = new ComboBox();
            cmbKeyboardLayout.Location = new Point(inputX, currentY);
            cmbKeyboardLayout.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbKeyboardLayout.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKeyboardLayout.BackColor = theme.GetColor("inputBackground");
            cmbKeyboardLayout.ForeColor = theme.GetFontColor("inputForeground");
            cmbKeyboardLayout.Items.AddRange(new object[] {
                lang.GetString("langRegConfig.keyboardLayout.options.selectOne"),
                lang.GetString("langRegConfig.keyboardLayout.options.en-US"),
                lang.GetString("langRegConfig.keyboardLayout.options.vi-VN"),
                lang.GetString("langRegConfig.keyboardLayout.options.zh-CN"),
                lang.GetString("langRegConfig.keyboardLayout.options.zh-TW"),
                lang.GetString("langRegConfig.keyboardLayout.options.ja-JP"),
                lang.GetString("langRegConfig.keyboardLayout.options.ko-KR"),
                lang.GetString("langRegConfig.keyboardLayout.options.de-DE"),
                lang.GetString("langRegConfig.keyboardLayout.options.fr-FR"),
                lang.GetString("langRegConfig.keyboardLayout.options.es-ES"),
                lang.GetString("langRegConfig.keyboardLayout.options.pt-BR")
            });

            // Status ring for Keyboard Layout
            ringKeyboardLayout = new StatusRing();
            ringKeyboardLayout.Location = new Point(cmbKeyboardLayout.Left - ui.GetValue("global.statusRing.borderWidth"), cmbKeyboardLayout.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringKeyboardLayout.Size = new Size(cmbKeyboardLayout.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbKeyboardLayout.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringKeyboardLayout.Visible = false;

            cmbKeyboardLayout.SelectedIndex = 0;
            cmbKeyboardLayout.SelectedIndexChanged += onConfigChanged;
            cmbKeyboardLayout.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    KeyboardLayout = GetKeyboardValueFromIndex(cmbKeyboardLayout.SelectedIndex);
                }
                ValidateKeyboardLayout();
            };
            tooltips.SetToolTip(cmbKeyboardLayout, "tooltips.langRegConfig.keyboardLayout");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Time Zone
            lblTimeZone = new Label();
            lblTimeZone.Location = new Point(labelX, currentY);
            lblTimeZone.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblTimeZone.Text = lang.GetString("langRegConfig.timeZone.label");
            lblTimeZone.Font = theme.GetFont("normal");
            lblTimeZone.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblTimeZone, "tooltips.langRegConfig.timeZone");

            cmbTimeZone = new ComboBox();
            cmbTimeZone.Location = new Point(inputX, currentY);
            cmbTimeZone.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbTimeZone.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimeZone.BackColor = theme.GetColor("inputBackground");
            cmbTimeZone.ForeColor = theme.GetFontColor("inputForeground");
            cmbTimeZone.Items.AddRange(new object[] {
                lang.GetString("langRegConfig.timeZone.options.selectOne"),
                lang.GetString("langRegConfig.timeZone.options.UTC-08:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC-07:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC-06:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC-05:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC"),
                lang.GetString("langRegConfig.timeZone.options.UTC+00:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC+01:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC+07:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC+08:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC+09:00"),
                lang.GetString("langRegConfig.timeZone.options.UTC+10:00")
            });

            // Status ring for Time Zone
            ringTimeZone = new StatusRing();
            ringTimeZone.Location = new Point(cmbTimeZone.Left - ui.GetValue("global.statusRing.borderWidth"), cmbTimeZone.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringTimeZone.Size = new Size(cmbTimeZone.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbTimeZone.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringTimeZone.Visible = false;

            cmbTimeZone.SelectedIndex = 0;
            cmbTimeZone.SelectedIndexChanged += onConfigChanged;
            cmbTimeZone.SelectedIndexChanged += (s, e) => 
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    TimeZone = GetTimeZoneValueFromIndex(cmbTimeZone.SelectedIndex);
                }
                ValidateTimeZone();
            };
            tooltips.SetToolTip(cmbTimeZone, "tooltips.langRegConfig.timeZone");

            // Add controls to Language & Region Config Content Panel
            pnlLangRegConfigContent.Controls.Add(lblSystemLocale);
            pnlLangRegConfigContent.Controls.Add(cmbSystemLocale);
            pnlLangRegConfigContent.Controls.Add(lblUserLocale);
            pnlLangRegConfigContent.Controls.Add(cmbUserLocale);
            pnlLangRegConfigContent.Controls.Add(toggleSameAsSystemLocale);
            pnlLangRegConfigContent.Controls.Add(lblWindowsUILanguage);
            pnlLangRegConfigContent.Controls.Add(lblWindowsUILanguage);
            pnlLangRegConfigContent.Controls.Add(cmbWindowsUILanguage);
            pnlLangRegConfigContent.Controls.Add(lblKeyboardLayout);
            pnlLangRegConfigContent.Controls.Add(cmbKeyboardLayout);
            pnlLangRegConfigContent.Controls.Add(lblTimeZone);
            pnlLangRegConfigContent.Controls.Add(cmbTimeZone);

            // Add status rings
            pnlLangRegConfigContent.Controls.Add(ringSystemLocale);
            pnlLangRegConfigContent.Controls.Add(ringWindowsUILanguage);
            pnlLangRegConfigContent.Controls.Add(ringKeyboardLayout);
            pnlLangRegConfigContent.Controls.Add(ringTimeZone);

            // Add controls to Language & Region Config Panel
            pnlLangRegConfig.Controls.Add(btnLangRegConfigToggle);
            pnlLangRegConfig.Controls.Add(lblLangRegConfigTitle);
            pnlLangRegConfig.Controls.Add(lblEnableLangReg);
            pnlLangRegConfig.Controls.Add(toggleEnableLangReg);
            pnlLangRegConfig.Controls.Add(pnlLangRegConfigSeparator);
            pnlLangRegConfig.Controls.Add(pnlLangRegConfigContent);

            // Apply current expansion state
            pnlLangRegConfigContent.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlLangRegConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            // Apply initial "Same as System Locale" state
            bool toggleState = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
            cmbUserLocale.Enabled = !toggleState;

            return pnlLangRegConfig;
        }

        public void LoadConfigIntoUI()
        {
            try
            {
                Console.WriteLine("LangRegConfig: Loading config into UI...");
                UpdateControlsFromModel();
                Console.WriteLine("LangRegConfig: Config loaded into UI successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LangRegConfig: Error loading config into UI: {ex.Message}");
            }
        }

        #endregion

        #region UI Interactions
        
        /// <summary>
        /// Toggles the expansion/collapse state of the section
        /// </summary>
        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlLangRegConfigContent.Visible = _isExpanded;
            pnlLangRegConfigSeparator.Visible = _isExpanded;

            // Update panel height based on state
            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("langRegConfig", "contentHeight", 390);
                pnlLangRegConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnLangRegConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.langReg"));
            }
            else
            {
                pnlLangRegConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnLangRegConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.langReg"));
            }

            // Update button icon
            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnLangRegConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnLangRegConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
            theme.UpdateToggleSwitchState(toggleEnableLangReg, enabled);
            EnableLangReg = enabled;

            // Update dependent controls muted state
            UpdateControlsFromModel();
        }

        /// <summary>
        /// Handles the toggle switch for "Same as System Locale"
        /// </summary>
        private void OnToggleSameAsSystemLocale()
        {
            // Don't allow toggling if section is disabled
            if (!EnableLangReg) return;
            
            ThemeManager theme = ThemeManager.Instance;

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleSameAsSystemLocale, newState);

            // Update UserLocale dropdown state
            cmbUserLocale.Enabled = !newState;

            // Update font styles based on toggle state
            if (newState)
            {
                // When toggle is ON
                lblUserLocale.Font = theme.GetFont("muted");
                lblUserLocale.ForeColor = theme.GetFontColor("muted");
                cmbUserLocale.Font = theme.GetFont("muted");
                cmbUserLocale.ForeColor = theme.GetFontColor("muted");
                lblSameAsSystemLocale.Font = theme.GetFont("normal");
                lblSameAsSystemLocale.ForeColor = theme.GetFontColor("normal");
            }
            else
            {
                // When toggle is OFF
                lblUserLocale.Font = theme.GetFont("normal");
                lblUserLocale.ForeColor = theme.GetFontColor("normal");
                cmbUserLocale.Font = theme.GetFont("normal");
                cmbUserLocale.ForeColor = theme.GetFontColor("inputForeground");
                lblSameAsSystemLocale.Font = theme.GetFont("muted");
                lblSameAsSystemLocale.ForeColor = theme.GetFontColor("muted");
            }

            // If turning ON (newState = true), sync User Locale to System Locale
            if (newState && cmbSystemLocale.SelectedIndex > 0)
            {
                cmbUserLocale.SelectedIndex = cmbSystemLocale.SelectedIndex;
            }

            // Revalidate both locales
            ValidateSystemLocale();
            ValidateUserLocale();
        }
        
        /// <summary>
        /// Handles the toggle for Enable General Configurations
        /// </summary>
        private void OnToggleEnableLangReg()
        {
            if (_isLoading) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle state
            bool currentState = theme.GetToggleSwitchState(toggleEnableLangReg);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableLangReg, newState);

            EnableLangReg = newState;

            // Enable or disable all dependent controls
            bool enableControls = newState;
            
            cmbSystemLocale.Enabled = enableControls;
            cmbUserLocale.Enabled = enableControls && !theme.GetToggleSwitchState(toggleSameAsSystemLocale);
            toggleSameAsSystemLocale.Enabled = enableControls;
            cmbWindowsUILanguage.Enabled = enableControls;
            cmbKeyboardLayout.Enabled = enableControls;
            cmbTimeZone.Enabled = enableControls;

            // Update muted state for toggles
            bool isMuted = !newState;
            theme.UpdateToggleSwitchMutedState(toggleSameAsSystemLocale, isMuted);

            // Update font styles based on enabled state
            if (enableControls)
            {
                // Normal state
                lblEnableLangReg.Font = theme.GetFont("normal");
                lblEnableLangReg.ForeColor = theme.GetFontColor("normal");
                
                lblSystemLocale.Font = theme.GetFont("normal");
                lblSystemLocale.ForeColor = theme.GetFontColor("normal");
                cmbSystemLocale.Font = theme.GetFont("normal");
                cmbSystemLocale.ForeColor = theme.GetFontColor("inputForeground");
                
                // User Locale depends on toggleSameAsSystemLocale state
                bool sameAsSystem = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
                lblUserLocale.Font = theme.GetFont(sameAsSystem ? "muted" : "normal");
                lblUserLocale.ForeColor = theme.GetFontColor(sameAsSystem ? "muted" : "normal");
                cmbUserLocale.Font = theme.GetFont(sameAsSystem ? "muted" : "normal");
                cmbUserLocale.ForeColor = theme.GetFontColor(sameAsSystem ? "muted" : "inputForeground");
                
                lblSameAsSystemLocale.Font = theme.GetFont(sameAsSystem ? "normal" : "muted");
                lblSameAsSystemLocale.ForeColor = theme.GetFontColor(sameAsSystem ? "normal" : "muted");
                lblSameAsSystemLocale.Cursor = Cursors.Hand;
                
                lblWindowsUILanguage.Font = theme.GetFont("normal");
                lblWindowsUILanguage.ForeColor = theme.GetFontColor("normal");
                cmbWindowsUILanguage.Font = theme.GetFont("normal");
                cmbWindowsUILanguage.ForeColor = theme.GetFontColor("inputForeground");
                
                lblKeyboardLayout.Font = theme.GetFont("normal");
                lblKeyboardLayout.ForeColor = theme.GetFontColor("normal");
                cmbKeyboardLayout.Font = theme.GetFont("normal");
                cmbKeyboardLayout.ForeColor = theme.GetFontColor("inputForeground");
                
                lblTimeZone.Font = theme.GetFont("normal");
                lblTimeZone.ForeColor = theme.GetFontColor("normal");
                cmbTimeZone.Font = theme.GetFont("normal");
                cmbTimeZone.ForeColor = theme.GetFontColor("inputForeground");
            }
            else
            {
                // Muted state
                lblEnableLangReg.Font = theme.GetFont("muted");
                lblEnableLangReg.ForeColor = theme.GetFontColor("muted");
                
                lblSystemLocale.Font = theme.GetFont("muted");
                lblSystemLocale.ForeColor = theme.GetFontColor("muted");
                cmbSystemLocale.Font = theme.GetFont("muted");
                cmbSystemLocale.ForeColor = theme.GetFontColor("muted");
                
                lblUserLocale.Font = theme.GetFont("muted");
                lblUserLocale.ForeColor = theme.GetFontColor("muted");
                cmbUserLocale.Font = theme.GetFont("muted");
                cmbUserLocale.ForeColor = theme.GetFontColor("muted");
                
                lblSameAsSystemLocale.Font = theme.GetFont("muted");
                lblSameAsSystemLocale.ForeColor = theme.GetFontColor("muted");
                lblSameAsSystemLocale.Cursor = Cursors.Default;
                
                lblWindowsUILanguage.Font = theme.GetFont("muted");
                lblWindowsUILanguage.ForeColor = theme.GetFontColor("muted");
                cmbWindowsUILanguage.Font = theme.GetFont("muted");
                cmbWindowsUILanguage.ForeColor = theme.GetFontColor("muted");
                
                lblKeyboardLayout.Font = theme.GetFont("muted");
                lblKeyboardLayout.ForeColor = theme.GetFontColor("muted");
                cmbKeyboardLayout.Font = theme.GetFont("muted");
                cmbKeyboardLayout.ForeColor = theme.GetFontColor("muted");
                
                lblTimeZone.Font = theme.GetFont("muted");
                lblTimeZone.ForeColor = theme.GetFontColor("muted");
                cmbTimeZone.Font = theme.GetFont("muted");
                cmbTimeZone.ForeColor = theme.GetFontColor("muted");
            }

            // Notify MainForm to update lock/unlock button
            _onEnableToggle?.Invoke();
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
            if (cmbSystemLocale == null ||
                cmbUserLocale == null ||
                cmbWindowsUILanguage == null ||
                cmbKeyboardLayout == null ||
                cmbTimeZone == null)
            {
                Console.WriteLine("UpdateFromControls: UI controls not initialized yet!");
                return;
            }

            // Map dropdown indices to actual values
            SystemLocale = GetLocaleValueFromIndex(cmbSystemLocale.SelectedIndex);
            UserLocale = GetLocaleValueFromIndex(cmbUserLocale.SelectedIndex);
            WindowsUILanguage = GetLocaleValueFromIndex(cmbWindowsUILanguage.SelectedIndex);
            KeyboardLayout = GetKeyboardValueFromIndex(cmbKeyboardLayout.SelectedIndex);
            TimeZone = GetTimeZoneValueFromIndex(cmbTimeZone.SelectedIndex);
            
            //ThemeManager theme = ThemeManager.Instance;
            //SameAsSystemLocale = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
        }

        /// <summary>
        /// Clears all UI controls and resets to default state
        /// </summary>
        public void ClearControls()
        {
            _isLoading = true;

            try
            {
                // Try to load empty state from src/config/empty.json
                string emptyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "config", "empty.json");
                if (File.Exists(emptyPath))
                {
                    try
                    {
                        string jsonContent = File.ReadAllText(emptyPath);
                        JObject empty = JObject.Parse(jsonContent);

                        // Note: empty.json uses the key "langRegConfig"
                        if (empty["langRegConfig"] is JObject langRegSection)
                        {
                            // Populate data model
                            SetValues(langRegSection);

                            // Update UI from model
                            UpdateControlsFromModel();
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"LangRegConfig.ClearControls: failed to read/parse empty.json: {ex.Message}");
                        // fall through to manual clear fallback
                    }
                }

                // Fallback: Reset all dropdowns to first item ("Select one")
                cmbSystemLocale.SelectedIndex = 0;
                cmbUserLocale.SelectedIndex = 0;
                cmbWindowsUILanguage.SelectedIndex = 0;
                cmbKeyboardLayout.SelectedIndex = 0;
                cmbTimeZone.SelectedIndex = 0;

                // Reset toggle switch to ON (SameAsSystemLocale default true)
                ThemeManager theme = ThemeManager.Instance;
                theme.UpdateToggleSwitchState(toggleSameAsSystemLocale, true);

                // Clear data model
                SystemLocale = "";
                UserLocale = "";
                WindowsUILanguage = "";
                KeyboardLayout = "";
                TimeZone = "";
                SameAsSystemLocale = true;
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
            if (cmbSystemLocale == null ||
                cmbUserLocale == null ||
                cmbWindowsUILanguage == null ||
                cmbKeyboardLayout == null ||
                cmbTimeZone == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: SystemLocale={SystemLocale}, UserLocale={UserLocale}, WindowsUILanguage={WindowsUILanguage}, KeyboardLayout={KeyboardLayout}, TimeZone={TimeZone}");

            try
            {
                ThemeManager theme = ThemeManager.Instance;

                // Enable/disable controls based on EnableLangReg
                bool enableControls = EnableLangReg;
                            
                cmbSystemLocale.Enabled = enableControls;
                cmbUserLocale.Enabled = enableControls && !theme.GetToggleSwitchState(toggleSameAsSystemLocale);
                toggleSameAsSystemLocale.Enabled = enableControls;
                cmbWindowsUILanguage.Enabled = enableControls;
                cmbKeyboardLayout.Enabled = enableControls;
                cmbTimeZone.Enabled = enableControls;

                // Update muted state for toggles
                bool isMuted = !enableControls;
                theme.UpdateToggleSwitchMutedState(toggleSameAsSystemLocale, isMuted);

                // Update font styles based on enabled state
                if (enableControls)
                {
                    // Normal state
                    lblEnableLangReg.Font = theme.GetFont("normal");
                    lblEnableLangReg.ForeColor = theme.GetFontColor("normal");
                    lblSystemLocale.Font = theme.GetFont("normal");
                    lblSystemLocale.ForeColor = theme.GetFontColor("normal");
                    cmbSystemLocale.Font = theme.GetFont("normal");
                    cmbSystemLocale.ForeColor = theme.GetFontColor("inputForeground");
                    
                    // User Locale depends on toggleSameAsSystemLocale state
                    bool sameAsSystem = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
                    lblUserLocale.Font = theme.GetFont(sameAsSystem ? "muted" : "normal");
                    lblUserLocale.ForeColor = theme.GetFontColor(sameAsSystem ? "muted" : "normal");
                    cmbUserLocale.Font = theme.GetFont(sameAsSystem ? "muted" : "normal");
                    cmbUserLocale.ForeColor = theme.GetFontColor(sameAsSystem ? "muted" : "inputForeground");
                    
                    lblSameAsSystemLocale.Font = theme.GetFont(sameAsSystem ? "normal" : "muted");
                    lblSameAsSystemLocale.ForeColor = theme.GetFontColor(sameAsSystem ? "normal" : "muted");
                    lblSameAsSystemLocale.Cursor = Cursors.Hand;
                    
                    lblWindowsUILanguage.Font = theme.GetFont("normal");
                    lblWindowsUILanguage.ForeColor = theme.GetFontColor("normal");
                    cmbWindowsUILanguage.Font = theme.GetFont("normal");
                    cmbWindowsUILanguage.ForeColor = theme.GetFontColor("inputForeground");
                    
                    lblKeyboardLayout.Font = theme.GetFont("normal");
                    lblKeyboardLayout.ForeColor = theme.GetFontColor("normal");
                    cmbKeyboardLayout.Font = theme.GetFont("normal");
                    cmbKeyboardLayout.ForeColor = theme.GetFontColor("inputForeground");
                    
                    lblTimeZone.Font = theme.GetFont("normal");
                    lblTimeZone.ForeColor = theme.GetFontColor("normal");
                    cmbTimeZone.Font = theme.GetFont("normal");
                    cmbTimeZone.ForeColor = theme.GetFontColor("inputForeground");
                }
                else
                {
                    // Muted state
                    lblEnableLangReg.Font = theme.GetFont("muted");
                    lblEnableLangReg.ForeColor = theme.GetFontColor("muted");

                    lblSystemLocale.Font = theme.GetFont("muted");
                    lblSystemLocale.ForeColor = theme.GetFontColor("muted");
                    cmbSystemLocale.Font = theme.GetFont("muted");
                    cmbSystemLocale.ForeColor = theme.GetFontColor("muted");
                    
                    lblUserLocale.Font = theme.GetFont("muted");
                    lblUserLocale.ForeColor = theme.GetFontColor("muted");
                    cmbUserLocale.Font = theme.GetFont("muted");
                    cmbUserLocale.ForeColor = theme.GetFontColor("muted");
                    
                    lblSameAsSystemLocale.Font = theme.GetFont("muted");
                    lblSameAsSystemLocale.ForeColor = theme.GetFontColor("muted");
                    lblSameAsSystemLocale.Cursor = Cursors.Default;
                    
                    lblWindowsUILanguage.Font = theme.GetFont("muted");
                    lblWindowsUILanguage.ForeColor = theme.GetFontColor("muted");
                    cmbWindowsUILanguage.Font = theme.GetFont("muted");
                    cmbWindowsUILanguage.ForeColor = theme.GetFontColor("muted");
                    
                    lblKeyboardLayout.Font = theme.GetFont("muted");
                    lblKeyboardLayout.ForeColor = theme.GetFontColor("muted");
                    cmbKeyboardLayout.Font = theme.GetFont("muted");
                    cmbKeyboardLayout.ForeColor = theme.GetFontColor("muted");
                    
                    lblTimeZone.Font = theme.GetFont("muted");
                    lblTimeZone.ForeColor = theme.GetFontColor("muted");
                    cmbTimeZone.Font = theme.GetFont("muted");
                    cmbTimeZone.ForeColor = theme.GetFontColor("muted");
                }

                // Map data model values to dropdown indices
                cmbSystemLocale.SelectedIndex = GetIndexFromLocaleValue(SystemLocale);
                cmbUserLocale.SelectedIndex = GetIndexFromLocaleValue(UserLocale);
                cmbWindowsUILanguage.SelectedIndex = GetIndexFromLocaleValue(WindowsUILanguage);
                cmbKeyboardLayout.SelectedIndex = GetIndexFromKeyboardValue(KeyboardLayout);
                cmbTimeZone.SelectedIndex = GetIndexFromTimeZoneValue(TimeZone);

                // Handle special case: if toggle is ON but values don't match in JSON
                if (SameAsSystemLocale && !string.IsNullOrEmpty(SystemLocale) && UserLocale != SystemLocale)
                {
                    // Turn toggle OFF automatically
                    theme.UpdateToggleSwitchState(toggleSameAsSystemLocale, false);
                    SameAsSystemLocale = false;
                }

                // Update User Locale dropdown state based on toggle
                bool toggleState = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
                cmbUserLocale.Enabled = !toggleState;

                // Validate after loading
                ValidateAllUIFields();
            }
            finally
            {
                // Always reset the flag, even if an error occurs
                _isLoading = false;
            }
        }
        
        /// <summary>
        /// Gets the configuration values as a dictionary
        /// </summary>
        public Dictionary<string, string> GetValues()
        {
            return new Dictionary<string, string>
            {
                ["SystemLocale"] = SystemLocale,
                ["UserLocale"] = UserLocale,
                ["WindowsUILanguage"] = WindowsUILanguage,
                ["KeyboardLayout"] = KeyboardLayout,
                ["TimeZone"] = TimeZone,
                ["SameAsSystemLocale"] = SameAsSystemLocale.ToString()
            };
        }

        /// <summary>
        /// Sets configuration values from JSON
        /// </summary>
        public void SetValues(dynamic json)
        {
            if (json.systemLocale != null)
                SystemLocale = json.systemLocale;
            if (json.userLocale != null)
                UserLocale = json.userLocale;
            if (json.windowsUILanguage != null)
                WindowsUILanguage = json.windowsUILanguage;
            if (json.keyboardLayout != null)
                KeyboardLayout = json.keyboardLayout;
            if (json.timeZone != null)
                TimeZone = json.timeZone;
            if (json.sameAsSystemLocale != null)
                SameAsSystemLocale = json.sameAsSystemLocale;
        }

        #endregion

        #region Value Mapping Helpers

        /// <summary>
        /// Maps dropdown index to locale value (en-US, vi-VN, etc.)
        /// </summary>
        private string GetLocaleValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "en-US"; // English (United States)
                case 2: return "vi-VN"; // Vietnamese (Vietnam)
                case 3: return "zh-CN"; // Chinese (Simplified, China)
                case 4: return "zh-TW"; // Chinese (Traditional, Taiwan)
                case 5: return "ja-JP"; // Japanese (Japan)
                case 6: return "ko-KR"; // Korean (Korea)
                case 7: return "de-DE"; // German (Germany)
                case 8: return "fr-FR"; // French (France)
                case 9: return "es-ES"; // Spanish (Spain)
                case 10: return "pt-BR"; // Portuguese (Brazil)
                default: return ""; // Index 0 or invalid
            }
        }

        /// <summary>
        /// Maps locale value to dropdown index
        /// </summary>
        private int GetIndexFromLocaleValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value)
            {
                case "en-US": return 1;
                case "vi-VN": return 2;
                case "zh-CN": return 3;
                case "zh-TW": return 4;
                case "ja-JP": return 5;
                case "ko-KR": return 6;
                case "de-DE": return 7;
                case "fr-FR": return 8;
                case "es-ES": return 9;
                case "pt-BR": return 10;
                default: return 0; // Unknown value
            }
        }

        /// <summary>
        /// Maps dropdown index to keyboard layout value (0409:00000409, etc.)
        /// </summary>
        private string GetKeyboardValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "0409:00000409"; // US
                case 2: return "042a:0000042a"; // Vietnamese
                case 3: return "0804:00000804"; // Chinese (Simplified)
                case 4: return "0404:00000404"; // Chinese (Traditional)
                case 5: return "0411:00000411"; // Japanese
                case 6: return "0412:00000412"; // Korean
                case 7: return "0407:00000407"; // German
                case 8: return "040c:0000040c"; // French
                case 9: return "0c0a:0000040a"; // Spanish
                case 10: return "0416:00000416"; // Portuguese (Brazil)
                default: return ""; // Index 0 or invalid
            }
        }

        /// <summary>
        /// Maps keyboard layout value to dropdown index
        /// </summary>
        private int GetIndexFromKeyboardValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value)
            {
                case "0409:00000409": return 1;
                case "042a:0000042a": return 2;
                case "0804:00000804": return 3;
                case "0404:00000404": return 4;
                case "0411:00000411": return 5;
                case "0412:00000412": return 6;
                case "0407:00000407": return 7;
                case "040c:0000040c": return 8;
                case "0c0a:0000040a": return 9;
                case "0416:00000416": return 10;
                default: return 0; // Unknown value
            }
        }

        /// <summary>
        /// Maps dropdown index to Windows timezone ID
        /// </summary>
        private string GetTimeZoneValueFromIndex(int index)
        {
            switch (index)
            {
                case 1: return "Pacific Standard Time"; // (UTC-08:00) Pacific Time (US & Canada)
                case 2: return "Mountain Standard Time"; // (UTC-07:00) Mountain Time (US & Canada)
                case 3: return "Central Standard Time"; // (UTC-06:00) Central Time (US & Canada)
                case 4: return "Eastern Standard Time"; // (UTC-05:00) Eastern Time (US & Canada)
                case 5: return "UTC"; // (UTC) Coordinated Universal Time
                case 6: return "GMT Standard Time"; // (UTC+00:00) Dublin, Edinburgh, Lisbon, London
                case 7: return "W. Europe Standard Time"; // (UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna
                case 8: return "SE Asia Standard Time"; // (UTC+07:00) Bangkok, Hanoi, Jakarta
                case 9: return "China Standard Time"; // (UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi
                case 10: return "Tokyo Standard Time"; // (UTC+09:00) Osaka, Sapporo, Tokyo
                case 11: return "AUS Eastern Standard Time"; // (UTC+10:00) Sydney, Melbourne
                default: return ""; // Index 0 or invalid
            }
        }

        /// <summary>
        /// Maps Windows timezone ID to dropdown index
        /// </summary>
        private int GetIndexFromTimeZoneValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            switch (value)
            {
                case "Pacific Standard Time": return 1;
                case "Mountain Standard Time": return 2;
                case "Central Standard Time": return 3;
                case "Eastern Standard Time": return 4;
                case "UTC": return 5;
                case "GMT Standard Time": return 6;
                case "W. Europe Standard Time": return 7;
                case "SE Asia Standard Time": return 8;
                case "China Standard Time": return 9;
                case "Tokyo Standard Time": return 10;
                case "AUS Eastern Standard Time": return 11;
                default: return 0; // Unknown value
            }
        }

        #endregion

        #region UI Validation Methods

        /// <summary>
        /// Validates all fields and updates status rings
        /// </summary>
        public void ValidateAllUIFields()
        {
            ValidateSystemLocale();
            ValidateUserLocale();
            ValidateWindowsUILanguage();
            ValidateKeyboardLayout();
            ValidateTimeZone();
        }

        private void ValidateSystemLocale()
        {
            ValidateLocales(); // Use combined validation
        }

        private void ValidateUserLocale()
        {
            ValidateLocales(); // Use combined validation
        }

        /// <summary>
        /// Combined validation for both System and User Locale using a single status ring
        /// </summary>
        private void ValidateLocales()
        {
            ThemeManager theme = ThemeManager.Instance;
            bool toggleState = theme.GetToggleSwitchState(toggleSameAsSystemLocale);
            
            // Check System Locale first
            if (cmbSystemLocale.SelectedIndex <= 0)
            {
                ringSystemLocale.SetStatus(ValidationStatus.Invalid);
                return;
            }
            
            // If toggle is ON, User Locale is auto-synced, so just validate System Locale
            if (toggleState)
            {
                ringSystemLocale.SetStatus(ValidationStatus.Valid);
                return;
            }
            
            // Toggle is OFF, validate User Locale separately
            if (cmbUserLocale.SelectedIndex <= 0)
            {
                ringSystemLocale.SetStatus(ValidationStatus.Invalid);
                return;
            }
            
            // Both locales are filled, check if they're different
            if (cmbSystemLocale.SelectedIndex != cmbUserLocale.SelectedIndex)
            {
                ringSystemLocale.SetStatus(ValidationStatus.Warning);
            }
            else
            {
                ringSystemLocale.SetStatus(ValidationStatus.Valid);
            }
        }

        private void ValidateWindowsUILanguage()
        {
            if (cmbWindowsUILanguage.SelectedIndex <= 0)
            {
                ringWindowsUILanguage.SetStatus(ValidationStatus.Invalid);
            }
            else
            {
                ringWindowsUILanguage.SetStatus(ValidationStatus.Valid);
            }
        }

        private void ValidateKeyboardLayout()
        {
            if (cmbKeyboardLayout.SelectedIndex <= 0)
            {
                ringKeyboardLayout.SetStatus(ValidationStatus.Invalid);
            }
            else
            {
                ringKeyboardLayout.SetStatus(ValidationStatus.Valid);
            }
        }

        private void ValidateTimeZone()
        {
            if (cmbTimeZone.SelectedIndex <= 0)
            {
                ringTimeZone.SetStatus(ValidationStatus.Invalid);
            }
            else
            {
                ringTimeZone.SetStatus(ValidationStatus.Valid);
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates all language and region fields
        /// </summary>
        public List<string> Validate()
        {
            List<string> errors = new List<string>();
            LangManager lang = LangManager.Instance;

            // Validate System Locale
            if (string.IsNullOrWhiteSpace(SystemLocale))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("langRegConfig.systemLocale.label")));
            }

            // Validate User Locale
            if (!SameAsSystemLocale && string.IsNullOrWhiteSpace(UserLocale))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("langRegConfig.userLocale.label")));
            }

            // Validate Windows UI Language
            if (string.IsNullOrWhiteSpace(WindowsUILanguage))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("langRegConfig.windowsUILanguage.label")));
            }

            // Validate Keyboard Layout
            if (string.IsNullOrWhiteSpace(KeyboardLayout))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("langRegConfig.keyboardLayout.label")));
            }

            // Validate Time Zone
            if (string.IsNullOrWhiteSpace(TimeZone))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("langRegConfig.timeZone.label")));
            }

            return errors;
        }

        #endregion
    }
}
