using System.Globalization;

namespace WordMasterApp.Converters
{
    public class EnumToProgressConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return 0.0;

            var enumType = value.GetType();

            if (!enumType.IsEnum) return 0.0;

            var intValue = (int)value;
            var enumValues = Enum.GetValues(enumType);
            int total = enumValues.Length;

            if (total <= 1) return 0.0;

            double progress = (double)(intValue + 1) / total;
            return Math.Min(Math.Max(progress, 0), 1);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
