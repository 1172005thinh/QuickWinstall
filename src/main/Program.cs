using System;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Initialize LanguageManager first to load saved language settings
            LangManager.Initialize();
            
            // Initialize IconManager and validate all icons
            bool iconsValid = IconManager.InitializeAndValidate();
            if (!iconsValid)
            {
                System.Diagnostics.Debug.WriteLine("Warning: Some icons are missing. Application will use fallbacks.");            }
            
        }
    }
}