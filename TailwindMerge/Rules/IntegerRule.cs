namespace TailwindMerge.Rules;

public sealed class IntegerRule : IRule
{
    public bool Execute(string value)
    {
        return int.TryParse(value, out _);
    }
}
