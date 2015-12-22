// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using NSubstitute;
using PartsUnlimited.WebsiteConfiguration;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PartsUnlimited.Utils
{
    public class ContentDeliveryNetworkConfigurationTests
    {
        [Fact]
        public void ImageContent()
        {
            const string ImageCdn = "some/test/path";

            var config = Substitute.For<IConfiguration>();
            config["images"].Returns(ImageCdn);

            var cdnConfig = new ContentDeliveryNetworkConfiguration(config);

            Assert.Equal(ImageCdn, cdnConfig.Images);
        }

        [Fact]
        public void ImageContentNull()
        {
            var config = Substitute.For<IConfiguration>();
            config["images"].Returns((string)null);
            var cdnConfig = new ContentDeliveryNetworkConfiguration(config);

            Assert.Null(cdnConfig.Images);
        }

        [Fact]
        public void ScriptStylesParse()
        {
            var values = new[]
            {
                new ConfigPathHelper{ Name="item1", Path="path1 path3  path4"},
                new ConfigPathHelper{ Name="item2", Path="path2"},
            };

            var config = Substitute.For<IConfiguration>();
            var scriptsConfigSection = CreateConfigSection(values);
            config.GetSection("Scripts").Returns(scriptsConfigSection);

            var cdnConfig = new ContentDeliveryNetworkConfiguration(config);

            Assert.Equal(2, cdnConfig.Scripts.Count);
            Assert.Equal(new[] { "path1", "path3", "path4" }, cdnConfig.Scripts["item1"]);
            Assert.Equal(new[] { "path2" }, cdnConfig.Scripts["item2"]);
        }

        private IConfigurationSection CreateConfigSection(IEnumerable<ConfigPathHelper> values)
        {
            var section = Substitute.For<IConfigurationSection>();
            var sections = new List<IConfigurationSection>();

            foreach (var value in values)
            {
                var emptySection = Substitute.For<IConfigurationSection>();
                emptySection.Key.Returns(value.Name);
                emptySection.Value.Returns(value.Path);
                sections.Add(emptySection);
            }

            section.GetChildren().Returns(sections);
            return section;
        }

        private class ConfigPathHelper
        {
            public string Path { get; set; }
            public string Name { get; set; }
        }
    }
}