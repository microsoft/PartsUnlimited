using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace PartsUnlimited.Models
{
    public class ProductModelBinder : IModelBinder
    {
        private readonly IProductBuilder _builder;

        public ProductModelBinder(IProductBuilder builder)
        {
            _builder = builder;
        }

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            var product = _builder.Build(bindingContext);
            return Task.FromResult(ModelBindingResult.Success("prod", product));
        }
    }
}