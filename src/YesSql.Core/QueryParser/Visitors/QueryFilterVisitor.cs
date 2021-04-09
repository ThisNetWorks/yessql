using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser.Visitors
{
    public class QueryFilterVisitor<T> : IFilterVisitor<QueryExecutionContext<T>, Func<IQuery<T>, ValueTask<IQuery<T>>>> where T : class
    {
        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(TermOperationNode node, QueryExecutionContext<T> argument)
            => node.Operation.Accept(this, argument);

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(AndTermNode node, QueryExecutionContext<T> argument)
        {            
            var predicates = new List<Func<IQuery<T>, Task<IQuery<T>>>>();
            foreach (var child in node.Children)
            {
                // Func<IQuery<T>, Task<IQuery<T>>> c = (q) => child.Operation.BuildAsync(context)(q).AsTask();

                Func<IQuery<T>, Task<IQuery<T>>> predicate = (q) => argument.Query.AllAsync(
                    (q) => child.Operation.Accept(this, argument)(q).AsTask()
                );
                predicates.Add(predicate);

            }

            Func<IQuery<T>, Task<IQuery<T>>> result = (Func<IQuery<T>, Task<IQuery<T>>>)Delegate.Combine(predicates.ToArray());

            return xyz => new ValueTask<IQuery<T>>(result(argument.Query));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(UnaryNode node, QueryExecutionContext<T> argument)
        {            
            var currentQuery = argument.CurrentTermOption.Query.MatchQuery;
            if (!node.UseMatch)
            {
                currentQuery = argument.CurrentTermOption.Query.NotMatchQuery;
            }

            return result => currentQuery(node.Value, argument.Query, argument);
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(NotUnaryNode node, QueryExecutionContext<T> argument)
        {           
            return result => new ValueTask<IQuery<T>>(argument.Query.AllAsync(
                 (q) => node.Operation.Accept(this, argument)(q).AsTask()
            ));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(OrNode node, QueryExecutionContext<T> argument)
        {
            return result => new ValueTask<IQuery<T>>(argument.Query.AnyAsync(
                (q) => node.Left.Accept(this, argument)(q).AsTask(),
                (q) => node.Right.Accept(this, argument)(q).AsTask()
            ));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(AndNode node, QueryExecutionContext<T> argument)
        {
            return result => new ValueTask<IQuery<T>>(argument.Query.AllAsync(
                (q) => node.Left.Accept(this, argument)(q).AsTask(),
                (q) => node.Right.Accept(this, argument)(q).AsTask()
            ));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(GroupNode node, QueryExecutionContext<T> argument)
            => node.Operation.Accept(this, argument);

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(TermNode node, QueryExecutionContext<T> argument)
            => node.Accept(this, argument);
    }

}
