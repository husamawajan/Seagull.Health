using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Seagull.Core;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Security;
using Seagull.Services.Topics;

namespace Seagull.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : ISitemapGenerator
    {
        #region Constants

        private const string DateFormat = @"yyyy-MM-dd";

        /// <summary>
        /// At now each provided sitemap file must have no more than 50000 URLs
        /// </summary>
        private const int maxSitemapUrlNumber = 50000;

        #endregion

        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly IWebHelper _webHelper;
        private readonly CommonSettings _commonSettings;
        private readonly SecuritySettings _securitySettings;

        #endregion

        #region Ctor

        public SitemapGenerator(IStoreContext storeContext,
            ITopicService topicService,
            IWebHelper webHelper,
            CommonSettings commonSettings,
            SecuritySettings securitySettings)
        {
            this._storeContext = storeContext;
            this._topicService = topicService;
            this._webHelper = webHelper;
            this._commonSettings = commonSettings;
            this._securitySettings = securitySettings;
        }

        #endregion

        #region Nested class

        /// <summary>
        /// Represents sitemap URL entry
        /// </summary>
        protected class SitemapUrl
        {
            public SitemapUrl(string location, UpdateFrequency frequency, DateTime updatedOn)
            {
                Location = location;
                UpdateFrequency = frequency;
                UpdatedOn = updatedOn;
            }

            /// <summary>
            /// Gets or sets URL of the page
            /// </summary>
            public string Location { get; set; }

            /// <summary>
            /// Gets or sets a value indicating how frequently the page is likely to change
            /// </summary>
            public UpdateFrequency UpdateFrequency { get; set; }

            /// <summary>
            /// Gets or sets the date of last modification of the file
            /// </summary>
            public DateTime UpdatedOn { get; set; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get HTTP protocol
        /// </summary>
        /// <returns>Protocol name as string</returns>
        protected virtual string GetHttpProtocol()
        {
            return _securitySettings.ForceSslForAllPages ? "https" : "http";
        }

        /// <summary>
        /// Generate URLs for the sitemap
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <returns>List of URL for the sitemap</returns>
        protected virtual IList<SitemapUrl> GenerateUrls(UrlHelper urlHelper)
        {
            var sitemapUrls = new List<SitemapUrl>();

            //home page
            var homePageUrl = urlHelper.RouteUrl("HomePage", null, GetHttpProtocol());
            sitemapUrls.Add(new SitemapUrl(homePageUrl, UpdateFrequency.Weekly, DateTime.UtcNow));

            //search products
            var productSearchUrl = urlHelper.RouteUrl("ProductSearch", null, GetHttpProtocol());
            sitemapUrls.Add(new SitemapUrl(productSearchUrl, UpdateFrequency.Weekly, DateTime.UtcNow));

            //contact us
            var contactUsUrl = urlHelper.RouteUrl("ContactUs", null, GetHttpProtocol());
            sitemapUrls.Add(new SitemapUrl(contactUsUrl, UpdateFrequency.Weekly, DateTime.UtcNow));


            //topics
            sitemapUrls.AddRange(GetTopicUrls(urlHelper));

            //custom URLs
            sitemapUrls.AddRange(GetCustomUrls());

            return sitemapUrls;
        }

        /// <summary>
        /// Get topic URLs for the sitemap
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <returns>Collection of sitemap URLs</returns>
        protected virtual IEnumerable<SitemapUrl> GetTopicUrls(UrlHelper urlHelper)
        {
            return _topicService.GetAllTopics(_storeContext.CurrentStore.Id).Where(t => t.IncludeInSitemap).Select(topic =>
            {
                var url = urlHelper.RouteUrl("Topic", new { SeName = topic.GetSeName() }, GetHttpProtocol());
                return new SitemapUrl(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            });
        }

        /// <summary>
        /// Get custom URLs for the sitemap
        /// </summary>
        /// <returns>Collection of sitemap URLs</returns>
        protected virtual IEnumerable<SitemapUrl> GetCustomUrls()
        {
            var storeLocation = _webHelper.GetStoreLocation();

            return _commonSettings.SitemapCustomUrls.Select(customUrl => 
                new SitemapUrl(string.Concat(storeLocation, customUrl), UpdateFrequency.Weekly, DateTime.UtcNow));
        }

        /// <summary>
        /// Write sitemap index file into the stream
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="stream">Stream</param>
        /// <param name="sitemapNumber">The number of sitemaps</param>
        protected virtual void WriteSitemapIndex(UrlHelper urlHelper, Stream stream, int sitemapNumber)
        {
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("sitemapindex");
                writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                //write URLs of all available sitemaps
                for (var id = 1; id <= sitemapNumber; id++)
                {
                    var url = urlHelper.RouteUrl("sitemap-indexed.xml", new { Id = id }, GetHttpProtocol());
                    var location = XmlHelper.XmlEncode(url);

                    writer.WriteStartElement("sitemap");
                    writer.WriteElementString("loc", location);
                    writer.WriteElementString("lastmod", DateTime.UtcNow.ToString(DateFormat));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write sitemap file into the stream
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="stream">Stream</param>
        /// <param name="sitemapUrls">List of sitemap URLs</param>
        protected virtual void WriteSitemap(UrlHelper urlHelper, Stream stream, IList<SitemapUrl> sitemapUrls)
        {
            using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset");
                writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                //write URLs from list to the sitemap
                foreach (var url in sitemapUrls)
                {
                    writer.WriteStartElement("url");
                    var location = XmlHelper.XmlEncode(url.Location);

                    writer.WriteElementString("loc", location);
                    writer.WriteElementString("changefreq", url.UpdateFrequency.ToString().ToLowerInvariant());
                    writer.WriteElementString("lastmod", url.UpdatedOn.ToString(DateFormat));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="id">Sitemap identifier</param>
        /// <returns>Sitemap.xml as string</returns>
        public virtual string Generate(UrlHelper urlHelper, int? id)
        {
            using (var stream = new MemoryStream())
            {
                Generate(urlHelper, stream, id);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="id">Sitemap identifier</param>
        /// <param name="stream">Stream of sitemap.</param>
        public virtual void Generate(UrlHelper urlHelper, Stream stream, int? id)
        {
            //generate all URLs for the sitemap
            var sitemapUrls = GenerateUrls(urlHelper);

            //split URLs into separate lists based on the max size 
            var sitemaps = sitemapUrls.Select((url, index) => new { Index = index, Value = url })
                .GroupBy(group => group.Index / maxSitemapUrlNumber).Select(group => group.Select(url => url.Value).ToList()).ToList();

            if (!sitemaps.Any())
                return;

            if (id.HasValue)
            {
                //requested sitemap does not exist
                if (id.Value == 0 || id.Value > sitemaps.Count)
                    return;

                //otherwise write a certain numbered sitemap file into the stream
                WriteSitemap(urlHelper, stream, sitemaps.ElementAt(id.Value - 1));
                
            }
            else
            {
                //URLs more than the maximum allowable, so generate a sitemap index file
                if (sitemapUrls.Count >= maxSitemapUrlNumber)
                {
                    //write a sitemap index file into the stream
                    WriteSitemapIndex(urlHelper, stream, sitemaps.Count);
                }
                else
                {
                    //otherwise generate a standard sitemap
                    WriteSitemap(urlHelper, stream, sitemaps.First());
                }
            }
        }

        #endregion
    }
}
