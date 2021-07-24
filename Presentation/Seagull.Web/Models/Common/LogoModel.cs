using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class LogoModel : BaseSeagullModel
    {
        public string StoreName { get; set; }

        public string LogoPath { get; set; }
    }
}