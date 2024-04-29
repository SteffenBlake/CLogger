using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CLogger.Common.Messages;

public class MessageBaseConverter : JsonConverter<MessageBase>
{
    public override MessageBase? Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options
    )
    {
        var raw = JsonSerializer.Deserialize<JsonObject>(ref reader, options)!;
        
        if (raw.ContainsKey(nameof(TestStartMessage.Started)))
        {
            return raw.Deserialize<TestStartMessage>(options);
        }
        if (raw.ContainsKey(nameof(TestRunCompleteMessage.ElapsedTimeInRunningTests)))
        {
            return raw.Deserialize<TestRunCompleteMessage>(options);
        }
        if (raw.ContainsKey(nameof(TestResultMessage.Outcome)))
        {
            return raw.Deserialize<TestResultMessage>(options);
        }

        throw new JsonException();
    }

    public override void Write(
        Utf8JsonWriter writer, MessageBase value, JsonSerializerOptions options
    )
    {
        JsonSerializer.Serialize(
            writer, value, value.GetType(), options
        );  
    }
}
