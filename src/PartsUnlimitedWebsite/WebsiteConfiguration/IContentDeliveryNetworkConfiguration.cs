// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IContentDeliveryNetworkConfiguration
    {
        string Images { get; }
        ILookup<string, string> Scripts { get; }
        ILookup<string, string> Styles { get; }
    }
}