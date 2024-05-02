using CLogger.Common.Enums;
using CLogger.Common.Model;
using NStack;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace CLogger.Tui.Views;

public class TestExplorer : FrameView
{
    private ModelState ModelState { get; }

    private TreeView TreeView { get; }

    public TestExplorer(
        ModelState modelState,
        View actionBar
    )
    {
        ModelState = modelState;
        
        Title = "Tests";
        X = 0;
        Y = Pos.Bottom(actionBar);
        Width = Dim.Percent(90f);
        Height = Dim.Fill(margin: 1);

        Add(TreeView = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            MultiSelect = true,
            ColorGetter = f => ((TreeInfo)f).ColorScheme
        });

        SetupScrollbar();
    }

    private void SetupScrollbar()
    {
        TreeView.Style.LeaveLastRow = true;

        var _scrollBar = new ScrollBarView(TreeView, true);

        _scrollBar.ChangedPosition += () =>
        {
            TreeView.ScrollOffsetVertical = _scrollBar.Position;
            if (TreeView.ScrollOffsetVertical != _scrollBar.Position)
            {
                _scrollBar.Position = TreeView.ScrollOffsetVertical;
            }
            TreeView.SetNeedsDisplay();
        };

        _scrollBar.OtherScrollBarView.ChangedPosition += () =>
        {
            TreeView.ScrollOffsetHorizontal = _scrollBar.OtherScrollBarView.Position;
            if (TreeView.ScrollOffsetHorizontal != _scrollBar.OtherScrollBarView.Position)
            {
                _scrollBar.OtherScrollBarView.Position = TreeView.ScrollOffsetHorizontal;
            }
            TreeView.SetNeedsDisplay();
        };

        TreeView.DrawContent += (e) =>
        {
            _scrollBar.Size = TreeView.ContentHeight;
            _scrollBar.Position = TreeView.ScrollOffsetVertical;
            _scrollBar.OtherScrollBarView.Size = TreeView.GetContentWidth(true);
            _scrollBar.OtherScrollBarView.Position = TreeView.ScrollOffsetHorizontal;
            _scrollBar.Refresh();
        };
    }

    // Maps a discrete path portion to a specific node
    private readonly Dictionary<string, TreeInfo> _targetMappings = [];

    public async Task BindAsync()
    {
        await Task.WhenAll(
            WatchDiscoveredTests(),
            WatchUpdatedTests()
        );
        Console.WriteLine("Test Explorer Unbound!");
    }

    private async Task WatchDiscoveredTests()
    {
        await foreach (var id in ModelState.OnNewTest.Subscribe())
        {
            // Ensure the set of TreeNode heirarchy actually exists
            var testInfo = ModelState.TestInfos[id];
            var path = testInfo.DeconstructPath();
            var targetId = "";
            TreeInfo? parent = null;
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
                        TreeView.AddObject(next);
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

    private async Task WatchUpdatedTests()
    {
        await foreach(var id in ModelState.OnUpdatedTest.Subscribe())
        {
            var path = ModelState.TestInfos[id].DeconstructPath();
            var targetId = string.Join('/', path);
            ReloadTestHeirarchy(_targetMappings[targetId]);
        }
        Console.WriteLine("WatchUpdatedTests Unbound!");
    }

    // Reloads a target test (and all of its parent nodes)
    private void ReloadTestHeirarchy(TreeInfo node)
    {
        var target = node;
        LoadTestState(target);
        while (target.Parent != null)
        {
            target = target.Parent;
            LoadTestState(target);
        }
        TreeView.RefreshObject(target);
    }

    private void LoadTestState(TreeInfo node)
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

    private class TreeInfo(string name, string id) : TreeNode
    {
        public readonly string Id = id;

        private readonly string Name = name;

        public TestState TestState { get; set; } = TestState.None;

        public TreeInfo? Parent { get; set; } = null;

        public List<TreeInfo> Infos { get; } = [];
        public override IList<ITreeNode> Children => Infos.Cast<ITreeNode>().ToList(); 

        private string _displayTest = "";
        public override string? ToString() => _displayTest;

        public ColorScheme ColorScheme { get; private set; } 
            = ColorSchemes.Standard;

        public bool Picked { get; set; } = false;

        public void ReloadState()
        {
            var prefix = TestState switch
            {
                TestState.None => "",
                TestState.Passed => "",
                TestState.Running => "",
                TestState.Failed => "",
                _ => "",
            };

            _displayTest = " " + (Picked ? ">" : "") + prefix + " " + Name;

            ColorScheme = TestState switch
            {
                TestState.None => Picked ? 
                    ColorSchemes.StandardPicked : ColorSchemes.Standard,
                TestState.Passed => Picked ?
                    ColorSchemes.GoodPicked : ColorSchemes.Good,
                TestState.Running => ColorSchemes.Standard,
                TestState.Failed => Picked ?
                    ColorSchemes.BadPicked : ColorSchemes.Bad,
                _ => Picked ?
                    ColorSchemes.WarnPicked : ColorSchemes.Warn
            };
        }
    }
}
