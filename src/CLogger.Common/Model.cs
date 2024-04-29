namespace CLogger.Common;

public class Model 
{
    public bool Running { get; set; } = true;

    public string PipeName { get; } = Guid.NewGuid().ToString();
}
