using Microsoft.AspNetCore.Mvc.ModelBinding;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql.Core.QueryParser;

namespace YesSql.Search.ModelBinding
{
    public class TermModelBinder<T> : IModelBinder where T : class
    {
        private readonly IQueryParser<T> _parser;

        public TermModelBinder(IQueryParser<T> parser)
        {
            _parser = parser;
        }
       

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name q=
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Success(new TermList<T>());

                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(new TermList<T>());
                
                return Task.CompletedTask;
            }

            var termList = _parser.Parse(value);

            bindingContext.Result = ModelBindingResult.Success(termList);
            
            return Task.CompletedTask;
        }
    }
}