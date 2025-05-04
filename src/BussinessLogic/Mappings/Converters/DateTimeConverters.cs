using AutoMapper;

namespace BussinessLogic.Mappings.Converters
{
    /// <summary>
    /// Converts a <see cref="DateOnly"/> value to a <see cref="DateTime"/> value using midnight (00:00)
    /// as the time component. </summary>
    public class DateOnlyToDateTimeConverter : ITypeConverter<DateOnly, DateTime>
    {

        /// <summary>
        /// Converts a <see cref="DateOnly"/> to <see cref="DateTime"/> by assigning <see cref="TimeOnly.MinValue"/> (00:00) as the time.
        /// </summary>
        /// <param name="source">The source <see cref="DateOnly"/> value to convert.</param>
        /// <param name="destination">The existing <see cref="DateTime"/> destination value (not used in this conversion).</param>
        /// <param name="context">The AutoMapper resolution context.</param>
        /// <returns>A <see cref="DateTime"/> with the date from <paramref name="source"/> and time set to 00:00.</returns>
        public DateTime Convert(DateOnly source, DateTime destination, ResolutionContext context)
        {
            return source.ToDateTime(TimeOnly.MinValue);
        }
    }
    /// <summary>
    /// Converts a <see cref="DateTime"/> value to a <see cref="DateOnly"/> by extracting the date component.
    /// </summary>
    public class DateTimeToDateOnlyConverter : ITypeConverter<DateTime, DateOnly>
    {
        /// <summary>
        /// Converts a <see cref="DateTime"/> to <see cref="DateOnly"/> by dropping the time component.
        /// </summary>
        /// <param name="source">The source <see cref="DateTime"/> value to convert.</param>
        /// <param name="destination">The existing <see cref="DateOnly"/> destination value (not used in this conversion).</param>
        /// <param name="context">The AutoMapper resolution context.</param>
        /// <returns>A <see cref="DateOnly"/> representing only the date portion of <paramref name="source"/>.</returns>
        public DateOnly Convert(DateTime source, DateOnly destination, ResolutionContext context)
        {
            return DateOnly.FromDateTime(source);
        }
    }
}