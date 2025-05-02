using System.Globalization;
using System.Resources;
using System.Reflection;

namespace KitBox_Project.Services
{
    public static class LocalizationManager
    {
        private static ResourceManager _resourceManager = new("KitBox_Project.Resources.Strings", Assembly.GetExecutingAssembly());
        private static CultureInfo _currentCulture = new("en");

        public static void SetLanguage(string cultureCode)
        {
            _currentCulture = new CultureInfo(cultureCode);
        }

        public static string Get(string key)
        {
            return _resourceManager.GetString(key, _currentCulture) ?? $"#{key}";
        }

        public static string CurrentLanguage => _currentCulture.TwoLetterISOLanguageName;
    }
}

