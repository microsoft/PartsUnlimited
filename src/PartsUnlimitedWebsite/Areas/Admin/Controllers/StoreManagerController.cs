// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Data.Entity;
using PartsUnlimited.Cache;
using PartsUnlimited.Hubs;
using PartsUnlimited.Models;
using PartsUnlimited.ViewModels;

namespace PartsUnlimited.Areas.Admin.Controllers
{
    public enum SortField { Name, Title, Price }
    public enum SortDirection { Up, Down }

    public class StoreManagerController : AdminController
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IHubContext _annoucementHub;
        private readonly ICacheCoordinator _cacheCoordinator;

        public StoreManagerController(IPartsUnlimitedContext context, IConnectionManager connectionManager, ICacheCoordinator cacheCoordinator)
        {
            _db = context;
            _annoucementHub = connectionManager.GetHubContext<AnnouncementHub>();
            _cacheCoordinator = cacheCoordinator;
        }

        //
        // GET: /StoreManager/

        public IActionResult Index(SortField sortField = SortField.Name, SortDirection sortDirection = SortDirection.Up)
        {
            // TODO [EF] Swap to native support for loading related data when available
            var products = from product in _db.Products
                           join category in _db.Categories on product.CategoryId equals category.CategoryId
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

            var sorted = Sort(products, sortField, sortDirection);

            return View(sorted);
        }

        private IQueryable<Product> Sort(IQueryable<Product> products, SortField sortField, SortDirection sortDirection)
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


        //
        // GET: /StoreManager/Details/5
        //
        public async Task<IActionResult> Details(int id)
        {
            string cacheId = CacheConstants.Key.ProductKey(id);
            var options = new PartsUnlimitedCacheOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
            Product product = await _cacheCoordinator.GetAsync(cacheId, LoadProductWithId(id), new InvokerOptions().WithCacheOptions(options).WhichRemovesIfNull());
            
            if (product != null)
            {
                // TODO [EF] We don't query related data as yet. We have to populate this until we do automatically.
                product.Category = _db.Categories.Single(g => g.CategoryId == product.CategoryId);
            }
            
            return View(product);
        }

        private Func<Product> LoadProductWithId(int id)
        {
            return delegate { return _db.Products.FirstOrDefault(a => a.ProductId == id); };
        }

        //
        // GET: /StoreManager/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync(HttpContext.RequestAborted);
                _annoucementHub.Clients.All.announcement(new ProductData { Title = product.Title, Url = Url.Action("Details", "Store", new { id = product.ProductId }) });
                await _cacheCoordinator.Remove(CacheConstants.Key.AnnouncementProduct);
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        //
        // GET: /StoreManager/Edit/5
        public IActionResult Edit(int id)
        {
            Product product = _db.Products.FirstOrDefault(a => a.ProductId == id);
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name", product.CategoryId).ToList();
            return View(product);
        }

        //
        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(product).State = EntityState.Modified;
                await _db.SaveChangesAsync(HttpContext.RequestAborted);
                //Invalidate the cache entry as it is modified
                await _cacheCoordinator.Remove(CacheConstants.Key.ProductKey(product.ProductId));
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        //
        // GET: /StoreManager/RemoveProduct/5
        public IActionResult RemoveProduct(int id)
        {
            Product product = _db.Products.FirstOrDefault(a => a.ProductId == id);
            return View(product);
        }

        //
        // POST: /StoreManager/RemoveProduct/5
        [HttpPost, ActionName("RemoveProduct")]
        public async Task<IActionResult> RemoveProductConfirmed(int id)
        {
            Product product = _db.Products.FirstOrDefault(a => a.ProductId == id);
            CartItem cartItem = _db.CartItems.FirstOrDefault(a => a.ProductId == id);
            List<OrderDetail> orderDetail = _db.OrderDetails.Where(a => a.ProductId == id).ToList();
            List<Raincheck> rainCheck = _db.RainChecks.Where(a => a.ProductId == id).ToList();

            if (product != null)
            {
                if (cartItem != null)
                {
                    _db.CartItems.Remove(cartItem);
                    await _db.SaveChangesAsync(HttpContext.RequestAborted);
                }

                if (orderDetail != null)
                {
                    _db.OrderDetails.RemoveRange(orderDetail);
                    await _db.SaveChangesAsync(HttpContext.RequestAborted);
                }

                if (rainCheck != null)
                {
                    _db.RainChecks.RemoveRange(rainCheck);
                    await _db.SaveChangesAsync(HttpContext.RequestAborted);
                }

                _db.Products.Remove(product);
                await _db.SaveChangesAsync(HttpContext.RequestAborted);
                //Remove the cache entry as it is removed
                await _cacheCoordinator.Remove(CacheConstants.Key.ProductKey(id));
            }

            return RedirectToAction("Index");
        }

#if TESTING
        //
        // GET: /StoreManager/GetProductIdFromName
        // Note: Added for automated testing purpose. Application does not use this.
        [HttpGet]
        public IActionResult GetProductIdFromName(string productName)
        {
            var product = db.Products.Where(a => a.Title == productName).FirstOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }

            return new ContentResult { Content = product.ProductId.ToString(), ContentType = "text/plain" };
        }
#endif
    }
}
