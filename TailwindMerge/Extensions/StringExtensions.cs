namespace TailwindMerge.Extensions;

internal static class StringExtensions
{
    internal static string Join(this IEnumerable<object> strings)
    {
        return string.Join(
            " ",
            strings.SelectMany(item =>
            {
                return item switch
                {
                    string str => [str],
                    IDictionary<string, bool> dict
                        => dict.Where(pair => pair.Value).Select(pair => pair.Key),
                    _ => []
                };
            })
        );
    }
}
