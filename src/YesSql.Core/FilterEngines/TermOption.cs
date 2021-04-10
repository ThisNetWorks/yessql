using System;
using System.Threading.Tasks;

namespace YesSql.Core.FilterEngines
{
    public abstract class TermOption
    {
        public TermOption(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// Whether one or many of the specified term is allowed.
        /// </summary>
        public bool Single { get; set; } = true;

        public Delegate MapTo { get; set; }
        public Delegate MapFrom { get; set; }
        public Func<string, string, TermNode> MapFromFactory { get; set; }
    }

    public class QueryTermOption<T> : TermOption where T : class
    {
        public QueryTermOption(string name, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchPredicate) : base(name)
        {
            MatchPredicate = matchPredicate;
        }

        public QueryTermOption(string name, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchPredicate, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchPredicate)
            : base(name)
        {
            MatchPredicate = matchPredicate;
            NotMatchPredicate = notMatchPredicate;
        }
        public Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> MatchPredicate { get; }
        public Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> NotMatchPredicate { get; }
    }

    public class EnumerableTermOption<T> : TermOption where T : class
    {
        public EnumerableTermOption(string name, Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> matchPredicate) : base(name)
        {
            MatchPredicate = matchPredicate;
        }

        public EnumerableTermOption(string name, Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> matchPredicate, Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> notMatchPredicate)
            : base(name)
        {
            MatchPredicate = matchPredicate;
            NotMatchPredicate = notMatchPredicate;
        }


        public Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> MatchPredicate { get; }
        public Func<string, T, EnumerableExecutionContext<T>, ValueTask<T>> NotMatchPredicate { get; }
    }


}
