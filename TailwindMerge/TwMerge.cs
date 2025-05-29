using System.Text.RegularExpressions;
using TailwindMerge.Models;
using TailwindMerge.Utilities;

namespace TailwindMerge;

/// <summary>
/// High-performance Tailwind CSS class merger with conflict resolution
/// </summary>
public sealed partial class TwMerge(TwConfig config)
{
    private const string importantModifier = "!";
    private readonly LruCache<string, string> cache = new(config.CacheSizeValue);
    private readonly ClassInspector classUtilities = new(config);

    public TwMerge()
        : this(TwConfig.Default()) { }

    /// <summary>
    /// Merges Tailwind CSS classes, resolving conflicts and removing duplicates
    /// </summary>
    /// <param name="classes">Individual class strings to merge</param>
    /// <returns>Merged class string with conflicts resolved</returns>
    public string Merge(params string[] classes) => this.MergeInternal(classes.AsSpan());

    /// <summary>
    /// Merges Tailwind CSS classes from a collection
    /// </summary>
    public string Merge(IEnumerable<string> classes) => this.MergeInternal(classes);

    /// <summary>
    /// Merges classes with conditional support (objects, dictionaries, etc.)
    /// </summary>
    /// <param name="inputs">Mixed inputs: strings, dictionaries, objects</param>
    public string Merge(params object[] inputs) => this.MergeInternal(FlattenInputs(inputs));

    /// <summary>
    /// Merges classes with ReadOnlySpan for zero-allocation scenarios
    /// </summary>
    public string Merge(ReadOnlySpan<string> classes) => this.MergeInternal(classes);

    private string MergeInternal(ReadOnlySpan<string> classes)
    {
        if (classes.IsEmpty)
            return string.Empty;

        var joinedInput = JoinClasses(classes);
        return this.MergeInternal(joinedInput);
    }

    private string MergeInternal(IEnumerable<string> classes)
    {
        var joinedInput = string.Join(" ", classes.Where(c => !string.IsNullOrWhiteSpace(c)));
        return this.MergeInternal(joinedInput);
    }

    private string MergeInternal(string joinedClassList)
    {
        if (string.IsNullOrWhiteSpace(joinedClassList))
            return string.Empty;

        // Check cache first
        if (this.cache.TryGet(joinedClassList, out var cachedResult) && cachedResult is not null)
        {
            return cachedResult;
        }

        var result = this.ProcessClasses(joinedClassList);
        this.cache.Set(joinedClassList, result);
        return result;
    }

    private string ProcessClasses(string classList)
    {
        // Split and process classes
        var classes = SplitClassPattern()
            .Split(classList.Trim())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(this.DetermineClassContext)
            .ToList();

        // Filter conflicts (last class wins)
        var filteredClasses = this.FilterConflictingClasses(classes);

        return string.Join(" ", filteredClasses.Select(c => c.OriginalClassName));
    }

    private ClassContext DetermineClassContext(string originalClassName)
    {
        var modifiersContext = this.classUtilities.SplitModifiers(originalClassName);
        var classGroupId = this.classUtilities.GetClassGroupId(modifiersContext.BaseClassName);
        var hasPostfixModifier = modifiersContext.MaybePostfixModifierPosition.HasValue;

        // Handle cases where class group isn't found initially
        if (string.IsNullOrEmpty(classGroupId))
        {
            if (!modifiersContext.MaybePostfixModifierPosition.HasValue)
            {
                return new ClassContext(false, originalClassName);
            }

            // Retry without postfix modifier
            classGroupId = this.classUtilities.GetClassGroupId(modifiersContext.BaseClassName);
            if (string.IsNullOrEmpty(classGroupId))
            {
                return new ClassContext(false, originalClassName);
            }

            hasPostfixModifier = false;
        }

        var variantModifier = string.Join(
            ":",
            ClassInspector.SortModifiers(modifiersContext.Modifiers)
        );
        var modifierId = modifiersContext.HasImportantModifier
            ? variantModifier + importantModifier
            : variantModifier;

        return new ClassContext(
            IsTailwindClass: true,
            OriginalClassName: originalClassName,
            HasPostfixModifier: hasPostfixModifier,
            ModifierId: modifierId,
            ClassGroupId: classGroupId
        );
    }

    private List<ClassContext> FilterConflictingClasses(List<ClassContext> classes)
    {
        var conflictTracker = new HashSet<string>();
        var result = new List<ClassContext>(classes.Count);

        // Process in reverse order (last wins)
        for (int i = classes.Count - 1; i >= 0; i--)
        {
            var context = classes[i];

            if (!context.IsTailwindClass)
            {
                result.Add(context);
                continue;
            }

            var variantModifier = context.ModifierId?.Replace(importantModifier, "") ?? "";
            var classId = variantModifier + context.ClassGroupId;

            if (conflictTracker.Contains(classId))
            {
                continue; // Skip conflicting class
            }

            conflictTracker.Add(classId);

            // Add all conflicting groups to tracker
            var conflicts = this.classUtilities.GetConflictingClassGroupIds(
                context.ClassGroupId,
                context.HasPostfixModifier
            );

            foreach (var conflictGroup in conflicts)
            {
                conflictTracker.Add(context.ModifierId + conflictGroup);
            }

            result.Add(context);
        }

        result.Reverse(); // Restore original order
        return result;
    }

    // High-performance class joining using spans
    private static string JoinClasses(ReadOnlySpan<string> classes)
    {
        if (classes.IsEmpty)
            return string.Empty;
        if (classes.Length == 1)
            return classes[0] ?? string.Empty;

        var totalLength = 0;
        var validCount = 0;

        // Calculate total length needed
        foreach (var cls in classes)
        {
            if (!string.IsNullOrWhiteSpace(cls))
            {
                totalLength += cls.Length;
                validCount++;
            }
        }

        if (validCount == 0)
            return string.Empty;
        if (validCount == 1)
        {
            foreach (var cls in classes)
            {
                if (!string.IsNullOrWhiteSpace(cls))
                    return cls;
            }
        }

        // Add space separators
        totalLength += validCount - 1;

        return string.Create(
            totalLength,
            classes,
            (span, state) =>
            {
                var position = 0;
                var isFirst = true;

                foreach (var cls in state)
                {
                    if (string.IsNullOrWhiteSpace(cls))
                        continue;

                    if (!isFirst)
                    {
                        span[position++] = ' ';
                    }

                    cls.AsSpan().CopyTo(span[position..]);
                    position += cls.Length;
                    isFirst = false;
                }
            }
        );
    }

    // Handle mixed inputs (strings, objects, dictionaries)
    private static IEnumerable<string> FlattenInputs(object[] inputs)
    {
        foreach (var input in inputs)
        {
            switch (input)
            {
                case null:
                    continue;

                case string str when !string.IsNullOrWhiteSpace(str):
                    yield return str;
                    break;

                case IEnumerable<string> enumerable:
                    foreach (var item in enumerable.Where(s => !string.IsNullOrWhiteSpace(s)))
                        yield return item;
                    break;

                case IDictionary<string, bool> conditionalClasses:
                    foreach (var kvp in conditionalClasses.Where(kvp => kvp.Value))
                        yield return kvp.Key;
                    break;

                case IDictionary<string, object> objectDict:
                    foreach (var kvp in objectDict)
                    {
                        if (IsTruthy(kvp.Value))
                            yield return kvp.Key;
                    }
                    break;

                // Handle anonymous objects like new { "class1" = true, "class2" = false }
                default:
                    var properties = input.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(input);
                        if (IsTruthy(value))
                            yield return prop.Name;
                    }
                    break;
            }
        }
    }

    private static bool IsTruthy(object? value) =>
        value switch
        {
            null => false,
            bool b => b,
            string s => !string.IsNullOrWhiteSpace(s),
            int i => i != 0,
            double d => d != 0.0,
            _ => true,
        };

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex SplitClassPattern();
}
