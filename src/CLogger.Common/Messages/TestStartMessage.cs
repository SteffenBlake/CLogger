namespace CLogger.Common.Messages;

public class TestStartMessage : MessageBase 
{
    public string Started { get; init; } = nameof(Started);

    public override Task<string> Invoke(Model modelState)
    {
        modelState.Running = true;

        return Task.FromResult("Test Run Started");
    }
}
