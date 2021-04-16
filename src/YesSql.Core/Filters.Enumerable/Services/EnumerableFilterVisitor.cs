using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Filters.Abstractions.Nodes;
using OrchardCore.Filters.Abstractions.Services;

namespace OrchardCore.Filters.Enumerable.Services
{
    public class EnumerableFilterVisitor<T> : IFilterVisitor<EnumerableExecutionContext<T>, Func<IEnumerable<T>, ValueTask<IEnumerable<T>>>>
    {
        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(TermNode node, EnumerableExecutionContext<T> argument)
            => node.Accept(this, argument);

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(TermOperationNode node, EnumerableExecutionContext<T> argument)
            => node.Operation.Accept(this, argument);

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(AndTermNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(UnaryNode node, EnumerableExecutionContext<T> argument)
        {
            var currentQuery = argument.CurrentTermOption.MatchPredicate;
            if (!node.UseMatch)
            {
                currentQuery = argument.CurrentTermOption.NotMatchPredicate;
            }

            return result => currentQuery(node.Value, argument.Item, argument);
        }

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(NotUnaryNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(OrNode node, EnumerableExecutionContext<T> argument)
        {
            // Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> result = async (source) => 
            // {
            //     var left = await node.Left.Accept(this, argument).Invoke(source);
            //     var right = await node.Right.Accept(this, argument).Invoke(source);

            //     return left.Union(right);

            // };

            // return result;

            return async (source) => 
            {
                var left = await node.Left.Accept(this, argument).Invoke(source);
                var right = await node.Right.Accept(this, argument).Invoke(source);

                return left.Union(right);
            };
        }

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(AndNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<IEnumerable<T>, ValueTask<IEnumerable<T>>> Visit(GroupNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }
    }

}
