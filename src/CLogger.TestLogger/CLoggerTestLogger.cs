using System.Collections;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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

    private static readonly List<Type> IgnoreList = [ 
        typeof(IEnumerable<TestProperty>) 
    ];
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            .WithAddedModifier(t => 
            {
                if (t.Kind != JsonTypeInfoKind.Object)
                {
                    return;
                }
                var toRemove = t.Properties
                    .Where(p => IgnoreList.Contains(p.PropertyType))
                    .ToList();

                foreach(var removal in toRemove)
                {
                    t.Properties.Remove(removal);
                }
            })
    };

    public void Initialize(
        TestLoggerEvents events, Dictionary<string, string?> parameters
    )
    {
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

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

        _writer.WriteLine("Logger: Initializing...");
        events.TestRunStart += OnTestStart;
        events.TestRunComplete += OnTestRunComplete;
        events.TestResult += OnTestResult;
        events.TestRunMessage += OnTestRunMessage;
        events.DiscoveredTests += OnDiscoveredTests;
        events.DiscoveryStart += OnDiscoveryStart;
        events.DiscoveryComplete += OnDiscoveryComplete;
    }

    private void OnTestStart(object? _, TestRunStartEventArgs e)
    {
        WriteData(nameof(OnTestStart), e);
    }

    private void OnTestRunComplete(object? _, TestRunCompleteEventArgs e)
    {
        WriteData(nameof(OnTestRunComplete), e);
        
        _writer!.Dispose();
        _client!.Dispose();
    }

    private void OnTestResult(object? _, TestResultEventArgs e)
    {
        WriteData(nameof(OnTestResult), e);
    }

    private void OnTestRunMessage(object? _, TestRunMessageEventArgs e)
    {
        WriteData(nameof(OnTestRunMessage), e);
    }

    private void OnDiscoveredTests(object? sender, DiscoveredTestsEventArgs e)
    {
        WriteData(nameof(OnDiscoveredTests), e);
    }

    private void OnDiscoveryComplete(object? sender, DiscoveryCompleteEventArgs e)
    {
        WriteData(nameof(OnDiscoveryComplete), e);
    }
    
    private void OnDiscoveryStart(object? sender, DiscoveryStartEventArgs e)
    {
        WriteData(nameof(OnDiscoveryStart), e);
    }

    private void WriteData(string category, object data)
    {
        _writer!.WriteLine($"====== COLLECTOR: {category} ======");

        try 
        {
            var raw = JsonSerializer.Serialize(data, jsonOptions);
            _writer!.WriteLine(raw);
        } 
        catch(Exception ex)
        {
            _writer!.WriteLine(ex.ToString());
        }

        _writer!.WriteLine("=====================================");
    }
}
