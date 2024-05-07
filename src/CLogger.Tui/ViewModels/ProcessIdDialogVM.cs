using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Views;
using Microsoft.Extensions.Logging;

namespace CLogger.Tui.ViewModels;

public class ProcessIdDialogVM(
    ILogger<ProcessIdDialogVM> logger,
    ProcessIdDialog processIdDialog,
    ModelState modelState
) : IViewModel
{
    public ILogger<ProcessIdDialogVM> Logger { get; } = logger;
    public ProcessIdDialog ProcessIdDialog { get; } = processIdDialog;
    public ModelState ModelState { get; } = modelState;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            WatchProcessIdAsync(cancellationToken),
            WatchNewTestsAsync(cancellationToken),
            WatchUpdatedTestsAsync(cancellationToken)
        );
    }

    private async Task WatchProcessIdAsync(CancellationToken cancellationToken)
    {
        var events = ModelState.MetaInfo.TestProcessId.Subscribe(cancellationToken);
        Logger.LogInformation("Listening for Process Id Events...");
        await foreach(var processId in events)
        {
            Logger.LogInformation("Process Id Recieved");
            ProcessIdDialog.ProcessIdText.Text = processId;
            ProcessIdDialog.Visible = true;
        }
    }

    private async Task WatchNewTestsAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Listening for new tests...");
        var events = ModelState.OnNewTest.Subscribe(cancellationToken);
        await foreach(var e in events)
        {
            Logger.LogInformation("New test recieved");
            if (!ProcessIdDialog.Visible)
            {
                continue;
            }
            if (ModelState.TestInfos[e].State == TestState.Running)
            {
                continue;
            }

            ProcessIdDialog.Visible = false;
        }
    }

    private async Task WatchUpdatedTestsAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Listening for updated tests...");
        var events = ModelState.OnUpdatedTest.Subscribe(cancellationToken);
        await foreach(var e in events)
        {
            Logger.LogInformation("Updated recieved");
            if (!ProcessIdDialog.Visible)
            {
                continue;
            }
            if (ModelState.TestInfos[e].State == TestState.Running)
            {
                continue;
            }

            ProcessIdDialog.Visible = false;
        }
    }
}
