using CLogger.Common.Enums;
using CLogger.Common.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace CLogger.Common.Messages;

public class TestDiscoveryMessage : MessageBase
{
    public required string? DisplayName { get; init; }
    
    public required string FullyQualifiedName { get; init; }

    public static TestDiscoveryMessage FromTestCase(TestCase result) => new()
    {
        DisplayName = result.DisplayName,
        FullyQualifiedName = result.FullyQualifiedName
    };

    public override async Task<bool> InvokeAsync(
        ModelState modelState, CancellationToken cancellationToken
    )
    {
        var info = ToInfo();
        await modelState.DiscoveredTestAsync(info, cancellationToken);
        return false;
    }

    private TestInfo ToInfo()
    {
        return new()
        {
            State = TestState.None,
            ErrorMessage = null,
            ErrorStackTrace = null,
            Duration = null,
            StartTime = null,
            EndTime = null,
            DisplayName = DisplayName,
            FullyQualifiedName = FullyQualifiedName
        };
    }
}
