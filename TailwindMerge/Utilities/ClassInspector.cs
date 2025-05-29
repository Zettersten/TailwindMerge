using System.Text.RegularExpressions;
using TailwindMerge.Models;

namespace TailwindMerge.Utilities;

public sealed partial class ClassInspector(TwConfig config)
{
    private const string importantModifier = "!";
    private const string arbitraryPropertyRegex = @"^\[(.+)]";
    private readonly ClassPart classMap = ClassMapFactory.Create(config);
    private readonly TwConfig config = config;

    public string? GetClassGroupId(string className)
    {
        var classParts = className.Split('-');

        // Classes like "-inset-1" produce an empty string as the first classPart.
        // We assume that classes for negative values are used correctly and remove it from classParts.
        if (classParts[0] == "" && classParts.Length != 1)
        {
            Array.Copy(classParts, 1, classParts, 0, classParts.Length - 1);
            Array.Resize(ref classParts, classParts.Length - 1);
        }

        return GetGroupRecursive(classParts, this.classMap)
            ?? GetGroupIdForArbitraryProperty(className);
    }

    public List<string> GetConflictingClassGroupIds(string? classGroupId, bool hasPostfixModifier)
    {
        var conflicts = classGroupId is not null
            ? this.config.ConflictingClassGroupsValue.GetValueOrDefault(classGroupId) ?? []
            : [];

        if (
            hasPostfixModifier
            && classGroupId is not null
            && this.config.ConflictingClassGroupModifiersValue.TryGetValue(
                classGroupId,
                out List<string>? value
            )
        )
        {
            conflicts.AddRange(value);
        }

        return conflicts;
    }

    public ClassModifiersContext SplitModifiers(string className)
    {
        var separator = this.config.SeparatorValue;
        var modifiers = new List<string>();
        var bracketDepth = 0;
        var modifierStart = 0;
        int? postfixModifierPosition = null;

        for (int index = 0; index < className.Length; index++)
        {
            var currentCharacter = className[index];

            if (bracketDepth == 0)
            {
                if (
                    currentCharacter == separator[0]
                    && (
                        separator.Length == 1
                        || className.Substring(index, separator.Length) == separator
                    )
                )
                {
                    modifiers.Add(className.Substring(modifierStart, index - modifierStart));
                    modifierStart = index + separator.Length;
                    continue;
                }

                if (currentCharacter == '/')
                {
                    postfixModifierPosition = index;
                    continue;
                }
            }

            if (currentCharacter == '[')
            {
                bracketDepth++;
            }
            else if (currentCharacter == ']')
            {
                bracketDepth--;
            }
        }

        var baseClassNameWithImportantModifier =
            modifiers.Count == 0 ? className : className[modifierStart..];

        var hasImportantModifier = baseClassNameWithImportantModifier.StartsWith(importantModifier);
        var baseClassName = hasImportantModifier
            ? baseClassNameWithImportantModifier.Substring(1)
            : baseClassNameWithImportantModifier;
        int? maybePostfixModifierPosition =
            postfixModifierPosition > modifierStart
                ? postfixModifierPosition - modifierStart
                : null;

        return new ClassModifiersContext(
            modifiers,
            hasImportantModifier,
            baseClassName,
            maybePostfixModifierPosition
        );
    }

    public static IReadOnlyList<string> SortModifiers(IReadOnlyList<string> modifiers)
    {
        if (modifiers.Count <= 1)
        {
            return modifiers;
        }

        var sortedModifiers = new List<string>();
        var unsortedModifiers = new List<string>();

        foreach (var modifier in modifiers)
        {
            var isArbitraryVariant = modifier[0] == '[';
            if (isArbitraryVariant)
            {
                unsortedModifiers.Sort();
                sortedModifiers.AddRange(unsortedModifiers);
                sortedModifiers.Add(modifier);
                unsortedModifiers.Clear();
            }
            else
            {
                unsortedModifiers.Add(modifier);
            }
        }

        unsortedModifiers.Sort();
        sortedModifiers.AddRange(unsortedModifiers);

        return sortedModifiers;
    }

    private static string? GetGroupRecursive(string[] classParts, ClassPart classPart)
    {
        if (classParts.Length == 0)
        {
            return classPart.ClassGroupId;
        }

        var currentClassPart = classParts[0];

        if (classPart.NextPart.TryGetValue(currentClassPart, out var nextClassPartObject))
        {
            var classGroupFromNextClassPart = GetGroupRecursive(
                classParts[1..],
                nextClassPartObject
            );

            if (classGroupFromNextClassPart != null)
            {
                return classGroupFromNextClassPart;
            }
        }

        if (classPart.Validators.Count == 0)
        {
            return null;
        }

        var classRest = string.Join("-", classParts);

        foreach (var classValidator in classPart.Validators)
        {
            if (classValidator.Rule.Execute(classRest))
            {
                return classValidator.ClassGroupId;
            }
        }

        return null;
    }

    private static string? GetGroupIdForArbitraryProperty(string className)
    {
        var match = ArbPropPattern().Match(className);

        if (match.Success)
        {
            var arbitraryPropertyClassName = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(arbitraryPropertyClassName))
            {
                var property = arbitraryPropertyClassName.Substring(
                    0,
                    arbitraryPropertyClassName.IndexOf(':')
                );
                return "arbitrary.." + property;
            }
        }

        return null;
    }

    [GeneratedRegex(arbitraryPropertyRegex)]
    private static partial Regex ArbPropPattern();
}
