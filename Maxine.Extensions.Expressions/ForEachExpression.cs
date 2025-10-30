//
// ForEachExpression.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2010 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable disable

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Maxine.Extensions.Expressions;

public class ForEachExpression : CustomExpression
{
    internal ForEachExpression(ParameterExpression variable, Expression enumerable, Expression body,
        LabelTarget breakTarget, LabelTarget continueTarget)
    {
        Variable = variable;
        Enumerable = enumerable;
        Body = body;
        BreakTarget = breakTarget;
        ContinueTarget = continueTarget;
    }

    public new ParameterExpression Variable { get; }

    public Expression Enumerable { get; }

    public Expression Body { get; }

    public LabelTarget BreakTarget { get; }

    public LabelTarget ContinueTarget { get; }

    public override Type Type => BreakTarget != null ? BreakTarget.Type : typeof(void);

    public override CustomExpressionType CustomNodeType => CustomExpressionType.ForEachExpression;

    public ForEachExpression Update(ParameterExpression variable, Expression enumerable, Expression body,
        LabelTarget breakTarget, LabelTarget continueTarget)
    {
        if (Variable == variable && Enumerable == enumerable && Body == body && BreakTarget == breakTarget &&
            ContinueTarget == continueTarget)
            return this;

        return ForEach(variable, enumerable, body, breakTarget, continueTarget);
    }

    public override Expression Reduce()
    {
        // Avoid allocating an unnecessary enumerator for arrays.
        return Enumerable.Type.IsArray ? ReduceForArray() : ReduceForEnumerable();
    }

    private Expression ReduceForArray()
    {
        var innerLoopBreak = Label("inner_loop_break");
        var innerLoopContinue = Label("inner_loop_continue");

        var @continue = ContinueTarget ?? Label("continue");
        var @break = BreakTarget ?? Label("break");

        var index = Variable(typeof(int), "i");

        return Block(
            [index, Variable],
            index.Assign(Constant(0)),
            Loop(
                Block(
                    IfThen(
                        IsFalse(
                            LessThan(
                                index,
                                ArrayLength(Enumerable))),
                        Break(innerLoopBreak)),
                    Variable.Assign(
                        ArrayIndex(
                            Enumerable,
                            index).Convert(Variable.Type)),
                    Body,
                    Label(@continue),
                    PreIncrementAssign(index)),
                innerLoopBreak,
                innerLoopContinue),
            Label(@break));
    }

    private Expression ReduceForEnumerable()
    {
        ResolveEnumerationMembers(out var getEnumerator, out var moveNext, out var getCurrent);

        var enumeratorType = getEnumerator.ReturnType;

        var enumerator = Variable(enumeratorType);

        var innerLoopContinue = Label("inner_loop_continue");
        var innerLoopBreak = Label("inner_loop_break");
        var @continue = ContinueTarget ?? Label("continue");
        var @break = BreakTarget ?? Label("break");

        Expression variableInitializer;

        if (Variable.Type.IsAssignableFrom(getCurrent.ReturnType))
            variableInitializer = enumerator.Property(getCurrent);
        else
            variableInitializer = enumerator.Property(getCurrent).Convert(Variable.Type);

        Expression loop = Block(
            [Variable],
            Goto(@continue),
            Loop(
                Block(
                    Variable.Assign(variableInitializer),
                    Body,
                    Label(@continue),
                    Condition(
                        Call(enumerator, moveNext),
                        Goto(innerLoopContinue),
                        Goto(innerLoopBreak))),
                innerLoopBreak,
                innerLoopContinue),
            Label(@break));

        var dispose = CreateDisposeOperation(enumeratorType, enumerator);

        return Block(
            [enumerator],
            enumerator.Assign(Call(Enumerable, getEnumerator)),
            dispose != null
                ? TryFinally(loop, dispose)
                : loop);
    }

    private void ResolveEnumerationMembers(
        out MethodInfo getEnumerator,
        out MethodInfo moveNext,
        out MethodInfo getCurrent)
    {
        Type enumerableType;
        Type enumeratorType;

        if (TryGetGenericEnumerableArgument(out var itemType))
        {
            enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);
            enumeratorType = typeof(IEnumerator<>).MakeGenericType(itemType);
        }
        else
        {
            enumerableType = typeof(IEnumerable);
            enumeratorType = typeof(IEnumerator);
        }

        moveNext = typeof(IEnumerator).GetMethod("MoveNext");
        getCurrent = enumeratorType.GetProperty("Current").GetGetMethod();
        getEnumerator = Enumerable.Type.GetMethod("GetEnumerator", BindingFlags.Public | BindingFlags.Instance);

        //
        // We want to avoid unnecessarily boxing an enumerator if it's a value type.  Look
        // for a GetEnumerator() method that conforms to the rules of the C# 'foreach'
        // pattern.  If we don't find one, fall back to IEnumerable[<T>].GetEnumerator().
        //

        if (getEnumerator == null || !enumeratorType.IsAssignableFrom(getEnumerator.ReturnType))
            getEnumerator = enumerableType.GetMethod("GetEnumerator");
    }

    private static Expression CreateDisposeOperation(Type enumeratorType, ParameterExpression enumerator)
    {
        var dispose = typeof(IDisposable).GetMethod("Dispose");

        if (typeof(IDisposable).IsAssignableFrom(enumeratorType)) return Call(enumerator, dispose);

        if (enumeratorType.IsValueType) return null;

        //
        // We don't know whether the enumerator implements IDisposable or not.  Emit a
        // runtime check.
        //

        var disposable = Variable(typeof(IDisposable));

        return Block(
            [disposable],
            disposable.Assign(enumerator.TypeAs(typeof(IDisposable))),
            disposable.ReferenceNotEqual(Constant(null)).IfThen(
                Call(
                    disposable,
                    "Dispose",
                    [])));
    }

    private bool TryGetGenericEnumerableArgument(out Type argument)
    {
        argument = null;

        foreach (var iface in Enumerable.Type.GetInterfaces())
        {
            if (!iface.IsGenericType)
                continue;

            var definition = iface.GetGenericTypeDefinition();
            if (definition != typeof(IEnumerable<>))
                continue;

            argument = iface.GetGenericArguments()[0];
            if (Variable.Type.IsAssignableFrom(argument))
                return true;
        }

        return false;
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor) =>
        Update(
            (ParameterExpression) visitor.Visit(Variable),
            visitor.Visit(Enumerable),
            visitor.Visit(Body),
            BreakTarget,
            ContinueTarget);

    public override Expression Accept(CustomExpressionVisitor visitor) => visitor.VisitForEachExpression(this);
}

public abstract partial class CustomExpression
{
    public static ForEachExpression ForEach(ParameterExpression variable, Expression enumerable, Expression body) =>
        ForEach(variable, enumerable, body, null);

    public static ForEachExpression ForEach(ParameterExpression variable, Expression enumerable, Expression body,
        LabelTarget breakTarget) => ForEach(variable, enumerable, body, breakTarget, null);

    public static ForEachExpression ForEach(ParameterExpression variable, Expression enumerable, Expression body,
        LabelTarget breakTarget, LabelTarget continueTarget)
    {
        if (variable == null)
            throw new ArgumentNullException(nameof(variable));
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        if (!typeof(IEnumerable).IsAssignableFrom(enumerable.Type))
            throw new ArgumentException("The enumerable must implement at least IEnumerable", nameof(enumerable));

        if (continueTarget != null && continueTarget.Type != typeof(void))
            throw new ArgumentException("Continue label target must be void", nameof(continueTarget));

        return new ForEachExpression(variable, enumerable, body, breakTarget, continueTarget);
    }
}