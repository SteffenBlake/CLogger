using System.IO.Pipes;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace CLogger.TestAdapter;

[FriendlyName(FriendlyName)]
[ExtensionUri(ExtensionUri)]
public class CLoggerTestAdapter : ITestLoggerWithParameters
{
    public const string ExtensionUri = "logger://SteffenBlake/CLogger/v1";
    public const string FriendlyName = "clogger";

    public void Initialize(TestLoggerEvents _, string __) =>
        throw new NotImplementedException();

    public void Initialize(
        TestLoggerEvents events, Dictionary<string, string?> parameters
    )
    {
        if (
            !parameters.TryGetValue("pipe", out var pipe) || 
            string.IsNullOrEmpty(pipe)
        )
        {
            throw new ArgumentException("'pipe' parameter is required");
        }

        var server = new NamedPipeServerStream(
            pipe, 
            PipeDirection.InOut
        );

        server.WaitForConnection();
        var writer = new StreamWriter(server);

        writer.WriteLine("CLIENT: Initializing...");
        events.TestRunStart += (_, e) => OnTestStart(e, writer);
        events.TestRunComplete += (_, e) => OnTestRunComplete(e, server, writer);
        events.TestResult += (_, e) => OnTestResult(e, writer);
        events.TestRunMessage += (_, e) => OnTestRunMessage(e, writer);
    }

    private static void OnTestStart(
        TestRunStartEventArgs e, StreamWriter writer
    )
    {
        writer.WriteLine("CLIENT: Test started");
    }

    private static void OnTestRunComplete(
        TestRunCompleteEventArgs e, 
        NamedPipeServerStream server, 
        StreamWriter writer
    )
    {
        writer.WriteLine("CLIENT: Tests ended");
        server.Flush();
        server.Dispose();
        writer.Flush();
        writer.Dispose();
    }

    private static void OnTestResult(TestResultEventArgs e, StreamWriter writer)
    {
        var raw = JsonSerializer.Serialize(e.Result);
        writer!.WriteLine("CLIENT: " + raw);
    }

    private static void OnTestRunMessage(TestRunMessageEventArgs e, StreamWriter writer)
    {
        writer!.WriteLine("CLIENT: " + e.Message);
    }
}
