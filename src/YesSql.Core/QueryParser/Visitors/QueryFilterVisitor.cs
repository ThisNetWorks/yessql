using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser.Visitors
{
    public class QueryFilterVisitor<T> : IFilterVisitor<FilterExecutionContext<IQuery<T>>, Func<IQuery<T>, ValueTask<IQuery<T>>>> where T : class
    {
        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(TermOperationNode node, FilterExecutionContext<IQuery<T>> argument)
            => node.Operation.Accept(this, argument);

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(AndTermNode node, FilterExecutionContext<IQuery<T>> argument)
        {            
            var predicates = new List<Func<IQuery<T>, Task<IQuery<T>>>>();
            foreach (var child in node.Children)
            {
                // Func<IQuery<T>, Task<IQuery<T>>> c = (q) => child.Operation.BuildAsync(context)(q).AsTask();

                Func<IQuery<T>, Task<IQuery<T>>> predicate = (q) => argument.Item.AllAsync(
                    (q) => child.Operation.Accept(this, argument)(q).AsTask()
                );
                predicates.Add(predicate);

            }

            Func<IQuery<T>, Task<IQuery<T>>> result = (Func<IQuery<T>, Task<IQuery<T>>>)Delegate.Combine(predicates.ToArray());

            return xyz => new ValueTask<IQuery<T>>(result(argument.Item));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(UnaryNode node, FilterExecutionContext<IQuery<T>> argument)
        {            
            var currentQuery = argument.CurrentTermOption.Query.MatchQuery;
            if (!node.UseMatch)
            {
                currentQuery = argument.CurrentTermOption.Query.NotMatchQuery;
            }

            return result => currentQuery(node.Value, argument.Item, argument);
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(NotUnaryNode node, FilterExecutionContext<IQuery<T>> argument)
        {           
            return result => new ValueTask<IQuery<T>>(argument.Item.AllAsync(
                 (q) => node.Operation.Accept(this, argument)(q).AsTask()
            ));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(OrNode node, FilterExecutionContext<IQuery<T>> argument)
        {
            return result => new ValueTask<IQuery<T>>(argument.Item.AnyAsync(
                (q) => node.Left.Accept(this, argument)(q).AsTask(),
                (q) => node.Right.Accept(this, argument)(q).AsTask()
            ));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(AndNode node, FilterExecutionContext<IQuery<T>> argument)
        {
            return result => new ValueTask<IQuery<T>>(argument.Item.AllAsync(
                (q) => node.Left.Accept(this, argument)(q).AsTask(),
                (q) => node.Right.Accept(this, argument)(q).AsTask()
            ));
        }

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(GroupNode node, FilterExecutionContext<IQuery<T>> argument)
            => node.Operation.Accept(this, argument);

        public Func<IQuery<T>, ValueTask<IQuery<T>>> Visit(TermNode node, FilterExecutionContext<IQuery<T>> argument)
            => node.Accept(this, argument);
    }

}
