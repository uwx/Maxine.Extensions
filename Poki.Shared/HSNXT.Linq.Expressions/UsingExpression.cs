//
// UsingExpression.cs
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

using System.Linq.Expressions;

namespace Poki.Shared.HSNXT.Linq.Expressions;

public class UsingExpression : CustomExpression
{
    internal UsingExpression(ParameterExpression variable, Expression disposable, Expression body)
    {
        Variable = variable;
        Disposable = disposable;
        Body = body;
    }

    public new ParameterExpression Variable { get; }

    public Expression Disposable { get; }

    public Expression Body { get; }

    public override Type Type => Body.Type;

    public override CustomExpressionType CustomNodeType => CustomExpressionType.UsingExpression;

    public UsingExpression Update(ParameterExpression variable, Expression disposable, Expression body)
    {
        if (Variable == variable && Disposable == disposable && Body == body)
            return this;

        return Using(variable, disposable, body);
    }

    public override Expression Reduce()
    {
        var endFinally = Label("end_finally");

        return Block(
            new[] {Variable},
            Variable.Assign(Disposable),
            TryFinally(
                Body,
                Block(
                    Variable.NotEqual(Constant(null)).Condition(
                        Block(
                            Call(
                                Variable.Convert(typeof(IDisposable)),
                                typeof(IDisposable).GetMethod("Dispose")),
                            Goto(endFinally)),
                        Goto(endFinally)),
                    Label(endFinally))));
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor) =>
        Update(
            (ParameterExpression) visitor.Visit(Variable),
            visitor.Visit(Disposable),
            visitor.Visit(Body));

    public override Expression Accept(CustomExpressionVisitor visitor) => visitor.VisitUsingExpression(this);
}

public abstract partial class CustomExpression
{
    public static UsingExpression Using(Expression disposable, Expression body) => Using(null, disposable, body);

    public static UsingExpression Using(ParameterExpression variable, Expression disposable, Expression body)
    {
        if (disposable == null)
            throw new ArgumentNullException(nameof(disposable));
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        if (!typeof(IDisposable).IsAssignableFrom(disposable.Type))
            throw new ArgumentException("The disposable must implement IDisposable", nameof(disposable));

        if (variable == null)
            variable = Parameter(disposable.Type);

        return new UsingExpression(variable, disposable, body);
    }
}