using System.Diagnostics;
using System.Net;
using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using Microsoft.Extensions.Logging;

namespace CLogger.Tui.ViewModels;

public class TestRunnerVM(
    ModelState modelState,
    CliOptions cliOptions,
    ILogger<TestRunnerVM> logger,
    ILoggerFactory loggerFactory
) : IViewModel
{
    private ModelState ModelState { get; } = modelState;
    private CliOptions CliOptions { get; } = cliOptions;
    private ILogger<TestRunnerVM> Logger { get; } = logger;

    // Clogger.Dotnet.log special logger
    // Specifically for logging dotnet test subprocess logs
    public ILogger DotnetLogger { get; } 
        = loggerFactory.CreateLogger("DotnetTest");

    private readonly List<string> _filterEscapeChars = 
        ["\\", "(", ")", "&", "|", "=", "!", "~"];

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Waiting for startup to finish...");

        // Wait for startup to be done
        while (ModelState.MetaInfo.State.Value == AppState.Starting)
        {
            await Task.Delay(500, cancellationToken);
        }

        Logger.LogInformation("Startup done, commencing first run...");

        var startupArgs = new RunTestsArgs(
            Discover: !CliOptions.Run,
            Debug: CliOptions.Debug,
            TestIds: []
        );
        await ExecuteAsync(startupArgs, cancellationToken);

        var events = ModelState.OnRunTests.Subscribe(cancellationToken);
        await foreach(var eventArgs in events)
        {
            await ExecuteAsync(eventArgs, cancellationToken);
        }
    }

    private async Task ExecuteAsync(
        RunTestsArgs eventArgs, CancellationToken cancellationToken
    )
    {
        Logger.LogInformation("Starting up test runner...");
        if (ModelState.MetaInfo.State.Value != AppState.Idle)
        {
            Logger.LogInformation("App state not idle, short circuiting...");
            return;
        }

        Logger.LogInformation("Setting app state to busy.");
        var runState = eventArgs.Debug ? AppState.Debugging : AppState.Running;
        await ModelState.MetaInfo.State.WriteAsync(runState, cancellationToken);

        Logger.LogInformation(
            "Loading dotnet test process with port: {ip}:{port}", 
            CliOptions.Domain,
            ModelState.MetaInfo.Port.Value
        );

        var dotnetTestProcess = SetupDotnetTest(eventArgs);

        var innerCancelled = new CancellationTokenSource();
        var combinedCancelled = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, innerCancelled.Token
        ).Token;
       
        try {
            // Spin up the three parallel operations
            var cancelTask = WatchAppStateAsync(
                eventArgs, dotnetTestProcess, combinedCancelled
            );

            var runTask = RunDotnetTestAsync(
                dotnetTestProcess, eventArgs, combinedCancelled
            );
            
            Logger.LogInformation("Waiting for any test runner task to complete.");
            var firstDone = await Task.WhenAny(cancelTask, runTask);
           
            if (firstDone == cancelTask)
            {
                Logger.LogInformation(
                    "Cancellation was requested, cancelling inner token early"
                );

                await innerCancelled.CancelAsync();
            }

            await ModelState.MetaInfo.State.WriteAsync(AppState.Finishing, cancellationToken);

            Logger.LogInformation("Waiting for all proc tasks to complete now.");
            await Task.WhenAll(cancelTask, runTask);
        }
        catch (OperationCanceledException){}

        Logger.LogInformation("Setting app state back to Idle");
        await ModelState.MetaInfo.State.WriteAsync(AppState.Idle, cancellationToken);
    }

    private Process SetupDotnetTest(RunTestsArgs eventArgs)
    {
        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.EnableRaisingEvents = true;
        process.StartInfo.CreateNoWindow = true;
        
        List<string> dotnetTestArgs = [
            "test",
            "--logger", 
            $"clogger;domain={CliOptions.Domain};port={ModelState.MetaInfo.Port.Value}",
        ];

        if (eventArgs.Discover)
        {
            dotnetTestArgs.Add("--list-tests");
        }

        if (eventArgs.TestIds.Any())
        {
            // https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=nunit#character-escaping 
            dotnetTestArgs.Add("--filter");
            var filters = new List<string>();
            foreach(var id in eventArgs.TestIds)
            {
                var escapedId = id;
                // https://github.com/Microsoft/vstest-docs/blob/main/docs/filter.md#syntax 
                foreach(var c in _filterEscapeChars)
                {
                    escapedId = escapedId.Replace(c, $"\\{c}");
                }
                // https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=mstest#character-escaping 
                escapedId = escapedId.Replace(",", "%2C");

                filters.Add($"FullyQualifiedName={escapedId}");
            }

            var quoteWrapped = "\"" + string.Join('|', filters) + "\"";
            dotnetTestArgs.Add(quoteWrapped);
        }

        if (CliOptions.Path != ".")
        {
            dotnetTestArgs.Add(Path.GetFullPath(CliOptions.Path));
        }
        
        process.StartInfo.Arguments = string.Join(" ", dotnetTestArgs);

        if (eventArgs.Debug)
        {
            process.StartInfo.EnvironmentVariables["VSTEST_HOST_DEBUG"] = "1";
            DotnetLogger.LogDebug("Debug mode! Exporting: VSTEST_HOST_DEBUG=1");
        }

        return process;
    }

    private async Task RunDotnetTestAsync(
        Process process,
        RunTestsArgs eventArgs,
        CancellationToken cancellationToken
    )
    {
        DotnetLogger.LogDebug("Executing following command:");
        DotnetLogger.LogDebug(
            "    {fileName} {args}", 
            process.StartInfo.FileName, 
            process.StartInfo.Arguments
        );

        process.Start();

        Logger.LogInformation("Waiting for dotnet test to finish...");
        var procTask = process.WaitForExitAsync(cancellationToken);

        var stdOutTask = HandleStdOutAsync(
            eventArgs, process, cancellationToken
        );
        var errOutTask = HandleStdErrorAsync(
            process, cancellationToken
        );

        await Task.WhenAll(procTask, stdOutTask, errOutTask);

        Logger.LogInformation("dotnet test finished!");
    }

    private async Task WatchAppStateAsync(
        RunTestsArgs eventArgs, 
        Process process, 
        CancellationToken cancellationToken
    )
    {
        Logger.LogInformation("Listening for cancel request...");
        var events = ModelState.MetaInfo.State.Subscribe(cancellationToken, out var id);
        await foreach(var appState in events)
        {
            if (appState == AppState.Idle || appState == AppState.Finishing)
            {
                Logger.LogInformation("App completed.");
                break;
            }
            else if (appState == AppState.Cancelling)
            {
                Logger.LogInformation("Test cancellation requested");
                ModelState.MetaInfo.State.TryUnsubscribe(id);

                process.Kill(entireProcessTree:true);
                await ModelState.CancelTestsAsync(eventArgs.TestIds, cancellationToken);

                Logger.LogInformation("Tests cancelled requested");
                break;
            }
        }
        Logger.LogInformation("Cancel task has completed");
    }

    private async Task HandleStdOutAsync(
        RunTestsArgs eventArgs, Process process, CancellationToken cancellationToken
    )
    {
        Logger.LogInformation("Listening on StandardOut of dotnet test...");
        while (!cancellationToken.IsCancellationRequested)
        {
            var nextRaw = await process.StandardOutput.ReadLineAsync(cancellationToken);
            if (nextRaw == null)
            {
                break;
            }

            var next = nextRaw.Trim()!;
            if (string.IsNullOrEmpty(next))
            {
                continue;
            }

            DotnetLogger.LogInformation(next!);

            if (!eventArgs.Debug)
            {
                continue;
            }

            // Check for Process Id (only in debug mode)
            if (!next.StartsWith("Process Id:"))
            {
                continue;
            }

            var procId = next.Split(" ")[2][0..^1];
            Logger.LogInformation("Process Id Recieved: {procId}", procId);

            await ModelState
                .MetaInfo
                .TestProcessId
                .WriteAsync(procId, cancellationToken);

            Logger.LogInformation("Process Id written to channel successfully");
        }

        Logger.LogInformation("Standard Out task completed successfully!");
    }

    private async Task HandleStdErrorAsync(
        Process process, CancellationToken cancellationToken
    )
    {
        Logger.LogInformation("Listening on StandardErr of dotnet test...");
        while (
            !cancellationToken.IsCancellationRequested
        )
        {
            var nextRaw = await process.StandardError.ReadLineAsync(cancellationToken);
            if (nextRaw == null)
            {
                break;
            }

            var next = nextRaw?.Trim();
            if (string.IsNullOrEmpty(next))
            {
                continue;
            }

            DotnetLogger.LogError(next!);
        }
        Logger.LogInformation("Standard Error task completed successfully!");
    }
}
