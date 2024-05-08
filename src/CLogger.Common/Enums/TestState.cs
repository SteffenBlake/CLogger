namespace CLogger.Common.Enums;

public enum TestState 
{
    None = 0,
    Passed = 1,
    Failed = 2,
    Skipped = 3,
    NotFound = 4,
    Canceled = 5,
    Running = 100, // Extra state we added on top of TestOutcome
    Debugging = 101,
}
