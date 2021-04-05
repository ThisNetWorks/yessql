using System;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser
{
    public abstract class OperatorNode : QueryNode
    {
    }

    public class UnaryNode : OperatorNode
    {

        public UnaryNode(string value, bool useMatch = true)
        {
            Value = value;
            UseMatch = useMatch;
        }        

        public string Value { get; }
        public bool UseMatch { get; }
        public bool HasValue => !String.IsNullOrEmpty(Value);

        public override Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context)
        {
            var currentQuery = context.CurrentTermOption.Query.MatchQuery;
            if (!UseMatch)
            {
                currentQuery = context.CurrentTermOption.Query.NotMatchQuery;
            }

            return BuildAsyncInternal(context, currentQuery);
        } 

        private Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsyncInternal<Tq>(QueryExecutionContext<Tq> context, Func<string, IQuery<Tq>, QueryExecutionContext<Tq>, ValueTask<IQuery<Tq>>> queryMethod) where Tq : class
        {
            return result => queryMethod(Value, context.Query, context);
        }         

        public override string ToNormalizedString()
            => ToString();

        public override string ToString()
            => $"{Value.ToString()}";
    }

    public class NotUnaryNode : OperatorNode
    {
        public NotUnaryNode(string operatorValue, UnaryNode operation)
        {
            OperatorValue = operatorValue;
            Operation = operation;
        }

        public string OperatorValue { get; }
        public UnaryNode Operation { get; }

        public override Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context)
        {      
            return result => new ValueTask<IQuery<Tq>>(context.Query.AllAsync(
                 (q) => Operation.BuildAsync(context)(q).AsTask()
            ));              
        }   

        public override string ToNormalizedString()
            => ToString();

        public override string ToString()
            => $"{OperatorValue} {Operation.ToString()}";
    }

    public class OrNode : OperatorNode
    {
        public OrNode(OperatorNode left, OperatorNode right, string value)
        {
            Left = left;
            Right = right;
            Value = value;
        }

        public OperatorNode Left { get; }
        public OperatorNode Right { get; }
        public string Value { get; }

        public override Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context)
        {
            return result => new ValueTask<IQuery<Tq>>(context.Query.AnyAsync(
                (q) => Left.BuildAsync(context)(q).AsTask(),
                (q) => Right.BuildAsync(context)(q).AsTask()
            ));
        }

        public override string ToNormalizedString()
            => $"({Left.ToNormalizedString()} OR {Right.ToNormalizedString()})";

        public override string ToString()
            => $"{Left.ToString()} {Value} {Right.ToString()}";
    }

    public class AndNode: OperatorNode 
    {
        public AndNode(OperatorNode left, OperatorNode right, string value)
        {
            Left = left;
            Right = right;
            Value = value;
        }

        public OperatorNode Left { get; }
        public OperatorNode Right { get; }
        public string Value { get; }

        public override Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context)
        {
            return result => new ValueTask<IQuery<Tq>>(context.Query.AllAsync(
                (q) => Left.BuildAsync(context)(q).AsTask(),
                (q) => Right.BuildAsync(context)(q).AsTask()
            ));
        }

        public override string ToNormalizedString()
            => $"({Left.ToNormalizedString()} AND {Right.ToNormalizedString()})";

        public override string ToString()
            => $"{Left.ToString()} {Value} {Right.ToString()}";
    }

    public class NotNode : AndNode
    {
        public NotNode(OperatorNode left, OperatorNode right, string value) : base(left, right, value)
        {
        }

        public override string ToNormalizedString()
            => $"({Left.ToNormalizedString()} NOT {Right.ToNormalizedString()})";

        public override string ToString()
            => $"{Left.ToString()} {Value} {Right.ToString()}";
    }

    /// <summary>
    /// Marks a node as being produced by a group request, i.e. () were specified
    /// </summary>

    public class GroupNode : OperatorNode 
    {
        public GroupNode(OperatorNode operation)
        {
            Operation = operation;
        }

        public OperatorNode Operation { get; }

        public override Func<IQuery<Tq>, ValueTask<IQuery<Tq>>> BuildAsync<Tq>(QueryExecutionContext<Tq> context)
            => Operation.BuildAsync(context);

        public override string ToNormalizedString()
            => ToString();

        public override string ToString()
            => $"({Operation.ToString()})";
    }

}
