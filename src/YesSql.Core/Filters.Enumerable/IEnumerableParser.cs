using OrchardCore.Filters.Enumerable.Services;
using OrchardCore.Filters.Services;

namespace OrchardCore.Filters.Enumerable
{
    public interface IEnumerableParser<T> : IFilterParser<EnumerableFilterResult<T>>
    {
    }
}