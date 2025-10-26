using System;
using System.Windows.Forms;
using QuickWinstall.Lib;
using QuickWinstall.Main;

namespace QuickWinstall
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable visual styles for Windows Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Initialize managers
                InitializeManagers();

                // Run the main form
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fatal error: {ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                    "QuickWinstall Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static void InitializeManagers()
        {
            try
            {
                // Load settings first
                var settingsManager = SettingsManager.Instance;
                
                // Apply settings (theme and language)
                settingsManager.ApplySettings();

                // Initialize other managers
                var uiValues = UIValues.Instance;
                var themeManager = ThemeManager.Instance;
                var iconManager = IconManager.Instance;
                var langManager = LangManager.Instance;
                var toolTipManager = ToolTipManager.Instance;
                var statusManager = StatusManager.Instance;
                var configValues = ConfigValues.Instance;
                var xmlGenerator = XMLGenerator.Instance;

                Console.WriteLine("All managers initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to initialize some managers: {ex.Message}");
            }
        }
    }
}
