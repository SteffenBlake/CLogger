using CLogger.Common.Model;
using CLogger.Tui.Coroutines;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class MainWindow : Window
{
    private ModelState ModelState { get; }

    private ActionBar ActionBar { get; }

    private TestExplorer TestExplorer { get; }

    private InfoPanel InfoPanel { get; }

    private InfoBar InfoBar { get; }

    public MainWindow(ModelState modelState)
    {
        ModelState = modelState;
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        Add(ActionBar = new(ModelState));
        Add(TestExplorer = new(ModelState, ActionBar));
        Add(InfoPanel = new(ModelState, ActionBar, TestExplorer));
        Add(InfoBar = new(ModelState, TestExplorer));

        Closing += (_) => ModelState.TryComplete();
    }

    public async Task BindAsync()
    {
        await Task.WhenAll(
           ActionBar.BindAsync(),
           TestExplorer.BindAsync(),
           InfoPanel.BindAsync(),
           InfoBar.BindAsync()
        );
    }
}
