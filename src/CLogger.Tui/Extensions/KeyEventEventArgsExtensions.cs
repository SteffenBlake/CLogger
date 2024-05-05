using Terminal.Gui;
using static Terminal.Gui.View;

namespace CLogger.Tui.Extensions;

public static class KeyEventEventArgsExtensions 
{
    public static void Deconstruct(
        this KeyEventEventArgs args,
        out Key key,
        out bool ctrl,
        out bool alt,
        out bool shift
    )
    {
        key = args.KeyEvent.Key;
        ctrl = args.KeyEvent.IsCtrl;
        alt = args.KeyEvent.IsAlt;
        shift = args.KeyEvent.IsShift;
    }
}
