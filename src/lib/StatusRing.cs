using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuickWinstall.Lib
{
    /// <summary>
    /// A visual indicator that draws a colored ring around input controls to show validation status
    /// </summary>
    public class StatusRing : Control
    {
        ThemeManager theme = ThemeManager.Instance;
        UIValues ui = UIValues.Instance;

        private Color _ringColor = Color.Transparent;
        private int _borderWidth;
        private int _cornerRadius;

        public StatusRing()
        {
            // Make the control transparent so it doesn't block the input underneath
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.TabStop = false;
            this.Enabled = false; // Prevent it from receiving mouse/keyboard input

            _borderWidth = ui.GetValue("global.statusRing.borderWidth");
            _cornerRadius = ui.GetValue("global.statusRing.cornerRadius");
        }

        /// <summary>
        /// Gets or sets the color of the status ring
        /// </summary>
        public Color RingColor
        {
            get => _ringColor;
            set
            {
                if (_ringColor != value)
                {
                    _ringColor = value;
                    this.Invalidate(); // Redraw when color changes
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the ring border
        /// </summary>
        public int BorderWidth
        {
            get => _borderWidth;
            set
            {
                if (_borderWidth != value)
                {
                    _borderWidth = Math.Max(1, value);
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the corner radius for rounded edges
        /// </summary>
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = Math.Max(0, value);
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Sets the status and updates the ring color
        /// </summary>
        public void SetStatus(ValidationStatus status)
        {
            switch (status)
            {
                case ValidationStatus.Valid:
                    RingColor = Color.Transparent;
                    this.Visible = false;
                    break;
                case ValidationStatus.Warning:
                    RingColor = theme.GetFontColor("warning"); // Theme warning color
                    this.Visible = true;
                    break;
                case ValidationStatus.Invalid:
                    RingColor = theme.GetFontColor("error"); // Theme error color
                    this.Visible = true;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_ringColor == Color.Transparent || _ringColor.A == 0)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate the rectangle for the ring (inset by half border width)
            int halfBorder = _borderWidth / 2;
            Rectangle rect = new Rectangle(
                halfBorder,
                halfBorder,
                this.Width - _borderWidth,
                this.Height - _borderWidth
            );

            // Draw main ring
            using (Pen pen = new Pen(_ringColor, _borderWidth))
            {
                if (_cornerRadius > 0)
                {
                    // Draw rounded rectangle
                    using (GraphicsPath path = GetRoundedRectPath(rect, _cornerRadius))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Draw regular rectangle
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        /// <summary>
        /// Creates a rounded rectangle path
        /// </summary>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            // Top-left corner
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Top edge
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            // Top-right corner
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Right edge
            path.AddLine(rect.Right, rect.Y + radius, rect.Right, rect.Bottom - radius);
            // Bottom-right corner
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Bottom edge
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);
            // Bottom-left corner
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            // Left edge
            path.AddLine(rect.X, rect.Bottom - radius, rect.X, rect.Y + radius);

            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Validation status for input controls
    /// </summary>
    public enum ValidationStatus
    {
        Valid,
        Warning,
        Invalid
    }
}
