using System.Collections.ObjectModel;


namespace Commons.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// Crée une nouvelle instance de <see cref="ObservableCollection{T}"/> à partir d’une séquence.
        /// </summary>
        /// <typeparam name="T">Le type des éléments de la séquence.</typeparam>
        /// <param name="source">La séquence d’origine à convertir en <see cref="ObservableCollection{T}"/>.</param>
        /// <returns>
        /// Une <see cref="ObservableCollection{T}"/> contenant tous les éléments de la séquence <paramref name="source"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Lancée si <paramref name="source"/> est <c>null</c>.
        /// </exception>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ObservableCollection<T>(source);
        }
    }
}
