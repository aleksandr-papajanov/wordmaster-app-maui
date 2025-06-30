using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMasterApp.Helpers
{
    public static class ThemeExtentions
    {
        public static Color GetColor(string resourceKeyBase)
        {
            if (Application.Current == null)
            {
                return Colors.Transparent;
            }

            var theme = Application.Current.RequestedTheme;
            var themeKey = theme == AppTheme.Dark ? $"{resourceKeyBase}Dark" : $"{resourceKeyBase}Light";

            if (Application.Current.Resources.TryGetValue(themeKey, out var colorObj) && colorObj is Color color)
            {
                return color;
            }

            // just in case
            if (Application.Current.Resources.TryGetValue(resourceKeyBase, out var fallbackObj) && fallbackObj is Color fallbackColor)
            {
                return fallbackColor;
            }

            return Colors.Transparent;
        }
    }
}
