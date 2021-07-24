using Seagull.Admin.Models.Common;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class UserAddressModel : BaseSeagullModel
    {
        public int UserId { get; set; }

        public AddressModel Address { get; set; }
    }
}