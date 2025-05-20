
using System.Globalization;


namespace Presentation.MAUI.Converters
{
    /// <summary>
    /// Converter that truncates a string to a specified maximum length and appends a suffix if truncated.
    /// </summary>
    /// <remarks>
    /// The converter parameter can specify the maximum length and suffix in the format "maxLength|suffix".
    /// For example: "100|..." will truncate to 100 characters and append "..." if the original string exceeds that length.
    /// Defaults to a max length of 50 and suffix "..." if no parameter is provided or parsing fails.
    /// </remarks>
    public class StringLimitConverter : IValueConverter
    {
        /// <summary>
        /// Truncates the input string if it exceeds the specified maximum length.
        /// </summary>
        /// <param name="value">The value produced by the binding source, expected to be a <see cref="string"/>.</param>
        /// <param name="targetType">The type of the binding target property (should be <see cref="string"/>).</param>
        /// <param name="parameter">
        /// Optional converter parameter in the format "maxLength|suffix".
        /// maxLength: integer for maximum allowed characters.
        /// suffix: string to append when truncation occurs.
        /// </param>
        /// <param name="culture">The culture to use (not used in this converter).</param>
        /// <returns>
        /// The original string if its length is within the limit; otherwise, a truncated string with the suffix appended.
        /// If the input is null or not a string, returns the original value.
        /// </returns>
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
