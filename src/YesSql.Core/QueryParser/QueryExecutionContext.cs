using System;

namespace YesSql.Core.QueryParser
{
    public class QueryExecutionContext<T> where T : class 
    {
        public QueryExecutionContext(IQuery<T> query, IServiceProvider serviceProvider)
        {
            Query = query;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
        public IQuery<T> Query { get; }

        public TermOption<T> CurrentTermOption { get; set; }
    }
}
