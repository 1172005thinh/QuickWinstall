using System;
using System.Windows.Forms;

namespace QuickWinstall.Lib
{
    #region LangHelper
    public static class LangHelper
    {
        #region RegisterForm
        public static void RegisterForm(Form form)
        {
            if (form is ILangRefreshable refreshableForm)
            {
                LangManager.langChanged += (sender, args) =>
                {
                    try
                    {
                        if (form != null && !form.IsDisposed && form.IsHandleCreated)
                        {
                            if (form.InvokeRequired)
                            {
                                form.Invoke(new Action(() => refreshableForm.RefreshLang()));
                            }
                            else
                            {
                                refreshableForm.RefreshLang();
                            }
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Form was disposed, ignore
                    }
                    catch (InvalidOperationException)
                    {
                        // Form handle not created or disposed, ignore
                    }
                };
            }
        }
        #endregion

        #region RefreshComboBoxItems
        public static void RefreshComboBoxItems(ComboBox comboBox, string[] items)
        {
            if (comboBox == null) return;

            var selectedIndex = comboBox.SelectedIndex;
            comboBox.Items.Clear();

            foreach (var item in items)
            {
                comboBox.Items.Add(LangManager.GetString(item));
            }

            if (selectedIndex >= 0 && selectedIndex < comboBox.Items.Count)
            {
                comboBox.SelectedIndex = selectedIndex;
            }
        }
        #endregion

        #region Theme ComboBox
        public static void RefreshThemeComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Settings_ThemeLight",
                "Settings_ThemeDark"
            });
        }
        #endregion

        #region Windows Edition ComboBox
        public static void RefreshWindowsEditionComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "GeneralConfig_WindowsHome",
                "GeneralConfig_WindowsPro",
                "GeneralConfig_WindowsEducation",
                "GeneralConfig_WindowsEnterprise"
            });
        }
        #endregion

        #region CPU Architecture ComboBox
        public static void RefreshCPUArchitectureComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "GeneralConfig_CPUIntelAMD",
                "GeneralConfig_CPUARM64"
            });
        }
        #endregion

        #region System Locale ComboBox
        public static void RefreshSystemLocaleComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "LangRegionConfig_SystemLocaleUnitedStates",
                "LangRegionConfig_SystemLocaleVietnam"
            });
        }
        #endregion

        #region User Locale ComboBox
        public static void RefreshUserLocaleComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "LangRegionConfig_UserLocaleUnitedStates",
                "LangRegionConfig_UserLocaleVietnam"
            });
        }
        #endregion

        #region Windows UI Lang ComboBox
        public static void RefreshWindowsUILangComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "LangRegionConfig_WindowsUILangEnglishUnitedStates",
                "LangRegionConfig_WindowsUILangVietnameseVietnam"
            });
        }
        #endregion

        #region Keyboard Layout ComboBox
        public static void RefreshKeyboardLayoutComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "LangRegionConfig_KeyboardLayoutUS",
                "LangRegionConfig_KeyboardLayoutVietnamese"
            });
        }
        #endregion

        #region Time Zone ComboBox
        public static void RefreshTimeZoneComboBox(ComboBox comboBox)
        {
            RefreshComboBoxItems(comboBox, new string[]
            {
                "Select",
                "LangRegionConfig_TimeZonePacific",
                "LangRegionConfig_TimeZoneSEAsia"
            });
        }
        #endregion
    }
    #endregion
}