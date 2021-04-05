using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser
{
    public abstract class QueryNode
    {
        public abstract Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context) where Tq : class;

        public abstract string ToNormalizedString();
    }

    public abstract class TermNode : QueryNode 
    {
        public TermNode(string termName, OperatorNode operation)
        {
            TermName = termName;
            Operation = operation;
        }

        public string TermName { get; }
        public OperatorNode Operation { get; }

        public override Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context)
            => Operation.BuildAsync(context);            
    }

    public class NamedTermNode : TermNode 
    {
        public NamedTermNode(string termName, OperatorNode operation) : base(termName, operation)
        {
        }

        public override string ToNormalizedString()
            => $"{TermName}:{Operation.ToNormalizedString()}";

        public override string ToString()
            => $"{TermName}:{Operation.ToString()}";
    }


    public class DefaultTermNode : TermNode
    {
        public DefaultTermNode(string termName, OperatorNode operation) : base(termName, operation)
        {
        }

        public override string ToNormalizedString() // normalizing includes the term name even if not specified.
            => $"{TermName}:{Operation.ToNormalizedString()}";

        public override string ToString()
            => $"{Operation.ToString()}";
    }
}
