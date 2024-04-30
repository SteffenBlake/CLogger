using System.Threading.Channels;
using CLogger.Common.Enums;

namespace CLogger.Common.Model;

public class ModelState 
{
    public ModelState()
    {
        MetaInfo = new(CancellationToken);

        OnNewTest = new(_newTestEvents.Reader);
        OnUpdatedTest = new(_updatedTestEvents.Reader);
        OnClearTests = new(_clearTestEvents.Reader);
        OnAppState = new(_appStateEvents.Reader);
    }

    public async Task PublishAsync()
    {
        await Task.WhenAll(
            OnNewTest.PublishAsync(CancellationToken),
            OnUpdatedTest.PublishAsync(CancellationToken),
            OnClearTests.PublishAsync(CancellationToken),
            OnAppState.PublishAsync(CancellationToken),
            MetaInfo.PublishAsync()
        );
    }

    private readonly CancellationTokenSource cancellationTokenSource = new();
    public CancellationToken CancellationToken => cancellationTokenSource.Token;

    public async Task CloseAsync() 
    {
        await cancellationTokenSource.CancelAsync();

        _newTestEvents.Writer.Complete();
        _appStateEvents.Writer.Complete();
        _clearTestEvents.Writer.Complete();
        _updatedTestEvents.Writer.Complete();

        await MetaInfo.CloseAsync();
    }

    public TestMetaInfo MetaInfo { get; }

    private readonly Dictionary<string, TestInfo> _testInfos = [];
    public IReadOnlyDictionary<string, TestInfo> TestInfos => _testInfos;

    private readonly Channel<string> _newTestEvents = Channel.CreateUnbounded<string>();
    public ChannelBroadcaster<string> OnNewTest { get; }

    private readonly Channel<string> _updatedTestEvents = Channel.CreateUnbounded<string>();
    public ChannelBroadcaster<string> OnUpdatedTest { get; }
    
    private readonly Channel<bool> _clearTestEvents = Channel.CreateUnbounded<bool>();
    public ChannelBroadcaster<bool> OnClearTests { get; }

    private readonly Channel<bool> _appStateEvents = Channel.CreateUnbounded<bool>();
    public ChannelBroadcaster<bool> OnAppState { get; }

    public async Task<bool> DiscoveredTestAsync(TestInfo testInfo)
    {
        if (_testInfos.ContainsKey(testInfo.FullyQualifiedName))
        {
            return false;
        }

        _testInfos[testInfo.FullyQualifiedName] = testInfo;

        await _newTestEvents.Writer.WriteAsync(
            testInfo.FullyQualifiedName, CancellationToken
        );

        return true;
    }

    public async Task TestResultAsync(TestInfo testInfo)
    {
        // New Test Discovered, dont bother sending Update signal as well
        if (await DiscoveredTestAsync(testInfo))
        {
            return;
        }
        
        _testInfos[testInfo.FullyQualifiedName] = testInfo;

        await _updatedTestEvents.Writer.WriteAsync(
            testInfo.FullyQualifiedName, CancellationToken
        );
    }

    public async Task ClearTestsAsync()
    {
        await _clearTestEvents.Writer.WriteAsync(true, CancellationToken);
    }

    public AppState State { get; private set; } = AppState.Idle;
    public async Task UpdateStateAsync(AppState state)
    {
        if (State == state)
        {
            return;
        }

        State = state;
        await _appStateEvents.Writer.WriteAsync(true, CancellationToken);
    }
}
