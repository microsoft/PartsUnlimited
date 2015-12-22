// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PartsUnlimited.Cache;
using PartsUnlimited.Models;
using PartsUnlimited.Repository;
using PartsUnlimited.ViewModels;

namespace PartsUnlimited.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICacheCoordinator _cacheCoordinator;
        private readonly IProductRepository _productRepository;

        public HomeController(ICacheCoordinator cacheCoordinator, IProductRepository productRepository)
        {
            _cacheCoordinator = cacheCoordinator;
            _productRepository = productRepository;
        }

        //
        // GET: /Home/
        public async Task<IActionResult> Index()
        {
            // Get most popular products
            var topSellingKey = CacheConstants.Key.TopSellingProducts;
            var topSellingOptions = new PartsUnlimitedCacheOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            IEnumerable<IProduct> topSellingProducts = await _cacheCoordinator.GetAsync(topSellingKey, () => GetTopSellingProducts(4), new CacheCoordinatorOptions().WithCacheOptions(topSellingOptions));

            // Get most new arrival products
            var newArrivalKey = CacheConstants.Key.NewArrivalProducts;
            var newArrivalOptions = new PartsUnlimitedCacheOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)).SetPriority(PartsUnlimitedCacheItemPriority.High);
            IEnumerable<IProduct> newProducts = await _cacheCoordinator.GetAsync(newArrivalKey, () => GetNewProducts(4), new CacheCoordinatorOptions().WithCacheOptions(newArrivalOptions));

            var viewModel = new HomeViewModel
            {
                NewProducts = newProducts,
                TopSellingProducts = topSellingProducts,
                CommunityPosts = GetCommunityPosts()
            };

            return View(viewModel);
        }

        //Can be removed and handled when HandleError filter is implemented
        //https://github.com/aspnet/Mvc/issues/623
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        private Task<IEnumerable<IProduct>> GetTopSellingProducts(int count)
        {
            // Group the order details by product and return
            // the products with the highest count
            return _productRepository.LoadTopSellingProducts(count);
        }

        private Task<IEnumerable<IProduct>> GetNewProducts(int count)
        {
            return _productRepository.LoadNewProducts(count);
        }

        private List<CommunityPost> GetCommunityPosts()
        {
            return new List<CommunityPost>{
                new CommunityPost {
                    Content= "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus commodo tellus lorem, et bibendum velit sagittis in. Integer nisl augue, cursus id tellus in, sodales porta.",
                    DatePosted = DateTime.Now,
                    Image = "community_1.png",
                    Source = CommunitySource.Facebook
                },
                new CommunityPost {
                    Content= " Donec tincidunt risus in ligula varius, feugiat placerat nisi condimentum. Quisque rutrum eleifend venenatis. Phasellus a hendrerit urna. Cras arcu leo, hendrerit vel mollis nec.",
                    DatePosted = DateTime.Now,
                    Image = "community_2.png",
                    Source = CommunitySource.Facebook
                },
                new CommunityPost {
                    Content= "Aenean vestibulum non lacus non molestie. Curabitur maximus interdum magna, ullamcorper facilisis tellus fermentum eu. Pellentesque iaculis enim ac vestibulum mollis.",
                    DatePosted = DateTime.Now,
                    Image = "community_3.png",
                    Source = CommunitySource.Facebook
                },
                new CommunityPost {
                    Content= "Ut consectetur sed justo vel convallis. Vestibulum quis metus leo. Nulla hendrerit pharetra dui, vel euismod lectus elementum sit amet. Nam dolor turpis, sodales non mi nec.",
                    DatePosted = DateTime.Now,
                    Image = "community_4.png",
                    Source = CommunitySource.Facebook
                }
            };
        }
    }
}
