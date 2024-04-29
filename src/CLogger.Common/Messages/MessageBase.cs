using System.Text.Json.Serialization;

namespace CLogger.Common.Messages;

[JsonConverter(typeof(MessageBaseConverter))]
public abstract class MessageBase 
{
    public abstract Task<string> Invoke(Model modelState); 
}
