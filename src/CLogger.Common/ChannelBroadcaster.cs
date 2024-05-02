using System.Threading.Channels;

namespace CLogger.Common;

public static class ChannelBroadcaster
{
    public static readonly List<object> All = [];
}

public class ChannelBroadcaster<T>
{
    public T Value { get; private set; }
    private CancellationToken CancellationToken { get; }

    private readonly List<Channel<T>> _targets = [];

    public ChannelBroadcaster(T value, CancellationToken cancellationToken)
    {
        Value = value;
        CancellationToken = cancellationToken;
        ChannelBroadcaster.All.Add(this);
    }

    public async Task WriteAsync(T data)
    {
        // Dont bother publishing the event if the value didnt change
        if (Equals(Value, data))
        {
            return;
        }
        Value = data;

        // Pre-start all the ValueTasks
        var writeTasks = _targets.Select(w => 
            w.Writer.WriteAsync(data, CancellationToken)
        ).ToList();

        // Await them (cant use Task.WhenAll with ValueTasks)
        foreach(var writeTask in writeTasks)
        {
            await writeTask;
        }
    }

    public IAsyncEnumerable<T> Subscribe()
    {
        var channel = Channel.CreateUnbounded<T>();
        _targets.Add(channel);
        return channel.Reader.ReadAllAsync(CancellationToken);
    }

    public bool TryComplete() 
    {
        var result = _targets.All(t => t.Writer.TryComplete());
        _targets.Clear();
        ChannelBroadcaster.All.Remove(this);
        return result;
    }

    public override string? ToString() => Value?.ToString();
}
