using System;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser.Builders
{
    public static class TermParserBuilderExtensions
    {
        public static UnaryParserBuilder<T> OneCondition<T>(this TermParserBuilder<T> builder, Func<string, IQuery<T>, IQuery<T>> matchQuery) where T : class
        {
            Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> valueQuery = (q, val, ctx) => new ValueTask<IQuery<T>>(matchQuery(q, val));

            return builder.OneCondition(valueQuery);
        }

        public static UnaryParserBuilder<T> OneCondition<T>(this TermParserBuilder<T> builder, Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> matchQuery) where T : class
        {
            var operatorBuilder = new UnaryParserBuilder<T>(builder.Name, matchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }        

        public static BooleanParserBuilder<T> ManyCondition<T>(
            this TermParserBuilder<T> builder, 
            Func<string, IQuery<T>, IQuery<T>> matchQuery,
            Func<string, IQuery<T>, IQuery<T>> notMatchQuery) where T : class
        {
            Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> valueMatch = (q, val, ctx) => new ValueTask<IQuery<T>>(matchQuery(q, val));
            Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> valueNotMatch = (q, val, ctx) => new ValueTask<IQuery<T>>(notMatchQuery(q, val));

            return builder.ManyCondition(valueMatch, valueNotMatch);
        } 

        public static BooleanParserBuilder<T> ManyCondition<T>(
            this TermParserBuilder<T> builder, 
            Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> matchQuery,
            Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> notMatchQuery) where T : class
        {
            var operatorBuilder = new BooleanParserBuilder<T>(builder.Name, matchQuery, notMatchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }                
    }
}
