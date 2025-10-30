using Microsoft.Extensions.DependencyInjection;

namespace Maxine.Extensions.Test;

[TestClass]
public class StartupTaskTests
{
    [TestMethod]
    public void AddStartupTask_WithGenericType_AddsToServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddStartupTask<TestStartupTask>();
        
        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IStartupTask));
        Assert.IsNotNull(serviceDescriptor);
        Assert.AreEqual(typeof(TestStartupTask), serviceDescriptor.ImplementationType);
    }

    [TestMethod]
    public void AddStartupTask_WithDelegate_AddsToServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddStartupTask(sp => Task.CompletedTask);
        
        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IStartupTask));
        Assert.IsNotNull(serviceDescriptor);
    }

    [TestMethod]
    public async Task DelegateStartupTask_ExecutesDelegate()
    {
        bool executed = false;
        var services = new ServiceCollection();
        services.AddStartupTask(sp =>
        {
            executed = true;
            return Task.CompletedTask;
        });
        
        var provider = services.BuildServiceProvider();
        var task = provider.GetService<IStartupTask>();
        
        Assert.IsNotNull(task);
        await task.ExecuteAsync();
        Assert.IsTrue(executed);
    }

    private class TestStartupTask : IStartupTask
    {
        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
