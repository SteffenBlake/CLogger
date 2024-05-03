using CLogger.Common.Enums;
using CLogger.Common.Model;
using CLogger.Tui.Models;
using CLogger.Tui.Views;

namespace CLogger.Tui.ViewModels;

public class TestExplorerVM(
    TestExplorer view,
    ModelState modelState
) : IViewModel
{
    private TestExplorer View { get; } = view;

    private ModelState ModelState { get; } = modelState;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            WatchDiscoveredTests(cancellationToken),
            WatchUpdatedTests(cancellationToken)
        );
    }

    // Maps a discrete path portion to a specific node
    private readonly Dictionary<string, TestTreeInfo> _targetMappings = [];

    private async Task WatchDiscoveredTests(CancellationToken cancellationToken)
    {
        await foreach (var id in ModelState.OnNewTest.Subscribe(cancellationToken))
        {
            // Ensure the set of TreeNode heirarchy actually exists
            var testInfo = ModelState.TestInfos[id];
            var path = testInfo.DeconstructPath();
            var targetId = "";
            TestTreeInfo? parent = null;
            for (var n = 0; n < path.Count; n++)
            {
                targetId += "/" + path[n];
                if (!_targetMappings.TryGetValue(targetId, out var next))
                {
                    _targetMappings[targetId] = next = new(
                        name: path[n],
                        id: id 
                    );
                    if (parent == null)
                    {
                        View.TreeView.AddObject(next);
                    }
                    else
                    {
                        parent.Infos.Add(next);
                        next.Parent = parent;
                    }
                }
                parent = next;
            }

            ReloadTestHeirarchy(parent!);
        }

        Console.WriteLine("WatchDiscoveredTests Unbound!");
    }

    private async Task WatchUpdatedTests(CancellationToken cancellationToken)
    {
        await foreach(var id in ModelState.OnUpdatedTest.Subscribe(cancellationToken))
        {
            var path = ModelState.TestInfos[id].DeconstructPath();
            var targetId = string.Join('/', path);
            ReloadTestHeirarchy(_targetMappings[targetId]);
        }
        Console.WriteLine("WatchUpdatedTests Unbound!");
    }

    // Reloads a target test (and all of its parent nodes)
    private void ReloadTestHeirarchy(TestTreeInfo node)
    {
        var target = node;
        LoadTestState(target);
        while (target.Parent != null)
        {
            target = target.Parent;
            LoadTestState(target);
        }
        View.TreeView.RefreshObject(target);
    }

    private void LoadTestState(TestTreeInfo node)
    {
        if (node.Infos.Count == 0)
        {
            node.TestState = ModelState.TestInfos[node.Id].State;
        }
        else
        {
            node.TestState = TestState.None;
            foreach(var child in node.Infos)
            {
                if (child.TestState == TestState.Running)
                {
                    node.TestState = TestState.Running;
                    return;
                }
                if (child.TestState == TestState.Failed)
                {
                    node.TestState = TestState.Failed;
                    return;
                }
                if (child.TestState == TestState.Passed)
                {
                    node.TestState = TestState.Passed;
                }
            }
        }
        node.ReloadState();
    }
}
