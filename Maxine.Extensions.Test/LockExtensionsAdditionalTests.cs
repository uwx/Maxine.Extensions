using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxine.Extensions;

namespace Maxine.Extensions.Test;

[TestClass]
public class LockExtensionsAdditionalTests
{
    [TestMethod]
    public void GrabReadLock_AcquiresAndReleasesReadLock()
    {
        var rwLock = new ReaderWriterLockSlim();

        using (rwLock.GrabReadLock())
        {
            Assert.IsTrue(rwLock.IsReadLockHeld);
        }

        Assert.IsFalse(rwLock.IsReadLockHeld);
    }

    [TestMethod]
    public async Task GrabReadLock_MultipleReaders_CanAcquireConcurrently()
    {
        var rwLock = new ReaderWriterLockSlim();
        var readersAcquired = 0;

        var task1 = Task.Run(() =>
        {
            using (rwLock.GrabReadLock())
            {
                Interlocked.Increment(ref readersAcquired);
                Thread.Sleep(50); // Hold lock briefly
            }
        });

        var task2 = Task.Run(() =>
        {
            using (rwLock.GrabReadLock())
            {
                Interlocked.Increment(ref readersAcquired);
                Thread.Sleep(50); // Hold lock briefly
            }
        });

        await Task.WhenAll(task1, task2);
        Assert.AreEqual(2, readersAcquired);
        Assert.AreEqual(0, rwLock.CurrentReadCount);
    }

    [TestMethod]
    public void GrabWriteLock_AcquiresAndReleasesWriteLock()
    {
        var rwLock = new ReaderWriterLockSlim();


        using (rwLock.GrabWriteLock())
        {
            Assert.IsTrue(rwLock.IsWriteLockHeld);
        }

        Assert.IsFalse(rwLock.IsWriteLockHeld);
    }

    [TestMethod]
    public void GrabWriteLock_BlocksReaders()
    {
        var rwLock = new ReaderWriterLockSlim();
        var readerBlocked = false;

        using (rwLock.GrabWriteLock())
        {
            var readerTask = Task.Run(() =>
            {
                if (!rwLock.TryEnterReadLock(50))
                {
                    readerBlocked = true;
                }
                else
                {
                    rwLock.ExitReadLock();
                }
            });

            readerTask.Wait();
            Assert.IsTrue(readerBlocked);
        }
    }

    [TestMethod]
    public void GrabReadLock_ReleasedOnException()
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
    public void GrabWriteLock_ReleasedOnException()
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

    [TestMethod]
    public void GrabReadLock_NestedCalls_Work()
    {
        var rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        using (rwLock.GrabReadLock())
        {
            Assert.IsTrue(rwLock.IsReadLockHeld);
            
            using (rwLock.GrabReadLock())
            {
                Assert.AreEqual(2, rwLock.RecursiveReadCount);
            }
            
            Assert.AreEqual(1, rwLock.RecursiveReadCount);
        }

        Assert.IsFalse(rwLock.IsReadLockHeld);
    }

    [TestMethod]
    public void GrabWriteLock_ExcludesOtherWriters()
    {
        var rwLock = new ReaderWriterLockSlim();
        var writerBlocked = false;
        var firstWriterAcquired = new ManualResetEventSlim(false);

        var writerTask = Task.Run(() =>
        {
            firstWriterAcquired.Wait(1000); // Wait for first writer to signal
            if (!rwLock.TryEnterWriteLock(50))
            {
                writerBlocked = true;
            }
            else
            {
                rwLock.ExitWriteLock();
            }
        });

        using (rwLock.GrabWriteLock())
        {
            Assert.IsTrue(rwLock.IsWriteLockHeld);
            firstWriterAcquired.Set(); // Signal that we have the lock
            Thread.Sleep(100); // Hold lock while second writer tries
        }

        writerTask.Wait();
        Assert.IsTrue(writerBlocked, "Second writer should have been blocked");
    }
}
