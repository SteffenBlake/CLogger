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

    [Value(
        index: 0,
        MetaName = "Path",
        Default = ".",
        HelpText = "The path to run tests from, defaults to $PWD",
        Required = false
    )]
    public string Path { get; init; } = ".";

    public async Task ApplyAsync(
        AppConfig config, CancellationToken cancellationToken
    )
    {
        await config.Path.WriteAsync(Path, cancellationToken);
    }
}
