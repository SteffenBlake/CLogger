using CLogger.Common.Channels;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class MainWindow : Window
{
    public MainWindow(
        ActionBar actionBar,
        TestExplorer testExplorer,
        InfoPanel infoPanel,
        InfoBar infoBar,
        ChannelBroadcasterContainer broadcasters
    )
    {
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        Add(actionBar);
        Add(testExplorer);
        Add(infoPanel);
        Add(infoBar);

        Closed += (_) => {
            broadcasters.TryComplete();
        };
    }
}
