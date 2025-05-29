using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public partial class ArbitraryValueRule : IRule
{
    private const string regexPattern = @"^\[(?:([a-z-]+):)?(.+)]$";

    protected virtual string? Parameter { get; }

    public bool Execute(string value)
    {
        var matches = ArbValueRegexPattern().Match(value);

        if (matches.Success)
        {
            if (!string.IsNullOrEmpty(matches.Groups[1].Value))
            {
                return matches.Groups[1].Value == this.Parameter;
            }

            return this.TestValue(matches.Groups[2].Value);
        }

        return false;
    }

    protected virtual bool TestValue(string value)
    {
        return true;
    }

    [GeneratedRegex(regexPattern)]
    private static partial Regex ArbValueRegexPattern();
}
