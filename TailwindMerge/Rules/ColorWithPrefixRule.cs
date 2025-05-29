namespace TailwindMerge.Rules;

/// <summary>
/// Matches color values that start with a dash and contain a color name
/// This distinguishes border colors from border widths
/// </summary>
public sealed class ColorWithPrefixRule : IRule
{
    public bool Execute(string classPart)
    {
        // Must start with a dash and have a color name pattern
        // Color names are alphabetic (red, blue, green, etc.)
        if (!classPart.StartsWith("-") || classPart.Length <= 1)
            return false;

        var colorPart = classPart.Substring(1);

        // Check if it starts with a letter (color names like "red", "blue", etc.)
        return !string.IsNullOrEmpty(colorPart) && char.IsLetter(colorPart[0]);
    }
}
