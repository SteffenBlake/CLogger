using CLogger.Common.Enums;
using CLogger.Common.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace CLogger.Common.Messages;

public class TestResultMessage : MessageBase 
{
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

    public override async Task InvokeAsync(
        ModelState modelState, CancellationToken cancellationToken
    )
    {
        var info = ToInfo();
        await modelState.TestResultAsync(info, cancellationToken); 
    }

    private TestInfo ToInfo()
    {
        return new()
        {
            State = (TestState)Outcome,
            ErrorMessage = ErrorMessage,
            ErrorStackTrace = ErrorStackTrace,
            Duration = Duration,
            StartTime = StartTime,
            EndTime = EndTime,
            DisplayName = DisplayName,
            FullyQualifiedName = FullyQualifiedName
        };
    }
}
