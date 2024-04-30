﻿using CLogger.Tui;
using CLogger.Tui.Models;
using CommandLine;

try 
{
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
        }).WithParsedAsync(Run);
}
// Don't log Operation Cancel Exceptions as its intended to happen
catch (OperationCanceledException) {}

static async Task Run(CliOptions options)
{
    /* Console.WriteLine( */
    /*     $"Waiting for debugger to attach, ProcessId: {Environment.ProcessId}" */
    /* ); */
    /* while (!Debugger.IsAttached) */
    /* { */
    /*     await Task.Delay(1000); */
    /* } */

    await Startup.RunAsync(options);
}

