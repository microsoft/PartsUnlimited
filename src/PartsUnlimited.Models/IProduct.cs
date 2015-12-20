namespace PartsUnlimited.Models
{
    public interface IProduct
    {
        string SkuNumber { get; set; }
        int ProductId { get; set; }
        int RecommendationId { get; set; }
        int CategoryId { get; set; }
        string Title { get; set; }
        decimal Price { get; set; }
        decimal SalePrice { get; set; }
        string ProductArtUrl { get; set; }
        Category Category { get; set; }
        string Description { get; set; }
        string ProductDetails { get; set; }
    }
}