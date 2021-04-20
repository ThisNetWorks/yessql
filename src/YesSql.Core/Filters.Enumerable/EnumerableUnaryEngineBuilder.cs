using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Filters.Abstractions.Builders;
using OrchardCore.Filters.Abstractions.Nodes;
using OrchardCore.Filters.Abstractions.Services;
using OrchardCore.Filters.Enumerable.Services;

namespace OrchardCore.Filters.Enumerable
{
    public class EnumerableUnaryEngineBuilder<T> : UnaryEngineBuilder<T, EnumerableTermOption<T>>
    {
        public EnumerableUnaryEngineBuilder(string name, Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> query) : base(new EnumerableTermOption<T>(name, query))
        {
        }

        public EnumerableUnaryEngineBuilder<T> MapTo<TModel>(Action<string, TModel> map)
        {
            _termOption.MapTo = map;

            return this;
        }

        public EnumerableUnaryEngineBuilder<T> MapFrom<TModel>(Func<TModel, (bool, string)> map)
        {
            Func<string, string, TermNode> factory = (name, value) => new NamedTermNode(name, new UnaryNode(value));

            return MapFrom(map, factory);
        }

        public EnumerableUnaryEngineBuilder<T> MapFrom<TModel>(Func<TModel, (bool, string)> map, Func<string, string, TermNode> factory)
        {
            Action<EnumerableFilterResult<T>, string, TermOption, TModel> mapFrom = (EnumerableFilterResult<T> terms, string name, TermOption termOption, TModel model) =>
            {
                (bool shouldMap, string value) mapResult = map(model);
                if (mapResult.shouldMap)
                {
                    var node = termOption.MapFromFactory(name, mapResult.value);
                    terms.TryAddOrReplace(node);
                }
            };

            _termOption.MapFrom = mapFrom;
            _termOption.MapFromFactory = factory;

            return this;
        }
    }
}
