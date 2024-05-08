using CLogger.Tui.Views;
using Terminal.Gui;
using CLogger.Tui.Models;

namespace CLogger.Tui.ViewModels;

public class KeybindsVM : IViewModel
{
    private ActionBar ActionBar { get; }
    private TestExplorer TestExplorer { get; }

    public KeybindsVM(
        ActionBar actionBar,
        TestExplorer testExplorer
    )
    {
        ActionBar = actionBar;
        TestExplorer = testExplorer;
        _keybinds =  new()
        {
            { Keybinds.ExplorerPick, TestExplorer.OnPick },
            { Keybinds.ExplorerUnpick, TestExplorer.OnUnpick },
            { Keybinds.Reload, ActionBar.Reload },
            { Keybinds.Run, ActionBar.Run },
            { Keybinds.Debug, ActionBar.Debug },
            { Keybinds.Cancel, ActionBar.Cancel },
        };
    }

    public Task BindAsync(CancellationToken cancellationToken)
    {
        Application.RootKeyEvent = OnKeyPress;
        return Task.CompletedTask;
    }

    private readonly Dictionary<KeybindInfo, Func<bool>> _keybinds;
    private bool OnKeyPress(KeyEvent e)
    {
        var info = KeybindInfo.FromEvent(e);
        if (_keybinds.TryGetValue(info, out var keybind))
        {
            return keybind();
        }
        return false;
    }
}
