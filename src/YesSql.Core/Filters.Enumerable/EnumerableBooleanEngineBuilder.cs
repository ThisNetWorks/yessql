using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using OrchardCore.Filters.Builders;
using OrchardCore.Filters.Enumerable.Services;

namespace OrchardCore.Filters.Enumerable
{
    public class EnumerableBooleanEngineBuilder<T> : BooleanEngineBuilder<T, EnumerableTermOption<T>> 
    {
        public EnumerableBooleanEngineBuilder(
            string name,
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> matchQuery,
            Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> notMatchQuery)
        {
            _termOption = new EnumerableTermOption<T>(name, matchQuery, notMatchQuery);
        }
    }
}
