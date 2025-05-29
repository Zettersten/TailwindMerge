namespace TailwindMerge.Models;

public sealed record ClassContext(
    bool IsTailwindClass,
    string OriginalClassName,
    bool HasPostfixModifier = false,
    string? ModifierId = null,
    string? ClassGroupId = null
);
