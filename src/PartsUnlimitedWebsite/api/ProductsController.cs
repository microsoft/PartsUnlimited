// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using PartsUnlimited.Repository;

namespace PartsUnlimited.api.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IPartsUnlimitedContext _context;
        private readonly IProductRepository _productRepository;

        public ProductsController(IPartsUnlimitedContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<IProduct>> Get(bool sale = false)
        {
            if (!sale)
            {
                return await _productRepository.LoadAllProducts();
            }

            return await _productRepository.LoadSaleProducts();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productRepository.Load(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return new ObjectResult(product);
        }
    }
}
