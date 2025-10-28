using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Bypass Windows 11 Hardware Checks Configuration section including UI, data model, and business logic
    /// </summary>
    public class BypassConfig
    {
        #region Data Model

        // TODO: Add bypass properties

        #endregion

        #region UI Components

        private Panel pnlBypassConfig = null!;
        private Button btnBypassConfigToggle = null!;
        private Label lblBypassConfigTitle = null!;
        private Panel pnlBypassConfigSeparator = null!;
        private Panel pnlBypassConfigContent = null!;
        
        private Label lblWorkInProgress = null!;

        private bool _isExpanded = true;
        private bool _isLoading = false;
        private Action _onSectionToggle = null!;

        #endregion

        #region UI Initialization

        public Panel InitializeUI(Panel parentContainer, EventHandler onConfigChanged, 
            Func<Button> createRoundedButton, Action onSectionToggle)
        {
            UIValues ui = UIValues.Instance;
            ThemeManager theme = ThemeManager.Instance;
            LangManager lang = LangManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            ToolTipManager tooltips = ToolTipManager.Instance;

            _onSectionToggle = onSectionToggle;
            int contentHeight = ui.GetSectionValue("bypassConfig", "contentHeight", 180);
            
            pnlBypassConfig = new Panel();
            pnlBypassConfig.Location = new Point(0, 0);
            pnlBypassConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlBypassConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnBypassConfigToggle = createRoundedButton();
            btnBypassConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnBypassConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            btnBypassConfigToggle.Image = iconMgr.GetIconAsImage("expand", theme.IsDarkTheme, ui.GlobalIconSize);
            btnBypassConfigToggle.Tag = "collapsed";
            btnBypassConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnBypassConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.bypass"));

            lblBypassConfigTitle = new Label();
            lblBypassConfigTitle.Location = new Point(btnBypassConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblBypassConfigTitle.Size = new Size(400, ui.GlobalLabelHeight);
            lblBypassConfigTitle.Text = lang.GetString("mainForm.sections.bypass");
            lblBypassConfigTitle.Font = theme.GetFont("subheader");
            lblBypassConfigTitle.UseMnemonic = false;
            lblBypassConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblBypassConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblBypassConfigTitle.Cursor = Cursors.Hand;
            lblBypassConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblBypassConfigTitle, "tooltips.bypassConfig.header", lang.GetString("tooltips.bypassConfig.header"));

            pnlBypassConfigSeparator = new Panel();
            pnlBypassConfigSeparator.Location = new Point(ui.GlobalTabX, btnBypassConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlBypassConfigSeparator.Size = new Size(pnlBypassConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlBypassConfigSeparator.BackColor = theme.GetColor("separator");

            pnlBypassConfigContent = new Panel();
            pnlBypassConfigContent.Location = new Point(0, pnlBypassConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlBypassConfigContent.Size = new Size(pnlBypassConfig.Width, contentHeight);
            pnlBypassConfigContent.BackColor = theme.GetColor("background");

            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlBypassConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("normal");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            pnlBypassConfigContent.Controls.Add(lblWorkInProgress);
            pnlBypassConfig.Controls.Add(btnBypassConfigToggle);
            pnlBypassConfig.Controls.Add(lblBypassConfigTitle);
            pnlBypassConfig.Controls.Add(pnlBypassConfigSeparator);
            pnlBypassConfig.Controls.Add(pnlBypassConfigContent);

            return pnlBypassConfig;
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlBypassConfigContent.Visible = _isExpanded;
            pnlBypassConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = 100;
                pnlBypassConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
            }
            else
            {
                pnlBypassConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnBypassConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnBypassConfigToggle.Tag = _isExpanded ? "collapsed" : "expanded";

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
