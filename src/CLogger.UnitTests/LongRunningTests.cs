namespace CLogger.UnitTests;

public class LongRunningTests 
{
    
    [Test]
    [TestCase(4000)]
    [TestCase(6000)]
    [TestCase(8000)]
    [Parallelizable]
    public async Task LongRunningTest(int delay)
    {
        await Task.Delay(delay);
        Assert.Pass();
    }
}
