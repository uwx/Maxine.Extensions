using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Maxine.Extensions;

public static class CancellationTokenExtensions
{
    /// <summary>
    /// Registers a delegate that will be called when this
    /// <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this token is already in the canceled state, the
    /// delegate will be run immediately and synchronously. Any exception the delegate generates will be
    /// propagated out of this method call.
    /// </para>
    /// <para>
    /// The current <see cref="System.Threading.ExecutionContext">ExecutionContext</see>, if one exists, will be captured
    /// along with the delegate and will be used when executing it.
    /// </para>
    /// </remarks>
    /// <param name="ct">The cancellation token</param>
    /// <param name="callback">The delegate to be executed when the <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked.  This may be null.</param>
    /// <returns>The <see cref="System.Threading.CancellationTokenRegistration"/> instance that can
    /// be used to unregister the callback.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenRegistration Register<T>(this CancellationToken ct, Action<T> callback, T state)
        => ct.Register(state1 => callback((T)state1!), state);

    /// <summary>Registers a delegate that will be called when this <see cref="CancellationToken">CancellationToken</see> is canceled.</summary>
    /// <remarks>
    /// If this token is already in the canceled state, the delegate will be run immediately and synchronously. Any exception the delegate
    /// generates will be propagated out of this method call. The current <see cref="ExecutionContext">ExecutionContext</see>, if one exists,
    /// will be captured along with the delegate and will be used when executing it. The current <see cref="SynchronizationContext"/> is not captured.
    /// </remarks>
    /// <param name="ct">The cancellation token</param>
    /// <param name="callback">The delegate to be executed when the <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked.  This may be null.</param>
    /// <returns>The <see cref="CancellationTokenRegistration"/> instance that can be used to unregister the callback.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenRegistration Register<T>(this CancellationToken ct, Action<T, CancellationToken> callback, T state)
        => ct.Register((state1, ct1) => callback((T)state1!, ct1), state);

    /// <summary>
    /// Registers a delegate that will be called when this
    /// <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this token is already in the canceled state, the
    /// delegate will be run immediately and synchronously. Any exception the delegate generates will be
    /// propagated out of this method call.
    /// </para>
    /// <para>
    /// The current <see cref="System.Threading.ExecutionContext">ExecutionContext</see>, if one exists,
    /// will be captured along with the delegate and will be used when executing it.
    /// </para>
    /// </remarks>
    /// <param name="ct">The cancellation token</param>
    /// <param name="callback">The delegate to be executed when the <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked.  This may be null.</param>
    /// <param name="useSynchronizationContext">A Boolean value that indicates whether to capture
    /// the current <see cref="System.Threading.SynchronizationContext">SynchronizationContext</see> and use it
    /// when invoking the <paramref name="callback"/>.</param>
    /// <returns>The <see cref="System.Threading.CancellationTokenRegistration"/> instance that can
    /// be used to unregister the callback.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
    /// <exception cref="System.ObjectDisposedException">The associated <see
    /// cref="System.Threading.CancellationTokenSource">CancellationTokenSource</see> has been disposed.</exception>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenRegistration Register<T>(this CancellationToken ct, Action<T> callback, T state, bool useSynchronizationContext)
        => ct.Register(state1 => callback((T)state1!), state, useSynchronizationContext);

    /// <summary>
    /// Registers a delegate that will be called when this
    /// <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this token is already in the canceled state, the delegate will be run immediately and synchronously.
    /// Any exception the delegate generates will be propagated out of this method call.
    /// </para>
    /// <para>
    /// <see cref="System.Threading.ExecutionContext">ExecutionContext</see> is not captured nor flowed
    /// to the callback's invocation.
    /// </para>
    /// </remarks>
    /// <param name="ct">The cancellation token</param>
    /// <param name="callback">The delegate to be executed when the <see cref="System.Threading.CancellationToken">CancellationToken</see> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked.  This may be null.</param>
    /// <returns>The <see cref="System.Threading.CancellationTokenRegistration"/> instance that can
    /// be used to unregister the callback.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="callback"/> is null.</exception>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenRegistration UnsafeRegister<T>(this CancellationToken ct, Action<T> callback, T state)
        => ct.UnsafeRegister(state1 => callback((T)state1!), state);

    /// <summary>Registers a delegate that will be called when this <see cref="CancellationToken">CancellationToken</see> is canceled.</summary>
    /// <remarks>
    /// If this token is already in the canceled state, the delegate will be run immediately and synchronously. Any exception the delegate
    /// generates will be propagated out of this method call. <see cref="ExecutionContext"/> is not captured nor flowed to the callback's invocation.
    /// </remarks>
    /// <param name="ct">The cancellation token</param>
    /// <param name="callback">The delegate to be executed when the <see cref="CancellationToken">CancellationToken</see> is canceled.</param>
    /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked.  This may be null.</param>
    /// <returns>The <see cref="CancellationTokenRegistration"/> instance that can be used to unregister the callback.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenRegistration UnsafeRegister<T>(this CancellationToken ct, Action<T, CancellationToken> callback, T state)
        => ct.UnsafeRegister((state1, ct1) => callback((T)state1!, ct1), state);
    
    /// <summary>Registers a delegate that will be called when this <see cref="CancellationToken">CancellationToken</see> is canceled.</summary>
    /// <remarks>
    /// If this token is already in the canceled state, the delegate will be run immediately and synchronously. Any exception the delegate
    /// generates will be propagated out of this method call. <see cref="ExecutionContext"/> is not captured nor flowed to the callback's invocation.
    /// </remarks>
    /// <param name="ct">The cancellation token</param>
    /// <param name="callback">The delegate to be executed when the <see cref="CancellationToken">CancellationToken</see> is canceled.</param>
    /// <returns>The <see cref="CancellationTokenRegistration"/> instance that can be used to unregister the callback.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenRegistration UnsafeRegister(this CancellationToken ct, Action callback)
        => ct.UnsafeRegister(_ => callback(), null);

    /// <summary>Schedules a Cancel operation on this <see cref="CancellationTokenSource"/>.</summary>
    /// <param name="self">The current <see cref="CancellationTokenSource"/>.</param>
    /// <param name="delay">The time span to wait before canceling this <see cref="CancellationTokenSource"/>.
    /// </param>
    /// <returns>The current <see cref="CancellationTokenSource"/>.</returns>
    /// <exception cref="ObjectDisposedException">The exception thrown when this <see
    /// cref="CancellationTokenSource"/> has been disposed.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The <paramref name="delay"/> is less than -1 or greater than maximum allowed timer duration.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The countdown for the delay starts during this call.  When the delay expires,
    /// this <see cref="CancellationTokenSource"/> is canceled, if it has
    /// not been canceled already.
    /// </para>
    /// <para>
    /// Subsequent calls to CancelAfter will reset the delay for this
    /// <see cref="CancellationTokenSource"/>, if it has not been canceled already.
    /// </para>
    /// </remarks>
    [PublicAPI]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationTokenSource WithCancelAfter(this CancellationTokenSource self, TimeSpan delay)
    {
        self.CancelAfter(delay);
        return self;
    }
}