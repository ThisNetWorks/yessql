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
        public DocumentTermOption(string name, DocumentTermQueryOption<T> query) : base(name)
        {
            Query = query;
        }

        public DocumentTermQueryOption<T> Query { get; }
    }
}
