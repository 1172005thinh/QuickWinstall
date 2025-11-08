using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Application Configuration section including UI, data model, and business logic
    /// </summary>
    public class AppConfig
    {
        #region Data Model

        // TODO: Add application properties

        #endregion

        #region UI Components

        private Panel pnlAppConfig = null!;
        private Button btnAppConfigToggle = null!;
        private Label lblAppConfigTitle = null!;
        private Panel pnlAppConfigSeparator = null!;
        private Panel pnlAppConfigContent = null!;
        
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
            int contentHeight = ui.GetSectionValue("appConfig", "contentHeight", 180);
            
            pnlAppConfig = new Panel();
            pnlAppConfig.Location = new Point(0, 0);
            pnlAppConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlAppConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnAppConfigToggle = createRoundedButton();
            btnAppConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnAppConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            // Set initial button state based on current _isExpanded state
            btnAppConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnAppConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnAppConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnAppConfigToggle, _isExpanded ? "tooltips.section.collapse" : "tooltips.section.expand", lang.GetString("mainForm.sections.app"));

            lblAppConfigTitle = new Label();
            lblAppConfigTitle.Location = new Point(btnAppConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblAppConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblAppConfigTitle.Text = lang.GetString("mainForm.sections.app");
            lblAppConfigTitle.Font = theme.GetFont("subheader");
            lblAppConfigTitle.UseMnemonic = false;
            lblAppConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblAppConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblAppConfigTitle.Cursor = Cursors.Hand;
            lblAppConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblAppConfigTitle, "tooltips.appConfig.header", lang.GetString("tooltips.appConfig.header"));

            pnlAppConfigSeparator = new Panel();
            pnlAppConfigSeparator.Location = new Point(ui.GlobalTabX, btnAppConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlAppConfigSeparator.Size = new Size(pnlAppConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlAppConfigSeparator.BackColor = theme.GetColor("separator");

            pnlAppConfigContent = new Panel();
            pnlAppConfigContent.Location = new Point(0, pnlAppConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlAppConfigContent.Size = new Size(pnlAppConfig.Width, contentHeight);
            pnlAppConfigContent.BackColor = theme.GetColor("background");

            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlAppConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("muted");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            pnlAppConfigContent.Controls.Add(lblWorkInProgress);
            pnlAppConfig.Controls.Add(btnAppConfigToggle);
            pnlAppConfig.Controls.Add(lblAppConfigTitle);
            pnlAppConfig.Controls.Add(pnlAppConfigSeparator);
            pnlAppConfig.Controls.Add(pnlAppConfigContent);

            // Apply current expansion state
            pnlAppConfigContent.Visible = _isExpanded;
            pnlAppConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlAppConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            return pnlAppConfig;
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlAppConfigContent.Visible = _isExpanded;
            pnlAppConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("appConfig", "contentHeight", 100);
                pnlAppConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
                ToolTipManager.Instance.SetToolTip(btnAppConfigToggle, "tooltips.section.collapse", LangManager.Instance.GetString("mainForm.sections.app"));
            }
            else
            {
                pnlAppConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
                ToolTipManager.Instance.SetToolTip(btnAppConfigToggle, "tooltips.section.expand", LangManager.Instance.GetString("mainForm.sections.app"));
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnAppConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnAppConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
