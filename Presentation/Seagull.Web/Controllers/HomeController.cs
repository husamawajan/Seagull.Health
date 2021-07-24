using System.Web.Mvc;
using Seagull.Web.Framework.Security;

namespace Seagull.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        [SeagullHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Index()
        {
            return Redirect(Url.Action("Login", "User"));
        }
        public virtual ActionResult GoToIndex()
        {
            return Redirect(Url.Action("Login", "User"));
        }
    }
}


