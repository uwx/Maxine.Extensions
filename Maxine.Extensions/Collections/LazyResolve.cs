using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Maxine.Extensions.Collections;

[DebuggerTypeProxy(typeof(LazyResolveDebugView<>))]
[DebuggerDisplay("Value={Value}")]
public class LazyResolve<T>(IServiceProvider services)
    where T : notnull
{
    private IServiceProvider? _services = services;
    
    internal T? ValueOrDefault;
    public T Value => ValueOrDefault ?? Get();

    private T Get()
    {
        var service = ValueOrDefault = _services!.GetRequiredService<T>();
        _services = null;
        return service;
    }
}

file class LazyResolveDebugView<T>(LazyResolve<T> lazy)
    where T : notnull
{
    public T? Value => lazy.ValueOrDefault;
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