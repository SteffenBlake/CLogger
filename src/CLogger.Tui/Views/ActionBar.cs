using CLogger.Common.Model;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class ActionBar : FrameView 
{
    public HoverButton ReloadBtn { get; }
    public HoverButton RunBtn { get; }
    public HoverButton DebugBtn { get; }
    
    public ActionBar(ModelState modelState)
    {
        Title = "Actions";
        X = Pos.Center();
        Y = 0;
        Width = Dim.Percent(100f);
        Height = 1;
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        Add(ReloadBtn = new("", "Reload")
        {
            X = 1,
            Y = 0,
            Height = 1,
        });

        Add(RunBtn = new("","Run Tests")
        {
            X = Pos.Right(ReloadBtn) + 1,
            Y = 0,
            Height = 1,
        });

        Add(DebugBtn = new("","Debug Tests")
        {
            X = Pos.Right(RunBtn) + 1,
            Y = 0,
            Height = 1,
        });
    }

    public Task BindAsync()
    {
        return Task.CompletedTask;
    }
}
