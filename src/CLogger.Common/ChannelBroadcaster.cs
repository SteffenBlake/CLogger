using System.Threading.Channels;

namespace CLogger.Common;

public class ChannelBroadcaster<T>(T value, CancellationToken cancellationToken)
{
    public T Value { get; private set; } = value;
    private CancellationToken CancellationToken { get; } = cancellationToken;

    private readonly List<ChannelWriter<T>> _writers = [];
    public async Task WriteAsync(T data)
    {
        // Dont bother publishing the event if the value didnt change
        if (Equals(Value, data))
        {
            return;
        }
        Value = data;

        // Pre-start all the ValueTasks
        var writeTasks = _writers.Select(w => 
            w.WriteAsync(data, CancellationToken)
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
        _writers.Add(channel.Writer);
        return channel.Reader.ReadAllAsync();
    }

    public void Complete() 
    {
        foreach(var writer in _writers)
        {
            writer.Complete();
        }
    }

    public override string? ToString() => Value?.ToString();
}
