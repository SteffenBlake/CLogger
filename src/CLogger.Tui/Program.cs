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
    "--logger", $"clogger;pipe={pipeName}",
    /* "--list-tests", */
    testPath,
];

process.StartInfo.Arguments = string.Join(" ", dotnetTestArgs);

process.StartInfo.RedirectStandardOutput = false;
process.StartInfo.RedirectStandardError = false;
process.StartInfo.RedirectStandardInput = false;
process.StartInfo.UseShellExecute = true;

process.StartInfo.CreateNoWindow = true;
process.OutputDataReceived += (_, e) => 
{
    if (e.Data == null || !e.Data.StartsWith("Process Id:"))
    {
        return;
    }
    var procIdRaw = e.Data.Split(" ")[2];
    Console.WriteLine($"PROCESS ID: {procIdRaw}");
};

process.StartInfo.EnvironmentVariables["VSTEST_HOST_DEBUG"] = "1";

process.Start();

var pipeServer = new NamedPipeServerStream(
    pipeName: pipeName,
    PipeDirection.InOut
);

await pipeServer.WaitForConnectionAsync();

using var reader = new StreamReader(pipeServer);

while (!reader.EndOfStream)
{
    Console.WriteLine(await reader.ReadLineAsync());
}

Console.WriteLine("Stream Ended");

process.WaitForExit();

Console.WriteLine();
Console.WriteLine("Process ended");
