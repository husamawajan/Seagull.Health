using System.Web.Mvc;
using Seagull.Core.Domain.Users;
using Seagull.Services.Users;
using Seagull.Services.Security;
using Seagull.Web.Factories;
using Seagull.Web.Framework.Security;

namespace Seagull.Web.Controllers
{
    [SeagullHttpsRequirement(SslRequirement.No)]
    public partial class ProfileController : BasePublicController
    {
        private readonly IProfileModelFactory _profileModelFactory;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly UserSettings _userSettings;

        public ProfileController(IProfileModelFactory profileModelFactory,
            IUserService userService,
            IPermissionService permissionService,
            UserSettings userSettings)
        {
            this._profileModelFactory = profileModelFactory;
            this._userService = userService;
            this._permissionService = permissionService;
            this._userSettings = userSettings;
        }

        public virtual ActionResult Index(int? id, int? page)
        {
            if (!_userSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("HomePage");
            }

            var userId = 0;
            if (id.HasValue)
            {
                userId = id.Value;
            }

            var user = _userService.GetUserById(userId);
            if (user == null)//|| user.IsGuest())
            {
                return RedirectToRoute("HomePage");
            }

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                DisplayEditLink(Url.Action("Edit", "User", new { id = user.Id, area = "Admin" }));

            var model = _profileModelFactory.PrepareProfileIndexModel(user, page);
            return View(model);
        }

        //profile info tab
        [ChildActionOnly]
        public virtual ActionResult Info(int userProfileId)
        {
            var user = _userService.GetUserById(userProfileId);
            if (user == null)
            {
                return RedirectToRoute("HomePage");
            }

            var model = _profileModelFactory.PrepareProfileInfoModel(user);
            return PartialView(model);
        }

        //latest posts tab
        [ChildActionOnly]
        public virtual ActionResult Posts(int userProfileId, int page)
        {
            var user = _userService.GetUserById(userProfileId);
            if (user == null)
            {
                return RedirectToRoute("HomePage");
            }
            
            //var model = _profileModelFactory.PrepareProfilePostsModel(user, page);
            return PartialView();
        }
    }
}
