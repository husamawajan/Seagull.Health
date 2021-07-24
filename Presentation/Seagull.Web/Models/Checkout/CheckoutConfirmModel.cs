using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Checkout
{
    public partial class CheckoutConfirmModel : BaseSeagullModel
    {
        public CheckoutConfirmModel()
        {
            Warnings = new List<string>();
        }

        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public string MinOrderTotalWarning { get; set; }

        public IList<string> Warnings { get; set; }
    }
}