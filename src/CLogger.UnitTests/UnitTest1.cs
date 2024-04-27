namespace CLogger.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Test2(int a)
    {
        Assert.Pass();
    }

    [Test]
    public void FailTest()
    {
        Assert.Fail();
    }
}
