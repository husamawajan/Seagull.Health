using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Models.Topics;

namespace Seagull.Web.Models.Common
{
    public partial class SitemapModel : BaseSeagullModel
    {
        public SitemapModel()
        {
            Topics = new List<TopicModel>();
        }

        public IList<TopicModel> Topics { get; set; }

        public bool NewsEnabled { get; set; }
        public bool BlogEnabled { get; set; }
        public bool ForumEnabled { get; set; }
    }
}