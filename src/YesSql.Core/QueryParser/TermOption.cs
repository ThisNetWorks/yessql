using System;

namespace YesSql.Core.QueryParser
{
    public abstract class TermOption
    {
        public TermOption(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <summary>
        /// Whether one or many of the specified term is allowed.
        /// </summary>
        public bool Single { get; set; }

        public Delegate MapTo { get; set; }
        public Delegate MapFrom { get; set; }
        public Func<string, string, TermNode> MapFromFactory { get; set; }
    }

    public class TermOption<T> : TermOption where T : class
    {
        public TermOption(string name, TermQueryOption<T> query) : base(name)
        {
            Query = query;
        }

        public TermQueryOption<T> Query { get; }
    }
}
