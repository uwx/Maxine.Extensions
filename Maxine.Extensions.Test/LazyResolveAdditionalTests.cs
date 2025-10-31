using Maxine.Extensions.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class LazyResolveAdditionalTests
{
    private interface ITestService
    {
        string GetMessage();
    }

    private class TestService : ITestService
    {
        public string GetMessage() => "Test Message";
    }

    [TestMethod]
    public void Value_CalledMultipleTimes_OnlyResolvesOnce()
    {
        // Tracks how many times the service was created
        var creationCount = 0;
        var services = new ServiceCollection()
            .AddTransient<ITestService>(sp => 
            {
                creationCount++;
                return new TestService();
            })
            .BuildServiceProvider();

        var lazyResolve = new LazyResolve<ITestService>(services);

        // Access Value multiple times
        var v1 = lazyResolve.Value;
        var v2 = lazyResolve.Value;
        var v3 = lazyResolve.Value;

        // Should only create the service once
        Assert.AreEqual(1, creationCount);
        Assert.AreSame(v1, v2);
        Assert.AreSame(v2, v3);
    }

    [TestMethod]
    public void Get_ClearsServiceProviderReference()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();

        var lazyResolve = new LazyResolve<ITestService>(services);

        // Before accessing Value, we can't directly check _services, but we know it should be set
        Assert.IsNull(lazyResolve.ValueOrDefault);

        // Access Value to trigger Get()
        _ = lazyResolve.Value;

        // After Get(), ValueOrDefault should be set
        Assert.IsNotNull(lazyResolve.ValueOrDefault);
        
        // The service provider reference should be cleared (we can't test this directly
        // but we can verify that subsequent Value calls still work)
        var service = lazyResolve.Value;
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void ValueOrDefault_DirectlySet_IsReturned()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();

        var lazyResolve = new LazyResolve<ITestService>(services);
        var customService = new TestService();

        // Directly set ValueOrDefault
        lazyResolve.ValueOrDefault = customService;

        // Value should return the directly-set value
        Assert.AreSame(customService, lazyResolve.Value);
    }

    [TestMethod]
    public void Value_WithScoped_WorksCorrectly()
    {
        var services = new ServiceCollection()
            .AddScoped<ITestService, TestService>()
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        var lazyResolve = new LazyResolve<ITestService>(scope.ServiceProvider);

        var service = lazyResolve.Value;

        Assert.IsNotNull(service);
        Assert.AreEqual("Test Message", service.GetMessage());
    }

    [TestMethod]
    public void Value_WithComplexType_WorksCorrectly()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();

        var lazyResolve = new LazyResolve<ITestService>(services);

        var service = lazyResolve.Value;

        Assert.IsNotNull(service);
        Assert.IsInstanceOfType<TestService>(service);
    }

    [TestMethod]
    public void Constructor_WithNullServiceProvider_DoesNotThrow()
    {
        // This creates the instance with null, but will throw when Value is accessed
        var lazyResolve = new LazyResolve<ITestService>(null!);

        Assert.IsNotNull(lazyResolve);
        Assert.IsNull(lazyResolve.ValueOrDefault);
    }

    [TestMethod]
    public void Value_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        var lazyResolve = new LazyResolve<ITestService>(null!);

        try
        {
            _ = lazyResolve.Value;
            Assert.Fail("Expected ArgumentNullException");
        }
        catch (ArgumentNullException)
        {
            // Expected
        }
    }

    [TestMethod]
    public void ValueOrDefault_SetToNull_ThenAccessValue_ResolvesService()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, TestService>()
            .BuildServiceProvider();

        var lazyResolve = new LazyResolve<ITestService>(services);
        
        // Initially null
        Assert.IsNull(lazyResolve.ValueOrDefault);
        
        // Set to null explicitly
        lazyResolve.ValueOrDefault = null;
        
        // Now access Value - should resolve from service provider
        var service = lazyResolve.Value;
        
        Assert.IsNotNull(service);
        Assert.IsNotNull(lazyResolve.ValueOrDefault);
    }

    private class CountingService : ITestService
    {
        private static int _instanceCount = 0;
        public int InstanceNumber { get; }

        public CountingService()
        {
            InstanceNumber = ++_instanceCount;
        }

        public string GetMessage() => $"Instance {InstanceNumber}";
    }

    [TestMethod]
    public void Value_MultipleInstances_EachResolvesIndependently()
    {
        var services = new ServiceCollection()
            .AddTransient<ITestService, CountingService>()
            .BuildServiceProvider();

        var lazy1 = new LazyResolve<ITestService>(services);
        var lazy2 = new LazyResolve<ITestService>(services);
        var lazy3 = new LazyResolve<ITestService>(services);

        var service1 = lazy1.Value;
        var service2 = lazy2.Value;
        var service3 = lazy3.Value;

        // Each should have resolved its own instance
        Assert.AreNotSame(service1, service2);
        Assert.AreNotSame(service2, service3);
        Assert.AreNotSame(service1, service3);
    }
}
