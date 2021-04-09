using System;

namespace YesSql.Core.DocumentParser
{
    public class DocumentExecutionContext<T> where T : class 
    {
        public DocumentExecutionContext(T enumerable, IServiceProvider serviceProvider)
        {
            Document = enumerable;
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }
        public T Document { get; }

        public DocumentTermOption<T> CurrentTermOption { get; set; }
    }
}
