using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Personalization Configuration section including UI, data model, and business logic
    /// </summary>
    public class PersonalConfig
    {
        #region Data Model

        // TODO: Add personalization properties

        #endregion

        #region UI Components

        private Panel pnlPersonalConfig = null!;
        private Button btnPersonalConfigToggle = null!;
        private Label lblPersonalConfigTitle = null!;
        private Panel pnlPersonalConfigSeparator = null!;
        private Panel pnlPersonalConfigContent = null!;
        
        private Label lblWorkInProgress = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false;
        private Action? _onSectionToggle = null;

        /// <summary>
        /// Gets whether the section is expanded
        /// </summary>
        public bool IsExpanded => _isExpanded;

        #endregion

        #region UI Initialization

        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged, 
            Func<Button> createRoundedButton, Action? onSectionToggle)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            _onSectionToggle = onSectionToggle;
            int contentHeight = ui.GetSectionValue("personalConfig", "contentHeight", 180);
            
            pnlPersonalConfig = new Panel();
            pnlPersonalConfig.Location = new Point(0, 0);
            pnlPersonalConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlPersonalConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnPersonalConfigToggle = createRoundedButton();
            btnPersonalConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnPersonalConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            // Set initial button state based on current _isExpanded state
            btnPersonalConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnPersonalConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnPersonalConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnPersonalConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.personal"));

            lblPersonalConfigTitle = new Label();
            lblPersonalConfigTitle.Location = new Point(btnPersonalConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblPersonalConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblPersonalConfigTitle.Text = lang.GetString("mainForm.sections.personal");
            lblPersonalConfigTitle.Font = theme.GetFont("subheader");
            lblPersonalConfigTitle.UseMnemonic = false;
            lblPersonalConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblPersonalConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblPersonalConfigTitle.Cursor = Cursors.Hand;
            lblPersonalConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblPersonalConfigTitle, "tooltips.personalConfig.header", lang.GetString("tooltips.personalConfig.header"));

            pnlPersonalConfigSeparator = new Panel();
            pnlPersonalConfigSeparator.Location = new Point(ui.GlobalTabX, btnPersonalConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlPersonalConfigSeparator.Size = new Size(pnlPersonalConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlPersonalConfigSeparator.BackColor = theme.GetColor("separator");

            pnlPersonalConfigContent = new Panel();
            pnlPersonalConfigContent.Location = new Point(0, pnlPersonalConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlPersonalConfigContent.Size = new Size(pnlPersonalConfig.Width, contentHeight);
            pnlPersonalConfigContent.BackColor = theme.GetColor("background");

            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlPersonalConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("muted");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            pnlPersonalConfigContent.Controls.Add(lblWorkInProgress);
            pnlPersonalConfig.Controls.Add(btnPersonalConfigToggle);
            pnlPersonalConfig.Controls.Add(lblPersonalConfigTitle);
            pnlPersonalConfig.Controls.Add(pnlPersonalConfigSeparator);
            pnlPersonalConfig.Controls.Add(pnlPersonalConfigContent);

            // Apply current expansion state
            pnlPersonalConfigContent.Visible = _isExpanded;
            pnlPersonalConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlPersonalConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            return pnlPersonalConfig;
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlPersonalConfigContent.Visible = _isExpanded;
            pnlPersonalConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("appConfig", "contentHeight", 100);
                pnlPersonalConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnPersonalConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.app"));
            }
            else
            {
                pnlPersonalConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnPersonalConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.app"));
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnPersonalConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnPersonalConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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

        #endregion

        #region Data Model Operations

        public void UpdateFromControls()
        {
            if (_isLoading) return;
            // TODO: Implement when UI controls are added
        }

        public void ClearControls()
        {
            // TODO: Implement when UI controls are added
        }

        public void UpdateControlsFromModel()
        {
            _isLoading = true;
            try
            {
                // TODO: Implement when UI controls are added
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void LoadConfigIntoUI()
        {
            UpdateControlsFromModel();
        }

        public void SetValues(dynamic json)
        {
            // TODO: Implement when properties are added
        }

        #endregion
    }
}
