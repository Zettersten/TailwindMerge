using System;

namespace TailwindMerge.Rules;

public interface IRule
{
    bool Execute(string value);
}
