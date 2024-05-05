using CLogger.Common.Enums;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace CLogger.Tui.Models;

public class TestTreeInfo(string name, string id) : TreeNode
{
    public readonly string Id = id;
    
    private readonly string Name = name;

    public TestState TestState { get; set; } = TestState.None;

    public TestTreeInfo? Parent { get; set; } = null;

    public List<TestTreeInfo> Infos { get; } = [];
    public override IList<ITreeNode> Children => Infos.Cast<ITreeNode>().ToList(); 

    private string _displayTest = "";
    public override string? ToString() => _displayTest;

    public ColorScheme ColorScheme { get; private set; }  = ColorSchemes.Standard;

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
            TestState.Running => ColorSchemes.Interest,
            TestState.Failed => Picked ?
                ColorSchemes.BadPicked : ColorSchemes.Bad,
            _ => Picked ?
                ColorSchemes.WarnPicked : ColorSchemes.Warn
        };
    }

    private IEnumerable<TestTreeInfo> GetDescendants()
    {
        // If this is a leaf, return itself
        if (Infos.Count == 0)
        {
            yield return this;
            yield break;
        }

        foreach(var descendant in Infos.SelectMany(i => i.GetDescendants()))
        {
            yield return descendant;
        }
    }

    public IEnumerable<TestTreeInfo> GetPicked()
    {
        return GetDescendants().Where(d => d.Picked);
    }

    public IEnumerable<string> GetIds()
    {
        return GetDescendants().Select(d => d.Id);
    }

    public void SetPicked(bool picked)
    {
        foreach(var descendant in GetDescendants())
        {
            descendant.Picked = picked;
            descendant.ReloadState();
        }
    }
}
