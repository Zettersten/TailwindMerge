namespace TailwindMerge.Rules;

public sealed class ArbitraryPositionRule : ArbitraryValueRule
{
    protected override string Parameter { get; } = "position";

    protected override bool TestValue(string value)
    {
        // TODO: Implement this
        return false;
    }
}
