﻿using System;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Admin.Models.Stores;
using Seagull.Core.Domain.Stores;
using Seagull.Services.Configuration;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Security;
using Seagull.Services.Stores;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Kendoui;

namespace Seagull.Admin.Controllers
{
    public partial class StoreController : BaseAdminController
    {
        #region Fields

        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IUserActivityService _userActivityService;

        #endregion

        #region Constructors

        public StoreController(IStoreService storeService,
            ISettingService settingService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IUserActivityService userActivityService)
        {
            this._storeService = storeService;
            this._settingService = settingService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._userActivityService = userActivityService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void PrepareLanguagesModel(StoreModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            
            model.AvailableLanguages.Add(new SelectListItem
            {
                Text = "---",
                Value = "0"
            });
            var languages = _languageService.GetAllLanguages(true);
            foreach (var language in languages)
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void UpdateAttributeLocales(Store store, StoreModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(store,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedKendoGridJson();

            var storeModels = _storeService.GetAllStores()
                .Select(x => x.ToModel())
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = storeModels,
                Total = storeModels.Count()
            };

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = new StoreModel();
            //languages
            PrepareLanguagesModel(model);
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(StoreModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                var store = model.ToEntity();
                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";
                _storeService.InsertStore(store);

                //activity log
                _userActivityService.InsertActivity("AddNewStore", _localizationService.GetResource("ActivityLog.AddNewStore"), store.Id);

                //locales
                UpdateAttributeLocales(store, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");

            var model = store.ToModel();
            //languages
            PrepareLanguagesModel(model);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = store.GetLocalized(x => x.Name, languageId, false, false);
            });
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(StoreModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(model.Id);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                store = model.ToEntity(store);
                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";
                _storeService.UpdateStore(store);

                //activity log
                _userActivityService.InsertActivity("EditStore", _localizationService.GetResource("ActivityLog.EditStore"), store.Id);

                //locales
                UpdateAttributeLocales(store, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //languages
            PrepareLanguagesModel(model);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var store = _storeService.GetStoreById(id);
            if (store == null)
                //No store found with the specified id
                return RedirectToAction("List");

            try
            {
                _storeService.DeleteStore(store);

                //activity log
                _userActivityService.InsertActivity("DeleteStore", _localizationService.GetResource("ActivityLog.DeleteStore"), store.Id);

                //when we delete a store we should also ensure that all "per store" settings will also be deleted
                var settingsToDelete = _settingService
                    .GetAllSettings()
                    .Where(s => s.StoreId == id)
                    .ToList();
                    _settingService.DeleteSettings(settingsToDelete);
                //when we had two stores and now have only one store, we also should delete all "per store" settings
                var allStores = _storeService.GetAllStores();
                if (allStores.Count == 1)
                {
                    settingsToDelete = _settingService
                        .GetAllSettings()
                        .Where(s => s.StoreId == allStores[0].Id)
                        .ToList();
                        _settingService.DeleteSettings(settingsToDelete);
                }


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new {id = store.Id});
            }
        }

        #endregion

    }
}
