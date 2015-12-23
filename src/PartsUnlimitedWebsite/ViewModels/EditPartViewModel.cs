// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc.Rendering;

namespace PartsUnlimited.ViewModels
{
    public class EditPartViewModel
    {
        public dynamic Product { get; set; }

        public SelectList CategoryList { get; set; }
    }
}