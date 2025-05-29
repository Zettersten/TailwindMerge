namespace TailwindMerge.Models;

public sealed record ClassModifiersContext(
    IReadOnlyList<string> Modifiers,
    bool HasImportantModifier,
    string BaseClassName,
    int? MaybePostfixModifierPosition
);
