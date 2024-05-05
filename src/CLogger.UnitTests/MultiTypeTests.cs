namespace CLogger.UnitTests;

public class MultiTypeTests : MultiTypeTestBase<IEnumerable<List<(int, string)>>, int, string> 
{
    [Test]
    public void MultiTypedParent()
    {
        Assert.Pass();
    }
}
