using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.Views;
using Terminal.Gui;

namespace CLogger.Tui.ViewModels;

public class ActionBarVM(
    ModelState modelState,
    ActionBar actionBar,
    TestExplorer testExplorer
) : IViewModel
{
    private ModelState ModelState { get; } = modelState;
    private ActionBar ActionBar { get; } = actionBar;
    public TestExplorer TestExplorer { get; } = testExplorer;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        ActionBar.ReloadBtn.Clicked += () => OnReload(cancellationToken);
        ActionBar.RunBtn.Clicked += () => OnRun(cancellationToken);
        ActionBar.DebugBtn.Clicked += () => OnDebug(cancellationToken);
        ActionBar.CancelBtn.Clicked += () => OnCancel(cancellationToken);

        await WatchForBusy(cancellationToken);
    }

    private void OnReload(CancellationToken cancellationToken)
    {
        if (!ActionBar.ReloadBtn.Enabled)
        {
            return;
        }

        Application.MainLoop.Invoke(() => {
            ModelState.ClearTestsAsync(cancellationToken).Wait(cancellationToken);
            var discover = new RunTestsArgs(
                Discover: true,
                Debug: false,
                TestIds: []
            );
            ModelState.RunTestsAsync(discover, cancellationToken).Wait(cancellationToken);
        });
    }

    private void OnRun(CancellationToken cancellationToken)
    {
        if (!ActionBar.RunBtn.Enabled)
        {
            return;
        }
        OnStart(debug:false, cancellationToken);
    }

    private void OnDebug(CancellationToken cancellationToken)
    {
        if (!ActionBar.RunBtn.Enabled)
        {
            return;
        }
        OnStart(debug:true, cancellationToken);
    }

    private void OnStart(bool debug, CancellationToken cancellationToken)
    {
        if (!ActionBar.ReloadBtn.Enabled)
        {
            return;
        }

        var testIds = TestExplorer.TreeView.Objects
            .Cast<TestTreeInfo>()
            .SelectMany(i => i.GetPicked())
            .Select(i => i.Id)
            .ToList();

        var run = new RunTestsArgs(
            Discover: false,
            Debug: debug,
            TestIds: testIds
        );

        ModelState.RunTestsAsync(run, cancellationToken).Wait(cancellationToken);
    }

    private void OnCancel(CancellationToken cancellationToken)
    {
        if (!ActionBar.CancelBtn.Enabled)
        {
            return;
        }

        ModelState.MetaInfo.State
            .WriteAsync(AppState.Cancelling, cancellationToken)
            .Wait(cancellationToken);
    }

    private async Task WatchForBusy(CancellationToken cancellationToken)
    {
        var events = ModelState.MetaInfo.State.Subscribe(cancellationToken);
        await foreach(var state in events)
        {
            ActionBar.ReloadBtn.Enabled = state == AppState.Idle;
            ActionBar.RunBtn.Enabled = state == AppState.Idle;
            ActionBar.DebugBtn.Enabled = state == AppState.Idle;
            ActionBar.CancelBtn.Enabled = 
                state == AppState.Running || state == AppState.Debugging;
            ActionBar.CancelBtn.Visible = state != AppState.Idle;
        }
    }
}
