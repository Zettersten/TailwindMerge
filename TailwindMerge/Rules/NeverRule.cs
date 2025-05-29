namespace TailwindMerge.Rules;

public sealed class NeverRule : IRule
{
    public bool Execute(string value)
    {
        return false;
    }
}
