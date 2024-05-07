using CLogger.Common.Enums;
using CLogger.Tui.Extensions;
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
        var prefix = TestState.ToIcon();
        var pickedCarrot = Picked ? ">" : ""; 
        _displayTest = " " + pickedCarrot + prefix + " " + Name;

        ColorScheme = TestState.ToColorScheme(Picked);
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
