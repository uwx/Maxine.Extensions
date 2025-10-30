using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Maxine.Extensions.Test;

[TestClass]
public class StartupTaskTests
{
    [TestMethod]
    public void TestAddStartupTask()
    {
        var services = new ServiceCollection();
        services.AddStartupTask<SampleStartupTask>();

        var provider = services.BuildServiceProvider();
        var task = provider.GetService<IStartupTask>();

        Assert.IsNotNull(task);
        Assert.IsInstanceOfType(task, typeof(SampleStartupTask));
    }

    [TestMethod]
    public async Task TestExecuteAsync()
    {
        var task = new SampleStartupTask();
        var cancellationTokenSource = new CancellationTokenSource();

        await task.ExecuteAsync(cancellationTokenSource.Token);

        Assert.IsTrue(task.Executed);
    }

    private class SampleStartupTask : IStartupTask
    {
        public bool Executed { get; private set; }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Executed = true;
            return Task.CompletedTask;
        }
    }
}