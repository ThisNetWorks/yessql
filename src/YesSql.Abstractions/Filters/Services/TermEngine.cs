using System;
using System.Collections.Generic;
using OrchardCore.Filters.Nodes;
using Parlot;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;

namespace OrchardCore.Filters.Services
{
    // TODO this as a abstract class is not that useful.
    // IFilterEngine.
    public abstract class TermEngine<T, TTermList, TTerm> where TTermList : FilterResult<T, TTerm> where TTerm : TermOption
    {
        protected Dictionary<string, TTerm> _termOptions;
        protected Parser<List<TermNode>> _termNodeParser;
        protected abstract Parser<TTermList> _parser { get; }

        public TermEngine(Parser<TermNode>[] termParsers, Dictionary<string, TTerm> termOptions)
        {
            _termOptions = termOptions;

            var terms = OneOf(termParsers);

            _termNodeParser = ZeroOrMany(terms);
        }


        public TTermList Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return Create(_termOptions);
            }

            var context = GetContext(new Scanner(text));

            ParseResult<TTermList> result = default(ParseResult<TTermList>);
            if (_parser.Parse(context, ref result))
            {
                return result.Value;
            }
            else
            {
                return Create(_termOptions);
            }
        }

        protected abstract TTermList Create(Dictionary<string, TTerm> termOptions);
        protected abstract ParseContext GetContext(Scanner scanner);
    }
}
