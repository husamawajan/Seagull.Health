using System;
using Seagull.Core;
using System.Linq;
using System.Web.Mvc;
using Seagull.Core.Domain.Common;
using Seagull.Services.Localization;
using Seagull.Services.Security;
using Seagull.Web.Framework.Controllers;
using Seagull.Services.Emails;
using Seagull.Admin.Models.Emails;
using Seagull.Admin.Extensions;
using Seagull.Web.Framework.Kendoui;
using Newtonsoft.Json;
using Seagull.Services.Helpers;
using Seagull.Core.Domain.Emails;
using Seagull.Services.Logging;
using Seagull.Admin.Helpers;

namespace Seagull.Admin.Controllers
{
    [AdminAuthorize]
    public class EmailController : BaseAdminController
    {
        #region Fields                                                                                                                                              

        private readonly IEmailService _emailService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors                                                                                                                                        

        public EmailController(IEmailService emailService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings,
            IUserActivityService userActivityService,
            IWorkContext workContext)
        {
            this._localizationService = localizationService;
            this._emailService = emailService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._userActivityService = userActivityService;
            this._workContext = workContext;
        }

        #endregion

        #region Emails                                                                                                                                        

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            // if (!_permissionService.Authorize(StandardPermissionProvider.ViewEmail))                                                                        
            // return AccessDeniedView();                                                                                                                          

            return View();
        }

        public ActionResult ListEmails(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ViewEmail))                                                                          
            // return AccessDeniedJson();                                                                                                                          
            var emails = _emailService.GetAllEmails(pagination, sort, search, search_operator, filter, 0, true);
            var AngularTable = new DataSourceAngular
            {
                data = emails.Select(x => x.ToModel()),
                data_count = emails.TotalCount,
                page_count = emails.TotalPages,
            };
            return Json(AngularTable);
        }

        public ActionResult PrepareEmail(int Id = 0)
        {
            //  if (Id > 0)                                                                                                                                             
            // if (!_permissionService.Authorize(StandardPermissionProvider.EditEmail))                                                                      
            //   return AccessDeniedView();                                                                                                                      
            // else if (!_permissionService.Authorize(StandardPermissionProvider.AddEmail))                                                                  
            // return AccessDeniedView();                                                                                                                      
            return View(Id);
        }

        public ActionResult CreateOrEditModel(int Id = 0)
        {
            JsonResultHelper result = new JsonResultHelper();
            var model = new EmailModel();
            if (Id > 0)
            {
                //if (!_permissionService.Authorize(StandardPermissionProvider.EditEmail))                                                                      
                //    return AccessDeniedJson();                                                                                                                      
                var email = _emailService.GetEmailById(Id);
                if (email == null)
                    //No Email found with the specified id                                                                                                    
                    return RedirectToAction("List");
                model = email.ToModel();
                result.data = model;
                return Json(result);
            }
            //if (!_permissionService.Authorize(StandardPermissionProvider.AddEmail))                                                                           
            //  return AccessDeniedJson();                                                                                                                          
            result.data = model;
            //default values                                                                                                                                        
            return Json(result);
        }

        public ActionResult CreateOrEdit(EmailModel model, bool continueEditing)
        {
            JsonResultHelper result = new JsonResultHelper();
            if (ModelState.IsValid)
            {
                Email email = new Email();
                if (model.Id > 0)
                {
                    //if (!_permissionService.Authorize(StandardPermissionProvider.EditEmail))                                                                  
                    //   return AccessDeniedJson();                                                                                                                  
                    // Update Entity                                                                                                                                
                    if (result.success)
                    {
                        email = _emailService.GetEmailById(model.Id);
                        email = model.ToEntity(email);
                        _emailService.UpdateEmail(email);
                        _userActivityService.InsertActivity("PublicStore.Edit", _localizationService.GetResource("ActivityLog.EditEmail"), email.Id);
                    }
                }
                else
                {
                    //if (!_permissionService.Authorize(StandardPermissionProvider.AddEmail))                                                                   
                    //    return AccessDeniedJson();                                                                                                                  
                    //Create Entity                                                                                                                                 
                    email = model.ToEntity();
                    _emailService.InsertEmail(email);
                    //activity log                                                                                                                                  
                    _userActivityService.InsertActivity("PublicStore.Add", _localizationService.GetResource("ActivityLog.AddNewEmail"), email.Id);
                }
                result.Id = email.Id;
                result.data = email.ToModel();
                result.url = continueEditing ? Url.Action("PrepareEmail", "Email", new { id = email.Id }) : Url.Action("List", "Email");
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
            // if (!_permissionService.Authorize(StandardPermissionProvider.DeleteEmail))                                                                     
            //  return AccessDeniedView();                                                                                                                          
            JsonResultHelper data = new JsonResultHelper();
            var email = _emailService.GetEmailById(id);
            if (email == null)
                //No classificaion found with the specified id                                                                                                      
                return RedirectToAction("List");

            _emailService.DeleteEmail(email);
            _userActivityService.InsertActivity("DeleteEmail", _localizationService.GetResource("ActivityLog.DeleteEmail"), email.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Emails.Deleted"));
            data.success = true;
            data.url = Url.Action("List", "Email").ToString();
            return Json(data);
        }

        #endregion
    }
}