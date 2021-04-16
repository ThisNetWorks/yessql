using OrchardCore.Filters.Services;
using System;
using System.Collections.Generic;
using YesSql;

namespace OrchardCore.Filters.Query.Services
{
    public class QueryExecutionContext<T> : FilterExecutionContext<IQuery<T>> where T : class
    {
        public QueryExecutionContext(IQuery<T> query, IServiceProvider serviceProvider) : base(query, serviceProvider)
        {
        }

        public QueryTermOption<T> CurrentTermOption { get; set; }
    }
}
