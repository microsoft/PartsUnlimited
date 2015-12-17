using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PartsUnlimited.Models;
using PartsUnlimited.Search;

namespace PartsUnlimited.Repository
{
    public interface IProductRepository  : IProductLoader
    {
        Task<IEnumerable<dynamic>> Search(ProductSearchCriteria searchCriteria);
        Task<IEnumerable<dynamic>> LoadSaleProducts();
        Task<IEnumerable<dynamic>> LoadAllProducts();
        Task Add(dynamic product, CancellationToken requestAborted);
        Task<dynamic> GetLatestProduct();
        Task Delete(dynamic product, CancellationToken requestAborted);
    }
}
