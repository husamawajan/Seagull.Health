using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Checkout
{
    public partial class CheckoutCompletedModel : BaseSeagullModel
    {
        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }
        public bool OnePageCheckoutEnabled { get; set; }
    }
}