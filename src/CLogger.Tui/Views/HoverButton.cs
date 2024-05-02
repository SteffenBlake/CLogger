using System;
using System.Linq;
using CommandLine.Text;
using NStack;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class HoverButton : Button
{
    public ustring LeaveText { get; set; }
    public ustring HelpText { get; set; }

    public HoverButton(ustring text, ustring helpMessage)
    {
        LeaveText = Text = text;
        HelpText = LeaveText + ": " + helpMessage;
        Enter += (_) => UpdateText();
        Leave += (_) => UpdateText();
        UpdateText();
    }

    private void UpdateText()
    {
        if (HasFocus)
        {
            Text = HelpText;
        }
        else
        {
            Text = LeaveText;
        }
    }
}
