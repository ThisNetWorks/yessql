using System;
using System.Threading.Tasks;

namespace YesSql.Core.QueryParser.Fluent
{
    public static partial class QueryParsers
    {
        public static QueryParser<T> QueryParser<T>(params TermParserBuilder<T>[] termParsers) where T : class
            => new QueryParser<T>(termParsers);

        public static NamedTermParserBuilder<T> NamedTermParser<T>(string name, OperatorParserBuilder<T> operatorParser) where T : class
            => new NamedTermParserBuilder<T>(name, operatorParser);

        public static DefaultTermParserBuilder<T> DefaultTermParser<T>(string name, OperatorParserBuilder<T> operatorParser) where T : class
            => new DefaultTermParserBuilder<T>(name, operatorParser);

        public static UnaryParserBuilder<T> OneConditionParser<T>(Func<string, IQuery<T>, IQuery<T>> query) where T : class
            => new UnaryParserBuilder<T>(query);

        public static UnaryParserBuilder<T> OneConditionParser<T>(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> query) where T : class
            => new UnaryParserBuilder<T>(query);            

        public static BooleanParserBuilder<T> ManyConditionParser<T>(Func<string, IQuery<T>, IQuery<T>> matchQuery, Func<string, IQuery<T>, IQuery<T>> notMatchQuery) where T : class
            => new BooleanParserBuilder<T>(matchQuery, notMatchQuery);

        public static BooleanParserBuilder<T> ManyConditionParser<T>(Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> matchQuery, Func<string, IQuery<T>, QueryExecutionContext<T>, ValueTask<IQuery<T>>> notMatchQuery) where T : class
            => new BooleanParserBuilder<T>(matchQuery, notMatchQuery);            
    }
}