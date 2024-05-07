using Terminal.Gui;

namespace CLogger.Tui.Views;

public class ProcessIdDialog : Dialog
{
    public TextField ProcessIdText { get; }
    public Button CopyButton { get; }

    public ProcessIdDialog()
    {
        Title = "Process Id Found!";
        X = Pos.Center();
        Y = Pos.Center();
        Width = 20;
        Height = 5;
        Visible = false;

        Add(ProcessIdText = new()
        {
            X = 1,
            Y = 1,
            Height = 1,
            Width = Dim.Percent(75),
            Text = "",
            Enabled = false,
            ColorScheme = ColorSchemes.InterestNoFocus
        });

        Add(CopyButton = new()
        {
            X = Pos.Right(ProcessIdText)+1,
            Y = Pos.Top(ProcessIdText),
            Height = 1,
            Width = Dim.Fill(margin:1),
            Text = " Copy",
            ColorScheme = ColorSchemes.Warn
        });
    }
    
}
