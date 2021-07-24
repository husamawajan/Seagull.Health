using System.Collections.Generic;
using Seagull.Admin.Models.Stores;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    public partial class StoreScopeConfigurationModel : BaseSeagullModel
    {
        public StoreScopeConfigurationModel()
        {
            Stores = new List<StoreModel>();
        }

        public int StoreId { get; set; }
        public IList<StoreModel> Stores { get; set; }
    }
}