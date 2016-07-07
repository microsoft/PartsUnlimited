// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PartsUnlimited
{
    public static class Extensions
    {
        public static DbSet<T> ToDbSet<T>(this IEnumerable<T> items)
            where T : class
        {
            var dbset = (DbSet<T>)Substitute.For(new[] { typeof(DbSet<T>), typeof(IQueryable<T>) }, null);

            Assert.IsAssignableFrom<IQueryable<T>>(dbset);
            Assert.IsAssignableFrom<DbSet<T>>(dbset);

            var queryable = items.AsQueryable();

            ((IQueryable<T>)dbset).Provider.Returns(queryable.Provider);
            ((IQueryable<T>)dbset).Expression.Returns(queryable.Expression);
            ((IQueryable<T>)dbset).ElementType.Returns(queryable.ElementType);
            ((IQueryable<T>)dbset).GetEnumerator().Returns(queryable.GetEnumerator());

            return dbset;
        }
    }
}