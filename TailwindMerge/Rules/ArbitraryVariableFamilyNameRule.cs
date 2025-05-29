using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryVariableFamilyNameRule : IRule
{
    private const string variableFamilyNameRegex = @"^\[(--[\w-]+|var\(--[\w-]+\))\]$";

    public bool Execute(string value)
    {
        if (!VariableFamilyNameRegex().IsMatch(value))
            return false;

        // Extract the variable part and validate it could be a font family name
        var match = VariableFamilyNameRegex().Match(value);
        if (match.Success)
        {
            var variable = match.Groups[1].Value;
            // CSS custom properties can represent any type, so we accept any valid variable
            return variable.StartsWith("--") || variable.StartsWith("var(--");
        }

        return false;
    }

    [GeneratedRegex(variableFamilyNameRegex)]
    private static partial Regex VariableFamilyNameRegex();
}
