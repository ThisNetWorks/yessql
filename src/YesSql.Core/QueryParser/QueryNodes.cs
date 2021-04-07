using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser
{
    public abstract class QueryNode
    {
        public abstract Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context) where T : class;

        public abstract string ToNormalizedString();
    }

    public abstract class TermNode : QueryNode
    {
        public TermNode(string termName)
        {
            TermName = termName;
        }

        public string TermName { get; }
    }

    public abstract class TermOperationNode : TermNode
    {
        public TermOperationNode(string termName, OperatorNode operation) : base(termName)
        {
            Operation = operation;
        }

        public OperatorNode Operation { get; }

        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
            => Operation.BuildAsync(context);
    }

    public class NamedTermNode : TermOperationNode
    {
        public NamedTermNode(string termName, OperatorNode operation) : base(termName, operation)
        {
        }

        public override string ToNormalizedString()
            => $"{TermName}:{Operation.ToNormalizedString()}";

        public override string ToString()
            => $"{TermName}:{Operation.ToString()}";
    }


    public class DefaultTermNode : TermOperationNode
    {
        public DefaultTermNode(string termName, OperatorNode operation) : base(termName, operation)
        {
        }

        public override string ToNormalizedString() // normalizing includes the term name even if not specified.
            => $"{TermName}:{Operation.ToNormalizedString()}";

        public override string ToString()
            => $"{Operation.ToString()}";
    }

    public abstract class CompoundTermNode : TermNode
    {
        public CompoundTermNode(string termName) : base(termName)
        {
        }

        public List<TermOperationNode> Children { get; } = new();
    }

    public class AndTermNode : CompoundTermNode
    {
        public AndTermNode(TermOperationNode existingTerm, TermOperationNode newTerm) : base(existingTerm.TermName)
        {
            Children.Add(existingTerm);
            Children.Add(newTerm);
        }

        // TODO this works, but really need to test it against taxonomies to see if the logic is correct.
        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
        {
            var predicates = new List<Func<IQuery<T>, Task<IQuery<T>>>>();
            foreach (var child in Children)
            {
                // Func<IQuery<T>, Task<IQuery<T>>> c = (q) => child.Operation.BuildAsync(context)(q).AsTask();

                Func<IQuery<T>, Task<IQuery<T>>> predicate = (q) => context.Query.AllAsync(
                    (q) => child.Operation.BuildAsync(context)(q).AsTask()
                );
                predicates.Add(predicate);

            }

            Func<IQuery<T>, Task<IQuery<T>>> result = (Func<IQuery<T>, Task<IQuery<T>>>)Delegate.Combine(predicates.ToArray());

            return xyz => new ValueTask<IQuery<T>>(result(context.Query));
        }

        public override string ToNormalizedString()
            => string.Join(" ", Children.Select(c => c.ToNormalizedString()));

        public override string ToString()
            => string.Join(" ", Children.Select(c => c.ToString()));
    }
}
