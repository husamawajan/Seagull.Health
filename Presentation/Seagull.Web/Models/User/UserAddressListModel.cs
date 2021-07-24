using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Models.Common;

namespace Seagull.Web.Models.User
{
    public partial class UserAddressListModel : BaseSeagullModel
    {
        public UserAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
    }
}