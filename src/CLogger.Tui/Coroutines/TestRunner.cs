using System.IO.Pipes;
using System.Text.Json;
using CLogger.Common.Enums;
using CLogger.Common.Messages;
using CLogger.Common.Model;

namespace CLogger.Tui.Coroutines;

public static class TestRunner
{
    public static async Task DiscoverAsync(ModelState modelState)
    {
        Console.WriteLine("Discovery Started");
        await ExecuteAsync(modelState, discover:true);
        Console.WriteLine("Discovery Finished");
    }

    public static async Task RunAsync(ModelState modelState)
    {
        await ExecuteAsync(modelState, discover:false);
    }

    private static async Task ExecuteAsync(
        ModelState modelState,
        bool discover
    )
    {
        if (modelState.AppState.Value == AppState.Busy)
        {
            return;
        }
       
        await modelState.AppState.WriteAsync(AppState.Busy);

        var pipeName = Guid.NewGuid().ToString();

        await Task.WhenAll(
            RunDotnetTestAsync(
                modelState,
                pipeName,
                discover
            ),
            HandleDotnetOutputAsync(
                modelState,
                pipeName
            )
        );

        await modelState.AppState.WriteAsync(AppState.Idle);
    }

    private static async Task RunDotnetTestAsync(
        ModelState modelState,
        string pipeName,
        bool discover
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

        if (discover)
        {
            dotnetTestArgs.Add("--list-tests");
        }

        if (modelState.AppConfig.Path.Value != ".")
        {
            dotnetTestArgs.Add(Path.GetFullPath(modelState.AppConfig.Path.Value));
        }
        
        process.StartInfo.Arguments = string.Join(" ", dotnetTestArgs);

        if (modelState.AppConfig.DebugMode.Value)
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
                modelState.MetaInfo.TestProcessId.WriteAsync(procId).Wait();
            };
        }
       
        process.Start();
        process.BeginOutputReadLine();
    
        await process.WaitForExitAsync(modelState.CancellationToken);
    }

    private static async Task HandleDotnetOutputAsync(
        ModelState modelState, 
        string pipeName
    )
    {
        using var pipeServer = new NamedPipeServerStream(
            pipeName: pipeName,
            PipeDirection.InOut
        );
       
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

        var actual = await Task.WhenAny(
            timeoutTask,
            pipeServer.WaitForConnectionAsync(modelState.CancellationToken)
        );
        if (actual == timeoutTask)
        {
            return;
        }

        using var reader = new StreamReader(pipeServer);
    
        while (
            !modelState.CancellationToken.IsCancellationRequested &&
            modelState.AppState.Value == AppState.Busy &&
            !reader.EndOfStream
        )
        {
            var next = await reader.ReadLineAsync(modelState.CancellationToken)
                ?? throw new InvalidOperationException();
            var msg = JsonSerializer.Deserialize<MessageBase>(next)
                ?? throw new InvalidOperationException();

            await msg.InvokeAsync(modelState);
        }
    }
}
