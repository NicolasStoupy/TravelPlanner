using System.ComponentModel.DataAnnotations;

namespace Commons.Extensions
{
    /// <summary>
    /// /// Provides an extension method to obtain the display name of an enum value using its DisplayAttribute.
    /// </summary>
    public static class EnumDisplayName
    {
        /// <summary>
        /// Retrieves the display name for an enum value based on the DisplayAttribute if present; otherwise, returns the enum's string representation.
        /// </summary>
        /// <param name="value">The enum value to retrieve the display name for.</param>
        /// <returns>
        /// A string representing the display name of the enum value if a DisplayAttribute is applied; otherwise, the enum value's name.
        /// </returns>
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