// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.Search;
using System.Threading.Tasks;

namespace PartsUnlimited.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductSearch _search;

        public SearchController(IProductSearch search)
        {
            _search = search;
        }

        public async Task<IActionResult> Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View(null);
            }

            var result = await _search.Search(q);

            return View(result);
        }
    }
}
