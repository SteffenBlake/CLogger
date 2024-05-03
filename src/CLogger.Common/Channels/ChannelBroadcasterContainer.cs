namespace CLogger.Common.Channels;

public class ChannelBroadcasterContainer 
{
    private readonly List<IChannelBroadcaster> _broadcasters = [];

    public void Register(IChannelBroadcaster broadcaster)
    {
        _broadcasters.Add(broadcaster);
    }

    public bool TryComplete()
    {
        var result = true;
        foreach(var broadcaster in _broadcasters)
        {
            result &= broadcaster.TryComplete();
        }
        return result;
    }
}
