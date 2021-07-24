using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class CurrencySelectorModel : BaseSeagullModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public int CurrentCurrencyId { get; set; }
    }
}