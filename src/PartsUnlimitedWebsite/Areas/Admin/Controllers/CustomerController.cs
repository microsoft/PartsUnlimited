// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsUnlimited.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Areas.Admin.Controllers
{
    public class CustomerController : AdminController
    {
        private readonly IPartsUnlimitedContext _context;

        public CustomerController(IPartsUnlimitedContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect(nameof(Find));
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Redirect(nameof(Find));
            }

            return View(user);
        }

        public async Task<IActionResult> Find(string username, string email, string phoneNumber)
        {
            IQueryable<ApplicationUser> query = _context.Users;

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(u => u.UserName == username);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email == email);
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                query = query.Where(u => u.PhoneNumber == phoneNumber);
            }

            // We only want cases where there is one instance.  SingleOrDefault will throw an exception
            // when there is more than one, so we take two and only use the result if it was the only one
            var result = await query.Take(2).ToListAsync();

            if (result.Count == 1)
            {
                return RedirectToAction(nameof(Index), new { id = result[0].Id });
            }

            return View();
        }
    }
}