using System;
using System.Collections.Generic;
using OrchardCore.Filters.Services;

namespace OrchardCore.Filters.Enumerable.Services
{
    public class EnumerableExecutionContext<T> : FilterExecutionContext<IEnumerable<T>>
    {
        public EnumerableExecutionContext(IEnumerable<T> item, IServiceProvider serviceProvider) : base(item, serviceProvider)
        {
        }

        public EnumerableTermOption<T> CurrentTermOption { get; set; }
    }
}
