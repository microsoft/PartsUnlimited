namespace PartsUnlimited.Models
{
    public interface IProduct
    {
        string SkuNumber { get; }
        int ProductId { get; }
        int RecommendationId { get; }
        int CategoryId { get; }
        string Title { get; }
        decimal Price { get; }
        decimal SalePrice { get; }
        string ProductArtUrl { get; }
        Category Category { get; set; }
        string Description { get; }
        string ProductDetails { get; }
    }
}