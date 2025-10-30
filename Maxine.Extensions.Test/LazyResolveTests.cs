using Maxine.Extensions.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class LazyResolveTests
{
    private interface ITestService
    {
        string GetMessage();
    }

    private class TestService : ITestService
    {
        public string GetMessage() => "Hello from TestService";
    }

    [TestMethod]
    public void Constructor_WithServiceProvider_CreatesInstance()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazyResolve = new LazyResolve<ITestService>(services);
        
        Assert.IsNotNull(lazyResolve);
    }

    [TestMethod]
    public void Value_FirstAccess_ResolvesService()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazyResolve = new LazyResolve<ITestService>(services);
        
        var service = lazyResolve.Value;
        
        Assert.IsNotNull(service);
        Assert.AreEqual("Hello from TestService", service.GetMessage());
    }

    [TestMethod]
    public void Value_MultipleAccess_ReturnsSameInstance()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazyResolve = new LazyResolve<ITestService>(services);
        
        var service1 = lazyResolve.Value;
        var service2 = lazyResolve.Value;
        
        Assert.AreSame(service1, service2);
    }

    [TestMethod]
    public void ValueOrDefault_BeforeFirstAccess_IsNull()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazyResolve = new LazyResolve<ITestService>(services);
        
        Assert.IsNull(lazyResolve.ValueOrDefault);
    }

    [TestMethod]
    public void ValueOrDefault_AfterFirstAccess_IsNotNull()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazyResolve = new LazyResolve<ITestService>(services);
        
        _ = lazyResolve.Value; // Trigger resolution
        
        Assert.IsNotNull(lazyResolve.ValueOrDefault);
    }

    [TestMethod]
    public void Value_ServiceNotRegistered_ThrowsException()
    {
        var services = new ServiceCollection()
            .BuildServiceProvider();
        
        var lazyResolve = new LazyResolve<ITestService>(services);
        
        try
        {
            _ = lazyResolve.Value;
            Assert.Fail("Expected InvalidOperationException");
        }
        catch (InvalidOperationException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void Value_WithSingleton_ReturnsSameInstanceAcrossLazyResolves()
    {
        var services = new ServiceCollection()
            .AddSingleton<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazy1 = new LazyResolve<ITestService>(services);
        var lazy2 = new LazyResolve<ITestService>(services);
        
        var service1 = lazy1.Value;
        var service2 = lazy2.Value;
        
        Assert.AreSame(service1, service2); // Singleton should return same instance
    }

    [TestMethod]
    public void Value_WithTransient_ReturnsDifferentInstancesAcrossLazyResolves()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();
        
        var lazy1 = new LazyResolve<ITestService>(services);
        var lazy2 = new LazyResolve<ITestService>(services);
        
        var service1 = lazy1.Value;
        var service2 = lazy2.Value;
        
        Assert.AreNotSame(service1, service2); // Transient should return different instances
    }
}
