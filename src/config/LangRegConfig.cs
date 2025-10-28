using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Language & Region Configuration section including UI, data model, and business logic
    /// </summary>
    public class LangRegConfig
    {
        #region Data Model

        // TODO: Add language and region properties

        #endregion

        #region UI Components

        private Panel pnlLangRegConfig = null!;
        private Button btnLangRegConfigToggle = null!;
        private Label lblLangRegConfigTitle = null!;
        private Panel pnlLangRegConfigSeparator = null!;
        private Panel pnlLangRegConfigContent = null!;
        
        private Label lblWorkInProgress = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false; // Flag to prevent event handlers during config loading
        private Action _onSectionToggle = null!;

        #endregion

        #region UI Initialization

        /// <summary>
        /// Initializes the Language & Region Config section UI and returns the main panel
        /// </summary>
        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged, 
            Func<Button> createRoundedButton, Action onSectionToggle)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            // Store callback
            _onSectionToggle = onSectionToggle;

            int contentHeight = 100; // Small height for work in progress message
            
            // Language & Region Config Panel
            pnlLangRegConfig = new Panel();
            pnlLangRegConfig.Location = new Point(0, 0);
            pnlLangRegConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlLangRegConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Language & Region Config Toggle Button            // Language & Region Config Toggle Button
            btnLangRegConfigToggle = createRoundedButton();
            btnLangRegConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnLangRegConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnLangRegConfigToggle.Image = iconMgr.GetIconAsImage("expand", theme.IsDarkTheme, ui.GlobalIconSize);
            btnLangRegConfigToggle.Tag = "collapsed";
            btnLangRegConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnLangRegConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.langReg"));

            // Language & Region Config Title
            lblLangRegConfigTitle = new Label();
            lblLangRegConfigTitle.Location = new Point(btnLangRegConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblLangRegConfigTitle.Size = new Size(300, ui.GlobalLabelHeight);
            lblLangRegConfigTitle.Text = lang.GetString("mainForm.sections.langReg");
            lblLangRegConfigTitle.Font = theme.GetFont("subheader");
            lblLangRegConfigTitle.UseMnemonic = false;
            lblLangRegConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblLangRegConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblLangRegConfigTitle.Cursor = Cursors.Hand;
            lblLangRegConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblLangRegConfigTitle, "tooltips.langRegConfig.header", lang.GetString("tooltips.langRegConfig.header"));

            // Line Separator
            pnlLangRegConfigSeparator = new Panel();
            pnlLangRegConfigSeparator.Location = new Point(ui.GlobalTabX, btnLangRegConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlLangRegConfigSeparator.Size = new Size(pnlLangRegConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlLangRegConfigSeparator.BackColor = theme.GetColor("separator");

            // Language & Region Config Content Panel
            pnlLangRegConfigContent = new Panel();
            pnlLangRegConfigContent.Location = new Point(0, pnlLangRegConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlLangRegConfigContent.Size = new Size(pnlLangRegConfig.Width, contentHeight);
            pnlLangRegConfigContent.BackColor = theme.GetColor("background");

            // Work In Progress Label
            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlLangRegConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("normal");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to Language & Region Config Content
            pnlLangRegConfigContent.Controls.Add(lblWorkInProgress);

            // Add controls to Language & Region Config Panel
            pnlLangRegConfig.Controls.Add(btnLangRegConfigToggle);
            pnlLangRegConfig.Controls.Add(lblLangRegConfigTitle);
            pnlLangRegConfig.Controls.Add(pnlLangRegConfigSeparator);
            pnlLangRegConfig.Controls.Add(pnlLangRegConfigContent);

            return pnlLangRegConfig;
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
                int contentHeight = 100; // Same as in InitializeUI
                pnlLangRegConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
            }
            else
            {
                pnlLangRegConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
            }

            // Update button icon
            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnLangRegConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnLangRegConfigToggle.Tag = _isExpanded ? "collapsed" : "expanded";

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

        #endregion

        #region Data Model Operations

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

            // TODO: Implement when UI controls are added
        }

        /// <summary>
        /// Clears all UI controls and resets to default state
        /// </summary>
        public void ClearControls()
        {
            // TODO: Implement when UI controls are added
        }

        /// <summary>
        /// Updates UI controls from the data model
        /// </summary>
        public void UpdateControlsFromModel()
        {
            // Set flag to prevent event handlers from firing during loading
            _isLoading = true;

            try
            {
                // TODO: Implement when UI controls are added
            }
            finally
            {
                // Always reset the flag, even if an error occurs
                _isLoading = false;
            }
        }

        /// <summary>
        /// Loads configuration into UI after config is loaded
        /// </summary>
        public void LoadConfigIntoUI()
        {
            UpdateControlsFromModel();
        }

        /// <summary>
        /// Sets configuration values from JSON
        /// </summary>
        public void SetValues(dynamic json)
        {
            // TODO: Implement when properties are added
        }

        #endregion
    }
}
