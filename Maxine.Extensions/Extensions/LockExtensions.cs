namespace Maxine.Extensions;

public static class LockExtensions
{
    public static DisposableReadLockHandle GrabReadLock(this ReaderWriterLockSlim @lock)
    {
        @lock.EnterReadLock();
        return new DisposableReadLockHandle(@lock);
    }

    public readonly struct DisposableReadLockHandle(ReaderWriterLockSlim @lock) : IDisposable
    {
        public void Dispose()
        {
            @lock.ExitReadLock();
        }
    }

    public static DisposableWriteLockHandle GrabWriteLock(this ReaderWriterLockSlim @lock)
    {
        @lock.EnterWriteLock();
        return new DisposableWriteLockHandle(@lock);
    }

    public readonly struct DisposableWriteLockHandle(ReaderWriterLockSlim @lock) : IDisposable
    {
        public void Dispose()
        {
            @lock.ExitWriteLock();
        }
    }
}