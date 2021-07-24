using System.Collections.Generic;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    public partial class MessageTemplateListModel : BaseSeagullModel
    {
        public MessageTemplateListModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.ContentManagement.MessageTemplates.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}