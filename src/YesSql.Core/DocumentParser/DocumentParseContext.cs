using System.Collections.Generic;
using Parlot;
using Parlot.Fluent;

namespace YesSql.Core.DocumentParser
{
    public class DocumentParseContext<T> : ParseContext where T : class
    {
        public DocumentParseContext(IReadOnlyDictionary<string, DocumentTermOption<T>> termOptions, Scanner scanner, bool useNewLines = false) : base(scanner, useNewLines)
        {
            TermOptions = termOptions;
        }

        public IReadOnlyDictionary<string, DocumentTermOption<T>> TermOptions { get; }

        public DocumentTermOption<T> CurrentTermOption { get; set; }
    }
}
