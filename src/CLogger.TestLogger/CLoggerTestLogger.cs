﻿using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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
        WriteData(new TestStartMessage());
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

    private void OnTestRunMessage(object? _, TestRunMessageEventArgs e)
    {
        /* WriteData(nameof(OnTestRunMessage), e); */
    }

    private void OnDiscoveredTests(object? sender, DiscoveredTestsEventArgs e)
    {
        /* WriteData(nameof(OnDiscoveredTests), e); */
    }

    private void OnDiscoveryComplete(object? sender, DiscoveryCompleteEventArgs e)
    {
        /* WriteData(nameof(OnDiscoveryComplete), e); */
    }
    
    private void OnDiscoveryStart(object? sender, DiscoveryStartEventArgs e)
    {
        /* WriteData(nameof(OnDiscoveryStart), e); */
    }

    private void WriteData<T>(T data)
        where T : MessageBase
    {
       _writer!.WriteLine(JsonSerializer.Serialize<MessageBase>(data)); 
    }
}
