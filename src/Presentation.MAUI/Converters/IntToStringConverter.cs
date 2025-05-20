using System.Globalization;


namespace Presentation.MAUI.Converters
{
    /// <summary>
    /// Converter that transforms an integer to its string representation and parses strings back to integers.
    /// </summary>
    public class IntToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts an integer value to a string.
        /// </summary>
        /// <param name="value">The value produced by the binding source, expected to be an <see cref="int"/> or convertible to string.</param>
        /// <param name="targetType">The type of the binding target property (should be <see cref="string"/>).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>
        /// The string representation of the input value; returns an empty string if the input is null.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? "";
        }
        /// <summary>
        /// Converts a string representation of a number back to an integer.
        /// </summary>
        /// <param name="value">The value produced by the binding target, expected to be a numeric string.</param>
        /// <param name="targetType">The type to convert to (should be <see cref="int"/>).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use for parsing.</param>
        /// <returns>
        /// The parsed integer value if conversion succeeds; otherwise, returns 0 as a default.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value as string, out int result))
                return result;
            return 0; // Valeur par défaut si conversion échoue
        }
    }
}
