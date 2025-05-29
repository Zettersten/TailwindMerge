using System.Text.RegularExpressions;

namespace TailwindMerge.Rules;

public sealed partial class TshirtSizeRule : IRule
{
    private const string regexPattern = @"^(\d+(\.\d+)?)?(xs|sm|md|lg|xl)$";

    public bool Execute(string value)
    {
        return TshirtPattern().IsMatch(value);
    }

    [GeneratedRegex(regexPattern)]
    private static partial Regex TshirtPattern();
}
