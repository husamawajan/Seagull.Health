using System;
using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Home
{
    public partial class SeagullCommerceNewsModel : BaseSeagullModel
    {
        public SeagullCommerceNewsModel()
        {
            Items = new List<NewsDetailsModel>();
        }

        public List<NewsDetailsModel> Items { get; set; }
        public bool HasNewItems { get; set; }
        public bool HideAdvertisements { get; set; }

        public class NewsDetailsModel : BaseSeagullModel
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string Summary { get; set; }
            public DateTimeOffset PublishDate { get; set; }
        }
    }
}