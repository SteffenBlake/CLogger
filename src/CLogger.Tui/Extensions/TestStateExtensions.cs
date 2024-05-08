using CLogger.Common.Enums;
using Terminal.Gui;

namespace CLogger.Tui.Extensions;

public static class TestStateExtensions 
{
   public static string ToIcon(this TestState state) =>
       state switch
       {
            TestState.None => "",
            TestState.Passed => "",
            TestState.Running => "",
            TestState.Debugging => "",
            TestState.Failed => "",
            _ => "",
       };

   public static ColorScheme ToColorScheme(
        this TestState state, bool picked
    ) => (state, picked) switch
        {
            (TestState.None, true) => ColorSchemes.StandardPicked,
            (TestState.None, false) => ColorSchemes.Standard,
            (TestState.Passed, true) => ColorSchemes.GoodPicked,
            (TestState.Passed, false) => ColorSchemes.Good,
            (TestState.Failed, true) => ColorSchemes.BadPicked,
            (TestState.Failed, false) => ColorSchemes.Bad,
            (TestState.Running, _) => ColorSchemes.Interest,
            (_, true) => ColorSchemes.WarnPicked,
            (_, false) => ColorSchemes.Warn
        };
}
