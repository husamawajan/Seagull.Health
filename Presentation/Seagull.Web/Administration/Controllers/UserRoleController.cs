using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Admin.Helpers;
using Seagull.Admin.Models.Users;
using Seagull.Core;
using Seagull.Core.Caching;
using Seagull.Core.Domain.Users;
using Seagull.Services;
using Seagull.Services.Users;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Security;
using Seagull.Services.Stores;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Kendoui;
using Newtonsoft.Json;
using Seagull.Services.Helpers;
using Seagull.Web.Framework;
namespace Seagull.Admin.Controllers
{
    public partial class UserRoleController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public UserRoleController(IUserService userService,
            ILocalizationService localizationService,
            IUserActivityService userActivityService,
            IPermissionService permissionService,

            IStoreService storeService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._userService = userService;
            this._localizationService = localizationService;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual UserRoleModel PrepareUserRoleModel(UserRole userRole)
        {
            var model = userRole.ToModel();
            return model;
        }

        #endregion

        #region User roles

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            return View();
        }
        public virtual ActionResult ListUserRoles(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var userRoles = _userService.GetAllUserRoles(pagination, sort, search, search_operator, filter, true);
            var AngularTable = new DataSourceAngular
            {
                data = userRoles.Select(PrepareUserRoleModel),
                data_count = userRoles.TotalCount,
                page_count = userRoles.TotalPages,

            };
            return Json(AngularTable);
        }
        public virtual ActionResult PrepareUserRole(int Id = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();
            return View(Id);
        }
        public virtual ActionResult CreateOrEditModel(int Id = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();
            JsonResultHelper result = new JsonResultHelper();
            var model = new UserRoleModel();
            //default values
            model.Active = true;
            if (Id > 0)
            {
                var userRole = _userService.GetUserRoleById(Id);
                if (userRole == null)
                    //No user role found with the specified id
                    return RedirectToAction("List");
                model = PrepareUserRoleModel(userRole);
                result.data = model;
                return Json(result);
            }
            return Json(result);
        }
        [HttpPost]
        public virtual ActionResult CreateOrEdit(UserRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();
            JsonResultHelper result = new JsonResultHelper();
            model.SystemName = model.Name;
            if (ModelState.IsValid)
            {
                UserRole userRole = new UserRole();
                if (model.Id > 0)
                {
                    // Update Entity
                    userRole = _userService.GetUserRoleById(model.Id);
                    userRole.Name = model.Name;
                    userRole.SystemName = model.Name;
                    if (userRole.IsSystemRole && !model.Active)
                    {
                        result.Msg.Add(_localizationService.GetResource("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));
                        result.success = false;
                    }

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result.Msg.Add(_localizationService.GetResource("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));
                        result.success = false;
                    }
                    if (SystemUserRoleNames.Registered.Equals(userRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithProductId > 0)
                    {
                        result.Msg.Add(_localizationService.GetResource("Admin.Users.UserRoles.Fields.PurchasedWithProduct.Registered"));
                        result.success = false;
                    }
                    if (result.success)
                    {
                        userRole = model.ToEntity(userRole);
                        _userService.UpdateUserRole(userRole);
                        //activity log
                        _userActivityService.InsertActivity("EditUserRole", _localizationService.GetResource("ActivityLog.EditUserRole"), userRole.Name);
                        //SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Updated"));
                        result.Msg.Add(_localizationService.GetResource("Admin.Users.UserRoles.Updated"));
                    }
                }
                else
                {
                    //Create Entity
                    userRole = model.ToEntity();
                    _userService.InsertUserRole(userRole);

                    result.Msg.Add(_localizationService.GetResource("Admin.Users.UserRoles.Added"));
                    //activity log
                    _userActivityService.InsertActivity("AddNewUserRole", _localizationService.GetResource("ActivityLog.AddNewUserRole"), userRole.Name);
                }
                result.Id = userRole.Id;
                result.data = userRole.ToModel();
                result.success = true;
                result.url = continueEditing ? Url.Action("PrepareUserRole", "UserRole", new { seagull = UrlHelperExtensions.ActionEncodedCustom(new { id = userRole.Id }) }) : Url.Action("List", "UserRole");
                return Json(result);
            }
            //If we got this far, something failed, redisplay form
            result.Id = model.Id;
            result.success = false;
            result.FormErrors = GetErrorsFromModelState();
            result.data = null;
            result.url = string.Empty;
            return Json(result);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(UserRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var userRole = model.ToEntity();
                _userService.InsertUserRole(userRole);

                //activity log
                _userActivityService.InsertActivity("AddNewUserRole", _localizationService.GetResource("ActivityLog.AddNewUserRole"), userRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                //No user role found with the specified id
                return RedirectToAction("List");

            var model = PrepareUserRoleModel(userRole);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(UserRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(model.Id);
            if (userRole == null)
                //No user role found with the specified id
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (userRole.IsSystemRole && !model.Active)
                        throw new SeagullException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new SeagullException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));

                    if (SystemUserRoleNames.Registered.Equals(userRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithProductId > 0)
                        throw new SeagullException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.PurchasedWithProduct.Registered"));

                    userRole = model.ToEntity(userRole);
                    _userService.UpdateUserRole(userRole);

                    //activity log
                    _userActivityService.InsertActivity("EditUserRole", _localizationService.GetResource("ActivityLog.EditUserRole"), userRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();
            JsonResultHelper data = new JsonResultHelper();
            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                //No user role found with the specified id
                return RedirectToAction("List");

            try
            {
                _userService.DeleteUserRole(userRole);

                //activity log
                _userActivityService.InsertActivity("DeleteUserRole", _localizationService.GetResource("ActivityLog.DeleteUserRole"), userRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Deleted"));
                data.success = true;
                data.url = Url.Action("List", "UserRole").ToString();

            }
            catch (Exception exc)
            {
                data.success = false;
                data.Msg.Add(exc.Message);
            }
            return Json(data);

        }






        #endregion
    }
}
