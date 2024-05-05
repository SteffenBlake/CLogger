using System.Text;
using CLogger.Common.Model;
using Terminal.Gui;

namespace CLogger.Tui.Views;

public class InfoPanel : FrameView 
{
    public FrameView DisplayNameFrame { get; }
    public FrameView DurationFrame { get; }
    public FrameView StartTimeFrame { get; }
    public FrameView EndTimeFrame { get; }
    public ScrollView ErrorScrollView { get; }
    public FrameView ErrorFrame { get; }
    public TextView ErrorText { get; }

    public InfoPanel(
        ActionBar actionBar,
        TestExplorer testExplorer
    )
    {
        Title = "Info";
        X = Pos.Right(testExplorer); 
        Y = Pos.Bottom(actionBar);
        Width = Dim.Fill();
        Height = Dim.Fill(margin:1);

        Add(DisplayNameFrame = new()
        {
            Height = 3,
            Width = Dim.Fill(),
            X = 0,
            Y = 0,
            Visible = false
        }); 
        Add(DurationFrame = new()
        {
            Height = 3,
            Width = Dim.Fill(),
            X = 0,
            Y = Pos.Bottom(DisplayNameFrame),
            Visible = false
        });
        Add(StartTimeFrame = new()
        {
            Height = 3,
            Width = Dim.Fill(),
            X = 0,
            Y = Pos.Bottom(DurationFrame),
            Visible = false
        });
        Add(EndTimeFrame = new()
        {
            Height = 3,
            Width = Dim.Fill(),
            X = 0,
            Y = Pos.Bottom(StartTimeFrame),
            Visible = false
        });
      
        Add(ErrorFrame = new()
        {
            Height = Dim.Fill(),
            Width = Dim.Fill(),
            X = 0,
            Y = Pos.Bottom(EndTimeFrame),
            Visible = false,
            ColorScheme = ColorSchemes.Bad
        });

        ErrorFrame.Add(ErrorScrollView = new()
        {
            Height = Dim.Fill(),
            Width = Dim.Fill(),
            X = 0,
            Y = 0,
            ShowVerticalScrollIndicator = true,
            ShowHorizontalScrollIndicator = false
        });
        ErrorScrollView.Add(ErrorText = new()
        {
            Height = Dim.Fill(),
            Width = Dim.Fill(),
            X = 0,
            Y = 0,
            ColorScheme = ColorSchemes.BadNoFocus,
            WordWrap = true,
            Enabled = true 
        });

        ErrorScrollView.DrawContent += (_) => RedrawError();
        RedrawError();
    }

    private void RedrawError()
    {
        var border = ErrorFrame.Border.GetSumThickness();
        var width = ErrorFrame.Border.ActualWidth - border.Left - border.Right - 3;
        if (width <= 0)
        {
            width = 1;
        }
        var height = ErrorText.Text.Count(c => c == '\n')+5;
        ErrorScrollView.ContentSize = new(width, height);
    }

    internal void LoadTestInfo(TestInfo data)
    {
        if (string.IsNullOrEmpty(data.DisplayName))
        {
            DisplayNameFrame.Visible = false;
        }
        else
        {
            DisplayNameFrame.Visible = true;
            DisplayNameFrame.Text = $"Name: {data.DisplayName}";
        }

        if (!data.Duration.HasValue)
        {
            DurationFrame.Visible = false;
        }
        else
        {
            DurationFrame.Visible = true;
            DurationFrame.Text = $"Duration: {data.Duration.Value}";
        }

        if (!data.StartTime.HasValue)
        {
            StartTimeFrame.Visible = false;
        }
        else
        {
            StartTimeFrame.Visible = true;
            StartTimeFrame.Text = $"Start Time: {data.StartTime.Value}";
        }

        if (!data.EndTime.HasValue)
        {
            EndTimeFrame.Visible = false;
        }
        else
        {
            EndTimeFrame.Visible = true;
            EndTimeFrame.Text = $"End Time: {data.EndTime.Value}";
        }

        if (
            string.IsNullOrEmpty(data.ErrorMessage) && 
            string.IsNullOrEmpty(data.ErrorStackTrace)
        )
        {
            ErrorFrame.Visible = false;
        }
        else
        {
            ErrorFrame.Visible = true;
            var message = new StringBuilder();
            if (!string.IsNullOrEmpty(data.ErrorMessage))
            {
                message.AppendLine(data.ErrorMessage);
            }
            if (!string.IsNullOrEmpty(data.ErrorStackTrace))
            {
                message.AppendLine(data.ErrorStackTrace);
            }
            ErrorText.Text = message.ToString();
            ErrorScrollView.SetNeedsDisplay();
        }
    }
}
