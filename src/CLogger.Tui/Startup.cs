using CLogger.Common;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.Views;
using Terminal.Gui;

namespace CLogger.Tui;

public static class Startup 
{
    public static async Task RunAsync(CliOptions options)
    {
        Application.Init();
        
        Colors.Base = ColorSchemes.Standard;
        Colors.Menu = ColorSchemes.StandardPicked;
        Colors.Dialog = ColorSchemes.StandardPicked;
        Colors.TopLevel = ColorSchemes.Standard;
        Colors.Error = ColorSchemes.Bad;

        using var cancelled = new CancellationTokenSource();
        var modelState = new ModelState(cancelled.Token);
        await options.ApplyAsync(modelState.AppConfig);

        var window = new MainWindow(modelState);
        var bindTask = window.BindAsync();

        Application.Run(window);

        Application.Shutdown();
        
        var timeoutTask = Task.Delay(5000);
        var result = await Task.WhenAny(timeoutTask, bindTask);
        if (result == timeoutTask)
        {
            // Cancel token shouldnt actually fire, as the below
            // Task should be nearly instant, unless a resource
            // Wasn't cleaned up properly
            cancelled.Cancel();
        }

        await bindTask;
    }
}
