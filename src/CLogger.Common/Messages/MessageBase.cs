using System.Text.Json.Serialization;
using CLogger.Common.Model;

namespace CLogger.Common.Messages;

[JsonConverter(typeof(MessageBaseConverter))]
public abstract class MessageBase 
{
    public abstract Task InvokeAsync(ModelState modelState); 
}
