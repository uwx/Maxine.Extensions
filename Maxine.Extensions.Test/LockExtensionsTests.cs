using Maxine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Maxine.Extensions.Test;

[TestClass]
public class LockExtensionsTests
{
    [TestMethod]
    public void GrabReadLock_AcquiresAndReleasesLock()
    {
        var rwLock = new ReaderWriterLockSlim();
        
        using (rwLock.GrabReadLock())
        {
            Assert.IsTrue(rwLock.IsReadLockHeld);
        }
        
        Assert.IsFalse(rwLock.IsReadLockHeld);
    }

    [TestMethod]
    public void GrabWriteLock_AcquiresAndReleasesLock()
    {
        var rwLock = new ReaderWriterLockSlim();
        
        using (rwLock.GrabWriteLock())
        {
            Assert.IsTrue(rwLock.IsWriteLockHeld);
        }
        
        Assert.IsFalse(rwLock.IsWriteLockHeld);
    }

    [TestMethod]
    public void GrabReadLock_MultipleReaders_CanAcquireSimultaneously()
    {
        var rwLock = new ReaderWriterLockSlim();
        
        using (rwLock.GrabReadLock())
        using (rwLock.GrabReadLock())
        {
            Assert.IsTrue(rwLock.IsReadLockHeld);
            Assert.AreEqual(2, rwLock.RecursiveReadCount);
        }
        
        Assert.IsFalse(rwLock.IsReadLockHeld);
    }

    [TestMethod]
    public void GrabWriteLock_ProtectsSharedResource()
    {
        var rwLock = new ReaderWriterLockSlim();
        var sharedValue = 0;
        
        using (rwLock.GrabWriteLock())
        {
            sharedValue = 42;
            Assert.IsTrue(rwLock.IsWriteLockHeld);
        }
        
        using (rwLock.GrabReadLock())
        {
            Assert.AreEqual(42, sharedValue);
        }
    }

    [TestMethod]
    public async Task GrabReadLock_WithAsyncContext_WorksCorrectly()
    {
        var rwLock = new ReaderWriterLockSlim();
        var executed = false;
        
        await Task.Run(() =>
        {
            using (rwLock.GrabReadLock())
            {
                executed = true;
                Assert.IsTrue(rwLock.IsReadLockHeld);
            }
        });
        
        Assert.IsTrue(executed);
        Assert.IsFalse(rwLock.IsReadLockHeld);
    }

    [TestMethod]
    public async Task GrabWriteLock_WithAsyncContext_WorksCorrectly()
    {
        var rwLock = new ReaderWriterLockSlim();
        var sharedValue = 0;
        
        await Task.Run(() =>
        {
            using (rwLock.GrabWriteLock())
            {
                sharedValue = 99;
                Assert.IsTrue(rwLock.IsWriteLockHeld);
            }
        });
        
        Assert.AreEqual(99, sharedValue);
        Assert.IsFalse(rwLock.IsWriteLockHeld);
    }

    [TestMethod]
    public void GrabReadLock_ExceptionInUsing_ReleasesLock()
    {
        var rwLock = new ReaderWriterLockSlim();
        
        try
        {
            using (rwLock.GrabReadLock())
            {
                Assert.IsTrue(rwLock.IsReadLockHeld);
                throw new InvalidOperationException("Test exception");
            }
        }
        catch (InvalidOperationException)
        {
            // Expected
        }
        
        Assert.IsFalse(rwLock.IsReadLockHeld);
    }

    [TestMethod]
    public void GrabWriteLock_ExceptionInUsing_ReleasesLock()
    {
        var rwLock = new ReaderWriterLockSlim();
        
        try
        {
            using (rwLock.GrabWriteLock())
            {
                Assert.IsTrue(rwLock.IsWriteLockHeld);
                throw new InvalidOperationException("Test exception");
            }
        }
        catch (InvalidOperationException)
        {
            // Expected
        }
        
        Assert.IsFalse(rwLock.IsWriteLockHeld);
    }
}
