using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace CLogger.Common.Messages;

public class TestResultMessage : MessageBase 
{
    public string TestResult { get; init; } = nameof(TestResult);

    public required TestOutcome Outcome { get; init; } 

    public required string? ErrorMessage { get; init; }

    public required string? ErrorStackTrace { get; init; }

    public required TimeSpan Duration { get; init; }

    public required DateTimeOffset StartTime { get; init; }

    public required DateTimeOffset EndTime { get; init; }

    public required string? DisplayName { get; init; }
    
    public required string FullyQualifiedName { get; init; }

    public static TestResultMessage FromTestResult(TestResult result) => new()
    {
        Outcome = result.Outcome,
        ErrorMessage = result.ErrorMessage,
        ErrorStackTrace = result.ErrorStackTrace,
        Duration = result.Duration,
        StartTime = result.StartTime,
        EndTime = result.EndTime,
        DisplayName = result.DisplayName,
        FullyQualifiedName = result.TestCase.FullyQualifiedName
    };

    public override Task<string> Invoke(Model modelState)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        return Task.FromResult(JsonSerializer.Serialize(this, options));
    }
}
