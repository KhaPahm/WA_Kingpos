using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System.Globalization;

namespace WA_Kingpos.Data
{
    public class CustomDateModelBinder : IModelBinder
    {

        public CustomDateModelBinder()
        {

        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != ValueProviderResult.None)
            {
                var value = valueProviderResult.FirstValue;
                if (DateTime.TryParse(value?.Trim(), CultureInfo.GetCultureInfo("en-GB"), DateTimeStyles.None, out var date))
                {
                    bindingContext.Result = ModelBindingResult.Success(date);
                    return Task.CompletedTask;
                }
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid date format. Use dd/MM/yyyy.");
            }
            return Task.CompletedTask;
        }
    }

    public class CustomDateModelBinderProvider : IModelBinderProvider
    {

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
            {
                return new BinderTypeModelBinder(typeof(CustomDateModelBinder));
            }
            return null;
        }
    }
}
