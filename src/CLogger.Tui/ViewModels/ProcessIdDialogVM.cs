using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Views;
using Terminal.Gui;

namespace CLogger.Tui.ViewModels;

public class ProcessIdDialogVM(
    ProcessIdDialog processIdDialog,
    ModelState modelState
) : IViewModel
{
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
        await foreach(var processId in events)
        {
            ProcessIdDialog.ProcessIdText.Text = processId.ToString();
            ProcessIdDialog.Visible = true;
        }
    }

    private async Task WatchNewTestsAsync(CancellationToken cancellationToken)
    {
        var events = ModelState.OnNewTest.Subscribe(cancellationToken);
        await foreach(var e in events)
        {
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
        var events = ModelState.OnUpdatedTest.Subscribe(cancellationToken);
        await foreach(var e in events)
        {
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
