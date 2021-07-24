using Seagull.Admin.Helpers;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Users;
using Seagull.Admin.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seagull.Services.Localization;
using Seagull.Services.Security;
using Seagull.Core.Caching;
using Seagull.Services.Users.Cache;
namespace Seagull.Admin.Controllers
{
    public class ChangePasswordController : BaseAdminController
    {

        
		#region Fields
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly UserSettings _userSettings;
        private readonly IWorkContext _iWorkContext;
        private readonly ILocalizationService _iLocalizationService;
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors
        public ChangePasswordController(

            IUserRegistrationService userRegistrationService,
            UserSettings userSettings,
            IWorkContext iWorkContext,
            ILocalizationService iLocalizationService,
            IUserService userService,
            IEncryptionService encryptionService,
            ICacheManager cacheManager,
             IPermissionService permissionService)
        {

            this._userRegistrationService = userRegistrationService;
            this._userSettings = userSettings;
            this._iWorkContext = iWorkContext;
            this._iLocalizationService = iLocalizationService;
            this._userService = userService;
            this._encryptionService = encryptionService;
            this._cacheManager = cacheManager;
            this._permissionService = permissionService;
        }
        public ChangePasswordController()
		{
           
		}

		#endregion 

      
        // GET: ChangePassword
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            
            
                //if (!_permissionService.Authorize(StandardPermissionProvider.ViewChangePassword))
                //   // if (!_iWorkContext.CurrentUser.PasswordIsExpired())
                //    return AccessDeniedView();

            return View(0);
        }

        [HttpPost]
        public ActionResult CreateOrEditModel(int Id = 0)
        {
           
                //if (!_permissionService.Authorize(StandardPermissionProvider.EditChangePassword))
                //   // if (!_iWorkContext.CurrentUser.PasswordIsExpired())
                //        return AccessDeniedJson();

            JsonResultHelper result = new JsonResultHelper();
            result.data = new ChangePasswordModel();
            result.Access = true;
            result.success = true;
            return Json(result);
        }
        [HttpPost]
        public virtual ActionResult CreateOrEdit(ChangePasswordModel model, bool continueEditing)
        {
           
                //if (!_permissionService.Authorize(StandardPermissionProvider.EditChangePassword))
                //   // if (!_iWorkContext.CurrentUser.PasswordIsExpired())
                //        return AccessDeniedJson();
            
            JsonResultHelper result = new JsonResultHelper();
            if (ModelState.IsValid)
            {
                var Email = _iWorkContext.CurrentUser.Email;
                var UserId = _iWorkContext.CurrentUser.Id;
                var UserPass = _userService.GetUserPasswords(UserId).LastOrDefault();
                var oldPass = UserPass.Password;
                var saltKey = UserPass.PasswordSalt;
                var EnteredOldPass = _encryptionService.CreatePasswordHash(model.OldPassword, saltKey, _userSettings.HashedPasswordFormat);
                if (oldPass.Equals(EnteredOldPass))
                {
                    if (model.NewPassword == model.ConfirmNewPassword)
                    {
                        var changePassRequest = new ChangePasswordRequest(Email, false, _userSettings.DefaultPasswordFormat, model.NewPassword);
                        var changePassResult = _userRegistrationService.ChangePassword(changePassRequest, false);
                        if (!changePassResult.Success)
                        {
                            result.success = false;
                            result.Msg.Add(_iLocalizationService.GetResource("Admin.User.PasswordDidNotChange"));
                        }
                    }
                    else
                    {
                        result.success = false;
                        result.Msg.Add(_iLocalizationService.GetResource("Admin.User.NotMachPassword"));
                    }
                }
                else
                {
                    result.success = false;
                    result.Msg.Add(_iLocalizationService.GetResource("Admin.User.UnCorrectOldPassword"));
                }
            }
            //result.Msg.Add(_iLocalizationService.GetLocaleStringResourceByName("Account.CheckUsernameAvailability.CurrentUsername").ToString());
            if (result.success)
            {
                result.Msg.Add(_iLocalizationService.GetResource("Admin.User.PasswordChanged"));
                result.url = Url.Action("Index", "Home");
            }
            return Json(result);
        }


        //public virtual ActionResult CreateOrEdit(ChangePasswordModel model, bool continueEditing)
        //{
        //    JsonResultHelper result = new JsonResultHelper();
        //    if (ModelState.IsValid)
        //    {
        //        var Email = _iWorkContext.CurrentUser.Email;
        //        var UserId = _iWorkContext.CurrentUser.Id;
        //        var UserPass = _userService.GetUserPasswords(UserId).FirstOrDefault();
        //        var oldPass = UserPass.Password;
        //        var saltKey = UserPass.PasswordSalt;
        //        UserPassword _pass = _userService.GetUserPasswords(_iWorkContext.CurrentUser.Id).FirstOrDefault();
        //        string pwd = "";
        //        var newOldPass = _encryptionService.CreatePasswordHash(model.OldPassword, saltKey, _userSettings.HashedPasswordFormat);
        //        switch (_pass.PasswordFormat)
        //        {
        //            case PasswordFormat.Encrypted:
        //                pwd = _encryptionService.EncryptText(model.OldPassword);
        //                break;
        //            case PasswordFormat.Hashed:
        //                pwd = _encryptionService.CreatePasswordHash(model.OldPassword, _pass.PasswordSalt, _userSettings.HashedPasswordFormat);
        //                break;
        //            default:
        //                pwd = "";
        //                break;
        //        }
        //        if (oldPass == pwd)
        //        {
        //            if (model.NewPassword == model.ConfirmNewPassword)
        //            {
        //                var changePassRequest = new ChangePasswordRequest(Email, false, _userSettings.DefaultPasswordFormat, model.NewPassword);
        //                var changePassResult = _userRegistrationService.ChangePassword(changePassRequest, true);
        //                if (!changePassResult.Success)
        //                {
        //                    result.success = false;
        //                    result.Msg.Add(_iLocalizationService.GetLocaleStringResourceByName("Admin.User.PasswordDidNotChange").ToString());
        //                }
        //            }
        //            else
        //            {
        //                result.success = false;
        //                result.Msg.Add(_iLocalizationService.GetLocaleStringResourceByName("Admin.User.NotMachPassword").ToString());
        //            }
        //        }
        //        else
        //        {
        //            result.success = false;
        //            result.Msg.Add(_iLocalizationService.GetLocaleStringResourceByName("Admin.User.UnCorrectOldPassword").ToString());
        //        }
        //    }

        //    return Json(result);
        //}
    }
}