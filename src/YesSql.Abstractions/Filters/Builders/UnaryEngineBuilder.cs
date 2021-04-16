using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrchardCore.Filters.Nodes;
using OrchardCore.Filters.Services;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;

namespace OrchardCore.Filters.Builders
{
    public abstract class UnaryEngineBuilder<T, TTermOption> : OperatorEngineBuilder<T, TTermOption> where TTermOption : TermOption
    {
        protected TTermOption _termOption;
        private static Parser<OperatorNode> _parser
            => Terms.String()
                .Or(
                    Terms.NonWhiteSpace()
                )
                    .Then<OperatorNode>((node) => new UnaryNode(node.ToString()));

        public UnaryEngineBuilder(TTermOption termOption)
        {
            _termOption = termOption;
        }

        public UnaryEngineBuilder<T, TTermOption> AllowMultiple()
        {
            _termOption.Single = false;

            return this;
        }

        public override (Parser<OperatorNode> Parser, TTermOption TermOption) Build()
            => (_parser, _termOption);


    }
}
