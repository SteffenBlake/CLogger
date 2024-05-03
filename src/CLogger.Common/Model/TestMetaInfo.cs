using CLogger.Common.Channels;
using CLogger.Common.Enums;

namespace CLogger.Common.Model;

public class TestMetaInfo(
    ChannelBroadcaster<AppState> state,
    ChannelBroadcaster<TimeSpan> elapsed,
    ChannelBroadcaster<int> testProcessId
)
{
    public ChannelBroadcaster<AppState> State { get; } = state; 

    public ChannelBroadcaster<TimeSpan> Elapsed { get; } = elapsed;

    public ChannelBroadcaster<int> TestProcessId { get; } = testProcessId;
}
