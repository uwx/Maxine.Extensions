using Maxine.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Maxine.Extensions.Test;

[TestClass]
public class MsdiExtensionsAdditionalTests
{
    private interface ITestService
    {
        string GetName();
    }

    private interface IAnotherService
    {
        string GetName();
    }

    private class TestService : ITestService
    {
        public string GetName() => "TestService";
    }

    private class DualService : ITestService, IAnotherService
    {
        public string GetName() => "DualService";
    }

    private class TestHostedService : IHostedService
    {
        public bool Started { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Started = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Started = false;
            return Task.CompletedTask;
        }
    }

    private class TestConfiguredService
    {
        public string Value { get; set; } = "default";
        public int Number { get; set; } = 0;
    }

    [TestMethod]
    public void AlsoAs_RegistersServiceWithAlias()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, DualService>()
                .AlsoAs<IAnotherService>();

        var provider = services.BuildServiceProvider();
        var testService = provider.GetRequiredService<ITestService>();
        var anotherService = provider.GetRequiredService<IAnotherService>();

        Assert.IsNotNull(testService);
        Assert.IsNotNull(anotherService);
        Assert.AreSame((object)testService, (object)anotherService);
    }

    [TestMethod]
    public void AlsoAs_MultipleAliases_AllResolveToSameInstance()
    {
        var services = new ServiceCollection();
        services.AddSingleton<DualService>()
                .AlsoAs<ITestService>()
                .AlsoAs<IAnotherService>();

        var provider = services.BuildServiceProvider();
        var original = provider.GetRequiredService<DualService>();
        var alias1 = provider.GetRequiredService<ITestService>();
        var alias2 = provider.GetRequiredService<IAnotherService>();

        Assert.AreSame((object)original, (object)alias1);
        Assert.AreSame((object)original, (object)alias2);
    }

    [TestMethod]
    public void AlsoAs_RespectsServiceLifetime()
    {
        var services = new ServiceCollection();
        services.AddTransient<ITestService, TestService>()
                .AlsoAs<IAnotherService>();

        var provider = services.BuildServiceProvider();
        var instance1 = provider.GetRequiredService<ITestService>();
        var instance2 = provider.GetRequiredService<ITestService>();

        Assert.AreNotSame((object)instance1, (object)instance2);
    }

    [TestMethod]
    public void AlsoAsHostedService_RegistersAsHostedService()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TestHostedService>()
                .AlsoAsHostedService();

        var provider = services.BuildServiceProvider();
        var hostedService = provider.GetRequiredService<IHostedService>();
        var originalService = provider.GetRequiredService<TestHostedService>();

        Assert.IsNotNull(hostedService);
        Assert.AreSame((object)originalService, (object)hostedService);
    }

    [TestMethod]
    public void AddConfiguredSingleton_CreatesConfigurableService()
    {
        var services = new ServiceCollection();
        services.AddConfiguredSingleton<TestConfiguredService>();
        services.Configure<TestConfiguredService>(options =>
        {
            options.Value = "configured";
            options.Number = 42;
        });

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<TestConfiguredService>();

        Assert.IsNotNull(service);
        Assert.AreEqual("configured", service.Value);
        Assert.AreEqual(42, service.Number);
    }

    [TestMethod]
    public void AddConfiguredSingleton_ReturnsSingletonInstance()
    {
        var services = new ServiceCollection();
        services.AddConfiguredSingleton<TestConfiguredService>();
        services.Configure<TestConfiguredService>(options => { }); // Need to configure options

        var provider = services.BuildServiceProvider();
        var instance1 = provider.GetRequiredService<TestConfiguredService>();
        var instance2 = provider.GetRequiredService<TestConfiguredService>();

        Assert.AreSame(instance1, instance2);
    }

    [TestMethod]
    public void AddConfiguredSingleton_WithoutConfiguration_UsesDefaults()
    {
        var services = new ServiceCollection();
        services.AddConfiguredSingleton<TestConfiguredService>();
        services.Configure<TestConfiguredService>(options => { }); // Need to configure options even if not setting values

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<TestConfiguredService>();

        Assert.IsNotNull(service);
        Assert.AreEqual("default", service.Value);
        Assert.AreEqual(0, service.Number);
    }

    [TestMethod]
    public void AlsoAs_ChainedRegistrations_Work()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        services.AddSingleton<DualService>()
                .AlsoAs<IAnotherService>();

        var provider = services.BuildServiceProvider();
        var testService = provider.GetRequiredService<ITestService>();
        var anotherService = provider.GetRequiredService<IAnotherService>();

        Assert.IsInstanceOfType<TestService>(testService);
        Assert.IsInstanceOfType<DualService>(anotherService);
    }
}
