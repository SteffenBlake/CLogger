namespace CLogger.Common.Model;

public class AppConfig(CancellationToken cancellationToken)
{
    public ChannelBroadcaster<bool> DebugMode { get; }
        = new(false, cancellationToken);

    public ChannelBroadcaster<string> Path { get; }
        = new(".", cancellationToken);
    
    public ChannelBroadcaster<bool> Run { get; }
        = new(false, cancellationToken);
    
    public bool TryComplete()
    {
        return
            DebugMode.TryComplete() &&
            Path.TryComplete() &&
            Run.TryComplete();
    }
}
