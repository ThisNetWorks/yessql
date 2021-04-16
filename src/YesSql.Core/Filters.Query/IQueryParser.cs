using OrchardCore.Filters.Query.Services;
using OrchardCore.Filters.Services;

namespace OrchardCore.Filters.Query
{
    public interface IQueryParser<T> : IFilterParser<QueryFilterResult<T>> where T : class
    {
    }
}
