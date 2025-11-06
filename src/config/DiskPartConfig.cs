using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Represents a single partition in the partition table
    /// </summary>
    public class PartitionEntry
    {
        public int ID { get; set; }
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public int SizeMB { get; set; }
        public string Letter { get; set; } = "";
        public string Format { get; set; } = "";
        public bool Active { get; set; }
        
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Type) &&
                   string.IsNullOrWhiteSpace(Name) &&
                   SizeMB == 0 &&
                   string.IsNullOrWhiteSpace(Letter) &&
                   string.IsNullOrWhiteSpace(Format);
        }
    }

    /// <summary>
    /// Handles Disk & Partition Configuration section including UI, data model, and business logic
    /// </summary>
    public class DiskPartConfig
    {
        #region Data Model

        public bool EnableAutoDiskPart { get; set; } = true;
        public bool EnableDiskPart { get; set; } = true;
        public int DiskID { get; set; } = 0;
        public bool WipeDisk { get; set; } = true;
        public string PartitionLayout { get; set; } = "";
        public bool UseRemainingSpace { get; set; } = true;
        public bool DisableBitLocker { get; set; } = true;
        public List<PartitionEntry> PartitionTable { get; set; } = new List<PartitionEntry>();

        #endregion

        #region UI Components

        private Panel pnlDiskPartConfig = null!;
        private Button btnDiskPartConfigToggle = null!;
        private Label lblDiskPartConfigTitle = null!;
        private Panel pnlDiskPartConfigSeparator = null!;
        private Panel pnlDiskPartConfigContent = null!;

        // Enable toggle
        private Label lblEnableDiskPart = null!;
        private Panel toggleEnableDiskPart = null!;

        // Enable Auto DiskPart
        private Label lblEnableAutoDiskPart = null!;
        private Panel toggleEnableAutoDiskPart = null!;

        // line separator
        private Panel pnlDiskPartConfigAutoDiskPartSeparator = null!;

        // Disk ID
        private Label lblDiskID = null!;
        private NumericUpDown nudDiskID = null!;
        private StatusRing ringDiskID = null!;

        // Wipe Disk
        private Label lblWipeDisk = null!;
        private Panel toggleWipeDisk = null!;

        // Partition Layout
        private Label lblPartitionLayout = null!;
        private ComboBox cmbPartitionLayout = null!;
        private StatusRing ringPartitionLayout = null!;

        // Partition Table
        private Label lblPartitionTable = null!;
        private Button btnQuickCreate = null!;
        private Button btnReset = null!;

        // Partition Table Header Labels
        private Label lblHeaderID = null!;
        private Label lblHeaderType = null!;
        private Label lblHeaderName = null!;
        private Label lblHeaderSize = null!;
        private Label lblHeaderLetter = null!;
        private Label lblHeaderFormat = null!;
        private Label lblHeaderActive = null!;

        // Partition Table Data Rows (8 rows)
        private List<Label> lblIDs = new List<Label>();
        private List<ComboBox> cmbTypes = new List<ComboBox>();
        private List<TextBox> txtNames = new List<TextBox>();
        private List<NumericUpDown> nudSizes = new List<NumericUpDown>();
        private List<ComboBox> cmbLetters = new List<ComboBox>();
        private List<ComboBox> cmbFormats = new List<ComboBox>();
        private List<Panel> toggleActives = new List<Panel>();

        // Use Remaining Space
        private Label lblUseRemainingSpace = null!;
        private Panel toggleUseRemainingSpace = null!;

        // Line Separator
        private Panel pnlDiskPartConfigAfterAutoDiskPartSeparator = null!;

        // Disable BitLocker
        private Label lblDisableBitLocker = null!;
        private Panel toggleDisableBitLocker = null!;

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

            // Calculate content height - will be larger once partition table is added
            int contentHeight = ui.GetSectionValue("diskPartConfig", "contentHeight", 800);

            pnlDiskPartConfig = new Panel();
            pnlDiskPartConfig.Location = new Point(0, 0);
            pnlDiskPartConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlDiskPartConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Toggle button
            btnDiskPartConfigToggle = createRoundedButton();
            btnDiskPartConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnDiskPartConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnDiskPartConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnDiskPartConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnDiskPartConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnDiskPartConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.diskPart"));

            // Title
            lblDiskPartConfigTitle = new Label();
            lblDiskPartConfigTitle.Location = new Point(btnDiskPartConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblDiskPartConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblDiskPartConfigTitle.Text = lang.GetString("mainForm.sections.diskPart");
            lblDiskPartConfigTitle.Font = theme.GetFont("subheader");
            lblDiskPartConfigTitle.UseMnemonic = false;
            lblDiskPartConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblDiskPartConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblDiskPartConfigTitle.Cursor = Cursors.Hand;
            lblDiskPartConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblDiskPartConfigTitle, "tooltips.diskPartConfig.header", lang.GetString("tooltips.diskPartConfig.header"));


            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);

            // Enable Disk & Partition Config
            // Only set EnableDiskPart on first initialization
            if (_isFirstInitialization)
            {
                EnableDiskPart = !SettingsManager.Instance.LockSectionsAtStartup;
                _isFirstInitialization = false;
            }
            toggleEnableDiskPart = theme.CreateToggleSwitch(new Point(ui.GlobalTabX * 2 + ui.GlobalBtnBox, btnDiskPartConfigToggle.Bottom + ui.GlobalSpacingY), toggleWidth, ui.GlobalInputHeight, EnableDiskPart);
            toggleEnableDiskPart.TabStop = false;
            toggleEnableDiskPart.Click += (s, e) => OnToggleEnableDiskPart();

            lblEnableDiskPart = new Label();
            lblEnableDiskPart.Location = new Point(toggleEnableDiskPart.Right + ui.GlobalSpacingX, btnDiskPartConfigToggle.Bottom + ui.GlobalSpacingY);
            lblEnableDiskPart.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblEnableDiskPart.Text = string.Format(lang.GetString("diskPartConfig.enableDiskPart.label", lang.GetString("mainForm.sections.diskPart")));
            lblEnableDiskPart.Font = theme.GetFont("normal");
            lblEnableDiskPart.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableDiskPart.Cursor = Cursors.Hand;
            lblEnableDiskPart.Click += (s, e) => OnToggleEnableDiskPart();
            tooltips.SetToolTip(lblEnableDiskPart, "tooltips.diskPartConfig.enableDiskPart");

            // Line Separator
            pnlDiskPartConfigSeparator = new Panel();
            pnlDiskPartConfigSeparator.Location = new Point(ui.GlobalTabX, toggleEnableDiskPart.Bottom + ui.GlobalSpacingY * 2);
            pnlDiskPartConfigSeparator.Size = new Size(pnlDiskPartConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlDiskPartConfigSeparator.BackColor = theme.GetColor("separator");

            // Content panel
            pnlDiskPartConfigContent = new Panel();
            pnlDiskPartConfigContent.Location = new Point(0, pnlDiskPartConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlDiskPartConfigContent.Size = new Size(pnlDiskPartConfig.Width, contentHeight);
            pnlDiskPartConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDiskPartConfigContent.AutoScroll = false;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int toggleX = pnlDiskPartConfigContent.Right - ui.GlobalTabX - toggleWidth;
            int currentY = ui.GlobalSpacingY;

            // Enable Auto DiskPart
            lblEnableAutoDiskPart = new Label();
            lblEnableAutoDiskPart.Location = new Point(labelX, currentY);
            lblEnableAutoDiskPart.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblEnableAutoDiskPart.Text = lang.GetString("diskPartConfig.enableAutoDiskPart.label");
            lblEnableAutoDiskPart.Font = theme.GetFont("normal");
            lblEnableAutoDiskPart.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableAutoDiskPart.Cursor = Cursors.Hand;
            tooltips.SetToolTip(lblEnableAutoDiskPart, "tooltips.diskPartConfig.enableAutoDiskPart");

            toggleEnableAutoDiskPart = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, true);
            toggleEnableAutoDiskPart.Click += (s, e) => OnToggleEnableAutoDiskPart();

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Line Separator
            pnlDiskPartConfigAutoDiskPartSeparator = new Panel();
            pnlDiskPartConfigAutoDiskPartSeparator.Location = new Point(labelX, currentY);
            pnlDiskPartConfigAutoDiskPartSeparator.Size = new Size(pnlDiskPartConfig.Width - labelX - ui.GlobalTabX, 1);
            pnlDiskPartConfigAutoDiskPartSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            labelX = ui.GlobalTabX * 3 + ui.GlobalBtnBox + ui.GlobalSpacingX;
            inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            toggleX = pnlDiskPartConfigContent.Width - toggleWidth - ui.GlobalSpacingX;

            // Disk ID
            lblDiskID = new Label();
            lblDiskID.Location = new Point(labelX, currentY);
            lblDiskID.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblDiskID.Text = lang.GetString("diskPartConfig.diskID.label");
            lblDiskID.Font = theme.GetFont("normal");
            lblDiskID.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblDiskID, "tooltips.diskPartConfig.diskID");

            nudDiskID = new NumericUpDown();
            nudDiskID.Location = new Point(inputX, currentY);
            nudDiskID.Size = new Size((int)(ui.GlobalInputWidth * 0.5), ui.GlobalInputHeight);
            nudDiskID.Minimum = 0;
            nudDiskID.Maximum = 255;
            nudDiskID.Value = DiskID;
            nudDiskID.Font = theme.GetFont("normal");
            nudDiskID.BackColor = theme.GetColor("inputBackground");
            nudDiskID.ForeColor = theme.GetFontColor("inputForeground");
            nudDiskID.ValueChanged += onConfigChanged;
            nudDiskID.ValueChanged += (s, e) => ValidateDiskID();
            tooltips.SetToolTip(nudDiskID, "tooltips.diskPartConfig.diskID");

            // Status ring for Disk ID
            int statusRingBorderExtra = 6;
            ringDiskID = new StatusRing();
            ringDiskID.Location = new Point(nudDiskID.Left - ui.GetValue("global.statusRing.borderWidth"), nudDiskID.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringDiskID.Size = new Size(nudDiskID.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), nudDiskID.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringDiskID.Visible = false;

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Wipe Disk
            lblWipeDisk = new Label();
            lblWipeDisk.Location = new Point(labelX, currentY);
            lblWipeDisk.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblWipeDisk.Text = lang.GetString("diskPartConfig.wipeDisk.label");
            lblWipeDisk.Font = theme.GetFont("subheader");
            lblWipeDisk.ForeColor = theme.GetFontColor("normal");
            lblWipeDisk.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblWipeDisk, "tooltips.diskPartConfig.wipeDisk");

            toggleWipeDisk = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, WipeDisk);
            toggleWipeDisk.TabStop = false;
            toggleWipeDisk.Click += (s, e) => OnToggleWipeDisk();

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Partition Layout
            lblPartitionLayout = new Label();
            lblPartitionLayout.Location = new Point(labelX, currentY);
            lblPartitionLayout.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblPartitionLayout.Text = lang.GetString("diskPartConfig.partitionLayout.label");
            lblPartitionLayout.Font = theme.GetFont("normal");
            lblPartitionLayout.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblPartitionLayout, "tooltips.diskPartConfig.partitionLayout");

            cmbPartitionLayout = new ComboBox();
            cmbPartitionLayout.Location = new Point(inputX, currentY);
            cmbPartitionLayout.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbPartitionLayout.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPartitionLayout.BackColor = theme.GetColor("inputBackground");
            cmbPartitionLayout.ForeColor = theme.GetFontColor("inputForeground");
            cmbPartitionLayout.Items.AddRange(new object[] {
                lang.GetString("diskPartConfig.partitionLayout.options.selectOne"),
                lang.GetString("diskPartConfig.partitionLayout.options.gptUEFI"),
                lang.GetString("diskPartConfig.partitionLayout.options.mbrBIOS")
            });

            // Status ring for Partition Layout
            ringPartitionLayout = new StatusRing();
            ringPartitionLayout.Location = new Point(cmbPartitionLayout.Left - ui.GetValue("global.statusRing.borderWidth"), cmbPartitionLayout.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringPartitionLayout.Size = new Size(cmbPartitionLayout.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), cmbPartitionLayout.Height + 2 * ui.GetValue("global.statusRing.borderWidth") + statusRingBorderExtra);
            ringPartitionLayout.Visible = false;

            cmbPartitionLayout.SelectedIndex = 0;
            cmbPartitionLayout.SelectedIndexChanged += onConfigChanged;
            cmbPartitionLayout.SelectedIndexChanged += (s, e) =>
            {
                if (!_isLoading)
                {
                    // Update property immediately
                    PartitionLayout = GetPartitionLayoutValueFromIndex(cmbPartitionLayout.SelectedIndex);
                }
                ValidatePartitionLayout();
            };
            tooltips.SetToolTip(cmbPartitionLayout, "tooltips.diskPartConfig.partitionLayout");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Partition Table section (placeholder for now)
            lblPartitionTable = new Label();
            lblPartitionTable.Location = new Point(labelX, currentY);
            lblPartitionTable.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblPartitionTable.Text = lang.GetString("diskPartConfig.partitionTable.title");
            lblPartitionTable.Font = theme.GetFont("normal");
            lblPartitionTable.TextAlign = ContentAlignment.MiddleLeft;

            btnQuickCreate = createRoundedButton();
            btnQuickCreate.Location = new Point(inputX, currentY);
            btnQuickCreate.Size = new Size((int)(ui.GlobalBtnWidth * 1.25), ui.GlobalBtnHeight);
            btnQuickCreate.Font = theme.GetFont("normal");
            btnQuickCreate.ForeColor = theme.GetFontColor("normal");
            btnQuickCreate.TextAlign = ContentAlignment.MiddleCenter;
            btnQuickCreate.Text = lang.GetString("diskPartConfig.partitionTable.quickCreate");
            btnQuickCreate.Enabled = false; // Will be enabled when partition layout is selected
            btnQuickCreate.Click += (s, e) => QuickCreatePartitionTable();
            tooltips.SetToolTip(btnQuickCreate, "tooltips.diskPartConfig.partitionTable.quickCreate");

            btnReset = createRoundedButton();
            btnReset.Location = new Point(btnQuickCreate.Right + ui.GlobalSpacingX, currentY);
            btnReset.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnReset.Image = iconMgr.GetIconAsImage("reset", theme.IsDarkTheme, ui.GlobalIconSize);
            btnReset.Click += (s, e) => ResetPartitionTable();
            tooltips.SetToolTip(btnReset, "tooltips.diskPartConfig.partitionTable.reset");

            currentY += ui.GlobalBtnHeight + ui.GlobalSpacingY * 2;

            // TODO: Add partition table rows here in next phase

            // Add placeholder for partition table
            Label lblPartitionTablePlaceholder = new Label();
            lblPartitionTablePlaceholder.Location = new Point(labelX, currentY);
            lblPartitionTablePlaceholder.Size = new Size(ui.GlobalInputWidth + ui.GlobalLabelWidth, ui.GlobalLabelHeight * 2);
            lblPartitionTablePlaceholder.Text = "[Partition Table - Coming in next phase]";
            lblPartitionTablePlaceholder.Font = theme.GetFont("muted");
            lblPartitionTablePlaceholder.ForeColor = theme.GetFontColor("muted");
            lblPartitionTablePlaceholder.TextAlign = ContentAlignment.MiddleCenter;

            currentY += lblPartitionTablePlaceholder.Height + ui.GlobalSpacingY * 2;

            // Use Remaining Space
            lblUseRemainingSpace = new Label();
            lblUseRemainingSpace.Location = new Point(labelX, currentY);
            lblUseRemainingSpace.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblUseRemainingSpace.Text = lang.GetString("diskPartConfig.useRemainingSpace.label");
            lblUseRemainingSpace.Font = theme.GetFont("subheader");
            lblUseRemainingSpace.ForeColor = theme.GetFontColor("normal");
            lblUseRemainingSpace.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblUseRemainingSpace, "tooltips.diskPartConfig.useRemainingSpace");

            toggleUseRemainingSpace = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, UseRemainingSpace);
            toggleUseRemainingSpace.TabStop = false;
            toggleUseRemainingSpace.Click += (s, e) => OnToggleUseRemainingSpace();

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;
            labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;

            // Line Separator
            pnlDiskPartConfigAfterAutoDiskPartSeparator = new Panel();
            pnlDiskPartConfigAfterAutoDiskPartSeparator.Location = new Point(labelX, currentY);
            pnlDiskPartConfigAfterAutoDiskPartSeparator.Size = new Size(pnlDiskPartConfigContent.Width, 1);
            pnlDiskPartConfigAfterAutoDiskPartSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            // Disable BitLocker
            lblDisableBitLocker = new Label();
            lblDisableBitLocker.Location = new Point(labelX, currentY);
            lblDisableBitLocker.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblDisableBitLocker.Text = lang.GetString("diskPartConfig.disableBitLocker.label");
            lblDisableBitLocker.Font = theme.GetFont("normal");
            lblDisableBitLocker.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblDisableBitLocker, "tooltips.diskPartConfig.disableBitLocker");

            toggleDisableBitLocker = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, DisableBitLocker);
            toggleDisableBitLocker.TabStop = false;
            toggleDisableBitLocker.Click += (s, e) => OnToggleDisableBitLocker();

            // Add controls to content panel
            pnlDiskPartConfigContent.Controls.Add(lblEnableAutoDiskPart);
            pnlDiskPartConfigContent.Controls.Add(toggleEnableAutoDiskPart);
            pnlDiskPartConfigContent.Controls.Add(pnlDiskPartConfigAutoDiskPartSeparator);
            pnlDiskPartConfigContent.Controls.Add(lblDiskID);
            pnlDiskPartConfigContent.Controls.Add(nudDiskID);
            pnlDiskPartConfigContent.Controls.Add(ringDiskID);
            pnlDiskPartConfigContent.Controls.Add(lblWipeDisk);
            pnlDiskPartConfigContent.Controls.Add(toggleWipeDisk);
            pnlDiskPartConfigContent.Controls.Add(lblPartitionLayout);
            pnlDiskPartConfigContent.Controls.Add(cmbPartitionLayout);
            pnlDiskPartConfigContent.Controls.Add(ringPartitionLayout);
            pnlDiskPartConfigContent.Controls.Add(lblPartitionTable);
            pnlDiskPartConfigContent.Controls.Add(btnQuickCreate);
            pnlDiskPartConfigContent.Controls.Add(btnReset);
            pnlDiskPartConfigContent.Controls.Add(lblPartitionTablePlaceholder);
            pnlDiskPartConfigContent.Controls.Add(lblUseRemainingSpace);
            pnlDiskPartConfigContent.Controls.Add(toggleUseRemainingSpace);
            pnlDiskPartConfigContent.Controls.Add(pnlDiskPartConfigAfterAutoDiskPartSeparator);
            pnlDiskPartConfigContent.Controls.Add(lblDisableBitLocker);
            pnlDiskPartConfigContent.Controls.Add(toggleDisableBitLocker);

            // Add controls to main panel
            pnlDiskPartConfig.Controls.Add(btnDiskPartConfigToggle);
            pnlDiskPartConfig.Controls.Add(lblDiskPartConfigTitle);
            pnlDiskPartConfig.Controls.Add(toggleEnableDiskPart);
            pnlDiskPartConfig.Controls.Add(lblEnableDiskPart);
            pnlDiskPartConfig.Controls.Add(pnlDiskPartConfigSeparator);
            pnlDiskPartConfig.Controls.Add(pnlDiskPartConfigContent);

            // Apply current expansion state
            pnlDiskPartConfigContent.Visible = _isExpanded;
            pnlDiskPartConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlDiskPartConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            // Apply muted state if EnableDiskPart is locked at startup
            if (!EnableDiskPart)
            {
                UpdateControlsFromModel();
            }

            return pnlDiskPartConfig;
        }

        public void LoadConfigIntoUI()
        {
            try
            {
                Console.WriteLine("DiskPartConfig: Load config into UI...");
                UpdateControlsFromModel();
                Console.WriteLine("DiskPartConfig: Config loaded into UI successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DiskPartConfig: Error loading config into UI: {ex.Message}");
            }
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlDiskPartConfigContent.Visible = _isExpanded;
            pnlDiskPartConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("diskPartConfig", "contentHeight", 800);
                pnlDiskPartConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnDiskPartConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.diskPart"));
            }
            else
            {
                pnlDiskPartConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnDiskPartConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.diskPart"));
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;

            string iconName = _isExpanded ? "expand" : "collapse";
            btnDiskPartConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnDiskPartConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
            theme.UpdateToggleSwitchState(toggleEnableDiskPart, enabled);
            EnableDiskPart = enabled;

            // Update dependent controls muted state
            UpdateControlsFromModel();
        }

        /// <summary>
        /// Handles the toggle for Enable Disk & Partition Config
        /// </summary>
        private void OnToggleEnableDiskPart()
        {
            if (_isLoading) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleEnableDiskPart);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableDiskPart, newState);

            EnableDiskPart = newState;

            // Enable or disable all dependent controls
            bool enableControls = newState;

            toggleEnableAutoDiskPart.Enabled = enableControls;
            toggleWipeDisk.Enabled = enableControls;
            nudDiskID.Enabled = enableControls;
            cmbPartitionLayout.Enabled = enableControls;
            btnQuickCreate.Enabled = enableControls && cmbPartitionLayout.SelectedIndex > 0;
            btnReset.Enabled = enableControls;
            toggleUseRemainingSpace.Enabled = enableControls;
            toggleDisableBitLocker.Enabled = enableControls;

            // Update visual appearance based on enabled state
            bool isMuted = !newState;

            //Update all toggle switches to muted or normal state
            theme.UpdateToggleSwitchState(toggleEnableAutoDiskPart, isMuted);
            theme.UpdateToggleSwitchState(toggleWipeDisk, isMuted);
            theme.UpdateToggleSwitchState(toggleDisableBitLocker, isMuted);

            // Update all labels to muted or normal state
            if (isMuted)
            {
                lblEnableDiskPart.Font = theme.GetFont("muted");
                lblEnableDiskPart.ForeColor = theme.GetFontColor("muted");

                lblEnableAutoDiskPart.Font = theme.GetFont("muted");
                lblEnableAutoDiskPart.ForeColor = theme.GetFontColor("muted");
                lblDiskID.Font = theme.GetFont("muted");
                lblDiskID.ForeColor = theme.GetFontColor("muted");
                nudDiskID.Font = theme.GetFont("muted");
                nudDiskID.ForeColor = theme.GetFontColor("muted");
                lblWipeDisk.Font = theme.GetFont("muted");
                lblWipeDisk.ForeColor = theme.GetFontColor("muted");
                lblPartitionLayout.Font = theme.GetFont("muted");
                lblPartitionLayout.ForeColor = theme.GetFontColor("muted");
                cmbPartitionLayout.Font = theme.GetFont("muted");
                cmbPartitionLayout.ForeColor = theme.GetFontColor("muted");
                lblPartitionTable.Font = theme.GetFont("muted");
                lblPartitionTable.ForeColor = theme.GetFontColor("muted");
                lblUseRemainingSpace.Font = theme.GetFont("muted");
                lblUseRemainingSpace.ForeColor = theme.GetFontColor("muted");
                lblDisableBitLocker.Font = theme.GetFont("muted");
                lblDisableBitLocker.ForeColor = theme.GetFontColor("muted");
            }
            else
            {
                lblEnableDiskPart.Font = theme.GetFont("normal");
                lblEnableDiskPart.ForeColor = theme.GetFontColor("normal");

                lblEnableAutoDiskPart.Font = theme.GetFont("normal");
                lblEnableAutoDiskPart.ForeColor = theme.GetFontColor("normal");
                lblDiskID.Font = theme.GetFont("normal");
                lblDiskID.ForeColor = theme.GetFontColor("normal");
                nudDiskID.Font = theme.GetFont("normal");
                nudDiskID.ForeColor = theme.GetFontColor("inputForeground");
                lblWipeDisk.Font = theme.GetFont("normal");
                lblWipeDisk.ForeColor = theme.GetFontColor("normal");
                lblPartitionLayout.Font = theme.GetFont("normal");
                lblPartitionLayout.ForeColor = theme.GetFontColor("normal");
                cmbPartitionLayout.Font = theme.GetFont("normal");
                cmbPartitionLayout.ForeColor = theme.GetFontColor("inputForeground");
                lblPartitionTable.Font = theme.GetFont("normal");
                lblPartitionTable.ForeColor = theme.GetFontColor("normal");
                lblUseRemainingSpace.Font = theme.GetFont("normal");
                lblUseRemainingSpace.ForeColor = theme.GetFontColor("normal");
                lblDisableBitLocker.Font = theme.GetFont("normal");
                lblDisableBitLocker.ForeColor = theme.GetFontColor("normal");
            }

            // Notify MainForm to update lock/unlock button
            _onEnableToggle?.Invoke();
        }

        /// <summary>
        /// Handles the toggle for Enable Auto DiskPart
        /// </summary>
        private void OnToggleEnableAutoDiskPart()
        {
            if (_isLoading || !EnableDiskPart) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleEnableAutoDiskPart);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableAutoDiskPart, newState);

            EnableAutoDiskPart = newState;

            // Update dependent controls enabled/disabled state
            nudDiskID.Enabled = newState;
            toggleWipeDisk.Enabled = newState;
            cmbPartitionLayout.Enabled = newState;
            toggleUseRemainingSpace.Enabled = newState;
            
            // Update label colors to muted/normal based on state
            if (newState)
            {
                lblDiskID.ForeColor = theme.GetFontColor("normal");
                lblWipeDisk.ForeColor = theme.GetFontColor("normal");
                lblPartitionLayout.ForeColor = theme.GetFontColor("normal");
                lblPartitionTable.ForeColor = theme.GetFontColor("normal");
                lblUseRemainingSpace.ForeColor = theme.GetFontColor("normal");
            }
            else
            {
                lblDiskID.ForeColor = theme.GetFontColor("muted");
                lblWipeDisk.ForeColor = theme.GetFontColor("muted");
                lblPartitionLayout.ForeColor = theme.GetFontColor("muted");
                lblPartitionTable.ForeColor = theme.GetFontColor("muted");
                lblUseRemainingSpace.ForeColor = theme.GetFontColor("muted");
            }

            // Update Quick Create button state
            UpdateQuickCreateButtonState();

            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle for Wipe Disk
        /// </summary>
        private void OnToggleWipeDisk()
        {
            if (_isLoading || !EnableDiskPart) return;
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleWipeDisk);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleWipeDisk, newState);
            WipeDisk = newState;
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle for Use Remaining Space
        /// </summary>
        private void OnToggleUseRemainingSpace()
        {
            if (_isLoading || !EnableDiskPart) return;
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleUseRemainingSpace);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleUseRemainingSpace, newState);
            UseRemainingSpace = newState;
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle for Disable BitLocker
        /// </summary>
        private void OnToggleDisableBitLocker()
        {
            if (_isLoading || !EnableDiskPart) return;
            ThemeManager theme = ThemeManager.Instance;
            bool currentState = theme.GetToggleSwitchState(toggleDisableBitLocker);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleDisableBitLocker, newState);
            DisableBitLocker = newState;
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the Quick Create button enabled state based on current selections
        /// </summary>
        private void UpdateQuickCreateButtonState()
        {
            if (btnQuickCreate == null || cmbPartitionLayout == null)
                return;

            ThemeManager theme = ThemeManager.Instance;

            // Button should be enabled when EnableAutoDiskPart is true AND a valid partition layout is selected
            bool shouldEnable = EnableAutoDiskPart && !string.IsNullOrEmpty(PartitionLayout);
            btnQuickCreate.Enabled = shouldEnable;

            // Apply muted or normal font style based on enabled state
            if (shouldEnable)
            {
                btnQuickCreate.Font = theme.GetFont("normal");
                btnQuickCreate.ForeColor = theme.GetFontColor("normal");
            }
            else
            {
                btnQuickCreate.Font = theme.GetFont("muted");
                btnQuickCreate.ForeColor = theme.GetFontColor("muted");
            }
        }

        private void QuickCreatePartitionTable()
        {
            // TODO: Implement in next phase
            // This will auto-generate partition table based on selected layout
            MessageBox.Show("Quick Create - Coming in next phase!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResetPartitionTable()
        {
            // Clear partition table
            PartitionTable.Clear();

            // TODO: In next phase, also clear all partition row controls

            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Data Model Operations

        public void UpdateFromControls()
        {
            if (_isLoading) return;

            // Check if UI is initialized
            if (toggleEnableAutoDiskPart == null ||
                nudDiskID == null ||
                toggleWipeDisk == null ||
                cmbPartitionLayout == null ||
                toggleUseRemainingSpace == null ||
                toggleDisableBitLocker == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet.");
                return;
            }

            ThemeManager theme = ThemeManager.Instance;

            // EnableDiskPart should never be saved - it's a safety lock that always resets to true
            // EnableDiskPart = theme.GetToggleSwitchState(toggleEnableDiskPart);

            EnableAutoDiskPart = theme.GetToggleSwitchState(toggleEnableAutoDiskPart);
            DiskID = (int)nudDiskID.Value;
            WipeDisk = theme.GetToggleSwitchState(toggleWipeDisk);
            PartitionLayout = GetPartitionLayoutValueFromIndex(cmbPartitionLayout.SelectedIndex);
            UseRemainingSpace = theme.GetToggleSwitchState(toggleUseRemainingSpace);
            DisableBitLocker = theme.GetToggleSwitchState(toggleDisableBitLocker);
        }

        public void ClearControls()
        {
            _isLoading = true;

            try
            {
                ThemeManager theme = ThemeManager.Instance;

                EnableDiskPart = true;

                // Reset all other controls to off state
                theme.UpdateToggleSwitchState(toggleEnableAutoDiskPart, false);
                theme.UpdateToggleSwitchState(toggleWipeDisk, false);
                theme.UpdateToggleSwitchState(toggleUseRemainingSpace, false);
                theme.UpdateToggleSwitchState(toggleDisableBitLocker, false);

                // Enable all controls (since EnableDiskPart is true)
                toggleEnableAutoDiskPart.Enabled = true;
                nudDiskID.Enabled = true;
                toggleWipeDisk.Enabled = true;
                cmbPartitionLayout.Enabled = true;
                btnQuickCreate.Enabled = false;
                btnReset.Enabled = true;
                toggleUseRemainingSpace.Enabled = true;
                toggleDisableBitLocker.Enabled = true;

                // Apply normal visual state (not muted)
                theme.UpdateToggleSwitchState(toggleEnableAutoDiskPart, false);
                theme.UpdateToggleSwitchState(toggleWipeDisk, false);
                theme.UpdateToggleSwitchState(toggleUseRemainingSpace, false);
                theme.UpdateToggleSwitchState(toggleDisableBitLocker, false);

                // Update all labels to normal state
                lblEnableAutoDiskPart.Font = theme.GetFont("normal");
                lblEnableAutoDiskPart.ForeColor = theme.GetFontColor("normal");
                lblDiskID.Font = theme.GetFont("normal");
                lblDiskID.ForeColor = theme.GetFontColor("normal");
                nudDiskID.Font = theme.GetFont("normal");
                nudDiskID.ForeColor = theme.GetFontColor("inputForeground");
                lblWipeDisk.Font = theme.GetFont("normal");
                lblWipeDisk.ForeColor = theme.GetFontColor("normal");
                lblPartitionLayout.Font = theme.GetFont("normal");
                lblPartitionLayout.ForeColor = theme.GetFontColor("normal");
                cmbPartitionLayout.Font = theme.GetFont("normal");
                cmbPartitionLayout.ForeColor = theme.GetFontColor("inputForeground");
                lblPartitionTable.Font = theme.GetFont("normal");
                lblPartitionTable.ForeColor = theme.GetFontColor("normal");
                lblUseRemainingSpace.Font = theme.GetFont("normal");
                lblUseRemainingSpace.ForeColor = theme.GetFontColor("normal");
                lblDisableBitLocker.Font = theme.GetFont("normal");
                lblDisableBitLocker.ForeColor = theme.GetFontColor("normal");

                // Reset ComboBoxes and NumericUpDowns to default selections
                if (nudDiskID != null) nudDiskID.Value = 0;
                if (cmbPartitionLayout != null) cmbPartitionLayout.SelectedIndex = 0;

                // Reset ComboBox lables to normal state
                if (lblDiskID != null)
                {
                    lblDiskID.Font = theme.GetFont("normal");
                    lblDiskID.ForeColor = theme.GetFontColor("normal");
                }
                if (lblPartitionLayout != null)
                {
                    lblPartitionLayout.Font = theme.GetFont("normal");
                    lblPartitionLayout.ForeColor = theme.GetFontColor("normal");
                }

                // Reset data model (except EnableDiskPart which stays true)
                EnableAutoDiskPart = false;
                DiskID = 0;
                WipeDisk = false;
                PartitionLayout = "";
                UseRemainingSpace = false;
                DisableBitLocker = false;
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
            if (toggleEnableAutoDiskPart == null ||
                nudDiskID == null ||
                toggleWipeDisk == null ||
                cmbPartitionLayout == null ||
                toggleUseRemainingSpace == null ||
                toggleDisableBitLocker == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet.");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: EnableAutoDiskPart={EnableAutoDiskPart}, DiskID={DiskID}, WipeDisk={WipeDisk}, PartitionLayout={PartitionLayout}, UseRemainingSpace={UseRemainingSpace}, DisableBitLocker={DisableBitLocker}");

            try
            {
                ThemeManager theme = ThemeManager.Instance;

                // Update all toggle states
                theme.UpdateToggleSwitchState(toggleEnableDiskPart, EnableDiskPart);
                theme.UpdateToggleSwitchState(toggleEnableAutoDiskPart, EnableAutoDiskPart);
                theme.UpdateToggleSwitchState(toggleWipeDisk, WipeDisk);
                theme.UpdateToggleSwitchState(toggleUseRemainingSpace, UseRemainingSpace);
                theme.UpdateToggleSwitchState(toggleDisableBitLocker, DisableBitLocker);

                // Enable/Disable dependent controls based on EnableDiskPart
                bool enableControls = EnableDiskPart;

                toggleEnableAutoDiskPart.Enabled = enableControls;
                nudDiskID.Enabled = enableControls && EnableAutoDiskPart;
                toggleWipeDisk.Enabled = enableControls && EnableAutoDiskPart;
                cmbPartitionLayout.Enabled = enableControls && EnableAutoDiskPart;
                btnQuickCreate.Enabled = enableControls && EnableAutoDiskPart && !string.IsNullOrEmpty(PartitionLayout);
                btnReset.Enabled = enableControls && EnableAutoDiskPart;
                toggleUseRemainingSpace.Enabled = enableControls && EnableAutoDiskPart;
                toggleDisableBitLocker.Enabled = enableControls;

                // Update visual appearance based on enabled state
                bool isMuted = !EnableDiskPart;

                // Update all labels to muted or normal state
                if (isMuted)
                {
                    lblEnableDiskPart.Font = theme.GetFont("muted");
                    lblEnableDiskPart.ForeColor = theme.GetFontColor("muted");

                    lblEnableAutoDiskPart.Font = theme.GetFont("muted");
                    lblEnableAutoDiskPart.ForeColor = theme.GetFontColor("muted");
                    lblDiskID.Font = theme.GetFont("muted");
                    lblDiskID.ForeColor = theme.GetFontColor("muted");
                    nudDiskID.Font = theme.GetFont("muted");
                    nudDiskID.ForeColor = theme.GetFontColor("muted");
                    lblWipeDisk.Font = theme.GetFont("muted");
                    lblWipeDisk.ForeColor = theme.GetFontColor("muted");
                    lblPartitionLayout.Font = theme.GetFont("muted");
                    lblPartitionLayout.ForeColor = theme.GetFontColor("muted");
                    cmbPartitionLayout.Font = theme.GetFont("muted");
                    cmbPartitionLayout.ForeColor = theme.GetFontColor("muted");
                    lblPartitionTable.Font = theme.GetFont("muted");
                    lblPartitionTable.ForeColor = theme.GetFontColor("muted");
                    lblUseRemainingSpace.Font = theme.GetFont("muted");
                    lblUseRemainingSpace.ForeColor = theme.GetFontColor("muted");
                    lblDisableBitLocker.Font = theme.GetFont("muted");
                    lblDisableBitLocker.ForeColor = theme.GetFontColor("muted");
                }
                else
                {
                    lblEnableDiskPart.Font = theme.GetFont("normal");
                    lblEnableDiskPart.ForeColor = theme.GetFontColor("normal");

                    lblEnableAutoDiskPart.Font = theme.GetFont("normal");
                    lblEnableAutoDiskPart.ForeColor = theme.GetFontColor("normal");
                    
                    // Check EnableAutoDiskPart for dependent controls
                    bool autoDiskPartEnabled = EnableAutoDiskPart;
                    
                    lblDiskID.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    lblDiskID.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "normal" : "muted");
                    nudDiskID.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    nudDiskID.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "inputForeground" : "muted");
                    lblWipeDisk.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    lblWipeDisk.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "normal" : "muted");
                    lblPartitionLayout.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    lblPartitionLayout.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "normal" : "muted");
                    cmbPartitionLayout.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    cmbPartitionLayout.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "inputForeground" : "muted");
                    lblPartitionTable.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    lblPartitionTable.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "normal" : "muted");
                    lblUseRemainingSpace.Font = theme.GetFont(autoDiskPartEnabled ? "normal" : "muted");
                    lblUseRemainingSpace.ForeColor = theme.GetFontColor(autoDiskPartEnabled ? "normal" : "muted");
                    lblDisableBitLocker.Font = theme.GetFont("normal");
                    lblDisableBitLocker.ForeColor = theme.GetFontColor("normal");
                }

                // Update ComboBox selections from model using helper methods
                // Note: Settings SelectedIndex will trigger SelectedIndexChanged event,
                // which will call validation methods automatically.
                nudDiskID.Value = DiskID;
                cmbPartitionLayout.SelectedIndex = GetIndexFromPartitionLayoutValue(PartitionLayout);

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
                // EnableDiskPart should never be loaded from JSON - always stays true (safety feature)
                // if (json.EnableDiskPart != null) EnableDiskPart = (bool)json.EnableDiskPart;

                if (json.EnableAutoDiskPart != null) EnableAutoDiskPart = (bool)json.EnableAutoDiskPart;
                if (json.DiskID != null) DiskID = (int)json.DiskID;
                if (json.WipeDisk != null) WipeDisk = (bool)json.WipeDisk;
                if (json.PartitionLayout != null) PartitionLayout = (string)json.PartitionLayout;
                //if (json.PartitionTable != null)
                if (json.UseRemainingSpace != null) UseRemainingSpace = (bool)json.UseRemainingSpace;
                if (json.DisableBitLocker != null) DisableBitLocker = (bool)json.DisableBitLocker;
            }
            catch { }
        }

        #endregion

        #region Value Mapping Helpers

        /// <summary>
        /// Maps the dropdown index to the PartitionLayout value (GPT/MBR)
        /// </summary>
        private string GetPartitionLayoutValueFromIndex(int index)
        {
            switch (index)
            {
                case 0: return ""; // "Select one" - no layout selected
                case 1: return "GPT";
                case 2: return "MBR";
                default: return "";
            }
        }

        /// <summary>
        /// Maps the PartitionLayout string value to the dropdown index
        /// </summary>
        private int GetIndexFromPartitionLayoutValue(string layout)
        {
            if (string.IsNullOrEmpty(layout))
                return 0; // "Select one"

            switch (layout.ToUpper())
            {
                case "GPT": return 1;
                case "MBR": return 2;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns diskPart configuration values as a dictionary for XML generation
        /// </summary>
        public Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            // Return diskPart configuration values as strings for XML generation
            values["DiskID"] = DiskID.ToString();
            values["WipeDisk"] = WipeDisk ? "true" : "false";
            values["DisableBitLocker"] = DisableBitLocker ? "1" : "0";

            return values;
        }

        #endregion

        #region UI Validation Methods

        /// <summary>
        /// Validates all ComboBoxes and NumericUpDowns and updates status rings
        /// </summary>
        public void ValidateAllUIFields()
        {
            ValidateDiskID();
            ValidatePartitionLayout();
        }

        /// <summary>
        /// Validation DiskID NumericUpDown and updates status ring
        /// </summary>
        private void ValidateDiskID()
        {
            if (ringDiskID == null || nudDiskID == null)
                return;

            if (nudDiskID.Value >= 0 && nudDiskID.Value <= 255 && !string.IsNullOrWhiteSpace(nudDiskID.Value.ToString()))
            {
                ringDiskID.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringDiskID.SetStatus(ValidationStatus.Invalid);
            }

            // Update the data model
            //DiskID = (int)nudDiskID.Value;
        }

        /// <summary>
        /// Validates Partition Layout ComboBox and updates status ring
        /// </summary>
        private void ValidatePartitionLayout()
        {
            if (ringPartitionLayout == null || cmbPartitionLayout == null)
                return;

            if (cmbPartitionLayout.SelectedIndex > 0 && !string.IsNullOrWhiteSpace(cmbPartitionLayout.SelectedItem?.ToString()))
            {
                ringPartitionLayout.SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringPartitionLayout.SetStatus(ValidationStatus.Invalid);
            }

            // Update Quick Create button state after validation
            UpdateQuickCreateButtonState();
        }

        /// <summary>
        /// Validates the DiskPart configuration and returns list of validation errors
        /// </summary>
        public List<string> Validate()
        {
            List<string> errors = new List<string>();

            // Only validate if DiskPart config is enabled
            if (!EnableDiskPart)
                return errors;
            
            LangManager lang = LangManager.Instance;
            
            // Validate DiskID
            if (string.IsNullOrWhiteSpace(DiskID.ToString()))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("diskPartConfig.diskID.label")));
            }
            
            // Validate Partition Layout
            if (string.IsNullOrWhiteSpace(PartitionLayout))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("diskPartConfig.partitionLayout.label")));
            }
            
            return errors;
        }

        #endregion
    }
}
