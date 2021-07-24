using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Checkout
{
    public partial class CheckoutProgressModel : BaseSeagullModel
    {
        public CheckoutProgressStep CheckoutProgressStep { get; set; }
    }

    public enum CheckoutProgressStep
    {
        Cart,
        Address,
        Shipping,
        Payment,
        Confirm,
        Complete
    }
}