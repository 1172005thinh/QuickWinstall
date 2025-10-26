using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles General Configuration section including UI, data model, and business logic
    /// </summary>
    public class GeneralConfig
    {
        #region Data Model

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
        
        private Label lblWindowsEdition = null!;
        private ComboBox cmbWindowsEdition = null!;
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
        private Label lblCPUArch = null!;
        private ComboBox cmbCPUArch = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false; // Flag to prevent event handlers during config loading

        #endregion

        #region UI Initialization

        /// <summary>
        /// Initializes the General Config section UI and returns the main panel
        /// </summary>
        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged, 
            Func<int, int, TextBox> createProductKeyTextBox, Func<Button> createRoundedButton)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            int contentHeight = ui.GetSectionValue("generalConfig", "contentHeight", 200);
            
            // General Config Panel
            pnlGeneralConfig = new Panel();
            pnlGeneralConfig.Location = new Point(0, 0);
            pnlGeneralConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlGeneralConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // General Config Toggle Button
            btnGeneralConfigToggle = createRoundedButton();
            btnGeneralConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnGeneralConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnGeneralConfigToggle.Image = iconMgr.GetIconAsImage("expand", theme.IsDarkTheme, ui.GlobalIconSize);
            btnGeneralConfigToggle.Tag = "collapsed";
            btnGeneralConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnGeneralConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.general"));

            // General Config Title
            lblGeneralConfigTitle = new Label();
            lblGeneralConfigTitle.Location = new Point(btnGeneralConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblGeneralConfigTitle.Size = new Size(300, ui.GlobalLabelHeight);
            lblGeneralConfigTitle.Text = lang.GetString("mainForm.sections.general");
            lblGeneralConfigTitle.Font = theme.GetFont("subheader");
            lblGeneralConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblGeneralConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblGeneralConfigTitle.Cursor = Cursors.Hand;
            lblGeneralConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblGeneralConfigTitle, "tooltips.generalConfig.header", lang.GetString("tooltips.generalConfig.header"));

            // Line Separator
            pnlGeneralConfigSeparator = new Panel();
            pnlGeneralConfigSeparator.Location = new Point(ui.GlobalTabX, btnGeneralConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlGeneralConfigSeparator.Size = new Size(pnlGeneralConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlGeneralConfigSeparator.BackColor = theme.GetColor("separator");

            // General Config Content Panel
            pnlGeneralConfigContent = new Panel();
            pnlGeneralConfigContent.Location = new Point(0, pnlGeneralConfigSeparator.Bottom + ui.GlobalSpacingY / 2);
            pnlGeneralConfigContent.Size = new Size(pnlGeneralConfig.Width, contentHeight);
            pnlGeneralConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlGeneralConfigContent.AutoScroll = true;

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
            cmbWindowsEdition.SelectedIndex = 0;
            cmbWindowsEdition.SelectedIndexChanged += onConfigChanged;
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

            txtProductKey1 = createProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            txtProductKey1.BackColor = theme.GetColor("inputBackground");
            txtProductKey1.Location = new Point(pkX, currentY);
            txtProductKey1.Tag = 1;
            tooltips.SetToolTip(txtProductKey1, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen1 = new Label();
            lblHyphen1.Location = new Point(pkX, currentY);
            lblHyphen1.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen1.Text = "-";
            lblHyphen1.Font = theme.GetFont("normal");
            lblHyphen1.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey2 = createProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            txtProductKey2.BackColor = theme.GetColor("inputBackground");
            txtProductKey2.Location = new Point(pkX, currentY);
            txtProductKey2.Tag = 2;
            tooltips.SetToolTip(txtProductKey2, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen2 = new Label();
            lblHyphen2.Location = new Point(pkX, currentY);
            lblHyphen2.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen2.Text = "-";
            lblHyphen2.Font = theme.GetFont("normal");
            lblHyphen2.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey3 = createProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            txtProductKey3.BackColor = theme.GetColor("inputBackground");
            txtProductKey3.Location = new Point(pkX, currentY);
            txtProductKey3.Tag = 3;
            txtProductKey3.Font = theme.GetFont("normal");
            tooltips.SetToolTip(txtProductKey3, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen3 = new Label();
            lblHyphen3.Location = new Point(pkX, currentY);
            lblHyphen3.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen3.Text = "-";
            lblHyphen3.Font = theme.GetFont("normal");
            lblHyphen3.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey4 = createProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            txtProductKey4.BackColor = theme.GetColor("inputBackground");
            txtProductKey4.Location = new Point(pkX, currentY);
            txtProductKey4.Tag = 4;
            tooltips.SetToolTip(txtProductKey4, "tooltips.generalConfig.productKey");
            pkX += pkWidth;

            lblHyphen4 = new Label();
            lblHyphen4.Location = new Point(pkX, currentY);
            lblHyphen4.Size = new Size(ui.GlobalSpacingX, ui.GlobalInputHeight);
            lblHyphen4.Text = "-";
            lblHyphen4.Font = theme.GetFont("normal");
            lblHyphen4.TextAlign = ContentAlignment.MiddleCenter;
            pkX += ui.GlobalSpacingX;

            txtProductKey5 = createProductKeyTextBox(pkWidth, ui.GlobalInputHeight);
            txtProductKey5.BackColor = theme.GetColor("inputBackground");
            txtProductKey5.Location = new Point(pkX, currentY);
            txtProductKey5.Tag = 5;
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
            cmbCPUArch.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCPUArch.BackColor = theme.GetColor("inputBackground");
            cmbCPUArch.ForeColor = theme.GetFontColor("inputForeground");
            cmbCPUArch.Items.AddRange(new object[] {
                lang.GetString("generalConfig.cpuArch.options.selectOne"),
                lang.GetString("generalConfig.cpuArch.options.x64"),
                lang.GetString("generalConfig.cpuArch.options.arm64")
            });
            cmbCPUArch.SelectedIndex = 0;
            cmbCPUArch.SelectedIndexChanged += onConfigChanged;
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

            // Set minimum size for content panel to enable horizontal scrollbar when form shrinks
            int minContentWidth = inputX + ui.GlobalInputWidth + ui.GlobalTabX;
            pnlGeneralConfigContent.MinimumSize = new Size(minContentWidth, contentHeight);

            // Add controls to General Config Panel
            pnlGeneralConfig.Controls.Add(btnGeneralConfigToggle);
            pnlGeneralConfig.Controls.Add(lblGeneralConfigTitle);
            pnlGeneralConfig.Controls.Add(pnlGeneralConfigSeparator);
            pnlGeneralConfig.Controls.Add(pnlGeneralConfigContent);

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

        #region UI Control Methods

        /// <summary>
        /// Toggles the visibility of the section content
        /// </summary>
        public void ToggleSection()
        {
            _isExpanded = !_isExpanded;
            pnlGeneralConfigContent.Visible = _isExpanded;
            pnlGeneralConfigSeparator.Visible = _isExpanded;

            // Update button icon
            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            UIValues ui = UIValues.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnGeneralConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnGeneralConfigToggle.Tag = _isExpanded ? "collapsed" : "expanded";
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
                textBox.Text = "XXXXX";
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
            if (cmbWindowsEdition == null || txtProductKey1 == null || cmbCPUArch == null)
            {
                Console.WriteLine("UpdateFromControls: UI controls not initialized yet!");
                return;
            }

            // Update WindowsEdition
            if (cmbWindowsEdition.SelectedIndex > 0)
            {
                WindowsEdition = cmbWindowsEdition.SelectedItem?.ToString() ?? "";
            }
            else
            {
                WindowsEdition = "";
            }

            // Update ProductKey
            string pk1 = txtProductKey1.Text.Trim();
            string pk2 = txtProductKey2.Text.Trim();
            string pk3 = txtProductKey3.Text.Trim();
            string pk4 = txtProductKey4.Text.Trim();
            string pk5 = txtProductKey5.Text.Trim();

            // Check if placeholders
            if (pk1 == "XXXXX") pk1 = "";
            if (pk2 == "XXXXX") pk2 = "";
            if (pk3 == "XXXXX") pk3 = "";
            if (pk4 == "XXXXX") pk4 = "";
            if (pk5 == "XXXXX") pk5 = "";

            if (string.IsNullOrEmpty(pk1) && string.IsNullOrEmpty(pk2) && 
                string.IsNullOrEmpty(pk3) && string.IsNullOrEmpty(pk4) && string.IsNullOrEmpty(pk5))
            {
                ProductKey = "";
            }
            else
            {
                ProductKey = $"{pk1}-{pk2}-{pk3}-{pk4}-{pk5}";
            }

            // Update CPUArchitecture
            if (cmbCPUArch.SelectedIndex > 0)
            {
                string selected = cmbCPUArch.SelectedItem?.ToString() ?? "";
                if (selected.Contains("x64"))
                    CPUArchitecture = "amd64";
                else if (selected.Contains("ARM64"))
                    CPUArchitecture = "arm64";
                else
                    CPUArchitecture = "";
            }
            else
            {
                CPUArchitecture = "";
            }
        }

        /// <summary>
        /// Clears all UI controls and resets to default state
        /// </summary>
        public void ClearControls()
        {
            // Check if UI is initialized
            if (cmbWindowsEdition == null || txtProductKey1 == null || cmbCPUArch == null)
                return;

            // Clear GeneralConfig inputs
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

        /// <summary>
        /// Updates UI controls from the data model
        /// </summary>
        public void UpdateControlsFromModel()
        {
            // Check if UI is initialized
            if (cmbWindowsEdition == null || txtProductKey1 == null || cmbCPUArch == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet!");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: WindowsEdition={WindowsEdition}, ProductKey={ProductKey}, CPUArchitecture={CPUArchitecture}");

            // Set flag to prevent event handlers from firing during loading
            _isLoading = true;

            try
            {
                LangManager lang = LangManager.Instance;
                ThemeManager theme = ThemeManager.Instance;

                // Update Windows Edition combo box
                if (!string.IsNullOrWhiteSpace(WindowsEdition))
                {
                    for (int i = 0; i < cmbWindowsEdition.Items.Count; i++)
                    {
                        if (cmbWindowsEdition.Items[i]?.ToString() == WindowsEdition)
                        {
                            cmbWindowsEdition.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cmbWindowsEdition.SelectedIndex = 0;
                }

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

                // Update CPU Architecture combo box
                if (!string.IsNullOrWhiteSpace(CPUArchitecture))
                {
                    string displayValue = CPUArchitecture.ToLower() switch
                    {
                        "amd64" => lang.GetString("generalConfig.cpuArch.options.x64"),
                        "arm64" => lang.GetString("generalConfig.cpuArch.options.arm64"),
                        _ => ""
                    };

                    if (!string.IsNullOrEmpty(displayValue))
                    {
                        for (int i = 0; i < cmbCPUArch.Items.Count; i++)
                        {
                            if (cmbCPUArch.Items[i]?.ToString() == displayValue)
                            {
                                cmbCPUArch.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cmbCPUArch.SelectedIndex = 0;
                }
            }
            finally
            {
                // Always reset the flag, even if an error occurs
                _isLoading = false;
            }
        }

        /// <summary>
        /// Helper to set a product key segment in a textbox
        /// </summary>
        private void SetProductKeySegment(TextBox textBox, string value, ThemeManager theme)
        {
            if (!string.IsNullOrWhiteSpace(value) && value != "XXXXX")
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

        #region Serialization

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
