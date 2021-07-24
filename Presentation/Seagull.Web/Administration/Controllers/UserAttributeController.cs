using System;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Admin.Models.Users;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Users;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Security;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Kendoui;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Controllers
{
    public partial class UserAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IUserAttributeService _userAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IUserActivityService _userActivityService;

        #endregion

        #region Constructors

        public UserAttributeController(IUserAttributeService userAttributeService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPermissionService permissionService,
            IUserActivityService userActivityService)
        {
            this._userAttributeService = userAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._userActivityService = userActivityService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateAttributeLocales(UserAttribute userAttribute, UserAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(userAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        [NonAction]
        protected  virtual void UpdateValueLocales(UserAttributeValue userAttributeValue, UserAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(userAttributeValue,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }

        #endregion
        
        #region User attributes

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult ListBlock()
        {
            return PartialView("ListBlock");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //we just redirect a user to the user settings page

            //select "user form fields" tab
            SaveSelectedTabName("tab-userformfields");
            return RedirectToAction("UserUser", "Setting");
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var userAttributes = _userAttributeService.GetAllUserAttributes();
            var gridModel = new DataSourceResult
            {
                Data = userAttributes.Select(x =>
                {
                    var attributeModel = x.ToModel();
                    return attributeModel;
                }),
                Total = userAttributes.Count()
            };
            return Json(gridModel);
        }
        
        //create
        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new UserAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(UserAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var userAttribute = model.ToEntity();
                _userAttributeService.InsertUserAttribute(userAttribute);

                //activity log
                _userActivityService.InsertActivity("AddNewUserAttribute", _localizationService.GetResource("ActivityLog.AddNewUserAttribute"), userAttribute.Id);

                //locales
                UpdateAttributeLocales(userAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = userAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(id);
            if (userAttribute == null)
                //No user attribute found with the specified id
                return RedirectToAction("List");

            var model = userAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = userAttribute.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(UserAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(model.Id);
            if (userAttribute == null)
                //No user attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                userAttribute = model.ToEntity(userAttribute);
                _userAttributeService.UpdateUserAttribute(userAttribute);

                //activity log
                _userActivityService.InsertActivity("EditUserAttribute", _localizationService.GetResource("ActivityLog.EditUserAttribute"), userAttribute.Id);

                //locales
                UpdateAttributeLocales(userAttribute, model);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = userAttribute.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(id);
            _userAttributeService.DeleteUserAttribute(userAttribute);

            //activity log
            _userActivityService.InsertActivity("DeleteUserAttribute", _localizationService.GetResource("ActivityLog.DeleteUserAttribute"), userAttribute.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Users.UserAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region User attribute values

        //list
        [HttpPost]
        public virtual ActionResult ValueList(int userAttributeId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var values = _userAttributeService.GetUserAttributeValues(userAttributeId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x => new UserAttributeValueModel
                {
                    Id = x.Id,
                    UserAttributeId = x.UserAttributeId,
                    Name = x.Name,
                    IsPreSelected = x.IsPreSelected,
                    DisplayOrder = x.DisplayOrder,
                }),
                Total = values.Count()
            };
            return Json(gridModel);
        }

        //create
        public virtual ActionResult ValueCreatePopup(int userAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(userAttributeId);
            if (userAttribute == null)
                //No user attribute found with the specified id
                return RedirectToAction("List");

            var model = new UserAttributeValueModel();
            model.UserAttributeId = userAttributeId;
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueCreatePopup(string btnId, string formId, UserAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var userAttribute = _userAttributeService.GetUserAttributeById(model.UserAttributeId);
            if (userAttribute == null)
                //No user attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                var cav = new UserAttributeValue
                {
                    UserAttributeId = model.UserAttributeId,
                    Name = model.Name,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder
                };

                _userAttributeService.InsertUserAttributeValue(cav);

                //activity log
                _userActivityService.InsertActivity("AddNewUserAttributeValue", _localizationService.GetResource("ActivityLog.AddNewUserAttributeValue"), cav.Id);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual ActionResult ValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _userAttributeService.GetUserAttributeValueById(id);
            if (cav == null)
                //No user attribute value found with the specified id
                return RedirectToAction("List");

            var model = new UserAttributeValueModel
            {
                UserAttributeId = cav.UserAttributeId,
                Name = cav.Name,
                IsPreSelected = cav.IsPreSelected,
                DisplayOrder = cav.DisplayOrder
            };

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = cav.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ValueEditPopup(string btnId, string formId, UserAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _userAttributeService.GetUserAttributeValueById(model.Id);
            if (cav == null)
                //No user attribute value found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                cav.Name = model.Name;
                cav.IsPreSelected = model.IsPreSelected;
                cav.DisplayOrder = model.DisplayOrder;
                _userAttributeService.UpdateUserAttributeValue(cav);

                //activity log
                _userActivityService.InsertActivity("EditUserAttributeValue", _localizationService.GetResource("ActivityLog.EditUserAttributeValue"), cav.Id);

                UpdateValueLocales(cav, model);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual ActionResult ValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var cav = _userAttributeService.GetUserAttributeValueById(id);
            if (cav == null)
                throw new ArgumentException("No user attribute value found with the specified id");
            _userAttributeService.DeleteUserAttributeValue(cav);

            //activity log
            _userActivityService.InsertActivity("DeleteUserAttributeValue", _localizationService.GetResource("ActivityLog.DeleteUserAttributeValue"), cav.Id);

            return new NullJsonResult();
        }


        #endregion
    }
}
