
using System.Globalization;


namespace Presentation.MAUI.Converters
{
    public class StringLimitConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string str || string.IsNullOrEmpty(str))
                return value;

            // Valeurs par défaut
            int maxLength = 50;
            string suffix = "...";

            if (parameter is string paramStr)
            {
                var parts = paramStr.Split('|');
                if (parts.Length > 0 && int.TryParse(parts[0], out int len))
                    maxLength = len;
                if (parts.Length > 1)
                    suffix = parts[1];
            }

            if (str.Length <= maxLength)
                return str;

            return str.Substring(0, maxLength) + suffix;
        }


        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
