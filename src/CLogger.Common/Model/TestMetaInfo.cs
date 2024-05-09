using CLogger.Common.Channels;
using CLogger.Common.Enums;

namespace CLogger.Common.Model;

public class TestMetaInfo(
    ChannelValueBroadcaster<AppState> state,
    ChannelValueBroadcaster<TimeSpan> elapsed,
    ChannelValueBroadcaster<string> testProcessId
)
{
    public ChannelValueBroadcaster<AppState> State { get; } = state; 

    public ChannelValueBroadcaster<TimeSpan> Elapsed { get; } = elapsed;

    public ChannelValueBroadcaster<string> TestProcessId { get; } = testProcessId;
}
