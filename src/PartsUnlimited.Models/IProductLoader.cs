using System.Threading.Tasks;

namespace PartsUnlimited.Models
{
    public interface IProductLoader
    {
        Task<IProduct> Load(int productId);
    }
}