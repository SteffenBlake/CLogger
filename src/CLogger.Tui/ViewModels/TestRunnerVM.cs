using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;
using CLogger.Common.Enums;
using CLogger.Common.Messages;
using CLogger.Common.Model;
using CLogger.Tui.Models;

namespace CLogger.Tui.ViewModels;

public class TestRunnerVM(
    ModelState modelState,
    CliOptions cliOptions
) : IViewModel
{
    private ModelState ModelState { get; } = modelState;
    private CliOptions CliOptions { get; } = cliOptions;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
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
        if (ModelState.MetaInfo.State.Value != AppState.Idle)
        {
            return;
        }

        await ModelState.MetaInfo.State.WriteAsync(AppState.Busy, cancellationToken);

        var pipeName = Guid.NewGuid().ToString();
        await Task.WhenAll(
            RunDotnetTestAsync(pipeName, eventArgs, cancellationToken),
            HandleDotnetOutputAsync(pipeName, cancellationToken)
        );

        await ModelState.MetaInfo.State.WriteAsync(AppState.Idle, cancellationToken);
    }

    private async Task RunDotnetTestAsync(
        string pipeName,
        RunTestsArgs eventArgs,
        CancellationToken cancellationToken
    )
    {
        using var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.EnableRaisingEvents = true;
        process.StartInfo.CreateNoWindow = true;
        
        List<string> dotnetTestArgs = [
            "test",
            "--logger", $"clogger;pipe={pipeName}",
        ];

        if (eventArgs.Discover)
        {
            dotnetTestArgs.Add("--list-tests");
        }
        if (eventArgs.TestIds.Any())
        {
            // https://learn.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=nunit#character-escaping 
            dotnetTestArgs.Add("--filter");
            var prefixed = eventArgs.TestIds.Select(id =>
                $"FullyQualifiedName={id.Replace(",", "%2C")}"
            ).ToList();
            dotnetTestArgs.Add(string.Join('|', prefixed));
        }

        if (ModelState.Config.Path.Value != ".")
        {
            dotnetTestArgs.Add(Path.GetFullPath(ModelState.Config.Path.Value));
        }
        
        process.StartInfo.Arguments = string.Join(" ", dotnetTestArgs);

        if (eventArgs.Debug)
        {
            process.StartInfo.EnvironmentVariables["VSTEST_HOST_DEBUG"] = "1";
            process.OutputDataReceived += (_, e) => 
            {
                if (e.Data == null || !e.Data.StartsWith("Process Id:"))
                {
                    return;
                }
                var procIdRaw = e.Data.Split(" ")[2][0..^1];
                var procId = int.Parse(procIdRaw);
                // Have to do stuff Sync inside events because events suck
                ModelState
                    .MetaInfo
                    .TestProcessId
                    .WriteAsync(procId, cancellationToken)
                    .Wait();
            };
        }
       
        process.Start();
        process.BeginOutputReadLine();
    
        await process.WaitForExitAsync(cancellationToken);
    }

    private async Task HandleDotnetOutputAsync(
        string pipeName,
        CancellationToken cancellationToken
    )
    {
        using var pipeServer = new NamedPipeServerStream(
            pipeName: pipeName,
            PipeDirection.InOut
        );
       
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        var actual = await Task.WhenAny(
            timeoutTask,
            pipeServer.WaitForConnectionAsync(cancellationToken)
        );
        if (actual == timeoutTask)
        {
            return;
        }

        using var reader = new StreamReader(pipeServer);
   
        while (
            !cancellationToken.IsCancellationRequested &&
            ModelState.MetaInfo.State.Value == AppState.Busy &&
            !reader.EndOfStream
        )
        {
            var next = await reader.ReadLineAsync(cancellationToken)
                ?? throw new InvalidOperationException();

            var msg = JsonSerializer.Deserialize<MessageBase>(next)
                ?? throw new InvalidOperationException();

            await msg.InvokeAsync(ModelState, cancellationToken);
        }
    }
}
