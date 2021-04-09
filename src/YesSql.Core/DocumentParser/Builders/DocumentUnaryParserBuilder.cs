using System;
using System.Threading.Tasks;
using Parlot.Fluent;
using YesSql.Core.QueryParser;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.DocumentParser.Builders
{
    public class DocumentUnaryParserBuilder<T> : DocumentOperatorParserBuilder<T> where T : class
    {
        private static Parser<OperatorNode> _parser
            => Terms.String()
                .Or(
                    Terms.NonWhiteSpace()
                )
                    .Then<OperatorNode>(static (node) => new UnaryNode(node.ToString()));

        private DocumentTermOption<T> _termOption;

        public DocumentUnaryParserBuilder(string name, Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> predicate)
        {
            _termOption = new DocumentTermOption<T>(name, new DocumentTermQueryOption<T>(predicate));
        }

        public DocumentUnaryParserBuilder<T> AllowMultiple()
        {
            _termOption.Single = false;

            return this;
        }

        public DocumentUnaryParserBuilder<T> MapTo<TModel>(Action<string, TModel> map)
        {
            _termOption.MapTo = map;

            return this;
        }

        public DocumentUnaryParserBuilder<T> MapFrom<TModel>(Func<TModel, (bool, string)> map)
        {
            Func<string, string, TermNode> factory = (name, value) => new NamedTermNode(name, new UnaryNode(value));

            return MapFrom(map, factory);
        }

        public DocumentUnaryParserBuilder<T> MapFrom<TModel>(Func<TModel, (bool, string)> map, Func<string, string, TermNode> factory)
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

        public override (Parser<OperatorNode> Parser, DocumentTermOption<T> TermOption) Build()
            => (_parser, _termOption);
    }
}
