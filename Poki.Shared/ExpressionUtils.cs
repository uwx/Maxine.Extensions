using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using Poki.Shared.HSNXT.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Poki.Shared;

public delegate object ObjectConstructor(params object?[] args);

public delegate TBaseType ParameterlessObjectConstructor<TBaseType>();

public delegate object DIMethodCaller(object instance, IServiceProvider serviceProvider, params object?[]? arguments);

public static class ExpressionUtils
{
    public static ObjectConstructor CreateFactory(Type type, ConstructorInfo constructor)
    {
        var arguments = typeof(object[]).Parameter("arguments");

        var parameters = constructor.GetParameters()
            .Select((p, i) => arguments.ArrayIndex(i).Convert(p.ParameterType));

        /*
         * (object[] arguments) => new constructor(
         *     (Param0Type) arguments[0],
         *     (Param1Type) arguments[1],
         *     ...
         * );
         */
        return Lambda<ObjectConstructor>(
            constructor.New(parameters), // TODO does it need a cast to Object?
            arguments
        ).CompileFast();
    }

    public static ParameterlessObjectConstructor<object> CreateParameterlessFactory(Type type)
        => CreateParameterlessFactory<object>(type); 

    public static ParameterlessObjectConstructor<TBaseType> CreateParameterlessFactory<TBaseType>(Type type)
    {
        /*
         * () => new constructor(
         *     (Param0Type) arguments[0],
         *     (Param1Type) arguments[1],
         *     ...
         * );
         */
        return Lambda<ParameterlessObjectConstructor<TBaseType>>(
            type.New()
        ).CompileFast();
    }

    public static Func<object, object?> CreatePropertyGetter(Type type, PropertyInfo property)
    {
        return CreatePropertyGetter<object, object?>(type, property);
    }

    public static Action<object, object?> CreatePropertySetter(Type type, PropertyInfo property)
    {
        return CreatePropertySetter<object, object?>(type, property);
    }
    
    public static Func<TInstanceBase, object?> CreatePropertyGetter<TInstanceBase>(Type type, PropertyInfo property)
    {
        return CreatePropertyGetter<TInstanceBase, object?>(type, property);
    }
    
    public static Action<TInstanceBase, object?> CreatePropertySetter<TInstanceBase>(Type type, PropertyInfo property)
    {
        return CreatePropertySetter<TInstanceBase, object?>(type, property);
    }
    
    public static Func<TInstanceBase, TResultBase> CreatePropertyGetter<TInstanceBase, TResultBase>(Type type, PropertyInfo property)
    {
        var instance = typeof(TInstanceBase).Parameter("instance");

        /*
         * (instance) => (object) ((InstanceType)instance).property
         */
        return Lambda<Func<TInstanceBase, TResultBase>>(
            instance.Convert(type).Property(property).Convert(typeof(TResultBase)),
            instance
        ).CompileFast();
    }
    
    public static Action<TInstanceBase, TResultBase> CreatePropertySetter<TInstanceBase, TResultBase>(Type type, PropertyInfo property)
    {
        var instance = typeof(TInstanceBase).Parameter("instance");
        var value = typeof(TResultBase).Parameter("value");

        return Lambda<Action<TInstanceBase, TResultBase>>(
            instance.Convert(type).Property(property) // ((InstanceType)instance).property
                .Assign(value.Convert(property.PropertyType)), // ... = (PropertyType) value
            instance, value
        ).CompileFast(); // (instance, value) => ...
    }

    public static MethodCallExpression CreateUnorderedMethodCallExpression(
        Type instanceType,
        MethodInfo method,
        Type[] argumentTypes,
        out ParameterExpression instanceParam,
        out ParameterExpression providedParameters
    )
    {
        instanceParam = typeof(object).Parameter("instance");
        providedParameters = typeof(object?[]).Parameter("providedParameters");

        var callArguments = new Expression[method.GetParameters().Length];

        var i = 0;
        foreach (var param in method.GetParameters())
        {
            Expression? expr = null;

            for (var j = 0; j < argumentTypes.Length; j++)
            {
                if (param.ParameterType.IsAssignableFrom(argumentTypes[j]))
                {
                    // Get provided parameter

                    expr = providedParameters.ArrayIndex(j);
                }
            }

            // Coalesce with default value if necessary
            if (param.HasDefaultValue)
            {
                var defaultValueExpression = param.DefaultValue.Constant();
                expr = expr.Coalesce(defaultValueExpression);
            }
            
            // Cast to parameter type
            expr = expr.Convert(param.ParameterType);

            callArguments[i++] = expr;
        }

        // Cast the instance param to the actual type
        var controllerInstance = instanceParam.Convert(instanceType);
        
        // Create the method call Instance.Method(arguments)
        return controllerInstance.Call(method, callArguments);
    }

    public static MethodCallExpression CreateDIMethodCallExpression(
        Type instanceType,
        MethodInfo method,
        Type[] argumentTypes,
        out ParameterExpression instanceParam,
        out ParameterExpression servicesParam,
        out ParameterExpression providedParameters
    )
    {
        instanceParam = typeof(object).Parameter("instance");
        servicesParam = typeof(IServiceProvider).Parameter("services");
        providedParameters = typeof(object?[]).Parameter("providedParameters");
        
        var callArguments = new Expression[method.GetParameters().Length];

        var i = 0;
        foreach (var param in method.GetParameters())
        {
            Expression? expr = null;

            for (var j = 0; j < argumentTypes.Length; j++)
            {
                if (param.ParameterType.IsAssignableFrom(argumentTypes[j]))
                {
                    // Get provided parameter

                    expr = providedParameters.ArrayIndex(j);
                }
            }

            expr ??= GetServiceInfo.Call(
                servicesParam, // sp
                param.ParameterType.Constant(typeof(Type)), // type
                $"{method.DeclaringType}:{method.Name}".Constant(), // requiredBy
                param.HasDefaultValue.Constant() // isDefaultParameterRequired
            );
            
            // Coalesce with default value if necessary
            if (param.HasDefaultValue)
            {
                var defaultValueExpression = param.DefaultValue.Constant();
                expr = expr.Coalesce(defaultValueExpression);
            }
            
            // Cast to parameter type
            expr = expr.Convert(param.ParameterType);

            callArguments[i++] = expr;
        }

        // Cast the instance param to the actual type
        var controllerInstance = instanceParam.Convert(instanceType);
        
        // Create the method call Instance.Method(arguments)
        return controllerInstance.Call(method, callArguments);
    }

    public static DIMethodCaller CreateDIMethodCaller(
        Type instanceType,
        MethodInfo method,
        Type[] argumentTypes
    )
    {
        var expr = CreateDIMethodCallExpression(
            instanceType,
            method,
            argumentTypes,
            out var instanceParam,
            out var servicesParam,
            out var providedParams
        );

        return Lambda<DIMethodCaller>(expr, instanceParam, servicesParam, providedParams).CompileFast();
    }

    private static readonly MethodInfo GetServiceInfo
        = typeof(ExpressionUtils).GetMethod(nameof(GetService), BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Did not find {nameof(GetService)} method in current type");

    // From ActivatorUtilities
    private static object? GetService(IServiceProvider sp, Type type, string requiredBy, bool isDefaultParameterRequired)
    {
        var service = sp.GetService(type);
        if (service == null && !isDefaultParameterRequired)
        {
            var message = $"Unable to resolve service for type '{type}' while attempting to call '{requiredBy}'.";
            throw new InvalidOperationException(message);
        }
        return service;
    }
    
    /// <summary>
    /// Coerces the static result type of an expression to a <see cref="ValueTask"/>.
    ///
    /// <list type="bullet">
    /// <item>If the type is <see cref="ValueTask"/>, returns the expression as-is</item>
    /// <item>If the type is <see cref="Task"/>, returns an expression wrappiung the Task in a <see cref="ValueTask"/></item>
    /// <item>If the type is <c>void</c>, returns <see cref="ValueTask.CompletedTask"/></item>
    /// <item>Otherwise, throws <see cref="InvalidOperationException"/></item>
    /// </list>
    /// </summary>
    /// <param name="expression">The input expression</param>
    /// <param name="expressionType">The type of the expression; defaults to <see cref="Expression.Type"/></param>
    /// <returns>The new expression</returns>
    /// <exception cref="InvalidOperationException">If the type of <paramref name="expression"/> is not wrappable</exception>
    public static Expression CoerceToValueTask(Expression expression, Type? expressionType = null)
    {
        expressionType ??= expression.Type;
        
        Expression invokerExpr;
        if (expressionType == typeof(void))
        {
            // Run expression and return ValueTask.CompletedTask

            invokerExpr = Block(
                expression,
                ValueTask.CompletedTask.Constant()
            );
        }
        else if (expressionType == typeof(ValueTask))
        {
            // Is already a ValueTask, return as-is

            invokerExpr = expression;
        }
        else if (expressionType == typeof(Task))
        {
            // new ValueTask(task)

            invokerExpr = NewValueTaskInfo.New(expression);
        }
        else
        {
            throw new InvalidOperationException($"{nameof(CoerceToValueTask)} expression must be void, {nameof(Task)} or {nameof(ValueTask)}");
        }

        return invokerExpr;
    }
    
    private static readonly ConstructorInfo NewValueTaskInfo
        = typeof(ValueTask).GetConstructor(new[] { typeof(Task) })
          ?? throw new InvalidOperationException($"Did not find {nameof(ValueTask)} single-argument constructor");
}
