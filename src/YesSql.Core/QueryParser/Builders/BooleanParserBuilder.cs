using System;
using System.Threading.Tasks;
using static Parlot.Fluent.Parsers;
using Parlot.Fluent;

namespace YesSql.Core.QueryParser.Builders
{

    public class BooleanParserBuilder<T> : OperatorParserBuilder<T> where T : class
    {
        private TermOption<T> _termOption;
        private Parser<OperatorNode> _parser;

        private BooleanParserBuilder()
        {
            var OperatorNode = Deferred<OperatorNode>();

            var AndOperator = Terms.Text("AND")
                .Or(
                    Literals.Text("&&")
                );

            var NotOperator = Terms.Text("NOT")
                .Or(
                    Literals.Text("!")
                );

            var OrTextOperators = Terms.Text("OR")
                .Or(
                    Terms.Text("||")
                );

            // Operators that need to be NOT next when the default OR ' ' operator is found.
            var NotOrOperators = OneOf(AndOperator, NotOperator, OrTextOperators);

            // Default operator.
            var OrOperator = Literals.Text(" ").AndSkip(Not(NotOrOperators))// With this is is now catching everything.
                .Or(
                    OrTextOperators
                );

            var GroupNode = Between(Terms.Char('('), OperatorNode, Terms.Char(')'))
                .Then<OperatorNode>(static x => new GroupNode(x));

            var SingleNode = Terms.String() // A term name is never enclosed in strings.
                .Or(
                    // This must be aborted when it is consuming the next term.
                    Terms.Identifier().AndSkip(Not(Literals.Char(':'))) // TODO when this is NonWhiteSpace it sucks up paranthese. Will Identifier catch accents, i.e. multilingual.
                )
                    .Then<OperatorNode>(static (node) => new UnaryNode(node.ToString()));

            var Primary = SingleNode.Or(GroupNode);

            var UnaryNode = NotOperator.And(Primary)
                .Then<OperatorNode>(static (node) =>
                {
                    // mutate with the neg query.
                    var unaryNode = node.Item2 as UnaryNode;

                    // TODO test what actually happens when just using NOT foo
                    return new NotUnaryNode(node.Item1, new UnaryNode(unaryNode.Value, false));
                })
                .Or(Primary);

            var AndNode = UnaryNode.And(ZeroOrMany(AndOperator.And(UnaryNode)))
                .Then<OperatorNode>(static node =>
                {
                    // unary
                    var result = node.Item1;

                    foreach (var op in node.Item2)
                    {
                        result = new AndNode(result, op.Item2, op.Item1);
                    }

                    return result;
                });

            OperatorNode.Parser = AndNode.And(ZeroOrMany(NotOperator.Or(OrOperator).And(AndNode)))
               .Then<OperatorNode>(static (node) =>
               {
                    static NotNode CreateNotNode(OperatorNode result, (string, OperatorNode) op)
                        => new NotNode(result, new UnaryNode(((UnaryNode)op.Item2).Value, false), op.Item1);
                    
                    static OrNode CreateOrNode(OperatorNode result, (string, OperatorNode) op)
                        => new OrNode(result, op.Item2, op.Item1);

                    // unary
                    var result = node.Item1;

                    foreach (var op in node.Item2)
                    {
                        result = op.Item1 switch
                        {
                            "NOT" => CreateNotNode(result, op),
                            "!" => CreateNotNode(result, op),
                            "OR" => CreateOrNode(result, op),
                            "||" => CreateOrNode(result, op),
                            " " => CreateOrNode(result, op),
                            _ => null
                        };
                    }

                   return result;
               });

            _parser = OperatorNode;

        }

        public BooleanParserBuilder(
            string name, 
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery,
            Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery) : this()
        {
            _termOption = new TermOption<T>(name, new TermQueryOption<T>(matchQuery, notMatchQuery), single: true);
        }

        public override (Parser<OperatorNode> Parser, TermOption<T> TermOption) Build()
            => (_parser, _termOption);
    }
}