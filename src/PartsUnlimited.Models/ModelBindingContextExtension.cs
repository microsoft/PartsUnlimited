using Microsoft.AspNet.Mvc.ModelBinding;

namespace PartsUnlimited.Models
{
    public static class ModelBindingContextExtension
    {
        public static T GetValue<T>(this ModelBindingContext context, string key)
        {
            var result = context.ValueProvider.GetValue(key);
            context.ModelState.SetModelValue(key, result);
            return (T)result.ConvertTo(typeof(T));
        }
    }
}