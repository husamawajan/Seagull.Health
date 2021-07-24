using System.Collections.Generic;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    public class CampaignListModel : BaseSeagullModel
    {
        public CampaignListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.Promotions.Campaigns.List.Stores")]
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}