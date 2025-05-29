using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryVariableLengthRule : IRule
{
    private const string variableLengthRegex = @"^\[(--[\w-]+|var\(--[\w-]+\))\]$";

    public bool Execute(string value)
    {
        if (!VariableLengthRegex().IsMatch(value))
            return false;

        // Extract the variable part and validate it could be a length
        var match = VariableLengthRegex().Match(value);
        if (match.Success)
        {
            var variable = match.Groups[1].Value;
            // CSS custom properties can represent any type, so we accept any valid variable
            return variable.StartsWith("--") || variable.StartsWith("var(--");
        }

        return false;
    }

    [GeneratedRegex(variableLengthRegex)]
    private static partial Regex VariableLengthRegex();
}
