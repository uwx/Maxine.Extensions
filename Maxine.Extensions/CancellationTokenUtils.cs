using System.Diagnostics;
using JetBrains.Annotations;

namespace Maxine.Extensions.Shared;

public static class CancellationTokenUtils
{
#pragma warning disable CA1068
    public static WrappedCTS WrapWithTimeout(CancellationToken existing, TimeSpan? timeout)
#pragma warning restore CA1068
    {
        var cts = timeout != null
            ? CancellationTokenSource.CreateLinkedTokenSource(existing)
                .WithCancelAfter(timeout.Value)
            : null;
        var ct = cts?.Token ?? existing;

        return new WrappedCTS(cts, ct);
    }

    public readonly struct WrappedCTS : IDisposable
    {
        private readonly CancellationTokenSource? _toDispose;
        public CancellationToken Token { get; }

        internal WrappedCTS(CancellationTokenSource? toDispose, CancellationToken token)
        {
            _toDispose = toDispose;
            Token = token;
        }

        public void Dispose()
        {
            _toDispose?.Dispose();
        }
    }
}

public class CancelableTaskSource<TResult> : IDisposable, IAsyncDisposable
{
    private readonly Lock _lock = new();
    private bool _ranCanceledCallback;
    
    private event Func<ValueTask>? Canceled;

    private Func<TResult>? _resultFactory;
    
    private readonly TaskCompletionSource<TResult> _tcs = new();
    private TimerEx? _timeoutTimer;
    private bool _started;
    
    private CancellationTokenRegistration? _registration;
    private CancellationToken _token;

    /// <summary>
    /// Gets a task that resolves when this cancelable task is canceled, via cancellation token or timeout.
    /// </summary>
    public Task<TResult> Task => _tcs.Task;

    /// <summary>
    /// A handler for exceptions due to cancellation; this event is invoked when the <c>onCancel</c> handler throws an
    /// exception, or does not transition the <see cref="CancelableTaskSource{TResult}"/> into a completed state.
    /// </summary>
    public event Action<Exception>? CancellationFailed;

    /// <summary>
    /// Creates a new cancelable task. By default, this cancelable task will transition to
    /// <see cref="TaskStatus.Canceled"/> when canceled.
    /// </summary>
    public CancelableTaskSource()
    {
    }

    /// <summary>
    /// Specifies that this cancelable task will transition to <see cref="TaskStatus.RanToCompletion"/> state when
    /// canceled, with a result specified as the return value of calling <paramref name="resultFactory"/>.
    /// </summary>
    /// <param name="resultFactory">
    /// The factory that produces a value to set this cancelable task's result to if it is canceled.
    /// </param>
    /// <returns>The current instance, for chaining.</returns>
    /// <remarks>This function can only be called once per instance.</remarks>
    public CancelableTaskSource<TResult> WithResultOnCancel([RequireStaticDelegate] Func<TResult> resultFactory)
    {
        if (_resultFactory != null)
        {
            throw new InvalidOperationException("Result factory is already defined");
        }
        
        _resultFactory = resultFactory;
        return this;
    }

    /// <summary>
    /// Registers a function to be called when the <see cref="CancelableTaskSource{TResult}"/> is canceled via the
    /// cancellation token specified with <see cref="WithCancellationToken"/> being canceled, or the timeout specified
    /// by <see cref="WithTimeout"/> expiring.
    /// </summary>
    /// <param name="handler">The callback to register.</param>
    /// <returns>The current instance, for chaining.</returns>
    /// <remarks>
    /// The function is not called when the <see cref="CancelableTaskSource{TResult}"/> is manually transitioned to a
    /// completed state by <see cref="TrySetCanceled"/>, <see cref="TrySetException(Exception)"/>,
    /// <see cref="TrySetException(IEnumerable{Exception})"/>, or <see cref="TrySetResult"/>.
    /// </remarks>
    public CancelableTaskSource<TResult> OnCanceled(Func<ValueTask> handler)
    {
        Canceled += handler;
        return this;
    }

    public CancelableTaskSource<TResult> WithCancellationToken(CancellationToken token)
    {
        if (_token.CanBeCanceled)
        {
            throw new InvalidOperationException("Cancellation token is already defined");
        }

        _token = token;
        return this;
    }

    public CancelableTaskSource<TResult> WithTimeout(TimeSpan? timeout)
    {
        if (_timeoutTimer != null)
        {
            throw new InvalidOperationException("Timeout is already defined");
        }
        
        if (timeout != null)
        {
            _timeoutTimer = TimerEx.Once(MarkCanceled, timeout.Value);
        }
        return this;
    }

    public CancelableTaskSource<TResult> Start()
    {
        if (Task.IsCompleted || _started)
        {
            return this;
        }

        _started = true;
        
        // Ensures the cleanup routine is ran on completion, regardless of whether it's successful or not.
        Task.ContinueWithRelativelySafely(_ => DisposeAsync(), CancellationToken.None);

        // Start the timer first; then register the CancellationToken _after_, that way if the token is already
        // canceled, we don't start the timer while it's already been disposed by the on-cancel cleanup routine.
        _timeoutTimer?.Start();

        if (_token.CanBeCanceled)
        {
            _registration = _token.UnsafeRegister(MarkCanceled);
        }

        return this;
    }

    public CancelableTaskSource<TResult> RestartTimeout()
    {
        if (Task.IsCompleted)
        {
            return this;
        }

        if (!_started)
        {
            Start();
        }

        try
        {
            _timeoutTimer?.Restart();
        }
        catch (ObjectDisposedException)
        {
            // Rare unavoidable race condition, safe to ignore.
        }

        return this;
    }

    /// <summary>
    /// Calls the <see cref="Canceled"/> handler and ensures the <see cref="_tcs"/> has transitioned to a completion
    /// state.
    /// </summary>
    /// <remarks>This method may be called from multiple threads.</remarks>
    private async void MarkCanceled()
    {
        // Ensure this method is only called once at most.
        lock (_lock)
        {
            if (_ranCanceledCallback)
            {
                return;
            }

            _ranCanceledCallback = true;
        }

        try
        {
            if (Canceled != null)
            {
                await Canceled();
            }

            if (_resultFactory != null)
            {
                _tcs.TrySetResult(_resultFactory());
            }
            else
            {
                _tcs.TrySetCanceled();
            }
        }
        catch (Exception ex)
        {
            var propagatedException = new AggregateException(ex).SetCurrentStackTrace();

            try
            {
                CancellationFailed?.Invoke(propagatedException);
            }
            finally // Make sure the tcs is transitioned to _something_ even if CancellationFailed throws
            {
                _tcs.TrySetException(propagatedException);
            }
        }
    }

    /// <inheritdoc cref="TaskCompletionSource{TResult}.TrySetException(Exception)"/>
    /// <remarks>This method does <b>not</b> call the <see cref="OnCanceled"/> callback.</remarks>
    public bool TrySetException(Exception exception) => _tcs.TrySetException(exception);

    /// <inheritdoc cref="TaskCompletionSource{TResult}.TrySetException(IEnumerable{Exception})"/>
    /// <remarks>This method does <b>not</b> call the <see cref="OnCanceled"/> callback.</remarks>
    public bool TrySetException(IEnumerable<Exception> exceptions) => _tcs.TrySetException(exceptions);

    /// <inheritdoc cref="TaskCompletionSource{TResult}.TrySetResult"/>
    /// <remarks>This method does <b>not</b> call the <see cref="OnCanceled"/> callback.</remarks>
    public bool TrySetResult(TResult result) => _tcs.TrySetResult(result);

    /// <inheritdoc cref="TaskCompletionSource{TResult}.TrySetCanceled(CancellationToken)"/>
    /// <remarks>This method does <b>not</b> call the <see cref="OnCanceled"/> callback.</remarks>
    public bool TrySetCanceled(CancellationToken cancellationToken = default) => _tcs.TrySetCanceled(cancellationToken);
    
    ~CancelableTaskSource()
    {
        CancelableTaskSource.OnLeakedScopeHandle(this);
        Debugger.Break();
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _tcs.TrySetCanceled();
        _timeoutTimer?.Dispose();
        _registration?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _tcs.TrySetCanceled();
        if (_timeoutTimer is { } timer)
            await timer.DisposeAsync();
        if (_registration is {} registration)
            await registration.DisposeAsync();
    }
}

public static class CancelableTaskSource
{
    public static Action<object> OnLeakedScopeHandle { internal get; set; } = _ =>
    {
        Console.WriteLine("Leaked scope handle in CancelableTaskSource!!");
        Debugger.Break();
    };
}