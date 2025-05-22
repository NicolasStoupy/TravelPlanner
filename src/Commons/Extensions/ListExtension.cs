using System.Collections.ObjectModel;


namespace Commons.Extensions
{
    public static class ListExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return new ObservableCollection<T>(source);
        }
    }
}
