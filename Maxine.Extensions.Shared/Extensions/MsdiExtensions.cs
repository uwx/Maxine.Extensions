using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Maxine.Extensions.Shared;

public static class MsdiExtensions
{
    /// <summary>
    /// Adds an alias registration for the most recently registered service of this <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="collection">The service collection</param>
    /// <typeparam name="T">The type to alias as</typeparam>
    /// <returns>The service collection</returns>
    public static IServiceCollection AlsoAs<
        [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] T
    >(this IServiceCollection collection)
        where T : class
    {
        var lastRealService = collection.GetLastRealService();
        var lastServiceType = lastRealService.ServiceType;
        collection.Add(new AlsoAsServiceDescriptor(
            typeof(T),
            services => services.GetRequiredService(lastServiceType),
            lastRealService.Lifetime
        ));

        return collection;
    }

    /// <summary>
    /// Registers the most recently registered service of this <see cref="IServiceCollection"/> as
    /// <see cref="IHostedService"/>.
    /// </summary>
    /// <param name="collection">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AlsoAsHostedService(this IServiceCollection collection)
    {
        return collection.AlsoAs<IHostedService>();
    }

    private static ServiceDescriptor GetLastRealService(this IServiceCollection collection)
    {
        var i = 1;
        while (collection[^i] is AlsoAsServiceDescriptor)
        {
            i++;
        }

        var lastRealService = collection[^i];
        return lastRealService;
    }

    private sealed class AlsoAsServiceDescriptor : ServiceDescriptor
    {
        // /// <inheritdoc/>
        // public AlsoAsServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime)
        // {
        // }
        //
        // /// <inheritdoc/>
        // public AlsoAsServiceDescriptor(Type serviceType, object instance) : base(serviceType, instance)
        // {
        // }

        /// <inheritdoc/>
        public AlsoAsServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime) : base(serviceType, factory, lifetime)
        {
        }
    }

    /// <summary>
    /// Adds a service, with singleton lifespan, that can be configured via
    /// <see cref="OptionsServiceCollectionExtensions.Configure{TOptions}(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Action{TOptions})"/>
    /// </summary>
    /// <param name="collection">The service collection</param>
    /// <typeparam name="T">The type of the service</typeparam>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddConfiguredSingleton<[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)] T>(this IServiceCollection collection) where T : class
    {
        collection.AddSingleton<T>(static services => services.GetRequiredService<IOptions<T>>().Value);
        collection.AddSingleton<IOptionsFactory<T>, OptionsFactoryWithParameters<T>>();
        return collection;
    }

    private class OptionsFactoryWithParameters<T> : OptionsFactory<T> where T : class
    {
        private readonly IServiceProvider _services;
        private readonly ObjectFactory _factory;
        
        public OptionsFactoryWithParameters(IEnumerable<IConfigureOptions<T>> setups, IEnumerable<IPostConfigureOptions<T>> postConfigures, IServiceProvider services) : base(setups, postConfigures)
        {
            _services = services;
            _factory = ActivatorUtilities.CreateFactory(typeof(T), []);
        }

        public OptionsFactoryWithParameters(IEnumerable<IConfigureOptions<T>> setups, IEnumerable<IPostConfigureOptions<T>> postConfigures, IEnumerable<IValidateOptions<T>> validations, IServiceProvider services) : base(setups, postConfigures, validations)
        {
            _services = services;
            _factory = ActivatorUtilities.CreateFactory(typeof(T), []);
        }
        
        protected override T CreateInstance(string name)
        {
            return (T)_factory(_services, []);
        }
    }
}
