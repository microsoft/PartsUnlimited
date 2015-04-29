// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace PartsUnlimited.Models
{
    public class CommunityPost
    {
        public string Image { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public CommunitySource Source { get; set; }
    }

    public enum CommunitySource
    {
        Facebook,
        Twitter
    }
}