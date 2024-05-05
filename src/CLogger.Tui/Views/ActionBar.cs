using Terminal.Gui;

namespace CLogger.Tui.Views;

public class ActionBar : FrameView 
{
    public Button ReloadBtn { get; }
    public Button RunBtn { get; }
    public Button DebugBtn { get; }
    
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
            Enabled = false
        });
        ReloadBtn.Enter += (_) => ReloadBtn.Text = ": Reload";
        ReloadBtn.Leave += (_) => ReloadBtn.Text = "";

        Add(RunBtn = new("")
        {
            X = Pos.Right(ReloadBtn) + 1,
            Y = 0,
            Height = 1,
            Enabled = false
        });
        RunBtn.Enter += (_) => RunBtn.Text = ": Run";
        RunBtn.Leave += (_) => RunBtn.Text = "";

        Add(DebugBtn = new("")
        {
            X = Pos.Right(RunBtn) + 1,
            Y = 0,
            Height = 1,
            Enabled = false
        });
        DebugBtn.Enter += (_) => DebugBtn.Text = ": Debug";
        DebugBtn.Leave += (_) => DebugBtn.Text = "";
    }
}
