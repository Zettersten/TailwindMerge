namespace TailwindMerge.Rules;

public sealed class ArbitraryPositionRule : ArbitraryValueRule
{
    protected override string Parameter { get; } = "position";

    private static readonly HashSet<string> validPositions = new(StringComparer.OrdinalIgnoreCase)
    {
        "static",
        "relative",
        "absolute",
        "fixed",
        "sticky",
        "inherit",
        "initial",
        "unset",
    };

    protected override bool TestValue(string value)
    {
        // Accept only valid CSS position keywords
        return validPositions.Contains(value);
    }
}
