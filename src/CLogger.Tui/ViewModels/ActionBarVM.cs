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
        ActionBar.RunBtn.Clicked += () => OnRun(debug:false, cancellationToken);
        ActionBar.DebugBtn.Clicked += () => OnRun(debug:true, cancellationToken);
        ActionBar.CancelBtn.Clicked += () => OnCancel(cancellationToken);

        await WatchForBusy(cancellationToken);
    }

    private void OnReload(CancellationToken cancellationToken)
    {
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

    private void OnRun(bool debug, CancellationToken cancellationToken)
    {
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
            ActionBar.CancelBtn.Enabled = state == AppState.Busy;
            ActionBar.CancelBtn.Visible = state != AppState.Idle;
        }
    }
}
