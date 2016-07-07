// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.Rendering;
using PartsUnlimited.Models;

namespace PartsUnlimited.ViewModels
{
    public class EditPartViewModel
    {
        public Product Product { get; set; }

        public SelectList CategoryList { get; set; }
    }
}