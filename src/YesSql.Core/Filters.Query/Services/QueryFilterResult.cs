using OrchardCore.Filters.Nodes;
using OrchardCore.Filters.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace OrchardCore.Filters.Query.Services
{
    public class QueryFilterResult<T> : FilterResult<T, QueryTermOption<T>> where T : class
    {
        public QueryFilterResult(IReadOnlyDictionary<string, QueryTermOption<T>> termOptions) : base(termOptions)
        { }


        public QueryFilterResult(List<TermNode> terms, IReadOnlyDictionary<string, QueryTermOption<T>> termOptions) : base(terms, termOptions)
        { }

        public void MapFrom<TModel>(TModel model)
        {
            foreach (var option in _termOptions)
            {
                if (option.Value.MapFrom is Action<QueryFilterResult<T>, string, TermOption, TModel> mappingMethod)
                {
                    mappingMethod(this, option.Key, option.Value, model);
                }
            }
        }

        public async ValueTask<IQuery<T>> ExecuteAsync(IQuery<T> query, IServiceProvider serviceProvider) //TODO if queryexecutioncontext provided, use that.
        {
            var context = new QueryExecutionContext<T>(query, serviceProvider);

            var visitor = new QueryFilterVisitor<T>();

            foreach (var term in _terms.Values)
            {
                // TODO optimize value task later.

                context.CurrentTermOption = _termOptions[term.TermName];

                var termQuery = visitor.Visit(term, context);
                query = await termQuery.Invoke(query);
                context.CurrentTermOption = null;
            }

            return query;
        }
    }
}
