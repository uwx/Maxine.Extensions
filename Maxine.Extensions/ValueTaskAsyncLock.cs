/*!
The MIT License (MIT)

Copyright (c) 2014 StephenCleary

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

using JetBrains.Annotations;

namespace Maxine.Extensions;

using System.Runtime.CompilerServices;
using Nito.AsyncEx;
using Nito.Collections;
using Nito.Disposables;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// Original idea from Stephen Toub: http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx

/// <summary>
/// A mutual exclusion lock that is compatible with async. Note that this lock is <b>not</b> recursive!
/// </summary>
/// <remarks>
/// <para>This is the <c>async</c>-ready almost-equivalent of the <c>lock</c> keyword or the <see cref="Mutex"/> type, similar to <a href="http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx">Stephen Toub's AsyncLock</a>. It's only <i>almost</i> equivalent because the <c>lock</c> keyword permits reentrancy, which is not currently possible to do with an <c>async</c>-ready lock.</para>
/// <para>An <see cref="ValueTaskAsyncLock"/> is either taken or not. The lock can be asynchronously acquired by calling <see autoUpgrade="true" cref="LockAsync()"/>, and it is released by disposing the result of that task. <see cref="LockAsync(CancellationToken)"/> takes an optional <see cref="CancellationToken"/>, which can be used to cancel the acquiring of the lock.</para>
/// <para>The task returned from <see autoUpgrade="true" cref="LockAsync()"/> will enter the <c>Completed</c> state when it has acquired the <see cref="ValueTaskAsyncLock"/>. That same task will enter the <c>Canceled</c> state if the <see cref="CancellationToken"/> is signaled before the wait is satisfied; in that case, the <see cref="ValueTaskAsyncLock"/> is not taken by that task.</para>
/// <para>You can call <see cref="LockAsync(CancellationToken)"/> with an already-cancelled <see cref="CancellationToken"/> to attempt to acquire the <see cref="ValueTaskAsyncLock"/> immediately without actually entering the wait queue.</para>
/// </remarks>
/// <example>
/// <para>The vast majority of use cases are to just replace a <c>lock</c> statement. That is, with the original code looking like this:</para>
/// <code>
/// private readonly object _mutex = new object();
/// public void DoStuff()
/// {
///     lock (_mutex)
///     {
///         Thread.Sleep(TimeSpan.FromSeconds(1));
///     }
/// }
/// </code>
/// <para>If we want to replace the blocking operation <c>Thread.Sleep</c> with an asynchronous equivalent, it's not directly possible because of the <c>lock</c> block. We cannot <c>await</c> inside of a <c>lock</c>.</para>
/// <para>So, we use the <c>async</c>-compatible <see cref="ValueTaskAsyncLock"/> instead:</para>
/// <code>
/// private readonly AsyncLock _mutex = new AsyncLock();
/// public async Task DoStuffAsync()
/// {
///     using (await _mutex.LockAsync())
///     {
///         await Task.Delay(TimeSpan.FromSeconds(1));
///     }
/// }
/// </code>
/// </example>
[DebuggerDisplay("Taken = {_taken}")]
[DebuggerTypeProxy(typeof(DebugView))]
[PublicAPI]
public sealed class ValueTaskAsyncLock
{
    /// <summary>
    /// Whether the lock is taken by a task.
    /// </summary>
    private bool _taken;

    /// <summary>
    /// The queue of TCSs that other tasks are awaiting to acquire the lock.
    /// </summary>
    private readonly IAsyncWaitQueue<IDisposable> _queue;

    /// <summary>
    /// The object used for mutual exclusion.
    /// </summary>
    private readonly object _mutex;

    /// <summary>
    /// Creates a new async-compatible mutual exclusion lock.
    /// </summary>
    public ValueTaskAsyncLock()
        : this(null)
    {
    }

    /// <summary>
    /// Creates a new async-compatible mutual exclusion lock using the specified wait queue.
    /// </summary>
    /// <param name="queue">The wait queue used to manage waiters. This may be <c>null</c> to use a default (FIFO) queue.</param>
    private ValueTaskAsyncLock(IAsyncWaitQueue<IDisposable>? queue)
    {
        _queue = queue ?? new DefaultAsyncWaitQueue<IDisposable>();
        _mutex = new object();
    }

    /// <summary>
    /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the lock. If this is already set, then this method will attempt to take the lock immediately (succeeding if the lock is currently available).</param>
    /// <returns>A disposable that releases the lock when disposed.</returns>
    private ValueTask<IDisposable> RequestLockAsync(CancellationToken cancellationToken)
    {
        lock (_mutex)
        {
            if (!_taken)
            {
                // If the lock is available, take it immediately.
                _taken = true;
                return ValueTask.FromResult<IDisposable>(new Key(this));
            }
            else
            {
                // Wait for the lock to become available or cancellation.
                return _queue.Enqueue(_mutex, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the lock. If this is already set, then this method will attempt to take the lock immediately (succeeding if the lock is currently available).</param>
    /// <returns>A disposable that releases the lock when disposed.</returns>
    public ValueAwaitableDisposable<IDisposable> LockAsync(CancellationToken cancellationToken)
    {
        return new ValueAwaitableDisposable<IDisposable>(RequestLockAsync(cancellationToken));
    }

    /// <summary>
    /// Asynchronously acquires the lock. Returns a disposable that releases the lock when disposed.
    /// </summary>
    /// <returns>A disposable that releases the lock when disposed.</returns>
    public ValueAwaitableDisposable<IDisposable> LockAsync()
    {
        return LockAsync(CancellationToken.None);
    }

    // /// <summary>
    // /// Synchronously acquires the lock. Returns a disposable that releases the lock when disposed. This method may block the calling thread.
    // /// </summary>
    // /// <param name="cancellationToken">The cancellation token used to cancel the lock. If this is already set, then this method will attempt to take the lock immediately (succeeding if the lock is currently available).</param>
    // public IDisposable Lock(CancellationToken cancellationToken)
    // {
    //     return RequestLockAsync(cancellationToken).WaitAndUnwrapException();
    // }
    //
    // /// <summary>
    // /// Synchronously acquires the lock. Returns a disposable that releases the lock when disposed. This method may block the calling thread.
    // /// </summary>
    // public IDisposable Lock()
    // {
    //     return Lock(CancellationToken.None);
    // }

    /// <summary>
    /// Releases the lock.
    /// </summary>
    private void ReleaseLock()
    {
        lock (_mutex)
        {
            if (_queue.IsEmpty)
                _taken = false;
            else
#pragma warning disable CA2000 // Dispose objects before losing scope
                _queue.Dequeue(new Key(this));
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
    }

    /// <summary>
    /// The disposable which releases the lock.
    /// </summary>
    private sealed class Key : SingleDisposable<ValueTaskAsyncLock>
    {
        /// <summary>
        /// Creates the key for a lock.
        /// </summary>
        /// <param name="valueTaskAsyncLock">The lock to release. May not be <c>null</c>.</param>
        public Key(ValueTaskAsyncLock valueTaskAsyncLock)
            : base(valueTaskAsyncLock)
        {
        }

        protected override void Dispose(ValueTaskAsyncLock context)
        {
            context.ReleaseLock();
        }
    }

    // ReSharper disable UnusedMember.Local
    [DebuggerNonUserCode]
    private sealed class DebugView(ValueTaskAsyncLock mutex)
    {
        public bool Taken => mutex._taken;

        public IAsyncWaitQueue<IDisposable> WaitQueue => mutex._queue;
    }
    // ReSharper restore UnusedMember.Local
}

/// <summary>
/// An awaitable wrapper around a task whose result is disposable. The wrapper is not disposable, so this prevents usage errors like "using (MyAsync())" when the appropriate usage should be "using (await MyAsync())".
/// </summary>
/// <typeparam name="T">The type of the result of the underlying task.</typeparam>
[PublicAPI]
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct ValueAwaitableDisposable<T> where T : IDisposable
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    /// <summary>
    /// The underlying task.
    /// </summary>
    private readonly ValueTask<T> _task;

    /// <summary>
    /// Initializes a new awaitable wrapper around the specified task.
    /// </summary>
    /// <param name="task">The underlying task to wrap. This may not be <c>null</c>.</param>
    public ValueAwaitableDisposable(ValueTask<T> task)
    {
        ArgumentNullException.ThrowIfNull(task);
        _task = task;
    }

    /// <summary>
    /// Returns the underlying task.
    /// </summary>
    public ValueTask<T> AsValueTask()
    {
        return _task;
    }

    /// <summary>
    /// Implicit conversion to the underlying task.
    /// </summary>
    /// <param name="source">The awaitable wrapper.</param>
#pragma warning disable CA2225 // Operator overloads have named alternates
    public static implicit operator ValueTask<T>(ValueAwaitableDisposable<T> source)
#pragma warning restore CA2225 // Operator overloads have named alternates
    {
        return source.AsValueTask();
    }

    /// <summary>
    /// Infrastructure. Returns the task awaiter for the underlying task.
    /// </summary>
    public ValueTaskAwaiter<T> GetAwaiter()
    {
        return _task.GetAwaiter();
    }

    /// <summary>
    /// Infrastructure. Returns a configured task awaiter for the underlying task.
    /// </summary>
    /// <param name="continueOnCapturedContext">Whether to attempt to marshal the continuation back to the captured context.</param>
    public ConfiguredValueTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext)
    {
        return _task.ConfigureAwait(continueOnCapturedContext);
    }
}

file static class AsyncWaitQueueExtensions
{
    /// <summary>
    /// Creates a new entry and queues it to this wait queue. If the cancellation token is already canceled, this method immediately returns a canceled task without modifying the wait queue.
    /// </summary>
    /// <param name="this">The wait queue.</param>
    /// <param name="mutex">A synchronization object taken while cancelling the entry.</param>
    /// <param name="token">The token used to cancel the wait.</param>
    /// <returns>The queued task.</returns>
    public static async ValueTask<T> Enqueue<T>(this IAsyncWaitQueue<T> @this, object mutex, CancellationToken token)
    {
        if (token.IsCancellationRequested)
            return await ValueTask.FromCanceled<T>(token);

        var ret = @this.Enqueue();
        if (!token.CanBeCanceled)
            return await ret;

        var registration = token.Register(() =>
        {
            lock (mutex)
                @this.TryCancel(ret, token);
        }, useSynchronizationContext: false);
        
        _ = ret.ContinueWith(_ => registration.Dispose(), CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        
        return await ret;
    }
}

/// <summary>
/// A collection of cancelable <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" /> instances. Implementations must assume the caller is holding a lock.
/// </summary>
/// <typeparam name="T">The type of the results. If this isn't needed, use <see cref="T:System.Object" />.</typeparam>
internal interface IAsyncWaitQueue<T>
{
    /// <summary>Gets whether the queue is empty.</summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Creates a new entry and queues it to this wait queue. The returned task must support both synchronous and asynchronous waits.
    /// </summary>
    /// <returns>The queued task.</returns>
    Task<T> Enqueue();

    /// <summary>
    /// Removes a single entry in the wait queue and completes it. This method may only be called if <see cref="P:Nito.AsyncEx.IAsyncWaitQueue`1.IsEmpty" /> is <c>false</c>. The task continuations for the completed task must be executed asynchronously.
    /// </summary>
    /// <param name="result">The result used to complete the wait queue entry. If this isn't needed, use <c>default(T)</c>.</param>
    void Dequeue(T? result = default);

    /// <summary>
    /// Removes all entries in the wait queue and completes them. The task continuations for the completed tasks must be executed asynchronously.
    /// </summary>
    /// <param name="result">The result used to complete the wait queue entries. If this isn't needed, use <c>default(T)</c>.</param>
    void DequeueAll(T? result = default);

    /// <summary>
    /// Attempts to remove an entry from the wait queue and cancels it. The task continuations for the completed task must be executed asynchronously.
    /// </summary>
    /// <param name="task">The task to cancel.</param>
    /// <param name="cancellationToken">The cancellation token to use to cancel the task.</param>
    bool TryCancel(Task task, CancellationToken cancellationToken);

    /// <summary>
    /// Removes all entries from the wait queue and cancels them. The task continuations for the completed tasks must be executed asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use to cancel the tasks.</param>
    void CancelAll(CancellationToken cancellationToken);
}

/// <summary>
/// The default wait queue implementation, which uses a double-ended queue.
/// </summary>
/// <typeparam name="T">The type of the results. If this isn't needed, use <see cref="Object"/>.</typeparam>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(DefaultAsyncWaitQueue<>.DebugView))]
file sealed class DefaultAsyncWaitQueue<T> : IAsyncWaitQueue<T>
{
    private readonly Deque<TaskCompletionSource<T>> _queue = [];

    private int Count => _queue.Count;

    bool IAsyncWaitQueue<T>.IsEmpty => Count == 0;

    Task<T> IAsyncWaitQueue<T>.Enqueue()
    {
        var tcs = TaskCompletionSourceExtensions.CreateAsyncTaskSource<T>();
        _queue.AddToBack(tcs);
        return tcs.Task;
    }

    void IAsyncWaitQueue<T>.Dequeue(T? result)
    {
        _queue.RemoveFromFront().TrySetResult(result!);
    }

    void IAsyncWaitQueue<T>.DequeueAll(T? result)
    {
        foreach (var source in _queue)
            source.TrySetResult(result!);
        _queue.Clear();
    }

    bool IAsyncWaitQueue<T>.TryCancel(Task task, CancellationToken cancellationToken)
    {
        for (int i = 0; i != _queue.Count; ++i)
        {
            if (_queue[i].Task == task)
            {
                _queue[i].TrySetCanceled(cancellationToken);
                _queue.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    void IAsyncWaitQueue<T>.CancelAll(CancellationToken cancellationToken)
    {
        foreach (var source in _queue)
            source.TrySetCanceled(cancellationToken);
        _queue.Clear();
    }

    [DebuggerNonUserCode]
    internal sealed class DebugView(DefaultAsyncWaitQueue<T> queue)
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Task<T>[] Tasks
        {
            get
            {
                var result = new List<Task<T>>(queue._queue.Count);
                foreach (var entry in queue._queue)
                    result.Add(entry.Task);
                return result.ToArray();
            }
        }
    }
}