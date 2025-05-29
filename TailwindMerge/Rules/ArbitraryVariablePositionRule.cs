using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryVariablePositionRule : IRule
{
    private const string variablePositionRegex = @"^\[(--[\w-]+|var\(--[\w-]+\))\]$";

    public bool Execute(string value)
    {
        if (!VariablePositionRegex().IsMatch(value))
            return false;

        // Extract the variable part and validate it could be a position
        var match = VariablePositionRegex().Match(value);
        if (match.Success)
        {
            var variable = match.Groups[1].Value;
            // CSS custom properties can represent any type, so we accept any valid variable
            return variable.StartsWith("--") || variable.StartsWith("var(--");
        }

        return false;
    }

    [GeneratedRegex(variablePositionRegex)]
    private static partial Regex VariablePositionRegex();
}
