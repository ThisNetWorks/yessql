using Microsoft.AspNetCore.Mvc.ModelBinding;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YesSql.Core.FilterEngines;
using YesSql.Core.FilterEngines.Builders;

namespace YesSql.Search.ModelBinding
{
    public class TermModelBinder<T> : IModelBinder where T : class
    {
        private readonly IQueryEngine<T> _parser;

        public TermModelBinder(IQueryEngine<T> parser)
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
                bindingContext.Result = ModelBindingResult.Success(_parser.Parse(String.Empty));

                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(_parser.Parse(String.Empty));
                
                return Task.CompletedTask;
            }

            var termList = _parser.Parse(value);

            bindingContext.Result = ModelBindingResult.Success(termList);
            
            return Task.CompletedTask;
        }
    }
}