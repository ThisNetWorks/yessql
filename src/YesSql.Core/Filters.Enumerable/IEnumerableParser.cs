using OrchardCore.Filters.Enumerable.Services;
using OrchardCore.Filters.Abstractions.Services;

namespace OrchardCore.Filters.Enumerable
{
    public interface IEnumerableParser<T> : IFilterParser<EnumerableFilterResult<T>>
    {
    }
}