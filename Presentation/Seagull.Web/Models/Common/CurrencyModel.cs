using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class CurrencyModel : BaseSeagullEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }
    }
}