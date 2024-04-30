using CLogger.Common.Model;
using CLogger.Tui.Coroutines;
using CLogger.Tui.Models;

namespace CLogger.Tui;

public static class Startup 
{
    public static async Task RunAsync(CliOptions options)
    {
        var modelState = new ModelState();

        var application = new Application(modelState);

        var publishTask = modelState.PublishAsync();
        var appTask = application.RunAsync();

        await TestRunner.DiscoverAsync(modelState, options);

        await TestRunner.RunAsync(modelState, options);

        await modelState.CloseAsync();

        await Task.WhenAll(publishTask, appTask);
    }
}
