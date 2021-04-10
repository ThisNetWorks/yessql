using System.Collections.Generic;
using Parlot;
using Parlot.Fluent;

namespace YesSql.Core.FilterEngines
{
    public class QueryParseContext<T> : ParseContext where T : class
    {
        public QueryParseContext(IReadOnlyDictionary<string, QueryTermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public IReadOnlyDictionary<string, QueryTermOption<T>> TermOptions { get; }
    }

    public class EnumerableParseContext<T> : ParseContext where T : class
    {
        public EnumerableParseContext(IReadOnlyDictionary<string, EnumerableTermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public IReadOnlyDictionary<string, EnumerableTermOption<T>> TermOptions { get; }
    }
}
