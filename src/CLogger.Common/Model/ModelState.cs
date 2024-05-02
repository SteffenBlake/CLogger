using CLogger.Common.Enums;

namespace CLogger.Common.Model;

public class ModelState(CancellationToken cancellationToken) 
{
    public CancellationToken CancellationToken { get; } = cancellationToken;

    public TestMetaInfo MetaInfo { get; } = new(cancellationToken);

    public AppConfig AppConfig { get; } = new(cancellationToken);

    private readonly Dictionary<string, TestInfo> _testInfos = [];
    public IReadOnlyDictionary<string, TestInfo> TestInfos => _testInfos;

    public ChannelBroadcaster<string> OnNewTest { get; } 
        = new("", cancellationToken);

    public ChannelBroadcaster<string> OnUpdatedTest { get; } 
        = new("", cancellationToken);
    
    public ChannelBroadcaster<bool> OnClearTests { get; } 
        = new(true, cancellationToken);

    public ChannelBroadcaster<AppState> AppState { get; } 
        = new(Enums.AppState.Idle, cancellationToken);

    public async Task<bool> DiscoveredTestAsync(TestInfo testInfo)
    {
        if (_testInfos.ContainsKey(testInfo.FullyQualifiedName))
        {
            return false;
        }

        _testInfos[testInfo.FullyQualifiedName] = testInfo;

        await OnNewTest.WriteAsync(testInfo.FullyQualifiedName);

        return true;
    }

    public async Task TestResultAsync(TestInfo testInfo)
    {
        // New Test Discovered, don't bother sending Update signal as well
        if (await DiscoveredTestAsync(testInfo))
        {
            return;
        }
        
        _testInfos[testInfo.FullyQualifiedName] = testInfo;

        await OnUpdatedTest.WriteAsync(testInfo.FullyQualifiedName);
    }

    public async Task ClearTestsAsync()
    {
        await OnClearTests.WriteAsync(true);
    }

    public bool TryComplete() 
    {
        return 
            OnNewTest.TryComplete() &&
            OnUpdatedTest.TryComplete() &&
            OnClearTests.TryComplete() &&
            AppState.TryComplete() &&
            
            MetaInfo.TryComplete() &&
            AppConfig.TryComplete();
    }
}
