using System.Threading.Channels;

namespace CLogger.Common.Channels;

public interface IChannelBroadcaster
{
    bool TryComplete();
}

public class ChannelBroadcaster<T> : IChannelBroadcaster
{
    public T Value { get; private set; } = default!;

    public ChannelBroadcaster(ChannelBroadcasterContainer container)
    {
        container.Register(this);
    }

    private readonly List<ChannelWriter<T>> _writers = [];

    public async Task WriteAsync(T data, CancellationToken cancellationToken)
    {
        // Don't bother publishing the event if the value didn't change
        if (Equals(Value, data))
        {
            return;
        }

        Value = data;

        // Pre-start all the ValueTasks
        var writeTasks = _writers.Select(w => 
            w.WriteAsync(data, cancellationToken)
        ).ToList();

        // Await them (cant use Task.WhenAll with ValueTasks)
        foreach(var writeTask in writeTasks)
        {
            await writeTask;
        }
    }

    private bool _completed = false;
    public IAsyncEnumerable<T> Subscribe(CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<T>();
        if (_completed)
        {
            channel.Writer.Complete();
        }
        _writers.Add(channel.Writer);
        return channel.Reader.ReadAllAsync(cancellationToken);
    }

    public bool TryComplete() 
    {
        _completed = true;
        Console.WriteLine($"{GetType()} Completed");
        return _writers.All(t => t.TryComplete());
    }

    public override string? ToString() => Value?.ToString();
}
