// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "CategoryMenu")]
    public class CategoryMenuComponent : ViewComponent
    {
        private readonly ICategoryLoader _categoryLoader;

        public CategoryMenuComponent(ICategoryLoader categoryLoader)
        {
            _categoryLoader = categoryLoader;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Category> categories = await _categoryLoader.LoadAll();
            return View(categories);
        }
    }
}
