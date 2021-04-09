using System;
using System.Threading.Tasks;
using YesSql.Core.QueryParser;

namespace YesSql.Core.DocumentParser.Builders
{
    public static class DocumentTermParserBuilderExtensions
    {
        public static DocumentUnaryParserBuilder<T> OneCondition<T>(this DocumentTermParserBuilder<T> builder, Func<string, T, T> matchQuery) where T : class
        {
            Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> valueQuery = (q, val, ctx) => new ValueTask<T>(matchQuery(q, val));

            return builder.OneCondition(valueQuery);
        }

        public static DocumentUnaryParserBuilder<T> OneCondition<T>(this DocumentTermParserBuilder<T> builder, Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> matchQuery) where T : class
        {
            var operatorBuilder = new DocumentUnaryParserBuilder<T>(builder.Name, matchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }        

        // public static BooleanParserBuilder<T> ManyCondition<T>(
        //     this TermParserBuilder<T> builder, 
        //     Func<string, IQuery<T>, IQuery<T>> matchQuery,
        //     Func<string, IQuery<T>, IQuery<T>> notMatchQuery) where T : class
        // {
        //     Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> valueMatch = (q, val, ctx) => new ValueTask<IQuery<T>>(matchQuery(q, val));
        //     Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> valueNotMatch = (q, val, ctx) => new ValueTask<IQuery<T>>(notMatchQuery(q, val));

        //     return builder.ManyCondition(valueMatch, valueNotMatch);
        // } 

        // public static BooleanParserBuilder<T> ManyCondition<T>(
        //     this TermParserBuilder<T> builder, 
        //     Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery,
        //     Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery) where T : class
        // {
        //     var operatorBuilder = new BooleanParserBuilder<T>(builder.Name, matchQuery, notMatchQuery);
        //     builder.SetOperator(operatorBuilder);

        //     return operatorBuilder;
        // }                
    }
}
