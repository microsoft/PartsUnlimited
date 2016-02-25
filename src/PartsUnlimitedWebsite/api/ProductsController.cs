// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace PartsUnlimited.api.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IPartsUnlimitedContext _context;

        public ProductsController(IPartsUnlimitedContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Product> Get(bool sale = false)
        {
            if (!sale)
            {
                return _context.Products;
            }

            return _context.Products.Where(p => p.Price != p.SalePrice);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return new ObjectResult(product);
        }
    }
}
