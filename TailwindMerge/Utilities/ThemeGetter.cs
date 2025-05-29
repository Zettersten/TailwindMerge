namespace TailwindMerge.Utilities;

public sealed class ThemeGetter(string key)
{
    public string Key { get; } = key;

    public List<object> Execute(Dictionary<string, dynamic> theme)
    {
        return theme.TryGetValue(this.Key, out var value) ? value : new List<object>();
    }
}
