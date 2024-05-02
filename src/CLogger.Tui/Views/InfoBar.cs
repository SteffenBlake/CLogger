using CLogger.Common.Model;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class InfoBar : FrameView 
{
    private ModelState ModelState { get; }
    
    public InfoBar(ModelState modelState, View testExplorer)
    {
        X = 0;
        Y = Pos.Bottom(testExplorer);
        Width = Dim.Fill();
        Height = 1;
        Border.BorderStyle = BorderStyle.None;

        ModelState = modelState;
    }

    public Task BindAsync()
    {
        return Task.CompletedTask;
    }
}
