using System.Threading.Tasks;

namespace PartsUnlimited.Models
{
    public interface IDataSeeder
    {
        Task Seed(SampleData data);
    }
}