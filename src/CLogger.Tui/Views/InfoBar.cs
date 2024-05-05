using Terminal.Gui;

namespace CLogger.Tui.Views;

public class InfoBar : FrameView 
{
    public InfoBar(TestExplorer testExplorer)
    {
        X = 0;
        Y = Pos.Bottom(testExplorer);
        Width = Dim.Fill();
        Height = 1;
        Border.BorderStyle = BorderStyle.None;
    }
}
