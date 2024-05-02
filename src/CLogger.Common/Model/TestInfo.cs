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

    public List<string> DeconstructPath()
    {
        var path = FullyQualifiedName.Split('.').ToList();

        // TestCases for a Test have a (...) at the end to
        // indicate the case
        // We will want to split that out to its own piece of the path
        var last = path.Last();
        if (last.EndsWith(')'))
        {
           var prefix = last.Split('(')[0];
           path.Insert(path.Count-1, prefix);
        }

        return path;
    }
}
