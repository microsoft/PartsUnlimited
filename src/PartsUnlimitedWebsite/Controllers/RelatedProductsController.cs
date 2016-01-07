// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Threading.Tasks;
using PartsUnlimited.Repository;

namespace PartsUnlimited.Controllers
{
    public class RelatedProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public RelatedProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> GetRelatedProducts(int productId)
        {
            var products = await _productRepository.LoadRelatedProducts(productId);

            var relatedProducts = products
                .Where(p => p != null && p.ProductId != productId)
                .ToList();

            return PartialView("_RelatedProducts", relatedProducts);
        }
    }
}