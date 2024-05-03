using CLogger.Common.Channels;
using CLogger.Tui.Views;

namespace CLogger.Tui.ViewModels;

public class MainWindowVM(
    ChannelBroadcasterContainer broadcasters,
    MainWindow view
) : IViewModel
{
    public ChannelBroadcasterContainer Broadcasters { get; } = broadcasters;
    public MainWindow View { get; } = view;

    public Task BindAsync(CancellationToken cancellationToken)
    {
        View.Closing += (_) => Broadcasters.TryComplete();
        return Task.CompletedTask;
    }
}
