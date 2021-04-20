using System.Collections.Generic;
using Parlot;
using Parlot.Fluent;

namespace OrchardCore.Filters.Enumerable.Services
{
    public class EnumerableParseContext<T> : ParseContext
    {
        public EnumerableParseContext(IReadOnlyDictionary<string, EnumerableTermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public IReadOnlyDictionary<string, EnumerableTermOption<T>> TermOptions { get; }
    }
}
