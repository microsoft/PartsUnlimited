// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PartsUnlimited.Utils
{
    public class ConfigurationExtensionsTests
    {
        [Fact]
        public void TestStrings()
        {
            var values = new[]
            {
                new ConfigPathHelper{ Name="item1", Path="path1 path3  path4\tpath5\rpath6\n\rpath7"},
                new ConfigPathHelper{ Name="item2", Path="path2"},
            };

            var config = CreateConfig(values);
            var lookup = config.ToLookup();

            Assert.Equal(2, lookup.Count);
            Assert.Equal(new[] { "path1", "path3", "path4", "path5", "path6", "path7" }, lookup["item1"]);
            Assert.Equal(new[] { "path2" }, lookup["item2"]);
        }

        private IConfiguration CreateConfig(IEnumerable<ConfigPathHelper> values)
        {
            var config = Substitute.For<IConfiguration>();
            var sections = new List<IConfigurationSection>();

            foreach (var value in values)
            {
                var emptySection = Substitute.For<IConfigurationSection>();
                emptySection.Key.Returns(value.Name);
                emptySection.Value.Returns(value.Path);
                sections.Add(emptySection);
            }

            config.GetChildren().Returns(sections);

            return config;
        }

        private class ConfigPathHelper
        {
            public string Path { get; set; }
            public string Name { get; set; }
        }
    }
}