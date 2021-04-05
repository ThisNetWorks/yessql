using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser
{
    // I think we might turn this into a SearchEngine
    // SearchEngine 
    // SearchManager -> this might be 
    // SearchScope
    // SearchParser

    public class TermOption<T> where T : class
    {
        public TermOption(bool oneOrMany, TermQueryOption<T> query)
        {
            OneOrMany = oneOrMany;
            Query = query;
        }

        /// <summary>
        /// Whether one or many of the specified term is allowed.
        /// </summary>
        public bool OneOrMany { get; }

        public TermQueryOption<T> Query { get; }
    }

    // This could be SearchScope
    public class TermList<T> where T : class
    {
        private Dictionary<string, TermOption<T>> _termOptions = new();


        public TermList()
        {
            Terms = new();
        }

        public TermList(List<TermNode> terms, Dictionary<string, TermOption<T>> termOptions)
        {
            Terms = terms;
            _termOptions = termOptions;
        }

        public List<TermNode> Terms { get; }

        // it's a function of termengine that decideds to add or replace.
        // not the parser itself.


        public async Task<IQuery<T>> ExecuteQueryAsync(IQuery<T> query, IServiceProvider serviceProvider) //TODO if queryexecutioncontext provided, use that.
        {
            var context = new QueryExecutionContext<T>(query, serviceProvider);

            foreach (var term in Terms)
            {
                // TODO optimize value task later.

                context.CurrentTermOption = _termOptions[term.TermName];

                var termQuery = term.BuildAsync(context);
                await termQuery.Invoke(query);
                context.CurrentTermOption = null;

            }

            return query;
        }        

        public string ToNormalizedString()
            => $"{String.Join(" ", Terms.Select(s => s.ToNormalizedString()))}";

        public override string ToString()
            => $"{String.Join(" ", Terms.Select(s => s.ToString()))}";
    }

    public class QueryExecutionContext<T> where T : class // struct?
    {
        public QueryExecutionContext(IQuery<T> query, IServiceProvider serviceProvider)
        {
            Query = query;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
        public IQuery<T> Query { get; }

        public TermOption<T> CurrentTermOption { get; set; }
    }
}
