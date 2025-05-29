namespace TailwindMerge.Rules;

public sealed class ArbitraryIntegerRule : ArbitraryValueRule
{
    protected override string Parameter { get; } = "number";

    protected override bool TestValue(string value)
    {
        if (int.TryParse(value, out _))
        {
            return true;
        }
        return false;
    }
}
