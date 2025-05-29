using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryVariableSizeRule : IRule
{
    private const string variableSizeRegex = @"^\[(--[\w-]+|var\(--[\w-]+\))\]$";

    public bool Execute(string value)
    {
        if (!VariableSizeRegex().IsMatch(value))
            return false;

        // Extract the variable part and validate it could be a size
        var match = VariableSizeRegex().Match(value);
        if (match.Success)
        {
            var variable = match.Groups[1].Value;
            // CSS custom properties can represent any type, so we accept any valid variable
            return variable.StartsWith("--") || variable.StartsWith("var(--");
        }

        return false;
    }

    [GeneratedRegex(variableSizeRegex)]
    private static partial Regex VariableSizeRegex();
}
