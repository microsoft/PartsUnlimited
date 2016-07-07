// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartsUnlimited.Models
{
    public class Order
    {
        [BindNever]
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        [Display(Name = "Order Date")]
        public System.DateTime OrderDate { get; set; }

        [BindNever]
        [Required]
        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(160)]
        public string Name { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [StringLength(40)]
        public string City { get; set; }

        [Required]
        [StringLength(40)]
        public string State { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [StringLength(10, MinimumLength = 5)]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(40)]
        public string Country { get; set; }

        [Required]
        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        public bool Processed { get; set; }

        [BindNever]
        public List<OrderDetail> OrderDetails { get; set; }
    }
}