using System.Threading.Channels;

namespace CLogger.Common.Channels;

public class ChannelBroadcaster<T> : IChannelBroadcaster
{
    public T Value { get; private set; } = default!;

    public ChannelBroadcaster(ChannelBroadcasterContainer container)
    {
        container.Register(this);
    }

    private readonly Dictionary<Guid, ChannelWriter<T>> _writers = [];

    public virtual async Task WriteAsync(T data, CancellationToken cancellationToken)
    {
        Value = data;

        var writeTasks = _writers.Values.Select(w => 
            w.WriteAsync(data, cancellationToken).AsTask()
        ).ToList();

        await Task.WhenAll(writeTasks);
    }

    private bool _completed = false;

    public IAsyncEnumerable<T> Subscribe(CancellationToken cancellationToken)
    {
        return Subscribe(cancellationToken, out _);
    }

    public IAsyncEnumerable<T> Subscribe(
        CancellationToken cancellationToken, out Guid id
    )
    {
        id = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<T>();
        if (_completed)
        {
            channel.Writer.Complete();
        }
        _writers[id] = channel.Writer;
        return channel.Reader.ReadAllAsync(cancellationToken);
    }

    public bool TryUnsubscribe(Guid id)
    {
        var removed = _writers.Remove(id, out var writer);
        var completed = writer?.TryComplete() ?? false;

        return removed && completed;
    }

    public bool TryComplete() 
    {
        _completed = true;
        Console.WriteLine($"{GetType()} Completed");
        return _writers.Values.All(t => t.TryComplete());
    }

    public override string? ToString() => Value?.ToString();
}
