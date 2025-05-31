using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.DTOs;

namespace WordMasterApp.Converters
{
    public class SelectedItemToColorMultiConverter : IMultiValueConverter
    {
        public Color NormalColor { get; set; }
        public Color SelectedColor { get; set; }

        public SelectedItemToColorMultiConverter()
        {
            NormalColor = Colors.Transparent;
            SelectedColor = Application.Current.RequestedTheme == AppTheme.Dark
                ? (Color)Application.Current.Resources["PrimaryDark"]
                : (Color)Application.Current.Resources["PrimaryLight"];
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is WordDTO current && values[1] is WordDTO selected && current.Id == selected.Id)
            {
                return SelectedColor;
            }

            return NormalColor;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}