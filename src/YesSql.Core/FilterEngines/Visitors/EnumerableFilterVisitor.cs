using System;
using System.Threading.Tasks;

namespace YesSql.Core.FilterEngines.Visitors
{
    public class EnumerableFilterVisitor<T> : IFilterVisitor<EnumerableExecutionContext<T>, Func<T, ValueTask<T>>> where T : class
    {
        public Func<T, ValueTask<T>> Visit(TermNode node, EnumerableExecutionContext<T> argument)
            => node.Accept(this, argument);

        public Func<T, ValueTask<T>> Visit(TermOperationNode node, EnumerableExecutionContext<T> argument)
            => node.Operation.Accept(this, argument);

        public Func<T, ValueTask<T>> Visit(AndTermNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<T, ValueTask<T>> Visit(UnaryNode node, EnumerableExecutionContext<T> argument)
        {
            var currentQuery = argument.CurrentTermOption.MatchPredicate;
            if (!node.UseMatch)
            {
                currentQuery = argument.CurrentTermOption.NotMatchPredicate;
            }

            return result => currentQuery(node.Value, argument.Item, argument);
        }

        public Func<T, ValueTask<T>> Visit(NotUnaryNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<T, ValueTask<T>> Visit(OrNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<T, ValueTask<T>> Visit(AndNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }

        public Func<T, ValueTask<T>> Visit(GroupNode node, EnumerableExecutionContext<T> argument)
        {
            throw new NotImplementedException();
        }
    }

}
