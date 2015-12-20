using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using PartsUnlimited.Areas.Admin.Controllers;
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

        public async Task<IEnumerable<IProduct>> Search(ProductSearchCriteria searchCriteria)
        {
            var lowercaseQuery = searchCriteria.TitleSearch.ToLower();
            var q = _context.Products
                .Where(p => p.Title.ToLower().Contains(lowercaseQuery));

            return await q.ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<IProduct>> LoadSaleProducts()
        {
            return await _context.Products.Where(p => p.Price != p.SalePrice).ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts()
        {
            return await _context.Products.ToAsyncEnumerable().ToList();
        }

        public async Task Add(IProduct product, CancellationToken requestAborted)
        {
            _context.Products.Add((Product)product);
            await _context.SaveChangesAsync(requestAborted);
        }

        public Task<IProduct> GetLatestProduct()
        {
            Product latestProduct = _context.Products.OrderByDescending(a => a.Created).FirstOrDefault();
            if ((latestProduct != null) && ((latestProduct.Created - DateTime.UtcNow).TotalDays <= 2))
            {
                return Task.FromResult<IProduct>(latestProduct);
            }

            return null;
        }

        public Task Delete(IProduct product, CancellationToken requestAborted)
        {
            _context.Products.Remove((Product)product);
            return _context.SaveChangesAsync(requestAborted);
        }

        public async Task<IEnumerable<IProduct>> LoadProductsForCategory(int categoryId)
        {
            return await _context.Products.Where(a => a.CategoryId == categoryId).ToAsyncEnumerable().ToList();
        }

        public async Task<IEnumerable<IProduct>> LoadProductsFromRecommendation(IEnumerable<string> recommendedProductIds)
        {
            var productTasks = recommendedProductIds
               .Select(item => _context.Products.SingleOrDefaultAsync(c => c.RecommendationId == Convert.ToInt32(item)))
               .ToList();

            return await Task.WhenAll(productTasks);
        }

        public async Task<IEnumerable<IProduct>> LoadTopSellingProducts(int count)
        {
            // TODO [EF] We don't query related data as yet, so the OrderByDescending isn't doing anything
            return await _context.Products
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<IProduct>> LoadNewProducts(int count)
        {
            return await _context.Products
               .OrderByDescending(a => a.Created)
               .Take(count)
               .ToListAsync();
        }

        public async Task<IEnumerable<IProduct>> LoadAllProducts(SortField sortField, SortDirection sortDirection)
        {
            // TODO [EF] Swap to native support for loading related data when available

            IQueryable<Product> products = from product in _context.Products
                                           join category in _context.Categories on product.CategoryId equals category.CategoryId
                                           select new Product()
                                           {
                                               ProductArtUrl = product.ProductArtUrl,
                                               ProductId = product.ProductId,
                                               CategoryId = product.CategoryId,
                                               Price = product.Price,
                                               Title = product.Title,
                                               Category = new Category()
                                               {
                                                   CategoryId = product.CategoryId,
                                                   Name = category.Name
                                               }
                                           };

            var sortedList = Sort(products, sortField, sortDirection);
            return await sortedList.ToListAsync();
        }

        public Task Save(IProduct product, CancellationToken token)
        {
            _context.Entry(product).State = EntityState.Modified;
            return _context.SaveChangesAsync(token);
        }

        private IQueryable<IProduct> Sort(IQueryable<Product> products, SortField sortField, SortDirection sortDirection)
        {
            if (sortField == SortField.Name)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Category.Name);
                }

                return products.OrderByDescending(o => o.Category.Name);
            }

            if (sortField == SortField.Price)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Price);
                }

                return products.OrderByDescending(o => o.Price);
            }

            if (sortField == SortField.Title)
            {
                if (sortDirection == SortDirection.Up)
                {
                    return products.OrderBy(o => o.Title);
                }

                return products.OrderByDescending(o => o.Title);
            }

            // Should not reach here, but return products for compiler
            return products;
        }

        public Task<IProduct> Load(int id)
        {
            var product = _context.Products.Single(p => p.ProductId == id);
            return Task.FromResult<IProduct>(product);
        }
    }
}