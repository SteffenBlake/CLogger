using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.Views;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace CLogger.Tui.ViewModels;

public class InfoPanelVM(
    InfoPanel infoPanel,
    TestExplorer testExplorer,
    ModelState modelState
) : IViewModel
{
    public InfoPanel InfoPanel { get; } = infoPanel;
    public TestExplorer TestExplorer { get; } = testExplorer;
    public ModelState ModelState { get; } = modelState;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        TestExplorer.TreeView.SelectionChanged += (_, _) => OnExplorerSelectionChanged();

        await WatchTestUpdates(cancellationToken);        
    }

    private async Task WatchTestUpdates(CancellationToken cancellationToken)
    {
        await foreach(var _ in ModelState.OnUpdatedTest.Subscribe(cancellationToken))
        {
            Application.MainLoop.Invoke(OnExplorerSelectionChanged);
        }
    }

    private void OnExplorerSelectionChanged()
    {
        if (TestExplorer.TreeView.SelectedObject is not TestTreeInfo target)
        {
            return;
        }
        var ids = target.GetIds().ToList();

        if (ids.Count == 1)
        {
            LoadTestInfo(ids[0]);
        }
        else
        {
            LoadTestAggregateInfo(ids);
        }
    }

    private void LoadTestInfo(string id)
    {
        var data = ModelState.TestInfos[id];
        InfoPanel.LoadTestInfo(data);
    }

    private void LoadTestAggregateInfo(List<string> ids)
    {
        var infos = ids.Select(id => ModelState.TestInfos[id]).ToList();

        var data = new TestInfo()
        {
            FullyQualifiedName = "",
            DisplayName = string.Join(
                " + ",
                infos
                    .Where(i => i.DisplayName != null)
                    .Select(i => i.DisplayName)
            ),
            Duration = infos
                .Select(i => i.Duration)
                .Aggregate((TimeSpan?)null, (a,b) => a+b),
            StartTime = null,
            EndTime = null,
            ErrorStackTrace = null,
            ErrorMessage = null,
            State = TestState.None
        };

        InfoPanel.LoadTestInfo(data);
    }
}
