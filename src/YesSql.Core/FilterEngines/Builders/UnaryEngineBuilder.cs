using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.FilterEngines.Builders
{
    public abstract class UnaryEngineBuilder<T, TTermOption> : OperatorEngineBuilder<T, TTermOption> where TTermOption : TermOption
    {
        private TTermOption _termOption;
        private static Parser<OperatorNode> _parser
            => Terms.String()
                .Or(
                    Terms.NonWhiteSpace()
                )
                    .Then<OperatorNode>(static (node) => new UnaryNode(node.ToString()));

        public UnaryEngineBuilder(TTermOption termOption)
        {
            _termOption = termOption;
        }

        public UnaryEngineBuilder<T, TTermOption> AllowMultiple()
        {
            _termOption.Single = false;

            return this;
        }

        public override (Parser<OperatorNode> Parser, TTermOption TermOption) Build()
            => (_parser, _termOption);

        // public UnaryEngineBuilder<T, TTermOption> MapFrom<TModel>(Func<TModel, (bool, string)> map)
        // {
        //     Func<string, string, TermNode> factory = (name, value) => new NamedTermNode(name, new UnaryNode(value));

        //     return MapFrom(map, factory);
        // }

        // public UnaryEngineBuilder<T, TTermOption> MapFrom<TModel>(Func<TModel, (bool, string)> map, Func<string, string, TermNode> factory)
        // {
        //     Action<QueryFilterEngine<T>, string, TermOption, TModel> mapFrom = (QueryFilterEngine<T> terms, string name, TermOption termOption, TModel model) =>
        //     {
        //         (bool shouldMap, string value) mapResult = map(model);
        //         if (mapResult.shouldMap)
        //         {
        //             var node = termOption.MapFromFactory(name, mapResult.value);
        //             terms.TryAddOrReplace(node);
        //         }
        //     };

        //     _termOption.MapFrom = mapFrom;
        //     _termOption.MapFromFactory = factory;

        //     return this;
        // }

        public UnaryEngineBuilder<T, TTermOption> MapTo<TModel>(Action<string, TModel> map)
        {
            _termOption.MapTo = map;

            return this;
        }
    }

    public class QueryUnaryEngineBuilder<T> : UnaryEngineBuilder<T, QueryTermOption<T>> where T : class
    {
        public QueryUnaryEngineBuilder(string name, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> query) : base(new QueryTermOption<T>(name, query))
        {
        }
    }


    public class DocumentUnaryEngineBuilder<T> : UnaryEngineBuilder<T, EnumerableTermOption<T>>
    {
        public DocumentUnaryEngineBuilder(string name, Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> query) : base(new EnumerableTermOption<T>(name, query))
        {
        }
    }
}
