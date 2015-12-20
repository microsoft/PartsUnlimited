using Microsoft.AspNet.Mvc.ModelBinding;

namespace PartsUnlimited.Models
{
    public class SqlProductBuilder : IProductBuilder
    {
        public IProduct Build(ModelBindingContext context)
        {
            var product = new Product
            {
                SkuNumber = context.GetValue<string>("SkuNumber"),
                ProductId = context.GetValue<int>("ProductId"),
                RecommendationId = context.GetValue<int>("RecommendationId"),
                CategoryId = context.GetValue<int>("CategoryId"),
                Title = context.GetValue<string>("Title"),
                Price = context.GetValue<decimal>("Price"),
                SalePrice = context.GetValue<decimal>("SalePrice"),
                ProductArtUrl = context.GetValue<string>("ProductArtUrl"),
                Description = context.GetValue<string>("Description"),
                ProductDetails = context.GetValue<string>("ProductDetails"),
                Category = context.GetValue<Category>("Category")
            };
            return product;
        }
    }
}