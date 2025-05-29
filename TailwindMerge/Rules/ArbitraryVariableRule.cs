using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class ArbitraryVariableRule : IRule
{
    private const string variableRegex = @"^\[--[\w-]+\]$|^var\(--[\w-]+\)$";

    public bool Execute(string value)
    {
        return VariableRegex().IsMatch(value);
    }

    [GeneratedRegex(variableRegex)]
    private static partial Regex VariableRegex();
}
