using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Models.Users;
using Seagull.Admin.Models.Security;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Users;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Security;
using Seagull.Core.Domain.Security;
using Seagull.Admin.Helpers;

namespace Seagull.Admin.Controllers
{
    public partial class SecurityController : BaseAdminController
	{
		#region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;

		#endregion

		#region Constructors

        public SecurityController(ILogger logger, IWorkContext workContext,
            IPermissionService permissionService,
            IUserService userService, ILocalizationService localizationService)
		{
            this._logger = logger;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._userService = userService;
            this._localizationService = localizationService;
		}

		#endregion 

        #region Methods

        public virtual ActionResult AccessDenied(string pageUrl)
        {
            var currentUser = _workContext.CurrentUser;
            //if (currentUser == null || currentUser.IsGuest())
            if (currentUser == null)
            {
                _logger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl));
                return View();
            }

            _logger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentUser.Email, currentUser.Email, pageUrl));


            return View();
        }

        public virtual ActionResult Permissions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();
            return View();
        }


        public virtual ActionResult PreparePermission()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();
            var model = new List<PermissionRecordModelAngular>();
            model = (from a in _permissionService.GetAllPermissionRecordCategory()
                     select new PermissionRecordModelAngular
                     {
                         title = a.Category,
                         childs = (from b in _permissionService.GetAllPermissionRecordByCategory(a.Category)
                                   select new PermissionRecordModelAngularChild
                                   {
                                       id = b.Id,
                                       title = _localizationService.GetResource(b.Name)
                                   }).ToList()
                     }).ToList();
            return Json(model);
        }
        public virtual ActionResult GetAllPermissionsForSelectedUserRole(int UserRoleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();
            var model = _permissionService.GetAllPermissionForUserRole(UserRoleId);
            return Json(model);
        }
        //[HttpPost, ActionName("Permissions")]
        [HttpPost]
        public virtual ActionResult PermissionsSave(int UserRoleId, int[] Permissions, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();
            JsonResultHelper result = new JsonResultHelper();
            UserRole userRoles = _userService.GetUserRoleById(UserRoleId);
            List<PermissionRecord> PermsToRemove = userRoles.PermissionRecords.Where(a => !Permissions.Contains(a.Id)).ToList();
            IEnumerable<int> PermToAdd = Permissions.Where(a => !userRoles.PermissionRecords.Select(r => r.Id).ToList().Contains(a));//userRoles.PermissionRecords.Where(a => !Permissions.Contains(a.Id) && !PermsToRemove.Select(e => e.Id).Contains(a.Id)).ToList();
            PermsToRemove.ForEach(a => { userRoles.PermissionRecords.Remove(a); _permissionService.UpdatePermissionRecord(a); });
            PermToAdd.ToList().ForEach(a => { userRoles.PermissionRecords.Add(_permissionService.GetPermissionRecordById(a)); _permissionService.UpdatePermissionRecord(_permissionService.GetPermissionRecordById(a)); });
            result.Access = true;
            result.data = _permissionService.GetAllPermissionForUserRole(UserRoleId);
            result.Msg.Add(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));
            result.success = true;
            result.url = continueEditing ? string.Empty : Url.Action("Index", "Home");
            return Json(result);
        }
        #endregion
    }
}
