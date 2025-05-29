namespace TailwindMerge.Rules;

public sealed class ArbitraryUrlRule : ArbitraryValueRule
{
    protected override string Parameter { get; } = "url";

    protected override bool TestValue(string value)
    {
        return value.StartsWith("url(");
    }
}
