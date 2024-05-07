using CLogger.Common.Channels;
using CLogger.Common.Enums;

namespace CLogger.Common.Model;

public class TestMetaInfo(
    ChannelBroadcaster<AppState> state,
    ChannelBroadcaster<TimeSpan> elapsed,
    ChannelBroadcaster<string> testProcessId
)
{
    public ChannelBroadcaster<AppState> State { get; } = state; 

    public ChannelBroadcaster<TimeSpan> Elapsed { get; } = elapsed;

    public ChannelBroadcaster<string> TestProcessId { get; } = testProcessId;
}
