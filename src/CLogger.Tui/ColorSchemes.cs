using Terminal.Gui;

namespace CLogger.Tui;

public static class ColorSchemes 
{
    public static readonly ColorScheme Standard = new()
    {
        Normal = new(Color.White, Color.Black),
        Focus = new(Color.White, Color.BrightCyan),
        Disabled = new(Color.Gray, Color.DarkGray),
        HotFocus = new (Color.White, Color.Cyan),
        HotNormal = new (Color.White, Color.Gray)
    };

    public static readonly ColorScheme Interest = new()
    {
        Normal = new(Color.Cyan, Color.Black),
        Focus = new(Color.Cyan, Color.Gray),
        Disabled = new(Color.Gray, Color.DarkGray),
        HotFocus = new (Color.Cyan, Color.White),
        HotNormal = new (Color.Cyan, Color.Gray)
    };

    public static readonly ColorScheme Good = new()
    {
        Normal = new(Color.BrightGreen, Color.Black),
        Focus = new(Color.BrightGreen, Color.BrightCyan),
        Disabled = new(Color.Green, Color.DarkGray),
        HotFocus = new (Color.BrightGreen, Color.Cyan),
        HotNormal = new (Color.BrightGreen, Color.Gray)
    };

    public static readonly ColorScheme Bad = new()
    {
        Normal = new(Color.BrightRed, Color.Black),
        Focus = new(Color.BrightRed, Color.BrightCyan),
        Disabled = new(Color.Brown, Color.Black),
        HotFocus = new (Color.BrightRed, Color.Cyan),
        HotNormal = new (Color.BrightRed, Color.Black)
    };

    public static readonly ColorScheme Warn = new()
    {
        Normal = new(Color.BrightYellow, Color.Black),
        Focus = new(Color.BrightYellow, Color.BrightCyan),
        Disabled = new(Color.Brown, Color.Black),
        HotFocus = new (Color.BrightYellow, Color.Cyan),
        HotNormal = new (Color.BrightYellow, Color.Gray)
    };

    public static readonly ColorScheme StandardPicked = new()
    {
        Normal = new(Color.White, Color.Blue),
        Focus = new(Color.White, Color.BrightCyan),
        Disabled = new(Color.Gray, Color.Black),
        HotFocus = new (Color.White, Color.Cyan),
        HotNormal = new (Color.White, Color.Gray)
    };

    public static readonly ColorScheme GoodPicked = new()
    {
        Normal = new(Color.BrightGreen, Color.Blue),
        Focus = new(Color.BrightGreen, Color.BrightCyan),
        Disabled = new(Color.Green, Color.DarkGray),
        HotFocus = new (Color.BrightGreen, Color.Cyan),
        HotNormal = new (Color.BrightGreen, Color.Gray)
    };

    public static readonly ColorScheme BadPicked = new()
    {
        Normal = new(Color.BrightRed, Color.Blue),
        Focus = new(Color.BrightRed, Color.BrightCyan),
        Disabled = new(Color.Red, Color.DarkGray),
        HotFocus = new (Color.BrightRed, Color.Cyan),
        HotNormal = new (Color.BrightRed, Color.Gray)
    };

    public static readonly ColorScheme WarnPicked = new()
    {
        Normal = new(Color.BrightYellow, Color.Blue),
        Focus = new(Color.BrightYellow, Color.BrightCyan),
        Disabled = new(Color.Brown, Color.DarkGray),
        HotFocus = new (Color.BrightYellow, Color.Cyan),
        HotNormal = new (Color.BrightYellow, Color.Gray)
    };

    public static readonly ColorScheme BadNoFocus = new()
    {
        Normal = new(Color.BrightRed, Color.Black),
        Focus = new(Color.BrightRed, Color.Black),
        Disabled = new(Color.Brown, Color.Black),
        HotFocus = new (Color.BrightRed, Color.Cyan),
        HotNormal = new (Color.BrightRed, Color.Cyan)
    };
}
