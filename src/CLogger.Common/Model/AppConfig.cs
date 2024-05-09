using CLogger.Common.Channels;

namespace CLogger.Common.Model;

public class AppConfig(
    ChannelValueBroadcaster<string> path
)
{
    public ChannelValueBroadcaster<string> Path { get; } = path;
}
