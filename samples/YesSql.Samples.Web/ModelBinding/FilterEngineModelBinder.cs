using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.Filters.Query;
using OrchardCore.Filters.Query.Services;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YesSql.Search.ModelBinding
{
    public abstract class FilterEngineModelBinder<TResult> : IModelBinder
    {
        public abstract TResult Parse(string text);

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
                bindingContext.Result = ModelBindingResult.Success(Parse(String.Empty));

                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(Parse(String.Empty));
                
                return Task.CompletedTask;
            }

            var filterResult = Parse(value);

            bindingContext.Result = ModelBindingResult.Success(filterResult);
            
            return Task.CompletedTask;
        }
    }
}