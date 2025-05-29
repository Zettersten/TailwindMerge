namespace TailwindMerge.Rules;

public sealed class AnyRule : IRule
{
    public bool Execute(string value)
    {
        return true;
    }
}
