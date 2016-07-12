// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.Queries;
using System.Threading.Tasks;

namespace PartsUnlimited.Areas.Admin.Controllers
{
    public class RaincheckController : AdminController
    {
        private readonly IRaincheckQuery _query;

        public RaincheckController(IRaincheckQuery query)
        {
            _query = query;
        }

        public async Task<IActionResult> Index()
        {
            var rainchecks = await _query.GetAllAsync();

            return View(rainchecks);
        }
    }
}
