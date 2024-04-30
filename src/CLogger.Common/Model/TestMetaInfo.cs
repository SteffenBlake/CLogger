using System.Threading.Channels;

namespace CLogger.Common.Model;

public class TestMetaInfo 
{
    public TestMetaInfo(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        OnElapsed = new(_elapsedEvents.Reader);
        OnTestProcessId = new(_procIdEvents.Reader);
    }

    private readonly CancellationToken _cancellationToken;

    private readonly Channel<bool> _elapsedEvents = Channel.CreateUnbounded<bool>();
    public ChannelBroadcaster<bool> OnElapsed { get; }

    private readonly Channel<bool> _procIdEvents = Channel.CreateUnbounded<bool>();
    public ChannelBroadcaster<bool> OnTestProcessId { get; }

    public async Task PublishAsync()
    {
        await Task.WhenAll(
            OnElapsed.PublishAsync(_cancellationToken),
            OnTestProcessId.PublishAsync(_cancellationToken)
        );
    }

    public TimeSpan Elapsed { get; private set; } = new();
    public async Task UpdateElapsedAsync(TimeSpan newTime)
    {
        if (Elapsed == newTime)
        {
            return;
        }

        Elapsed = newTime;
        await _elapsedEvents.Writer.WriteAsync(true, _cancellationToken);
    }

    public int TestProcessId { get; private set; }
    public async Task UpdateTestProcessIdAsync(int newProcId)
    {
        if (TestProcessId == newProcId)
        {
            return;
        }

        TestProcessId = newProcId;
        await _elapsedEvents.Writer.WriteAsync(true, _cancellationToken);
    }

    public Task CloseAsync()
    {
        _elapsedEvents.Writer.Complete();
        _procIdEvents.Writer.Complete();

        return Task.CompletedTask;
    }
}
