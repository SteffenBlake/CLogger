using CLogger.Tui.Models;
using Terminal.Gui;

namespace CLogger.Tui;

public static class Keybinds 
{
    public static readonly KeybindInfo ExplorerPick = new(Key:Key.p);
    
    public static readonly KeybindInfo ExplorerUnpick = new(Key:Key.u);

    public static readonly KeybindInfo Reload = new(Key:Key.l);

    public static readonly KeybindInfo Run = new(Key:Key.r);

    public static readonly KeybindInfo Debug = new(Key:Key.d);

    public static readonly KeybindInfo Cancel = new(Key:Key.c);
}
