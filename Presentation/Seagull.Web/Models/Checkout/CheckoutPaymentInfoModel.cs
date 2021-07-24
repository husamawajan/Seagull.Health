using System.Web.Routing;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Checkout
{
    public partial class CheckoutPaymentInfoModel : BaseSeagullModel
    {
        public string PaymentInfoActionName { get; set; }
        public string PaymentInfoControllerName { get; set; }
        public RouteValueDictionary PaymentInfoRouteValues { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool DisplayOrderTotals { get; set; }
    }
}