using CLogger.Common.Channels;

namespace CLogger.Common.Model;

public class ModelState(
    TestMetaInfo testMetaInfo,
    AppConfig config,
    ChannelBroadcaster<string> onNewTest,
    ChannelBroadcaster<string> onUpdatedTest,
    ChannelBroadcaster<bool> onClearTests,
    ChannelBroadcaster<RunTestsArgs> onRunTests
) 
{
    public TestMetaInfo MetaInfo { get; } = testMetaInfo;

    public AppConfig Config { get; } = config;

    private readonly Dictionary<string, TestInfo> _testInfos = [];
    public IReadOnlyDictionary<string, TestInfo> TestInfos => _testInfos;

    public ChannelBroadcaster<string> OnNewTest { get; } = onNewTest;

    public ChannelBroadcaster<string> OnUpdatedTest { get; } = onUpdatedTest;
    
    public ChannelBroadcaster<bool> OnClearTests { get; } = onClearTests; 

    public ChannelBroadcaster<RunTestsArgs> OnRunTests { get; } = onRunTests; 
    
    public async Task<bool> DiscoveredTestAsync(
        TestInfo testInfo, CancellationToken cancellationToken
    )
    {
        if (_testInfos.ContainsKey(testInfo.FullyQualifiedName))
        {
            return false;
        }

        _testInfos[testInfo.FullyQualifiedName] = testInfo;

        await OnNewTest.WriteAsync(testInfo.FullyQualifiedName, cancellationToken);

        return true;
    }

    public async Task TestResultAsync(
        TestInfo testInfo, CancellationToken cancellationToken
    )
    {
        // New Test Discovered, don't bother sending Update signal as well
        if (await DiscoveredTestAsync(testInfo, cancellationToken))
        {
            return;
        }
        
        _testInfos[testInfo.FullyQualifiedName] = testInfo;

        await OnUpdatedTest.WriteAsync(
            testInfo.FullyQualifiedName, cancellationToken
        );
    }

    public async Task ClearTestsAsync(CancellationToken cancellationToken)
    {
        await OnClearTests.WriteAsync(true, cancellationToken);
    }
}
