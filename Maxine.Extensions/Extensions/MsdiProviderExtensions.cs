using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Maxine.Extensions;

public sealed class Provider<T> : IDisposable, IAsyncDisposable
{
    private T? _instance;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Provide(T instance)
    {
        _instance = instance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T RetrieveOrThrow(string? throwMessage = null)
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        static T Throw(string? message)
            => throw new InvalidOperationException(message ?? $"An instance of {typeof(T)} is not provided for this scope");

        return _instance ?? Throw(throwMessage);
    }
    
    // Handles disposing the service in case it's provided but never retrieved (so the container wouldn't dispose it)

    public void Dispose()
    {
        if (_instance is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_instance is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable.DisposeAsync();
        }

        if (_instance is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        return ValueTask.CompletedTask;
    }
}

public static class MsdiProviderExtensions
{
    public static IServiceCollection AddScopedProvided<TService>(this IServiceCollection services, string? throwMessage = null)
        where TService : class
    {
        services.AddScoped<Provider<TService>>();
        return services.AddScoped(s => s.GetRequiredService<Provider<TService>>().RetrieveOrThrow(throwMessage));
    }

    public static TService Provide<TService>(this IServiceProvider services, TService serviceInstance)
    {
        services.GetRequiredService<Provider<TService>>().Provide(serviceInstance);
        return serviceInstance;
    }

    public static TService Provide<TService>(this AsyncServiceScope scope, TService serviceInstance)
    {
        scope.ServiceProvider.Provide(serviceInstance);
        return serviceInstance;
    }

    public static TService Provide<TService>(this IServiceScope scope, TService serviceInstance)
    {
        scope.ServiceProvider.Provide(serviceInstance);
        return serviceInstance;
    }
}