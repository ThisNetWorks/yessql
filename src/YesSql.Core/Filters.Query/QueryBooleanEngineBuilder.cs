using System;
using System.Threading.Tasks;
using static Parlot.Fluent.Parsers;
using Parlot.Fluent;
using System.Collections.Generic;
using YesSql;
using OrchardCore.Filters.Builders;
using OrchardCore.Filters.Query.Services;

namespace OrchardCore.Filters.Query
{
    public class QueryBooleanEngineBuilder<T> : BooleanEngineBuilder<T, QueryTermOption<T>> where T : class
    {
        public QueryBooleanEngineBuilder(
            string name,
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery,
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery)
        {
            _termOption = new QueryTermOption<T>(name, matchQuery, notMatchQuery);
        }
    }
}
