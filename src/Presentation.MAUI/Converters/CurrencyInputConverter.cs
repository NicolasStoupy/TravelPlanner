using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Converters
{
    /// <summary>
    /// Value converter that formats a decimal as a currency string with two decimal places and a euro symbol,
    /// and parses such strings back into decimal values.
    /// </summary>
    public class CurrencyInputConverter : IValueConverter
    {
        /// <summary>
        /// Converts a decimal value to a formatted currency string (e.g., "123.45 €").
        /// </summary>
        /// <param name="value">The value produced by the binding source, expected to be a <see cref="decimal"/>.</param>
        /// <param name="targetType">The type of the binding target property (should be <see cref="string"/>).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use for formatting.</param>
        /// <returns>
        /// A string representation of the decimal with two decimal places and a trailing euro symbol;
        /// returns "0.00 €" if the input is not a decimal.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d)
                return d.ToString("F2") + " €";

            return "0.00 €";
        }
        /// <summary>
        /// Converts a currency-formatted string back to a decimal value.
        /// </summary>
        /// <param name="value">The value produced by the binding target, expected to be a string like "123.45 €".</param>
        /// <param name="targetType">The type to convert to (should be <see cref="decimal"/>).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use for parsing.</param>
        /// <returns>
        /// The parsed decimal value if conversion succeeds; otherwise 0m.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value?.ToString()?.Replace("€", "").Trim();

            if (decimal.TryParse(input, NumberStyles.Number, culture, out var result))
                return result;

            return 0m;
        }
    }
}