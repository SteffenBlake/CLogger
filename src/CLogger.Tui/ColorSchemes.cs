using Terminal.Gui;

namespace CLogger.Tui;

public static class ColorSchemes 
{
    public static readonly ColorScheme Standard = new()
    {
        Normal = new(Color.White, Color.Black),
        Focus = new(Color.White, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.White, Color.Cyan),
        HotNormal = new (Color.White, Color.DarkGray)
    };

    public static readonly ColorScheme Interest = new()
    {
        Normal = new(Color.Cyan, Color.Black),
        Focus = new(Color.Cyan, Color.Gray),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.Cyan, Color.White),
        HotNormal = new (Color.Cyan, Color.Gray)
    };

    public static readonly ColorScheme Good = new()
    {
        Normal = new(Color.Green, Color.Black),
        Focus = new(Color.Green, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.Green, Color.Cyan),
        HotNormal = new (Color.Green, Color.Blue)
    };

    public static readonly ColorScheme Bad = new()
    {
        Normal = new(Color.Red, Color.Black),
        Focus = new(Color.Red, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.Red, Color.Cyan),
        HotNormal = new (Color.Red, Color.Blue)
    };

    public static readonly ColorScheme Warn = new()
    {
        Normal = new(Color.BrightYellow, Color.Black),
        Focus = new(Color.BrightYellow, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.BrightYellow, Color.Cyan),
        HotNormal = new (Color.BrightYellow, Color.Blue)
    };

    public static readonly ColorScheme StandardPicked = new()
    {
        Normal = new(Color.White, Color.Blue),
        Focus = new(Color.White, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.White, Color.Cyan),
        HotNormal = new (Color.White, Color.Gray)
    };

    public static readonly ColorScheme GoodPicked = new()
    {
        Normal = new(Color.BrightGreen, Color.Blue),
        Focus = new(Color.BrightGreen, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.BrightGreen, Color.Cyan),
        HotNormal = new (Color.BrightGreen, Color.Gray)
    };

    public static readonly ColorScheme BadPicked = new()
    {
        Normal = new(Color.BrightRed, Color.Blue),
        Focus = new(Color.BrightRed, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.BrightRed, Color.Cyan),
        HotNormal = new (Color.BrightRed, Color.Gray)
    };

    public static readonly ColorScheme WarnPicked = new()
    {
        Normal = new(Color.BrightYellow, Color.Blue),
        Focus = new(Color.BrightYellow, Color.BrightCyan),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.BrightYellow, Color.Cyan),
        HotNormal = new (Color.BrightYellow, Color.Gray)
    };

    public static readonly ColorScheme InterestNoFocus = new()
    {
        Normal = new(Color.Blue, Color.Black),
        Focus = new(Color.Blue, Color.Black),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.Blue, Color.Cyan),
        HotNormal = new (Color.Blue, Color.Cyan)
    };

    public static readonly ColorScheme BadNoFocus = new()
    {
        Normal = new(Color.BrightRed, Color.Black),
        Focus = new(Color.BrightRed, Color.Black),
        Disabled = new(Color.DarkGray, Color.Black),
        HotFocus = new (Color.BrightRed, Color.Cyan),
        HotNormal = new (Color.BrightRed, Color.Cyan)
    };
}
