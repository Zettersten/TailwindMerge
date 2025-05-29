using TailwindMerge.Rules;
using TailwindMerge.Utilities;

namespace TailwindMerge.Tests;

public class IntegrationTests
{
    public static readonly TwMerge TwMerge = new();

    [Fact]
    public void Should_Merge_CssClassNames()
    {
        var classes = TwMerge.Merge("w-3", "w-4");
        var expected = "w-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Remove_Duplicate_Classes()
    {
        var classes = TwMerge.Merge("text-center", "text-center");
        var expected = "text-center";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Handle_Empty_Inputs()
    {
        var classes = TwMerge.Merge("", null!, "p-4");
        var expected = "p-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_With_Modifiers()
    {
        var classes = TwMerge.Merge("hover:w-3", "hover:w-4");
        var expected = "hover:w-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Arbitrary_Properties()
    {
        var classes = TwMerge.Merge("[mask-type:luminance]", "[mask-type:alpha]");
        var expected = "[mask-type:alpha]";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Multiple_Conflicting_Classes()
    {
        var classes = TwMerge.Merge("w-2", "w-3", "w-4");
        var expected = "w-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Classes_With_Important_Modifier()
    {
        var classes = TwMerge.Merge("w-3", "!w-4");
        var expected = "!w-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Text_Color_Utilities()
    {
        var classes = TwMerge.Merge("text-red-500", "text-blue-500");
        var expected = "text-blue-500";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Background_Color_Utilities()
    {
        var classes = TwMerge.Merge("bg-green-200", "bg-green-300");
        var expected = "bg-green-300";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Spacing_Utilities()
    {
        var classes = TwMerge.Merge("mt-2", "mt-4", "mb-2");
        var expected = "mt-4 mb-2";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Flex_And_Grid_Utilities()
    {
        var classes = TwMerge.Merge("flex", "grid");
        var expected = "grid";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Responsive_Utilities()
    {
        var classes = TwMerge.Merge("sm:w-2", "sm:w-4", "md:w-2");
        var expected = "sm:w-4 md:w-2";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Pseudo_Classes()
    {
        var classes = TwMerge.Merge("hover:bg-red-500", "hover:bg-blue-500", "focus:bg-green-500");
        var expected = "hover:bg-blue-500 focus:bg-green-500";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Dark_Mode_Classes()
    {
        var classes = TwMerge.Merge("dark:text-white", "dark:text-black");
        var expected = "dark:text-black";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Order_Sensitive_Utilities()
    {
        var classes = TwMerge.Merge("ring-2", "ring-inset", "ring-4");
        var expected = "ring-inset ring-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Mixed_Utilities()
    {
        var classes = TwMerge.Merge("p-2", "m-2", "p-4", "text-lg", "text-sm");
        var expected = "m-2 p-4 text-sm";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_With_Arbitrary_Values()
    {
        var classes = TwMerge.Merge("w-[32px]", "w-[64px]");
        var expected = "w-[64px]";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_With_Multiple_Modifiers()
    {
        var classes = TwMerge.Merge("sm:hover:bg-red-500", "sm:hover:bg-blue-500");
        var expected = "sm:hover:bg-blue-500";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Text_Align_And_Font_Weight()
    {
        var classes = TwMerge.Merge("text-left", "font-bold", "text-right", "font-light");
        var expected = "text-right font-light";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Text_Size_And_Font_Family()
    {
        var classes = TwMerge.Merge("text-sm", "font-sans", "text-lg", "font-mono");
        var expected = "text-lg font-mono";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Leading_And_Tracking()
    {
        var classes = TwMerge.Merge(
            "leading-tight",
            "tracking-wide",
            "leading-loose",
            "tracking-tighter"
        );
        var expected = "leading-loose tracking-tighter";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Text_Decoration_And_Transform()
    {
        var classes = TwMerge.Merge("underline", "uppercase", "line-through", "lowercase");
        var expected = "line-through lowercase";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Text_Color_And_Background_Color()
    {
        var classes = TwMerge.Merge(
            "text-red-500",
            "bg-blue-100",
            "text-green-600",
            "bg-yellow-200"
        );
        var expected = "text-green-600 bg-yellow-200";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Padding_And_Margin()
    {
        var classes = TwMerge.Merge("p-2", "m-2", "p-4", "m-0");
        var expected = "p-4 m-0";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Space_Between_And_Divide()
    {
        var classes = TwMerge.Merge("space-x-2", "divide-y-2", "space-x-4", "divide-y-4");
        var expected = "space-x-4 divide-y-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Width_And_Height()
    {
        var classes = TwMerge.Merge("w-8", "h-8", "w-16", "h-16");
        var expected = "w-16 h-16";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Border_And_Radius()
    {
        var classes = TwMerge.Merge("border", "rounded", "border-2", "rounded-lg");
        var expected = "border-2 rounded-lg";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Shadow_And_Opacity()
    {
        var classes = TwMerge.Merge("shadow", "opacity-50", "shadow-lg", "opacity-100");
        var expected = "shadow-lg opacity-100";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Flex_And_Justify_Items()
    {
        var classes = TwMerge.Merge("flex", "justify-start", "flex-col", "justify-end");
        var expected = "flex-col justify-end";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Debug_Opacity_ClassMap()
    {
        var config = TwConfig.Default();

        _ = new ClassInspector(config);

        // Check if opacity is in the class groups
        var hasOpacityGroup = config.ClassGroupsValue.ContainsKey("opacity");

        Assert.True(hasOpacityGroup, "Opacity should be in class groups");

        // Check what the opacity class group contains
        if (hasOpacityGroup)
        {
            var opacityGroup = config.ClassGroupsValue["opacity"];

            // This should help us see what's in the opacity group
            Console.WriteLine($"Opacity group has {opacityGroup.Count} items");

            foreach (var item in opacityGroup)
            {
                Console.WriteLine($"Item type: {item.GetType()}");
            }
        }
    }

    [Fact]
    public void Debug_Opacity_Classes()
    {
        var inspector = new ClassInspector(TwConfig.Default());
        var opacity50 = inspector.GetClassGroupId("opacity-50");
        var opacity100 = inspector.GetClassGroupId("opacity-100");

        // These should both return "opacity"
        Assert.Equal("opacity", opacity50);
        Assert.Equal("opacity", opacity100);
    }

    [Fact]
    public void Debug_NumberRule()
    {
        var numberRule = new NumberRule();
        var result50 = numberRule.Execute("50");
        var result100 = numberRule.Execute("100");

        Assert.True(result50, "NumberRule should match '50'");
        Assert.True(result100, "NumberRule should match '100'");
    }

    [Fact]
    public void Should_Merge_Grid_And_Items_Align()
    {
        var classes = TwMerge.Merge("grid", "items-center", "grid-cols-2", "items-end");
        var expected = "grid-cols-2 items-end";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Overflow_And_ZIndex()
    {
        var classes = TwMerge.Merge("overflow-auto", "z-10", "overflow-hidden", "z-50");
        var expected = "overflow-hidden z-50";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Transition_And_Duration()
    {
        var classes = TwMerge.Merge(
            "transition",
            "duration-100",
            "transition-colors",
            "duration-300"
        );
        var expected = "transition-colors duration-300";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Responsive_Text_And_Spacing()
    {
        var classes = TwMerge.Merge(
            "sm:text-xs",
            "md:text-lg",
            "sm:text-base",
            "md:text-xl",
            "sm:p-2",
            "sm:p-4"
        );
        var expected = "sm:text-base md:text-xl sm:p-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Text_Size()
    {
        var classes = TwMerge.Merge("sm:text-xs", "sm:text-lg", "md:text-base", "md:text-xl");
        var expected = "sm:text-lg md:text-xl";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Font_Weight()
    {
        var classes = TwMerge.Merge(
            "sm:font-bold",
            "sm:font-light",
            "md:font-medium",
            "md:font-black"
        );
        var expected = "sm:font-light md:font-black";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Padding()
    {
        var classes = TwMerge.Merge("sm:p-2", "sm:p-4", "md:p-1", "md:p-8");
        var expected = "sm:p-4 md:p-8";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Margin()
    {
        var classes = TwMerge.Merge("sm:m-2", "sm:m-0", "md:m-4", "md:m-1");
        var expected = "sm:m-0 md:m-1";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Text_Align()
    {
        var classes = TwMerge.Merge(
            "sm:text-left",
            "sm:text-right",
            "md:text-center",
            "md:text-justify"
        );
        var expected = "sm:text-right md:text-justify";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Width_And_Height()
    {
        var classes = TwMerge.Merge("sm:w-8", "sm:w-16", "md:h-8", "md:h-16");
        var expected = "sm:w-16 md:h-16";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Border_And_Radius()
    {
        var classes = TwMerge.Merge("sm:border", "sm:border-2", "md:rounded", "md:rounded-lg");
        var expected = "sm:border-2 md:rounded-lg";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Shadow_And_Opacity()
    {
        var classes = TwMerge.Merge("sm:shadow", "sm:shadow-lg", "md:opacity-50", "md:opacity-100");
        var expected = "sm:shadow-lg md:opacity-100";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Flex_And_Justify()
    {
        var classes = TwMerge.Merge("sm:flex", "sm:flex-col", "md:justify-start", "md:justify-end");
        var expected = "sm:flex-col md:justify-end";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Grid_And_Items()
    {
        var classes = TwMerge.Merge("sm:grid", "sm:grid-cols-2", "md:items-center", "md:items-end");
        var expected = "sm:grid-cols-2 md:items-end";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Overflow_And_ZIndex()
    {
        var classes = TwMerge.Merge("sm:overflow-auto", "sm:overflow-hidden", "md:z-10", "md:z-50");
        var expected = "sm:overflow-hidden md:z-50";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Transition_And_Duration()
    {
        var classes = TwMerge.Merge(
            "sm:transition",
            "sm:transition-colors",
            "md:duration-100",
            "md:duration-300"
        );
        var expected = "sm:transition-colors md:duration-300";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Space_And_Divide()
    {
        var classes = TwMerge.Merge(
            "sm:space-x-2",
            "sm:space-x-4",
            "md:divide-y-2",
            "md:divide-y-4"
        );
        var expected = "sm:space-x-4 md:divide-y-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Text_Color_And_Bg()
    {
        var classes = TwMerge.Merge(
            "sm:text-red-500",
            "sm:text-blue-500",
            "md:bg-green-200",
            "md:bg-green-300"
        );
        var expected = "sm:text-blue-500 md:bg-green-300";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_And_Md_Arbitrary_Values()
    {
        var classes = TwMerge.Merge("sm:w-[32px]", "sm:w-[64px]", "md:h-[10px]", "md:h-[20px]");
        var expected = "sm:w-[64px] md:h-[20px]";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_Text_Size()
    {
        var classes = TwMerge.Merge("sm:text-xs", "sm:text-lg");
        var expected = "sm:text-lg";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Md_Text_Size()
    {
        var classes = TwMerge.Merge("md:text-base", "md:text-2xl");
        var expected = "md:text-2xl";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Lg_Text_Align()
    {
        var classes = TwMerge.Merge("lg:text-left", "lg:text-center");
        var expected = "lg:text-center";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Xl_Font_Weight()
    {
        var classes = TwMerge.Merge("xl:font-bold", "xl:font-extrabold");
        var expected = "xl:font-extrabold";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_2xl_Font_Family()
    {
        var classes = TwMerge.Merge("2xl:font-sans", "2xl:font-mono");
        var expected = "2xl:font-mono";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Sm_Padding()
    {
        var classes = TwMerge.Merge("sm:p-2", "sm:p-8");
        var expected = "sm:p-8";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Md_Margin()
    {
        var classes = TwMerge.Merge("md:m-1", "md:m-6");
        var expected = "md:m-6";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Lg_Width()
    {
        var classes = TwMerge.Merge("lg:w-1/2", "lg:w-full");
        var expected = "lg:w-full";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Xl_Height()
    {
        var classes = TwMerge.Merge("xl:h-8", "xl:h-32");
        var expected = "xl:h-32";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_2xl_Max_Width()
    {
        var classes = TwMerge.Merge("2xl:max-w-xs", "2xl:max-w-2xl");
        var expected = "2xl:max-w-2xl";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Debug_Flex_Conflicts()
    {
        var config = TwConfig.Default();
        var inspector = new ClassInspector(config);

        var flexGroupId = inspector.GetClassGroupId("flex");
        var flexColGroupId = inspector.GetClassGroupId("flex-col");

        Console.WriteLine($"flex group: {flexGroupId}");
        Console.WriteLine($"flex-col group: {flexColGroupId}");

        if (flexGroupId != null)
        {
            var conflicts = inspector.GetConflictingClassGroupIds(flexGroupId, false);
            Console.WriteLine($"flex conflicts with: {string.Join(", ", conflicts)}");
        }
    }

    [Fact]
    public void Should_Merge_Sm_Grid_Cols()
    {
        var classes = TwMerge.Merge("sm:grid-cols-2", "sm:grid-cols-4");
        var expected = "sm:grid-cols-4";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Md_Flex_Direction()
    {
        var classes = TwMerge.Merge("md:flex-row", "md:flex-col");
        var expected = "md:flex-col";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Lg_Justify_Content()
    {
        var classes = TwMerge.Merge("lg:justify-start", "lg:justify-between");
        var expected = "lg:justify-between";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_Xl_Items_Align()
    {
        var classes = TwMerge.Merge("xl:items-center", "xl:items-end");
        var expected = "xl:items-end";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Should_Merge_2xl_Space_Between()
    {
        var classes = TwMerge.Merge("2xl:space-x-2", "2xl:space-x-8");
        var expected = "2xl:space-x-8";
        Assert.Equal(expected, classes);
    }

    [Fact]
    public void Debug_Border_Classes()
    {
        var inspector = new ClassInspector(TwConfig.Default());

        var borderGroupId = inspector.GetClassGroupId("border");
        var border2GroupId = inspector.GetClassGroupId("border-2");

        Console.WriteLine($"border group: {borderGroupId}");
        Console.WriteLine($"border-2 group: {border2GroupId}");

        // These should both be "border-width"
        Assert.Equal("border-width", borderGroupId);
        Assert.Equal("border-width", border2GroupId);
    }
}
