using System.Threading.Channels;

namespace CLogger.Common;

public class ChannelBroadcaster<T>(ChannelReader<T> dataStream)
{
    private ChannelReader<T> DataStream { get; } = dataStream;

    private readonly List<ChannelWriter<T>> _writers = [];

    public async Task PublishAsync(CancellationToken cancellationToken)
    {
        await foreach(var next in DataStream.ReadAllAsync())
        {
            var writeTasks = _writers.Select(w => 
                w.WriteAsync(next, cancellationToken)
            );
            foreach(var writeTask in writeTasks)
            {
                await writeTask;
            }
        }

        foreach(var writer in _writers)
        {
            writer.Complete();
        }
    }

    public ChannelReader<T> Subscribe()
    {
        var channel = Channel.CreateUnbounded<T>();
        _writers.Add(channel.Writer);
        return channel.Reader;
    }
}
