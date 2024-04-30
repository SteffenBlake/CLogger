using System.Text.Json;
using System.Text.Json.Serialization;
using CLogger.Common.Model;

namespace CLogger.Tui;

public class Application(ModelState modelState) 
{
    private ModelState ModelState { get; } = modelState;

    public async Task RunAsync()
    {
        await Task.WhenAll(
            WatchTestProcessId(), 
            WatchElapsed(),
            WatchAppState(), 
            WatchNewTests(), 
            WatchUpdatedTests()
        );
    }
    
    
    private async Task WatchTestProcessId()
    {
        var events = ModelState.MetaInfo.OnTestProcessId.Subscribe().ReadAllAsync();
        await foreach(var _ in events)
        {
            Console.WriteLine($"PROCESS ID: {ModelState.MetaInfo.TestProcessId}");
        }
    }
    
    private async Task WatchElapsed()
    {
        var events = ModelState.MetaInfo.OnElapsed.Subscribe().ReadAllAsync();
        await foreach(var _ in events)
        {
            Console.WriteLine($"TOTAL ELAPSED: {ModelState.MetaInfo.Elapsed}");
        }
    }
    
    private async Task WatchAppState()
    {
        var events = ModelState.OnAppState.Subscribe().ReadAllAsync();
        await foreach(var _ in events)
        {
            Console.WriteLine($"APP STATE: {ModelState.State}");
        }
    }
    
    private async Task WatchNewTests()
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        var events = ModelState.OnNewTest.Subscribe().ReadAllAsync();
        await foreach(var testId in events)
        {
            var testInfo = ModelState.TestInfos[testId];
            var raw = JsonSerializer.Serialize(testInfo, options);
            Console.WriteLine("DISCOVERED TEST: " + raw);
        }
    }
    
    
    private async Task WatchUpdatedTests()
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        options.Converters.Add(new JsonStringEnumConverter());
        
        var events = ModelState.OnUpdatedTest.Subscribe().ReadAllAsync();
        await foreach(var testId in events)
        {
            var testInfo = ModelState.TestInfos[testId];
            var raw = JsonSerializer.Serialize(testInfo, options);
            Console.WriteLine("UPDATED TEST: " + raw);
        }
    }
}
