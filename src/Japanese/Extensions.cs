namespace HCB.Japanese;

public static class Extensions
{
    public static int IndexOf<T>(this IEnumerable<T> source, T item)
    {
        return source.IndexOf(item, EqualityComparer<T>.Default);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> equalityComparer)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (equalityComparer is null)
        {
            throw new ArgumentNullException(nameof(equalityComparer));
        }

        var i = 0;
        foreach (var candidate in source)
        {
            if (equalityComparer.Equals(item, candidate))
            {
                return i;
            }

            i++;
        }

        return -1;
    }

    /// <summary>
    /// Concatenates multiple arrays of the same type into one IEnumerable 
    /// </summary>
    public static IEnumerable<T> Concat<T>(this IEnumerable<IEnumerable<T>> arrays)
    {
        var result = Enumerable.Empty<T>();
        foreach (var array in arrays)
        {
            result = result.Concat(array);
        }
        return result;
    }

    /// <summary>
    /// Concatenates multiple arrays of the same type into one IEnumerable 
    /// </summary>
    public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] arrays)
    {
        var result = Enumerable.Empty<T>();
        foreach (var array in arrays)
        {
            result = result.Concat(array);
        }
        return result;
    }

    /// <summary>
    /// Concatenates arrays of strings into a single string
    /// </summary>
    public static string ConcatString(this IEnumerable<string> arrays)
    {
        var result = string.Empty;
        foreach (var array in arrays)
            result += array;
        return result;
    }
}