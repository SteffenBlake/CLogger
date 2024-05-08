using CLogger.Tui.Models;
using Terminal.Gui;

namespace CLogger.Tui;

public static class Keybinds 
{
    public static readonly KeybindInfo Quit = new(Key:Key.q, IsCtrl:true);

    public static readonly KeybindInfo ExplorerPick = new(Key:Key.p);
    
    public static readonly KeybindInfo ExplorerUnpick = new(Key:Key.u);

    public static readonly KeybindInfo Reload = new(Key:Key.o);

    public static readonly KeybindInfo Run = new(Key:Key.r);

    public static readonly KeybindInfo Debug = new(Key:Key.d);

    public static readonly KeybindInfo Cancel = new(Key:Key.c);

    public static readonly KeybindInfo ScrollLeft = new(Key:Key.h);
    
    public static readonly KeybindInfo ScrollDown = new(Key:Key.j);
    
    public static readonly KeybindInfo ScrollUp = new(Key:Key.k);
    
    public static readonly KeybindInfo ScrollRight = new(Key:Key.l);
}
