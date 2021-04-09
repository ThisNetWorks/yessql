using System;
using System.Threading.Tasks;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.QueryParser.Builders
{
    public class UnaryParserBuilder<T> : OperatorParserBuilder<T> where T : class
    {
        private static Parser<OperatorNode> _parser
            => Terms.String()
                .Or(
                    Terms.NonWhiteSpace()
                )
                    .Then<OperatorNode>(static (node) => new UnaryNode(node.ToString()));

        // private TermQOption<T, IQuery<T>, FilterExecutionContext<IQuery<T>>> _termOption;
        private QueryTermOption2<T> _termOption;

        public UnaryParserBuilder(string name, Func<string, IQuery<T>, FilterExecutionContext<IQuery<T>>, ValueTask<IQuery<T>>> query)
        {
            _termOption = new QueryTermOption2<T>(name, query);
            // _termOption = new TermQOption<T, IQuery<T>, FilterExecutionContext<IQuery<T>>>(name, query);
        }

        public UnaryParserBuilder<T> AllowMultiple()
        {
            _termOption.Single = false;

            return this;
        }

        public UnaryParserBuilder<T> MapTo<TModel>(Action<string, TModel> map)
        {
            _termOption.MapTo = map;

            return this;
        }

        public UnaryParserBuilder<T> MapFrom<TModel>(Func<TModel, (bool, string)> map)
        {
            Func<string, string, TermNode> factory = (name, value) => new NamedTermNode(name, new UnaryNode(value));

            return MapFrom(map, factory);
        }

        public UnaryParserBuilder<T> MapFrom<TModel>(Func<TModel, (bool, string)> map, Func<string, string, TermNode> factory)
        {
            Action<TermList<T>, string, TermOption, TModel> mapFrom = (TermList<T> terms, string name, TermOption termOption, TModel model) =>
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

        public override (Parser<OperatorNode> Parser, QueryTermOption2<T> TermOption) Build()
            => (_parser, _termOption);
    }
}
