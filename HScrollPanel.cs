using System;
using System.Windows.Forms;

namespace QuickWinstall
{
    public class HScrollPanel : Panel
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Check if Shift is held
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                // Mark as handled so no vertical scroll happens
                if (e is HandledMouseEventArgs hme)
                    hme.Handled = true;

                // Scroll horizontally instead of vertically
                if (this.HorizontalScroll.Enabled)
                {
                    // Negative delta → natural scrolling direction
                    int scrollAmount = -e.Delta / 3; // adjust sensitivity

                    int currentPos = this.HorizontalScroll.Value;
                    int newPos = Math.Max(this.HorizontalScroll.Minimum,
                        Math.Min(this.HorizontalScroll.Maximum - this.HorizontalScroll.LargeChange + 1,
                        currentPos + scrollAmount));

                    if (newPos != currentPos)
                    {
                        this.HorizontalScroll.Value = newPos;
                        this.PerformLayout();
                    }
                }
            }
            else
            {
                // If Shift is not pressed → use normal behavior (vertical scroll)
                base.OnMouseWheel(e);
            }
        }
    }
}