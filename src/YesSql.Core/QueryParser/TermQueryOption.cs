using System;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser
{
    /*
    public class TermQueryOption<TQuery, TContext> where TQuery : class where TContext : FilterExecutionContext<TQuery>
    {
        public TermQueryOption(Func<string, TQuery, TContext, ValueTask<TQuery>> matchQuery)
        {
            MatchQuery = matchQuery;
        }        

        public TermQueryOption(Func<string, TQuery, TContext, ValueTask<TQuery>> matchQuery, Func<string, TQuery, TContext, ValueTask<TQuery>> notMatchQuery)
        {
            MatchQuery = matchQuery;
            NotMatchQuery = notMatchQuery;
        }

        public Func<string, TQuery, TContext, ValueTask<TQuery>> MatchQuery { get; }
        public Func<string, TQuery, TContext, ValueTask<TQuery>> NotMatchQuery { get; }
    }

    public class QueryTermQueryOption<T> : TermQueryOption<IQuery<T>, QueryExecutionContext<T>> where T : class
    {
        public QueryTermQueryOption(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery) : base(matchQuery)
        {
        }

        public QueryTermQueryOption(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery)
            : base (matchQuery, notMatchQuery)
        {
        }
    }   

    public class QueryTermDocumentOption<T> : TermQueryOption<T, DocumentExecutionContext<T>> where T : class
    {
        public QueryTermDocumentOption(Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> matchQuery) : base (matchQuery)
        {
        }

        public QueryTermDocumentOption(Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> matchQuery, Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> notMatchQuery)
            : base (matchQuery, notMatchQuery)
        {
        }
    }    
    */
}
