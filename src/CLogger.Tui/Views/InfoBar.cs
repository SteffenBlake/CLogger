using CLogger.Common.Enums;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class InfoBar : FrameView 
{
    public Label PickText { get; }
    public Label UnpickSpacer { get; }
    public Label UnpickText { get; }
    public Label ScrollSpacer { get; }
    public Label ScrollText { get; }
    public Label ReloadSpacer { get; }
    public Label ReloadText { get; }
    public Label RunSpacer { get; }
    public Label RunText { get; }
    public Label DebugSpacer { get; }
    public Label DebugText { get; }
    public Label CancelSpacer { get; }
    public Label CancelText { get; }

    public Label QuitText { get; }

    public Label StatusText { get; }

    public InfoBar(TestExplorer testExplorer)
    {
        X = 0;
        Y = Pos.Bottom(testExplorer);
        Width = Dim.Fill();
        Height = 1;
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        PickText = AddLabel(null, "[P]ick", ColorSchemes.Good);
        ScrollSpacer = AddSpacer(PickText);
        ScrollText = AddLabel(ScrollSpacer, "<H/J/K/L>: Scroll", ColorSchemes.Standard);
        UnpickSpacer = AddSpacer(ScrollText);
        UnpickText = AddLabel(UnpickSpacer, "[U]npick", ColorSchemes.Bad);

        ReloadSpacer = AddSpacer(UnpickText);
        ReloadText = AddLabel(ReloadSpacer, "Rel[O]ad", ColorSchemes.Interest);
        RunSpacer = AddSpacer(ReloadText);
        RunText = AddLabel(RunSpacer, "[R]un", ColorSchemes.Good);
        DebugSpacer = AddSpacer(RunText);
        DebugText = AddLabel(DebugSpacer, "[D]ebug", ColorSchemes.Warn);

        CancelSpacer = AddSpacer(UnpickText);
        CancelText = AddLabel(CancelSpacer, "[C]ancel", ColorSchemes.Bad);

        Add(QuitText = new()
        {
            Text = "| C-Q: Quit",
            X = Pos.AnchorEnd(12),
            Y = 0,
            Width = 12,
            Height = 1,
            ColorScheme = ColorSchemes.Bad,
        });

        Add(StatusText = new()
        {
            X = Pos.Left(QuitText),
            Y = 0,
            Width = 1,
            Height = 1,
            ColorScheme = ColorSchemes.Interest
        });

        OnState(AppState.Running);
    }

    private Label AddLabel(
        Label? prior, string text, ColorScheme colorScheme
    )
    {
        Label label;
        Add(label = new Label()
        {
            Text = text,
            X = prior == null ? 0 : Pos.Right(prior),
            Y = 0,
            AutoSize = true,
            ColorScheme = colorScheme
        });
        return label;
    }

    private Label AddSpacer(Label prior)
    {
        Label spacer;
        Add(spacer = new()
        {
            Text = " | ",
            X = Pos.Right(prior),
            Y = 0,
            AutoSize = true,
            ColorScheme = ColorSchemes.Standard
        });
        return spacer;
    }

    public void OnState(AppState appState)
    {
        var status = "Status: " + appState.ToString();
        var width = status.Length+1;

        var colorscheme = appState switch
        {
            AppState.Running => ColorSchemes.Interest,
            AppState.Idle => ColorSchemes.Standard,
            AppState.Finishing => ColorSchemes.Good,
            AppState.Cancelling => ColorSchemes.Bad,
            _ => ColorSchemes.Warn
        };

        StatusText.Width = width;
        StatusText.X = Pos.Left(QuitText)-width;
        StatusText.Text = status;
        StatusText.ColorScheme = colorscheme;

        ReloadSpacer.Visible = appState == AppState.Idle;
        ReloadText.Visible = appState == AppState.Idle;
        RunSpacer.Visible = appState == AppState.Idle;
        RunText.Visible = appState == AppState.Idle;
        DebugSpacer.Visible = appState == AppState.Idle;
        DebugText.Visible = appState == AppState.Idle;

        CancelSpacer.Visible = appState == AppState.Running;
        CancelText.Visible = appState == AppState.Running;
    }
}
