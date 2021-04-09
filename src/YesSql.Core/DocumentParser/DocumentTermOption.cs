using System;
using YesSql.Core.QueryParser;

namespace YesSql.Core.DocumentParser
{
    public abstract class DocumentTermOption
    {
        public DocumentTermOption(string name)
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

    public class DocumentTermOption<T> : DocumentTermOption where T : class
    {
        // TODO the thought might be to bring these two together. they were original seperate for a reason.
        // the reason is gone.
        public DocumentTermOption(string name, QueryTermDocumentOption<T> query) : base(name)
        {
            Query = query;
        }

        public QueryTermDocumentOption<T> Query { get; }
    }
}
