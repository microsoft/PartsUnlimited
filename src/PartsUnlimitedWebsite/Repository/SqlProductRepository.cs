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
    public class SqlProductRepository : IProductRepository
    {
        private readonly IPartsUnlimitedContext _context;

        public SqlProductRepository(IPartsUnlimitedContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<dynamic>> Search(ProductSearchCriteria searchCriteria)
        {
            var lowercaseQuery = searchCriteria.TitleSearch.ToLower();
            var q = _context.Products
                .Where(p => p.Title.ToLower().Contains(lowercaseQuery));

            return await q.ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<dynamic>> LoadSaleProducts()
        {
            return await _context.Products.Where(p => p.Price != p.SalePrice).ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<dynamic>> LoadAllProducts()
        {
            return await _context.Products.ToAsyncEnumerable().ToList();
        }

        public async Task Add(dynamic product, CancellationToken requestAborted)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync(requestAborted);
        }

        public Task<dynamic> GetLatestProduct()
        {
            Product latestProduct = _context.Products.OrderByDescending(a => a.Created).FirstOrDefault();
            if ((latestProduct != null) && ((latestProduct.Created - DateTime.UtcNow).TotalDays <= 2))
            {
                return Task.FromResult<dynamic>(latestProduct);
            }

            return null;
        }

        public async Task Delete(dynamic product, CancellationToken requestAborted)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(requestAborted);
        }

        public Task<dynamic> Load(int id)
        {
            var product = _context.Products.Single(p => p.ProductId == id);
            return Task.FromResult<dynamic>(product);
        }
    }
}