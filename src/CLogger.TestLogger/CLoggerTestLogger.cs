using System.IO.Pipes;
using System.Text.Json;
using CLogger.Common.Messages;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace CLogger.TestLogger;

[FriendlyName(FriendlyName)]
[ExtensionUri(ExtensionUri)]
public class CLoggerTestLogger : ITestLoggerWithParameters
{
    public const string ExtensionUri = "logger://SteffenBlake/CLogger/v1";
    public const string FriendlyName = "clogger";

    public void Initialize(TestLoggerEvents _, string __) =>
        throw new NotImplementedException();

    private NamedPipeClientStream? _client;
    private StreamWriter? _writer;

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

        _client = new NamedPipeClientStream(
            ".",
            pipe,
            PipeDirection.InOut
        );

        _client.Connect();
        _writer = new StreamWriter(_client)
        {
            AutoFlush = true
        };

        events.TestRunComplete += OnTestRunComplete;
        events.TestResult += OnTestResult;
        events.DiscoveredTests += OnDiscoveredTests;
    }

    private void OnTestRunComplete(object? _, TestRunCompleteEventArgs e)
    {
        WriteData(TestRunCompleteMessage.FromArgs(e));
        _writer!.Dispose();
        _client!.Dispose();
    }

    private void OnTestResult(object? _, TestResultEventArgs e)
    {
        WriteData(
            Common.Messages.TestResultMessage.FromTestResult(e.Result)
        );
    }

    private void OnDiscoveredTests(object? sender, DiscoveredTestsEventArgs e)
    {
        if (e.DiscoveredTestCases == null)
        {
            return;
        }

        foreach(var discovered in e.DiscoveredTestCases)
        {
            WriteData(
                TestDiscoveryMessage.FromTestCase(discovered)
            );
        }
    }

    private void WriteData<T>(T data)
        where T : MessageBase
    {
       _writer!.WriteLine(JsonSerializer.Serialize<MessageBase>(data)); 
    }
}
