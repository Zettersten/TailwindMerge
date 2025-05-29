namespace TailwindMerge.Rules;

public sealed class ArbitraryNumberRule : ArbitraryValueRule
{
    protected override string Parameter { get; } = "number";

    protected override bool TestValue(string value)
    {
        return double.TryParse(value, out _);
    }
}
