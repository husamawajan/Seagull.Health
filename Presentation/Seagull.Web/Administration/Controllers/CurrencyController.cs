using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Admin.Models.Directory;
using Seagull.Core;
using Seagull.Core.Domain.Directory;
using Seagull.Services.Configuration;
using Seagull.Services.Directory;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Security;
using Seagull.Services.Stores;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Kendoui;

namespace Seagull.Admin.Controllers
{
    public partial class CurrencyController :  BaseAdminController
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ISettingService _settingService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUserActivityService _userActivityService;

        #endregion

        #region Ctor

        public CurrencyController(ICurrencyService currencyService, 
            CurrencySettings currencySettings, 
            ISettingService settingService,
            IDateTimeHelper dateTimeHelper, 
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ILocalizedEntityService localizedEntityService, 
            ILanguageService languageService,
            IStoreService storeService, 
            IStoreMappingService storeMappingService,
            IUserActivityService userActivityService)
        {
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._settingService = settingService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._userActivityService = userActivityService;
        }
        
        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Currency currency, CurrencyModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(currency, x => x.Name, localized.Name, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(CurrencyModel model, Currency currency, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && currency != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(currency).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Currency currency, CurrencyModel model)
        {
            currency.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(currency);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(currency, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region Methods

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List(bool liveRates = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (liveRates)
            {
                try
                {
                    var primaryExchangeCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
                    if (primaryExchangeCurrency == null)
                        throw new SeagullException("Primary exchange rate currency is not set");

                    ViewBag.Rates = _currencyService.GetCurrencyLiveRates(primaryExchangeCurrency.CurrencyCode);
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }
            ViewBag.ExchangeRateProviders = new List<SelectListItem>();
            foreach (var erp in _currencyService.LoadAllExchangeRateProviders())
            {
                ViewBag.ExchangeRateProviders.Add(new SelectListItem
                {
                    Text = erp.PluginDescriptor.FriendlyName,
                    Value = erp.PluginDescriptor.SystemName,
                    Selected = erp.PluginDescriptor.SystemName.Equals(_currencySettings.ActiveExchangeRateProviderSystemName, StringComparison.InvariantCultureIgnoreCase)
                });
            }
            ViewBag.AutoUpdateEnabled = _currencySettings.AutoUpdateEnabled;
           
            return View();
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult List(FormCollection formValues)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.ActiveExchangeRateProviderSystemName = formValues["exchangeRateProvider"];
            _currencySettings.AutoUpdateEnabled = !formValues["autoUpdateEnabled"].Equals("false");
            _settingService.SaveSetting(_currencySettings);
            return RedirectToAction("List", "Currency");
        }

        [HttpPost]
        public virtual ActionResult ListGrid(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedKendoGridJson();

            var currenciesModel = _currencyService.GetAllCurrencies(true).Select(x => x.ToModel()).ToList();
            foreach (var currency in currenciesModel)
                currency.IsPrimaryExchangeRateCurrency = currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId;
            foreach (var currency in currenciesModel)
                currency.IsPrimaryStoreCurrency = currency.Id == _currencySettings.PrimaryStoreCurrencyId;

            var gridModel = new DataSourceResult
            {
                Data = currenciesModel,
                Total = currenciesModel.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ApplyRate(string currencyCode, decimal rate)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyByCode(currencyCode);
            if (currency != null)
            {
                currency.Rate = rate;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);
            }

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual ActionResult MarkAsPrimaryExchangeRateCurrency(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.PrimaryExchangeRateCurrencyId = id;
            _settingService.SaveSetting(_currencySettings);

            return Json(new { result = true });
        }

        [HttpPost]
        public virtual ActionResult MarkAsPrimaryStoreCurrency(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            _currencySettings.PrimaryStoreCurrencyId = id;
            _settingService.SaveSetting(_currencySettings);

            return Json(new { result = true });
        }

        #endregion

        #region Create / Edit / Delete

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var model = new CurrencyModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.Published = true;
            model.Rate = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var currency = model.ToEntity();
                currency.CreatedOnUtc = DateTime.UtcNow;
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.InsertCurrency(currency);

                //activity log
                _userActivityService.InsertActivity("AddNewCurrency", _localizationService.GetResource("ActivityLog.AddNewCurrency"), currency.Id);

                //locales
                UpdateLocales(currency, model);
                //Stores
                SaveStoreMappings(currency, model);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Added"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = currency.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //Stores
            PrepareStoresMappingModel(model, null, true);

            return View(model);
        }
        
        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null)
                //No currency found with the specified id
                return RedirectToAction("List");

            var model = currency.ToModel();
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = currency.GetLocalized(x => x.Name, languageId, false, false);
            });
            //Stores
            PrepareStoresMappingModel(model, currency, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(CurrencyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(model.Id);
            if (currency == null)
                //No currency found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //ensure we have at least one published currency
                var allCurrencies = _currencyService.GetAllCurrencies();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id &&
                    !model.Published)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                currency = model.ToEntity(currency);
                currency.UpdatedOnUtc = DateTime.UtcNow;
                _currencyService.UpdateCurrency(currency);

                //activity log
                _userActivityService.InsertActivity("EditCurrency", _localizationService.GetResource("ActivityLog.EditCurrency"), currency.Id);

                //locales
                UpdateLocales(currency, model);
                //Stores
                SaveStoreMappings(currency, model);


                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Updated"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new {id = currency.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(currency.CreatedOnUtc, DateTimeKind.Utc);

            //Stores
            PrepareStoresMappingModel(model, currency, true);

            return View(model);
        }
        
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCurrencies))
                return AccessDeniedView();

            var currency = _currencyService.GetCurrencyById(id);
            if (currency == null)
                //No currency found with the specified id
                return RedirectToAction("List");
            
            try
            {
                if (currency.Id == _currencySettings.PrimaryStoreCurrencyId)
                    throw new SeagullException(_localizationService.GetResource("Admin.Configuration.Currencies.CantDeletePrimary"));

                if (currency.Id == _currencySettings.PrimaryExchangeRateCurrencyId)
                    throw new SeagullException(_localizationService.GetResource("Admin.Configuration.Currencies.CantDeleteExchange"));

                //ensure we have at least one published currency
                var allCurrencies = _currencyService.GetAllCurrencies();
                if (allCurrencies.Count == 1 && allCurrencies[0].Id == currency.Id)
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Configuration.Currencies.PublishedCurrencyRequired"));
                    return RedirectToAction("Edit", new { id = currency.Id });
                }

                _currencyService.DeleteCurrency(currency);

                //activity log
                _userActivityService.InsertActivity("DeleteCurrency", _localizationService.GetResource("ActivityLog.DeleteCurrency"), currency.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Currencies.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = currency.Id });
            }
        }

        #endregion
    }
}
