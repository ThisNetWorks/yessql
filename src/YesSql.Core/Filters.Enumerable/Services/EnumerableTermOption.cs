using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Filters.Abstractions.Services;

namespace OrchardCore.Filters.Enumerable.Services
{
    public class EnumerableTermOption<T> : TermOption
    {
        public EnumerableTermOption(string name, Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> matchPredicate) : base(name)
        {
            MatchPredicate = matchPredicate;
        }

        public EnumerableTermOption(string name, Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> matchPredicate, Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> notMatchPredicate)
            : base(name)
        {
            MatchPredicate = matchPredicate;
            NotMatchPredicate = notMatchPredicate;
        }


        public Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> MatchPredicate { get; }
        public Func<string, IEnumerable<T>, EnumerableExecutionContext<T>, ValueTask<IEnumerable<T>>> NotMatchPredicate { get; }
    }
}