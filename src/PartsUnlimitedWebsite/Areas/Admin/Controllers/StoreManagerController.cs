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
using PartsUnlimited.Cache;
using PartsUnlimited.Hubs;
using PartsUnlimited.Models;
using PartsUnlimited.Repository;
using PartsUnlimited.ViewModels;

namespace PartsUnlimited.Areas.Admin.Controllers
{
    public class StoreManagerController : AdminController
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IHubContext _annoucementHub;
        private readonly ICacheCoordinator _cacheCoordinator;
        private readonly IProductRepository _productRepository;

        public StoreManagerController(IPartsUnlimitedContext context, IConnectionManager connectionManager, 
            ICacheCoordinator cacheCoordinator, IProductRepository productRepository)
        {
            _db = context;
            _annoucementHub = connectionManager.GetHubContext<AnnouncementHub>();
            _cacheCoordinator = cacheCoordinator;
            _productRepository = productRepository;
        }

        //
        // GET: /StoreManager/

        public async Task<IActionResult> Index(SortField sortField = SortField.Name, SortDirection sortDirection = SortDirection.Up)
        {
            IEnumerable<IProduct> products = await _productRepository.LoadAllProducts(sortField, sortDirection);
            return View(products);
        }

        //
        // GET: /StoreManager/Details/5
        //
        public async Task<IActionResult> Details(int id)
        {
            string cacheId = CacheConstants.Key.ProductKey(id);
            var options = new PartsUnlimitedCacheOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
            IProduct product = await _cacheCoordinator.GetAsync(cacheId, LoadProductWithId(id), new CacheCoordinatorOptions().WithCacheOptions(options).WhichRemovesIfNull());
            
            if (product != null)
            {
                // TODO [EF] We don't query related data as yet. We have to populate this until we do automatically.
                int categoryId = product.CategoryId;
                product.Category = _db.Categories.Single(g => g.CategoryId == categoryId);
            }
            
            return View(product);
        }

        private Func<Task<IProduct>> LoadProductWithId(int id)
        {
            return async () => await _productRepository.Load(id);
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
            if (TryValidateModel(product) && ModelState.IsValid)
            {
                await _productRepository.Add(product, HttpContext.RequestAborted);
                _annoucementHub.Clients.All.announcement(new ProductData { Title = product.Title, Url = Url.Action("Details", "Store", new { id = product.ProductId }) });
                await _cacheCoordinator.Remove(CacheConstants.Key.AnnouncementProduct);
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        //
        // GET: /StoreManager/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            IProduct product = await _productRepository.Load(id);
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name", product.CategoryId).ToList();
            return View(product);
        }

        //
        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit([ModelBinder(BinderType = typeof(ProductModelBinder))]IProduct product)
        {
            if (TryValidateModel(product) && ModelState.IsValid)
            {
                await _productRepository.Save(product, HttpContext.RequestAborted);
                //Invalidate the cache entry as it is modified
                await _cacheCoordinator.Remove(CacheConstants.Key.ProductKey(product.ProductId));
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        //
        // GET: /StoreManager/RemoveProduct/5
        public async Task<IActionResult> RemoveProduct(int id)
        {
            IProduct product = await _productRepository.Load(id);
            return View(product);
        }

        //
        // POST: /StoreManager/RemoveProduct/5
        [HttpPost, ActionName("RemoveProduct")]
        public async Task<IActionResult> RemoveProductConfirmed(int id)
        {
            IProduct product = await _productRepository.Load(id);
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

                await _productRepository.Delete(product, HttpContext.RequestAborted);
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
