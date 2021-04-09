using System;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser
{
    public abstract class TermOption
    {
        public TermOption(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// Whether one or many of the specified term is allowed.
        /// </summary>
        public bool Single { get; set; } = true;

        public Delegate MapTo { get; set; }
        public Delegate MapFrom { get; set; }
        public Func<string, string, TermNode> MapFromFactory { get; set; }
    }

    public class TermQOption<TQuery, TQueryContext, TContext> : TermOption where TQuery : class where TContext : FilterExecutionContext<TQueryContext> // how to make this IQuery<T> or just T
    {
        public TermQOption(string name, Func<string, TQuery, TContext, ValueTask<TQuery>> matchQuery) : base(name)
        {
            MatchQuery = matchQuery;
        }        

        public TermQOption(string name, Func<string, TQuery, TContext, ValueTask<TQuery>> matchQuery, Func<string, TQuery, TContext, ValueTask<TQuery>> notMatchQuery) : base(name)
        {
            MatchQuery = matchQuery;
            NotMatchQuery = notMatchQuery;
        }

        public Func<string, TQuery, TContext, ValueTask<TQuery>> MatchQuery { get; }
        public Func<string, TQuery, TContext, ValueTask<TQuery>> NotMatchQuery { get; }
    }

    public class TermOption<T> : TermOption where T : class
    {
        // the idea is to bring these two together. they were seperate but the reason for that has changed.
        public TermOption(string name, QueryTermQueryOption<T> query) : base(name)
        {
            Query = query;
        }

        public QueryTermQueryOption<T> Query { get; }
    }

//    public class TermQueryOption<TQuery, TContext> where TQuery : class where TContext : FilterExecutionContext<TQuery>
//     {
//         public TermQueryOption(Func<string, TQuery, TContext, ValueTask<TQuery>> matchQuery)
//         {
//             MatchQuery = matchQuery;
//         }        

//         public TermQueryOption(Func<string, TQuery, TContext, ValueTask<TQuery>> matchQuery, Func<string, TQuery, TContext, ValueTask<TQuery>> notMatchQuery)
//         {
//             MatchQuery = matchQuery;
//             NotMatchQuery = notMatchQuery;
//         }

//         public Func<string, TQuery, TContext, ValueTask<TQuery>> MatchQuery { get; }
//         public Func<string, TQuery, TContext, ValueTask<TQuery>> NotMatchQuery { get; }
//     }

    public class QueryTermOption2<T> : TermQOption<IQuery<T>, IQuery<T>, FilterExecutionContext<IQuery<T>>> where T : class
    {
        public QueryTermOption2(string name, Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> matchQuery) : base(name, matchQuery)
        {
        }

        public QueryTermOption2(string name, Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> matchQuery, Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> notMatchQuery)
            : base (name, matchQuery, notMatchQuery)
        {
        }
    }   

    public class DocumentTermOption2<T> : TermQOption<T, T, FilterExecutionContext<T>> where T : class
    {
        public DocumentTermOption2(string name, Func<string, T, FilterExecutionContext<T>, ValueTask<T>> matchQuery) : base (name, matchQuery)
        {
        }

        public DocumentTermOption2(string name, Func<string, T, FilterExecutionContext<T>, ValueTask<T>> matchQuery, Func<string, T, FilterExecutionContext<T>, ValueTask<T>> notMatchQuery)
            : base (name, matchQuery, notMatchQuery)
        {
        }
    }    


}
