using Seagull.Web.Framework.Mvc;
using Seagull.Web.Models.Common;

namespace Seagull.Web.Models.User
{
    public partial class UserAddressEditModel : BaseSeagullModel
    {
        public UserAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
    }
}