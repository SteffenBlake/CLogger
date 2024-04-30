using CLogger.Common.Enums;

namespace CLogger.Common.Model;

public class TestInfo 
{
    public required TestState State { get; set; } 

    public required string? ErrorMessage { get; set; }

    public required string? ErrorStackTrace { get; set; }

    public required TimeSpan? Duration { get; set; }

    public required DateTimeOffset? StartTime { get; set; }

    public required DateTimeOffset? EndTime { get; set; }

    public required string? DisplayName { get; set; }
    
    public required string FullyQualifiedName { get; set; }
}
