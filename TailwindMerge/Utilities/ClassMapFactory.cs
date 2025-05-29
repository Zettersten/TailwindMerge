using System.Buffers;
using TailwindMerge.Models;
using TailwindMerge.Rules;

namespace TailwindMerge.Utilities;

/// <summary>
/// Factory for creating hierarchical class maps from Tailwind configuration
/// </summary>
public static class ClassMapFactory
{
    public static ClassPart Create(TwConfig config) => new ClassMapBuilder(config).Build();
}

/// <summary>
/// Builds class map using builder pattern with separated concerns
/// </summary>
internal sealed class ClassMapBuilder(TwConfig config)
{
    private readonly TwConfig config = config;
    private readonly ClassDefinitionProcessor processor = new(config.ThemeValue);
    private readonly PrefixApplier prefixApplier = new(config.PrefixValue);

    public ClassPart Build()
    {
        var classMap = new ClassPart();
        var prefixedGroups = this.prefixApplier.Apply(this.config.ClassGroupsValue);

        foreach (var (groupId, classGroup) in prefixedGroups)
        {
            this.processor.ProcessGroup(classGroup, classMap, groupId);
        }

        return classMap;
    }
}

/// <summary>
/// Processes different types of class definitions using modern pattern matching
/// </summary>
internal sealed class ClassDefinitionProcessor(Dictionary<string, object> theme)
{
    private readonly Dictionary<string, object> theme = theme;
    private readonly ClassPathNavigator navigator = new();

    public void ProcessGroup(List<object> classGroup, ClassPart classPart, string classGroupId)
    {
        foreach (var definition in classGroup)
        {
            this.ProcessDefinition(definition, classPart, classGroupId);
        }
    }

    private void ProcessDefinition(object definition, ClassPart classPart, string classGroupId)
    {
        switch (definition)
        {
            case Dictionary<string, List<object>> nestedDict:
                this.ProcessNestedDictionary(nestedDict, classPart, classGroupId);
                break;

            case string classString:
                this.ProcessStringDefinition(classString, classPart, classGroupId);
                break;

            case ThemeGetter themeGetter:
                this.ProcessThemeGetter(themeGetter, classPart, classGroupId);
                break;

            case IRule rule:
                ProcessRule(rule, classPart, classGroupId);
                break;
        }
    }

    private void ProcessNestedDictionary(
        Dictionary<string, List<object>> nestedDict,
        ClassPart classPart,
        string classGroupId
    )
    {
        foreach (var (key, nestedGroup) in nestedDict)
        {
            var nestedPart = this.navigator.NavigateToPath(classPart, key);
            this.ProcessGroup(nestedGroup, nestedPart, classGroupId);
        }
    }

    private void ProcessStringDefinition(
        string classString,
        ClassPart classPart,
        string classGroupId
    )
    {
        var targetPart = string.IsNullOrEmpty(classString)
            ? classPart
            : this.navigator.NavigateToPath(classPart, classString);

        targetPart.SetClassGroupId(classGroupId);
    }

    private void ProcessThemeGetter(
        ThemeGetter themeGetter,
        ClassPart classPart,
        string classGroupId
    )
    {
        var themeResult = themeGetter.Execute(this.theme);
        this.ProcessGroup(themeResult, classPart, classGroupId);
    }

    private static void ProcessRule(IRule rule, ClassPart classPart, string classGroupId) =>
        classPart.Validators.Add(new ClassValidator(classGroupId, rule));
}

/// <summary>
/// High-performance class path navigation using modern C# memory features
/// </summary>
internal sealed class ClassPathNavigator
{
    private const char pathSeparator = '-';

    public static ClassPart NavigateToPath(ClassPart startPart, ReadOnlySpan<char> path)
    {
        if (path.IsEmpty)
            return startPart;

        var currentPart = startPart;
        var pathSpan = path;

        while (!pathSpan.IsEmpty)
        {
            var separatorIndex = pathSpan.IndexOf(pathSeparator);
            var segment = separatorIndex == -1 ? pathSpan : pathSpan[..separatorIndex];

            currentPart = GetOrCreatePart(currentPart, segment);

            pathSpan =
                separatorIndex == -1 ? ReadOnlySpan<char>.Empty : pathSpan[(separatorIndex + 1)..];
        }

        return currentPart;
    }

    public ClassPart NavigateToPath(ClassPart startPart, string path) =>
        NavigateToPath(startPart, path.AsSpan());

    private static ClassPart GetOrCreatePart(ClassPart parent, ReadOnlySpan<char> segment)
    {
        var segmentString = segment.ToString();

        if (!parent.NextPart.TryGetValue(segmentString, out var existingPart))
        {
            existingPart = new ClassPart();
            parent.NextPart[segmentString] = existingPart;
        }

        return existingPart;
    }
}

/// <summary>
/// Handles prefix application with optimized memory allocation
/// </summary>
internal sealed class PrefixApplier(string? prefix)
{
    private readonly string? prefix = prefix;

    public Dictionary<string, List<object>> Apply(Dictionary<string, List<object>> classGroups) =>
        string.IsNullOrEmpty(this.prefix) ? classGroups : this.ApplyPrefixToGroups(classGroups);

    private Dictionary<string, List<object>> ApplyPrefixToGroups(
        Dictionary<string, List<object>> classGroups
    )
    {
        var result = new Dictionary<string, List<object>>(classGroups.Count);

        foreach (var (groupId, classGroup) in classGroups)
        {
            result[groupId] = classGroup.ConvertAll(this.ApplyPrefixToDefinition);
        }

        return result;
    }

    private object ApplyPrefixToDefinition(object definition) =>
        definition switch
        {
            string str => this.prefix + str,
            Dictionary<string, object> dict => this.ApplyPrefixToDict(dict),
            _ => definition
        };

    private Dictionary<string, object> ApplyPrefixToDict(Dictionary<string, object> dict)
    {
        var result = new Dictionary<string, object>(dict.Count);

        foreach (var (key, value) in dict)
        {
            result[this.prefix + key] = value;
        }

        return result;
    }
}
