// using Parlot.Fluent;

// namespace YesSql.Core.FilterEngines.Builders
// {
//     public abstract class TermEngineBuilder<T, TTermOption>  where TTermOption : TermOption
//     {
//         public TermEngineBuilder(string name)
//         {
//             Name = name;
//         }

//         public string Name { get; }
//         public bool Single { get; }

//         protected OperatorEngineBuilder<T, TTermOption> _operatorParser;

//         public TermEngineBuilder<T, TTermOption> SetOperator(OperatorEngineBuilder<T, TTermOption> operatorParser)
//         {
//             _operatorParser = operatorParser;

//             return this;
//         }

//         public abstract (Parser<TermNode> Parser, TTermOption TermOption) Build();
//     }
// }
