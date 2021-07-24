using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Models.Common;

namespace Seagull.Web.Models.Checkout
{
    public partial class CheckoutBillingAddressModel : BaseSeagullModel
    {
        public CheckoutBillingAddressModel()
        {
            ExistingAddresses = new List<AddressModel>();
            NewAddress = new AddressModel();
        }

        public IList<AddressModel> ExistingAddresses { get; set; }

        public AddressModel NewAddress { get; set; }


        public bool ShipToSameAddress { get; set; }
        public bool ShipToSameAddressAllowed { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool NewAddressPreselected { get; set; }
    }
}