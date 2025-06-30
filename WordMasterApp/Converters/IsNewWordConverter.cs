using System.Globalization;
using Microsoft.Maui.Controls;
using WordMasterApp.EntityViewModels;

namespace WordMasterApp.Converters
{
    public class IsNewWordConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is WordEntityViewModel wordWrapper && !wordWrapper.IsManaged;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}