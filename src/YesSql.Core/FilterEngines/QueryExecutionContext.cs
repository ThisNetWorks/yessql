using System;

namespace YesSql.Core.FilterEngines
{
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

    public class QueryExecutionContext<T> : FilterExecutionContext<IQuery<T>> where T : class
    {
        public QueryExecutionContext(IQuery<T> query, IServiceProvider serviceProvider) : base(query, serviceProvider)
        {
        }

        public QueryTermOption<T> CurrentTermOption { get; set; }
    }

    public class EnumerableExecutionContext<T> : FilterExecutionContext<T>
    {
        public EnumerableExecutionContext(T item, IServiceProvider serviceProvider) : base(item, serviceProvider)
        {
        }

        public EnumerableTermOption<T> CurrentTermOption { get; set; }
    }
}
