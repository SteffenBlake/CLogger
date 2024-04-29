using MessagePack;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace CLogger.Common.Messages;

public class TestRunCompleteMessage : MessageBase 
{
    public required TimeSpan ElapsedTimeInRunningTests { get; init; }

    public static TestRunCompleteMessage FromArgs(TestRunCompleteEventArgs args)
    {
        return new ()
        {
           ElapsedTimeInRunningTests = args.ElapsedTimeInRunningTests 
        };
    }

    public override Task<string> Invoke(Model modelState)
    {
        modelState.Running = false;

        return Task.FromResult("Test Run Completed");
    }
}
