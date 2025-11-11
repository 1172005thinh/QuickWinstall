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
    public class AccountEntry
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
    public class UserAccConfig
    {
        #region Data Model

        public bool EnableAutoUserAcc { get; set; } = true;
        public bool EnableUserAcc { get; set; } = true;
        public int DiskID { get; set; } = 0;
        public bool WipeDisk { get; set; } = true;
        public string PartitionLayout { get; set; } = "";
        public bool UseRemainingSpace { get; set; } = true;
        public int InstallToPartitionID { get; set; } = 0;
        public bool DisableBitLocker { get; set; } = true;
        public List<PartitionEntry> PartitionTable { get; set; } = new List<PartitionEntry>();

        #endregion

        #region UI Components

        private Panel pnlUserAccConfig = null!;
        private Button btnUserAccConfigToggle = null!;
        private Label lblUserAccConfigTitle = null!;
        private Panel pnlUserAccConfigSeparator = null!;
        private Panel pnlUserAccConfigContent = null!;

        // Enable toggle
        private Label lblEnableUserAcc = null!;
        private Panel toggleEnableUserAcc = null!;

        // Enable Auto UserAcc
        private Label lblEnableAutoUserAcc = null!;
        private Panel toggleEnableAutoUserAcc = null!;

        // line separator
        private Panel pnlUserAccConfigAutoUserAccSeparator = null!;

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
        private int partitionRows = 8;

        // Partition Table Header Labels
        private Label lblHeaderID = null!;
        private Label lblHeaderType = null!;
        private Label lblHeaderName = null!;
        private Label lblHeaderSize = null!;
        private Label lblHeaderLetter = null!;
        private Label lblHeaderFormat = null!;
        private Label lblHeaderActive = null!;

        // Partition Table Data Rows
        private List<Label> lblIDs = new List<Label>();
        private List<ComboBox> cmbTypes = new List<ComboBox>();
        private List<TextBox> txtNames = new List<TextBox>();
        private List<NumericUpDown> nudSizes = new List<NumericUpDown>();
        private List<ComboBox> cmbLetters = new List<ComboBox>();
        private List<ComboBox> cmbFormats = new List<ComboBox>();
        private List<Panel> toggleActives = new List<Panel>();
        private List<StatusRing> ringPartitionRows = new List<StatusRing>();

        // Use Remaining Space
        private Label lblUseRemainingSpace = null!;
        private Panel toggleUseRemainingSpace = null!;
        
        // Install Windows to partition ID
        private Label lblInstallToPartitionID = null!;
        private NumericUpDown nudInstallToPartitionID = null!;
        private StatusRing ringInstallToPartitionID = null!;

        // Line Separator
        private Panel pnlUserAccConfigAfterAutoUserAccSeparator = null!;

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

            // Clear all lists to prevent disposed control references
            ResetPartitionTableControls();

            // Calculate content height - will be larger once partition table is added
            int contentHeight = ui.GetSectionValue("UserAccConfig", "contentHeight", 910);

            pnlUserAccConfig = new Panel();
            pnlUserAccConfig.Location = new Point(0, 0);
            pnlUserAccConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlUserAccConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Toggle button
            btnUserAccConfigToggle = createRoundedButton();
            btnUserAccConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnUserAccConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnUserAccConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnUserAccConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnUserAccConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnUserAccConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.UserAcc"));

            // Title
            lblUserAccConfigTitle = new Label();
            lblUserAccConfigTitle.Location = new Point(btnUserAccConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblUserAccConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblUserAccConfigTitle.Text = lang.GetString("mainForm.sections.UserAcc");
            lblUserAccConfigTitle.Font = theme.GetFont("subheader");
            lblUserAccConfigTitle.UseMnemonic = false;
            lblUserAccConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblUserAccConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblUserAccConfigTitle.Cursor = Cursors.Hand;
            lblUserAccConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblUserAccConfigTitle, "tooltips.UserAccConfig.header", lang.GetString("tooltips.UserAccConfig.header"));


            int toggleWidth = (int)(ui.GlobalInputWidth * 0.15);

            // Enable Disk & Partition Config
            // Only set EnableUserAcc on first initialization
            if (_isFirstInitialization)
            {
                EnableUserAcc = !SettingsManager.Instance.LockSectionsAtStartup;
                _isFirstInitialization = false;
            }
            toggleEnableUserAcc = theme.CreateToggleSwitch(new Point(ui.GlobalTabX * 2 + ui.GlobalBtnBox, btnUserAccConfigToggle.Bottom + ui.GlobalSpacingY), toggleWidth, ui.GlobalInputHeight, EnableUserAcc);
            toggleEnableUserAcc.TabStop = false;
            toggleEnableUserAcc.Click += (s, e) => OnToggleEnableUserAcc();

            lblEnableUserAcc = new Label();
            lblEnableUserAcc.Location = new Point(toggleEnableUserAcc.Right + ui.GlobalSpacingX, btnUserAccConfigToggle.Bottom + ui.GlobalSpacingY);
            lblEnableUserAcc.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblEnableUserAcc.Text = string.Format(lang.GetString("UserAccConfig.enableUserAcc.label", lang.GetString("mainForm.sections.UserAcc")));
            lblEnableUserAcc.Font = theme.GetFont("normal");
            lblEnableUserAcc.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableUserAcc.Cursor = Cursors.Hand;
            lblEnableUserAcc.Click += (s, e) => OnToggleEnableUserAcc();
            tooltips.SetToolTip(lblEnableUserAcc, "tooltips.UserAccConfig.enableUserAcc");

            // Line Separator
            pnlUserAccConfigSeparator = new Panel();
            pnlUserAccConfigSeparator.Location = new Point(ui.GlobalTabX, toggleEnableUserAcc.Bottom + ui.GlobalSpacingY * 2);
            pnlUserAccConfigSeparator.Size = new Size(pnlUserAccConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlUserAccConfigSeparator.BackColor = theme.GetColor("separator");

            // Content panel
            pnlUserAccConfigContent = new Panel();
            pnlUserAccConfigContent.Location = new Point(0, pnlUserAccConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlUserAccConfigContent.Size = new Size(pnlUserAccConfig.Width, contentHeight);
            pnlUserAccConfigContent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlUserAccConfigContent.AutoScroll = false;

            int labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            int inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            int toggleX = pnlUserAccConfigContent.Right - ui.GlobalTabX - toggleWidth;
            int currentY = ui.GlobalSpacingY;

            // Enable Auto UserAcc
            lblEnableAutoUserAcc = new Label();
            lblEnableAutoUserAcc.Location = new Point(labelX, currentY);
            lblEnableAutoUserAcc.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblEnableAutoUserAcc.Text = lang.GetString("UserAccConfig.enableAutoUserAcc.label");
            lblEnableAutoUserAcc.Font = theme.GetFont("normal");
            lblEnableAutoUserAcc.TextAlign = ContentAlignment.MiddleLeft;
            lblEnableAutoUserAcc.Cursor = Cursors.Hand;
            tooltips.SetToolTip(lblEnableAutoUserAcc, "tooltips.UserAccConfig.enableAutoUserAcc");

            toggleEnableAutoUserAcc = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, true);
            toggleEnableAutoUserAcc.Click += (s, e) => OnToggleEnableAutoUserAcc();

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Line Separator
            pnlUserAccConfigAutoUserAccSeparator = new Panel();
            pnlUserAccConfigAutoUserAccSeparator.Location = new Point(labelX, currentY);
            pnlUserAccConfigAutoUserAccSeparator.Size = new Size(pnlUserAccConfig.Width - labelX - ui.GlobalTabX, 1);
            pnlUserAccConfigAutoUserAccSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            labelX = ui.GlobalTabX * 3 + ui.GlobalBtnBox + ui.GlobalSpacingX;
            inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;
            toggleX = pnlUserAccConfigContent.Width - toggleWidth - ui.GlobalSpacingX;

            // Disk ID
            lblDiskID = new Label();
            lblDiskID.Location = new Point(labelX, currentY);
            lblDiskID.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblDiskID.Text = lang.GetString("UserAccConfig.diskID.label");
            lblDiskID.Font = theme.GetFont("normal");
            lblDiskID.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblDiskID, "tooltips.UserAccConfig.diskID");

            nudDiskID = new NumericUpDown();
            nudDiskID.Location = new Point(inputX, currentY);
            nudDiskID.Size = new Size((int)(ui.GlobalInputWidth * 0.2), ui.GlobalInputHeight);
            nudDiskID.Minimum = 0;
            nudDiskID.Maximum = 255;
            nudDiskID.Value = DiskID;
            nudDiskID.Font = theme.GetFont("normal");
            nudDiskID.BackColor = theme.GetColor("inputBackground");
            nudDiskID.ForeColor = theme.GetFontColor("inputForeground");
            nudDiskID.TextAlign = HorizontalAlignment.Right;
            nudDiskID.ValueChanged += onConfigChanged;
            nudDiskID.ValueChanged += (s, e) =>
            {
                if (!_isLoading)
                {
                    nudDiskID.Focus();
                    // Update property immediately
                    DiskID = (int)nudDiskID.Value;
                }
                ValidateDiskID();
            };
            tooltips.SetToolTip(nudDiskID, "tooltips.UserAccConfig.diskID");

            // Status ring for Disk ID
            int statusRingBorderExtra = 6;
            ringDiskID = new StatusRing();
            ringDiskID.Location = new Point(nudDiskID.Left - ui.GetValue("global.statusRing.borderWidth"), nudDiskID.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringDiskID.Size = new Size(nudDiskID.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), nudDiskID.Height + 2 * ui.GetValue("global.statusRing.borderWidth"));
            ringDiskID.Visible = false;

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Wipe Disk
            lblWipeDisk = new Label();
            lblWipeDisk.Location = new Point(labelX, currentY);
            lblWipeDisk.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblWipeDisk.Text = lang.GetString("UserAccConfig.wipeDisk.label");
            lblWipeDisk.Font = theme.GetFont("subheader");
            lblWipeDisk.ForeColor = theme.GetFontColor("normal");
            lblWipeDisk.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblWipeDisk, "tooltips.UserAccConfig.wipeDisk");

            toggleWipeDisk = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, WipeDisk);
            toggleWipeDisk.TabStop = false;
            toggleWipeDisk.Click += (s, e) => OnToggleWipeDisk();

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Partition Layout
            lblPartitionLayout = new Label();
            lblPartitionLayout.Location = new Point(labelX, currentY);
            lblPartitionLayout.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblPartitionLayout.Text = lang.GetString("UserAccConfig.partitionLayout.label");
            lblPartitionLayout.Font = theme.GetFont("normal");
            lblPartitionLayout.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblPartitionLayout, "tooltips.UserAccConfig.partitionLayout");

            cmbPartitionLayout = new ComboBox();
            cmbPartitionLayout.Location = new Point(inputX, currentY);
            cmbPartitionLayout.Size = new Size(ui.GlobalInputWidth, ui.GlobalInputHeight);
            cmbPartitionLayout.FlatStyle = FlatStyle.Flat;
            cmbPartitionLayout.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPartitionLayout.Font = theme.GetFont("normal");
            cmbPartitionLayout.BackColor = theme.GetColor("inputBackground");
            cmbPartitionLayout.ForeColor = theme.GetFontColor("inputForeground");
            cmbPartitionLayout.Items.AddRange(new object[] {
                lang.GetString("UserAccConfig.partitionLayout.options.selectOne"),
                lang.GetString("UserAccConfig.partitionLayout.options.gptUEFI"),
                lang.GetString("UserAccConfig.partitionLayout.options.mbrBIOS")
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
                    cmbPartitionLayout.Focus();
                    // Update property immediately
                    PartitionLayout = GetPartitionLayoutValueFromIndex(cmbPartitionLayout.SelectedIndex);
                }
                ValidatePartitionLayout();
            };
            tooltips.SetToolTip(cmbPartitionLayout, "tooltips.UserAccConfig.partitionLayout");

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Partition Table section (placeholder for now)
            lblPartitionTable = new Label();
            lblPartitionTable.Location = new Point(labelX, currentY);
            lblPartitionTable.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblPartitionTable.Text = lang.GetString("UserAccConfig.partitionTable.title");
            lblPartitionTable.Font = theme.GetFont("normal");
            lblPartitionTable.TextAlign = ContentAlignment.MiddleLeft;

            btnQuickCreate = createRoundedButton();
            btnQuickCreate.Location = new Point(inputX, currentY);
            btnQuickCreate.Size = new Size((int)(ui.GlobalBtnWidth * 1.25), ui.GlobalBtnHeight);
            btnQuickCreate.Font = theme.GetFont("normal");
            btnQuickCreate.ForeColor = theme.GetFontColor("normal");
            btnQuickCreate.TextAlign = ContentAlignment.MiddleCenter;
            btnQuickCreate.Text = lang.GetString("UserAccConfig.partitionTable.quickCreate");
            btnQuickCreate.Enabled = false; // Will be enabled when partition layout is selected
            btnQuickCreate.Click += (s, e) => QuickCreatePartitionTable();
            tooltips.SetToolTip(btnQuickCreate, "tooltips.UserAccConfig.partitionTable.quickCreate");
            // Apply full button theme for proper background color
            theme.ApplyButtonTheme(btnQuickCreate);

            btnReset = createRoundedButton();
            btnReset.Location = new Point(btnQuickCreate.Right + ui.GlobalSpacingX, currentY);
            btnReset.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnReset.Image = iconMgr.GetIconAsImage("reset", theme.IsDarkTheme, ui.GlobalIconSize);
            btnReset.Click += (s, e) => ResetPartitionTable();
            tooltips.SetToolTip(btnReset, "tooltips.UserAccConfig.partitionTable.reset");

            currentY += ui.GlobalBtnHeight + ui.GlobalSpacingY * 2;

            // Partition Table Header
            int headerY = currentY;
            int columnSpacing = ui.GlobalSpacingX / 2;
            
            // Column widths from spec
            int colID = (int)(ui.GlobalLabelWidth * 0.15);
            int colType = (int)(ui.GlobalLabelWidth * 0.6);
            int colName = (int)(ui.GlobalLabelWidth * 0.9);
            int colSize = (int)(ui.GlobalLabelWidth * 0.5);
            int colLetter = (int)(ui.GlobalLabelWidth * 0.3);
            int colFormat = (int)(ui.GlobalLabelWidth * 0.4);
            int colActive = toggleWidth;

            int currentX = labelX;

            // ID Header
            lblHeaderID = new Label();
            lblHeaderID.Location = new Point(currentX, headerY);
            lblHeaderID.Size = new Size(colID, ui.GlobalLabelHeight);
            lblHeaderID.Text = lang.GetString("UserAccConfig.partitionTable.id");
            lblHeaderID.Font = theme.GetFont("subheader");
            lblHeaderID.ForeColor = theme.GetFontColor("normal");
            lblHeaderID.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderID, "tooltips.UserAccConfig.partitionTable.id");
            currentX += colID + columnSpacing;

            // Type Header
            lblHeaderType = new Label();
            lblHeaderType.Location = new Point(currentX, headerY);
            lblHeaderType.Size = new Size(colType, ui.GlobalLabelHeight);
            lblHeaderType.Text = lang.GetString("UserAccConfig.partitionTable.type");
            lblHeaderType.Font = theme.GetFont("subheader");
            lblHeaderType.ForeColor = theme.GetFontColor("normal");
            lblHeaderType.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderType, "tooltips.UserAccConfig.partitionTable.type");
            currentX += colType + columnSpacing;

            // Name Header
            lblHeaderName = new Label();
            lblHeaderName.Location = new Point(currentX, headerY);
            lblHeaderName.Size = new Size(colName, ui.GlobalLabelHeight);
            lblHeaderName.Text = lang.GetString("UserAccConfig.partitionTable.name");
            lblHeaderName.Font = theme.GetFont("subheader");
            lblHeaderName.ForeColor = theme.GetFontColor("normal");
            lblHeaderName.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderName, "tooltips.UserAccConfig.partitionTable.name");
            currentX += colName + columnSpacing;

            // Size Header
            lblHeaderSize = new Label();
            lblHeaderSize.Location = new Point(currentX, headerY);
            lblHeaderSize.Size = new Size(colSize, ui.GlobalLabelHeight);
            lblHeaderSize.Text = lang.GetString("UserAccConfig.partitionTable.size");
            lblHeaderSize.Font = theme.GetFont("subheader");
            lblHeaderSize.ForeColor = theme.GetFontColor("normal");
            lblHeaderSize.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderSize, "tooltips.UserAccConfig.partitionTable.size");
            currentX += colSize + columnSpacing;

            // Letter Header
            lblHeaderLetter = new Label();
            lblHeaderLetter.Location = new Point(currentX, headerY);
            lblHeaderLetter.Size = new Size(colLetter, ui.GlobalLabelHeight);
            lblHeaderLetter.Text = lang.GetString("UserAccConfig.partitionTable.letter");
            lblHeaderLetter.Font = theme.GetFont("subheader");
            lblHeaderLetter.ForeColor = theme.GetFontColor("normal");
            lblHeaderLetter.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderLetter, "tooltips.UserAccConfig.partitionTable.letter");
            currentX += colLetter + columnSpacing;

            // Format Header
            lblHeaderFormat = new Label();
            lblHeaderFormat.Location = new Point(currentX, headerY);
            lblHeaderFormat.Size = new Size(colFormat, ui.GlobalLabelHeight);
            lblHeaderFormat.Text = lang.GetString("UserAccConfig.partitionTable.format");
            lblHeaderFormat.Font = theme.GetFont("subheader");
            lblHeaderFormat.ForeColor = theme.GetFontColor("normal");
            lblHeaderFormat.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderFormat, "tooltips.UserAccConfig.partitionTable.format");
            currentX += colFormat + columnSpacing;

            // Active Header
            lblHeaderActive = new Label();
            lblHeaderActive.Location = new Point(currentX, headerY);
            lblHeaderActive.Size = new Size(colActive, ui.GlobalLabelHeight);
            lblHeaderActive.Text = lang.GetString("UserAccConfig.partitionTable.active");
            lblHeaderActive.Font = theme.GetFont("subheader");
            lblHeaderActive.ForeColor = theme.GetFontColor("normal");
            lblHeaderActive.TextAlign = ContentAlignment.MiddleCenter;
            tooltips.SetToolTip(lblHeaderActive, "tooltips.UserAccConfig.partitionTable.active");

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY;

            // Create partition rows
            for (int i = 0; i < partitionRows; i++)
            {
                currentX = labelX;
                int rowY = currentY;

                // ID Label (auto-numbered)
                Label lblID = new Label();
                lblID.Location = new Point(currentX, rowY);
                lblID.Size = new Size(colID, ui.GlobalLabelHeight);
                lblID.Text = LangManager.Instance.GetString("UserAccConfig.partitionTable.noID");
                lblID.Font = theme.GetFont("normal");
                lblID.ForeColor = theme.GetFontColor("normal");
                lblID.TextAlign = ContentAlignment.MiddleCenter;
                lblIDs.Add(lblID);
                currentX += colID + columnSpacing;

                // Type ComboBox
                ComboBox cmbType = new ComboBox();
                cmbType.Location = new Point(currentX, rowY);
                cmbType.Size = new Size(colType, ui.GlobalInputHeight);
                cmbType.Font = theme.GetFont("normal");
                cmbType.ForeColor = theme.GetFontColor("inputForeground");
                cmbType.BackColor = theme.GetColor("inputBackground");
                cmbType.FlatStyle = FlatStyle.Flat;
                cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbType.Items.AddRange(new object[] {
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.selectOne"),
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.primary"),
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.extended"),
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.logical"),
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.recovery"),
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.efi"),
                    lang.GetString("UserAccConfig.partitionTable.typeOptions.msr")
                });
                cmbType.SelectedIndex = 0;
                int rowIndex = i;
                cmbType.SelectedIndexChanged += (s, e) => OnPartitionRowChanged(rowIndex);
                cmbType.SelectedIndexChanged += onConfigChanged;
                cmbTypes.Add(cmbType);
                currentX += colType + columnSpacing;

                // Name TextBox
                TextBox txtName = new TextBox();
                txtName.Location = new Point(currentX, rowY);
                txtName.Size = new Size(colName, ui.GlobalInputHeight);
                txtName.Font = theme.GetFont("placeholder");
                txtName.ForeColor = theme.GetFontColor("placeholder");
                txtName.BackColor = theme.GetColor("inputBackground");
                txtName.BorderStyle = BorderStyle.Fixed3D;
                txtName.Text = lang.GetString("UserAccConfig.partitionTable.namePlaceholder");
                txtName.Enter += (s, e) =>
                {
                    if (txtName.Text == lang.GetString("UserAccConfig.partitionTable.namePlaceholder"))
                    {
                        txtName.Text = "";
                        txtName.Font = theme.GetFont("normal");
                        txtName.ForeColor = theme.GetFontColor("inputForeground");
                    }
                };
                txtName.Leave += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        txtName.Text = lang.GetString("UserAccConfig.partitionTable.namePlaceholder");
                        txtName.Font = theme.GetFont("placeholder");
                        txtName.ForeColor = theme.GetFontColor("placeholder");
                    }
                    // Validate when losing focus
                    OnPartitionRowChanged(rowIndex);
                };
                txtName.TextChanged += onConfigChanged;
                txtNames.Add(txtName);
                currentX += colName + columnSpacing;

                // Size NumericUpDown
                NumericUpDown nudSize = new NumericUpDown();
                nudSize.Location = new Point(currentX, rowY);
                nudSize.Size = new Size(colSize, ui.GlobalInputHeight);
                nudSize.Font = theme.GetFont("normal");
                nudSize.ForeColor = theme.GetFontColor("inputForeground");
                nudSize.BackColor = theme.GetColor("inputBackground");
                nudSize.Minimum = 0;
                nudSize.Maximum = 1024 * 1024 * 100; // 100 TB
                nudSize.Value = 0;
                nudSize.TextAlign = HorizontalAlignment.Right;
                nudSize.ValueChanged += (s, e) => OnPartitionRowChanged(rowIndex);
                nudSize.ValueChanged += onConfigChanged;
                nudSizes.Add(nudSize);
                currentX += colSize + columnSpacing;

                // Letter ComboBox
                ComboBox cmbLetter = new ComboBox();
                cmbLetter.Location = new Point(currentX, rowY);
                cmbLetter.Size = new Size(colLetter, ui.GlobalInputHeight);
                cmbLetter.Font = theme.GetFont("normal");
                cmbLetter.ForeColor = theme.GetFontColor("inputForeground");
                cmbLetter.BackColor = theme.GetColor("inputBackground");
                cmbLetter.FlatStyle = FlatStyle.Flat;
                cmbLetter.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbLetter.Items.Add(lang.GetString("UserAccConfig.partitionTable.formatOptions.selectOne"));
                // Add drive letters A-Z
                for (char c = 'A'; c <= 'Z'; c++)
                {
                    cmbLetter.Items.Add(c.ToString());
                }
                cmbLetter.SelectedIndex = 0;
                cmbLetter.SelectedIndexChanged += (s, e) => OnPartitionRowChanged(rowIndex);
                cmbLetter.SelectedIndexChanged += onConfigChanged;
                cmbLetters.Add(cmbLetter);
                currentX += colLetter + columnSpacing;

                // Format ComboBox
                ComboBox cmbFormat = new ComboBox();
                cmbFormat.Location = new Point(currentX, rowY);
                cmbFormat.Size = new Size(colFormat, ui.GlobalInputHeight);
                cmbFormat.Font = theme.GetFont("normal");
                cmbFormat.ForeColor = theme.GetFontColor("inputForeground");
                cmbFormat.BackColor = theme.GetColor("inputBackground");
                cmbFormat.FlatStyle = FlatStyle.Flat;
                cmbFormat.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbFormat.Items.AddRange(new object[] {
                    lang.GetString("UserAccConfig.partitionTable.formatOptions.selectOne"),
                    lang.GetString("UserAccConfig.partitionTable.formatOptions.ntfs"),
                    lang.GetString("UserAccConfig.partitionTable.formatOptions.fat32")
                });
                cmbFormat.SelectedIndex = 0;
                cmbFormat.SelectedIndexChanged += (s, e) => OnPartitionRowChanged(rowIndex);
                cmbFormat.SelectedIndexChanged += onConfigChanged;
                cmbFormats.Add(cmbFormat);
                currentX += colFormat + columnSpacing;

                // Active Toggle
                Panel toggleActive = theme.CreateToggleSwitch(new Point(currentX, rowY), colActive, ui.GlobalInputHeight, false);
                toggleActive.TabStop = false;
                int toggleIndex = i;
                toggleActive.Click += (s, e) => OnPartitionActiveToggle(toggleIndex);
                toggleActive.Click += onConfigChanged;
                toggleActives.Add(toggleActive);

                // Status Ring for the entire row (covers Type to Format)
                StatusRing ringRow = new StatusRing();
                int ringStartX = cmbType.Left - ui.GetValue("global.statusRing.borderWidth");
                int ringEndX = cmbFormat.Right + ui.GetValue("global.statusRing.borderWidth");
                ringRow.Location = new Point(ringStartX, rowY - ui.GetValue("global.statusRing.borderWidth"));
                ringRow.Size = new Size(ringEndX - ringStartX, ui.GlobalInputHeight + 2 * ui.GetValue("global.statusRing.borderWidth"));
                ringRow.Visible = false;
                ringPartitionRows.Add(ringRow);

                currentY += ui.GlobalInputHeight + ui.GlobalSpacingY;
            }

            currentY += ui.GlobalSpacingY * 2;

            // Use Remaining Space
            lblUseRemainingSpace = new Label();
            lblUseRemainingSpace.Location = new Point(labelX, currentY);
            lblUseRemainingSpace.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblUseRemainingSpace.Text = lang.GetString("UserAccConfig.useRemainingSpace.label");
            lblUseRemainingSpace.Font = theme.GetFont("subheader");
            lblUseRemainingSpace.ForeColor = theme.GetFontColor("normal");
            lblUseRemainingSpace.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblUseRemainingSpace, "tooltips.UserAccConfig.useRemainingSpace");

            toggleUseRemainingSpace = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, UseRemainingSpace);
            toggleUseRemainingSpace.TabStop = false;
            toggleUseRemainingSpace.Click += (s, e) => OnToggleUseRemainingSpace();

            currentY += ui.GlobalInputHeight + ui.GlobalSpacingY * 2;

            // Install Windows to Partition ID
            lblInstallToPartitionID = new Label();
            lblInstallToPartitionID.Location = new Point(labelX, currentY);
            lblInstallToPartitionID.Size = new Size(ui.GlobalLabelWidth, ui.GlobalLabelHeight);
            lblInstallToPartitionID.Text = lang.GetString("UserAccConfig.installToPartitionID.label");
            lblInstallToPartitionID.Font = theme.GetFont("normal");
            lblInstallToPartitionID.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblInstallToPartitionID, "tooltips.UserAccConfig.installToPartitionID");

            nudInstallToPartitionID = new NumericUpDown();
            nudInstallToPartitionID.Location = new Point(inputX, currentY);
            nudInstallToPartitionID.Size = new Size((int)(ui.GlobalInputWidth * 0.2), ui.GlobalInputHeight);
            nudInstallToPartitionID.Minimum = 0;
            nudInstallToPartitionID.Maximum = partitionRows;
            nudInstallToPartitionID.Value = InstallToPartitionID;
            nudInstallToPartitionID.Font = theme.GetFont("normal");
            nudInstallToPartitionID.BackColor = theme.GetColor("inputBackground");
            nudInstallToPartitionID.ForeColor = theme.GetFontColor("inputForeground");
            nudInstallToPartitionID.TextAlign = HorizontalAlignment.Right;
            nudInstallToPartitionID.ValueChanged += onConfigChanged;
            nudInstallToPartitionID.ValueChanged += (s, e) => ValidateInstallToPartitionID();
            tooltips.SetToolTip(nudInstallToPartitionID, "tooltips.UserAccConfig.installToPartitionID");

            // Status ring for Install To Partition ID
            ringInstallToPartitionID = new StatusRing();
            ringInstallToPartitionID.Location = new Point(nudInstallToPartitionID.Left - ui.GetValue("global.statusRing.borderWidth"), nudInstallToPartitionID.Top - ui.GetValue("global.statusRing.borderWidth"));
            ringInstallToPartitionID.Size = new Size(nudInstallToPartitionID.Width + 2 * ui.GetValue("global.statusRing.borderWidth"), nudInstallToPartitionID.Height + 2 * ui.GetValue("global.statusRing.borderWidth"));
            ringInstallToPartitionID.Visible = false;

            currentY += ui.GlobalLabelHeight + ui.GlobalSpacingY * 2;
            labelX = ui.GlobalTabX * 2 + ui.GlobalBtnBox;
            inputX = labelX + ui.GlobalLabelWidth + ui.GlobalSpacingX;

            // Line Separator
            pnlUserAccConfigAfterAutoUserAccSeparator = new Panel();
            pnlUserAccConfigAfterAutoUserAccSeparator.Location = new Point(labelX, currentY);
            pnlUserAccConfigAfterAutoUserAccSeparator.Size = new Size(pnlUserAccConfigContent.Width - labelX - ui.GlobalTabX, 1);
            pnlUserAccConfigAfterAutoUserAccSeparator.BackColor = theme.GetColor("separator");

            currentY += ui.GlobalSpacingY * 2;

            // Disable BitLocker
            lblDisableBitLocker = new Label();
            lblDisableBitLocker.Location = new Point(labelX, currentY);
            lblDisableBitLocker.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblDisableBitLocker.Text = lang.GetString("UserAccConfig.disableBitLocker.label");
            lblDisableBitLocker.Font = theme.GetFont("normal");
            lblDisableBitLocker.TextAlign = ContentAlignment.MiddleLeft;
            tooltips.SetToolTip(lblDisableBitLocker, "tooltips.UserAccConfig.disableBitLocker");

            toggleDisableBitLocker = theme.CreateToggleSwitch(new Point(toggleX, currentY), toggleWidth, ui.GlobalInputHeight, DisableBitLocker);
            toggleDisableBitLocker.TabStop = false;
            toggleDisableBitLocker.Click += (s, e) => OnToggleDisableBitLocker();

            // Add controls to content panel
            pnlUserAccConfigContent.Controls.Add(lblEnableAutoUserAcc);
            pnlUserAccConfigContent.Controls.Add(toggleEnableAutoUserAcc);
            pnlUserAccConfigContent.Controls.Add(pnlUserAccConfigAutoUserAccSeparator);
            pnlUserAccConfigContent.Controls.Add(lblDiskID);
            pnlUserAccConfigContent.Controls.Add(nudDiskID);
            pnlUserAccConfigContent.Controls.Add(ringDiskID);
            pnlUserAccConfigContent.Controls.Add(lblWipeDisk);
            pnlUserAccConfigContent.Controls.Add(toggleWipeDisk);
            pnlUserAccConfigContent.Controls.Add(lblPartitionLayout);
            pnlUserAccConfigContent.Controls.Add(cmbPartitionLayout);
            pnlUserAccConfigContent.Controls.Add(ringPartitionLayout);
            pnlUserAccConfigContent.Controls.Add(lblPartitionTable);
            pnlUserAccConfigContent.Controls.Add(btnQuickCreate);
            pnlUserAccConfigContent.Controls.Add(btnReset);
            
            // Add partition table headers
            pnlUserAccConfigContent.Controls.Add(lblHeaderID);
            pnlUserAccConfigContent.Controls.Add(lblHeaderType);
            pnlUserAccConfigContent.Controls.Add(lblHeaderName);
            pnlUserAccConfigContent.Controls.Add(lblHeaderSize);
            pnlUserAccConfigContent.Controls.Add(lblHeaderLetter);
            pnlUserAccConfigContent.Controls.Add(lblHeaderFormat);
            pnlUserAccConfigContent.Controls.Add(lblHeaderActive);
            
            // Add partition table rows
            for (int i = 0; i < partitionRows; i++)
            {
                pnlUserAccConfigContent.Controls.Add(lblIDs[i]);
                pnlUserAccConfigContent.Controls.Add(cmbTypes[i]);
                pnlUserAccConfigContent.Controls.Add(txtNames[i]);
                pnlUserAccConfigContent.Controls.Add(nudSizes[i]);
                pnlUserAccConfigContent.Controls.Add(cmbLetters[i]);
                pnlUserAccConfigContent.Controls.Add(cmbFormats[i]);
                pnlUserAccConfigContent.Controls.Add(toggleActives[i]);
                pnlUserAccConfigContent.Controls.Add(ringPartitionRows[i]);
            }
            
            pnlUserAccConfigContent.Controls.Add(lblUseRemainingSpace);
            pnlUserAccConfigContent.Controls.Add(toggleUseRemainingSpace);
            pnlUserAccConfigContent.Controls.Add(lblInstallToPartitionID);
            pnlUserAccConfigContent.Controls.Add(nudInstallToPartitionID);
            pnlUserAccConfigContent.Controls.Add(ringInstallToPartitionID);
            pnlUserAccConfigContent.Controls.Add(pnlUserAccConfigAfterAutoUserAccSeparator);
            pnlUserAccConfigContent.Controls.Add(lblDisableBitLocker);
            pnlUserAccConfigContent.Controls.Add(toggleDisableBitLocker);

            // Add controls to main panel
            pnlUserAccConfig.Controls.Add(btnUserAccConfigToggle);
            pnlUserAccConfig.Controls.Add(lblUserAccConfigTitle);
            pnlUserAccConfig.Controls.Add(toggleEnableUserAcc);
            pnlUserAccConfig.Controls.Add(lblEnableUserAcc);
            pnlUserAccConfig.Controls.Add(pnlUserAccConfigSeparator);
            pnlUserAccConfig.Controls.Add(pnlUserAccConfigContent);

            // Apply current expansion state
            pnlUserAccConfigContent.Visible = _isExpanded;
            pnlUserAccConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlUserAccConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            // Apply muted state if EnableUserAcc is locked at startup
            if (!EnableUserAcc)
            {
                UpdateControlsFromModel();
            }

            return pnlUserAccConfig;
        }

        public void LoadConfigIntoUI()
        {
            try
            {
                Console.WriteLine("UserAccConfig: Load config into UI...");
                UpdateControlsFromModel();
                Console.WriteLine("UserAccConfig: Config loaded into UI successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UserAccConfig: Error loading config into UI: {ex.Message}");
            }
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlUserAccConfigContent.Visible = _isExpanded;
            pnlUserAccConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("UserAccConfig", "contentHeight", 910);
                pnlUserAccConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnUserAccConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.UserAcc"));
            }
            else
            {
                pnlUserAccConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnUserAccConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.UserAcc"));
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;

            string iconName = _isExpanded ? "expand" : "collapse";
            btnUserAccConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnUserAccConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
            theme.UpdateToggleSwitchState(toggleEnableUserAcc, enabled);
            EnableUserAcc = enabled;

            // Update dependent controls muted state
            UpdateControlsFromModel();
        }

        /// <summary>
        /// Handles the toggle for Enable Disk & Partition Config
        /// </summary>
        private void OnToggleEnableUserAcc()
        {
            if (_isLoading) return;

            ThemeManager theme = ThemeManager.Instance;

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleEnableUserAcc);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableUserAcc, newState);

            EnableUserAcc = newState;

            // Restore focus to the toggle switch to prevent scroll jumping
            toggleEnableUserAcc.Focus();

            // Enable or disable all dependent controls
            bool enableControls = newState;

            toggleEnableAutoUserAcc.Enabled = enableControls;
            toggleWipeDisk.Enabled = enableControls;
            nudDiskID.Enabled = enableControls;
            cmbPartitionLayout.Enabled = enableControls;
            btnQuickCreate.Enabled = enableControls && cmbPartitionLayout.SelectedIndex > 0;
            btnReset.Enabled = enableControls;
            toggleUseRemainingSpace.Enabled = enableControls;
            nudInstallToPartitionID.Enabled = enableControls;
            toggleDisableBitLocker.Enabled = enableControls;
            
            // Enable/disable partition table controls (only if EnableAutoUserAcc is also true)
            bool enablePartitionTable = enableControls && EnableAutoUserAcc;
            for (int i = 0; i < partitionRows; i++)
            {
                cmbTypes[i].Enabled = enablePartitionTable;
                txtNames[i].Enabled = enablePartitionTable;
                nudSizes[i].Enabled = enablePartitionTable;
                cmbLetters[i].Enabled = enablePartitionTable;
                cmbFormats[i].Enabled = enablePartitionTable;
                toggleActives[i].Enabled = enablePartitionTable;
            }

            // Update visual appearance based on enabled state
            bool isMuted = !newState;

            //Update all toggle switches to muted or normal state
            theme.UpdateToggleSwitchMutedState(toggleEnableAutoUserAcc, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleWipeDisk, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleUseRemainingSpace, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleDisableBitLocker, isMuted);

            // Update all labels to muted or normal state
            if (isMuted)
            {
                lblEnableUserAcc.Font = theme.GetFont("muted");
                lblEnableUserAcc.ForeColor = theme.GetFontColor("muted");

                lblEnableAutoUserAcc.Font = theme.GetFont("muted");
                lblEnableAutoUserAcc.ForeColor = theme.GetFontColor("muted");
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
                lblInstallToPartitionID.Font = theme.GetFont("muted");
                lblInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                nudInstallToPartitionID.Font = theme.GetFont("muted");
                nudInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                lblUseRemainingSpace.ForeColor = theme.GetFontColor("muted");
                lblDisableBitLocker.Font = theme.GetFont("muted");
                lblDisableBitLocker.ForeColor = theme.GetFontColor("muted");

                // Mute partition table headers
                lblHeaderID.Font = theme.GetFont("muted");
                lblHeaderID.ForeColor = theme.GetFontColor("muted");
                lblHeaderType.Font = theme.GetFont("muted");
                lblHeaderType.ForeColor = theme.GetFontColor("muted");
                lblHeaderName.Font = theme.GetFont("muted");
                lblHeaderName.ForeColor = theme.GetFontColor("muted");
                lblHeaderSize.Font = theme.GetFont("muted");
                lblHeaderSize.ForeColor = theme.GetFontColor("muted");
                lblHeaderLetter.Font = theme.GetFont("muted");
                lblHeaderLetter.ForeColor = theme.GetFontColor("muted");
                lblHeaderFormat.Font = theme.GetFont("muted");
                lblHeaderFormat.ForeColor = theme.GetFontColor("muted");
                lblHeaderActive.Font = theme.GetFont("muted");
                lblHeaderActive.ForeColor = theme.GetFontColor("muted");

                // Mute partition table rows
                for (int i = 0; i < partitionRows; i++)
                {
                    lblIDs[i].Font = theme.GetFont("muted");
                    lblIDs[i].ForeColor = theme.GetFontColor("muted");
                    cmbTypes[i].Font = theme.GetFont("muted");
                    cmbTypes[i].ForeColor = theme.GetFontColor("muted");
                    txtNames[i].Font = theme.GetFont("muted");
                    txtNames[i].ForeColor = theme.GetFontColor("muted");
                    nudSizes[i].Font = theme.GetFont("muted");
                    nudSizes[i].ForeColor = theme.GetFontColor("muted");
                    cmbLetters[i].Font = theme.GetFont("muted");
                    cmbLetters[i].ForeColor = theme.GetFontColor("muted");
                    cmbFormats[i].Font = theme.GetFont("muted");
                    cmbFormats[i].ForeColor = theme.GetFontColor("muted");
                    theme.UpdateToggleSwitchMutedState(toggleActives[i], true);
                }
            }
            else
            {
                lblEnableUserAcc.Font = theme.GetFont("normal");
                lblEnableUserAcc.ForeColor = theme.GetFontColor("normal");

                lblEnableAutoUserAcc.Font = theme.GetFont("normal");
                lblEnableAutoUserAcc.ForeColor = theme.GetFontColor("normal");
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
                lblInstallToPartitionID.Font = theme.GetFont("normal");
                lblInstallToPartitionID.ForeColor = theme.GetFontColor("normal");
                nudInstallToPartitionID.Font = theme.GetFont("normal");
                nudInstallToPartitionID.ForeColor = theme.GetFontColor("inputForeground");
                lblDisableBitLocker.Font = theme.GetFont("normal");
                lblDisableBitLocker.ForeColor = theme.GetFontColor("normal");

                // Update partition table headers based on EnableAutoUserAcc state
                bool autoUserAccEnabled = EnableAutoUserAcc;
                string headerFont = autoUserAccEnabled ? "subheader" : "muted";
                string headerColor = autoUserAccEnabled ? "normal" : "muted";

                lblHeaderID.Font = theme.GetFont(headerFont);
                lblHeaderID.ForeColor = theme.GetFontColor(headerColor);
                lblHeaderType.Font = theme.GetFont(headerFont);
                lblHeaderType.ForeColor = theme.GetFontColor(headerColor);
                lblHeaderName.Font = theme.GetFont(headerFont);
                lblHeaderName.ForeColor = theme.GetFontColor(headerColor);
                lblHeaderSize.Font = theme.GetFont(headerFont);
                lblHeaderSize.ForeColor = theme.GetFontColor(headerColor);
                lblHeaderLetter.Font = theme.GetFont(headerFont);
                lblHeaderLetter.ForeColor = theme.GetFontColor(headerColor);
                lblHeaderFormat.Font = theme.GetFont(headerFont);
                lblHeaderFormat.ForeColor = theme.GetFontColor(headerColor);
                lblHeaderActive.Font = theme.GetFont(headerFont);
                lblHeaderActive.ForeColor = theme.GetFontColor(headerColor);

                // Update partition table rows based on EnableAutoUserAcc state
                string rowFont = autoUserAccEnabled ? "normal" : "muted";
                string rowColor = autoUserAccEnabled ? "normal" : "muted";
                string inputColor = autoUserAccEnabled ? "inputForeground" : "muted";

                for (int i = 0; i < partitionRows; i++)
                {
                    lblIDs[i].Font = theme.GetFont(rowFont);
                    lblIDs[i].ForeColor = theme.GetFontColor(rowColor);
                    cmbTypes[i].Font = theme.GetFont(rowFont);
                    cmbTypes[i].ForeColor = theme.GetFontColor(inputColor);

                    // Handle txtName based on whether it has placeholder text
                    bool isPlaceholder = IsPlaceholderText(txtNames[i]);
                    if (autoUserAccEnabled)
                    {
                        txtNames[i].Font = theme.GetFont(isPlaceholder ? "placeholder" : "normal");
                        txtNames[i].ForeColor = theme.GetFontColor(isPlaceholder ? "placeholder" : "inputForeground");
                    }
                    else
                    {
                        txtNames[i].Font = theme.GetFont("muted");
                        txtNames[i].ForeColor = theme.GetFontColor("muted");
                    }

                    nudSizes[i].Font = theme.GetFont(rowFont);
                    nudSizes[i].ForeColor = theme.GetFontColor(inputColor);
                    cmbLetters[i].Font = theme.GetFont(rowFont);
                    cmbLetters[i].ForeColor = theme.GetFontColor(inputColor);
                    cmbFormats[i].Font = theme.GetFont(rowFont);
                    cmbFormats[i].ForeColor = theme.GetFontColor(inputColor);
                    theme.UpdateToggleSwitchMutedState(toggleActives[i], !autoUserAccEnabled);
                }
            }
            
            // Update toggle active states for all partition rows
            // Pass false to preserve user's manual toggle state (don't reset to ON)
            UpdateAllPartitionRowToggleStates(resetStatesToOn: false);
            
            // Update Quick Create button state
            UpdateQuickCreateButtonState();

            // Notify MainForm to update lock/unlock button
            _onEnableToggle?.Invoke();

            // Restore focus to the toggle switch to prevent scroll jumping
            toggleEnableUserAcc.Focus();
        }

        /// <summary>
        /// Handles the toggle for Enable Auto UserAcc
        /// </summary>
        private void OnToggleEnableAutoUserAcc()
        {
            if (_isLoading || !EnableUserAcc) return;

            ThemeManager theme = ThemeManager.Instance;
            
            // Restore focus to the toggle switch to prevent scroll jumping
            toggleEnableAutoUserAcc.Focus();

            // Toggle the state
            bool currentState = theme.GetToggleSwitchState(toggleEnableAutoUserAcc);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleEnableAutoUserAcc, newState);

            EnableAutoUserAcc = newState;

            // Update dependent controls enabled/disabled state
            nudDiskID.Enabled = newState;
            toggleWipeDisk.Enabled = newState;
            cmbPartitionLayout.Enabled = newState;
            btnReset.Enabled = newState;
            toggleUseRemainingSpace.Enabled = newState;
            nudInstallToPartitionID.Enabled = newState;
            
            // Enable/disable partition table controls
            for (int i = 0; i < partitionRows; i++)
            {
                cmbTypes[i].Enabled = newState;
                txtNames[i].Enabled = newState;
                nudSizes[i].Enabled = newState;
                cmbLetters[i].Enabled = newState;
                cmbFormats[i].Enabled = newState;
                toggleActives[i].Enabled = newState;
            }

            // Update dependent controls muted/normal state
            bool isMuted = !newState;
            theme.UpdateToggleSwitchMutedState(toggleWipeDisk, isMuted);
            theme.UpdateToggleSwitchMutedState(toggleUseRemainingSpace, isMuted);

            // Update label colors to muted/normal based on state
            if (newState)
            {
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
                lblInstallToPartitionID.Font = theme.GetFont("normal");
                lblInstallToPartitionID.ForeColor = theme.GetFontColor("normal");
                nudInstallToPartitionID.Font = theme.GetFont("normal");
                nudInstallToPartitionID.ForeColor = theme.GetFontColor("inputForeground");

                // Update partition table headers to normal
                lblHeaderID.Font = theme.GetFont("subheader");
                lblHeaderID.ForeColor = theme.GetFontColor("normal");
                lblHeaderType.Font = theme.GetFont("subheader");
                lblHeaderType.ForeColor = theme.GetFontColor("normal");
                lblHeaderName.Font = theme.GetFont("subheader");
                lblHeaderName.ForeColor = theme.GetFontColor("normal");
                lblHeaderSize.Font = theme.GetFont("subheader");
                lblHeaderSize.ForeColor = theme.GetFontColor("normal");
                lblHeaderLetter.Font = theme.GetFont("subheader");
                lblHeaderLetter.ForeColor = theme.GetFontColor("normal");
                lblHeaderFormat.Font = theme.GetFont("subheader");
                lblHeaderFormat.ForeColor = theme.GetFontColor("normal");
                lblHeaderActive.Font = theme.GetFont("subheader");
                lblHeaderActive.ForeColor = theme.GetFontColor("normal");

                // Update partition table rows to normal
                for (int i = 0; i < partitionRows; i++)
                {
                    lblIDs[i].Font = theme.GetFont("normal");
                    lblIDs[i].ForeColor = theme.GetFontColor("normal");
                    cmbTypes[i].Font = theme.GetFont("normal");
                    cmbTypes[i].ForeColor = theme.GetFontColor("inputForeground");

                    // Handle txtName based on whether it has placeholder text
                    bool isPlaceholder = IsPlaceholderText(txtNames[i]);
                    txtNames[i].Font = theme.GetFont(isPlaceholder ? "placeholder" : "normal");
                    txtNames[i].ForeColor = theme.GetFontColor(isPlaceholder ? "placeholder" : "inputForeground");

                    nudSizes[i].Font = theme.GetFont("normal");
                    nudSizes[i].ForeColor = theme.GetFontColor("inputForeground");
                    cmbLetters[i].Font = theme.GetFont("normal");
                    cmbLetters[i].ForeColor = theme.GetFontColor("inputForeground");
                    cmbFormats[i].Font = theme.GetFont("normal");
                    cmbFormats[i].ForeColor = theme.GetFontColor("inputForeground");
                    theme.UpdateToggleSwitchMutedState(toggleActives[i], false);
                }
            }
            else
            {
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
                lblInstallToPartitionID.Font = theme.GetFont("muted");
                lblInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                nudInstallToPartitionID.Font = theme.GetFont("muted");
                nudInstallToPartitionID.ForeColor = theme.GetFontColor("muted");

                // Mute partition table headers
                lblHeaderID.Font = theme.GetFont("muted");
                lblHeaderID.ForeColor = theme.GetFontColor("muted");
                lblHeaderType.Font = theme.GetFont("muted");
                lblHeaderType.ForeColor = theme.GetFontColor("muted");
                lblHeaderName.Font = theme.GetFont("muted");
                lblHeaderName.ForeColor = theme.GetFontColor("muted");
                lblHeaderSize.Font = theme.GetFont("muted");
                lblHeaderSize.ForeColor = theme.GetFontColor("muted");
                lblHeaderLetter.Font = theme.GetFont("muted");
                lblHeaderLetter.ForeColor = theme.GetFontColor("muted");
                lblHeaderFormat.Font = theme.GetFont("muted");
                lblHeaderFormat.ForeColor = theme.GetFontColor("muted");
                lblHeaderActive.Font = theme.GetFont("muted");
                lblHeaderActive.ForeColor = theme.GetFontColor("muted");

                // Mute partition table rows
                for (int i = 0; i < partitionRows; i++)
                {
                    lblIDs[i].Font = theme.GetFont("muted");
                    lblIDs[i].ForeColor = theme.GetFontColor("muted");
                    cmbTypes[i].Font = theme.GetFont("muted");
                    cmbTypes[i].ForeColor = theme.GetFontColor("muted");
                    txtNames[i].Font = theme.GetFont("muted");
                    txtNames[i].ForeColor = theme.GetFontColor("muted");
                    nudSizes[i].Font = theme.GetFont("muted");
                    nudSizes[i].ForeColor = theme.GetFontColor("muted");
                    cmbLetters[i].Font = theme.GetFont("muted");
                    cmbLetters[i].ForeColor = theme.GetFontColor("muted");
                    cmbFormats[i].Font = theme.GetFont("muted");
                    cmbFormats[i].ForeColor = theme.GetFontColor("muted");
                    theme.UpdateToggleSwitchMutedState(toggleActives[i], true);
                }
            }
            
            // Update toggle active states for all partition rows
            // Pass false to preserve user's manual toggle state (don't reset to ON)
            UpdateAllPartitionRowToggleStates(resetStatesToOn: false);
            
            // Update Quick Create button state
            UpdateQuickCreateButtonState();

            _onConfigChanged?.Invoke(this, EventArgs.Empty);
            
            // Restore focus to the toggle switch to prevent scroll jumping
            toggleEnableAutoUserAcc.Focus();
        }

        /// <summary>
        /// Checks if a partition name textbox contains placeholder text
        /// </summary>
        private bool IsPlaceholderText(TextBox txtName)
        {
            string placeholderText = LangManager.Instance.GetString("UserAccConfig.partitionTable.namePlaceholder");
            return txtName.Text == placeholderText;
        }

        /// <summary>
        /// Handles the toggle for Wipe Disk
        /// </summary>
        private void OnToggleWipeDisk()
        {
            if (_isLoading || !EnableUserAcc) return;
            ThemeManager theme = ThemeManager.Instance;
            toggleWipeDisk.Focus();
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
            if (_isLoading || !EnableUserAcc) return;
            ThemeManager theme = ThemeManager.Instance;
            toggleUseRemainingSpace.Focus();
            bool currentState = theme.GetToggleSwitchState(toggleUseRemainingSpace);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleUseRemainingSpace, newState);
            UseRemainingSpace = newState;
            
            // Re-validate all partition rows since size validation depends on UseRemainingSpace
            ValidateAllPartitionRows();
            
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the toggle for Disable BitLocker
        /// </summary>
        private void OnToggleDisableBitLocker()
        {
            if (_isLoading || !EnableUserAcc) return;
            ThemeManager theme = ThemeManager.Instance;
            toggleDisableBitLocker.Focus();
            bool currentState = theme.GetToggleSwitchState(toggleDisableBitLocker);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggleDisableBitLocker, newState);
            DisableBitLocker = newState;
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles changes to partition table row inputs
        /// </summary>
        private void OnPartitionRowChanged(int rowIndex)
        {
            if (_isLoading) return;

            // Update row ID numbering based on which rows have data
            UpdatePartitionRowIDs();
            
            // Update toggle active state for this row
            UpdatePartitionRowToggleState(rowIndex);
            
            // Validate this partition row
            ValidatePartitionRow(rowIndex);
            
            // Validate all other rows too (in case of duplicate names/letters)
            for (int i = 0; i < partitionRows; i++)
            {
                if (i != rowIndex && HasPartitionRowData(i))
                {
                    ValidatePartitionRow(i);
                }
            }
            
            // Validate InstallToPartitionID since partition count may have changed
            ValidateInstallToPartitionID();
            
            // TODO: Validate partition row
            // TODO: Update PartitionTable data model
        }

        /// <summary>
        /// Handles the active toggle for a partition row
        /// </summary>
        private void OnPartitionActiveToggle(int rowIndex)
        {
            if (_isLoading) return;
            
            // Only allow toggling if the row has data
            if (!HasPartitionRowData(rowIndex)) return;

            ThemeManager theme = ThemeManager.Instance;
            toggleActives[rowIndex].Focus();
            Panel toggle = toggleActives[rowIndex];
            bool currentState = theme.GetToggleSwitchState(toggle);
            bool newState = !currentState;
            theme.UpdateToggleSwitchState(toggle, newState);
            
            // TODO: Update PartitionTable data model
        }

        /// <summary>
        /// Updates the ID labels for partition rows based on which rows have data
        /// </summary>
        private void UpdatePartitionRowIDs()
        {
            int currentID = 1;

            for (int i = 0; i < partitionRows; i++)
            {
                bool rowHasData = HasPartitionRowData(i);

                if (rowHasData)
                {
                    lblIDs[i].Text = currentID.ToString();
                    currentID++;
                }
                else
                {
                    lblIDs[i].Text = LangManager.Instance.GetString("UserAccConfig.partitionTable.noID");
                }
            }
        }

        /// <summary>
        /// Checks if a partition row has any data entered
        /// </summary>
        private bool HasPartitionRowData(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= partitionRows) return false;
            
            bool hasType = cmbTypes[rowIndex].SelectedIndex > 0;
            bool hasName = !string.IsNullOrWhiteSpace(txtNames[rowIndex].Text) && 
                          txtNames[rowIndex].Text != LangManager.Instance.GetString("UserAccConfig.partitionTable.namePlaceholder");
            bool hasSize = nudSizes[rowIndex].Value > 0;
            bool hasLetter = cmbLetters[rowIndex].SelectedIndex > 0;
            bool hasFormat = cmbFormats[rowIndex].SelectedIndex > 0;
            
            return hasType || hasName || hasSize || hasLetter || hasFormat;
        }

        /// <summary>
        /// Updates the active toggle switch state and appearance for a specific partition row
        /// Enables and sets to true if row has data, disables and sets to false if row is empty
        /// Also applies muted state based on parent enable states (EnableUserAcc, EnableAutoUserAcc)
        /// </summary>
        /// <param name="rowIndex">The row index to update</param>
        /// <param name="resetStateToOn">If true, sets toggle to ON when row has data. If false, preserves current state.</param>
        private void UpdatePartitionRowToggleState(int rowIndex, bool resetStateToOn = true)
        {
            if (rowIndex < 0 || rowIndex >= partitionRows) return;
            if (toggleActives[rowIndex] == null) return;

            ThemeManager theme = ThemeManager.Instance;
            bool rowHasData = HasPartitionRowData(rowIndex);
            bool isUserAccEnabled = EnableUserAcc;
            bool isAutoUserAccEnabled = EnableAutoUserAcc;

            toggleActives[rowIndex].Focus();

            // Determine if the toggle should be enabled
            bool shouldBeEnabled = rowHasData && isUserAccEnabled && isAutoUserAccEnabled;

            // Determine if the toggle should be muted
            bool shouldBeMuted = !isUserAccEnabled || !isAutoUserAccEnabled;

            if (rowHasData)
            {
                // Row has data - enable toggle and optionally set to true
                toggleActives[rowIndex].Enabled = shouldBeEnabled;
                
                if (!_isLoading && resetStateToOn)
                {
                    theme.UpdateToggleSwitchState(toggleActives[rowIndex], true);
                }
                
                // Apply muted state
                theme.UpdateToggleSwitchMutedState(toggleActives[rowIndex], shouldBeMuted);
            }
            else
            {
                // Row is empty - disable toggle and set to false
                toggleActives[rowIndex].Enabled = false;
                
                if (!_isLoading)
                {
                    theme.UpdateToggleSwitchState(toggleActives[rowIndex], false);
                }
                
                // Apply muted state (always muted when disabled)
                theme.UpdateToggleSwitchMutedState(toggleActives[rowIndex], true);
            }
        }

        /// <summary>
        /// Updates all partition row toggle states
        /// </summary>
        /// <param name="resetStatesToOn">If true, sets all toggles with data to ON. If false, preserves current states.</param>
        private void UpdateAllPartitionRowToggleStates(bool resetStatesToOn = true)
        {
            for (int i = 0; i < partitionRows; i++)
            {
                UpdatePartitionRowToggleState(i, resetStatesToOn);
            }
        }

        /// <summary>
        /// Updates the Quick Create button enabled state based on current selections
        /// </summary>
        private void UpdateQuickCreateButtonState()
        {
            if (btnQuickCreate == null || cmbPartitionLayout == null)
                return;

            ThemeManager theme = ThemeManager.Instance;

            // Button should be enabled when EnableAutoUserAcc is true AND a valid partition layout is selected
            bool shouldEnable = EnableUserAcc && EnableAutoUserAcc && !string.IsNullOrEmpty(PartitionLayout);
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
            if (cmbPartitionLayout == null || cmbPartitionLayout.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a partition layout first.", "Quick Create", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isLoading = true;

            try
            {
                // Get the selected partition layout
                string layoutValue = GetPartitionLayoutValueFromIndex(cmbPartitionLayout.SelectedIndex);
                
                // Load empty.json to get default partition templates
                string emptyJsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "config", "empty.json");
                
                if (!System.IO.File.Exists(emptyJsonPath))
                {
                    MessageBox.Show("Configuration file not found: empty.json", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isLoading = false;
                    return;
                }

                string jsonContent = System.IO.File.ReadAllText(emptyJsonPath);
                dynamic? config = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent);
                
                if (config?.UserAccConfig == null)
                {
                    MessageBox.Show("Invalid configuration file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isLoading = false;
                    return;
                }

                // Get the appropriate default partition table based on layout
                dynamic? defaultPartitions = null;
                int installToID = 0;
                
                if (layoutValue == "GPT")
                {
                    defaultPartitions = config.UserAccConfig.defaultGPT;
                    installToID = 3; // Windows partition
                }
                else if (layoutValue == "MBR")
                {
                    defaultPartitions = config.UserAccConfig.defaultMBR;
                    installToID = 2; // Windows partition
                }
                else
                {
                    MessageBox.Show("Invalid partition layout selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isLoading = false;
                    return;
                }

                if (defaultPartitions == null)
                {
                    MessageBox.Show("Default partition template not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _isLoading = false;
                    return;
                }

                // Clear the partition table data model
                PartitionTable.Clear();
                
                // Clear all partition row controls manually (don't call ResetPartitionTable to avoid _isLoading conflicts)
                ThemeManager theme = ThemeManager.Instance;
                LangManager lang = LangManager.Instance;
                
                for (int i = 0; i < partitionRows; i++)
                {
                    cmbTypes[i].SelectedIndex = 0;
                    txtNames[i].Text = lang.GetString("UserAccConfig.partitionTable.namePlaceholder");
                    txtNames[i].Font = theme.GetFont("placeholder");
                    txtNames[i].ForeColor = theme.GetFontColor("placeholder");
                    nudSizes[i].Value = 0;
                    cmbLetters[i].SelectedIndex = 0;
                    cmbFormats[i].SelectedIndex = 0;
                    theme.UpdateToggleSwitchState(toggleActives[i], false);
                    lblIDs[i].Text = lang.GetString("UserAccConfig.partitionTable.noID");
                }

                // Load the default partitions into the PartitionTable data model
                foreach (var partition in defaultPartitions)
                {
                    PartitionEntry entry = new PartitionEntry
                    {
                        ID = (int)partition.id,
                        Type = (string)partition.type ?? "",
                        Name = (string)partition.name ?? "",
                        SizeMB = (int)partition.sizeMB,
                        Letter = (string)partition.letter ?? "",
                        Format = (string)partition.format ?? "",
                        Active = (bool)partition.active
                    };
                    PartitionTable.Add(entry);
                }

                // Load the partition table data into UI
                LoadPartitionTableFromModel();

                // Set UseRemainingSpace to ON
                theme.UpdateToggleSwitchState(toggleUseRemainingSpace, true);
                UseRemainingSpace = true;

                // Set InstallToPartitionID
                nudInstallToPartitionID.Value = installToID;
                InstallToPartitionID = installToID;

                // Update row IDs and validate
                UpdatePartitionRowIDs();
                ValidateAllPartitionRows();
                ValidateInstallToPartitionID();
                
                // Update all partition row toggle states to enable and unmute them
                // This is important if Reset was called before QuickCreate
                UpdateAllPartitionRowToggleStates(resetStatesToOn: true);

                // Trigger config changed event
                _onConfigChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating default partition table: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }


        private void ResetPartitionTable()
        {
            _isLoading = true;

            try
            {
                // Clear partition table data model
                PartitionTable.Clear();

                // Clear all partition row controls
                for (int i = 0; i < partitionRows; i++)
                {
                    cmbTypes[i].SelectedIndex = 0;
                    txtNames[i].Text = LangManager.Instance.GetString("UserAccConfig.partitionTable.namePlaceholder");
                    txtNames[i].Font = ThemeManager.Instance.GetFont("placeholder");
                    txtNames[i].ForeColor = ThemeManager.Instance.GetFontColor("placeholder");
                    nudSizes[i].Value = 0;
                    cmbLetters[i].SelectedIndex = 0;
                    cmbFormats[i].SelectedIndex = 0;
                    ThemeManager.Instance.UpdateToggleSwitchState(toggleActives[i], false);
                    lblIDs[i].Text = LangManager.Instance.GetString("UserAccConfig.partitionTable.noID");
                }
            }
            finally
            {
                _isLoading = false;
            }
            
            _onConfigChanged?.Invoke(this, EventArgs.Empty);
            
            // Update toggle active states for all partition rows (all should be disabled after reset)
            UpdateAllPartitionRowToggleStates();
            
            // Validate InstallToPartitionID since all partitions were cleared
            ValidateInstallToPartitionID();
        }

        #endregion

        #region Data Model Operations

        public void UpdateFromControls()
        {
            if (_isLoading) return;

            // Check if UI is initialized
            if (toggleEnableAutoUserAcc == null ||
                nudDiskID == null ||
                toggleWipeDisk == null ||
                cmbPartitionLayout == null ||
                toggleUseRemainingSpace == null ||
                nudInstallToPartitionID == null ||
                toggleDisableBitLocker == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet.");
                return;
            }

            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;

            // EnableUserAcc should never be saved - it's a safety lock that always resets to true
            // EnableUserAcc = theme.GetToggleSwitchState(toggleEnableUserAcc);

            EnableAutoUserAcc = theme.GetToggleSwitchState(toggleEnableAutoUserAcc);
            DiskID = (int)nudDiskID.Value;
            WipeDisk = theme.GetToggleSwitchState(toggleWipeDisk);
            PartitionLayout = GetPartitionLayoutValueFromIndex(cmbPartitionLayout.SelectedIndex);
            UseRemainingSpace = theme.GetToggleSwitchState(toggleUseRemainingSpace);
            InstallToPartitionID = (int)nudInstallToPartitionID.Value;
            DisableBitLocker = theme.GetToggleSwitchState(toggleDisableBitLocker);

            // Update PartitionTable from UI controls
            PartitionTable.Clear();
            for (int i = 0; i < partitionRows; i++)
            {
                // Only save rows that have data
                if (HasPartitionRowData(i))
                {
                    string name = txtNames[i].Text;
                    // Don't save placeholder text
                    if (IsPlaceholderText(txtNames[i]))
                    {
                        name = "";
                    }

                    // Only save letter if a valid letter is selected (not the default "selectOne" option)
                    string letter = "";
                    if (cmbLetters[i].SelectedIndex > 0)
                    {
                        letter = cmbLetters[i].SelectedItem?.ToString() ?? "";
                    }

                    PartitionEntry entry = new PartitionEntry
                    {
                        ID = int.Parse(lblIDs[i].Text),
                        Type = cmbTypes[i].SelectedItem?.ToString() ?? "",
                        Name = name,
                        SizeMB = (int)nudSizes[i].Value,
                        Letter = letter,
                        Format = cmbFormats[i].SelectedItem?.ToString() ?? "",
                        Active = theme.GetToggleSwitchState(toggleActives[i])
                    };
                    PartitionTable.Add(entry);
                }
            }
        }

        /// <summary>
        /// Resets all PartitionTable controls to prevent disposed references
        /// </summary>
        private void ResetPartitionTableControls()
        {
            cmbTypes.Clear();
            txtNames.Clear();
            nudSizes.Clear();
            cmbLetters.Clear();
            cmbFormats.Clear();
            toggleActives.Clear();
            ringPartitionRows.Clear();
            lblIDs.Clear();
        }

        /// <summary>
        /// Clears all controls to default states
        /// </summary>
        public void ClearControls()
        {
            _isLoading = true;

            try
            {
                ThemeManager theme = ThemeManager.Instance;

                EnableUserAcc = true;

                // Reset all other controls to off state
                theme.UpdateToggleSwitchState(toggleEnableAutoUserAcc, false);
                theme.UpdateToggleSwitchState(toggleWipeDisk, false);
                theme.UpdateToggleSwitchState(toggleUseRemainingSpace, false);
                theme.UpdateToggleSwitchState(toggleDisableBitLocker, false);

                // Since EnableAutoUserAcc will be false, disable dependent controls
                toggleEnableAutoUserAcc.Enabled = true; // This one stays enabled under EnableUserAcc
                nudDiskID.Enabled = false;
                toggleWipeDisk.Enabled = false;
                cmbPartitionLayout.Enabled = false;
                btnQuickCreate.Enabled = false;
                btnReset.Enabled = false;
                toggleUseRemainingSpace.Enabled = false;
                nudInstallToPartitionID.Enabled = false;
                toggleDisableBitLocker.Enabled = true; // This stays enabled under EnableUserAcc

                // Apply normal visual state to EnableAutoUserAcc (not muted since EnableUserAcc is true)
                theme.UpdateToggleSwitchMutedState(toggleEnableAutoUserAcc, false);
                theme.UpdateToggleSwitchMutedState(toggleDisableBitLocker, false);
                
                // Apply muted state to dependent controls (since EnableAutoUserAcc is false)
                theme.UpdateToggleSwitchMutedState(toggleWipeDisk, true);
                theme.UpdateToggleSwitchMutedState(toggleUseRemainingSpace, true);

                // Update all labels
                lblEnableAutoUserAcc.Font = theme.GetFont("normal");
                lblEnableAutoUserAcc.ForeColor = theme.GetFontColor("normal");
                lblDisableBitLocker.Font = theme.GetFont("normal");
                lblDisableBitLocker.ForeColor = theme.GetFontColor("normal");
                
                // Muted labels for dependent controls
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
                lblInstallToPartitionID.Font = theme.GetFont("muted");
                lblInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                nudInstallToPartitionID.Font = theme.GetFont("muted");
                nudInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                
                // Mute partition table headers
                lblHeaderID.Font = theme.GetFont("muted");
                lblHeaderID.ForeColor = theme.GetFontColor("muted");
                lblHeaderType.Font = theme.GetFont("muted");
                lblHeaderType.ForeColor = theme.GetFontColor("muted");
                lblHeaderName.Font = theme.GetFont("muted");
                lblHeaderName.ForeColor = theme.GetFontColor("muted");
                lblHeaderSize.Font = theme.GetFont("muted");
                lblHeaderSize.ForeColor = theme.GetFontColor("muted");
                lblHeaderLetter.Font = theme.GetFont("muted");
                lblHeaderLetter.ForeColor = theme.GetFontColor("muted");
                lblHeaderFormat.Font = theme.GetFont("muted");
                lblHeaderFormat.ForeColor = theme.GetFontColor("muted");
                lblHeaderActive.Font = theme.GetFont("muted");
                lblHeaderActive.ForeColor = theme.GetFontColor("muted");
                
                // Disable and mute partition table row controls
                bool autoUserAccEnabled = EnableAutoUserAcc;
                for (int i = 0; i < partitionRows; i++)
                {
                    lblIDs[i].Font = theme.GetFont("muted");
                    lblIDs[i].ForeColor = theme.GetFontColor("muted");
                    
                    cmbTypes[i].Enabled = false;
                    cmbTypes[i].Font = theme.GetFont("muted");
                    cmbTypes[i].ForeColor = theme.GetFontColor("muted");
                    
                    // Update Name TextBox with placeholder handling
                    bool isPlaceholder = IsPlaceholderText(txtNames[i]);
                    if (autoUserAccEnabled)
                    {
                        txtNames[i].Font = theme.GetFont(isPlaceholder ? "placeholder" : "normal");
                        txtNames[i].ForeColor = theme.GetFontColor(isPlaceholder ? "placeholder" : "inputForeground");
                    }
                    else
                    {
                        txtNames[i].Font = theme.GetFont("muted");
                        txtNames[i].ForeColor = theme.GetFontColor("muted");
                    }
                    
                    nudSizes[i].Enabled = false;
                    nudSizes[i].Font = theme.GetFont("muted");
                    nudSizes[i].ForeColor = theme.GetFontColor("muted");
                    
                    cmbLetters[i].Enabled = false;
                    cmbLetters[i].Font = theme.GetFont("muted");
                    cmbLetters[i].ForeColor = theme.GetFontColor("muted");
                    
                    cmbFormats[i].Enabled = false;
                    cmbFormats[i].Font = theme.GetFont("muted");
                    cmbFormats[i].ForeColor = theme.GetFontColor("muted");
                    
                    toggleActives[i].Enabled = false;
                    theme.UpdateToggleSwitchMutedState(toggleActives[i], true);
                }

                // Reset ComboBoxes and NumericUpDowns to default selections
                if (nudDiskID != null) nudDiskID.Value = 0;
                if (nudInstallToPartitionID != null) nudInstallToPartitionID.Value = 0;
                if (cmbPartitionLayout != null) cmbPartitionLayout.SelectedIndex = 0;

                // Remove duplicate label updates (already done above)
                
                // Reset data model (except EnableUserAcc which stays true)
                EnableAutoUserAcc = false;
                DiskID = 0;
                WipeDisk = false;
                PartitionLayout = "";
                UseRemainingSpace = false;
                InstallToPartitionID = 0;
                DisableBitLocker = false;
                ResetPartitionTable();
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
            if (toggleEnableAutoUserAcc == null ||
                nudDiskID == null ||
                toggleWipeDisk == null ||
                cmbPartitionLayout == null ||
                toggleUseRemainingSpace == null ||
                nudInstallToPartitionID == null ||
                toggleDisableBitLocker == null)
            {
                Console.WriteLine("UpdateControlsFromModel: UI controls not initialized yet.");
                return;
            }

            Console.WriteLine($"UpdateControlsFromModel: EnableAutoUserAcc={EnableAutoUserAcc}, DiskID={DiskID}, WipeDisk={WipeDisk}, PartitionLayout={PartitionLayout}, UseRemainingSpace={UseRemainingSpace}, InstallToPartitionID={InstallToPartitionID}, DisableBitLocker={DisableBitLocker}");

            try
            {
                ThemeManager theme = ThemeManager.Instance;

                // Update all toggle states
                theme.UpdateToggleSwitchState(toggleEnableUserAcc, EnableUserAcc);
                theme.UpdateToggleSwitchState(toggleEnableAutoUserAcc, EnableAutoUserAcc);
                theme.UpdateToggleSwitchState(toggleWipeDisk, WipeDisk);
                theme.UpdateToggleSwitchState(toggleUseRemainingSpace, UseRemainingSpace);
                theme.UpdateToggleSwitchState(toggleDisableBitLocker, DisableBitLocker);

                // Enable/Disable dependent controls based on EnableUserAcc
                bool enableControls = EnableUserAcc;

                toggleEnableAutoUserAcc.Enabled = enableControls;
                nudDiskID.Enabled = enableControls && EnableAutoUserAcc;
                toggleWipeDisk.Enabled = enableControls && EnableAutoUserAcc;
                cmbPartitionLayout.Enabled = enableControls && EnableAutoUserAcc;
                btnQuickCreate.Enabled = enableControls && EnableAutoUserAcc && !string.IsNullOrEmpty(PartitionLayout);
                btnReset.Enabled = enableControls && EnableAutoUserAcc;
                toggleUseRemainingSpace.Enabled = enableControls && EnableAutoUserAcc;
                nudInstallToPartitionID.Enabled = enableControls && EnableAutoUserAcc;
                toggleDisableBitLocker.Enabled = enableControls;
                
                // Enable/disable partition table controls
                bool enablePartitionTable = enableControls && EnableAutoUserAcc;
                for (int i = 0; i < partitionRows; i++)
                {
                    cmbTypes[i].Enabled = enablePartitionTable;
                    txtNames[i].Enabled = enablePartitionTable;
                    nudSizes[i].Enabled = enablePartitionTable;
                    cmbLetters[i].Enabled = enablePartitionTable;
                    cmbFormats[i].Enabled = enablePartitionTable;
                    toggleActives[i].Enabled = enablePartitionTable;
                }

                // Update visual appearance based on enabled state
                bool isMuted = !EnableUserAcc;

                // Update all toggle switches to muted or normal state
                theme.UpdateToggleSwitchMutedState(toggleEnableAutoUserAcc, isMuted);
                theme.UpdateToggleSwitchMutedState(toggleDisableBitLocker, isMuted);
                
                // WipeDisk and UseRemainingSpace depend on both EnableUserAcc AND EnableAutoUserAcc
                bool mutedForAutoUserAccToggles = isMuted || !EnableAutoUserAcc;
                theme.UpdateToggleSwitchMutedState(toggleWipeDisk, mutedForAutoUserAccToggles);
                theme.UpdateToggleSwitchMutedState(toggleUseRemainingSpace, mutedForAutoUserAccToggles);

                // Update all labels to muted or normal state
                if (isMuted)
                {
                    lblEnableUserAcc.Font = theme.GetFont("muted");
                    lblEnableUserAcc.ForeColor = theme.GetFontColor("muted");

                    lblEnableAutoUserAcc.Font = theme.GetFont("muted");
                    lblEnableAutoUserAcc.ForeColor = theme.GetFontColor("muted");
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
                    lblInstallToPartitionID.Font = theme.GetFont("muted");
                    lblInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                    nudInstallToPartitionID.Font = theme.GetFont("muted");
                    nudInstallToPartitionID.ForeColor = theme.GetFontColor("muted");
                    lblDisableBitLocker.Font = theme.GetFont("muted");
                    lblDisableBitLocker.ForeColor = theme.GetFontColor("muted");

                    // Mute partition table headers
                    lblHeaderID.Font = theme.GetFont("muted");
                    lblHeaderID.ForeColor = theme.GetFontColor("muted");
                    lblHeaderType.Font = theme.GetFont("muted");
                    lblHeaderType.ForeColor = theme.GetFontColor("muted");
                    lblHeaderName.Font = theme.GetFont("muted");
                    lblHeaderName.ForeColor = theme.GetFontColor("muted");
                    lblHeaderSize.Font = theme.GetFont("muted");
                    lblHeaderSize.ForeColor = theme.GetFontColor("muted");
                    lblHeaderLetter.Font = theme.GetFont("muted");
                    lblHeaderLetter.ForeColor = theme.GetFontColor("muted");
                    lblHeaderFormat.Font = theme.GetFont("muted");
                    lblHeaderFormat.ForeColor = theme.GetFontColor("muted");
                    lblHeaderActive.Font = theme.GetFont("muted");
                    lblHeaderActive.ForeColor = theme.GetFontColor("muted");

                    // Mute partition table row controls
                    for (int i = 0; i < partitionRows; i++)
                    {
                        lblIDs[i].Font = theme.GetFont("muted");
                        lblIDs[i].ForeColor = theme.GetFontColor("muted");
                        
                        cmbTypes[i].Font = theme.GetFont("muted");
                        cmbTypes[i].ForeColor = theme.GetFontColor("muted");
                        
                        txtNames[i].Font = theme.GetFont("muted");
                        txtNames[i].ForeColor = theme.GetFontColor("muted");
                        
                        nudSizes[i].Font = theme.GetFont("muted");
                        nudSizes[i].ForeColor = theme.GetFontColor("muted");
                        
                        cmbLetters[i].Font = theme.GetFont("muted");
                        cmbLetters[i].ForeColor = theme.GetFontColor("muted");
                        
                        cmbFormats[i].Font = theme.GetFont("muted");
                        cmbFormats[i].ForeColor = theme.GetFontColor("muted");
                    }
                }
                else
                {
                    lblEnableUserAcc.Font = theme.GetFont("normal");
                    lblEnableUserAcc.ForeColor = theme.GetFontColor("normal");

                    lblEnableAutoUserAcc.Font = theme.GetFont("normal");
                    lblEnableAutoUserAcc.ForeColor = theme.GetFontColor("normal");
                    
                    // Check EnableAutoUserAcc for dependent controls
                    bool autoUserAccEnabled = EnableAutoUserAcc;
                    
                    lblDiskID.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    lblDiskID.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                    nudDiskID.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    nudDiskID.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                    lblWipeDisk.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    lblWipeDisk.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                    lblPartitionLayout.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    lblPartitionLayout.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                    cmbPartitionLayout.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    cmbPartitionLayout.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                    lblPartitionTable.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    lblPartitionTable.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                    lblUseRemainingSpace.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    lblUseRemainingSpace.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                    lblInstallToPartitionID.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    lblInstallToPartitionID.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                    nudInstallToPartitionID.Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                    nudInstallToPartitionID.ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                    lblDisableBitLocker.Font = theme.GetFont("normal");
                    lblDisableBitLocker.ForeColor = theme.GetFontColor("normal");

                    // Update partition table headers
                    string headerFont = autoUserAccEnabled ? "subheader" : "muted";
                    string headerColor = autoUserAccEnabled ? "subheader" : "muted";
                    lblHeaderID.Font = theme.GetFont(headerFont);
                    lblHeaderID.ForeColor = theme.GetFontColor(headerColor);
                    lblHeaderType.Font = theme.GetFont(headerFont);
                    lblHeaderType.ForeColor = theme.GetFontColor(headerColor);
                    lblHeaderName.Font = theme.GetFont(headerFont);
                    lblHeaderName.ForeColor = theme.GetFontColor(headerColor);
                    lblHeaderSize.Font = theme.GetFont(headerFont);
                    lblHeaderSize.ForeColor = theme.GetFontColor(headerColor);
                    lblHeaderLetter.Font = theme.GetFont(headerFont);
                    lblHeaderLetter.ForeColor = theme.GetFontColor(headerColor);
                    lblHeaderFormat.Font = theme.GetFont(headerFont);
                    lblHeaderFormat.ForeColor = theme.GetFontColor(headerColor);
                    lblHeaderActive.Font = theme.GetFont(headerFont);
                    lblHeaderActive.ForeColor = theme.GetFontColor(headerColor);

                    // Update partition table row controls
                    for (int i = 0; i < partitionRows; i++)
                    {
                        // Update ID label
                        lblIDs[i].Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                        lblIDs[i].ForeColor = theme.GetFontColor(autoUserAccEnabled ? "normal" : "muted");
                        
                        // Update Type ComboBox
                        cmbTypes[i].Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                        cmbTypes[i].ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                        
                        // Update Name TextBox with placeholder handling
                        bool isPlaceholder = IsPlaceholderText(txtNames[i]);
                        if (autoUserAccEnabled)
                        {
                            txtNames[i].Font = theme.GetFont(isPlaceholder ? "placeholder" : "normal");
                            txtNames[i].ForeColor = theme.GetFontColor(isPlaceholder ? "placeholder" : "inputForeground");
                        }
                        else
                        {
                            txtNames[i].Font = theme.GetFont("muted");
                            txtNames[i].ForeColor = theme.GetFontColor("muted");
                        }
                        
                        // Update Size NumericUpDown
                        nudSizes[i].Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                        nudSizes[i].ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                        
                        // Update Letter ComboBox
                        cmbLetters[i].Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                        cmbLetters[i].ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                        
                        // Update Format ComboBox
                        cmbFormats[i].Font = theme.GetFont(autoUserAccEnabled ? "normal" : "muted");
                        cmbFormats[i].ForeColor = theme.GetFontColor(autoUserAccEnabled ? "inputForeground" : "muted");
                    }
                }

                // Update ComboBox selections from model using helper methods
                // Note: Settings SelectedIndex will trigger SelectedIndexChanged event,
                // which will call validation methods automatically.
                nudDiskID.Value = DiskID;
                cmbPartitionLayout.SelectedIndex = GetIndexFromPartitionLayoutValue(PartitionLayout);
                nudInstallToPartitionID.Value = InstallToPartitionID;

                // Load partition table data from model
                LoadPartitionTableFromModel();

                // Update toggle active states for all partition rows
                UpdateAllPartitionRowToggleStates();

                // Update Quick Create button state
                UpdateQuickCreateButtonState();

                // Validate after loading
                ValidateAllUIFields();
            }
            finally
            {
                _isLoading = false;
            }
        }

        /// <summary>
        /// Loads partition table data from PartitionTable model into UI controls
        /// </summary>
        private void LoadPartitionTableFromModel()
        {
            if (PartitionTable == null || PartitionTable.Count == 0)
            {
                // No partition data to load
                return;
            }

            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;

            // Load each partition entry into corresponding row
            for (int i = 0; i < Math.Min(PartitionTable.Count, partitionRows); i++)
            {
                PartitionEntry entry = PartitionTable[i];

                // Set Type
                if (!string.IsNullOrEmpty(entry.Type))
                {
                    for (int j = 0; j < cmbTypes[i].Items.Count; j++)
                    {
                        string? itemText = cmbTypes[i].Items[j]?.ToString();
                        if (itemText == entry.Type)
                        {
                            cmbTypes[i].SelectedIndex = j;
                            break;
                        }
                    }
                }

                // Set Name
                if (!string.IsNullOrEmpty(entry.Name))
                {
                    txtNames[i].Text = entry.Name;
                    txtNames[i].Font = theme.GetFont("normal");
                    txtNames[i].ForeColor = theme.GetFontColor("inputForeground");
                }
                else
                {
                    // Set placeholder
                    txtNames[i].Text = lang.GetString("UserAccConfig.partitionTable.namePlaceholder");
                    txtNames[i].Font = theme.GetFont("placeholder");
                    txtNames[i].ForeColor = theme.GetFontColor("placeholder");
                }

                // Set Size
                nudSizes[i].Value = entry.SizeMB;

                // Set Letter
                if (!string.IsNullOrEmpty(entry.Letter))
                {
                    for (int j = 0; j < cmbLetters[i].Items.Count; j++)
                    {
                        string? itemText = cmbLetters[i].Items[j]?.ToString();
                        if (itemText == entry.Letter)
                        {
                            cmbLetters[i].SelectedIndex = j;
                            break;
                        }
                    }
                }

                // Set Format
                if (!string.IsNullOrEmpty(entry.Format))
                {
                    for (int j = 0; j < cmbFormats[i].Items.Count; j++)
                    {
                        string? itemText = cmbFormats[i].Items[j]?.ToString();
                        if (itemText == entry.Format)
                        {
                            cmbFormats[i].SelectedIndex = j;
                            break;
                        }
                    }
                }

                // Set Active toggle
                theme.UpdateToggleSwitchState(toggleActives[i], entry.Active);
            }

            // Update IDs after loading data
            UpdatePartitionRowIDs();
        }

        public void SetValues(dynamic json)
        {
            if (json == null) return;

            try
            {
                // EnableUserAcc should never be loaded from JSON - always stays true (safety feature)
                // if (json.enableUserAcc != null) EnableUserAcc = (bool)json.enableUserAcc;

                if (json.enableAutoUserAcc != null) EnableAutoUserAcc = (bool)json.enableAutoUserAcc;
                if (json.diskID != null) DiskID = (int)json.diskID;
                if (json.wipeDisk != null) WipeDisk = (bool)json.wipeDisk;
                if (json.partitionLayout != null) PartitionLayout = (string)json.partitionLayout;
                if (json.useRemainingSpace != null) UseRemainingSpace = (bool)json.useRemainingSpace;
                if (json.installToPartitionID != null) InstallToPartitionID = (int)json.installToPartitionID;
                if (json.disableBitLocker != null) DisableBitLocker = (bool)json.disableBitLocker;

                // Load partition table
                if (json.partitionTable != null)
                {
                    PartitionTable.Clear();
                    foreach (var partitionJson in json.partitionTable)
                    {
                        PartitionEntry entry = new PartitionEntry
                        {
                            ID = partitionJson.id != null ? (int)partitionJson.id : 0,
                            Type = partitionJson.type != null ? (string)partitionJson.type : "",
                            Name = partitionJson.name != null ? (string)partitionJson.name : "",
                            SizeMB = partitionJson.sizeMB != null ? (int)partitionJson.sizeMB : 0,
                            Letter = partitionJson.letter != null ? (string)partitionJson.letter : "",
                            Format = partitionJson.format != null ? (string)partitionJson.format : "",
                            Active = partitionJson.active != null ? (bool)partitionJson.active : false
                        };
                        PartitionTable.Add(entry);
                    }
                }
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
        /// Returns UserAcc configuration values as a dictionary for XML generation
        /// </summary>
        public Dictionary<string, string> GetValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            // Return UserAcc configuration values as strings for XML generation
            values["DisableBitLocker"] = DisableBitLocker ? "1" : "0";

            // Only set DiskID and InstallToPartitionID if EnableAutoUserAcc is true
            if (EnableAutoUserAcc)
            {
                values["DiskID"] = DiskID.ToString();
                values["InstallToPartitionID"] = InstallToPartitionID.ToString();
                values["InstallTo"] = $@"<InstallTo>
						<DiskID>{DiskID}</DiskID>
						<PartitionID>{InstallToPartitionID}</PartitionID>
					</InstallTo>";
            }
            else
            {
                values["DiskID"] = "";
                values["InstallToPartitionID"] = "";
                values["InstallTo"] = "";
            }

            // Generate PartitionTable XML if EnableAutoUserAcc is true
            if (EnableAutoUserAcc && PartitionTable.Count > 0)
            {
                values["PartitionTable"] = GeneratePartitionTableXML();
            }
            else
            {
                values["PartitionTable"] = "";
            }

            return values;
        }

        /// <summary>
        /// Generates the XML for DiskConfiguration section based on PartitionTable data.
        /// Uses XML elements approach as per Microsoft documentation for better structure and readability.
        /// </summary>
        private string GeneratePartitionTableXML()
        {
            if (PartitionTable == null || PartitionTable.Count == 0)
            {
                return "";
            }

            System.Text.StringBuilder xml = new System.Text.StringBuilder();

            // Start DiskConfiguration
            xml.AppendLine("<DiskConfiguration>");
            xml.AppendLine("\t\t\t\t<Disk wcm:action=\"add\">");
            xml.AppendLine($"\t\t\t\t\t<DiskID>{DiskID}</DiskID>");

            // Add WillWipeDisk if enabled
            if (WipeDisk)
            {
                xml.AppendLine("\t\t\t\t\t<WillWipeDisk>true</WillWipeDisk>");
            }

            // Start CreatePartitions
            xml.AppendLine("\t\t\t\t\t<CreatePartitions>");

            int order = 1;
            foreach (var partition in PartitionTable)
            {
                xml.AppendLine("\t\t\t\t\t\t<CreatePartition wcm:action=\"add\">");
                xml.AppendLine($"\t\t\t\t\t\t\t<Order>{order}</Order>");
                xml.AppendLine($"\t\t\t\t\t\t\t<Type>{partition.Type}</Type>");

                // Add Size only if not using remaining space or not the last partition
                if (!UseRemainingSpace || order < PartitionTable.Count)
                {
                    xml.AppendLine($"\t\t\t\t\t\t\t<Size>{partition.SizeMB}</Size>");
                }
                else
                {
                    // Last partition with UseRemainingSpace enabled - use Extend
                    xml.AppendLine("\t\t\t\t\t\t\t<Extend>true</Extend>");
                }

                xml.AppendLine("\t\t\t\t\t\t</CreatePartition>");
                order++;
            }

            xml.AppendLine("\t\t\t\t\t</CreatePartitions>");
            // Start ModifyPartitions
            xml.AppendLine("\t\t\t\t\t<ModifyPartitions>");

            for (int i = 0; i < PartitionTable.Count; i++)
            {
                var partition = PartitionTable[i];
                xml.AppendLine("\t\t\t\t\t\t<ModifyPartition wcm:action=\"add\">");
                xml.AppendLine($"\t\t\t\t\t\t\t<Order>{i + 1}</Order>");
                xml.AppendLine($"\t\t\t\t\t\t\t<PartitionID>{partition.ID}</PartitionID>");

                // Add Label (Name) if not empty and not MSR partition
                if (!string.IsNullOrEmpty(partition.Name) && partition.Type != "MSR")
                {
                    xml.AppendLine($"\t\t\t\t\t\t\t<Label>{System.Security.SecurityElement.Escape(partition.Name)}</Label>");
                }

                // Add Letter if not empty
                if (!string.IsNullOrEmpty(partition.Letter))
                {
                    xml.AppendLine($"\t\t\t\t\t\t\t<Letter>{partition.Letter}</Letter>");
                }

                // Add Format if not empty and not MSR partition
                if (!string.IsNullOrEmpty(partition.Format) && partition.Type != "MSR")
                {
                    xml.AppendLine($"\t\t\t\t\t\t\t<Format>{partition.Format}</Format>");
                }

                // Add TypeID for Recovery partition (converts Recovery to Primary with WinRE GUID)
                if (partition.Type == "Recovery")
                {
                    xml.AppendLine("\t\t\t\t\t\t\t<TypeID>de94bba4-06d1-4d40-a16a-bfd50179d6ac</TypeID>");
                }

                // Temporarily ignore Active flag (Fix #3)
                // if (partition.Active)
                // {
                //     xml.AppendLine("\t\t\t\t\t\t\t<Active>true</Active>");
                // }

                xml.AppendLine("\t\t\t\t\t\t</ModifyPartition>");
            }

            xml.AppendLine("\t\t\t\t\t</ModifyPartitions>");
            xml.AppendLine("\t\t\t\t</Disk>");
            xml.AppendLine("\t\t\t\t<WillShowUI>OnError</WillShowUI>");
            xml.AppendLine("\t\t\t</DiskConfiguration>");

            return xml.ToString();
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
            ValidateInstallToPartitionID();
            ValidateAllPartitionRows();
        }

        /// <summary>
        /// Validation DiskID NumericUpDown and updates status ring
        /// </summary>
        private void ValidateDiskID()
        {
            if (ringDiskID == null || nudDiskID == null)
                return;

            // NumericUpDown enforces Min (0) and Max (255) automatically
            // So the value is always valid within the range
            // We could show a warning if DiskID is 0 (which might be unintended for some users)
            // But for now, we'll just mark it as valid since it's within the valid range
            ringDiskID.SetStatus(ValidationStatus.Valid);

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
        /// Validates InstallToPartitionID NumericUpDown and updates status ring
        /// </summary>
        private void ValidateInstallToPartitionID()
        {
            if (ringInstallToPartitionID == null || nudInstallToPartitionID == null)
                return;

            // Count the actual number of non-empty partitions
            int actualPartitionCount = 0;
            for (int i = 0; i < partitionRows; i++)
            {
                if (HasPartitionRowData(i))
                    actualPartitionCount++;
            }

            // Value must be greater than 0 and not exceed the actual partition count
            if (nudInstallToPartitionID.Value == 0 || nudInstallToPartitionID.Value > actualPartitionCount)
            {
                ringInstallToPartitionID.SetStatus(ValidationStatus.Invalid);
                return;
            }
            
            ringInstallToPartitionID.SetStatus(ValidationStatus.Valid);
        }

        /// <summary>
        /// Validates a specific partition table row and updates its status ring
        /// </summary>
        private void ValidatePartitionRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= partitionRows) return;
            if (ringPartitionRows[rowIndex] == null) return;

            // Only validate rows that have data
            if (!HasPartitionRowData(rowIndex))
            {
                ringPartitionRows[rowIndex].Visible = false;
                return;
            }

            bool isValid = true;

            // Validate Type: must have value from list (not "-- Select --")
            if (cmbTypes[rowIndex].SelectedIndex <= 0)
            {
                isValid = false;
            }

            // Validate Name: must have value and be unique
            if (isValid)
            {
                string name = txtNames[rowIndex].Text;
                if (string.IsNullOrWhiteSpace(name) || IsPlaceholderText(txtNames[rowIndex]) && cmbTypes[rowIndex].SelectedItem?.ToString() != "MSR")
                {
                    isValid = false;
                }
                else
                {
                    // Check for duplicates
                    for (int i = 0; i < partitionRows; i++)
                    {
                        if (i != rowIndex && HasPartitionRowData(i))
                        {
                            string otherName = txtNames[i].Text;
                            if (!IsPlaceholderText(txtNames[i]) && 
                                name.Equals(otherName, StringComparison.OrdinalIgnoreCase))
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Validate Size: only check if UseRemainingSpace is On
            if (isValid && UseRemainingSpace)
            {
                // Find the last partition with data
                int lastPartitionIndex = -1;
                for (int i = partitionRows - 1; i >= 0; i--)
                {
                    if (HasPartitionRowData(i))
                    {
                        lastPartitionIndex = i;
                        break;
                    }
                }

                // If this is the last partition, size must be 0
                if (rowIndex == lastPartitionIndex && nudSizes[rowIndex].Value != 0)
                {
                    isValid = false;
                }
            }

            // Validate Letter: if has value, must be unique; empty is acceptable
            if (isValid)
            {
                // Only validate if a letter is selected (not empty/default)
                if (cmbLetters[rowIndex].SelectedIndex > 0)
                {
                    // Check for duplicate letters
                    string letter = cmbLetters[rowIndex].SelectedItem?.ToString() ?? "";
                    for (int i = 0; i < partitionRows; i++)
                    {
                        if (i != rowIndex && HasPartitionRowData(i))
                        {
                            string otherLetter = cmbLetters[i].SelectedItem?.ToString() ?? "";
                            if (cmbLetters[i].SelectedIndex > 0 && letter == otherLetter)
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                }
            }

            // Validate Format: must have value from list (not "-- Select --")
            if (isValid && (cmbFormats[rowIndex].SelectedIndex <= 0 && cmbTypes[rowIndex].SelectedItem?.ToString() != "MSR" || (cmbTypes[rowIndex].SelectedItem?.ToString() == "Recovery" && cmbFormats[rowIndex].SelectedItem?.ToString() != "NTFS")))
            {
                isValid = false;
            }

            // Update status ring
            if (isValid)
            {
                ringPartitionRows[rowIndex].SetStatus(ValidationStatus.Valid);
            }
            else
            {
                ringPartitionRows[rowIndex].SetStatus(ValidationStatus.Invalid);
            }
        }

        /// <summary>
        /// Validates all partition table rows
        /// </summary>
        private void ValidateAllPartitionRows()
        {
            for (int i = 0; i < partitionRows; i++)
            {
                ValidatePartitionRow(i);
            }
        }

        /// <summary>
        /// Validates the UserAcc configuration and returns list of validation errors
        /// </summary>
        public List<string> Validate()
        {
            List<string> errors = new List<string>();

            // Only validate if UserAcc config is enabled
            if (!EnableUserAcc)
                return errors;
            
            // If AutoUserAcc is disabled, user wants manual configuration
            // Skip validation of automatic partition configuration fields
            if (!EnableAutoUserAcc)
                return errors;
            
            LangManager lang = LangManager.Instance;
            
            // Validate DiskID
            if (string.IsNullOrWhiteSpace(DiskID.ToString()))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("UserAccConfig.diskID.label")));
            }

            // Validate Partition Layout
            if (string.IsNullOrWhiteSpace(PartitionLayout))
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("UserAccConfig.partitionLayout.label")));
            }
            
            // Validate InstallToPartitionID
            int actualPartitionCount = 0;
            for (int i = 0; i < partitionRows; i++)
            {
                if (HasPartitionRowData(i))
                    actualPartitionCount++;
            }
            if (InstallToPartitionID == 0 || InstallToPartitionID > actualPartitionCount)
            {
                errors.Add(lang.GetString("validation.required", lang.GetString("UserAccConfig.installToPartitionID.label")));
            }
            
            return errors;
        }

        #endregion
    }
}
