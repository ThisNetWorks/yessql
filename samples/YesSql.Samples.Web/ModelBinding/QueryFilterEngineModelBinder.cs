using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.Filters.Query;
using OrchardCore.Filters.Query.Services;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YesSql.Search.ModelBinding
{
    public class QueryFilterEngineModelBinder<T> : FilterEngineModelBinder<QueryFilterResult<T>> where T : class
    {
        private readonly IQueryParser<T> _parser;

        public QueryFilterEngineModelBinder(IQueryParser<T> parser)
        {
            _parser = parser;
        }

        public override QueryFilterResult<T> Parse(string text)
            => _parser.Parse(text);
    }
}