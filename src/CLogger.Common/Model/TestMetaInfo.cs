namespace CLogger.Common.Model;

public class TestMetaInfo(CancellationToken cancellationToken)
{
    public ChannelBroadcaster<TimeSpan> Elapsed { get; } 
        = new(default, cancellationToken);

    public ChannelBroadcaster<int> TestProcessId { get; } 
        = new(0, cancellationToken);

    public void Complete()
    {
        Elapsed.Complete();
        TestProcessId.Complete();
    }
}
