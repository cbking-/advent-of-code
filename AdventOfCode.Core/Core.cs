namespace Core;
public static class Helpers
{
    public static async Task<string[]> LoadDataAsync(string fileName)
    {
        using var file = new StreamReader(fileName);
        var data = await file.ReadToEndAsync();
        return data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    //https://stackoverflow.com/a/14663233/17400290
    public class IntArrayKeyComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[]? x, int[]? y)
        {
            if (x?.Length != y?.Length)
            {
                return false;
            }
            for (int i = 0; i < x?.Length; i++)
            {
                if (x[i] != y?[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(int[] obj)
        {
            int result = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    result = result * 23 + obj[i];
                }
            }
            return result;
        }
    }

    //based on http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/
    ///<summary>
    /// Gets the next value or, if at the end of the list, gets the first value.
    ///</summary>
    public static T? GetNext<T>(IEnumerable<T> list, T current)
    {
        try
        {
            return list.SkipWhile(x => x != null && !x.Equals(current)).Skip(1).FirstOrDefault() ?? list.First();
        }
        catch
        {
            return default(T);
        }
    }

    //based on http://www.herlitz.nu/2011/12/01/getting-the-previous-and-next-record-from-list-using-linq/
    ///<summary>
    /// Gets the previous value or, if at the beginning of the list, gets the last value.
    ///</summary>
    public static T? GetPrevious<T>(IEnumerable<T> list, T current)
    {
        try
        {
            return list.TakeWhile(x => x != null && !x.Equals(current)).LastOrDefault() ?? list.Last();
        }
        catch
        {
            return default(T);
        }
    }

     public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((Item, Index) => (Item, Index));
    }

    //https://codereview.stackexchange.com/a/237442
    /// <summary>
    /// Finds all the divisors of any positive integer passed as argument. 
    /// Returns an array of int with all the divisors of the argument.
    /// Returns null if the argument is zero or negative.
    /// </summary>
    public static int[] GetDivisors(int n)
    {
        if (n <= 0)
        {
            return new int[] { };
        }

        List<int> divisors = new List<int>();

        for (int i = 1; i <= Math.Sqrt(n); i++)
        {
            if (n % i == 0)
            {
                divisors.Add(i);
                if (i != n / i)
                {
                    divisors.Add(n / i);
                }
            }
        }

        divisors.Sort();

        return divisors.ToArray();
    }
}
