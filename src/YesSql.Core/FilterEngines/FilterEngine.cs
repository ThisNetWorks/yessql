// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using YesSql.Core.FilterEngines.Visitors;

// namespace YesSql.Core.FilterEngines
// {
//     public abstract class FilterEngine<T, TTermOption> : IEnumerable<TermNode> where TTermOption : TermOption
//     {
//         protected IReadOnlyDictionary<string, TTermOption> _termOptions;

//         protected Dictionary<string, TermNode> _terms = new();

//         // this would become part of the engine. which may well be passed into the TermList

//         // Yup that's engine stuff.

//         public FilterEngine(IReadOnlyDictionary<string, TTermOption> termOptions)
//         {
//             _termOptions = termOptions;
//         }

//         public FilterEngine(List<TermNode> terms, IReadOnlyDictionary<string, TTermOption> termOptions)
//         {
//             _termOptions = termOptions;

//             foreach (var term in terms)
//             {
//                 TryAddOrReplace(term);
//             }
//         }

//         public void MapTo<TModel>(TModel model)
//         {
//             foreach (var term in _terms.Values)
//             {
//                 var option = _termOptions[term.TermName];

//                 if (option.MapTo is Action<string, TModel> action &&
//                     term is TermOperationNode operationNode &&
//                     operationNode.Operation is UnaryNode node)
//                 {
//                     action(node.Value, model);
//                 }
//             }
//         }

//         public string ToNormalizedString()
//             => $"{String.Join(" ", _terms.Values.Select(s => s.ToNormalizedString()))}";

//         public override string ToString()
//             => $"{String.Join(" ", _terms.Values.Select(s => s.ToString()))}";

//         public bool TryAddOrReplace(TermNode term)
//         {
//             // Check the term options 
//             if (!_termOptions.TryGetValue(term.TermName, out var termOption))
//             {
//                 return false;
//             }

//             if (_terms.TryGetValue(term.TermName, out var existingTerm))
//             {
//                 if (termOption.Single)
//                 {
//                     // Replace
//                     _terms[term.TermName] = term;
//                     return true;
//                 }

//                 // Add
//                 if (existingTerm is CompoundTermNode compound)
//                 {
//                     compound.Children.Add(term as TermOperationNode);
//                 }
//                 else
//                 {
//                     // TODO this isn't going to work when removing from list, 
//                     // i.e. search says tax:a tax:b but model says just tax:b
//                     // for that we need a Merge extension.
//                     var newCompound = new AndTermNode(existingTerm as TermOperationNode, term as TermOperationNode);
//                     _terms[term.TermName] = newCompound;
//                     return true;
//                 }
//             }

//             _terms[term.TermName] = term;

//             return true;
//         }

//         public IEnumerator<TermNode> GetEnumerator()
//             => _terms.Values.GetEnumerator();

//         IEnumerator IEnumerable.GetEnumerator()
//             => _terms.Values.GetEnumerator();
//     }

//     public class QueryFilterEngine<T> : FilterEngine<T, QueryTermOption<T>> where T : class
//     {
//         public QueryFilterEngine(IReadOnlyDictionary<string, QueryTermOption<T>> termOptions) : base(termOptions)
//         { }


//         public QueryFilterEngine(List<TermNode> terms, IReadOnlyDictionary<string, QueryTermOption<T>> termOptions) : base(terms, termOptions)
//         { }

//         public void MapFrom<TModel>(TModel model)
//         {
//             foreach (var option in _termOptions)
//             {
//                 if (option.Value.MapFrom is Action<QueryFilterEngine<T>, string, TermOption, TModel> mappingMethod)
//                 {
//                     mappingMethod(this, option.Key, option.Value, model);
//                 }
//             }
//         }

//         public async ValueTask<IQuery<T>> ExecuteAsync(IQuery<T> query, IServiceProvider serviceProvider) //TODO if queryexecutioncontext provided, use that.
//         {
//             var context = new QueryExecutionContext<T>(query, serviceProvider);

//             var visitor = new QueryFilterVisitor<T>();

//             foreach (var term in _terms.Values)
//             {
//                 // TODO optimize value task later.

//                 context.CurrentTermOption = _termOptions[term.TermName];

//                 var termQuery = visitor.Visit(term, context);
//                 query = await termQuery.Invoke(query);
//                 context.CurrentTermOption = null;
//             }

//             return query;
//         }
//     }

//     public class EnumerableFilterEngine<T> : FilterEngine<T, EnumerableTermOption<T>>
//     {
//         public EnumerableFilterEngine(IReadOnlyDictionary<string, EnumerableTermOption<T>> termOptions) : base(termOptions)
//         { }


//         public EnumerableFilterEngine(List<TermNode> terms, IReadOnlyDictionary<string, EnumerableTermOption<T>> termOptions) : base(terms, termOptions)
//         { }
        
//         public void MapFrom<TModel>(TModel model)
//         {
//             foreach (var option in _termOptions)
//             {
//                 if (option.Value.MapFrom is Action<EnumerableFilterEngine<T>, string, TermOption, TModel> mappingMethod)
//                 {
//                     mappingMethod(this, option.Key, option.Value, model);
//                 }
//             }
//         }

//         public async ValueTask<IEnumerable<T>> ExecuteAsync(IEnumerable<T> source, IServiceProvider serviceProvider) //TODO if queryexecutioncontext provided, use that.
//         {
//             var context = new EnumerableExecutionContext<T>(source, serviceProvider);
//             var visitor = new EnumerableFilterVisitor<T>();

//             foreach (var term in _terms.Values)
//             {
//                 // TODO optimize value task later.

//                 context.CurrentTermOption = _termOptions[term.TermName];

//                 var termQuery = visitor.Visit(term, context);
//                 source = await termQuery.Invoke(source);
//                 context.CurrentTermOption = null;
//             }

//             return source;
//         }

//     }


//     public class QueryMapToContext<TModel>
//     {
//         public QueryMapToContext(TModel model)
//         {
//             Model = model;
//         }

//         public TModel Model { get; }
//         public TermOption CurrentTermOption { get; set; }
//     }
// }
