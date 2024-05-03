namespace CLogger.Tui.ViewModels;

public interface IViewModel 
{
    Task BindAsync(CancellationToken cancellationToken);
}
