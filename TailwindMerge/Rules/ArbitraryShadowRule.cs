using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryShadowRule : ArbitraryValueRule
{
    private const string shadowRegex = @"^-?((\d+)?\.?(\d+)[a-z]+|0)_-?((\d+)?\.?(\d+)[a-z]+|0)";

    protected override bool TestValue(string value)
    {
        return ShadowRegex().IsMatch(value);
    }

    [GeneratedRegex(shadowRegex)]
    private static partial Regex ShadowRegex();
}
