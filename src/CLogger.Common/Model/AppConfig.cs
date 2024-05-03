using CLogger.Common.Channels;

namespace CLogger.Common.Model;

public class AppConfig
{
    public ChannelBroadcaster<string> Path { get; }

    public AppConfig(
        ChannelBroadcaster<string> path
    )
    {
        Path = path;
    }
}
