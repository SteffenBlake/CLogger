using CLogger.Common.Model;
using CommandLine;

namespace CLogger.Tui.Models;

public class CliOptions 
{
    [Option(
        shortName: 'r',
        longName: "run",
        Default = false, 
        HelpText = "Automatically start a test run on startup",
        Required = false
    )]
    public bool Run { get; init; } = false;

    [Option(
        shortName: 'd',
        longName: "debug",
        Default = false, 
        HelpText = "Runs the test runner in Debug Mode",
        Required = false
    )]
    public bool Debug { get; init; } = false;


    [Option(
        shortName: 'p',
        longName: "port",
        Default = 0, 
        HelpText = "Specifies the TCP port the TestListener and TestAdapters communicate on",
        Required = false
    )]
    public int Port { get; init; } = 0;

    [Option(
        shortName: 'i',
        longName: "ip",
        Default = "127.0.0.1", 
        HelpText = "Specifies the  the IP TestListener and TestAdapters communicate on",
        Required = false
    )]
    public string Domain { get; set; } = "127.0.0.1";

    [Value(
        index: 0,
        MetaName = "Path",
        Default = ".",
        HelpText = "The path to run tests from, defaults to $PWD",
        Required = false
    )]
    public string Path { get; init; } = ".";
}
