namespace Maxine.Extensions.Shared;

/// <summary>
/// Provides extensions for safely applying continuations to a <see cref="Task"/> or <see cref="Task{TResult}"/>.
/// </summary>
public static class TaskExtensions
{
    // https://stackoverflow.com/a/44136947
    
    /// <summary>
    /// Creates a continuation that executes when the target <see cref="Task{TResult}"/> completes.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the result produced by this <see cref="Task{TResult}"/>.
    /// </typeparam>
    /// <typeparam name="TNewResult">
    /// The type of the result produced by the continuation.
    /// </typeparam>
    /// <param name="task">The task to apply the continuation to.</param>
    /// <param name="continuationFunction">
    /// A function to run when the <see cref="Task{TResult}"/> completes. When run, the delegate will be passed as
    /// an argument this completed task.
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that will be assigned to the new task.</param>
    /// <returns>A new continuation <see cref="Task{TNewResult}"/>.</returns>
    /// <remarks>
    /// This method uses sane defaults compared to the original <see cref="Task{TResult}.ContinueWith{TNewResult}(Func{Task{TResult},TNewResult},CancellationToken)"/>:
    /// <list type="bullet">
    /// <item>
    /// The used <see cref="TaskScheduler"/> is always <see cref="TaskScheduler.Default"/>, which mirrors the
    /// functionality of <see cref="Task{TResult}.ConfigureAwait"/>(false)
    /// </item>
    /// <item>
    /// The task's continuation executes synchronously if possible, and the child <see cref="Task{TResult}"/> is not
    /// tied to the parent, mirroring the behavior of <c>await</c>.
    /// </item>
    /// </list>
    /// </remarks>
    public static Task<TNewResult> ContinueWithRelativelySafely<TResult, TNewResult>(
        this Task<TResult> task,
        Func<Task<TResult>, TNewResult> continuationFunction,
        CancellationToken cancellationToken = default
    )
    {
        return task.ContinueWith(
            continuationFunction,
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default
        );
    }
    
    /// <summary>
    /// Creates a continuation that executes when the target <see cref="Task"/> completes.
    /// </summary>
    /// <param name="task">The task to apply the continuation to.</param>
    /// <param name="continuationAction">
    /// An action to run when the <see cref="Task"/> completes. When run, the delegate will be
    /// passed the completed task as an argument.
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that will be assigned to the new task.</param>
    /// <returns>A new continuation <see cref="Task"/>.</returns>
    /// <remarks>
    /// This method uses sane defaults compared to the original <see cref="Task.ContinueWith(Action{Task},CancellationToken)"/>:
    /// <list type="bullet">
    /// <item>
    /// The used <see cref="TaskScheduler"/> is always <see cref="TaskScheduler.Default"/>, which mirrors the
    /// functionality of <see cref="Task.ConfigureAwait"/>(false)
    /// </item>
    /// <item>
    /// The task's continuation executes synchronously if possible, and the child <see cref="Task"/> is not
    /// tied to the parent, mirroring the behavior of <c>await</c>.
    /// </item>
    /// </list>
    /// </remarks>
    public static Task ContinueWithRelativelySafely(
        this Task task,
        Action<Task> continuationAction,
        CancellationToken cancellationToken = default
    )
    {
        return task.ContinueWith(
            continuationAction,
            cancellationToken,
            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default
        );
    }
}