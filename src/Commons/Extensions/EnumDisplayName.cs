
using System.ComponentModel.DataAnnotations;


namespace Commons.Extensions
{
    public static class EnumDisplayName
    {
        /// <summary>
        /// Obtient le nom d’affichage d’une valeur d’énumération en utilisant l’attribut <see cref="DisplayAttribute"/>.
        /// </summary>
        /// <param name="value">La valeur d’énumération pour laquelle récupérer le nom d’affichage.</param>
        /// <returns>
        /// Si l’attribut <see cref="DisplayAttribute"/> est présent sur la valeur d’énumération, retourne sa propriété <c>Name</c>;
        /// sinon, retourne le résultat de <c>value.ToString()</c>.
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