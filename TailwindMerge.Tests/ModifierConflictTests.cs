using TailwindMerge.Utilities;

namespace TailwindMerge.Tests;

public class ModifierConflictTests
{
    public static readonly TwMerge TwMerge = new();

    [Fact]
    public void Should_Resolve_Same_Group_Conflicts_With_Same_Modifier()
    {
        // These should work - classes with same modifier and same group should conflict
        var result1 = TwMerge.Merge("sm:text-xs", "sm:text-lg");
        Assert.Equal("sm:text-lg", result1);

        var result2 = TwMerge.Merge("md:text-base", "md:text-2xl");
        Assert.Equal("md:text-2xl", result2);

        var result3 = TwMerge.Merge("lg:text-sm", "lg:text-xl");
        Assert.Equal("lg:text-xl", result3);
    }

    [Fact]
    public void Should_Preserve_Different_Groups_With_Same_Modifier()
    {
        // These should NOT conflict - different groups should coexist
        var result1 = TwMerge.Merge("sm:text-lg", "sm:text-white");
        Assert.Equal("sm:text-lg sm:text-white", result1);

        var result2 = TwMerge.Merge("md:text-center", "md:text-red-500");
        Assert.Equal("md:text-center md:text-red-500", result2);
    }

    [Fact]
    public void Should_Preserve_Same_Group_With_Different_Modifiers()
    {
        // These should NOT conflict - different modifiers should coexist
        var result1 = TwMerge.Merge("sm:text-lg", "md:text-xl");
        Assert.Equal("sm:text-lg md:text-xl", result1);

        var result2 = TwMerge.Merge("hover:text-white", "focus:text-blue-500");
        Assert.Equal("hover:text-white focus:text-blue-500", result2);
    }

    [Fact]
    public void Should_Resolve_Complex_Mixed_Modifier_Conflicts()
    {
        // Complex case from failing test
        var result = TwMerge.Merge(
            "sm:text-xs", // text-size group with sm modifier
            "md:text-lg", // text-size group with md modifier
            "sm:text-base", // text-size group with sm modifier (should replace sm:text-xs)
            "md:text-xl" // text-size group with md modifier (should replace md:text-lg)
        );
        Assert.Equal("sm:text-base md:text-xl", result);
    }

    [Fact]
    public void Should_Identify_Class_Groups_With_Modifiers()
    {
        // Test the core issue: GetClassGroupId should work with base class names from modifiers
        var inspector = new ClassInspector(TwConfig.Default());

        // Test modifier parsing and base class group identification
        var smTextLg = inspector.SplitModifiers("sm:text-lg");
        var mdTextXl = inspector.SplitModifiers("md:text-xl");
        var smTextWhite = inspector.SplitModifiers("sm:text-white");
        var mdTextCenter = inspector.SplitModifiers("md:text-center");
        var smW4 = inspector.SplitModifiers("sm:w-4");
        var lgP2 = inspector.SplitModifiers("lg:p-2");

        // These should return the correct group IDs for base class names
        Assert.Equal("text-size", inspector.GetClassGroupId(smTextLg.BaseClassName));
        Assert.Equal("text-size", inspector.GetClassGroupId(mdTextXl.BaseClassName));
        Assert.Equal("text-color", inspector.GetClassGroupId(smTextWhite.BaseClassName));
        Assert.Equal("text-align", inspector.GetClassGroupId(mdTextCenter.BaseClassName));
        Assert.Equal("w", inspector.GetClassGroupId(smW4.BaseClassName));
        Assert.Equal("p", inspector.GetClassGroupId(lgP2.BaseClassName));

        // Test that modifiers are correctly parsed
        Assert.Equal(new[] { "sm" }, smTextLg.Modifiers);
        Assert.Equal(new[] { "md" }, mdTextXl.Modifiers);
        Assert.Equal("text-lg", smTextLg.BaseClassName);
        Assert.Equal("text-xl", mdTextXl.BaseClassName);
    }

    [Fact]
    public void Should_Handle_Arbitrary_Values_With_Modifiers()
    {
        // Test arbitrary values with modifiers
        var result1 = TwMerge.Merge("sm:text-[14px]", "sm:text-[16px]");
        Assert.Equal("sm:text-[16px]", result1);

        var result2 = TwMerge.Merge("md:leading-[1.2]", "md:leading-[1.5]");
        Assert.Equal("md:leading-[1.5]", result2);

        // Mixed arbitrary and standard
        var result3 = TwMerge.Merge("sm:text-lg", "sm:text-[18px]");
        Assert.Equal("sm:text-[18px]", result3);
    }

    [Fact]
    public void Should_Debug_Class_Group_Detection_Algorithm()
    {
        // Debug test to understand the full TwMerge workflow
        var inspector = new ClassInspector(TwConfig.Default());

        // Test without modifiers (these should work)
        var group1 = inspector.GetClassGroupId("text-lg");
        var group2 = inspector.GetClassGroupId("text-white");
        var group3 = inspector.GetClassGroupId("w-4");

        // Test with modifiers by properly parsing them first
        var mod4 = inspector.SplitModifiers("sm:text-lg");
        var mod5 = inspector.SplitModifiers("md:text-white");
        var mod6 = inspector.SplitModifiers("sm:w-4");

        var group4 = inspector.GetClassGroupId(mod4.BaseClassName);
        var group5 = inspector.GetClassGroupId(mod5.BaseClassName);
        var group6 = inspector.GetClassGroupId(mod6.BaseClassName);

        // Test the core issue: arbitrary values
        var arbGroup1 = inspector.GetClassGroupId("text-[58px]");
        var arbGroup2 = inspector.GetClassGroupId("leading-[0.9]");

        // Output for debugging
        Console.WriteLine($"text-lg group: '{group1}'");
        Console.WriteLine($"text-white group: '{group2}'");
        Console.WriteLine($"w-4 group: '{group3}'");
        Console.WriteLine($"sm:text-lg -> text-lg group: '{group4}'");
        Console.WriteLine($"md:text-white -> text-white group: '{group5}'");
        Console.WriteLine($"sm:w-4 -> w-4 group: '{group6}'");
        Console.WriteLine($"text-[58px] group: '{arbGroup1}' (should be 'text-size')");
        Console.WriteLine($"leading-[0.9] group: '{arbGroup2}' (should be 'leading')");

        // The basic functionality should work
        Assert.NotNull(group1);
        Assert.NotNull(group2);
        Assert.NotNull(group3);
        Assert.NotNull(group4);
        Assert.NotNull(group5);
        Assert.NotNull(group6);

        // The core issue: arbitrary values classification
        Assert.Equal("text-size", arbGroup1); // This might fail - showing the core bug
        Assert.Equal("leading", arbGroup2); // This might fail - showing the core bug
    }

    [Fact]
    public void Should_Classify_Arbitrary_Values_Correctly()
    {
        // Test the core issue: arbitrary values should be classified correctly
        var inspector = new ClassInspector(TwConfig.Default());

        // Test arbitrary font sizes (should be text-size, not text-color)
        var textArb1 = inspector.GetClassGroupId("text-[58px]");
        var textArb2 = inspector.GetClassGroupId("text-[1.5rem]");
        var textArb3 = inspector.GetClassGroupId("text-[14px]");

        // Test regular text sizes
        var textLg = inspector.GetClassGroupId("text-lg");
        var textXl = inspector.GetClassGroupId("text-xl");

        // Test color values (should be text-color)
        var textColor1 = inspector.GetClassGroupId("text-red-500");
        var textColor2 = inspector.GetClassGroupId("text-[#ff0000]");
        var textWhite = inspector.GetClassGroupId("text-white");

        // Test leading arbitrary values
        var leadingArb = inspector.GetClassGroupId("leading-[0.9]");
        var leadingArb2 = inspector.GetClassGroupId("leading-[1.5]");

        // Assert all arbitrary font sizes are classified as text-size
        Assert.Equal("text-size", textArb1);
        Assert.Equal("text-size", textArb2);
        Assert.Equal("text-size", textArb3);

        // Assert regular text sizes work
        Assert.Equal("text-size", textLg);
        Assert.Equal("text-size", textXl);

        // Assert colors are classified correctly
        Assert.Equal("text-color", textColor1);
        Assert.Equal("text-color", textColor2);
        Assert.Equal("text-color", textWhite);

        // Assert leading works
        Assert.Equal("leading", leadingArb);
        Assert.Equal("leading", leadingArb2);
    }

    [Fact]
    public void Should_Properly_Merge_Text_Size_With_Arbitrary_Values()
    {
        // This test demonstrates that the arbitrary value classification bug is fixed
        // Previously, text-[58px] was incorrectly classified as text-color instead of text-size

        var result1 = TwMerge.Merge("text-lg", "text-[58px]");
        Assert.Equal("text-[58px]", result1); // text-size should conflict properly

        var result2 = TwMerge.Merge("text-[24px]", "text-xl");
        Assert.Equal("text-xl", result2); // text-size should conflict properly

        var result3 = TwMerge.Merge("text-red-500", "text-[#ff0000]");
        Assert.Equal("text-[#ff0000]", result3); // text-color should conflict properly

        // Mixed scenarios that should NOT conflict
        var result4 = TwMerge.Merge("text-lg", "text-red-500");
        Assert.Equal("text-lg text-red-500", result4); // size and color should coexist

        var result5 = TwMerge.Merge("text-[58px]", "text-white");
        Assert.Equal("text-[58px] text-white", result5); // arbitrary size and color should coexist
    }
}
