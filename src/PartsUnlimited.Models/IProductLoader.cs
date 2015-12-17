using System.Threading.Tasks;

namespace PartsUnlimited.Models
{
    public interface IProductLoader
    {
        Task<dynamic> Load(int productId);
    }
}