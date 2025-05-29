using TailwindMerge.Rules;

namespace TailwindMerge.Models;

public sealed record ClassValidator(string ClassGroupId, IRule Rule);
