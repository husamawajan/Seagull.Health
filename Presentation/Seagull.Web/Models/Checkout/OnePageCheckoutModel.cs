using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Checkout
{
    public partial class OnePageCheckoutModel : BaseSeagullModel
    {
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
    }
}