using CLogger.Tui;
using CLogger.Tui.Models;
using CommandLine;
using Terminal.Gui;

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
    }).WithParsedAsync(Startup.RunAsync);
