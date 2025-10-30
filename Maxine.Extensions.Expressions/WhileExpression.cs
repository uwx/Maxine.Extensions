//
// WhileExpression.cs
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

namespace Maxine.Extensions.Expressions;

public class WhileExpression : CustomExpression
{
    internal WhileExpression(Expression test, Expression body, LabelTarget breakTarget, LabelTarget continueTarget)
    {
        Test = test;
        Body = body;
        BreakTarget = breakTarget;
        ContinueTarget = continueTarget;
    }

    public Expression Test { get; }

    public Expression Body { get; }

    public LabelTarget BreakTarget { get; }

    public LabelTarget ContinueTarget { get; }

    public override Type Type => BreakTarget != null ? BreakTarget.Type : typeof(void);

    public override CustomExpressionType CustomNodeType => CustomExpressionType.WhileExpression;

    public WhileExpression Update(Expression test, Expression body, LabelTarget breakTarget,
        LabelTarget continueTarget)
    {
        if (Test == test && Body == body && BreakTarget == breakTarget && ContinueTarget == continueTarget)
            return this;

        return While(test, body, breakTarget, continueTarget);
    }

    public override Expression Reduce()
    {
        var innerLoopBreak = Label("inner_loop_break");
        var innerLoopContinue = Label("inner_loop_continue");

        var @continue = ContinueTarget ?? Label("continue");
        var @break = BreakTarget ?? Label("break");

        return Block(
            Loop(
                Block(
                    Label(@continue),
                    Test.Condition(
                        Block(
                            Body,
                            Goto(innerLoopContinue)),
                        Goto(innerLoopBreak))),
                innerLoopBreak,
                innerLoopContinue),
            Label(@break));
    }

    protected override Expression VisitChildren(ExpressionVisitor visitor) =>
        Update(
            visitor.Visit(Test),
            visitor.Visit(Body),
            ContinueTarget,
            BreakTarget);

    public override Expression Accept(CustomExpressionVisitor visitor) => visitor.VisitWhileExpression(this);
}

public abstract partial class CustomExpression
{
    public static WhileExpression While(Expression test, Expression body) => While(test, body, null);

    public static WhileExpression While(Expression test, Expression body, LabelTarget breakTarget) =>
        While(test, body, breakTarget, null);

    public static WhileExpression While(Expression test, Expression body, LabelTarget breakTarget,
        LabelTarget continueTarget)
    {
        if (test == null)
            throw new ArgumentNullException(nameof(test));
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        if (test.Type != typeof(bool))
            throw new ArgumentException("Test must be a boolean expression", nameof(test));

        if (continueTarget != null && continueTarget.Type != typeof(void))
            throw new ArgumentException("Continue label target must be void", nameof(continueTarget));

        return new WhileExpression(test, body, breakTarget, continueTarget);
    }
}