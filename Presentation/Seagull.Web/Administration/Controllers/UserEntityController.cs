using System;
using Seagull.Core;
using System.Linq;
using System.Web.Mvc;
using Seagull.Core.Domain.Common;
using Seagull.Services.Localization;
using Seagull.Services.Security;
using Seagull.Web.Framework.Controllers;
using Seagull.Services.UserEntitys;
using Seagull.Admin.Models.UserEntitys;
using Seagull.Admin.Extensions;
using Seagull.Web.Framework.Kendoui;
using Newtonsoft.Json;
using Seagull.Services.Helpers;
using Seagull.Core.Domain.UserEntitys;
using Seagull.Services.Logging;
using Seagull.Admin.Helpers;
using Seagull.Services.Users;
using Seagull.Core.Domain.Users;

namespace Seagull.Admin.Controllers
{
	[AdminAuthorize]
    public class UserEntityController : BaseAdminController
    {
        #region Fields

        private readonly IUserEntityService _userentityService;
		private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
	    private readonly IUserActivityService _userActivityService;
		private readonly IWorkContext _workContext;
        private readonly IUserService _userService;

		#endregion

		#region Constructors

        public UserEntityController(IUserEntityService userentityService, 
			ILocalizationService localizationService,
            IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings,
			IUserActivityService userActivityService,
			IWorkContext workContext,
            IUserService userService)
		{
			this._localizationService = localizationService;
            this._userentityService = userentityService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
			this._userActivityService = userActivityService;
			this._workContext = workContext;
            this._userService = userService;
		}

		#endregion

		#region UserEntitys

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            // if (!_permissionService.Authorize(StandardPermissionProvider.ViewUserEntity))
          //  return AccessDeniedView();

				 return View();
		}

		public ActionResult ListUserEntitys(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
             // if (!_permissionService.Authorize(StandardPermissionProvider.ViewUserEntity))
          //  return AccessDeniedView();
                var userentitys = _userentityService.GetAllUserEntitys(pagination, sort, search, search_operator, filter, true);
				var AngularTable = new DataSourceAngular
				{
					data = userentitys.Select(x => x.ToModel()),
					data_count = userentitys.TotalCount,
					 page_count = userentitys.TotalPages,
				};
                 return Json(AngularTable);
		}
        
        public ActionResult PrepareUserEntity(int Id = 0)
        {
                // if (!_permissionService.Authorize(StandardPermissionProvider.UserEntity))
          //  return AccessDeniedView();
			    return View(Id);
        }

        public ActionResult CreateOrEditModel(int Id = 0)
        {
            JsonResultHelper result = new JsonResultHelper();
            var model = new UserEntityModel();
              // if (!_permissionService.Authorize(StandardPermissionProvider.UserEntity))
          //  return AccessDeniedView();
			  if (Id > 0)
            {
                var userentity = _userentityService.GetUserEntityById(Id);
                if (userentity == null)
                    //No UserEntity found with the specified id
                    return RedirectToAction("List");
                model = userentity.ToModel();
                result.data = model;
                return Json(result);
            }
              result.data = model;
            //default values
              return Json(result);
        }

		public ActionResult CreateOrEdit(UserEntityModel model, bool continueEditing)
        {
		   JsonResultHelper result = new JsonResultHelper();
		    if (ModelState.IsValid)
            {
			UserEntity userentity = new UserEntity();
                if (model.Id > 0)
                {
				// if (!_permissionService.Authorize(StandardPermissionProvider.EditUserEntity))
          //  return AccessDeniedView();
			     	// Update Entity
					if (result.success)
                    {
                    userentity  = _userentityService.GetUserEntityById(model.Id);
                   	userentity = model.ToEntity(userentity);
                    userentity.UpdatedBy = _workContext.CurrentUser.Id;
                    userentity.UpdatedDate = DateTime.Now;
					_userentityService.UpdateUserEntity(userentity);
					 _userActivityService.InsertActivity("EditUserEntity", _localizationService.GetResource("ActivityLog.EditUserEntity"), userentity.Id);
					 }
			    }
					 else
                {
					// if (!_permissionService.Authorize(StandardPermissionProvider.AddUserEntity))
          //  return AccessDeniedView();
                    //Create Entity
                    	userentity = model.ToEntity();
                        userentity.CreatedBy = _workContext.CurrentUser.Id;
                        userentity.CreatedDate = DateTime.Now;
                    _userentityService.InsertUserEntity(userentity);
                    //activity log
                    _userActivityService.InsertActivity("AddNewUserEntity", _localizationService.GetResource("ActivityLog.AddNewUserEntity"),  userentity.Id);
                }
					 result.Id = userentity.Id;
                     result.data = userentity.ToModel(false);
                result.url = continueEditing ? Url.Action("PrepareUserEntity", "UserEntity", new { id = userentity.Id }) : Url.Action("List", "UserEntity");
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
		// if (!_permissionService.Authorize(StandardPermissionProvider.DeleteUserEntity))
          //  return AccessDeniedView();
		       JsonResultHelper data = new JsonResultHelper();
				var userentity = _userentityService.GetUserEntityById(id);
				if (userentity == null)
					//No classificaion found with the specified id
					return RedirectToAction("List");

				_userentityService.DeleteUserEntity(userentity);
			    _userActivityService.InsertActivity("DeleteUserEntity", _localizationService.GetResource("ActivityLog.DeleteUserEntity"), userentity.Id);

				SuccessNotification(_localizationService.GetResource("Admin.Configuration.UserEntitys.Deleted"));
				 data.success = true;
                data.url = Url.Action("List", "UserEntity").ToString();
				 return Json(data);
		}
         public ActionResult ListOfUsers(pagination pagination, sort sort, string search, string search_operator, string filter , int id = 0)
         {
             if (!_permissionService.Authorize(StandardPermissionProvider.AddUserEntity))
                 return AccessDeniedJson();
             string Token = string.Empty;
             PagedList<User> User = null;
           
             if (id > 0)
             {
                 User = _userService.GetAllUsers(pagination, sort, search, search_operator, filter, true, id);     
             }
             if (User == null) return Json(User);
             var AngularTable = new DataSourceAngular
             {
                 data = User.Select(x => x.ToModel()),
                 data_count = User.TotalCount,
                 page_count = User.TotalPages,
             };
             return Json(AngularTable);
         }

		#endregion
    }
}
