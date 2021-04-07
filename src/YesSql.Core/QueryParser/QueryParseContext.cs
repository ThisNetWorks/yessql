using System.Collections.Generic;
using Parlot;
using Parlot.Fluent;

namespace YesSql.Core.QueryParser
{
    public class QueryParseContext<T> : ParseContext where T : class
    {
        public QueryParseContext(IReadOnlyDictionary<string, TermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public IReadOnlyDictionary<string, TermOption<T>> TermOptions { get; }

        public TermOption<T> CurrentTermOption { get; set; }
    }
}
