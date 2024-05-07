using CLogger.Tui.Models;
using Terminal.Gui;

namespace CLogger.Tui;

public static class Keybinds 
{
    public static readonly KeybindInfo ExplorerPick = new(Key:Key.P);
    
    public static readonly KeybindInfo ExplorerUnpick = new(Key:Key.U);

    public static readonly KeybindInfo Reload = new(Key:Key.L);

    public static readonly KeybindInfo Run = new(Key:Key.R);

    public static readonly KeybindInfo Debug = new(Key:Key.D);

    public static readonly KeybindInfo Cancel = new(Key:Key.C);
}
