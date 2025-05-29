namespace TailwindMerge.Rules;

public sealed class ArbitrarySizeRule : ArbitraryValueRule
{
    protected override string Parameter { get; } = "size";

    protected override bool TestValue(string value)
    {
        return false;
    }
}
