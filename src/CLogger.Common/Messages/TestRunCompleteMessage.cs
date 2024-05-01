using CLogger.Common.Enums;
using CLogger.Common.Model;
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

    public override async Task InvokeAsync(ModelState modelState)
    {
       await modelState.MetaInfo.Elapsed.WriteAsync(ElapsedTimeInRunningTests);
    }
}
