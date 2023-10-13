namespace lib.Labs;

public static class IEnumerableExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection, int seed = 123)
    {
        var random = new Random(seed);
        var result = new List<T>();
        foreach (var item in collection)
        {
            var j = random.Next(result.Count + 1);
            if (j == result.Count)
            {
                result.Add(item);
            }
            else
            {
                result.Add(result[j]);
                result[j] = item;
            }
        }

        return result;
    }
}