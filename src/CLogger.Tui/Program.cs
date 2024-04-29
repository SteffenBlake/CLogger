using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Serialization;
using CLogger.Common;
using CLogger.Common.Messages;
using CLogger.Tui.Models;
using CommandLine;
using MessagePack;

try 
{
    var parser = new Parser(options =>
    {
        options.AutoHelp = true;
        options.CaseSensitive = false;
        options.IgnoreUnknownArguments = false;
        options.CaseInsensitiveEnumValues = true;
        options.HelpWriter = Console.Out;
    });
    
    _ = await parser.ParseArguments<CliOptions>(args)
        .WithNotParsed(errors => {
            foreach(var error in errors)
            {
                Console.Error.WriteLine(error);
            }
        }).WithParsedAsync(Run);
}
// Don't log Operation Cancel Exceptions as its intended to happen
catch (OperationCanceledException) {}

static async Task Run(CliOptions options)
{
    /* if (options.Debug) */
    /* { */
    /*     Console.WriteLine( */
    /*         $"Waiting for debugger to attach, ProcessId: {Environment.ProcessId}" */
    /*     ); */
    /* } */
    /* while (options.Debug && !Debugger.IsAttached) */
    /* { */
    /*     await Task.Delay(1000); */
    /* } */

    var modelState = new Model();
    var cancelled = new CancellationTokenSource();
    var pipeName = Guid.NewGuid().ToString().Replace("-", "");
   
    await Task.WhenAll(
        RunTests(options, modelState, cancelled),
        HandleInput(modelState, cancelled.Token)
    );
}

static async Task RunTests(
    CliOptions options,
    Model modelState, 
    CancellationTokenSource cancellationTokenSource
)
{
    var process = new System.Diagnostics.Process();
    process.StartInfo.FileName = "dotnet";
    
    List<string> dotnetTestArgs = [
        "test",
        "--logger", $"clogger;pipe={modelState.PipeName}",
        /* "--list-tests", */
    ];
    if (options.Path != ".")
    {
        dotnetTestArgs.Add(Path.GetFullPath(options.Path));
    }
    
    process.StartInfo.Arguments = string.Join(" ", dotnetTestArgs);
    
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.EnableRaisingEvents = true;
    
    process.StartInfo.CreateNoWindow = true;
    process.OutputDataReceived += (_, e) => 
    {
        if (e.Data == null || !e.Data.StartsWith("Process Id:"))
        {
            return;
        }
        var procIdRaw = e.Data.Split(" ")[2][0..^1];
        Console.WriteLine($"PROCESS ID: {procIdRaw}");
    };
   
    if (options.Debug)
    {
        process.StartInfo.EnvironmentVariables["VSTEST_HOST_DEBUG"] = "1";
    }
   
    process.ErrorDataReceived += (_, _) => cancellationTokenSource.Cancel();
    process.Exited += (_, _) => cancellationTokenSource.Cancel();

    process.Start();
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    await process.WaitForExitAsync();
}


static async Task HandleInput(
    Model modelState, 
    CancellationToken cancellationToken
)
{
    var pipeServer = new NamedPipeServerStream(
        pipeName: modelState.PipeName,
        PipeDirection.InOut
    );
    
    await pipeServer.WaitForConnectionAsync(cancellationToken);

    using var reader = new StreamReader(pipeServer);

    while (
        !cancellationToken.IsCancellationRequested &&
        modelState.Running &&
        !reader.EndOfStream
    )
    {
        var next = await reader.ReadLineAsync(cancellationToken)
            ?? throw new InvalidOperationException();
        var msg = JsonSerializer.Deserialize<MessageBase>(next)
            ?? throw new InvalidOperationException();
        var output = await msg.Invoke(modelState);
        Console.WriteLine(output);
    }
}
