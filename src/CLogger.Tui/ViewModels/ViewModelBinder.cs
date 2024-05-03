namespace CLogger.Tui.ViewModels;

public class ViewModelBinder(IEnumerable<IViewModel> viewModels)
{
    public IEnumerable<IViewModel> ViewModels { get; } = viewModels;

    public async Task BindAsync(CancellationToken cancellationToken)
    {
        List<Task> tasks = [];
        foreach(var viewModel in ViewModels)
        {
            tasks.Add(viewModel.BindAsync(cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}
