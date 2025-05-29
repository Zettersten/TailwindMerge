namespace TailwindMerge.Rules;

public sealed class NumberRule : IRule
{
    public bool Execute(string value)
    {
        return double.TryParse(value, out _);
    }
}
