namespace TailwindMerge.Rules;

public sealed class ArbitraryImageRule : IRule
{
    public bool Execute(string value)
    {
        return value.StartsWith('[')
            && value.EndsWith(']')
            && (
                value.Contains("url(")
                || value.Contains("linear-gradient(")
                || value.Contains("radial-gradient(")
                || value.Contains("conic-gradient(")
            );
    }
}
