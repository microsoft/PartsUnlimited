// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// This takes a config and parses it into a lookup.  The values are split at whitespace
        /// </summary>
        public static ILookup<string, string> ToLookup(this IConfiguration config)
        {
            return config.ToLookup(' ', '\r', '\n', '\t');
        }

        public static ILookup<string, string> ToLookup(this IConfiguration config, params char[] separator)
        {
            if (config == null)
            {
                return Enumerable.Empty<string>().ToLookup(s => s, s => s);
            }

            var subkeys = config.GetChildren();
            var names = subkeys.Select(s => s.Key.Split(':').Last()).ToList();
            var values = subkeys.Select(s => s.Value).ToList();

            if (values.Any(v => v == null))
            {
                throw new InvalidDataException("Values must be a string");
            }

            var splitValues = values.Select(v => v.Split(separator, StringSplitOptions.RemoveEmptyEntries));

            return names
                .Zip(splitValues, Tuple.Create)
                .SelectMany(o => o.Item2.Select(item2 => Tuple.Create(o.Item1, item2)))
                .ToLookup(o => o.Item1, o => o.Item2);
        }

        public static T Get<T>(this IConfiguration configuration, string key)
        {
            return (T)Convert.ChangeType(configuration[key], typeof(T));
        }
    }
}