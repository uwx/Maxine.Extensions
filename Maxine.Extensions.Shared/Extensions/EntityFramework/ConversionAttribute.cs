using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace Maxine.Extensions.Shared.EntityFramework;

internal static class FactoryStorage
{
    private static readonly Dictionary<Type, ObjectFactory> Factories = new();

    public static ObjectFactory GetOrCreateFactory(Type type)
    {
        if (Factories.TryGetValue(type, out var factory))
        {
            return factory;
        }

        return Factories[type] = ActivatorUtilities.CreateFactory(type, []);
    }
}

public class ConversionAttribute<TConverter, TComparer> : EfCustomPropertyAttribute
    where TConverter : ValueConverter
    where TComparer : ValueComparer
{
    private readonly ObjectFactory? _converterFactory;
    private readonly ObjectFactory? _comparerFactory;

    public ConversionAttribute()
    {
        _converterFactory = FactoryStorage.GetOrCreateFactory(typeof(TConverter));
        _comparerFactory = FactoryStorage.GetOrCreateFactory(typeof(TComparer));
    }
    
    public override void OnModelCreating(PropertyContext ctx)
    {
        ctx.Entity
            .Property(ctx.Property.Name)
            .HasConversion(
                (ValueConverter) _converterFactory!(ctx.Services, null),
                (ValueComparer?) _comparerFactory?.Invoke(ctx.Services, null)
            );
    }
}

public class ConversionAttribute<TConverter> : EfCustomPropertyAttribute
    where TConverter : ValueConverter
{
    private readonly ObjectFactory? _converterFactory;

    public ConversionAttribute()
    {
        _converterFactory = FactoryStorage.GetOrCreateFactory(typeof(TConverter));
    }
    
    public override void OnModelCreating(PropertyContext ctx)
    {
        ctx.Entity
            .Property(ctx.Property.Name)
            .HasConversion(
                (ValueConverter) _converterFactory!(ctx.Services, null)
            );
    }
}

public class ComparerAttribute<TComparer> : EfCustomPropertyAttribute
    where TComparer : ValueComparer
{
    private readonly ObjectFactory? _comparerFactory;

    public ComparerAttribute()
    {
        _comparerFactory = FactoryStorage.GetOrCreateFactory(typeof(TComparer));
    }
    
    public override void OnModelCreating(PropertyContext ctx)
    {
        ctx.Entity
            .Property(ctx.Property.Name)
            .HasConversion(
                ctx.Property.PropertyType,
                (ValueComparer?) _comparerFactory?.Invoke(ctx.Services, null)
            );
    }
}
