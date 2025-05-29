using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed class AnyNonArbitraryRule : IRule
{
    public bool Execute(string value)
    {
        // Matches any value except arbitrary values (those in square brackets)
        return !value.StartsWith('[') || !value.EndsWith(']');
    }
}
