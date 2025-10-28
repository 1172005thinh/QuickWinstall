using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles OOBE Configuration section including UI, data model, and business logic
    /// </summary>
    public class OOBEConfig
    {
        #region Data Model

        // TODO: Add OOBE properties

        #endregion

        #region UI Components

        private Panel pnlOOBEConfig = null!;
        private Button btnOOBEConfigToggle = null!;
        private Label lblOOBEConfigTitle = null!;
        private Panel pnlOOBEConfigSeparator = null!;
        private Panel pnlOOBEConfigContent = null!;
        
        private Label lblWorkInProgress = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false;
        private Action? _onSectionToggle = null;

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
            int contentHeight = ui.GetSectionValue("oobeConfig", "contentHeight", 180);
            
            pnlOOBEConfig = new Panel();
            pnlOOBEConfig.Location = new Point(0, 0);
            pnlOOBEConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlOOBEConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnOOBEConfigToggle = createRoundedButton();
            btnOOBEConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnOOBEConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            // Set initial button state based on current _isExpanded state
            btnOOBEConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnOOBEConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnOOBEConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnOOBEConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.oobe"));

            lblOOBEConfigTitle = new Label();
            lblOOBEConfigTitle.Location = new Point(btnOOBEConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblOOBEConfigTitle.Size = new Size(400, ui.GlobalLabelHeight);
            lblOOBEConfigTitle.Text = lang.GetString("mainForm.sections.oobe");
            lblOOBEConfigTitle.Font = theme.GetFont("subheader");
            lblOOBEConfigTitle.UseMnemonic = false;
            lblOOBEConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblOOBEConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblOOBEConfigTitle.Cursor = Cursors.Hand;
            lblOOBEConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblOOBEConfigTitle, "tooltips.oobeConfig.header", lang.GetString("tooltips.oobeConfig.header"));

            pnlOOBEConfigSeparator = new Panel();
            pnlOOBEConfigSeparator.Location = new Point(ui.GlobalTabX, btnOOBEConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlOOBEConfigSeparator.Size = new Size(pnlOOBEConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlOOBEConfigSeparator.BackColor = theme.GetColor("separator");

            pnlOOBEConfigContent = new Panel();
            pnlOOBEConfigContent.Location = new Point(0, pnlOOBEConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlOOBEConfigContent.Size = new Size(pnlOOBEConfig.Width, contentHeight);
            pnlOOBEConfigContent.BackColor = theme.GetColor("background");

            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlOOBEConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("muted");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            pnlOOBEConfigContent.Controls.Add(lblWorkInProgress);
            pnlOOBEConfig.Controls.Add(btnOOBEConfigToggle);
            pnlOOBEConfig.Controls.Add(lblOOBEConfigTitle);
            pnlOOBEConfig.Controls.Add(pnlOOBEConfigSeparator);
            pnlOOBEConfig.Controls.Add(pnlOOBEConfigContent);

            // Apply current expansion state
            pnlOOBEConfigContent.Visible = _isExpanded;
            pnlOOBEConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlOOBEConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            return pnlOOBEConfig;
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlOOBEConfigContent.Visible = _isExpanded;
            pnlOOBEConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("appConfig", "contentHeight", 100);
                pnlOOBEConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
            }
            else
            {
                pnlOOBEConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnOOBEConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnOOBEConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
