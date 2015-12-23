// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information. 
using Microsoft.AspNet.Mvc.ModelBinding;

namespace PartsUnlimited.Models
{
    public interface IProductBuilder
    {
        IProduct Build(ModelBindingContext context);
    }
}