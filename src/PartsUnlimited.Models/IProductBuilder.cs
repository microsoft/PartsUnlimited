using Microsoft.AspNet.Mvc.ModelBinding;

namespace PartsUnlimited.Models
{
    public interface IProductBuilder
    {
        IProduct Build(ModelBindingContext context);
    }
}