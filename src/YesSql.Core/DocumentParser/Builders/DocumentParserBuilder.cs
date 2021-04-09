using System;
using System.Collections.Generic;
using System.Linq;
using YesSql.Core.QueryParser;

namespace YesSql.Core.DocumentParser.Builders
{
    public class DocumentParserBuilder<T> where T : class
    {
        private Dictionary<string, DocumentTermParserBuilder<T>> _termBuilders = new Dictionary<string, DocumentTermParserBuilder<T>>();

        public DocumentParserBuilder<T> SetTermParser(DocumentTermParserBuilder<T> builder)
        {
            _termBuilders[builder.Name] = builder;

            return this;
        }

        public DocumentParser<T> Build()
        {
            var builders = _termBuilders.Values.Select(x => x.Build());

            var parsers = builders.Select(x => x.Parser).ToArray();
            var termOptions = builders.Select(x => x.TermOption).ToDictionary(k => k.Name, v => v);

            return new DocumentParser<T>(parsers, termOptions);
        }
    }

    public static class DocumentParserBuilderExtensions
    {
        public static DocumentParserBuilder<T> WithNamedTerm<T>(this DocumentParserBuilder<T> builder, string name, Action<DocumentNamedTermParserBuilder<T>> action) where T : class
        {
            var parserBuilder = new DocumentNamedTermParserBuilder<T>(name);
            action(parserBuilder);

            builder.SetTermParser(parserBuilder);
            return builder;
        }

        // public static QueryParserBuilder<T> WithDefaultTerm<T>(this QueryParserBuilder<T> builder, string name, Action<DefaultTermParserBuilder<T>> action) where T : class
        // {
        //     var parserBuilder = new DefaultTermParserBuilder<T>(name);
        //     action(parserBuilder);

        //     builder.SetTermParser(parserBuilder);
        //     return builder;
        // }        
    }
}
