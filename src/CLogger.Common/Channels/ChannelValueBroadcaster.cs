namespace CLogger.Common.Channels;

public class ChannelValueBroadcaster<T>(
    ChannelBroadcasterContainer container
) : ChannelBroadcaster<T>(container)
{
    public override Task WriteAsync(T data, CancellationToken cancellationToken)
    {
        // Don't bother publishing the event if the value didn't change
        if (Equals(Value, data))
        {
            return Task.CompletedTask;
        }

        return base.WriteAsync(data, cancellationToken);
    }
}
