using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Maxine.Extensions.Shared;

[DebuggerTypeProxy(typeof(LazyResolveDebugView<>))]
[DebuggerDisplay("Value={Value}")]
public class LazyResolve<T> where T : notnull
{
    private IServiceProvider? _services;
    
    internal T? ValueOrDefault;
    public T Value => ValueOrDefault ?? Get();

    private T Get()
    {
        var service = ValueOrDefault = _services!.GetRequiredService<T>();
        _services = null;
        return service;
    }

    public LazyResolve(IServiceProvider services)
    {
        _services = services;
    }
}

file class LazyResolveDebugView<T> where T : notnull
{
    private readonly LazyResolve<T> _lazy;

    public LazyResolveDebugView(LazyResolve<T> lazy)
    {
        _lazy = lazy;
    }

    public T? Value => _lazy.ValueOrDefault;
}

// [DebuggerTypeProxy(typeof(LazyResolveDebugView))]
// [DebuggerDisplay("Value={Value}")]
// public class LazyResolve
// {
//     private readonly IServiceProvider _services;
//     private readonly Type _type;
//     
//     internal object? ValueOrDefault;
//     public object Value => ValueOrDefault ??= _services.GetRequiredService(_type);
//
//     public LazyResolve(IServiceProvider services, Type type)
//     {
//         _services = services;
//         _type = type;
//     }
// }
//
// file class LazyResolveDebugView
// {
//     private readonly LazyResolve _lazy;
//
//     public LazyResolveDebugView(LazyResolve lazy)
//     {
//         _lazy = lazy;
//     }
//
//     public object? Value => _lazy.ValueOrDefault;
// }