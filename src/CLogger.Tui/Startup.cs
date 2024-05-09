using CLogger.Common.Channels;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.ViewModels;
using CLogger.Tui.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;
using NLog.Extensions.Logging;
using NLog.Config;
using NLog.Targets;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace CLogger.Tui;

public static class Startup 
{
    public static async Task RunAsync(CliOptions options)
    {
        Application.Init();

        try {
            Application.Driver.UnChecked = new Rune('');
            Application.Driver.Checked = new Rune('');

            Application.IsMouseDisabled = true;
            Application.Driver.StopReportingMouseMoves();
            
            Colors.Base = ColorSchemes.Standard;
            Colors.Menu = ColorSchemes.Standard;
            Colors.Dialog = ColorSchemes.Standard;
            Colors.TopLevel = ColorSchemes.Standard;
            Colors.Error = ColorSchemes.Bad;

            using var cancelled = new CancellationTokenSource();

            var builder = Host.CreateApplicationBuilder();
            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton(cancelled);

            AddNLogging(builder.Logging);

            // Model
            builder.Services.AddSingleton<ModelState>();
            builder.Services.AddSingleton<AppConfig>();
            builder.Services.AddSingleton<TestMetaInfo>();
            
            // Services
            builder.Services.AddSingleton<ChannelBroadcasterContainer>();
            builder.Services.AddTransient(typeof(ChannelBroadcaster<>));
            builder.Services.AddTransient(typeof(ChannelValueBroadcaster<>));

            // Views
            builder.Services.AddSingleton<MainWindow>();
            builder.Services.AddSingleton<ActionBar>();
            builder.Services.AddSingleton<TestExplorer>();
            builder.Services.AddSingleton<InfoPanel>();
            builder.Services.AddSingleton<InfoBar>();
            builder.Services.AddSingleton<ProcessIdDialog>();

            // ViewModels
            builder.Services.AddSingleton<ViewModelBinder>();
            builder.Services.AddSingleton<IViewModel, TestRunnerVM>(); 
            builder.Services.AddSingleton<IViewModel, MainWindowVM>(); 
            builder.Services.AddSingleton<IViewModel, ActionBarVM>(); 
            builder.Services.AddSingleton<IViewModel, TestExplorerVM>(); 
            builder.Services.AddSingleton<IViewModel, InfoPanelVM>(); 
            builder.Services.AddSingleton<IViewModel, InfoBarVM>(); 
            builder.Services.AddSingleton<IViewModel, KeybindsVM>(); 
            builder.Services.AddSingleton<IViewModel, ProcessIdDialogVM>(); 
            
            var app = builder.Build();

            var appConfig = app.Services.GetRequiredService<AppConfig>();
            await options.ApplyAsync(appConfig, cancelled.Token);

            var binder = app.Services.GetRequiredService<ViewModelBinder>();
            var binderTask = binder.BindAsync(cancelled.Token);
            var mainWindow = app.Services.GetRequiredService<MainWindow>();

            Application.Run(mainWindow);
            Application.MainLoop.Invoke(binderTask.Wait);
            Application.Shutdown();

            cancelled.Cancel();
        }
        catch (Exception)
        {
            Application.Shutdown();
            throw;
        }
    }

    private static void AddNLogging(ILoggingBuilder logging)
    {
        var logDir = Path.Join(
            Path.GetDirectoryName(
                Assembly.GetEntryAssembly()?.Location ?? 
                    throw new InvalidOperationException()
            ),
            "var/log"
        );
        var loggingConfig = new LoggingConfiguration();

        // general logging
        loggingConfig.AddTarget(new FileTarget()
        {
            Name = "CLogger.TUI",
            FileName = Path.Join(logDir, "CLogger.TUI.log"),
            ArchiveOldFileOnStartup = true,                
            Layout = 
                "${shortdate} ${level} ${callsite}: ${message}${exception:format=ToString}",
        });
        loggingConfig.AddRule(
            NLog.LogLevel.Trace,
            NLog.LogLevel.Fatal,
            "CLogger.TUI",
            "CLogger.*"
        );

        // dotnet test sub process logging
        loggingConfig.AddTarget(new FileTarget()
        {
            Name = "DotnetTest",
            FileName = Path.Join(logDir, "CLogger.Dotnet.log"),
            ArchiveOldFileOnStartup = true,                
            Layout = 
                "${shortdate} ${level} : ${message}",
        });
        loggingConfig.AddRule(
            NLog.LogLevel.Trace,
            NLog.LogLevel.Fatal,
            "DotnetTest",
            "DotnetTest"
        );
        
        logging.ClearProviders();
        logging.AddNLog(loggingConfig);
    }
}
