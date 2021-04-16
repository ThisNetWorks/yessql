// using System;
// using System.Threading.Tasks;
// using YesSql.Core.FilterEngines.Visitors;

// namespace YesSql.Core.FilterEngines
// {
//     public abstract class OperatorNode : FilterNode
//     {
//     }

//     public class UnaryNode : OperatorNode
//     {

//         public UnaryNode(string value, bool useMatch = true)
//         {
//             Value = value;
//             UseMatch = useMatch;
//         }

//         public string Value { get; }
//         public bool UseMatch { get; }
//         public bool HasValue => !String.IsNullOrEmpty(Value);

//         // public override Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsync<T>(QueryExecutionContext<T> context)
//         // {
//         //     var currentQuery = context.CurrentTermOption.Query.MatchQuery;
//         //     if (!UseMatch)
//         //     {
//         //         currentQuery = context.CurrentTermOption.Query.NotMatchQuery;
//         //     }

//         //     return BuildAsyncInternal(context, currentQuery);
//         // } 

//         // private Func<IQuery<T>, ValueTask<IQuery<T>>> BuildAsyncInternal<T>(QueryExecutionContext<T> context, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> queryMethod) where T : class
//         // {
//         //     return result => queryMethod(Value, context.Item, context);
//         // }         

//         public override string ToNormalizedString()
//             => ToString();

//         public override string ToString()
//             => $"{Value.ToString()}";

//         public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
//             => visitor.Visit(this, argument);

//         // public override Func<T, ValueTask<T>> BuildDocumentAsync<T>(FilterExecutionContext<T> context)
//         // {
//         //     var currentQuery = context.CurrentTermOption.Query.MatchQuery;
//         //     if (!UseMatch)
//         //     {
//         //         currentQuery = context.CurrentTermOption.Query.NotMatchQuery;
//         //     }

//         //     return BuildDocumentAsyncInternal(context, currentQuery);

//         // }

//         // private Func<T, ValueTask<T>> BuildDocumentAsyncInternal<T>(FilterExecutionContext<T> context, Func<string, T, FilterExecutionContext<T>, ValueTask<T>> queryMethod) where T : class
//         // {
//         //     // return result => queryMethod(Value, context.Document, context);

//         //     Func<T, ValueTask<T>> result = (t) => queryMethod(Value, context.Item, context);

//         //     return result;
//         // }          
//     }

//     public class NotUnaryNode : OperatorNode
//     {
//         public NotUnaryNode(string operatorValue, UnaryNode operation)
//         {
//             OperatorValue = operatorValue;
//             Operation = operation;
//         }

//         public string OperatorValue { get; }
//         public UnaryNode Operation { get; }

//         public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
//             => visitor.Visit(this, argument);
//         public override string ToNormalizedString()
//             => ToString();

//         public override string ToString()
//             => $"{OperatorValue} {Operation.ToString()}";
//     }

//     public class OrNode : OperatorNode
//     {
//         public OrNode(OperatorNode left, OperatorNode right, string value)
//         {
//             Left = left;
//             Right = right;
//             Value = value;
//         }

//         public OperatorNode Left { get; }
//         public OperatorNode Right { get; }
//         public string Value { get; }

//         public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
//             => visitor.Visit(this, argument);

//         public override string ToNormalizedString()
//             => $"({Left.ToNormalizedString()} OR {Right.ToNormalizedString()})";

//         public override string ToString()
//             => $"{Left.ToString()} {Value} {Right.ToString()}";
//     }

//     public class AndNode : OperatorNode
//     {
//         public AndNode(OperatorNode left, OperatorNode right, string value)
//         {
//             Left = left;
//             Right = right;
//             Value = value;
//         }

//         public OperatorNode Left { get; }
//         public OperatorNode Right { get; }
//         public string Value { get; }

//         public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
//             => visitor.Visit(this, argument);
//         public override string ToNormalizedString()
//             => $"({Left.ToNormalizedString()} AND {Right.ToNormalizedString()})";

//         public override string ToString()
//             => $"{Left.ToString()} {Value} {Right.ToString()}";
//     }

//     public class NotNode : AndNode
//     {
//         public NotNode(OperatorNode left, OperatorNode right, string value) : base(left, right, value)
//         {
//         }

//         public override string ToNormalizedString()
//             => $"({Left.ToNormalizedString()} NOT {Right.ToNormalizedString()})";

//         public override string ToString()
//             => $"{Left.ToString()} {Value} {Right.ToString()}";
//     }

//     /// <summary>
//     /// Marks a node as being produced by a group request, i.e. () were specified
//     /// </summary>

//     public class GroupNode : OperatorNode
//     {
//         public GroupNode(OperatorNode operation)
//         {
//             Operation = operation;
//         }

//         public OperatorNode Operation { get; }

//         public override TResult Accept<TArgument, TResult>(IFilterVisitor<TArgument, TResult> visitor, TArgument argument)
//             => visitor.Visit(this, argument);

//         public override string ToNormalizedString()
//             => ToString();

//         public override string ToString()
//             => $"({Operation.ToString()})";
//     }

// }
