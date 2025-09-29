namespace QuickWinstall
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Main Layout Controls
        private System.Windows.Forms.Panel panelActivity;
        private System.Windows.Forms.Panel panelControl;
        // Removed toolbar panel and expand/collapse buttons
        
        // Settings Explorer (ScrollableContent panel)
        private System.Windows.Forms.Panel scrollableContent;
        
        // Control Area Buttons
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnCancel;
        
        // General Section Controls
        private System.Windows.Forms.GroupBox groupGeneral;
        private System.Windows.Forms.Label lblPCName;
        private System.Windows.Forms.TextBox txtPCName;
        private System.Windows.Forms.Label lblWindowsEdition;
        private System.Windows.Forms.ComboBox cmbWindowsEdition;
        private System.Windows.Forms.Label lblCPUArchitecture;
        private System.Windows.Forms.ComboBox cmbCPUArchitecture;
        private System.Windows.Forms.Label lblProductKey;
        private System.Windows.Forms.TextBox txtProductKey;
        
        // Language and Region Section Controls
        private System.Windows.Forms.GroupBox groupLanguageRegion;
        private System.Windows.Forms.Label lblSystemLocale;
        private System.Windows.Forms.ComboBox cmbSystemLocale;
        private System.Windows.Forms.Label lblUserLocale;
        private System.Windows.Forms.ComboBox cmbUserLocale;
        private System.Windows.Forms.CheckBox chkSameAsSystemLocale;
        private System.Windows.Forms.Label lblUILanguage;
        private System.Windows.Forms.ComboBox cmbUILanguage;
        private System.Windows.Forms.Label lblTimeZone;
        private System.Windows.Forms.ComboBox cmbTimeZone;
        
        // Account Section Controls  
        private System.Windows.Forms.GroupBox groupAccount;
        private System.Windows.Forms.Label[] lblAccountNames;
        private System.Windows.Forms.TextBox[] txtAccountNames;
        private System.Windows.Forms.Label[] lblAccountDisplayNames;
        private System.Windows.Forms.TextBox[] txtAccountDisplayNames;
        private System.Windows.Forms.Label[] lblAccountPasswords;
        private System.Windows.Forms.TextBox[] txtAccountPasswords;
        private System.Windows.Forms.Label[] lblAccountGroups;
        private System.Windows.Forms.ComboBox[] cmbAccountGroups;
        
        // Bypass Windows 11 Check Controls
        private System.Windows.Forms.GroupBox groupBypass;
        private System.Windows.Forms.CheckBox chkBypassAll;
        private System.Windows.Forms.CheckBox chkBypassTPM;
        private System.Windows.Forms.CheckBox chkBypassRAM;
        private System.Windows.Forms.CheckBox chkBypassSecureBoot;
        private System.Windows.Forms.CheckBox chkBypassCPU;
        private System.Windows.Forms.CheckBox chkBypassStorage;
        private System.Windows.Forms.CheckBox chkBypassDisk;
        
        // BitLocker Section Controls
        private System.Windows.Forms.GroupBox groupBitLocker;
        private System.Windows.Forms.CheckBox chkDisableBitLocker;
        
        // Disk Configuration Section (placeholder)
        private System.Windows.Forms.GroupBox groupDiskConfiguration;
        
        private System.Windows.Forms.ToolTip toolTip;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Initialize main layout panels
            this.panelActivity = new System.Windows.Forms.Panel();
            this.panelControl = new System.Windows.Forms.Panel();
            this.scrollableContent = new System.Windows.Forms.Panel();
            
            // Initialize control buttons
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            
            // Initialize group boxes
            this.groupGeneral = new System.Windows.Forms.GroupBox();
            this.groupLanguageRegion = new System.Windows.Forms.GroupBox();
            this.groupAccount = new System.Windows.Forms.GroupBox();
            this.groupBypass = new System.Windows.Forms.GroupBox();
            this.groupBitLocker = new System.Windows.Forms.GroupBox();
            this.groupDiskConfiguration = new System.Windows.Forms.GroupBox();
            
            // Initialize General section controls
            this.lblPCName = new System.Windows.Forms.Label();
            this.txtPCName = new System.Windows.Forms.TextBox();
            this.lblWindowsEdition = new System.Windows.Forms.Label();
            this.cmbWindowsEdition = new System.Windows.Forms.ComboBox();
            this.lblCPUArchitecture = new System.Windows.Forms.Label();
            this.cmbCPUArchitecture = new System.Windows.Forms.ComboBox();
            this.lblProductKey = new System.Windows.Forms.Label();
            this.txtProductKey = new System.Windows.Forms.TextBox();
            
            // Initialize Language and Region section controls
            this.lblSystemLocale = new System.Windows.Forms.Label();
            this.cmbSystemLocale = new System.Windows.Forms.ComboBox();
            this.lblUserLocale = new System.Windows.Forms.Label();
            this.cmbUserLocale = new System.Windows.Forms.ComboBox();
            this.chkSameAsSystemLocale = new System.Windows.Forms.CheckBox();
            this.lblUILanguage = new System.Windows.Forms.Label();
            this.cmbUILanguage = new System.Windows.Forms.ComboBox();
            this.lblTimeZone = new System.Windows.Forms.Label();
            this.cmbTimeZone = new System.Windows.Forms.ComboBox();
            
            // Initialize Account section arrays
            this.lblAccountNames = new System.Windows.Forms.Label[4];
            this.txtAccountNames = new System.Windows.Forms.TextBox[4];
            this.lblAccountDisplayNames = new System.Windows.Forms.Label[4];
            this.txtAccountDisplayNames = new System.Windows.Forms.TextBox[4];
            this.lblAccountPasswords = new System.Windows.Forms.Label[4];
            this.txtAccountPasswords = new System.Windows.Forms.TextBox[4];
            this.lblAccountGroups = new System.Windows.Forms.Label[4];
            this.cmbAccountGroups = new System.Windows.Forms.ComboBox[4];
            
            // Initialize Bypass section controls
            this.chkBypassAll = new System.Windows.Forms.CheckBox();
            this.chkBypassTPM = new System.Windows.Forms.CheckBox();
            this.chkBypassRAM = new System.Windows.Forms.CheckBox();
            this.chkBypassSecureBoot = new System.Windows.Forms.CheckBox();
            this.chkBypassCPU = new System.Windows.Forms.CheckBox();
            this.chkBypassStorage = new System.Windows.Forms.CheckBox();
            this.chkBypassDisk = new System.Windows.Forms.CheckBox();
            
            // Initialize BitLocker controls
            this.chkDisableBitLocker = new System.Windows.Forms.CheckBox();
            
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            
            this.SuspendLayout();
            
            // Setup Main Form
            this.SetupMainForm();
            this.SetupLayout();
            this.SetupControls();
            this.SetupEventHandlers();
            
            this.ResumeLayout(false);
        }

        private void SetupMainForm()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.MaximumSize = new System.Drawing.Size(1200, 1080);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Autounattend Generator";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            
            // Handle form closing
            this.FormClosing += MainForm_FormClosing;
        }

        private void SetupLayout()
        {
            const int padding = 10;
            const int controlHeight = 70; // Reduced height for better proportion
            
            // Setup Control Panel (bottom)
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl.Height = controlHeight;
            this.panelControl.BackColor = System.Drawing.Color.White;
            // Remove padding to avoid conflicts with button positioning
            
            // Setup Activity Panel (fill remaining space)
            this.panelActivity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelActivity.BackColor = System.Drawing.Color.White;
            this.panelActivity.Padding = new System.Windows.Forms.Padding(padding);
            
            // Setup Scrollable Content Panel
            this.scrollableContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollableContent.AutoScroll = true;
            this.scrollableContent.BackColor = System.Drawing.Color.White;
            
            // Add panels to form
            this.Controls.Add(this.panelActivity);
            this.Controls.Add(this.panelControl);
            
            // Add child panels
            this.panelActivity.Controls.Add(this.scrollableContent);
        }

        private void SetupControls()
        {
            SetupControlButtons();
            SetupGeneralSection();
            SetupLanguageRegionSection();
            SetupAccountSection();
            SetupBypassSection();
            SetupBitLockerSection();
            SetupDiskConfigurationSection();
        }

        private void SetupControlButtons()
        {
            const int buttonWidth = 100;
            const int buttonHeight = 35;
            const int padding = 15; // Increased padding for better spacing
            const int spacing = 10;
            
            // Calculate positions - center vertically with proper margins
            int centerY = (70 - buttonHeight) / 2; // Use fixed panel height
            int rightEdge = this.panelControl.Width - padding; // Use Width instead of ClientSize
            
            // Cancel button (rightmost)
            this.btnCancel.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnCancel.Location = new System.Drawing.Point(rightEdge - buttonWidth, centerY);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Generate button
            this.btnGenerate.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnGenerate.Location = new System.Drawing.Point(rightEdge - (buttonWidth * 2) - spacing, centerY);
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.BackColor = System.Drawing.Color.White;
            this.btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerate.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Reset button (leftmost of right group)
            this.btnReset.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnReset.Location = new System.Drawing.Point(padding + buttonWidth + spacing, centerY);
            this.btnReset.Text = "Reset";
            this.btnReset.BackColor = System.Drawing.Color.White;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Settings button (leftmost)
            this.btnSettings.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnSettings.Location = new System.Drawing.Point(padding, centerY);
            this.btnSettings.Text = "Settings";
            this.btnSettings.BackColor = System.Drawing.Color.White;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Set anchoring for responsive layout
            this.btnSettings.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnGenerate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            
            // Add buttons to control panel
            this.panelControl.Controls.Add(this.btnSettings);
            this.panelControl.Controls.Add(this.btnReset);
            this.panelControl.Controls.Add(this.btnGenerate);
            this.panelControl.Controls.Add(this.btnCancel);
        }

        private void SetupGeneralSection()
        {
            const int padding = 10;
            const int labelHeight = 23;
            const int controlHeight = 27;
            const int spacing = 8;
            
            // Group box setup
            this.groupGeneral.Text = "General";
            this.groupGeneral.Location = new System.Drawing.Point(padding, padding);
            this.groupGeneral.Size = new System.Drawing.Size(this.scrollableContent.Width - (padding * 2), 180);
            this.groupGeneral.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            
            int yPos = 25;
            
            // PC Name
            this.lblPCName.Text = "PC Name:";
            this.lblPCName.Location = new System.Drawing.Point(padding, yPos);
            this.lblPCName.Size = new System.Drawing.Size(120, labelHeight);
            
            this.txtPCName.Location = new System.Drawing.Point(140, yPos - 2);
            this.txtPCName.Size = new System.Drawing.Size(200, controlHeight);
            this.txtPCName.Text = "PC";
            this.txtPCName.MaxLength = 15;
            this.toolTip.SetToolTip(this.txtPCName, "My computer name");
            
            yPos += controlHeight + spacing;
            
            // Windows Edition
            this.lblWindowsEdition.Text = "Windows Edition:";
            this.lblWindowsEdition.Location = new System.Drawing.Point(padding, yPos);
            this.lblWindowsEdition.Size = new System.Drawing.Size(120, labelHeight);
            
            this.cmbWindowsEdition.Location = new System.Drawing.Point(140, yPos - 2);
            this.cmbWindowsEdition.Size = new System.Drawing.Size(200, controlHeight);
            this.cmbWindowsEdition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWindowsEdition.Items.AddRange(new string[] {
                "Windows 11 Home", "Windows 11 Pro", "Windows 11 Enterprise", "Windows 11 Education"
            });
            this.cmbWindowsEdition.SelectedIndex = 1; // Windows 11 Pro default
            
            yPos += controlHeight + spacing;
            
            // CPU Architecture
            this.lblCPUArchitecture.Text = "CPU Architecture:";
            this.lblCPUArchitecture.Location = new System.Drawing.Point(padding, yPos);
            this.lblCPUArchitecture.Size = new System.Drawing.Size(120, labelHeight);
            
            this.cmbCPUArchitecture.Location = new System.Drawing.Point(140, yPos - 2);
            this.cmbCPUArchitecture.Size = new System.Drawing.Size(200, controlHeight);
            this.cmbCPUArchitecture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCPUArchitecture.Items.AddRange(new string[] {
                "Intel/AMD 32-bit", "Intel/AMD 64-bit", "Windows ARM64"
            });
            this.cmbCPUArchitecture.SelectedIndex = 1; // Intel/AMD 64-bit default
            
            yPos += controlHeight + spacing;
            
            // Product Key
            this.lblProductKey.Text = "Product Key:";
            this.lblProductKey.Location = new System.Drawing.Point(padding, yPos);
            this.lblProductKey.Size = new System.Drawing.Size(120, labelHeight);
            
            this.txtProductKey.Location = new System.Drawing.Point(140, yPos - 2);
            this.txtProductKey.Size = new System.Drawing.Size(300, controlHeight);
            this.txtProductKey.MaxLength = 29;
            this.toolTip.SetToolTip(this.txtProductKey, "Windows 11 activation key");
            
            // Add controls to group
            this.groupGeneral.Controls.Add(this.lblPCName);
            this.groupGeneral.Controls.Add(this.txtPCName);
            this.groupGeneral.Controls.Add(this.lblWindowsEdition);
            this.groupGeneral.Controls.Add(this.cmbWindowsEdition);
            this.groupGeneral.Controls.Add(this.lblCPUArchitecture);
            this.groupGeneral.Controls.Add(this.cmbCPUArchitecture);
            this.groupGeneral.Controls.Add(this.lblProductKey);
            this.groupGeneral.Controls.Add(this.txtProductKey);
            
            this.scrollableContent.Controls.Add(this.groupGeneral);
        }

        private void SetupLanguageRegionSection()
        {
            const int padding = 10;
            const int labelHeight = 23;
            const int controlHeight = 27;
            const int spacing = 8;
            
            // Group box setup
            this.groupLanguageRegion.Text = "Language and Region";
            this.groupLanguageRegion.Location = new System.Drawing.Point(padding, this.groupGeneral.Bottom + padding);
            this.groupLanguageRegion.Size = new System.Drawing.Size(this.scrollableContent.Width - (padding * 2), 150);
            this.groupLanguageRegion.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            
            int yPos = 25;
            
            // System Locale
            this.lblSystemLocale.Text = "System Locale:";
            this.lblSystemLocale.Location = new System.Drawing.Point(padding, yPos);
            this.lblSystemLocale.Size = new System.Drawing.Size(120, labelHeight);
            
            this.cmbSystemLocale.Location = new System.Drawing.Point(140, yPos - 2);
            this.cmbSystemLocale.Size = new System.Drawing.Size(150, controlHeight);
            this.cmbSystemLocale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSystemLocale.Items.AddRange(new string[] { "en-US", "vi-VN" });
            this.cmbSystemLocale.SelectedIndex = 0;
            
            yPos += controlHeight + spacing;
            
            // User Locale
            this.lblUserLocale.Text = "User Locale:";
            this.lblUserLocale.Location = new System.Drawing.Point(padding, yPos);
            this.lblUserLocale.Size = new System.Drawing.Size(120, labelHeight);
            
            this.cmbUserLocale.Location = new System.Drawing.Point(140, yPos - 2);
            this.cmbUserLocale.Size = new System.Drawing.Size(150, controlHeight);
            this.cmbUserLocale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserLocale.Items.AddRange(new string[] { "en-US", "vi-VN" });
            this.cmbUserLocale.SelectedIndex = 0;
            
            this.chkSameAsSystemLocale.Text = "Same as System Locale";
            this.chkSameAsSystemLocale.Location = new System.Drawing.Point(300, yPos);
            this.chkSameAsSystemLocale.Size = new System.Drawing.Size(180, controlHeight);
            this.chkSameAsSystemLocale.Checked = true;
            
            yPos += controlHeight + spacing;
            
            // UI Language
            this.lblUILanguage.Text = "UI Language:";
            this.lblUILanguage.Location = new System.Drawing.Point(padding, yPos);
            this.lblUILanguage.Size = new System.Drawing.Size(120, labelHeight);
            
            this.cmbUILanguage.Location = new System.Drawing.Point(140, yPos - 2);
            this.cmbUILanguage.Size = new System.Drawing.Size(150, controlHeight);
            this.cmbUILanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUILanguage.Items.AddRange(new string[] { "en-US", "vi-VN" });
            this.cmbUILanguage.SelectedIndex = 0;
            
            // Time Zone
            this.lblTimeZone.Text = "Time Zone:";
            this.lblTimeZone.Location = new System.Drawing.Point(300, yPos);
            this.lblTimeZone.Size = new System.Drawing.Size(80, labelHeight);
            
            this.cmbTimeZone.Location = new System.Drawing.Point(390, yPos - 2);
            this.cmbTimeZone.Size = new System.Drawing.Size(200, controlHeight);
            this.cmbTimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimeZone.Items.AddRange(new string[] {
                "Pacific Standard Time", "Mountain Standard Time", "SE Asia Standard Time"
            });
            this.cmbTimeZone.SelectedIndex = 0;
            
            // Add controls to group
            this.groupLanguageRegion.Controls.Add(this.lblSystemLocale);
            this.groupLanguageRegion.Controls.Add(this.cmbSystemLocale);
            this.groupLanguageRegion.Controls.Add(this.lblUserLocale);
            this.groupLanguageRegion.Controls.Add(this.cmbUserLocale);
            this.groupLanguageRegion.Controls.Add(this.chkSameAsSystemLocale);
            this.groupLanguageRegion.Controls.Add(this.lblUILanguage);
            this.groupLanguageRegion.Controls.Add(this.cmbUILanguage);
            this.groupLanguageRegion.Controls.Add(this.lblTimeZone);
            this.groupLanguageRegion.Controls.Add(this.cmbTimeZone);
            
            this.scrollableContent.Controls.Add(this.groupLanguageRegion);
        }

        private void SetupAccountSection()
        {
            const int padding = 10;
            const int controlHeight = 27;
            const int spacing = 8;
            const int columnWidth = 120;
            
            // Group box setup
            this.groupAccount.Text = "Account Manager";
            this.groupAccount.Location = new System.Drawing.Point(padding, this.groupLanguageRegion.Bottom + padding);
            this.groupAccount.Size = new System.Drawing.Size(this.scrollableContent.Width - (padding * 2), 180);
            this.groupAccount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            
            // Header labels
            var headerLabels = new string[] { "Account Name", "Display Name", "Password", "Group" };
            for (int col = 0; col < headerLabels.Length; col++)
            {
                var headerLabel = new System.Windows.Forms.Label();
                headerLabel.Text = headerLabels[col];
                headerLabel.Location = new System.Drawing.Point(padding + (col * (columnWidth + 10)), 25);
                headerLabel.Size = new System.Drawing.Size(columnWidth, 20);
                headerLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                this.groupAccount.Controls.Add(headerLabel);
            }
            
            // Account defaults
            string[] accountDefaults = { "OEM", "", "", "" };
            string[] groupDefaults = { "Administrators", "Users", "Users", "Users" };
            
            // Create 4 rows of account controls
            for (int row = 0; row < 4; row++)
            {
                int yPos = 50 + (row * (controlHeight + spacing));
                
                // Account Name
                this.txtAccountNames[row] = new System.Windows.Forms.TextBox();
                this.txtAccountNames[row].Location = new System.Drawing.Point(padding, yPos);
                this.txtAccountNames[row].Size = new System.Drawing.Size(columnWidth, controlHeight);
                this.txtAccountNames[row].Text = accountDefaults[row];
                this.txtAccountNames[row].MaxLength = 20;
                
                // Display Name
                this.txtAccountDisplayNames[row] = new System.Windows.Forms.TextBox();
                this.txtAccountDisplayNames[row].Location = new System.Drawing.Point(padding + columnWidth + 10, yPos);
                this.txtAccountDisplayNames[row].Size = new System.Drawing.Size(columnWidth, controlHeight);
                this.txtAccountDisplayNames[row].Text = accountDefaults[row];
                this.txtAccountDisplayNames[row].MaxLength = 256;
                
                // Password
                this.txtAccountPasswords[row] = new System.Windows.Forms.TextBox();
                this.txtAccountPasswords[row].Location = new System.Drawing.Point(padding + (columnWidth + 10) * 2, yPos);
                this.txtAccountPasswords[row].Size = new System.Drawing.Size(columnWidth, controlHeight);
                this.txtAccountPasswords[row].UseSystemPasswordChar = false; // Plain text as specified
                
                // Group
                this.cmbAccountGroups[row] = new System.Windows.Forms.ComboBox();
                this.cmbAccountGroups[row].Location = new System.Drawing.Point(padding + (columnWidth + 10) * 3, yPos);
                this.cmbAccountGroups[row].Size = new System.Drawing.Size(columnWidth, controlHeight);
                this.cmbAccountGroups[row].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.cmbAccountGroups[row].Items.AddRange(new string[] { "Administrators", "Users" });
                this.cmbAccountGroups[row].SelectedItem = groupDefaults[row];
                
                // Add controls to group
                this.groupAccount.Controls.Add(this.txtAccountNames[row]);
                this.groupAccount.Controls.Add(this.txtAccountDisplayNames[row]);
                this.groupAccount.Controls.Add(this.txtAccountPasswords[row]);
                this.groupAccount.Controls.Add(this.cmbAccountGroups[row]);
            }
            
            this.scrollableContent.Controls.Add(this.groupAccount);
        }

        private void SetupBypassSection()
        {
            const int padding = 10;
            const int controlHeight = 27;
            const int spacing = 8;
            
            // Group box setup
            this.groupBypass.Text = "Bypass Windows 11 Check";
            this.groupBypass.Location = new System.Drawing.Point(padding, this.groupAccount.Bottom + padding);
            this.groupBypass.Size = new System.Drawing.Size(this.scrollableContent.Width - (padding * 2), 120);
            this.groupBypass.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            
            int yPos = 25;
            
            // Bypass All Check
            this.chkBypassAll.Text = "Bypass All Check";
            this.chkBypassAll.Location = new System.Drawing.Point(padding, yPos);
            this.chkBypassAll.Size = new System.Drawing.Size(150, controlHeight);
            this.chkBypassAll.Checked = true;
            
            yPos += controlHeight + spacing;
            
            // Individual bypass options (2 columns)
            var bypassOptions = new System.Windows.Forms.CheckBox[] {
                this.chkBypassTPM, this.chkBypassRAM, this.chkBypassSecureBoot,
                this.chkBypassCPU, this.chkBypassStorage, this.chkBypassDisk
            };
            
            var bypassTexts = new string[] {
                "Bypass TPM Check", "Bypass RAM Check", "Bypass SecureBoot Check",
                "Bypass CPU Check", "Bypass Storage Check", "Bypass Disk Check"
            };
            
            for (int i = 0; i < bypassOptions.Length; i++)
            {
                int col = i % 3;
                int row = i / 3;
                
                bypassOptions[i].Text = bypassTexts[i];
                bypassOptions[i].Location = new System.Drawing.Point(padding + 30 + (col * 200), yPos + (row * controlHeight));
                bypassOptions[i].Size = new System.Drawing.Size(180, controlHeight);
                bypassOptions[i].Checked = true;
                
                this.groupBypass.Controls.Add(bypassOptions[i]);
            }
            
            this.groupBypass.Controls.Add(this.chkBypassAll);
            this.scrollableContent.Controls.Add(this.groupBypass);
        }

        private void SetupBitLockerSection()
        {
            const int padding = 10;
            const int controlHeight = 27;
            
            // Group box setup
            this.groupBitLocker.Text = "BitLocker";
            this.groupBitLocker.Location = new System.Drawing.Point(padding, this.groupBypass.Bottom + padding);
            this.groupBitLocker.Size = new System.Drawing.Size(this.scrollableContent.Width - (padding * 2), 60);
            this.groupBitLocker.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            
            // Disable BitLocker
            this.chkDisableBitLocker.Text = "Disable BitLocker";
            this.chkDisableBitLocker.Location = new System.Drawing.Point(padding, 25);
            this.chkDisableBitLocker.Size = new System.Drawing.Size(150, controlHeight);
            this.chkDisableBitLocker.Checked = true;
            
            this.groupBitLocker.Controls.Add(this.chkDisableBitLocker);
            this.scrollableContent.Controls.Add(this.groupBitLocker);
        }

        private void SetupDiskConfigurationSection()
        {
            const int padding = 10;
            
            // Group box setup (placeholder)
            this.groupDiskConfiguration.Text = "Disk Configuration";
            this.groupDiskConfiguration.Location = new System.Drawing.Point(padding, this.groupBitLocker.Bottom + padding);
            this.groupDiskConfiguration.Size = new System.Drawing.Size(this.scrollableContent.Width - (padding * 2), 60);
            this.groupDiskConfiguration.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            
            // Placeholder label
            var placeholderLabel = new System.Windows.Forms.Label();
            placeholderLabel.Text = "(To be implemented)";
            placeholderLabel.Location = new System.Drawing.Point(padding, 25);
            placeholderLabel.Size = new System.Drawing.Size(200, 23);
            placeholderLabel.ForeColor = System.Drawing.Color.Gray;
            placeholderLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            
            this.groupDiskConfiguration.Controls.Add(placeholderLabel);
            this.scrollableContent.Controls.Add(this.groupDiskConfiguration);
        }

        private void SetupEventHandlers()
        {
            // Form events
            this.Load += MainForm_Load;
            this.Resize += MainForm_Resize;
            
            // Button events
            this.btnSettings.Click += BtnSettings_Click;
            this.btnReset.Click += BtnReset_Click;
            this.btnGenerate.Click += BtnGenerate_Click;
            this.btnCancel.Click += BtnCancel_Click;
            
            // Field validation events
            this.txtPCName.TextChanged += TxtPCName_TextChanged;
            this.txtProductKey.TextChanged += TxtProductKey_TextChanged;
            
            // Account events
            for (int i = 0; i < 4; i++)
            {
                int index = i; // Capture for closure
                this.txtAccountNames[i].TextChanged += (s, e) => AccountName_TextChanged(s, e, index);
                this.txtAccountDisplayNames[i].TextChanged += (s, e) => AccountDisplayName_TextChanged(s, e, index);
                this.txtAccountPasswords[i].TextChanged += (s, e) => AccountPassword_TextChanged(s, e, index);
            }
            
            // Checkbox events
            this.chkSameAsSystemLocale.CheckedChanged += ChkSameAsSystemLocale_CheckedChanged;
            this.chkBypassAll.CheckedChanged += ChkBypassAll_CheckedChanged;
        }

        #endregion
    }
}