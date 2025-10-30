using System.Linq.Expressions;

namespace Maxine.Extensions.Expressions;

public static partial class FluentExtensions
{
    public static BinaryExpression ArrayIndex(this Expression array, int index)
        => Expression.ArrayIndex(array, index.Constant());
}