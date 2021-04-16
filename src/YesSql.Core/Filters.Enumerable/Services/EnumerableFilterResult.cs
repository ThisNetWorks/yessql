using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Filters.Abstractions.Nodes;
using OrchardCore.Filters.Abstractions.Services;

namespace OrchardCore.Filters.Enumerable.Services
{
    public class EnumerableFilterResult<T> : FilterResult<T, EnumerableTermOption<T>>
    {
        public EnumerableFilterResult(IReadOnlyDictionary<string, EnumerableTermOption<T>> termOptions) : base(termOptions)
        { }


        public EnumerableFilterResult(List<TermNode> terms, IReadOnlyDictionary<string, EnumerableTermOption<T>> termOptions) : base(terms, termOptions)
        { }
        
        public void MapFrom<TModel>(TModel model)
        {
            foreach (var option in _termOptions)
            {
                if (option.Value.MapFrom is Action<EnumerableFilterResult<T>, string, TermOption, TModel> mappingMethod)
                {
                    mappingMethod(this, option.Key, option.Value, model);
                }
            }
        }

        public async ValueTask<IEnumerable<T>> ExecuteAsync(IEnumerable<T> source, IServiceProvider serviceProvider) //TODO if queryexecutioncontext provided, use that.
        {
            var context = new EnumerableExecutionContext<T>(source, serviceProvider);
            var visitor = new EnumerableFilterVisitor<T>();

            foreach (var term in _terms.Values)
            {
                // TODO optimize value task later.

                context.CurrentTermOption = _termOptions[term.TermName];

                var termQuery = visitor.Visit(term, context);
                source = await termQuery.Invoke(source);
                context.CurrentTermOption = null;
            }

            return source;
        }
    }
}
