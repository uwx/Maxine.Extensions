//
// ForExpression.cs
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
// the following tests:
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

public class ForExpression : CustomExpression
{
    internal ForExpression(ParameterExpression variable, Expression initializer, Expression test, Expression step,
        Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
    {
        Variable = variable;
        Initializer = initializer;
        Test = test;
        Step = step;
        Body = body;
        BreakTarget = breakTarget;
        ContinueTarget = continueTarget;
    }

    public new ParameterExpression Variable { get; }

    public Expression Initializer { get; }

    public Expression Test { get; }

    public Expression Step { get; }

    public Expression Body { get; }

    public LabelTarget BreakTarget { get; }

    public LabelTarget ContinueTarget { get; }

    public override Type Type => BreakTarget != null ? BreakTarget.Type : typeof(void);

    public override CustomExpressionType CustomNodeType => CustomExpressionType.ForExpression;

    public ForExpression Update(ParameterExpression variable, Expression initializer, Expression test,
        Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
    {
        if (Variable == variable && Initializer == initializer && Test == test && Step == step && Body == body &&
            BreakTarget == breakTarget && ContinueTarget == continueTarget)
            return this;

        return For(variable, initializer, test, step, body, breakTarget, continueTarget);
    }

    public override Expression Reduce()
    {
        var innerLoopBreak = Label("inner_loop_break");
        var innerLoopContinue = Label("inner_loop_continue");

        var @continue = ContinueTarget ?? Label("continue");
        var @break = BreakTarget ?? Label("break");

        return Block(
            [Variable],
            Variable.Assign(Initializer),
            Loop(
                Block(
                    IfThen(
                        IsFalse(Test),
                        Break(innerLoopBreak)),
                    Body,
                    Label(@continue),
                    Step),
                innerLoopBreak,
                innerLoopContinue),
            Label(@break));
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor) =>
        Update(
            (ParameterExpression) visitor.Visit(Variable),
            visitor.Visit(Initializer),
            visitor.Visit(Test),
            visitor.Visit(Step),
            visitor.Visit(Body),
            BreakTarget,
            ContinueTarget);

    public override Expression Accept(CustomExpressionVisitor visitor) => visitor.VisitForExpression(this);
}

public abstract partial class CustomExpression
{
    public static ForExpression For(ParameterExpression variable, Expression initializer, Expression test,
        Expression step, Expression body) => For(variable, initializer, test, step, body, null);

    public static ForExpression For(ParameterExpression variable, Expression initializer, Expression test,
        Expression step, Expression body, LabelTarget breakTarget) =>
        For(variable, initializer, test, step, body, breakTarget, null);

    public static ForExpression For(ParameterExpression variable, Expression initializer, Expression test,
        Expression step, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
    {
        if (variable == null)
            throw new ArgumentNullException(nameof(variable));
        if (initializer == null)
            throw new ArgumentNullException(nameof(initializer));
        if (test == null)
            throw new ArgumentNullException(nameof(test));
        if (step == null)
            throw new ArgumentNullException(nameof(step));
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        if (!variable.Type.IsAssignableFrom(initializer.Type))
            throw new ArgumentException("Initializer must be assignable to variable", nameof(initializer));

        if (test.Type != typeof(bool))
            throw new ArgumentException("Test must be a boolean expression", nameof(test));

        if (continueTarget != null && continueTarget.Type != typeof(void))
            throw new ArgumentException("Continue label target must be void", nameof(continueTarget));

        return new ForExpression(variable, initializer, test, step, body, breakTarget, continueTarget);
    }
}