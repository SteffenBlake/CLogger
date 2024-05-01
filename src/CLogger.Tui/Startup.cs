using CLogger.Common.Model;
using CLogger.Tui.Coroutines;
using CLogger.Tui.Models;

namespace CLogger.Tui;

public static class Startup 
{
    public static async Task RunAsync(CliOptions options)
    {
        var cancelled = new CancellationTokenSource();
        var modelState = new ModelState(cancelled.Token);

        var application = new Application(modelState);

        var appTask = application.RunAsync();

        await TestRunner.DiscoverAsync(modelState, options);

        await TestRunner.RunAsync(modelState, options);

        modelState.Complete();

        await appTask;
    }
}
