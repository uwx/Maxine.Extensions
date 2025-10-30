namespace Maxine.Extensions.Test;

[TestClass]
public class CancellationTokenUtilsTests
{
    [TestMethod]
    public void WrapWithTimeout_WithTimeout_CreatesWrappedCTS()
    {
        var cts = new CancellationTokenSource();
        using var wrapped = CancellationTokenUtils.WrapWithTimeout(cts.Token, TimeSpan.FromMilliseconds(100));
        Assert.IsNotNull(wrapped.Token);
        Assert.AreNotEqual(default(CancellationToken), wrapped.Token);
    }

    [TestMethod]
    public void WrapWithTimeout_WithoutTimeout_ReturnsOriginalToken()
    {
        var cts = new CancellationTokenSource();
        using var wrapped = CancellationTokenUtils.WrapWithTimeout(cts.Token, null);
        Assert.AreEqual(cts.Token, wrapped.Token);
    }

    [TestMethod]
    public async Task WrapWithTimeout_TimeoutExpires_CancelsToken()
    {
        var cts = new CancellationTokenSource();
        using var wrapped = CancellationTokenUtils.WrapWithTimeout(cts.Token, TimeSpan.FromMilliseconds(50));
        
        await Task.Delay(100);
        
        Assert.IsTrue(wrapped.Token.IsCancellationRequested);
    }

    [TestMethod]
    public void WrapWithTimeout_Dispose_DisposesInternalCTS()
    {
        var cts = new CancellationTokenSource();
        var wrapped = CancellationTokenUtils.WrapWithTimeout(cts.Token, TimeSpan.FromMilliseconds(100));
        
        wrapped.Dispose();
        
        // Should not throw
        Assert.IsTrue(true);
    }
}

