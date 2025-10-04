using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuickWinstall
{
    public partial class MainForm : Form
    {
        private AppSettings _settings;
        private AppSettings _currentSettings;
        private bool _hasUnsavedChanges = false;
        private bool[] _displayNameManuallyEdited = new bool[4]; // Track if display name was manually edited

        public MainForm()
        {
            InitializeComponent();
            InitializeApplication();
            
            // Add RDP stability measures
            SetupRDPStability();
        }

        private void SetupRDPStability()
        {
            // Force layout refresh on resize to handle RDP DPI changes
            this.Resize += (s, e) => {
                this.SuspendLayout();
                this.PerformLayout();
                this.ResumeLayout(true);
            };
            
            // Handle session changes (RDP connect/disconnect)
            Microsoft.Win32.SystemEvents.SessionSwitch += (s, e) => {
                if (e.Reason == Microsoft.Win32.SessionSwitchReason.RemoteConnect ||
                    e.Reason == Microsoft.Win32.SessionSwitchReason.RemoteDisconnect)
                {
                    this.Invalidate(true);
                    this.PerformLayout();
                }
            };
        }

        private void InitializeApplication()
        {
            _settings = AppSettings.Load();
            _currentSettings = _settings.Clone();
            
            // Initialize language manager
            LanguageManager.Instance.LoadLanguage(_settings.UILanguage);
            
            ApplyLanguage(_settings.UILanguage);
            LoadSettings();
            ApplyValidationLogic();
        }

        private void ApplyLanguage(string language)
        {
            // Load language through LanguageManager
            LanguageManager.Instance.LoadLanguage(language);
            LanguageManager.Instance.ApplyCultureToThread(language);
            
            // Update UI text based on selected language
            UpdateUIText();
        }

        private void UpdateUIText()
        {
            // Update main form title
            this.Text = LanguageManager.Instance.GetString("MainForm_Title", "Autounattend Generator");
            
            // Update group box titles
            groupGeneral.Text = LanguageManager.Instance.GetString("MainForm_GeneralTab", "General");
            groupLanguageRegion.Text = LanguageManager.Instance.GetString("MainForm_LanguageRegionTab", "Language and Region");
            groupAccount.Text = LanguageManager.Instance.GetString("MainForm_AccountTab", "Account Manager");
            groupBypass.Text = LanguageManager.Instance.GetString("MainForm_BypassTab", "Bypass Windows 11 Check");
            groupBitLocker.Text = LanguageManager.Instance.GetString("MainForm_BitLockerTab", "BitLocker");
            groupDiskConfiguration.Text = LanguageManager.Instance.GetString("MainForm_DiskConfigTab", "Disk Configuration");
            
            // Update buttons
            btnSettings.Text = LanguageManager.Instance.GetString("MainForm_SettingsButton", "Settings");
            btnReset.Text = LanguageManager.Instance.GetString("MainForm_ResetButton", "Reset");
            btnGenerate.Text = LanguageManager.Instance.GetString("MainForm_GenerateButton", "Generate");
            btnCancel.Text = LanguageManager.Instance.GetString("MainForm_CancelButton", "Cancel");
            
            // Update labels
            lblPCName.Text = LanguageManager.Instance.GetString("MainForm_PCName", "PC Name:");
            lblWindowsEdition.Text = LanguageManager.Instance.GetString("MainForm_WindowsEdition", "Windows Edition:");
            lblCPUArchitecture.Text = LanguageManager.Instance.GetString("MainForm_CPUArchitecture", "CPU Architecture:");
            lblProductKey.Text = LanguageManager.Instance.GetString("MainForm_ProductKey", "Product Key:");
            
            lblSystemLocale.Text = LanguageManager.Instance.GetString("MainForm_SystemLocale", "System Locale:");
            lblUserLocale.Text = LanguageManager.Instance.GetString("MainForm_UserLocale", "User Locale:");
            chkSameAsSystemLocale.Text = LanguageManager.Instance.GetString("MainForm_SameAsSystemLocale", "Same as System Locale");
            lblUILanguage.Text = LanguageManager.Instance.GetString("MainForm_UILanguage", "UI Language:");
            lblTimeZone.Text = LanguageManager.Instance.GetString("MainForm_TimeZone", "Time Zone:");
            
            // Update bypass checkboxes
            chkBypassAll.Text = LanguageManager.Instance.GetString("MainForm_BypassAllChecks", "Bypass All Check");
            chkBypassTPM.Text = LanguageManager.Instance.GetString("MainForm_BypassTPM", "Bypass TPM Check");
            chkBypassRAM.Text = LanguageManager.Instance.GetString("MainForm_BypassRAM", "Bypass RAM Check");
            chkBypassSecureBoot.Text = LanguageManager.Instance.GetString("MainForm_BypassSecureBoot", "Bypass SecureBoot Check");
            chkBypassCPU.Text = LanguageManager.Instance.GetString("MainForm_BypassCPU", "Bypass CPU Check");
            chkBypassStorage.Text = LanguageManager.Instance.GetString("MainForm_BypassStorage", "Bypass Storage Check");
            chkBypassDisk.Text = LanguageManager.Instance.GetString("MainForm_BypassDisk", "Bypass Disk Check");
            
            chkDisableBitLocker.Text = LanguageManager.Instance.GetString("MainForm_DisableBitLocker", "Disable BitLocker");
        }

        private string GetLocalizedString(string key)
        {
            return LanguageManager.Instance.GetString(key);
        }

        private void LoadSettings()
        {
            // General Section
            txtPCName.Text = _currentSettings.PCName;
            cmbWindowsEdition.SelectedItem = _currentSettings.WindowsEdition;
            txtProductKey.Text = _currentSettings.ProductKey;

            // Language and Region Section
            cmbSystemLocale.SelectedItem = _currentSettings.SystemLocale;
            cmbUserLocale.SelectedItem = _currentSettings.UserLocale;
            chkSameAsSystemLocale.Checked = _currentSettings.SameAsSystemLocale;
            cmbUILanguage.SelectedItem = GetLanguageDisplayName(_currentSettings.UILanguage);

            // Account Section
            if (txtAccountNames.Length > 0)
            {
                txtAccountNames[0].Text = _currentSettings.Account1Name;
                txtAccountDisplayNames[0].Text = _currentSettings.Account1DisplayName;
                cmbAccountGroups[0].SelectedItem = _currentSettings.Account1Type;
                
                if (txtAccountNames.Length > 1)
                {
                    txtAccountNames[1].Text = _currentSettings.Account2Name;
                    txtAccountDisplayNames[1].Text = _currentSettings.Account2DisplayName;
                    cmbAccountGroups[1].SelectedItem = _currentSettings.Account2Type;
                }
            }

            // Windows 11 Bypass Section
            chkBypassAll.Checked = _currentSettings.BypassAllChecks;
            chkBypassTPM.Checked = _currentSettings.BypassTPM;
            chkBypassRAM.Checked = _currentSettings.BypassRAM;
            chkBypassSecureBoot.Checked = _currentSettings.BypassSecureBoot;
            chkBypassCPU.Checked = _currentSettings.BypassCPU;
            chkBypassStorage.Checked = _currentSettings.BypassStorage;
            chkBypassDisk.Checked = _currentSettings.BypassDisk;

            _hasUnsavedChanges = false;
        }

        private void ApplyValidationLogic()
        {
            // Set up same as system locale logic
            UpdateUserLocaleState();
            
            // Set up bypass all logic
            UpdateBypassCheckboxes();
            
            // Set up account display name logic
            for (int i = 0; i < txtAccountNames.Length; i++)
            {
                UpdateAccountDisplayName(i);
                UpdateAccountGroupState(i);
            }
        }

        #region Event Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Additional initialization after form load
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Handle resize if needed
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check for validation errors before closing
            var validationErrors = ValidateRequiredFields();
            if (validationErrors.Count > 0)
            {
                var errorMessage = GetLocalizedString("Error_ValidationFailed") ?? "Please fix the following errors before closing:";
                errorMessage += "\n\n" + string.Join("\n", validationErrors);
                
                MessageDialogs.ShowDialog(
                    errorMessage,
                    MessageDialogs.DialogType.Error,
                    MessageDialogs.DialogButtons.OK,
                    GetLocalizedString("Error_Title") ?? "Validation Error");
                
                e.Cancel = true;
                return;
            }

            if (_hasUnsavedChanges)
            {
                var result = MessageDialogs.ShowDialog(
                    GetLocalizedString("Warning_UnsavedChanges") ?? "Save your changes?",
                    MessageDialogs.DialogType.Warning,
                    MessageDialogs.DialogButtons.YesNoCancel,
                    GetLocalizedString("Warning_Title") ?? "Warning");

                switch (result)
                {
                    case DialogResult.Yes:
                        // Save changes and exit
                        SaveCurrentSettings();
                        break;
                    case DialogResult.No:
                        // Exit without saving
                        break;
                    case DialogResult.Cancel:
                        // Cancel exit
                        e.Cancel = true;
                        return;
                }
            }
        }

        // Toolbar Events region removed - all sections are now always expanded

        #region Button Events

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(_settings))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    _settings = settingsForm.Settings;
                    _settings.Save();
                    ApplyLanguage(_settings.UILanguage);
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var result = MessageDialogs.ShowDialog(
                GetLocalizedString("Warning_ConfirmReset") ?? "Confirm default reset?",
                MessageDialogs.DialogType.Warning,
                MessageDialogs.DialogButtons.YesCancel,
                GetLocalizedString("Warning_Title") ?? "Warning");

            if (result == DialogResult.Yes)
            {
                _currentSettings = new AppSettings();
                LoadSettings();
                _hasUnsavedChanges = true;
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            // Validate required fields
            var errors = ValidateRequiredFields();
            if (errors.Count > 0)
            {
                var result = MessageDialogs.ShowDialog(
                    $"{string.Join(Environment.NewLine, errors)}{Environment.NewLine}{Environment.NewLine}" +
                    (GetLocalizedString("Warning_ContinueAnyway") ?? "Continue anyway?"),
                    MessageDialogs.DialogType.Warning,
                    MessageDialogs.DialogButtons.YesCancel,
                    GetLocalizedString("Warning_Title") ?? "Warning");

                if (result == DialogResult.Cancel)
                    return;
            }

            try
            {
                GenerateXML();
                MessageDialogs.ShowDialog(
                    GetLocalizedString("Success_XMLGenerated") ?? "XML file generated successfully!",
                    MessageDialogs.DialogType.Info,
                    MessageDialogs.DialogButtons.OK,
                    GetLocalizedString("Info_Title") ?? "Information");
            }
            catch (Exception ex)
            {
                MessageDialogs.ShowDialog(
                    $"{GetLocalizedString("Error_XMLGenerationFailed") ?? "Failed to generate XML file"}{Environment.NewLine}{ex.Message}",
                    MessageDialogs.DialogType.Error,
                    MessageDialogs.DialogButtons.OK,
                    GetLocalizedString("Error_Title") ?? "Error");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Validation Events

        private void TxtPCName_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            ValidatePCName(textBox);
            _hasUnsavedChanges = true;
        }

        private void TxtProductKey_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            ValidateProductKey(textBox);
            _hasUnsavedChanges = true;
        }

        private void AccountName_TextChanged(object sender, EventArgs e, int index)
        {
            var textBox = sender as TextBox;
            
            // Check if user is trying to clear all accounts
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                int accountsWithNames = 0;
                for (int i = 0; i < txtAccountNames.Length; i++)
                {
                    if (i != index && !string.IsNullOrWhiteSpace(txtAccountNames[i]?.Text))
                    {
                        accountsWithNames++;
                    }
                }
                
                // If this would leave no accounts, prevent clearing for the first account
                if (accountsWithNames == 0 && index == 0)
                {
                    // Restore a default name to prevent having zero accounts
                    textBox.Text = "OEM";
                    textBox.SelectAll(); // Select all text so user can easily type over it
                }
            }
            
            // Reset manual edit tracking if account name is cleared
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                _displayNameManuallyEdited[index] = false;
            }
            
            ValidateAccountName(textBox, index);
            UpdateAccountDisplayName(index);
            UpdateAccountGroupState(index);
            _hasUnsavedChanges = true;
        }

        private void AccountDisplayName_TextChanged(object sender, EventArgs e, int index)
        {
            var textBox = sender as TextBox;
            ValidateAccountDisplayName(textBox);
            _displayNameManuallyEdited[index] = true; // Mark as manually edited
            _hasUnsavedChanges = true;
        }

        private void AccountPassword_TextChanged(object sender, EventArgs e, int index)
        {
            var textBox = sender as TextBox;
            ValidateAccountPassword(textBox);
            _hasUnsavedChanges = true;
        }

        #endregion

        #region Checkbox Events

        private void ChkSameAsSystemLocale_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUserLocaleState();
            _hasUnsavedChanges = true;
        }

        private void ChkBypassAll_CheckedChanged(object sender, EventArgs e)
        {
            UpdateBypassCheckboxes();
            _hasUnsavedChanges = true;
        }



        #endregion

        #endregion

        #region Validation Methods

        private void ValidatePCName(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetFieldError(textBox, GetLocalizedString("ValidationError_PCNameRequired") ?? "PC Name is required");
            }
            else if (textBox.Text.Length > 15)
            {
                SetFieldError(textBox, GetLocalizedString("ValidationError_PCNameTooLong") ?? "PC Name cannot exceed 15 characters");
            }
            else
            {
                ClearFieldError(textBox);
            }
        }

        private void ValidateProductKey(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text.Length > 29)
            {
                SetFieldError(textBox, GetLocalizedString("ValidationError_ProductKeyTooLong") ?? "Product key cannot exceed 29 characters");
            }
            else
            {
                ClearFieldError(textBox);
            }
        }

        private void ValidateAccountName(TextBox textBox, int index)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text) && textBox.Text.Length > 20)
            {
                SetFieldError(textBox, GetLocalizedString("ValidationError_AccountNameTooLong") ?? "Account name cannot exceed 20 characters");
                return;
            }

            // Check if this would be the last account being cleared
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                int accountsWithNames = 0;
                for (int i = 0; i < txtAccountNames.Length; i++)
                {
                    // Skip the current field being cleared
                    if (i != index && !string.IsNullOrWhiteSpace(txtAccountNames[i]?.Text))
                    {
                        accountsWithNames++;
                    }
                }

                // If there would be no accounts left, show warning
                if (accountsWithNames == 0)
                {
                    SetFieldError(textBox, GetLocalizedString("ValidationError_LastAccountCannotBeEmpty") ?? "At least one account must be configured");
                    return;
                }
            }

            ClearFieldError(textBox);
        }

        private void ValidateAccountDisplayName(TextBox textBox)
        {
            if (textBox.Text.Length > 256)
            {
                SetFieldError(textBox, GetLocalizedString("ValidationError_DisplayNameTooLong") ?? "Display name cannot exceed 256 characters");
            }
            else
            {
                ClearFieldError(textBox);
            }
        }

        private void ValidateAccountPassword(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text.Length < 8)
            {
                SetFieldError(textBox, GetLocalizedString("ValidationError_PasswordTooShort") ?? "Password must be at least 8 characters");
            }
            else
            {
                ClearFieldError(textBox);
            }
        }

        private void SetFieldError(Control control, string message)
        {
            control.BackColor = Color.LightPink;
            toolTip.SetToolTip(control, message);
        }

        private void ClearFieldError(Control control)
        {
            control.BackColor = SystemColors.Window;
            toolTip.SetToolTip(control, "");
        }

        private List<string> ValidateRequiredFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txtPCName.Text))
            {
                errors.Add(GetLocalizedString("ValidationError_PCNameRequired") ?? "PC Name is required");
            }

            // Check that at least one account exists
            bool hasAtLeastOneAccount = false;
            for (int i = 0; i < txtAccountNames.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(txtAccountNames[i]?.Text))
                {
                    hasAtLeastOneAccount = true;
                    break;
                }
            }

            if (!hasAtLeastOneAccount)
            {
                errors.Add(GetLocalizedString("ValidationError_AtLeastOneAccountRequired") ?? "At least one user account is required for Windows installation");
            }

            return errors;
        }

        #endregion

        #region Logic Methods

        private void UpdateUserLocaleState()
        {
            if (chkSameAsSystemLocale.Checked)
            {
                cmbUserLocale.SelectedItem = cmbSystemLocale.SelectedItem;
                cmbUserLocale.Enabled = false;
                cmbUserLocale.BackColor = SystemColors.Control;
            }
            else
            {
                cmbUserLocale.Enabled = true;
                cmbUserLocale.BackColor = SystemColors.Window;
            }
        }

        private void UpdateBypassCheckboxes()
        {
            bool bypassAll = chkBypassAll.Checked;
            chkBypassTPM.Checked = bypassAll;
            chkBypassRAM.Checked = bypassAll;
            chkBypassSecureBoot.Checked = bypassAll;
            chkBypassCPU.Checked = bypassAll;
            chkBypassStorage.Checked = bypassAll;
            chkBypassDisk.Checked = bypassAll;
        }

        private void UpdateAccountDisplayName(int index)
        {
            if (index >= 0 && index < txtAccountNames.Length && index < txtAccountDisplayNames.Length)
            {
                var accountName = txtAccountNames[index].Text.Trim();
                var displayName = txtAccountDisplayNames[index].Text.Trim();

                // Only auto-update if display name hasn't been manually edited
                if (!string.IsNullOrEmpty(accountName) && !_displayNameManuallyEdited[index])
                {
                    txtAccountDisplayNames[index].Text = accountName;
                }
            }
        }

        private void UpdateAccountGroupState(int index)
        {
            if (index >= 0 && index < txtAccountNames.Length && index < cmbAccountGroups.Length)
            {
                var accountName = txtAccountNames[index].Text.Trim();
                cmbAccountGroups[index].Enabled = !string.IsNullOrEmpty(accountName);
                
                if (!cmbAccountGroups[index].Enabled)
                {
                    cmbAccountGroups[index].BackColor = SystemColors.Control;
                }
                else
                {
                    cmbAccountGroups[index].BackColor = SystemColors.Window;
                }
            }
        }

        #endregion

        #region XML Generation

        private void GenerateXML()
        {
            // Save current settings first
            SaveCurrentSettings();

            // Load template XML
            string templatePath = Path.Combine(Application.StartupPath ?? "", "template.xml");
            if (!File.Exists(templatePath))
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(), "template.xml");
            }

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Template file not found: template.xml");
            }

            string xmlContent = File.ReadAllText(templatePath);

            // Replace placeholders
            xmlContent = ReplacePlaceholders(xmlContent);

            // Save to output location
            var outputPath = Path.Combine(_settings.DefaultFileLocation, "autounattend.xml");
            File.WriteAllText(outputPath, xmlContent);
        }

        private string MapCPUArchitecture(string displayText)
        {
            return displayText switch
            {
                "Intel/AMD 32-bit" => "x86",
                "Intel/AMD 64-bit" => "amd64", 
                "Windows ARM64" => "arm64",
                _ => "amd64" // Default to amd64 if unknown
            };
        }

        private string ReplacePlaceholders(string xmlContent)
        {
            // First, replace simple variables
            var replacements = new Dictionary<string, string>
            {
                // General Settings
                {"{{PC_NAME}}", txtPCName.Text.Trim()},
                {"{{WINDOWS_EDITION}}", cmbWindowsEdition.SelectedItem?.ToString() ?? ""},
                {"{{CPU_ARCHITECTURE}}", MapCPUArchitecture(cmbCPUArchitecture.SelectedItem?.ToString())},
                {"{{PRODUCT_KEY}}", txtProductKey.Text.Trim()},
                
                // Language and Region Settings
                {"{{SYSTEM_LOCALE}}", cmbSystemLocale.SelectedItem?.ToString() ?? ""},
                {"{{USER_LOCALE}}", cmbUserLocale.SelectedItem?.ToString() ?? ""},
                {"{{UI_LANGUAGE}}", GetLanguageCode(cmbUILanguage.SelectedItem?.ToString() ?? "")},
                {"{{TIME_ZONE}}", cmbTimeZone.SelectedItem?.ToString() ?? ""},
                
                // Windows 11 Bypass Options (1 = enabled, 0 = disabled)
                {"{{BYPASS_TPM}}", chkBypassTPM?.Checked == true ? "1" : "0"},
                {"{{BYPASS_RAM}}", chkBypassRAM?.Checked == true ? "1" : "0"},
                {"{{BYPASS_SECUREBOOT}}", chkBypassSecureBoot?.Checked == true ? "1" : "0"},
                {"{{BYPASS_CPU}}", chkBypassCPU?.Checked == true ? "1" : "0"},
                {"{{BYPASS_STORAGE}}", chkBypassStorage?.Checked == true ? "1" : "0"},
                {"{{BYPASS_DISK}}", chkBypassDisk?.Checked == true ? "1" : "0"},
                
                // BitLocker Settings (1 = disabled, 0 = enabled)
                {"{{BITLOCKER_DISABLED}}", chkDisableBitLocker?.Checked == true ? "1" : "0"}
            };

            // Replace simple variables first
            foreach (var replacement in replacements)
            {
                xmlContent = xmlContent.Replace(replacement.Key, replacement.Value);
            }

            // Handle dynamic account generation
            xmlContent = GenerateAccountsXML(xmlContent);

            return xmlContent;
        }

        private string GenerateAccountsXML(string xmlContent)
        {
            var localAccountsSection = new StringBuilder();
            int accountCount = 0;
            
            // Build accounts only for those that have names
            for (int i = 0; i < Math.Min(4, txtAccountNames.Length); i++)
            {
                var accountName = txtAccountNames[i]?.Text?.Trim() ?? "";
                if (string.IsNullOrEmpty(accountName)) continue;

                var displayName = txtAccountDisplayNames[i]?.Text?.Trim();
                if (string.IsNullOrEmpty(displayName))
                    displayName = accountName; // Use account name as display name if not specified

                var accountType = cmbAccountGroups[i]?.SelectedItem?.ToString() ?? (i == 0 ? "Administrators" : "Users");
                var password = txtAccountPasswords[i]?.Text?.Trim() ?? "";

                localAccountsSection.AppendLine($"\t\t\t\t<LocalAccount wcm:action=\"add\">");
                localAccountsSection.AppendLine($"\t\t\t\t\t<Name>{accountName}</Name>");
                localAccountsSection.AppendLine($"\t\t\t\t\t<DisplayName>{displayName}</DisplayName>");
                localAccountsSection.AppendLine($"\t\t\t\t\t<Group>{accountType}</Group>");
                localAccountsSection.AppendLine($"\t\t\t\t\t<Password>");
                localAccountsSection.AppendLine($"\t\t\t\t\t\t<Value>{password}</Value>");
                localAccountsSection.AppendLine($"\t\t\t\t\t\t<PlainText>true</PlainText>");
                localAccountsSection.AppendLine($"\t\t\t\t\t</Password>");
                localAccountsSection.AppendLine($"\t\t\t\t</LocalAccount>");
                accountCount++;
            }
            
            // Safety check: If no accounts were generated, create a default one
            if (accountCount == 0)
            {
                localAccountsSection.AppendLine($"\t\t\t\t<LocalAccount wcm:action=\"add\">");
                localAccountsSection.AppendLine($"\t\t\t\t\t<Name>Administrator</Name>");
                localAccountsSection.AppendLine($"\t\t\t\t\t<DisplayName>Administrator</DisplayName>");
                localAccountsSection.AppendLine($"\t\t\t\t\t<Group>Administrators</Group>");
                localAccountsSection.AppendLine($"\t\t\t\t\t<Password>");
                localAccountsSection.AppendLine($"\t\t\t\t\t\t<Value></Value>");
                localAccountsSection.AppendLine($"\t\t\t\t\t\t<PlainText>true</PlainText>");
                localAccountsSection.AppendLine($"\t\t\t\t\t</Password>");
                localAccountsSection.AppendLine($"\t\t\t\t</LocalAccount>");
            }

            // Replace the entire LocalAccounts section with our dynamically generated accounts
            var pattern = @"<LocalAccounts>.*?</LocalAccounts>";
            var replacement = $"<LocalAccounts>\n{localAccountsSection.ToString().TrimEnd()}\n\t\t\t</LocalAccounts>";
            
            return System.Text.RegularExpressions.Regex.Replace(xmlContent, pattern, replacement, 
                System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        #endregion

        #region Settings Management

        private void SaveCurrentSettings()
        {
            // Update settings from UI
            _currentSettings.PCName = txtPCName.Text.Trim();
            _currentSettings.WindowsEdition = cmbWindowsEdition.SelectedItem?.ToString() ?? "";
            _currentSettings.ProductKey = txtProductKey.Text.Trim();
            
            _currentSettings.SystemLocale = cmbSystemLocale.SelectedItem?.ToString() ?? "";
            _currentSettings.UserLocale = cmbUserLocale.SelectedItem?.ToString() ?? "";
            _currentSettings.SameAsSystemLocale = chkSameAsSystemLocale.Checked;
            _currentSettings.UILanguage = GetLanguageCode(cmbUILanguage.SelectedItem?.ToString() ?? "");
            
            if (txtAccountNames.Length > 0)
            {
                _currentSettings.Account1Name = txtAccountNames[0].Text.Trim();
                _currentSettings.Account1DisplayName = txtAccountDisplayNames[0].Text.Trim();
                _currentSettings.Account1Type = cmbAccountGroups[0].SelectedItem?.ToString() ?? "";
            }
            
            if (txtAccountNames.Length > 1)
            {
                _currentSettings.Account2Name = txtAccountNames[1].Text.Trim();
                _currentSettings.Account2DisplayName = txtAccountDisplayNames[1].Text.Trim();
                _currentSettings.Account2Type = cmbAccountGroups[1].SelectedItem?.ToString() ?? "";
            }
            
            _currentSettings.BypassAllChecks = chkBypassAll.Checked;
            _currentSettings.BypassTPM = chkBypassTPM.Checked;
            _currentSettings.BypassRAM = chkBypassRAM.Checked;
            _currentSettings.BypassSecureBoot = chkBypassSecureBoot.Checked;
            _currentSettings.BypassCPU = chkBypassCPU.Checked;
            _currentSettings.BypassStorage = chkBypassStorage.Checked;
            _currentSettings.BypassDisk = chkBypassDisk.Checked;

            // Copy to main settings and save
            _settings = _currentSettings.Clone();
            _settings.Save();
            _hasUnsavedChanges = false;
        }

        private string GetLanguageCode(string displayName)
        {
            switch (displayName)
            {
                case "English": return "en-US";
                case "Tiếng Việt": return "vi-VN";
                default: return "en-US";
            }
        }

        private string GetLanguageDisplayName(string code)
        {
            switch (code)
            {
                case "en-US": return "English";
                case "vi-VN": return "Tiếng Việt";
                default: return "English";
            }
        }

        #endregion

        // Handle RDP and DPI change window messages
        protected override void WndProc(ref Message m)
        {
            const int WM_WTSSESSION_CHANGE = 0x02B1;
            const int WM_DPICHANGED = 0x02E0;
            const int WM_DISPLAYCHANGE = 0x007E;
            
            switch (m.Msg)
            {
                case WM_WTSSESSION_CHANGE:
                    // Handle RDP session changes
                    this.BeginInvoke(new MethodInvoker(() => {
                        this.SuspendLayout();
                        this.Invalidate(true);
                        this.PerformLayout();
                        this.ResumeLayout(true);
                    }));
                    break;
                    
                case WM_DPICHANGED:
                case WM_DISPLAYCHANGE:
                    // Handle DPI or display changes
                    this.BeginInvoke(new MethodInvoker(() => {
                        this.PerformLayout();
                    }));
                    break;
            }
            
            base.WndProc(ref m);
        }
    }
}