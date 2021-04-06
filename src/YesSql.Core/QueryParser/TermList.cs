using System;
using System.Collections;
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

    public class TermMapToOption
    {}

    public class TermMapToOption<TModel> : TermMapToOption
    {

    }

    public class TermOption
    {
        public Delegate MapTo { get; set; }
        public Delegate MapFrom { get; set; }
        public Func<string, string, TermNode> MapFromFactory { get; set; }
    }

    public class TermOption<T> : TermOption where T : class
    {
        public TermOption(TermQueryOption<T> query, bool single = true)
        {
            Query = query;
            Single = single;
        }

        /// <summary>
        /// Whether one or many of the specified term is allowed.
        /// </summary>
        public bool Single { get; }

        public TermQueryOption<T> Query { get; }
    }

    // This could be SearchScope
    public class TermList<T> : IEnumerable<TermNode> where T : class
    {
        private Dictionary<string, TermOption<T>> _termOptions;

        // This is always added to.
        private Dictionary<string, TermNode> _terms = new();


        public TermList(Dictionary<string, TermOption<T>> termOptions)
        {
            _termOptions = termOptions;
        }

        public TermList(List<TermNode> terms, Dictionary<string, TermOption<T>> termOptions)
        {
            Terms = terms;
            _termOptions = termOptions;

            foreach (var term in terms)
            {
                TryAddOrReplace(term);
            }
        }

        public bool TryAddOrReplace(TermNode term)
        {
            // Check the term options 
            if (!_termOptions.TryGetValue(term.TermName, out var termOption))
            {
                return false;
            }

            if (_terms.TryGetValue(term.TermName, out var existingTerm))
            {
                if (termOption.Single)
                {
                    // Replace
                    _terms[term.TermName] = term;
                    return true;
                }

                // Add
                if (existingTerm is CompoundTermNode compound)
                {
                    compound.Children.Add(term as TermOperationNode);
                }
                else
                {
                    // this isn't going to work when removing from list, 
                    // i.e. search says tax:a tax:b but model says just tax:b
                    // for that we need a Merge extension.
                    var newCompound = new AndTermNode(existingTerm as TermOperationNode, term as TermOperationNode);
                    _terms[term.TermName] = newCompound;
                    return true;
                }
            }

            _terms[term.TermName] = term;

            return true;
        }

        public List<TermNode> Terms { get; } = new();

        // it's a function of termengine that decideds to add or replace.
        // not the parser itself.


        public async ValueTask<IQuery<T>> ExecuteQueryAsync(IQuery<T> query, IServiceProvider serviceProvider) //TODO if queryexecutioncontext provided, use that.
        {
            var context = new QueryExecutionContext<T>(query, serviceProvider);

            foreach (var term in _terms.Values)
            {
                // TODO optimize value task later.

                context.CurrentTermOption = _termOptions[term.TermName];

                var termQuery = term.BuildAsync(context);
                await termQuery.Invoke(query);
                context.CurrentTermOption = null;
            }

            return query;
        }

        public void MapTo<TModel>(TModel model)
        {
            foreach (var term in _terms.Values)
            {
                var option = _termOptions[term.TermName];

                if (option.MapTo is Action<string, TModel> action && 
                    term is TermOperationNode operationNode && 
                    operationNode.Operation is UnaryNode node)
                {
                    action(node.Value, model);
                }
            }            
        }

        public void MapFrom<TModel>(TModel model)
        {
            foreach(var option in _termOptions)
            {
                if (option.Value.MapFrom is Action<TermList<T>, string, TermOption, TModel> mappingMethod)
                {
                    mappingMethod(this, option.Key, option.Value, model);
                }
            }
        }


        public string ToNormalizedString()
            => $"{String.Join(" ", _terms.Values.Select(s => s.ToNormalizedString()))}";

        public override string ToString()
            => $"{String.Join(" ", _terms.Values.Select(s => s.ToString()))}";

        public IEnumerator<TermNode> GetEnumerator()
            => _terms.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _terms.Values.GetEnumerator();
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

    public class QueryMapToContext<TModel>
    {
        public QueryMapToContext(TModel model)
        {
            Model = model;
        }

        // public TermOption<T> CurrentTermOption { get; set; }

        public TModel Model { get; }
        public TermOption CurrentTermOption { get; set; }
    }
}
