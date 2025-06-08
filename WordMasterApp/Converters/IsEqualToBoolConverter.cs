using System.Globalization;
using Microsoft.Maui.Controls;

namespace WordMasterApp.Converters
{
    public class IsEqualToBoolConverter : IMultiValueConverter, IMarkupExtension
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            var first = values[0];
            var second = values[1];
            return first != null && second != null && first.Equals(second);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}