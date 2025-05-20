using System.Globalization;


namespace Presentation.MAUI.Converters
{
    /// <summary>
    /// Provides a value converter that transforms a byte array into an <see cref="ImageSource"/> for MAUI controls.
    /// If the conversion fails or the input is null or empty, a fallback image ("noimage.png") is returned.
    /// </summary>
    public class ByteArrayToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a byte array to an <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="value">The value produced by the binding source, expected to be a byte array.</param>
        /// <param name="targetType">The type of the binding target property (should be <see cref="ImageSource"/>).</param>
        /// <param name="parameter">Optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>
        /// An <see cref="ImageSource"/> created from the provided byte array if valid; otherwise, the string "noimage.png" as a fallback.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is byte[] bytes && bytes.Length > 0)
                {

                    return ImageSource.FromStream(() => new MemoryStream(bytes));
                }
            }
            catch (Exception ex)
            {

                return "noimage.png";
            }


            return "noimage.png";
        }
        /// <summary>
        /// ConvertBack is not implemented for this converter.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Optional converter parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Throws <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown always, as conversion back is not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
