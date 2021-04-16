using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.Filters.Enumerable;
using OrchardCore.Filters.Enumerable.Services;
using OrchardCore.Filters.Query;
using OrchardCore.Filters.Query.Services;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YesSql.Search.ModelBinding
{
    public class EnumerableFilterEngineModelBinder<T> : FilterEngineModelBinder<EnumerableFilterResult<T>>
    {
        private readonly IEnumerableParser<T> _parser;

        public EnumerableFilterEngineModelBinder(IEnumerableParser<T> parser)
        {
            _parser = parser;
        }

        public override EnumerableFilterResult<T> Parse(string text)
            => _parser.Parse(text);
    }
}