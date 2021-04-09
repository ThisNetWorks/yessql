using System;
using System.Collections.Generic;
using Parlot;
using Parlot.Fluent;
using YesSql.Core.QueryParser;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.DocumentParser
{
    public interface IDocumentParser<T> where T : class
    {
        DocumentTermList<T> Parse(string text);
    }

    public class DocumentParser<T> : IDocumentParser<T> where T : class
    {
        private Dictionary<string, DocumentTermOption<T>> _termOptions;
        private Parser<DocumentTermList<T>> _parser;

        public DocumentParser(Parser<TermNode>[] termParsers, Dictionary<string, DocumentTermOption<T>> termOptions)
        {
            _termOptions = termOptions;

            var terms = OneOf(termParsers);

            _parser = ZeroOrMany(terms)
                    .Then(static (context, terms) => 
                    {
                        var ctx = (DocumentParseContext<T>)context;

                        return new DocumentTermList<T>(terms, ctx.TermOptions);
                    });                    
        }

        public DocumentTermList<T> Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return new DocumentTermList<T>(_termOptions);
            }

            var context = new DocumentParseContext<T>(_termOptions, new Scanner(text));

            ParseResult<DocumentTermList<T>> result = default(ParseResult<DocumentTermList<T>>);
            if (_parser.Parse(context, ref result))
            {
                return result.Value;
            }
            else
            {
                return new DocumentTermList<T>(_termOptions);
            }
        }
    }
}
