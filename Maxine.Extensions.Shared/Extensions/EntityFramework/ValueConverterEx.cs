using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Maxine.Extensions.Shared.EntityFramework;

/// <summary>
/// A <see cref="ValueConverter{TModel,TProvider}"/> implementation that allows for capturing <c>this</c> within the
/// converter expressions.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TProvider"></typeparam>
public abstract class ValueConverterEx<TModel, TProvider> : ValueConverter<TModel, TProvider>
{
    private static readonly Expression<Func<TModel, TProvider>> DefaultConvertToProviderExpression = a => default!;
    private static readonly Expression<Func<TProvider, TModel>> DefaultConvertFromProviderExpression = a => default!;

    /// <summary>An expression to convert objects when writing data to the store.</summary>
    public required Expression<Func<TModel, TProvider>> ConvertToProviderImpl { get; init; }

    /// <summary>An expression to convert objects when reading data from the store.</summary>
    public required Expression<Func<TProvider, TModel>> ConvertFromProviderImpl { get; init; }
    
    public sealed override Expression<Func<TModel, TProvider>> ConvertToProviderExpression => ConvertToProviderImpl;

    public sealed override Expression<Func<TProvider, TModel>> ConvertFromProviderExpression => ConvertFromProviderImpl;

    /// <summary>Initializes a new instance of the <see cref="ValueConverterEx{TModel,TProvider}"/> class.</summary>
    /// <param name="mappingHints">
    /// Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
    /// facets for the converted data.
    /// </param>
    protected ValueConverterEx(ConverterMappingHints? mappingHints = null)
        : base(DefaultConvertToProviderExpression, DefaultConvertFromProviderExpression, mappingHints)
    {
    }
}
