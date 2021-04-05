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
}
