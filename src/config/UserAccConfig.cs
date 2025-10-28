using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles User Account Configuration section including UI, data model, and business logic
    /// </summary>
    public class UserAccConfig
    {
        #region Data Model

        // TODO: Add user account properties

        #endregion

        #region UI Components

        private Panel pnlUserAccConfig = null!;
        private Button btnUserAccConfigToggle = null!;
        private Label lblUserAccConfigTitle = null!;
        private Panel pnlUserAccConfigSeparator = null!;
        private Panel pnlUserAccConfigContent = null!;
        
        private Label lblWorkInProgress = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false; // Flag to prevent event handlers during config loading
        private Action _onSectionToggle = null!;

        #endregion

        #region UI Initialization

        /// <summary>
        /// Initializes the User Account Config section UI and returns the main panel
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
            int contentHeight = ui.GetSectionValue("userAccConfig", "contentHeight", 180);
            
            // User Account Config Panel
            pnlUserAccConfig = new Panel();
            pnlUserAccConfig.Location = new Point(0, 0);
            pnlUserAccConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlUserAccConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // User Account Config Toggle Button
            btnUserAccConfigToggle = createRoundedButton();
            btnUserAccConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnUserAccConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            // Set initial button state based on current _isExpanded state
            btnUserAccConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnUserAccConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnUserAccConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnUserAccConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.userAcc"));

            // User Account Config Title
            lblUserAccConfigTitle = new Label();
            lblUserAccConfigTitle.Location = new Point(btnUserAccConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblUserAccConfigTitle.Size = new Size(400, ui.GlobalLabelHeight);
            lblUserAccConfigTitle.Text = lang.GetString("mainForm.sections.userAcc");
            lblUserAccConfigTitle.Font = theme.GetFont("subheader");
            lblUserAccConfigTitle.UseMnemonic = false;
            lblUserAccConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblUserAccConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblUserAccConfigTitle.Cursor = Cursors.Hand;
            lblUserAccConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblUserAccConfigTitle, "tooltips.userAccConfig.header", lang.GetString("tooltips.userAccConfig.header"));

            // Line Separator
            pnlUserAccConfigSeparator = new Panel();
            pnlUserAccConfigSeparator.Location = new Point(ui.GlobalTabX, btnUserAccConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlUserAccConfigSeparator.Size = new Size(pnlUserAccConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlUserAccConfigSeparator.BackColor = theme.GetColor("separator");

            // User Account Config Content Panel
            pnlUserAccConfigContent = new Panel();
            pnlUserAccConfigContent.Location = new Point(0, pnlUserAccConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlUserAccConfigContent.Size = new Size(pnlUserAccConfig.Width, contentHeight);
            pnlUserAccConfigContent.BackColor = theme.GetColor("background");

            // Work In Progress Label
            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlUserAccConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("muted");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to User Account Config Content
            pnlUserAccConfigContent.Controls.Add(lblWorkInProgress);

            // Add controls to User Account Config Panel
            pnlUserAccConfig.Controls.Add(btnUserAccConfigToggle);
            pnlUserAccConfig.Controls.Add(lblUserAccConfigTitle);
            pnlUserAccConfig.Controls.Add(pnlUserAccConfigSeparator);
            pnlUserAccConfig.Controls.Add(pnlUserAccConfigContent);

            // Apply current expansion state
            pnlUserAccConfigContent.Visible = _isExpanded;
            pnlUserAccConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlUserAccConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            return pnlUserAccConfig;
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
            pnlUserAccConfigContent.Visible = _isExpanded;
            pnlUserAccConfigSeparator.Visible = _isExpanded;

            // Update panel height based on state
            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("appConfig", "contentHeight", 100);
                pnlUserAccConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
            }
            else
            {
                pnlUserAccConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
            }

            // Update button icon
            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnUserAccConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnUserAccConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
