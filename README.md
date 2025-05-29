![TailwindMerge Logo](https://raw.githubusercontent.com/Zettersten/TailwindMerge/main/icon.png)


# TailwindMerge 🍃

![NuGet Version](https://img.shields.io/nuget/v/TwMerge)

A high-performance .NET library for merging Tailwind CSS class names with full conflict resolution, deduplication, and support for all major Tailwind v4 utilities, responsive breakpoints, and modifiers.

## Features

- **Tailwind v4 Support**: Handles all major class groups, including text, font, spacing, layout, color, and more
- **Conflict Resolution**: Automatically removes conflicting or duplicate classes, keeping only the last in each group
- **Responsive & Modifier Aware**: Correctly merges classes with responsive prefixes (sm:, md:, etc.) and modifiers (hover:, focus:, dark:, etc.)
- **Arbitrary Value Support**: Handles arbitrary values and custom properties (e.g., `w-[32px]`)
- **High Performance**: Zero-allocation merging with LRU caching for repeated merges
- **Customizable**: Extend or override class groups and conflict rules via configuration

## Installation

```
dotnet add package TwMerge
```

## Quick Start

1. Add TailwindMerge to your project and create a merger instance:

```csharp
using TailwindMerge;
 
// Create instance with default config
var twMerge = new TwMerge(); 
```

2. Merge Tailwind class names:

```csharp
// merged == "w-4 hover:w-4 text-blue-500"
var merged = twMerge.Merge("w-3", "w-4", "hover:w-3", "hover:w-4", "text-red-500", "text-blue-500"); 
```

## Core Components

### TwMerge

The main entry point for merging Tailwind CSS class names, resolving conflicts and removing duplicates.

### TwConfig

Configures class groups, conflict rules, theme, and other options. Use `TwConfig.Default()` for standard Tailwind v4 support, or customize as needed.

## TwConfig Properties Reference

Each property in `TwConfig` serves a specific purpose in controlling how Tailwind classes are merged and resolved:

### CacheSize
**Type**: `int` (default: 500)  
**Purpose**: Controls the size of the LRU cache for memoizing merge results.

```csharp
var config = new TwConfig().CacheSize(1000); // Increase cache for better performance
```

**When to customize**: 
- Increase for applications with many repeated class combinations
- Decrease for memory-constrained environments
- Set to 0 to disable caching entirely

### Separator
**Type**: `string` (default: ":")  
**Purpose**: Defines the character(s) used to separate modifiers from base classes.

```csharp
var config = new TwConfig().Separator("__"); // Custom separator
// Now supports: "hover__bg-blue-500" instead of "hover:bg-blue-500"
```

**When to customize**: 
- When using a custom Tailwind configuration with different separator
- To avoid conflicts with CSS-in-JS libraries that use colons

### Prefix
**Type**: `string` (default: "")  
**Purpose**: Adds a prefix to all Tailwind class names for namespacing.

```csharp
var config = new TwConfig().Prefix("tw-");
// Supports: "tw-bg-blue-500", "tw-hover:bg-red-500"
```

**When to customize**: 
- When using Tailwind's `prefix` config option
- To avoid conflicts with existing CSS frameworks
- For component library isolation

### Theme
**Type**: `Dictionary<string, object>`  
**Purpose**: Defines theme values that can be referenced by class groups (colors, spacing, sizes, etc.).

```csharp
var customTheme = new Dictionary<string, object>
{
    ["colors"] = new List<object> { /* custom color definitions */ },
    ["spacing"] = new List<object> { /* custom spacing values */ }
};
var config = new TwConfig().Theme(customTheme);
```

**When to customize**: 
- When extending Tailwind's default theme with custom values
- When using custom color palettes or spacing scales
- For design system integration

### ClassGroups
**Type**: `Dictionary<string, List<object>>`  
**Purpose**: Defines all available Tailwind class groups and their valid class patterns.

```csharp
var customClassGroups = new Dictionary<string, List<object>>
{
    ["custom-size"] = new List<object>
    {
        new Dictionary<string, List<object>>
        {
            ["size"] = new List<object> { "tiny", "huge", "massive" }
        }
    }
};
var config = new TwConfig().ClassGroups(customClassGroups, extend: true);
```

**When to customize**: 
- Adding custom utility classes from Tailwind plugins
- Supporting company-specific design tokens
- Integrating with custom CSS frameworks

### ConflictingClassGroups
**Type**: `Dictionary<string, List<string>>`  
**Purpose**: Defines which class groups conflict with each other (e.g., `p-4` conflicts with `px-2`).

```csharp
var customConflicts = new Dictionary<string, List<string>>
{
    ["custom-margin"] = new List<string> { "margin", "margin-x", "margin-y" },
    ["custom-layout"] = new List<string> { "display", "position" }
};
var config = new TwConfig().ConflictingClassGroups(customConflicts, extend: true);
```

**When to customize**: 
- When adding custom utilities that should conflict with existing ones
- For design system rules (e.g., preventing certain class combinations)
- When creating semantic utility groups

### ConflictingClassGroupModifiers
**Type**: `Dictionary<string, List<string>>`  
**Purpose**: Defines additional conflicts that occur when classes have postfix modifiers (e.g., `text-lg/tight` conflicts with `leading-*`).

```csharp
var modifierConflicts = new Dictionary<string, List<string>>
{
    ["font-size"] = new List<string> { "leading", "line-height" },
    ["custom-text"] = new List<string> { "tracking", "letter-spacing" }
};
var config = new TwConfig().ConflictingClassGroupModifiers(modifierConflicts);
```

**When to customize**: 
- When custom utilities should conflict only when used with postfix modifiers
- For advanced typography or spacing utilities
- When implementing complex design system rules

### OrderSensitiveModifiers
**Type**: `List<string>`  
**Purpose**: Specifies modifiers where order matters for CSS specificity (e.g., `before:`, `after:`, `first-letter:`).

```csharp
var orderSensitive = new List<string> 
{ 
    "before", "after", "first-letter", "first-line", 
    "custom-pseudo", "custom-element" 
};
var config = new TwConfig().OrderSensitiveModifiers(orderSensitive);
```

**When to customize**: 
- When adding custom pseudo-element utilities
- For CSS-in-JS integration where selector order matters
- When implementing advanced styling patterns

## Custom Implementation Examples

### 1. Design System Integration

```csharp
// For a company design system with custom tokens
var designSystemConfig = new TwConfig()
    .Prefix("ds-")
    .Theme(new Dictionary<string, object>
    {
        ["colors"] = new List<object> 
        { 
            "brand-primary", "brand-secondary", "accent-blue", "accent-green" 
        },
        ["spacing"] = new List<object> 
        { 
            "micro", "tiny", "small", "medium", "large", "xlarge", "huge" 
        }
    })
    .ClassGroups(new Dictionary<string, List<object>>
    {
        ["brand-colors"] = new List<object>
        {
            new Dictionary<string, List<object>>
            {
                ["bg"] = new List<object> { "brand-primary", "brand-secondary" },
                ["text"] = new List<object> { "brand-primary", "brand-secondary" }
            }
        }
    }, extend: true)
    .ConflictingClassGroups(new Dictionary<string, List<string>>
    {
        ["brand-colors"] = new List<string> { "bg-color", "text-color" }
    }, extend: true);

var dstwMerge = new TwMerge(designSystemConfig);
var result = dstwMerge.Merge("ds-bg-brand-primary", "ds-bg-accent-blue");
// Result: "ds-bg-accent-blue" (brand colors conflict with regular colors)
```

### 2. Plugin Integration

```csharp
// Supporting Tailwind plugins like @tailwindcss/typography
var typographyConfig = new TwConfig()
    .ClassGroups(new Dictionary<string, List<object>>
    {
        ["prose"] = new List<object>
        {
            "prose",
            new Dictionary<string, List<object>>
            {
                ["prose"] = new List<object> { "sm", "base", "lg", "xl", "2xl" }
            }
        },
        ["prose-colors"] = new List<object>
        {
            new Dictionary<string, List<object>>
            {
                ["prose"] = new List<object> 
                { 
                    "gray", "red", "yellow", "green", "blue", "indigo", "purple", "pink" 
                }
            }
        }
    }, extend: true)
    .ConflictingClassGroups(new Dictionary<string, List<string>>
    {
        ["prose"] = new List<string> { "prose-colors" }
    }, extend: true);

var proseTwMerge = new TwMerge(typographyConfig);
var result = proseTwMerge.Merge("prose", "prose-lg", "prose-gray", "prose-blue");
// Result: "prose-lg prose-blue"
```

### 3. Framework-Specific Setup

```csharp
// For React/Blazor component libraries
var componentConfig = new TwConfig()
    .Separator("__")  // Avoid conflicts with CSS-in-JS
    .CacheSize(2000)  // Higher cache for component reuse
    .ConflictingClassGroupModifiers(new Dictionary<string, List<string>>
    {
        ["component-size"] = new List<string> { "padding", "margin", "gap" }
    })
    .OrderSensitiveModifiers(new List<string> 
    { 
        "before", "after", "hover", "focus", "active", "group-hover" 
    });

var componentTwMerge = new TwMerge(componentConfig);
```

### 4. Performance-Optimized Setup

```csharp
// For high-frequency merging scenarios
var performanceConfig = new TwConfig()
    .CacheSize(5000)  // Large cache
    .Theme(minimalTheme)  // Reduced theme for faster lookups
    .ClassGroups(essentialClassGroups);  // Only required class groups

var fastTwMerge = new TwMerge(performanceConfig);
```

## When to Use Custom Configurations

### **Extend Default Config When**:
- Adding custom utilities from Tailwind plugins
- Supporting additional design tokens
- Integrating with existing component libraries
- Adding company-specific styling patterns

### **Override Default Config When**:
- Using a completely different design system
- Heavy customization of Tailwind's default classes
- Performance optimization for specific use cases
- Legacy CSS framework migration

### **Configuration Best Practices**:
1. **Always extend rather than replace** when possible to maintain Tailwind compatibility
2. **Use consistent naming conventions** for custom class groups
3. **Test conflict resolution** thoroughly when adding custom conflicts
4. **Document custom configurations** for team members
5. **Consider performance impact** of large theme objects or class groups

## Requirements

- .NET 9.0 or higher

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.
