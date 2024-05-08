using CLogger.Tui.Models;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class TestExplorer : FrameView
{
    public TreeView TreeView { get; }

    public TestExplorer(
        ActionBar actionBar
    )
    {
        Title = "Tests";
        X = 0;
        Y = Pos.Bottom(actionBar);
        Width = Dim.Percent(20f);
        Height = Dim.Fill(margin: 1);

        Add(TreeView = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            MultiSelect = false,
            ColorGetter = f => ((TestTreeInfo)f).ColorScheme
        });

        TreeView.Style.CollapseableSymbol = new Rune('');
        TreeView.Style.ExpandableSymbol = new Rune('');

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

        TreeView.KeyPress += (e) =>
        {
            if (e.KeyEvent.Key == Keybinds.ScrollLeft.Key)
            {
                _scrollBar.OtherScrollBarView.Position--;
            }
            if (e.KeyEvent.Key == Keybinds.ScrollDown.Key)
            {
                _scrollBar.Position++;
            }
            if (e.KeyEvent.Key == Keybinds.ScrollUp.Key)
            {
                _scrollBar.Position--;
            }
            if (e.KeyEvent.Key == Keybinds.ScrollRight.Key)
            {
                _scrollBar.OtherScrollBarView.Position++;
            }
        };

        _scrollBar.AddKeyBinding(Key.h, Command.ScrollLeft);
        _scrollBar.AddKeyBinding(Key.j, Command.ScrollDown);
        _scrollBar.AddKeyBinding(Key.k, Command.ScrollUp);
        _scrollBar.AddKeyBinding(Key.l, Command.ScrollRight);
    }

    public bool OnPick() => OnSetPicked(true);

    public bool OnUnpick() => OnSetPicked(false);

    public bool OnSetPicked(bool picked)
    {
        if (TreeView.SelectedObject is not TestTreeInfo selected)
        {
            return false;
        }

        selected.SetPicked(picked);
        TreeView.SetNeedsDisplay();
        
        return true;
    }
}
