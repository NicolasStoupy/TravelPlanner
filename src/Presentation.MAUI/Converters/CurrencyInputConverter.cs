using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Converters
{
    public class CurrencyInputConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d)
                return d.ToString("F2") + " €";

            return "0.00 €";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value?.ToString()?.Replace("€", "").Trim();

            if (decimal.TryParse(input, NumberStyles.Number, culture, out var result))
                return result;

            return 0m;
        }
    }
}
