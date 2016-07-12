// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using PartsUnlimited.WebsiteConfiguration;
using System;
using System.IO;
using System.Linq;
using Xunit;
using System.Xml;
using System.Text.Encodings.Web;

namespace PartsUnlimited.Utils
{
    public class ContentDeliveryNetworkExtensionsTests
    {
        private enum ContentLinkType { Javascript, CSS };

        private IHtmlHelper _myHtmlHelper = Substitute.For<IHtmlHelper>();

        [Fact]
        public void NullSrc()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _myHtmlHelper.Image(null, "something"));
            Assert.Throws<ArgumentOutOfRangeException>(() => _myHtmlHelper.Image("", "something"));
            Assert.Throws<ArgumentOutOfRangeException>(() => _myHtmlHelper.Image(" ", "something"));
        }

        [Fact]
        public void AltTagDisplayed()
        {
            const string src = "somepath.png";
            const string alt = "some alternate text";

            ContentDeliveryNetworkExtensions.Configuration = null;

            var tag = _myHtmlHelper.Image(src, alt);

            Assert.Null(ContentDeliveryNetworkExtensions.Configuration);
            Verify(tag, src, alt);
        }

        [Fact]
        public void NoConfiguration()
        {
            const string src = "somepath.png";

            ContentDeliveryNetworkExtensions.Configuration = null;

            var tag = _myHtmlHelper.Image(src, null);

            Assert.Null(ContentDeliveryNetworkExtensions.Configuration);
            Verify(tag, src, null);
        }

        [Fact]
        public void WithConfiguration()
        {
            const string cdnPath = "something/asdf";
            const string src = "somepath.png";
            var expectedUrl = string.Format("{0}/{1}", cdnPath, src);

            var config = Substitute.For<IContentDeliveryNetworkConfiguration>();
            config.Images.Returns(cdnPath);

            ContentDeliveryNetworkExtensions.Configuration = config;

            var tag = _myHtmlHelper.Image(src, null);

            Verify(tag, expectedUrl, null);
        }

        [Fact]
        public void ScriptTest()
        {
            var values = new[]
            {
                Tuple.Create("item1","path1"),
                Tuple.Create("item2","path2"),
                Tuple.Create("item1","path3")
            };

            var lookup = values.ToLookup(s => s.Item1, s => s.Item2);
            var config = Substitute.For<IContentDeliveryNetworkConfiguration>();
            config.Scripts.Returns(lookup);

            ContentDeliveryNetworkExtensions.Configuration = config;

            foreach (var value in values)
            {
                VerifyContentLinks(_myHtmlHelper.Script(value.Item1), value.Item1, ContentLinkType.Javascript, lookup);
            }
        }

        [Fact]
        public void StyleTests()
        {
            var values = new[]
            {
                Tuple.Create("item1","path1"),
                Tuple.Create("item2","path2"),
                Tuple.Create("item1","path3")
            };

            var lookup = values.ToLookup(s => s.Item1, s => s.Item2);
            var config = Substitute.For<IContentDeliveryNetworkConfiguration>();
            config.Styles.Returns(lookup);

            ContentDeliveryNetworkExtensions.Configuration = config;

            foreach (var value in values)
            {
                VerifyContentLinks(_myHtmlHelper.Styles(value.Item1), value.Item1, ContentLinkType.CSS, lookup);
            }
        }


        private void VerifyContentLinks(IHtmlContent html, string path, ContentLinkType contentType, ILookup<string, string> lookup)
        {
            var xmlDoc = new XmlDocument();
            var doc = xmlDoc.CreateDocumentFragment();

            using (var writer = new StringWriter())
            {
                html.WriteTo(writer, HtmlEncoder.Default);
                doc.InnerXml = writer.ToString();
            }

            var childNodes = doc.ChildNodes.Cast<XmlNode>().Where(n => !(n is XmlWhitespace)).ToList();
            var expectedList = lookup[path];

            Assert.Equal(expectedList.Count(), childNodes.Count);

            foreach (var pair in childNodes.Zip(expectedList, Tuple.Create))
            {
                var node = pair.Item1;
                var expected = pair.Item2;

                if (contentType == ContentLinkType.Javascript)
                {
                    Assert.Equal("script", node.Name);
                    Assert.Equal(2, node.Attributes.Count);

                    Assert.Equal("text/javascript", node.Attributes.GetNamedItem("type").Value);
                    Assert.Equal(expected, node.Attributes.GetNamedItem("src").Value);
                }
                else
                {
                    Assert.Equal("link", node.Name);
                    Assert.Equal(2, node.Attributes.Count);

                    Assert.Equal("stylesheet", node.Attributes.GetNamedItem("rel").Value);
                    Assert.Equal(expected, node.Attributes.GetNamedItem("href").Value);
                }
            }
        }

        private void Verify(IHtmlContent html, string src, string alt)
        {
            var doc = new XmlDocument();

            using (var writer = new StringWriter())
            {
                html.WriteTo(writer, HtmlEncoder.Default);
                doc.LoadXml(writer.ToString());
            }

            Assert.Equal(1, doc.ChildNodes.Count);

            var node = doc.ChildNodes.Item(0);

            Assert.Equal("img", node.Name);

            var srcNode = node.Attributes.GetNamedItem("src");

            Assert.NotNull(srcNode);
            Assert.Equal("src", srcNode.Name);
            Assert.Equal(src, srcNode.Value);

            var altNode = node.Attributes.GetNamedItem("alt");

            if (!string.IsNullOrWhiteSpace(alt))
            {
                Assert.NotNull(altNode);
                Assert.Equal("alt", altNode.Name);
                Assert.Equal(alt, altNode.Value);
            }
            else
            {
                Assert.Null(altNode);
            }
        }
    }
}