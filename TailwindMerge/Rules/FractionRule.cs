using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class FractionRule : IRule
{
    private const string fractionRegex = @"^\d+/\d+$";

    public bool Execute(string value)
    {
        return FractionRegex().IsMatch(value);
    }

    [GeneratedRegex(fractionRegex)]
    private static partial Regex FractionRegex();
}
