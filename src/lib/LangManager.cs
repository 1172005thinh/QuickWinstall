using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace QuickWinstall.Lib
{
    /// <summary>
    /// Manages application localization and language resources
    /// </summary>
    public static class LangManager
    {
        
    }

    public interface ILanguageRefreshable
    {
        void RefreshLanguage();
    }
}
