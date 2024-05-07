using System.Text;
using Terminal.Gui;

namespace CLogger.Tui.Models;

public record KeybindInfo(
    Key Key, bool IsCtrl = false, bool IsAlt = false, bool IsShift = false
)
{
    public static KeybindInfo FromEvent(KeyEvent e) =>
        new(e.Key & Key.CharMask, e.IsCtrl, e.IsAlt, e.IsShift);

    public override string? ToString()
    {
        var output = new StringBuilder();
        if (IsCtrl)
        {
            output.Append('c');
        }
        if (IsAlt)
        {
            output.Append('a');
        }
        if (IsShift)
        {
            output.Append('s');
        }
        if (IsCtrl || IsAlt || IsShift)
        {
            output.Append('-');
        }

        output.Append(Key);

        return output.ToString();
    }
}
