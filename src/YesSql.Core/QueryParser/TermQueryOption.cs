using Parlot;
using Parlot.Fluent;
using System;
using System.Threading.Tasks;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser
{
    public class TermQueryOption<T>  where T : class
    {
        public TermQueryOption(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery)
        {
            MatchQuery = matchQuery;
        }

        public TermQueryOption(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery)
        {
            MatchQuery = matchQuery;
            NotMatchQuery = notMatchQuery;
        }

        public Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> MatchQuery { get; }

        public Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> NotMatchQuery { get; }
    }

    public class TermQueryOption2<T> where T : class // this might go.
    {
        public TermQueryOption2(Func<string, T, QueryExecutionContext<T>, ValueTask<T>> matchQuery)
        {
            MatchQuery = matchQuery;
        }

        public TermQueryOption2(Func<string, T, QueryExecutionContext<T>, ValueTask<T>> matchQuery, Func<string, T, QueryExecutionContext<T>, ValueTask<T>> notMatchQuery)
        {
            MatchQuery = matchQuery;
            NotMatchQuery = notMatchQuery;
        }

        public Func<string, T, QueryExecutionContext<T>, ValueTask<T>> MatchQuery { get; }

        public Func<string, T, QueryExecutionContext<T>, ValueTask<T>> NotMatchQuery { get; }
    }    
}
