using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TailwindMerge.Rules;
using TailwindMerge.Utilities;

namespace TailwindMerge;

public sealed class TwConfig
{
    public int CacheSizeValue { get; private set; }
    public string SeparatorValue { get; private set; }
    public string PrefixValue { get; private set; }
    public Dictionary<string, object> ThemeValue { get; private set; }
    public Dictionary<string, List<object>> ClassGroupsValue { get; private set; }
    public Dictionary<string, List<string>> ConflictingClassGroupsValue { get; private set; }
    public Dictionary<string, List<string>> ConflictingClassGroupModifiersValue
    {
        get;
        private set;
    }
    public List<string> OrderSensitiveModifiersValue { get; private set; }

    public TwConfig(
        int cacheSize = 500,
        string separator = ":",
        string? prefix = null,
        Dictionary<string, object>? theme = null,
        Dictionary<string, List<object>>? classGroups = null,
        Dictionary<string, List<string>>? conflictingClassGroups = null,
        Dictionary<string, List<string>>? conflictingClassGroupModifiers = null,
        List<string>? orderSensitiveModifiers = null
    )
    {
        this.CacheSizeValue = cacheSize;
        this.SeparatorValue = separator;
        this.PrefixValue = prefix ?? string.Empty;
        this.ThemeValue = theme ?? new Dictionary<string, object>();
        this.ClassGroupsValue = classGroups ?? new Dictionary<string, List<object>>();
        this.ConflictingClassGroupsValue =
            conflictingClassGroups ?? new Dictionary<string, List<string>>();
        this.ConflictingClassGroupModifiersValue =
            conflictingClassGroupModifiers ?? new Dictionary<string, List<string>>();
        this.OrderSensitiveModifiersValue = orderSensitiveModifiers ?? new List<string>();

        this.Validate();
    }

    public static TwConfig FromArray(Dictionary<string, object> config, bool extend = true)
    {
        var output = Default();

        foreach (var keyValuePair in config)
        {
            var methodName =
                char.ToLowerInvariant(keyValuePair.Key[0]) + keyValuePair.Key.Substring(1);
            var method = output.GetType().GetMethod(methodName);

            method?.Invoke(output, [keyValuePair.Value, extend]);
        }

        return output;
    }

    public TwConfig Separator(string separator)
    {
        this.SeparatorValue = separator;
        return this.Validate();
    }

    public TwConfig CacheSize(int cacheSize)
    {
        this.CacheSizeValue = cacheSize;
        return this.Validate();
    }

    public TwConfig Prefix(string prefix)
    {
        this.PrefixValue = prefix;
        return this.Validate();
    }

    public TwConfig Theme(Dictionary<string, object> theme, bool extend = true)
    {
        this.ThemeValue = extend ? Merge(this.ThemeValue, theme) : theme;
        return this.Validate();
    }

    public TwConfig ClassGroups(Dictionary<string, List<object>> classGroups, bool extend = true)
    {
        this.ClassGroupsValue = extend ? Merge(this.ClassGroupsValue, classGroups) : classGroups;
        return this.Validate();
    }

    public TwConfig ConflictingClassGroups(
        Dictionary<string, List<string>> conflictingClassGroups,
        bool extend = true
    )
    {
        this.ConflictingClassGroupsValue = extend
            ? Merge(this.ConflictingClassGroupsValue, conflictingClassGroups)
            : conflictingClassGroups;
        return this.Validate();
    }

    public TwConfig ConflictingClassGroupModifiers(
        Dictionary<string, List<string>> conflictingClassGroupModifiers,
        bool extend = true
    )
    {
        this.ConflictingClassGroupModifiersValue = extend
            ? Merge(this.ConflictingClassGroupModifiersValue, conflictingClassGroupModifiers)
            : conflictingClassGroupModifiers;
        return this.Validate();
    }

    public TwConfig OrderSensitiveModifiers(
        List<string> orderSensitiveModifiers,
        bool extend = true
    )
    {
        this.OrderSensitiveModifiersValue = extend
            ? this.OrderSensitiveModifiersValue.Concat(orderSensitiveModifiers).ToList()
            : orderSensitiveModifiers;
        return this.Validate();
    }

    private TwConfig Validate()
    {
        foreach (var kvp in this.ThemeValue)
        {
            if (!(kvp.Key is not null) || !IsClassGroup(kvp.Value))
            {
                throw new Exception("`theme` must be a dictionary of class group");
            }
        }

        foreach (var kvp in this.ClassGroupsValue)
        {
            if (!(kvp.Key is not null) || !IsClassGroup(kvp.Value))
            {
                throw new Exception(
                    "`classGroups` must be an associative dictionary of class group"
                );
            }
        }

        foreach (var kvp in this.ConflictingClassGroupsValue)
        {
            if (!(kvp.Key is not null) || !IsListOfStrings(kvp.Value))
            {
                throw new Exception(
                    "`conflictingClassGroups` must be an associative dictionary of string list"
                );
            }
        }

        foreach (var kvp in this.ConflictingClassGroupModifiersValue)
        {
            if (!(kvp.Key is not null) || !IsListOfStrings(kvp.Value))
            {
                throw new Exception(
                    "`conflictingClassGroupModifiers` must be an associative dictionary of string list"
                );
            }
        }

        return this;
    }

    private static Dictionary<T, U> Merge<T, U>(Dictionary<T, U> a, Dictionary<T, U> b)
        where T : notnull
    {
        // Merge logic here
        // This is a simple merge method. You may need to customize this method based on your specific merge logic.
        var result = new Dictionary<T, U>(a);

        foreach (var kvp in b)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }

    public static TwConfig Default() => defaultConfig.Value;

    private static readonly Lazy<TwConfig> defaultConfig =
        new(
            () =>
            {
                using var jsonConfigStream = Assembly
                    .GetAssembly(typeof(TwConfig))
                    ?.GetManifestResourceStream("TailwindMerge.Config.tailwind-default.json");

                if (jsonConfigStream is null or { Length: 0 })
                {
                    throw new Exception(
                        "Default Tailwind configuration not found. Ensure the resource is embedded correctly."
                    );
                }

                var configData =
                    JsonSerializer.Deserialize<TwConfigData>(jsonConfigStream)
                    ?? throw new InvalidOperationException("Failed to deserialize configuration");

                return FromConfigData(configData);
            },
            true
        );

    private static TwConfig FromConfigData(TwConfigData data)
    {
        var theme = ConvertConfigItems(data.Theme);
        var classGroups = ConvertClassGroups(data.ClassGroups);

        return new TwConfig(
            cacheSize: data.CacheSize,
            separator: data.Separator,
            prefix: data.Prefix,
            theme: theme,
            classGroups: classGroups,
            conflictingClassGroups: data.ConflictingClassGroups,
            conflictingClassGroupModifiers: data.ConflictingClassGroupModifiers,
            orderSensitiveModifiers: data.OrderSensitiveModifiers
        );
    }

    private static Dictionary<string, object> ConvertConfigItems(
        Dictionary<string, List<TwConfigItem>> items
    )
    {
        var result = new Dictionary<string, object>();

        foreach (var (key, value) in items)
        {
            result[key] = ConvertConfigItemList(value);
        }

        return result;
    }

    private static Dictionary<string, List<object>> ConvertClassGroups(
        Dictionary<string, List<TwConfigItem>> classGroups
    )
    {
        var result = new Dictionary<string, List<object>>();

        foreach (var (key, value) in classGroups)
        {
            result[key] = ConvertConfigItemList(value);
        }

        return result;
    }

    private static List<object> ConvertConfigItemList(List<TwConfigItem> items)
    {
        var result = new List<object>();

        foreach (var item in items)
        {
            result.Add(ConvertConfigItem(item));
        }

        return result;
    }

    private static object ConvertConfigItem(TwConfigItem item)
    {
        return item.Type switch
        {
            "rule" => RuleFactory.Create(item.Value!),
            "themeGetter" => ThemeGetterFactory.Create(item.Value!),
            "string" => item.Value!,
            "number" => int.Parse(item.Value!),
            "dictionary" => ConvertDictionary(item.Dictionary!),
            "list" => ConvertConfigItemList(item.List!),
            _ => throw new ArgumentException($"Unknown config item type: {item.Type}"),
        };
    }

    private static Dictionary<string, List<object>> ConvertDictionary(
        Dictionary<string, List<TwConfigItem>> dict
    )
    {
        var result = new Dictionary<string, List<object>>();

        foreach (var (key, value) in dict)
        {
            result[key] = ConvertConfigItemList(value);
        }

        return result;
    }

    private static bool IsListOfStrings(object value)
    {
        if (value is List<string>)
        {
            return true;
        }

        return false;
    }

    private static bool IsClassGroup(object value)
    {
        // Check if the object is a valid class group
        // Return true if valid, false otherwise
        return value is List<object> || value is IRule || value is ThemeGetter;
    }
}

// Supporting classes for JSON deserialization
internal sealed class TwConfigData
{
    [JsonPropertyName("cacheSize")]
    public int CacheSize { get; set; } = 500;

    [JsonPropertyName("separator")]
    public string Separator { get; set; } = ":";

    [JsonPropertyName("prefix")]
    public string? Prefix { get; set; }

    [JsonPropertyName("theme")]
    public Dictionary<string, List<TwConfigItem>> Theme { get; set; } = [];

    [JsonPropertyName("classGroups")]
    public Dictionary<string, List<TwConfigItem>> ClassGroups { get; set; } = [];

    [JsonPropertyName("conflictingClassGroups")]
    public Dictionary<string, List<string>> ConflictingClassGroups { get; set; } = [];

    [JsonPropertyName("conflictingClassGroupModifiers")]
    public Dictionary<string, List<string>> ConflictingClassGroupModifiers { get; set; } = [];

    [JsonPropertyName("orderSensitiveModifiers")]
    public List<string> OrderSensitiveModifiers { get; set; } = [];
}

internal sealed class TwConfigItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("dictionary")]
    public Dictionary<string, List<TwConfigItem>>? Dictionary { get; set; }

    [JsonPropertyName("list")]
    public List<TwConfigItem>? List { get; set; }
}

internal static class RuleFactory
{
    internal static object Create(string ruleName) =>
        ruleName switch
        {
            "TshirtSizeRule" => new TshirtSizeRule(),
            "ArbitraryLengthRule" => new ArbitraryLengthRule(),
            "AnyRule" => new AnyRule(),
            "AnyNonArbitraryRule" => new AnyNonArbitraryRule(),
            "ArbitraryPositionRule" => new ArbitraryPositionRule(),
            "ArbitrarySizeRule" => new ArbitrarySizeRule(),
            "ArbitraryUrlRule" => new ArbitraryUrlRule(),
            "ArbitraryShadowRule" => new ArbitraryShadowRule(),
            "IntegerRule" => new IntegerRule(),
            "LengthRule" => new LengthRule(),
            "NumberRule" => new NumberRule(),
            "PercentRule" => new PercentRule(),
            "ArbitraryValueRule" => new ArbitraryValueRule(),
            "ArbitraryNumberRule" => new ArbitraryNumberRule(),
            "ArbitraryIntegerRule" => new ArbitraryIntegerRule(),
            "ArbitraryImageRule" => new ArbitraryImageRule(),
            "FractionRule" => new FractionRule(),
            "ArbitraryVariableRule" => new ArbitraryVariableRule(),
            "ArbitraryVariableLengthRule" => new ArbitraryVariableLengthRule(),
            "ArbitraryVariablePositionRule" => new ArbitraryVariablePositionRule(),
            "ArbitraryVariableSizeRule" => new ArbitraryVariableSizeRule(),
            "ArbitraryVariableShadowRule" => new ArbitraryVariableShadowRule(),
            "ArbitraryVariableImageRule" => new ArbitraryVariableImageRule(),
            "ArbitraryVariableFamilyNameRule" => new ArbitraryVariableFamilyNameRule(),
            "ColorWithPrefixRule" => new ColorWithPrefixRule(),
            _ => new NeverRule(),
        };
}

internal static class ThemeGetterFactory
{
    internal static ThemeGetter Create(string theme) => new(theme);
}
