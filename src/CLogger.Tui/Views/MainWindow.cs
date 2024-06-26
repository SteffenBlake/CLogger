using Terminal.Gui;

namespace CLogger.Tui.Views;

public class MainWindow : Window
{
    public MainWindow(
        ActionBar actionBar,
        TestExplorer testExplorer,
        InfoPanel infoPanel,
        InfoBar infoBar,
        ProcessIdDialog processIdDialog
    )
    {
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        Add(actionBar);
        Add(testExplorer);
        Add(infoPanel);
        Add(infoBar);
        Add(processIdDialog);
    }
}
