
using System.ComponentModel.DataAnnotations;


namespace Commons.Extensions
{
    public static class EnumDisplayName
    {
        public static string ToDisplayName(this Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())?
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                is DisplayAttribute[] { Length: > 0 } attrs
                ? attrs[0].Name!
                : value.ToString();
        }
    }
}