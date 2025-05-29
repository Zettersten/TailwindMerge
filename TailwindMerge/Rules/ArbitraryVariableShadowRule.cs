using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryVariableShadowRule : IRule
{
    private const string variableShadowRegex = @"^\[(--[\w-]+|var\(--[\w-]+\))\]$";

    public bool Execute(string value)
    {
        if (!VariableShadowRegex().IsMatch(value))
            return false;

        // Extract the variable part and validate it could be a shadow
        var match = VariableShadowRegex().Match(value);
        if (match.Success)
        {
            var variable = match.Groups[1].Value;
            // CSS custom properties can represent any type, so we accept any valid variable
            return variable.StartsWith("--") || variable.StartsWith("var(--");
        }

        return false;
    }

    [GeneratedRegex(variableShadowRegex)]
    private static partial Regex VariableShadowRegex();
}
