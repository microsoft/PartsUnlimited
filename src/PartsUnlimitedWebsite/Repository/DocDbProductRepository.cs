using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PartsUnlimited.Search;

namespace PartsUnlimited.Repository
{

    public class DocDbProductRepository : IProductRepository
    {
        public Task<IEnumerable<dynamic>> Search(ProductSearchCriteria searchCriteria)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<dynamic>> LoadSaleProducts()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<dynamic>> LoadAllProducts()
        {
            throw new System.NotImplementedException();
        }

        public Task<IQueryable<dynamic>> LoadQueryableProducts(object order)
        {
            throw new System.NotImplementedException();
        }

        public Task Add(dynamic product, CancellationToken requestAborted)
        {
            throw new System.NotImplementedException();
        }

        public Task<dynamic> GetLatestProduct()
        {
            throw new System.NotImplementedException();
        }

        public Task Delete(dynamic product, CancellationToken requestAborted)
        {
            throw new System.NotImplementedException();
        }

        public Task<dynamic> Load(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}