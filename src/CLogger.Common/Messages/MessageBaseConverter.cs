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
        
        if (raw.ContainsKey(nameof(TestRunCompleteMessage.ElapsedTimeInRunningTests)))
        {
            return raw.Deserialize<TestRunCompleteMessage>(options);
        }
        // Both TestResult and TestDiscovery have the DisplayName field
        // But only TestResult has the Outcome field, so check that first
        // To distinguish them
        if (raw.ContainsKey(nameof(TestResultMessage.Outcome)))
        {
            return raw.Deserialize<TestResultMessage>(options);
        }
        if (raw.ContainsKey(nameof(TestDiscoveryMessage.DisplayName)))
        {
            return raw.Deserialize<TestDiscoveryMessage>(options);
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
