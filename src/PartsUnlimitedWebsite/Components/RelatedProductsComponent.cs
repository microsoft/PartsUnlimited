// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "RelatedProducts")]
    public class RelatedProductsComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(dynamic product)
        {
            return Task.FromResult<IViewComponentResult>(View(product));
        }
    }
}