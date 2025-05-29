using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryLengthRule : ArbitraryValueRule
{
    private const string lengthUnitRegex =
        @"\d+(%|px|r?em|[sdl]?v([hwib]|min|max)|pt|pc|in|cm|mm|cap|ch|ex|r?lh|cq(w|h|i|b|min|max))|\b(calc|min|max|clamp)\(.+\)|^0$";

    protected override string Parameter { get; } = "length";

    protected override bool TestValue(string value)
    {
        return LengthUnitRegex().IsMatch(value);
    }

    [GeneratedRegex(lengthUnitRegex)]
    private static partial Regex LengthUnitRegex();
}
