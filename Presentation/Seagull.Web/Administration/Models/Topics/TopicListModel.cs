using System.Collections.Generic;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Topics
{
    public partial class TopicListModel : BaseSeagullModel
    {
        public TopicListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.ContentManagement.Topics.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}