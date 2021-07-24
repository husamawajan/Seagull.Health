using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class AdminHeaderLinksModel : BaseSeagullModel
    {
        public string ImpersonatedUserName { get; set; }
        public bool IsUserImpersonated { get; set; }
        public bool DisplayAdminLink { get; set; }
        public string EditPageUrl { get; set; }
    }
}