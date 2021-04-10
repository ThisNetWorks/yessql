using System;
using System.Threading.Tasks;

namespace YesSql.Core.FilterEngines.Builders
{
    public static class QueryTermFilterBuilderExtensions
    {
        public static QueryUnaryEngineBuilder<T> OneCondition<T>(this TermEngineBuilder<T, QueryTermOption<T>> builder, Func<string, IQuery<T>, IQuery<T>> matchQuery) where T : class
        {
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> valueQuery = (q, val, ctx) => new ValueTask<IQuery<T>>(matchQuery(q, val));

            return builder.OneCondition(valueQuery);
        }

        public static QueryUnaryEngineBuilder<T> OneCondition<T>(this TermEngineBuilder<T, QueryTermOption<T>> builder, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery) where T : class
        {
            var operatorBuilder = new QueryUnaryEngineBuilder<T>(builder.Name, matchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }

        public static QueryBooleanEngineBuilder<T> ManyCondition<T>(
            this TermEngineBuilder<T, QueryTermOption<T>> builder,
            Func<string, IQuery<T>, IQuery<T>> matchQuery,
            Func<string, IQuery<T>, IQuery<T>> notMatchQuery) where T : class
        {
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> valueMatch = (q, val, ctx) => new ValueTask<IQuery<T>>(matchQuery(q, val));
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> valueNotMatch = (q, val, ctx) => new ValueTask<IQuery<T>>(notMatchQuery(q, val));

            return builder.ManyCondition(valueMatch, valueNotMatch);
        }

        public static QueryBooleanEngineBuilder<T> ManyCondition<T>(
            this TermEngineBuilder<T, QueryTermOption<T>> builder,
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery,
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery) where T : class
        {
            var operatorBuilder = new QueryBooleanEngineBuilder<T>(builder.Name, matchQuery, notMatchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }
    }

    public static class EnumerableTermFilterBuilderExtensions
    {
        public static DocumentUnaryEngineBuilder<T> OneCondition<T>(this TermEngineBuilder<T, EnumerableTermOption<T>> builder, Func<string, T, T> matchQuery) where T : class
        {
            Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> valueQuery = (q, val, ctx) => new ValueTask<T>(matchQuery(q, val));

            return builder.OneCondition(valueQuery);
        }

        public static DocumentUnaryEngineBuilder<T> OneCondition<T>(this TermEngineBuilder<T, EnumerableTermOption<T>> builder, Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> matchQuery) where T : class
        {
            var operatorBuilder = new DocumentUnaryEngineBuilder<T>(builder.Name, matchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }

        public static DocumentBooleanEngineBuilder<T> ManyCondition<T>(
            this TermEngineBuilder<T, EnumerableTermOption<T>> builder,
            Func<string, T, T> matchQuery,
            Func<string, T, T> notMatchQuery) where T : class
        {
            Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> valueMatch = (q, val, ctx) => new ValueTask<T>(matchQuery(q, val));
            Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> valueNotMatch = (q, val, ctx) => new ValueTask<T>(notMatchQuery(q, val));

            return builder.ManyCondition(valueMatch, valueNotMatch);
        }

        public static DocumentBooleanEngineBuilder<T> ManyCondition<T>(
            this TermEngineBuilder<T, EnumerableTermOption<T>> builder,
            Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> matchQuery,
            Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> notMatchQuery) where T : class
        {
            var operatorBuilder = new DocumentBooleanEngineBuilder<T>(builder.Name, matchQuery, notMatchQuery);
            builder.SetOperator(operatorBuilder);

            return operatorBuilder;
        }
    }
}
