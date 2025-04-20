using AutoMapper;


namespace BussinessLogic.Mappings.Converters
{
    public class DateOnlyToDateTimeConverter : ITypeConverter<DateOnly, DateTime>
    {
        public DateTime Convert(DateOnly source, DateTime destination, ResolutionContext context)
        {
            return source.ToDateTime(TimeOnly.MinValue); 
        }
    }
    public class DateTimeToDateOnlyConverter : ITypeConverter<DateTime, DateOnly>
    {
        public DateOnly Convert(DateTime source, DateOnly destination, ResolutionContext context)
        {
            return DateOnly.FromDateTime(source);
        }
    }
}
