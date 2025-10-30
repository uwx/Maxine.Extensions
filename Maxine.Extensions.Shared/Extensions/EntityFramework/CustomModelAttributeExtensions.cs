using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maxine.Extensions.Shared.EntityFramework;

[Flags]
public enum DatabaseType
{
    PostgreSql = 1,
    Sqlite = 2,
    InMemory = 4
}

/// <summary>
/// Interface to attach to a class extending <see cref="DbContext"/> that exposes useful properties for custom model
/// attributes.
/// </summary>
public interface ICustomDatabaseContext
{
    DatabaseType Type { get; }
}

/// <summary>
/// <para>A base class for attributes that can be applied to a property in an entity of an Entity Framework model.</para>
///
/// <para>Upon calling <see cref="CustomModelAttributeExtensions.BuildCustomAttributes"/> from an implementation of
/// <see cref="DbContext.OnModelCreating"/>, any properties in all entity in the model, which contain an attribute
/// deriving from this type, will cause a call to <see cref="OnModelCreating"/> with the relevant property information
/// and a context parameter containing the <see cref="EntityTypeBuilder"/> which can be used to configure the property
/// or the model it's contained in.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class EfCustomPropertyAttribute : Attribute
{
    /// <summary>
    /// Represents data shared to a custom model attribute during a call to
    /// <see cref="EfCustomPropertyAttribute.OnModelCreating"/>.
    /// </summary>
    /// <param name="Model">The builder for the model being created</param>
    /// <param name="Entity">The builder for the entity in the model corresponding to the CLR type of the type containing
    /// the property with the custom model attribute</param>
    /// <param name="EntityType">The entity type in the <see cref="IMutableModel"/> for the model being created</param>
    /// <param name="DatabaseContext">A handle to the class extending <see cref="DbContext"/> with useful properties</param>
    /// <param name="Services">The service provider the database context provided
    /// <see cref="CustomModelAttributeExtensions.BuildCustomAttributes"/> with</param>
    public readonly record struct Context(
        ModelBuilder Model,
        EntityTypeBuilder Entity,
        IMutableEntityType EntityType,
        ICustomDatabaseContext DatabaseContext,
        IServiceProvider Services
    )
    {
        public Type ClrType => EntityType.ClrType;
    }

    /// <summary>
    /// Represents property information for a call to <see cref="EfCustomPropertyAttribute.OnModelCreating"/>, of the
    /// property which the attribute was applied on.
    /// </summary>
    /// <param name="Property">The reflection information of the property</param>
    public readonly record struct PropertyContext(
        Context _backingContext,
        PropertyInfo Property
    )
    {
        private readonly Context _backingContext = _backingContext;
        
        /// <inheritdoc cref="Context.Model"/>
        public ModelBuilder Model => _backingContext.Model;
        /// <inheritdoc cref="Context.Entity"/>
        public EntityTypeBuilder Entity => _backingContext.Entity;
        /// <inheritdoc cref="Context.EntityType"/>
        public IMutableEntityType EntityType => _backingContext.EntityType;
        /// <inheritdoc cref="Context.DatabaseContext"/>
        public ICustomDatabaseContext DatabaseContext => _backingContext.DatabaseContext;
        /// <inheritdoc cref="Context.Services"/>
        public IServiceProvider Services => _backingContext.Services;
    }

    protected static readonly DatabaseType AllDatabaseTypes = Enum.GetValues<DatabaseType>().Aggregate(static (a, b) => a | b);

    /// <summary>
    /// Whether or not more than one property is allowed to have this attribute in their containing type.
    /// </summary>
    public virtual bool CanHaveMultiple => true;

    /// <summary>
    /// Restricts this attribute to have an effect only when running one of the set database types
    /// </summary>
    public DatabaseType OnlyForDatabases { get; set; } = AllDatabaseTypes;
    
    public abstract void OnModelCreating(PropertyContext ctx);
}

/// <summary>
/// The wrapper exception type for any errors encountered during a call to
/// <see cref="CustomModelAttributeExtensions.BuildCustomAttributes"/>.
/// </summary>
public sealed class CustomModelAttributeException : Exception
{
    /// <inheritdoc/>
    public CustomModelAttributeException(string message) : base(message)
    {
    }
    
    /// <inheritdoc/>
    public CustomModelAttributeException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    /// <inheritdoc/>
    public CustomModelAttributeException(Exception innerException) : base(null, innerException)
    {
    }
}

/// <summary>
/// Contains extension methods for activating the functionality behind custom model attributes for Entity Framework
/// entity properties.
/// </summary>
public static class CustomModelAttributeExtensions
{
    /// <summary>
    /// <para>
    /// This method should be called in a class inheriting <see cref="DbContext"/>'s implementation of
    /// <see cref="DbContext.OnModelCreating"/>, preferably after other operations have taken place, in order to walk
    /// through the entities defined in the <see cref="ModelBuilder"/>, look for properties with attributes that inherit
    /// from <see cref="EfCustomPropertyAttribute"/>, and call their respective
    /// <see cref="EfCustomPropertyAttribute.OnModelCreating"/> methods to operate on the ModelBuilder.
    /// </para>
    /// <para>
    /// This method also wires all entity properties with <see cref="DefaultValueAttribute"/> using
    /// <see cref="RelationalPropertyBuilderExtensions.HasDefaultValue(PropertyBuilder, object)"/>.
    /// </para>
    /// </summary>
    /// <param name="model"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="dbContext"></param>
    /// <returns>The existing <see cref="ModelBuilder"/>.</returns>
    /// <exception cref="CustomModelAttributeException">
    /// If an exception is thrown from within an attribute's <see cref="EfCustomPropertyAttribute.OnModelCreating"/>
    /// method.
    /// </exception>
    [PublicAPI]
    public static ModelBuilder BuildCustomAttributes(this ModelBuilder model, IServiceProvider serviceProvider, ICustomDatabaseContext dbContext)
    {
        HashSet<Type>? uniqueAttributeTally = null;
        
        foreach (var entityType in model.Model.GetEntityTypes())
        {
            uniqueAttributeTally?.Clear();

            EfCustomPropertyAttribute.Context? backingContext = null;

            EfCustomPropertyAttribute.Context GetContext()
            {
                return backingContext ??= new EfCustomPropertyAttribute.Context
                {
                    Model = model,
                    Entity = model.Entity(entityType.ClrType),
                    EntityType = entityType,
                    DatabaseContext = dbContext,
                    Services = serviceProvider
                };
            }

            foreach (var prop in entityType.ClrType.GetProperties())
            {
                var attrs = Attribute.GetCustomAttributes(prop);

                // note to self: we don't restart the tally for every new property, that ruins the entire point, we
                // have AttributeUsage.AllowMultiple for that

                foreach (var attr in attrs)
                {
                    if (attr is DefaultValueAttribute defaultValueAttr)
                    {
                        var ctx = GetContext();
                        
                        ctx.Entity
                            .Property(prop.Name)
                            .HasDefaultValue(defaultValueAttr.Value);

                        continue;
                    }
                    
                    if (attr is not EfCustomPropertyAttribute processorAttr)
                        continue;

                    if (!processorAttr.OnlyForDatabases.HasFlag(dbContext.Type))
                        continue;
                    
                    // lazily initialize the context (so we don't do model.Entity when we don't need to)

                    var definition = new EfCustomPropertyAttribute.PropertyContext(GetContext(), prop);

                    if (!processorAttr.CanHaveMultiple)
                    {
                        // lazily initialize the tally
                        uniqueAttributeTally ??= [];
                        
                        if (!uniqueAttributeTally.Add(processorAttr.GetType()))
                        {
                            throw new CustomModelAttributeException(
                                $"The model {prop} contains multiple {processorAttr.GetType()} attributes");
                        }
                    }

                    try
                    {
                        processorAttr.OnModelCreating(definition);
                    }
                    catch (Exception ex) when (ex is not CustomModelAttributeException)
                    {
                        throw new CustomModelAttributeException("An aggregate exception occurred", ex);
                    }
                }
            }
        }

        // allow chaining
        return model;
    }
}