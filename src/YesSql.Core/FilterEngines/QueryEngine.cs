// using System;
// using System.Collections.Generic;
// using Parlot;
// using Parlot.Fluent;
// using static Parlot.Fluent.Parsers;

// namespace YesSql.Core.FilterEngines
// {
//     public interface IQueryEngine<T> where T : class
//     {
//         QueryFilterEngine<T> Parse(string text);
//     }

//     // TODO this as a abstract class is not that useful.
//     public abstract class TermEngine<T, TTermList, TTerm> where TTermList : FilterEngine<T, TTerm> where TTerm : TermOption
//     {
//         protected Dictionary<string, TTerm> _termOptions;
//         protected Parser<List<TermNode>> _termNodeParser;
//         protected abstract Parser<TTermList> _parser { get; }

//         public TermEngine(Parser<TermNode>[] termParsers, Dictionary<string, TTerm> termOptions)
//         {
//             _termOptions = termOptions;

//             var terms = OneOf(termParsers);

//             _termNodeParser = ZeroOrMany(terms);
//         }


//         public TTermList Parse(string text)
//         {
//             if (String.IsNullOrEmpty(text))
//             {
//                 return Create(_termOptions);
//             }

//             var context = GetContext(new Scanner(text));

//             ParseResult<TTermList> result = default(ParseResult<TTermList>);
//             if (_parser.Parse(context, ref result))
//             {
//                 return result.Value;
//             }
//             else
//             {
//                 return Create(_termOptions);
//             }
//         }

//         protected abstract TTermList Create(Dictionary<string, TTerm> termOptions);
//         protected abstract ParseContext GetContext(Scanner scanner);
//     }

//     public class QueryEngine<T> : TermEngine<T, QueryFilterEngine<T>, QueryTermOption<T>>, IQueryEngine<T> where T : class
//     {
//         public QueryEngine(Parser<TermNode>[] termParsers, Dictionary<string, QueryTermOption<T>> termOptions) : base(termParsers, termOptions)
//         { }

//         protected override Parser<QueryFilterEngine<T>> _parser
//             => _termNodeParser
//                 .Then(static (context, terms) =>
//                 {
//                     var ctx = (QueryParseContext<T>)context;

//                     return new QueryFilterEngine<T>(terms, ctx.TermOptions);
//                 });

//         protected override QueryFilterEngine<T> Create(Dictionary<string, QueryTermOption<T>> termOptions)
//             => new QueryFilterEngine<T>(_termOptions);

//         protected override ParseContext GetContext(Scanner scanner)
//             => new QueryParseContext<T>(_termOptions, scanner);
//     }

//     public class EnumerableEngine<T> : TermEngine<T, EnumerableFilterEngine<T>, EnumerableTermOption<T>>
//     {
//         public EnumerableEngine(Parser<TermNode>[] termParsers, Dictionary<string, EnumerableTermOption<T>> termOptions) : base(termParsers, termOptions)
//         { }

//         protected override Parser<EnumerableFilterEngine<T>> _parser
//             => _termNodeParser
//                 .Then(static (context, terms) =>
//                     {
//                         var ctx = (EnumerableParseContext<T>)context;

//                         return new EnumerableFilterEngine<T>(terms, ctx.TermOptions);
//                     });

//         protected override EnumerableFilterEngine<T> Create(Dictionary<string, EnumerableTermOption<T>> termOptions)
//             => new EnumerableFilterEngine<T>(_termOptions);

//         protected override ParseContext GetContext(Scanner scanner)
//             => new EnumerableParseContext<T>(_termOptions, scanner);
//     }
// }
