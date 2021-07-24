using System;
using Seagull.Core;
using System.Linq;
using System.Web.Mvc;
using Seagull.Core.Domain.Common;
using Seagull.Services.Localization;
using Seagull.Services.Security;
using Seagull.Web.Framework.Controllers;
using Seagull.Services.UserTypes;
using Seagull.Admin.Models.UserTypes;
using Seagull.Admin.Extensions;
using Seagull.Web.Framework.Kendoui;
using Newtonsoft.Json;
using Seagull.Services.Helpers;
using Seagull.Core.Domain.UserTypes;
using Seagull.Services.Logging;
using Seagull.Admin.Helpers;

namespace Seagull.Admin.Controllers
{
	[AdminAuthorize]
    public class UserTypeController : BaseAdminController
    {
        #region Fields

        private readonly IUserTypeService _usertypeService;
		private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
	    private readonly IUserActivityService _userActivityService;
		private readonly IWorkContext _workContext;

		#endregion

		#region Constructors

        public UserTypeController(IUserTypeService usertypeService, 
			ILocalizationService localizationService,
            IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings,
			IUserActivityService userActivityService,
			IWorkContext workContext)
		{
			this._localizationService = localizationService;
            this._usertypeService = usertypeService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
			this._userActivityService = userActivityService;
			this._workContext = workContext;
		}

		#endregion

		#region UserTypes

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            // if (!_permissionService.Authorize(StandardPermissionProvider.ViewUserType))
          //  return AccessDeniedView();

				 return View();
		}

		public ActionResult ListUserTypes(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
             // if (!_permissionService.Authorize(StandardPermissionProvider.ViewUserType))
          //  return AccessDeniedView();
                var usertypes = _usertypeService.GetAllUserTypes(pagination, sort, search, search_operator, filter, true);
				var AngularTable = new DataSourceAngular
				{
					data = usertypes.Select(x => x.ToModel()),
					data_count = usertypes.TotalCount,
					 page_count = usertypes.TotalPages,
				};
                 return Json(AngularTable);
		}
        
        public ActionResult PrepareUserType(int Id = 0)
        {
                // if (!_permissionService.Authorize(StandardPermissionProvider.UserType))
          //  return AccessDeniedView();
			    return View(Id);
        }

        public ActionResult CreateOrEditModel(int Id = 0)
        {
              // if (!_permissionService.Authorize(StandardPermissionProvider.UserType))
          //  return AccessDeniedView();
            JsonResultHelper result = new JsonResultHelper();
            var model = new UserTypeModel();
            // if (!_permissionService.Authorize(StandardPermissionProvider.UserEntity))
            //  return AccessDeniedView();
            if (Id > 0)
            {
                var useretype = _usertypeService.GetUserTypeById(Id);
                if (useretype == null)
                    //No UserEntity found with the specified id
                    return RedirectToAction("List");
                model = useretype.ToModel();
                result.data = model;
                return Json(result);
            }
            result.data = model;
            //default values
            return Json(result);
        }

		public ActionResult CreateOrEdit(UserTypeModel model, bool continueEditing)
        {
		   JsonResultHelper result = new JsonResultHelper();
		    if (ModelState.IsValid)
            {
			UserType usertype = new UserType();
                if (model.Id > 0)
                {
				// if (!_permissionService.Authorize(StandardPermissionProvider.EditUserType))
          //  return AccessDeniedView();
			     	// Update Entity
					if (result.success)
                    {
                    usertype  = _usertypeService.GetUserTypeById(model.Id);
                   	usertype = model.ToEntity(usertype);
                    usertype.UpdatedBy = _workContext.CurrentUser.Id;
                    usertype.UpdatedDate = DateTime.Now;
					_usertypeService.UpdateUserType(usertype);
					 _userActivityService.InsertActivity("EditUserType", _localizationService.GetResource("ActivityLog.EditUserType"), usertype.Id);
					 }
			    }
					 else
                {
					// if (!_permissionService.Authorize(StandardPermissionProvider.AddUserType))
          //  return AccessDeniedView();
                    //Create Entity
                    	usertype = model.ToEntity();
                        usertype.CreatedBy = _workContext.CurrentUser.Id;
                        usertype.CreatedDate = DateTime.Now;
                    _usertypeService.InsertUserType(usertype);
                    //activity log
                    _userActivityService.InsertActivity("AddNewUserType", _localizationService.GetResource("ActivityLog.AddNewUserType"),  usertype.Id);
                }
					 result.Id = usertype.Id;
                result.data = usertype.ToModel();
                result.url = continueEditing ? Url.Action("PrepareUserType", "UserType", new { id = usertype.Id }) : Url.Action("List", "UserType");
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

		 [HttpPost]
		public ActionResult Delete(int id)
        {	
		// if (!_permissionService.Authorize(StandardPermissionProvider.DeleteUserType))
          //  return AccessDeniedView();
		       JsonResultHelper data = new JsonResultHelper();
				var usertype = _usertypeService.GetUserTypeById(id);
				if (usertype == null)
					//No classificaion found with the specified id
					return RedirectToAction("List");

				_usertypeService.DeleteUserType(usertype);
			    _userActivityService.InsertActivity("DeleteUserType", _localizationService.GetResource("ActivityLog.DeleteUserType"), usertype.Id);

				SuccessNotification(_localizationService.GetResource("Admin.Configuration.UserTypes.Deleted"));
				 data.success = true;
                data.url = Url.Action("List", "UserType").ToString();
				 return Json(data);
		}

		#endregion
    }
}
	
