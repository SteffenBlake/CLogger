using CLogger.Tui;
using CLogger.Tui.Models;
using CommandLine;
using Terminal.Gui;

Application.Init();

Colors.Base = ColorSchemes.Standard;
Colors.Menu = ColorSchemes.StandardPicked;
Colors.Dialog = ColorSchemes.StandardPicked;
Colors.TopLevel = ColorSchemes.Standard;
Colors.Error = ColorSchemes.Bad;

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
