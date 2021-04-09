using System;
using YesSql.Core.DocumentParser;

namespace YesSql.Core.QueryParser
{
    // public abstract class FilterExecutionContext
    // {

    // }
    public class FilterExecutionContext<T>
    {
        public FilterExecutionContext(T item, IServiceProvider serviceProvider)
        {
            Item = item;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
        public T Item { get; }
    }

    // public class QueryExecutionContext<T> : FilterExecutionContext<IQuery<T>> where T : class 
    // {
    //     public QueryExecutionContext(IQuery<T> query, IServiceProvider serviceProvider) : base (query, serviceProvider)
    //     {
    //     }

    //     public TermOption<T> CurrentTermOption { get; set; }
    // }

    // public class DocumentExecutionContext<T> : FilterExecutionContext<T>  where T : class 
    // {
    //     public DocumentExecutionContext(T item, IServiceProvider serviceProvider) : base (item, serviceProvider)
    //     {
    //     }

    //     public DocumentTermOption<T> CurrentTermOption { get; set; }
    // }    
}
