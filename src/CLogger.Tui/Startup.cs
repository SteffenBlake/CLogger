using CLogger.Common.Channels;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.ViewModels;
using CLogger.Tui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;

namespace CLogger.Tui;

public static class Startup 
{
    public static async Task RunAsync(CliOptions options)
    {
        using var cancelled = new CancellationTokenSource();

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(cancelled);

        // Model
        builder.Services.AddSingleton<ModelState>();
        builder.Services.AddSingleton<AppConfig>();
        builder.Services.AddSingleton<TestMetaInfo>();
        
        // Services
        builder.Services.AddSingleton<ChannelBroadcasterContainer>();
        builder.Services.AddTransient(typeof(ChannelBroadcaster<>));

        // Views
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<ActionBar>();
        builder.Services.AddSingleton<TestExplorer>();
        builder.Services.AddSingleton<InfoPanel>();
        builder.Services.AddSingleton<InfoBar>();

        // ViewModels
        builder.Services.AddSingleton<ViewModelBinder>();
        builder.Services.AddSingleton<IViewModel, TestExplorerVM>(); 
        builder.Services.AddSingleton<IViewModel, TestRunnerVM>(); 

        var app = builder.Build();

        var appConfig = app.Services.GetRequiredService<AppConfig>();
        await options.ApplyAsync(appConfig, cancelled.Token);

        var binder = app.Services.GetRequiredService<ViewModelBinder>();
        var binderTask = binder.BindAsync(cancelled.Token);
        var mainWindow = app.Services.GetRequiredService<MainWindow>();

        Application.Run(mainWindow);

        Application.Shutdown();
        
        var timeoutTask = Task.Delay(5000);
        var result = await Task.WhenAny(timeoutTask, binderTask);
        if (result == timeoutTask)
        {
            // Cancel token shouldnt actually fire, as the below
            // Task should be nearly instant, unless a resource
            // Wasn't cleaned up properly
            cancelled.Cancel();
        }

        await binderTask;

    }
}
