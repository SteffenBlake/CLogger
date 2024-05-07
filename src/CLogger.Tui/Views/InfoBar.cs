using Terminal.Gui;

namespace CLogger.Tui.Views;

public class InfoBar : FrameView 
{
    public Label QuitText { get; }
    public Label PickSpacer { get; }
    public Label PickText { get; }
    public Label UnpickSpacer { get; }
    public Label UnpickText { get; }
    public Label ReloadSpacer { get; }
    public Label ReloadText { get; }
    public Label RunSpacer { get; }
    public Label RunText { get; }
    public Label DebugSpacer { get; }
    public Label DebugText { get; }
    public Label CancelSpacer { get; }
    public Label CancelText { get; }

    public InfoBar(TestExplorer testExplorer)
    {
        X = 0;
        Y = Pos.Bottom(testExplorer);
        Width = Dim.Fill();
        Height = 1;
        Border.BorderStyle = BorderStyle.None;
        Border.DrawMarginFrame = false;

        QuitText = AddLabel(null, "[Q]uit", ColorSchemes.Bad);
        PickSpacer = AddSpacer(QuitText);
        PickText = AddLabel(PickSpacer, "[P]ick", ColorSchemes.Good);
        UnpickSpacer = AddSpacer(PickText);
        UnpickText = AddLabel(UnpickSpacer, "[U]npick", ColorSchemes.Bad);

        ReloadSpacer = AddSpacer(UnpickText);
        ReloadText = AddLabel(ReloadSpacer, "Re[L]oad", ColorSchemes.Interest);
        RunSpacer = AddSpacer(ReloadText);
        RunText = AddLabel(RunSpacer, "[R]un", ColorSchemes.Good);
        DebugSpacer = AddSpacer(RunText);
        DebugText = AddLabel(DebugSpacer, "[D]ebug", ColorSchemes.Warn);

        CancelSpacer = AddSpacer(UnpickText);
        CancelText = AddLabel(CancelSpacer, "[C]ancel", ColorSchemes.Bad);

        OnState(busy:true);
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

    public void OnState(bool busy)
    {
         ReloadSpacer.Visible = !busy;
         ReloadText.Visible = !busy;
         RunSpacer.Visible = !busy;
         RunText.Visible = !busy;
         DebugSpacer.Visible = !busy;
         DebugText.Visible = !busy;

         CancelSpacer.Visible = busy;
         CancelText.Visible = busy;
    }
}
