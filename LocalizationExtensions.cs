using System;
using System.Windows.Forms;

namespace QuickWinstall
{
    // Extension methods to help with localization
    public static class LocalizationExtensions
    {
        public static void SetLocalizedText(this Control control, string key)
        {
            control.Text = LanguageManager.Instance.GetString(key, control.Text);
        }

        public static void SetLocalizedToolTip(this ToolTip toolTip, Control control, string key)
        {
            string text = LanguageManager.Instance.GetString(key);
            toolTip.SetToolTip(control, text);
        }

        public static void LocalizeForm(this Form form)
        {
            // Recursively localize all controls in the form
            LocalizeControls(form);
        }

        private static void LocalizeControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // Check if control has a Tag with localization key
                if (control.Tag is string key && !string.IsNullOrEmpty(key))
                {
                    control.SetLocalizedText(key);
                }

                // Recursively process child controls
                if (control.HasChildren)
                {
                    LocalizeControls(control);
                }
            }
        }

        public static void RefreshLanguage(this Form form)
        {
            form.SuspendLayout();
            try
            {
                form.LocalizeForm();
            }
            finally
            {
                form.ResumeLayout(true);
            }
        }
    }
}