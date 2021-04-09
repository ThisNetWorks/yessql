using System;
using System.Threading.Tasks;
using YesSql.Core.DocumentParser;
using YesSql.Core.QueryParser.Visitors;

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

        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
        {
            var currentQuery = context.CurrentTermOption.Query.MatchQuery;
            if (!UseMatch)
            {
                currentQuery = context.CurrentTermOption.Query.NotMatchQuery;
            }

            return BuildAsyncInternal(context, currentQuery);
        } 

        private Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsyncInternal<T>(QueryExecutionContext<T> context, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> queryMethod) where T : class
        {
            return result => queryMethod(Value, context.Query, context);
        }         

        public override string ToNormalizedString()
            => ToString();

        public override string ToString()
            => $"{Value.ToString()}";

        public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
            => visitor.Visit(this, argument);

        public override Func<T, ValueTask<T>> BuildDocumentAsync<T>(DocumentExecutionContext<T> context)
        {
            var currentQuery = context.CurrentTermOption.Query.MatchQuery;
            if (!UseMatch)
            {
                currentQuery = context.CurrentTermOption.Query.NotMatchQuery;
            }

            return BuildDocumentAsyncInternal(context, currentQuery);

        }

        private Func<T, ValueTask<T>> BuildDocumentAsyncInternal<T>(DocumentExecutionContext<T> context, Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> queryMethod) where T : class
        {
            // return result => queryMethod(Value, context.Document, context);

            Func<T, ValueTask<T>> result = (t) => queryMethod(Value, context.Document, context);

            return result;
        }          
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

        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
        {      
            return result => new ValueTask<IQuery<T>>(context.Query.AllAsync(
                 (q) => Operation.BuildAsync(context)(q).AsTask()
            ));              
        }   
        public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
            => visitor.Visit(this, argument);
        public override string ToNormalizedString()
            => ToString();

        public override string ToString()
            => $"{OperatorValue} {Operation.ToString()}";

        public override Func<T, ValueTask<T>> BuildDocumentAsync<T>(DocumentExecutionContext<T> context)
        {
            throw new NotImplementedException();
        }
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

        public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
            => visitor.Visit(this, argument);

        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
        {
            return result => new ValueTask<IQuery<T>>(context.Query.AnyAsync(
                (q) => Left.BuildAsync(context)(q).AsTask(),
                (q) => Right.BuildAsync(context)(q).AsTask()
            ));
        }

        public override Func<T, ValueTask<T>> BuildDocumentAsync<T>(DocumentExecutionContext<T> context)
        {
            throw new NotImplementedException();
        }

        public override string ToNormalizedString()
            => $"({Left.ToNormalizedString()} OR {Right.ToNormalizedString()})";

        public override string ToString()
            => $"{Left.ToString()} {Value} {Right.ToString()}";
    }

    public class AndNode : OperatorNode 
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

        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
        {
            return result => new ValueTask<IQuery<T>>(context.Query.AllAsync(
                (q) => Left.BuildAsync(context)(q).AsTask(),
                (q) => Right.BuildAsync(context)(q).AsTask()
            ));
        }
        public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
            => visitor.Visit(this, argument);
        public override string ToNormalizedString()
            => $"({Left.ToNormalizedString()} AND {Right.ToNormalizedString()})";

        public override string ToString()
            => $"{Left.ToString()} {Value} {Right.ToString()}";

        public override Func<T, ValueTask<T>> BuildDocumentAsync<T>(DocumentExecutionContext<T> context)
        {
            throw new NotImplementedException();
        }
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

        public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
            => visitor.Visit(this, argument);

        public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
            => Operation.BuildAsync(context);

        public override Func<T, ValueTask<T>> BuildDocumentAsync<T>(DocumentExecutionContext<T> context)
        {
            throw new NotImplementedException();
        }

        public override string ToNormalizedString()
            => ToString();

        public override string ToString()
            => $"({Operation.ToString()})";
    }

}
