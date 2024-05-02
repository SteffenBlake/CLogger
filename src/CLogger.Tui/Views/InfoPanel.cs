using CLogger.Common.Model;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class InfoPanel : FrameView 
{
    private ModelState ModelState { get; }
    
    public InfoPanel(
        ModelState modelState, 
        View actionBar,
        View testExplorer
    )
    {
        Title = "Info";
        X = Pos.Right(testExplorer); 
        Y = Pos.Bottom(actionBar);
        Width = Dim.Fill();
        Height = Dim.Fill(margin:1);
        ModelState = modelState;
    }

    public Task BindAsync()
    {
        return Task.CompletedTask;
    }
}
