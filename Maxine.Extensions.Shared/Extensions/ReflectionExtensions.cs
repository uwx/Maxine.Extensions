using System.Reflection;

namespace Maxine.Extensions.Shared;

public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the default constructor for a given type, or <c>null</c> if no public parameterless constructor exists in
    /// the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The default constructor, or <c>null</c> if none exists.</returns>
    public static ConstructorInfo? GetDefaultConstructor(this Type type) => type.GetConstructor([]);
    
    /// <summary>
    /// Reflectively invokes a constructor with no parameters.
    /// </summary>
    /// <param name="ctor">The constructor.</param>
    /// <returns>The created object.</returns>
    public static object Invoke(this ConstructorInfo ctor) => ctor.Invoke(null);

    /// <summary>
    /// Reflectively invokes a method with no parameters.
    /// </summary>
    /// <param name="method">The constructor.</param>
    /// <param name="obj">The instance type, or <c>null</c> for static methods.</param>
    /// <returns>The method's return value.</returns>
    public static object? Invoke(this MethodInfo method, object? obj) => method.Invoke(obj, null);
}