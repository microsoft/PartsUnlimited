// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using PartsUnlimited.Models;
using PartsUnlimited.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PartsUnlimited.WebApi
{
    [Route("api/[controller]")]
    public class RaincheckController : Controller
    {
        private readonly IRaincheckQuery _query;

        public RaincheckController(IRaincheckQuery query)
        {
            _query = query;
        }

        [HttpGet]
        public Task<IEnumerable<Raincheck>> Get()
        {
            return _query.GetAllAsync();
        }

        [HttpGet("{id}")]
        public Task<Raincheck> Get(int id)
        {
            return _query.FindAsync(id);
        }

        [HttpPost]
        public Task<int> Post([FromBody]Raincheck raincheck)
        {
            return _query.AddAsync(raincheck);
        }
    }
}
