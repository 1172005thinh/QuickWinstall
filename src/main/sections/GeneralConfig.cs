using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic.ApplicationServices;
using QuickWinstall.Lib;

namespace QuickWinstall.Sections
{
    public partial class GeneralConfig : UserControl, ILangRefreshable
    {
        private Panel headerPanel;
        private Button expandCollapseButton;
        private Label headerLabel;
        private Panel contentPanel;
        private Label windowsEditionLabel;
        private ComboBox windowsEditionCombo;
        private Label productKeyLabel;
        private TextBox[] productKeyTextBoxes;
        private Label[] productKeySeparators;
        private Label cpuArchitectureLabel;
        private ComboBox cpuArchitectureCombo;

        private bool isExpanded = true;
        private bool _isValidating = false;
        public event EventHandler ValueChanged;

        #region GeneralConfig
        public GeneralConfig()
        {
            Initialize();
            LoadDefaults();
            AttachEventHandlers();
        }
        #endregion

        #region Initialize
        private void Initialize()
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            LangManager.Initialize();

            Size = new Size(mainFormConfig.SectionWidth, mainFormConfig.SectionGeneralConfigHeight);
            Padding = new Padding(globalConfig.SectionPadding);

            #region Header Panel
            headerPanel = new Panel
            {
                Name = "headerPanel",
                Dock = DockStyle.Top,
                Height = globalConfig.SectionHeaderHeight,
                Padding = new Padding(globalConfig.SectionPadding),
                Cursor = Cursors.Hand
            };
            headerPanel.Paint += HearderPanel_Paint;
            #endregion

            #region ExpandCollapseButton
            expandCollapseButton = new Button()
            {
                Name = "expandCollapseButton",
                Size = new Size(globalConfig.ExpandCollapseButtonBox, globalConfig.ExpandCollapseButtonBox),
                Location = new Point(globalConfig.Padding, (globalConfig.SectionHeaderHeight - globalConfig.ExpandCollapseButtonBox) / 2),
            };
            ToolTipManager.SetToolTip(expandCollapseButton, LangManager.GetString("GeneralConfig_ExpandCollapse_Tooltip", "Expand/Collapse this section."));
            ThemeManager.SetButtonStyle(expandCollapseButton, ThemeManager.Type.Normal);
            IconManager.SetButtonIcon(expandCollapseButton, false, IconManager.Icons.Expand, new Size(globalConfig.IconSize, globalConfig.IconSize));
            expandCollapseButton.Click += ExpandCollapseButton_Click;
            #endregion

            #region Header Label
            headerLabel = new Label()
            {
                Name = "headerLabel",
                Text = LangManager.GetString("GeneralConfig_Header", "General Configuration"),
                Size = new Size(globalConfig.SectionHeaderTextTitleWidth, globalConfig.SectionHeaderTextTitleHeight),
                Location = new Point(expandCollapseButton.Right + globalConfig.Spacing, (globalConfig.SectionHeaderHeight - globalConfig.SectionHeaderTextTitleHeight) / 2),
            };
            ToolTipManager.SetToolTip(headerLabel, LangManager.GetString("GeneralConfig_Header_Tooltip", "Configure general Windows installation settings."));
            ThemeManager.SetLabelStyle(headerLabel, ThemeManager.Type.Header);
            headerLabel.Click += ExpandCollapseButton_Click;
            #endregion

            #region Content Panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(globalConfig.SectionPadding),
                Visible = isExpanded
            };
            #endregion

            // Add controls to header panel
            headerPanel.Controls.AddRange(new Control[]
            {
                expandCollapseButton,
                headerLabel
            });

            #region Windows Edition
            windowsEditionLabel = new Label
            {
                Name = "windowsEditionLabel",
                Text = LangManager.GetString("GeneralConfig_WindowsEditionLabel", "Windows Edition:"),
                Size = new Size(globalConfig.LabelWidth, globalConfig.LabelHeight),
                Location = new Point(globalConfig.Tab, globalConfig.RowHeight),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            ThemeManager.SetLabelStyle(windowsEditionLabel, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(windowsEditionLabel, LangManager.GetString("GeneralConfig_WindowsEdition_Tooltip", "Select the Windows edition to install."));

            windowsEditionCombo = new ComboBox
            {
                Name = "windowsEditionCombo",
                Size = new Size(globalConfig.ComboboxWidth, globalConfig.ComboboxHeight),
                Location = new Point(windowsEditionLabel.Right + globalConfig.Spacing, windowsEditionLabel.Top),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            ThemeManager.SetComboBoxStyle(windowsEditionCombo, ThemeManager.Type.Normal);

            windowsEditionCombo.Items.AddRange(new string[]
            {
                LangManager.GetString("Select", "Select"),
                LangManager.GetString("GeneralConfig_WindowsHome", "Windows 11 Home"),
                LangManager.GetString("GeneralConfig_WindowsPro", "Windows 11 Pro"),
                LangManager.GetString("GeneralConfig_WindowsEducation", "Windows 11 Education"),
                LangManager.GetString("GeneralConfig_WindowsEnterprise", "Windows 11 Enterprise")
            });
            #endregion

            #region Product Key
            productKeyLabel = new Label
            {
                Name = "productKeyLabel",
                Text = LangManager.GetString("GeneralConfig_ProductKeyLabel", "Product Key:"),
                Size = new Size(globalConfig.LabelWidth, globalConfig.LabelHeight),
                Location = new Point(globalConfig.Tab, windowsEditionLabel.Bottom + globalConfig.Spacing),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };
            ThemeManager.SetLabelStyle(productKeyLabel, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(productKeyLabel, LangManager.GetString("GeneralConfig_ProductKey_Tooltip", "Leave blank to skip Activation.\n Leave blank to use your embedded OEM key if available."));

            productKeyTextBoxes = new TextBox[5];
            productKeySeparators = new Label[4];

            for (int i = 0; i < 5; i++)
            {
                productKeyTextBoxes[i] = new TextBox
                {
                    Name = $"productKeyTextBox{i + 1}",
                    PlaceholderText = LangManager.GetString("GeneralConfig_ProductKey_Placeholder", "XXXXX"),
                    Size = new Size((globalConfig.TextboxWidth - globalConfig.Spacing * 4) / 5, globalConfig.TextboxHeight),
                    Location = new Point(productKeyLabel.Right + globalConfig.Spacing + i * ((globalConfig.TextboxWidth - globalConfig.Spacing * 4) / 5 + globalConfig.Spacing), productKeyLabel.Top),
                    MaxLength = 5,
                    CharacterCasing = CharacterCasing.Upper,
                    TextAlign = HorizontalAlignment.Center
                };
                ThemeManager.SetTextBoxStyle(productKeyTextBoxes[i], ThemeManager.Type.Normal);
                ToolTipManager.SetToolTip(productKeyTextBoxes[i], LangManager.GetString("GeneralConfig_ProductKey_Part_Tooltip", "Enter 5 characters."));

                if (i < 4)
                {
                    productKeySeparators[i] = new Label
                    {
                        Name = $"productKeySeparator{i + 1}",
                        Text = LangManager.GetString("GeneralConfig_ProductKey_Separator", "-"),
                        Size = new Size(globalConfig.Spacing, globalConfig.TextboxHeight),
                        Location = new Point(productKeyTextBoxes[i].Right, productKeyTextBoxes[i].Top),
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    };
                    ThemeManager.SetLabelStyle(productKeySeparators[i], ThemeManager.Type.Normal);
                }
            }
            #endregion

            #region CPU Architecture
            cpuArchitectureLabel = new Label
            {
                Name = "cpuArchitectureLabel",
                Text = LangManager.GetString("GeneralConfig_CPUArchitectureLabel", "CPU Architecture:"),
                Size = new Size(globalConfig.LabelWidth, globalConfig.LabelHeight),
                Location = new Point(globalConfig.Tab, productKeyLabel.Bottom + globalConfig.Spacing),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            ThemeManager.SetLabelStyle(cpuArchitectureLabel, ThemeManager.Type.Normal);
            ToolTipManager.SetToolTip(cpuArchitectureLabel, LangManager.GetString("GeneralConfig_CPUArchitecture_Tooltip", "Select the CPU architecture for installation.\n Windows 11 unfortunately does not support x86."));

            cpuArchitectureCombo = new ComboBox
            {
                Name = "cpuArchitectureCombo",
                Size = new Size(globalConfig.ComboboxWidth, globalConfig.ComboboxHeight),
                Location = new Point(cpuArchitectureLabel.Right + globalConfig.Spacing, cpuArchitectureLabel.Top),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            ThemeManager.SetComboBoxStyle(cpuArchitectureCombo, ThemeManager.Type.Normal);

            cpuArchitectureCombo.Items.AddRange(new string[]
            {
                LangManager.GetString("Select", "Select"),
                LangManager.GetString("GeneralConfig_CPUIntelAMD", "Intel/AMD (x64)"),
                LangManager.GetString("GeneralConfig_CPUARM64", "ARM64")
            });
            #endregion

            // Add controls to content panel
            contentPanel.Controls.Add(windowsEditionLabel);
            contentPanel.Controls.Add(windowsEditionCombo);
            contentPanel.Controls.Add(productKeyLabel);
            for (int i = 0; i < 5; i++)
            {
                contentPanel.Controls.Add(productKeyTextBoxes[i]);
                if (i < 4)
                {
                    contentPanel.Controls.Add(productKeySeparators[i]);
                }
            }
            contentPanel.Controls.Add(cpuArchitectureLabel);
            contentPanel.Controls.Add(cpuArchitectureCombo);

            // Add panels to main control
            Controls.AddRange(new Control[]
            {
                contentPanel,
                headerPanel
            });
        }
        #endregion

        #region AttachEventHandlers
        private void AttachEventHandlers()
        {
            windowsEditionCombo.SelectedIndexChanged += (s, e) => OnValueChanged(s, e);
            cpuArchitectureCombo.SelectedIndexChanged += (s, e) => OnValueChanged(s, e);

            for (int i = 0; i < productKeyTextBoxes.Length; i++)
            {
                // Capture the current index
                int idx = i;

                /*
                // Press TAB to move to next TextBox
                // Press SHIFT + TAB to move to previous TextBox
                productKeyTextBoxes[i].KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Tab && !e.Shift)
                    {
                        productKeyTextBoxes[idx + 1].Focus();
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Tab && e.Shift)
                    {
                        productKeyTextBoxes[idx - 1].Focus();
                        e.Handled = true;
                    }
                };
                */

                // Auto move to next TextBox when current TextBox is filled
                productKeyTextBoxes[i].TextChanged += (s, e) =>
                {
                    var textBox = s as TextBox;
                    if (textBox.Text.Length == textBox.MaxLength && idx < productKeyTextBoxes.Length - 1)
                    {
                        productKeyTextBoxes[idx + 1].Focus();
                    }
                };

                // Allow only alphanumeric characters
                productKeyTextBoxes[i].KeyPress += (s, e) =>
                {
                    if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                };

                // Trigger ValueChanged event
                productKeyTextBoxes[i].TextChanged += (s, e) => OnValueChanged(s, e);
            }
        }
        #endregion

        #region OnValueChanged
        private void OnValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region LoadDefaults
        private void LoadDefaults()
        {
            try
            {
                var defaults = Defaults.LoadFromAppFolder();
                ApplyDefaults(defaults.GeneralConfig);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading defaults: {ex.Message}");
            }
        }
        #endregion

        #region ApplyDefaults
        private void ApplyDefaults(GeneralConfigDefaults defaults)
        {
            windowsEditionCombo.SelectedItem = defaults.WindowsEdition switch
            {
                "Windows 11 Home" => LangManager.GetString("GeneralConfig_WindowsHome", "Windows 11 Home"),
                "Windows 11 Pro" => LangManager.GetString("GeneralConfig_WindowsPro", "Windows 11 Pro"),
                "Windows 11 Education" => LangManager.GetString("GeneralConfig_WindowsEducation", "Windows 11 Education"),
                "Windows 11 Enterprise" => LangManager.GetString("GeneralConfig_WindowsEnterprise", "Windows 11 Enterprise"),
                _ => LangManager.GetString("Select", "Select"),
            };

            for (int i = 0; i < productKeyTextBoxes.Length && i < defaults.ProductKey.Length; i++)
            {
                productKeyTextBoxes[i].Text = defaults.ProductKey[i] ?? "";
            }

            cpuArchitectureCombo.SelectedItem = defaults.CPUArchitecture switch
            {
                "Intel/AMD (x64)" => LangManager.GetString("GeneralConfig_CPUIntelAMD", "Intel/AMD (x64)"),
                "Windows ARM64" => LangManager.GetString("GeneralConfig_CPUARM64", "Windows ARM64"),
                _ => LangManager.GetString("Select", "Select"),
            };

            // Restore to expanded state
            isExpanded = defaults.Expanded;
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;
            contentPanel.Visible = isExpanded;

            if (defaults.Expanded)
            {
                // Expanded
                IconManager.SetButtonIcon(expandCollapseButton, false, IconManager.Icons.Expand, new Size(globalConfig.IconSize, globalConfig.IconSize));
                Height = mainFormConfig.SectionGeneralConfigHeight;
            }
            else
            {
                // Collapsed
                IconManager.SetButtonIcon(expandCollapseButton, false, IconManager.Icons.Collapse, new Size(globalConfig.IconSize, globalConfig.IconSize));
                Height = mainFormConfig.SectionHeaderHeight;
            }
        }
        #endregion

        #region ValidateConfig
        public string ValidateConfig()
        {
            if (_isValidating) return null;

            try
            {
                _isValidating = true;

                string result = ValidateConfigInternal();
                return result;
            }
            finally
            {
                _isValidating = false;
            }
        }

        private string ValidateConfigInternal()
        {
            // Validate Windows Edition
            if (windowsEditionCombo.SelectedIndex <= 0)
            {
                ThemeManager.SetComboBoxStyle(windowsEditionCombo, ThemeManager.Type.Error);
                ThemeManager.SetLabelStyle(headerLabel, ThemeManager.Type.Error);
                return LangManager.GetString("GeneralConfig_Error_WindowsEdition", "Please select a valid Windows edition.");
            }

            // Validate Product Key
            bool isAnyProductKeyPartFilled = productKeyTextBoxes.Any(tb => !string.IsNullOrWhiteSpace(tb.Text));
            bool isAllProductKeyPartsFilled = productKeyTextBoxes.All(tb => tb.Text.Length == tb.MaxLength);

            if (isAnyProductKeyPartFilled && !isAllProductKeyPartsFilled)
            {
                for (int i = 0; i < productKeyTextBoxes.Length; i++)
                {
                    ThemeManager.SetTextBoxStyle(productKeyTextBoxes[i], ThemeManager.Type.Error);
                    ThemeManager.SetLabelStyle(headerLabel, ThemeManager.Type.Error);
                }
                return LangManager.GetString("GeneralConfig_Error_ProductKey", "Please enter a complete product key or leave all fields blank.");
            }

            // Validate CPU Architecture
            if (cpuArchitectureCombo.SelectedIndex <= 0)
            {
                ThemeManager.SetComboBoxStyle(cpuArchitectureCombo, ThemeManager.Type.Error);
                ThemeManager.SetLabelStyle(headerLabel, ThemeManager.Type.Error);
                return LangManager.GetString("GeneralConfig_Error_CPUArchitecture", "Please select a valid CPU architecture.");
            }

            return null;
        }
        #endregion

        #region ClearErrorStyles
        public void ClearErrorStyles()
        {
            ThemeManager.SetComboBoxStyle(windowsEditionCombo, ThemeManager.Type.Normal);
            for (int i = 0; i < productKeyTextBoxes.Length; i++)
            {
                ThemeManager.SetTextBoxStyle(productKeyTextBoxes[i], ThemeManager.Type.Normal);
            }
            ThemeManager.SetComboBoxStyle(cpuArchitectureCombo, ThemeManager.Type.Normal);
            ThemeManager.SetLabelStyle(headerLabel, ThemeManager.Type.Normal);
        }
        #endregion

        #region ResetToDefaults
        public void ResetToDefaults()
        {
            ClearErrorStyles();
            LoadDefaults();
        }
        #endregion

        #region GetCurrentConfigs
        public SettingsManager.GeneralConfigSettings GetCurrentConfigs()
        {
            return new SettingsManager.GeneralConfigSettings
            {
                WindowsEdition = windowsEditionCombo.SelectedIndex > 0 ? windowsEditionCombo.SelectedItem.ToString() : null,
                ProductKey = productKeyTextBoxes.Select(tb => string.IsNullOrWhiteSpace(tb.Text) ? null : tb.Text).ToArray(),
                CPUArchitecture = cpuArchitectureCombo.SelectedIndex > 0 ? cpuArchitectureCombo.SelectedItem.ToString() : null,
                Expanded = isExpanded
            };
        }
        #endregion

        #region LoadConfigs
        public void LoadConfigs(SettingsManager.GeneralConfigSettings settings)
        {
            if (settings == null) return;

            windowsEditionCombo.SelectedItem = settings.WindowsEdition switch
            {
                "Windows 11 Home" => LangManager.GetString("GeneralConfig_WindowsHome", "Windows 11 Home"),
                "Windows 11 Pro" => LangManager.GetString("GeneralConfig_WindowsPro", "Windows 11 Pro"),
                "Windows 11 Education" => LangManager.GetString("GeneralConfig_WindowsEducation", "Windows 11 Education"),
                "Windows 11 Enterprise" => LangManager.GetString("GeneralConfig_WindowsEnterprise", "Windows 11 Enterprise"),
                _ => LangManager.GetString("Select", "Select"),
            };

            for (int i = 0; i < productKeyTextBoxes.Length; i++)
            {
                if (settings.ProductKey != null && i < settings.ProductKey.Length)
                {
                    productKeyTextBoxes[i].Text = settings.ProductKey[i] ?? "";
                }
                else
                {
                    productKeyTextBoxes[i].Text = "";
                }
            }

            cpuArchitectureCombo.SelectedItem = settings.CPUArchitecture switch
            {
                "Intel/AMD (x64)" => LangManager.GetString("GeneralConfig_CPUIntelAMD", "Intel/AMD (x64)"),
                "Windows ARM64" => LangManager.GetString("GeneralConfig_CPUARM64", "Windows ARM64"),
                _ => LangManager.GetString("Select", "Select"),
            };
        }
        #endregion

        #region ExpandCollapseButton_Click
        private void ExpandCollapseButton_Click(object sender, EventArgs e)
        {
            var defaults = Defaults.LoadFromAppFolder();
            var config = Config.LoadFromAppFolder();
            var globalConfig = config.Global;
            var mainFormConfig = config.MainForm;

            isExpanded = !isExpanded;
            contentPanel.Visible = isExpanded;

            if (isExpanded)
            {
                // Expanded
                IconManager.SetButtonIcon(expandCollapseButton, false, IconManager.Icons.Expand, new Size(globalConfig.IconSize, globalConfig.IconSize));
                Height = mainFormConfig.SectionGeneralConfigHeight;
            }
            else
            {
                // Collapsed
                IconManager.SetButtonIcon(expandCollapseButton, false, IconManager.Icons.Collapse, new Size(globalConfig.IconSize, globalConfig.IconSize));
                Height = mainFormConfig.SectionHeaderHeight;
            }
        }
        #endregion

        #region HearderPanel_Paint
        private void HearderPanel_Paint(object sender, PaintEventArgs e)
        {
            ThemeManager.DrawSeparatorLine(e.Graphics, headerPanel.Width, headerPanel.Height);
        }
        #endregion

        #region RefreshLang
        public void RefreshLang()
        {
            try
            {
                if (headerLabel != null)
                    headerLabel.Text = LangManager.GetString("GeneralConfig_Header", "General Configuration");
                ToolTipManager.SetToolTip(headerLabel, LangManager.GetString("GeneralConfig_Header_Tooltip", "Configure general Windows installation settings."));

                if (windowsEditionLabel != null)
                    windowsEditionLabel.Text = LangManager.GetString("GeneralConfig_WindowsEditionLabel", "Windows Edition:");
                ToolTipManager.SetToolTip(windowsEditionLabel, LangManager.GetString("GeneralConfig_WindowsEdition_Tooltip", "Select the Windows edition to install."));
                if (productKeyLabel != null)
                    productKeyLabel.Text = LangManager.GetString("GeneralConfig_ProductKeyLabel", "Product Key:");
                ToolTipManager.SetToolTip(productKeyLabel, LangManager.GetString("GeneralConfig_ProductKey_Tooltip", "Enter the product key for Windows installation."));
                if (productKeyTextBoxes != null)
                {
                    foreach (var tb in productKeyTextBoxes)
                    {
                        tb.PlaceholderText = LangManager.GetString("GeneralConfig_ProductKey_Placeholder", "XXXXX");
                        ToolTipManager.SetToolTip(tb, LangManager.GetString("GeneralConfig_ProductKey_Part_Tooltip", "Enter 5 characters."));
                    }
                }
                if (cpuArchitectureLabel != null)
                    cpuArchitectureLabel.Text = LangManager.GetString("GeneralConfig_CPUArchitectureLabel", "CPU Architecture:");
                ToolTipManager.SetToolTip(cpuArchitectureLabel, LangManager.GetString("GeneralConfig_CPUArchitecture_Tooltip", "Select the CPU architecture for installation.\n Windows 11 unfortunately does not support x86."));

                if (windowsEditionCombo != null)
                {
                    LangHelper.RefreshWindowsEditionComboBox(windowsEditionCombo);
                }
                if (cpuArchitectureCombo != null)
                {
                    LangHelper.RefreshCPUArchitectureComboBox(cpuArchitectureCombo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing language: {ex.Message}");
            }
        }
        #endregion
    }
}