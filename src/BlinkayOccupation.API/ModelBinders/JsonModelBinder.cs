using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace BlinkayOccupation.API.ModelBinders
{
    public class JsonModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var key = bindingContext.FieldName; // el nombre del campo, por ejemplo "arguments"
            var valueProviderResult = bindingContext.ValueProvider.GetValue(key);

            if (valueProviderResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var rawValue = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(rawValue))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            try
            {
                var result = JsonConvert.DeserializeObject(rawValue, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonException)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid JSON.");
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
    }

}
