using System;
using System.Drawing;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall.Config
{
    /// <summary>
    /// Handles Disk & Partition Configuration section including UI, data model, and business logic
    /// </summary>
    public class DiskPartConfig
    {
        #region Data Model

        // TODO: Add disk/partition properties

        #endregion

        #region UI Components

        private Panel pnlDiskPartConfig = null!;
        private Button btnDiskPartConfigToggle = null!;
        private Label lblDiskPartConfigTitle = null!;
        private Panel pnlDiskPartConfigSeparator = null!;
        private Panel pnlDiskPartConfigContent = null!;
        
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
            int contentHeight = ui.GetSectionValue("diskPartConfig", "contentHeight", 180);
            
            pnlDiskPartConfig = new Panel();
            pnlDiskPartConfig.Location = new Point(0, 0);
            pnlDiskPartConfig.Size = new Size(parentContainer.Width - 20, ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight);
            pnlDiskPartConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            btnDiskPartConfigToggle = createRoundedButton();
            btnDiskPartConfigToggle.Location = new Point(ui.GlobalTabX, ui.GlobalSpacingY);
            btnDiskPartConfigToggle.Size = new Size(ui.GlobalBtnBox, ui.GlobalBtnBox);
            // Set initial button state based on current _isExpanded state
            btnDiskPartConfigToggle.Image = iconMgr.GetIconAsImage(_isExpanded ? "expand" : "collapse", theme.IsDarkTheme, ui.GlobalIconSize);
            btnDiskPartConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";
            btnDiskPartConfigToggle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(btnDiskPartConfigToggle, "tooltips.section.expandCollapse", lang.GetString("mainForm.sections.diskPart"));

            lblDiskPartConfigTitle = new Label();
            lblDiskPartConfigTitle.Location = new Point(btnDiskPartConfigToggle.Right + ui.GlobalSpacingX, ui.GlobalSpacingY + (ui.GlobalBtnBox - ui.GlobalLabelHeight) / 2);
            lblDiskPartConfigTitle.Size = new Size(ui.GlobalLabelWidth * 2, ui.GlobalLabelHeight);
            lblDiskPartConfigTitle.Text = lang.GetString("mainForm.sections.diskPart");
            lblDiskPartConfigTitle.Font = theme.GetFont("subheader");
            lblDiskPartConfigTitle.UseMnemonic = false;
            lblDiskPartConfigTitle.ForeColor = theme.GetFontColor("subheader");
            lblDiskPartConfigTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblDiskPartConfigTitle.Cursor = Cursors.Hand;
            lblDiskPartConfigTitle.Click += (sender, e) => ToggleSection();
            tooltips.SetToolTip(lblDiskPartConfigTitle, "tooltips.diskPartConfig.header", lang.GetString("tooltips.diskPartConfig.header"));

            pnlDiskPartConfigSeparator = new Panel();
            pnlDiskPartConfigSeparator.Location = new Point(ui.GlobalTabX, btnDiskPartConfigToggle.Bottom + ui.GlobalSpacingY / 2);
            pnlDiskPartConfigSeparator.Size = new Size(pnlDiskPartConfig.Width - 2 * ui.GlobalTabX, 1);
            pnlDiskPartConfigSeparator.BackColor = theme.GetColor("separator");

            pnlDiskPartConfigContent = new Panel();
            pnlDiskPartConfigContent.Location = new Point(0, pnlDiskPartConfigSeparator.Bottom + ui.GlobalSpacingY);
            pnlDiskPartConfigContent.Size = new Size(pnlDiskPartConfig.Width, contentHeight);
            pnlDiskPartConfigContent.BackColor = theme.GetColor("background");

            lblWorkInProgress = new Label();
            lblWorkInProgress.Location = new Point(ui.GlobalTabX * 2, ui.GlobalSpacingY);
            lblWorkInProgress.Size = new Size(pnlDiskPartConfigContent.Width - ui.GlobalTabX * 4, contentHeight - ui.GlobalSpacingY * 2);
            lblWorkInProgress.Text = "Work in progress...";
            lblWorkInProgress.Font = theme.GetFont("muted");
            lblWorkInProgress.ForeColor = theme.GetFontColor("muted");
            lblWorkInProgress.TextAlign = ContentAlignment.MiddleCenter;

            pnlDiskPartConfigContent.Controls.Add(lblWorkInProgress);
            pnlDiskPartConfig.Controls.Add(btnDiskPartConfigToggle);
            pnlDiskPartConfig.Controls.Add(lblDiskPartConfigTitle);
            pnlDiskPartConfig.Controls.Add(pnlDiskPartConfigSeparator);
            pnlDiskPartConfig.Controls.Add(pnlDiskPartConfigContent);

            // Apply current expansion state
            pnlDiskPartConfigContent.Visible = _isExpanded;
            pnlDiskPartConfigSeparator.Visible = _isExpanded;
            if (!_isExpanded)
            {
                pnlDiskPartConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2;
            }

            return pnlDiskPartConfig;
        }

        #endregion

        #region UI Interactions

        private void ToggleSection()
        {
            UIValues ui = UIValues.Instance;
            _isExpanded = !_isExpanded;
            pnlDiskPartConfigContent.Visible = _isExpanded;
            pnlDiskPartConfigSeparator.Visible = _isExpanded;

            if (_isExpanded)
            {
                int contentHeight = ui.GetSectionValue("appConfig", "contentHeight", 100);
                pnlDiskPartConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY + 2 + contentHeight;
            }
            else
            {
                pnlDiskPartConfig.Height = ui.GlobalBtnBox + ui.GlobalSpacingY * 2;
            }

            ThemeManager theme = ThemeManager.Instance;
            IconManager iconMgr = IconManager.Instance;
            
            string iconName = _isExpanded ? "expand" : "collapse";
            btnDiskPartConfigToggle.Image = iconMgr.GetIconAsImage(iconName, theme.IsDarkTheme, ui.GlobalIconSize);
            btnDiskPartConfigToggle.Tag = _isExpanded ? "expanded" : "collapsed";

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
