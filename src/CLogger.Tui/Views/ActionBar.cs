using Terminal.Gui;

namespace CLogger.Tui.Views;

public class ActionBar : FrameView 
{
    public Button ReloadBtn { get; }
    public Button RunBtn { get; }
    public Button DebugBtn { get; }
    public Button CancelBtn { get; }

    public ActionBar()
    {
        Title = "Actions";
        X = Pos.Center();
        Y = 0;
        Width = Dim.Percent(100f);
        Height = 1;
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        Add(ReloadBtn = new("")
        {
            X = 1,
            Y = 0,
            Height = 1,
            Enabled = false,
            ColorScheme = ColorSchemes.Interest
        });
        ReloadBtn.Enter += (_) => ReloadBtn.Text = ": Reload";
        ReloadBtn.Leave += (_) => ReloadBtn.Text = "";

        Add(RunBtn = new("")
        {
            X = Pos.Right(ReloadBtn) + 1,
            Y = 0,
            Height = 1,
            Enabled = false,
            ColorScheme = ColorSchemes.Good
        });
        RunBtn.Enter += (_) => RunBtn.Text = ": Run";
        RunBtn.Leave += (_) => RunBtn.Text = "";

        Add(DebugBtn = new("")
        {
            X = Pos.Right(RunBtn) + 1,
            Y = 0,
            Height = 1,
            Enabled = false,
            ColorScheme = ColorSchemes.Warn
        });
        DebugBtn.Enter += (_) => DebugBtn.Text = ": Debug";
        DebugBtn.Leave += (_) => DebugBtn.Text = "";

        Add(CancelBtn = new("")
        {
            X = Pos.Right(DebugBtn) + 1,
            Y = 0,
            Height = 1,
            Enabled = true,
            ColorScheme = ColorSchemes.Bad
        });
        CancelBtn.Enter += (_) => CancelBtn.Text = ": Cancel";
        CancelBtn.Leave += (_) => CancelBtn.Text = "";
    }

    public bool Reload()
    {
        ReloadBtn.OnClicked();
        return true;
    }
    
    public bool Run()
    {
        RunBtn.OnClicked();
        return true;
    }
    
    public bool Debug()
    {
        DebugBtn.OnClicked();
        return true;
    }

    public bool Cancel()
    {
        CancelBtn.OnClicked();
        return true;
    }
}
