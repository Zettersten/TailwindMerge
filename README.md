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
dotnet add package TailwindMerge
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

## Configuration Options

You can customize merging behavior by creating a custom `TwConfig`:

```csharp
var config = new TwConfig()
    .CacheSize(1000)
    .ClassGroups(customClassGroups)
    .Separator(":")
    .Prefix("tw-")
    .ConflictingClassGroups(customConflicts);

var twMerge = new TwMerge(config);
```


## Example Usage

```csharp
var twMerge = new TwMerge();
var result = twMerge.Merge( "sm:text-xs", "sm:text-lg", "md:text-base", "md:text-xl", "p-2", "p-4", "hover:bg-red-500", "hover:bg-blue-500" ); // result == "sm:text-lg md:text-xl p-4 hover:bg-blue-500"
```

## Requirements

- .NET 9.0 or higher

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.
