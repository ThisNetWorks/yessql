using Parlot;
using Parlot.Fluent;
using System;
using System.Threading.Tasks;
using YesSql.Indexes;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser
{
    public abstract class OperatorParserBuilder<T> where T : class
    {
        public abstract TermOption<T> TermOption { get; }
        public abstract Parser<OperatorNode> Parser { get; }
    }

    public class UnaryParserBuilder<T> : OperatorParserBuilder<T> where T : class
    {
        public UnaryParserBuilder(Func<string, IQuery<T>, IQuery<T>> query, bool single = true)
        {
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> valueQuery = (q, val, ctx) => new ValueTask<IQuery<T>>(query(q, val));

            TermOption = new TermOption<T>(new TermQueryOption<T>(valueQuery), single);
        }


        public UnaryParserBuilder(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> query, bool single = true)
        {
            TermOption = new TermOption<T>(new TermQueryOption<T>(query), single);
        }

        public override Parser<OperatorNode> Parser
            => Terms.String()
                .Or(
                    Terms.NonWhiteSpace()
                )
                    .Then<OperatorNode>(static (node) => new UnaryNode(node.ToString()));

        public override TermOption<T> TermOption { get; }
    }
}
