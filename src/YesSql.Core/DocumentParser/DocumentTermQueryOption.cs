using Parlot;
using Parlot.Fluent;
using System;
using System.Threading.Tasks;
using static Parlot.Fluent.Parsers;

namespace YesSql.Core.DocumentParser
{
    public class DocumentTermQueryOption<T>  where T : class
    {
        public DocumentTermQueryOption(Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> matchQuery)
        {
            MatchQuery = matchQuery;
        }

        public DocumentTermQueryOption(Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> matchQuery, Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> notMatchQuery)
        {
            MatchQuery = matchQuery;
            NotMatchQuery = notMatchQuery;
        }

        public Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> MatchQuery { get; }

        public Func<string, T, DocumentExecutionContext<T>, ValueTask<T>> NotMatchQuery { get; }
    }
}
