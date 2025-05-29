using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class LengthRule : IRule
{
    private const string fractionRegex = @"^\d+/\d+$";
    private static readonly string[] stringLengths = ["px", "full", "screen"];

    public bool Execute(string value)
    {
        return double.TryParse(value, out _)
            || stringLengths.Contains(value)
            || FractionRegex().IsMatch(value);
    }

    [GeneratedRegex(fractionRegex)]
    private static partial Regex FractionRegex();
}
