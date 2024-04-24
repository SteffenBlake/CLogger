using System.IO.Pipes;

var rootPath = Path.GetFullPath(Path.Join(
    Directory.GetCurrentDirectory(),
    "../../../../"
));

var adapterPath = Path.Join(
    rootPath,
    "CLogger.TestAdapter/bin/Debug/net8.0"
);
var testPath = Path.Join(
    rootPath,
    "CLogger.UnitTests/CLogger.UnitTests.csproj"
);

var pipeName = Guid.NewGuid().ToString().Replace("-", "");

var process = new System.Diagnostics.Process();
process.StartInfo.FileName = "dotnet";

List<string> dotnetTestArgs = [
    "test", 
    /* "--test-adapter-path", adapterPath, */
    "--logger", $"clogger;pipe={pipeName}",
    testPath,
];

process.StartInfo.Arguments = string.Join(" ", dotnetTestArgs);

Console.WriteLine("Invoking:");
Console.WriteLine(process.StartInfo.FileName + " " + process.StartInfo.Arguments);

process.StartInfo.CreateNoWindow = true;
process.StartInfo.UseShellExecute = true;
process.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
process.ErrorDataReceived += (_, e) => Console.Error.WriteLine(e.Data);

process.Start();

var pipeClient = new NamedPipeClientStream(
    serverName: ".",
    pipeName: pipeName,
    PipeDirection.InOut
);

await pipeClient.ConnectAsync();

using var reader = new StreamReader(pipeClient);

while (!reader.EndOfStream)
{
    Console.Write(await reader.ReadLineAsync());
}

Console.WriteLine("Stream Ended");

process.WaitForExit();

Console.WriteLine();
Console.WriteLine("Process ended");
