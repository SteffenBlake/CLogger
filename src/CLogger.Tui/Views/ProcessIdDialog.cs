using Terminal.Gui;

namespace CLogger.Tui.Views;

public class ProcessIdDialog : Dialog
{
    public TextField ProcessIdText { get; }

    public ProcessIdDialog()
    {
        Title = "Process Id Found!";
        X = Pos.Center();
        Y = Pos.Center();
        Width = 20;
        Height = 3;
        Visible = false;

        Add(ProcessIdText = new()
        {
            X = Pos.Center(),
            Y = 0,
            Height = 1,
            Width = Dim.Fill(),
            Text = "",
            TextAlignment = TextAlignment.Centered,
            Enabled = true,
            ReadOnly = true,
            ColorScheme = ColorSchemes.InterestNoFocus
        });
    }
}
