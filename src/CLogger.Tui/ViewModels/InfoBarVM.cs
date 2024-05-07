using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Views;
using Terminal.Gui;

namespace CLogger.Tui.ViewModels;

public class InfoBarVM(
    InfoBar infoBar,
    ModelState modelState
) : IViewModel
{
    private InfoBar InfoBar { get; } = infoBar;
    private ModelState ModelState { get; } = modelState;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        await WatchForBusy(cancellationToken);
    }

    private async Task WatchForBusy(CancellationToken cancellationToken)
    {
        var events = ModelState.MetaInfo.State.Subscribe(cancellationToken);
        await foreach(var state in events)
        {
            Application.MainLoop.Invoke(() => 
                InfoBar.OnState(state == AppState.Busy)
            );
        }
    }
}
