using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Admin.Models.Common;
using Seagull.Admin.Models.Settings;
using Seagull.Admin.Models.Stores;
using Seagull.Core;
using Seagull.Core.Configuration;
using Seagull.Core.Domain;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Domain.Media;
using Seagull.Core.Domain.Security;
using Seagull.Core.Domain.Seo;
using Seagull.Services;
using Seagull.Services.Common;
using Seagull.Services.Configuration;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Services.Media;
using Seagull.Services.Security;
using Seagull.Services.Stores;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Kendoui;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Framework.Security;
using Seagull.Web.Framework.Security.Captcha;
using Seagull.Web.Framework.Themes;

namespace Seagull.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
	{
		#region Fields

        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeProvider _themeProvider;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly SeagullConfig _config;
        #endregion

        #region Ctor

        public SettingController(ISettingService settingService,
            ICountryService countryService, 
            IStateProvinceService stateProvinceService,
            IAddressService addressService, 
            ICurrencyService currencyService,
            IPictureService pictureService, 
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IEncryptionService encryptionService,
            IThemeProvider themeProvider,
            IUserService userService, 
            IUserActivityService userActivityService,
            IPermissionService permissionService,
            IFulltextService fulltextService, 
            IMaintenanceService maintenanceService,
            IStoreService storeService,
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            SeagullConfig config)
        {
            this._settingService = settingService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._encryptionService = encryptionService;
            this._themeProvider = themeProvider;
            this._userService = userService;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._config = config;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        [ChildActionOnly]
        public virtual ActionResult Mode(string modeName = "settings-advanced-mode")
        {
            var model = new ModeModel()
            {
                ModeName = modeName,
                Enabled = _workContext.CurrentUser.GetAttribute<bool>(modeName)
            };
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult StoreScopeConfiguration()
        {
            var allStores = _storeService.GetAllStores();
            if (allStores.Count < 2)
                return Content("");

            var model = new StoreScopeConfigurationModel();
            foreach (var s in allStores)
            {
                model.Stores.Add(new StoreModel
                {
                    Id = s.Id,
                    Name = s.Name
                });
            }
            model.StoreId = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            return PartialView(model);
        }
        public virtual ActionResult ChangeStoreScopeConfiguration(int storeid, string returnUrl = "")
        {
            var store = _storeService.GetStoreById(storeid);
            if (store != null || storeid == 0)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentUser,
                    SystemUserAttributeNames.AdminAreaStoreScopeConfiguration, storeid);
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

      
        

        public virtual ActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            var model = mediaSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AvatarPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AvatarPictureSize, storeScope);
                model.ProductThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSize, storeScope);
                model.ProductDetailsPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductDetailsPictureSize, storeScope);
                model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeScope);
                model.AssociatedProductPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AssociatedProductPictureSize, storeScope);
                model.CategoryThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CategoryThumbPictureSize, storeScope);
                model.ManufacturerThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ManufacturerThumbPictureSize, storeScope);
                model.VendorThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.VendorThumbPictureSize, storeScope);
                model.CartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CartThumbPictureSize, storeScope);
                model.MiniCartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MiniCartThumbPictureSize, storeScope);
                model.MaximumImageSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MaximumImageSize, storeScope);
                model.MultipleThumbDirectories_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MultipleThumbDirectories, storeScope);
                model.DefaultImageQuality_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultImageQuality, storeScope);
                model.ImportProductImagesUsingHash_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ImportProductImagesUsingHash, storeScope);
                model.DefaultPictureZoomEnabled_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultPictureZoomEnabled, storeScope);
            }
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;
            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult Media(MediaSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            mediaSettings = model.ToEntity(mediaSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductThumbPictureSize, model.ProductThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductDetailsPictureSize, model.ProductDetailsPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AssociatedProductPictureSize, model.AssociatedProductPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CategoryThumbPictureSize, model.CategoryThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ManufacturerThumbPictureSize, model.ManufacturerThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.VendorThumbPictureSize, model.VendorThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CartThumbPictureSize, model.CartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MiniCartThumbPictureSize, model.MiniCartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MultipleThumbDirectories, model.MultipleThumbDirectories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ImportProductImagesUsingHash, model.ImportProductImagesUsingHash_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultPictureZoomEnabled, model.DefaultPictureZoomEnabled_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();
            
            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public virtual ActionResult ChangePictureStorage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }



        public virtual ActionResult UserUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var userSettings = _settingService.LoadSetting<UserSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

            //merge settings
            var model = new UserUserSettingsModel();
            model.UserSettings = userSettings.ToModel();
            model.AddressSettings = addressSettings.ToModel();

            model.DateTimeSettings.AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            return View(model);
        }
        [HttpPost]
        public virtual ActionResult UserUser(UserUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var userSettings = _settingService.LoadSetting<UserSettings>(storeScope);
            var addressSettings = _settingService.LoadSetting<AddressSettings>(storeScope);
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>(storeScope);

            userSettings = model.UserSettings.ToEntity(userSettings);
            _settingService.SaveSetting(userSettings);

            addressSettings = model.AddressSettings.ToEntity(addressSettings);
            _settingService.SaveSetting(addressSettings);

            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            _settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("UserUser");
        }






        public virtual ActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new GeneralCommonSettingsModel();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            model.ActiveStoreScopeConfiguration = storeScope;
            //store information
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            model.StoreInformationSettings.StoreClosed = storeInformationSettings.StoreClosed;
            //themes
            model.StoreInformationSettings.DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme;
            model.StoreInformationSettings.AvailableStoreThemes = _themeProvider
                .GetThemeConfigurations()
                .Select(x => new GeneralCommonSettingsModel.StoreInformationSettingsModel.ThemeConfigurationModel
                {
                    ThemeTitle = x.ThemeTitle,
                    ThemeName = x.ThemeName,
                    PreviewImageUrl = x.PreviewImageUrl,
                    PreviewText = x.PreviewText,
                    SupportRtl = x.SupportRtl,
                    Selected = x.ThemeName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                })
                .ToList();
            model.StoreInformationSettings.AllowUserToSelectTheme = storeInformationSettings.AllowUserToSelectTheme;
            model.StoreInformationSettings.LogoPictureId = storeInformationSettings.LogoPictureId;
            //EU Cookie law
            model.StoreInformationSettings.DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            model.StoreInformationSettings.FacebookLink = storeInformationSettings.FacebookLink;
            model.StoreInformationSettings.TwitterLink = storeInformationSettings.TwitterLink;
            model.StoreInformationSettings.YoutubeLink = storeInformationSettings.YoutubeLink;
            model.StoreInformationSettings.GooglePlusLink = storeInformationSettings.GooglePlusLink;
            //contact us
            model.StoreInformationSettings.SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm;
            model.StoreInformationSettings.UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm;
            //sitemap
            model.StoreInformationSettings.SitemapEnabled = commonSettings.SitemapEnabled;
            model.StoreInformationSettings.SitemapIncludeCategories = commonSettings.SitemapIncludeCategories;
            model.StoreInformationSettings.SitemapIncludeManufacturers = commonSettings.SitemapIncludeManufacturers;
            model.StoreInformationSettings.SitemapIncludeProducts = commonSettings.SitemapIncludeProducts;

            //override settings
            if (storeScope > 0)
            {
                model.StoreInformationSettings.StoreClosed_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.StoreClosed, storeScope);
                model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DefaultStoreTheme, storeScope);
                model.StoreInformationSettings.AllowUserToSelectTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.AllowUserToSelectTheme, storeScope);
                model.StoreInformationSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.LogoPictureId, storeScope);
                model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope);
                model.StoreInformationSettings.FacebookLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.FacebookLink, storeScope);
                model.StoreInformationSettings.TwitterLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.TwitterLink, storeScope);
                model.StoreInformationSettings.YoutubeLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.YoutubeLink, storeScope);
                model.StoreInformationSettings.GooglePlusLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.GooglePlusLink, storeScope);
                model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope);
                model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope);
                model.StoreInformationSettings.SitemapEnabled_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapEnabled, storeScope);
                model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeCategories, storeScope);
                model.StoreInformationSettings.SitemapIncludeManufacturers_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeManufacturers, storeScope);
                model.StoreInformationSettings.SitemapIncludeProducts_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeProducts, storeScope);
            }

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment;
            model.SeoSettings.PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList();
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.WwwRequirement = (int)seoSettings.WwwRequirement;
            model.SeoSettings.WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList();
            model.SeoSettings.EnableJsBundling = seoSettings.EnableJsBundling;
            model.SeoSettings.EnableCssBundling = seoSettings.EnableCssBundling;
            model.SeoSettings.TwitterMetaTags = seoSettings.TwitterMetaTags;
            model.SeoSettings.OpenGraphMetaTags = seoSettings.OpenGraphMetaTags;
            model.SeoSettings.CustomHeadTags = seoSettings.CustomHeadTags;
            //override settings
            if (storeScope > 0)
            {
                model.SeoSettings.PageTitleSeparator_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeparator, storeScope);
                model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeoAdjustment, storeScope);
                model.SeoSettings.DefaultTitle_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultTitle, storeScope);
                model.SeoSettings.DefaultMetaKeywords_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaKeywords, storeScope);
                model.SeoSettings.DefaultMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaDescription, storeScope);
                model.SeoSettings.GenerateProductMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.GenerateProductMetaDescription, storeScope);
                model.SeoSettings.ConvertNonWesternChars_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.ConvertNonWesternChars, storeScope);
                model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CanonicalUrlsEnabled, storeScope);
                model.SeoSettings.WwwRequirement_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.WwwRequirement, storeScope);
                model.SeoSettings.EnableJsBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableJsBundling, storeScope);
                model.SeoSettings.EnableCssBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableCssBundling, storeScope);
                model.SeoSettings.TwitterMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.TwitterMetaTags, storeScope);
                model.SeoSettings.OpenGraphMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.OpenGraphMetaTags, storeScope);
                model.SeoSettings.CustomHeadTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CustomHeadTags, storeScope);
            }
            
            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            model.SecuritySettings.ForceSslForAllPages = securitySettings.ForceSslForAllPages;
            model.SecuritySettings.EnableXsrfProtectionForAdminArea = securitySettings.EnableXsrfProtectionForAdminArea;
            model.SecuritySettings.EnableXsrfProtectionForPublicStore = securitySettings.EnableXsrfProtectionForPublicStore;
            model.SecuritySettings.HoneypotEnabled = securitySettings.HoneypotEnabled;

            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            model.CaptchaSettings = captchaSettings.ToModel();
            model.CaptchaSettings.AvailableReCaptchaVersions = ReCaptchaVersion.Version1.ToSelectList(false).ToList();

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(storeScope);
            model.PdfSettings.LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled;
            model.PdfSettings.LogoPictureId = pdfSettings.LogoPictureId;
            model.PdfSettings.DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders;
            model.PdfSettings.InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1;
            model.PdfSettings.InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2;
            //override settings
            if (storeScope > 0)
            {
                model.PdfSettings.LetterPageSizeEnabled_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LetterPageSizeEnabled, storeScope);
                model.PdfSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.LogoPictureId, storeScope);
                model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeScope);
                model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn1, storeScope);
                model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore = _settingService.SettingExists(pdfSettings, x => x.InvoiceFooterTextColumn2, storeScope);
            }

            //localization
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(storeScope);
            model.LocalizationSettings.UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection;
            model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
            model.LocalizationSettings.AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage;
            model.LocalizationSettings.LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup;
            model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup;
            model.LocalizationSettings.LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup;

            //full-text support
            model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            model.FullTextSettings.Enabled = commonSettings.UseFullTextSearch;
            model.FullTextSettings.SearchMode = (int)commonSettings.FullTextMode;
            model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();
            
            //display default menu item
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem = displayDefaultMenuItemSettings.DisplayHomePageMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem = displayDefaultMenuItemSettings.DisplayNewProductsMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem = displayDefaultMenuItemSettings.DisplayProductSearchMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem = displayDefaultMenuItemSettings.DisplayUserInfoMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem;

            model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayHomePageMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayUserInfoMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeScope);


            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            //store information settings
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowUserToSelectTheme = model.StoreInformationSettings.AllowUserToSelectTheme;
            storeInformationSettings.LogoPictureId = model.StoreInformationSettings.LogoPictureId;
            //EU Cookie law
            storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
            storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
            storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
            storeInformationSettings.GooglePlusLink = model.StoreInformationSettings.GooglePlusLink;
            //contact us
            commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
            commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;
            //sitemap
            commonSettings.SitemapEnabled = model.StoreInformationSettings.SitemapEnabled;
            commonSettings.SitemapIncludeCategories = model.StoreInformationSettings.SitemapIncludeCategories;
            commonSettings.SitemapIncludeManufacturers = model.StoreInformationSettings.SitemapIncludeManufacturers;
            commonSettings.SitemapIncludeProducts = model.StoreInformationSettings.SitemapIncludeProducts;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.StoreClosed, model.StoreInformationSettings.StoreClosed_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DefaultStoreTheme, model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.AllowUserToSelectTheme, model.StoreInformationSettings.AllowUserToSelectTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.LogoPictureId, model.StoreInformationSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DisplayEuCookieLawWarning, model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.FacebookLink, model.StoreInformationSettings.FacebookLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.TwitterLink, model.StoreInformationSettings.TwitterLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.YoutubeLink, model.StoreInformationSettings.YoutubeLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.GooglePlusLink, model.StoreInformationSettings.GooglePlusLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SubjectFieldOnContactUsForm, model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.UseSystemEmailForContactUsForm, model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapEnabled, model.StoreInformationSettings.SitemapEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeCategories, model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeManufacturers, model.StoreInformationSettings.SitemapIncludeManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeProducts, model.StoreInformationSettings.SitemapIncludeProducts_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();



            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            seoSettings.GenerateProductMetaDescription = model.SeoSettings.GenerateProductMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
            seoSettings.EnableJsBundling = model.SeoSettings.EnableJsBundling;
            seoSettings.EnableCssBundling = model.SeoSettings.EnableCssBundling;
            seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
            seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
            seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeparator, model.SeoSettings.PageTitleSeparator_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeoAdjustment, model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultTitle, model.SeoSettings.DefaultTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaKeywords, model.SeoSettings.DefaultMetaKeywords_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaDescription, model.SeoSettings.DefaultMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.GenerateProductMetaDescription, model.SeoSettings.GenerateProductMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.ConvertNonWesternChars, model.SeoSettings.ConvertNonWesternChars_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CanonicalUrlsEnabled, model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.WwwRequirement, model.SeoSettings.WwwRequirement_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableJsBundling, model.SeoSettings.EnableJsBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableCssBundling, model.SeoSettings.EnableCssBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.TwitterMetaTags, model.SeoSettings.TwitterMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.OpenGraphMetaTags, model.SeoSettings.OpenGraphMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CustomHeadTags, model.SeoSettings.CustomHeadTags_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();



            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!String.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!String.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtectionForAdminArea = model.SecuritySettings.EnableXsrfProtectionForAdminArea;
            securitySettings.EnableXsrfProtectionForPublicStore = model.SecuritySettings.EnableXsrfProtectionForPublicStore;
            securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
            _settingService.SaveSetting(securitySettings);

            //captcha settings
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            captchaSettings = model.CaptchaSettings.ToEntity(captchaSettings);
            _settingService.SaveSetting(captchaSettings);
            if (captchaSettings.Enabled &&
                (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                ErrorNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError"));
            }

            //PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>(storeScope);
            pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
            pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
            pdfSettings.DisablePdfInvoicesForPendingOrders = model.PdfSettings.DisablePdfInvoicesForPendingOrders;
            pdfSettings.InvoiceFooterTextColumn1 = model.PdfSettings.InvoiceFooterTextColumn1;
            pdfSettings.InvoiceFooterTextColumn2 = model.PdfSettings.InvoiceFooterTextColumn2;
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.LetterPageSizeEnabled, model.PdfSettings.LetterPageSizeEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.LogoPictureId, model.PdfSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.InvoiceFooterTextColumn1, model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(pdfSettings, x => x.InvoiceFooterTextColumn2, model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore, storeScope, false);
           
            //now clear settings cache
            _settingService.ClearCache();




            //localization settings
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>(storeScope);
            localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                //clear cached values of routes
                System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();
            }
            localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
            localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
            localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
            _settingService.SaveSetting(localizationSettings);

            //full-text
            commonSettings.FullTextMode = (FulltextSearchMode)model.FullTextSettings.SearchMode;
            _settingService.SaveSetting(commonSettings);

            //display default menu item
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeScope);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            displayDefaultMenuItemSettings.DisplayHomePageMenuItem = model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem;
            displayDefaultMenuItemSettings.DisplayNewProductsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem;
            displayDefaultMenuItemSettings.DisplayProductSearchMenuItem = model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem;
            displayDefaultMenuItemSettings.DisplayUserInfoMenuItem = model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem;
            displayDefaultMenuItemSettings.DisplayBlogMenuItem = model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem;
            displayDefaultMenuItemSettings.DisplayForumsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem;
            displayDefaultMenuItemSettings.DisplayContactUsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem;

            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayHomePageMenuItem, model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayUserInfoMenuItem, model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.Updated"));
            
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public virtual ActionResult ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new SeagullException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new SeagullException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                //update password information
                //optimization - load only passwords with PasswordFormat.Encrypted
                var userPasswords = _userService.GetUserPasswords(passwordFormat: PasswordFormat.Encrypted);
                foreach (var userPassword in userPasswords)
                {
                    var decryptedPassword = _encryptionService.DecryptText(userPassword.Password, oldEncryptionPrivateKey);
                    var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    userPassword.Password = encryptedPassword;
                    _userService.UpdateUserPassword(userPassword);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            return RedirectToAction("GeneralCommon");
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public virtual ActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            try
            {
                if (! _fulltextService.IsFullTextSupported())
                    throw new SeagullException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported"));

                if (commonSettings.UseFullTextSearch)
                {
                    _fulltextService.DisableFullText();

                    commonSettings.UseFullTextSearch = false;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                }
                else
                {
                    _fulltextService.EnableFullText();

                    commonSettings.UseFullTextSearch = true;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            
            return RedirectToAction("GeneralCommon");
        }




        //all settings
        public virtual ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return View();
        }
        [HttpPost]
        //do not validate request token (XSRF)
        //for some reasons it does not work with "filtering" support
        [AdminAntiForgery(true)] 
        public virtual ActionResult AllSettings(DataSourceRequest command, AllSettingsListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var query = _settingService.GetAllSettings().AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchSettingName))
                query = query.Where(s => s.Name.ToLowerInvariant().Contains(model.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(model.SearchSettingValue))
                query = query.Where(s => s.Value.ToLowerInvariant().Contains(model.SearchSettingValue.ToLowerInvariant()));

            var settings = query.ToList()
                .Select(x =>
                            {
                                string storeName;
                                if (x.StoreId == 0)
                                {
                                    storeName = _localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
                                }
                                else
                                {
                                    var store = _storeService.GetStoreById(x.StoreId);
                                    storeName = store != null ? store.Name : "Unknown";
                                }
                                var settingModel = new SettingModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Value = x.Value,
                                    Store = storeName,
                                    StoreId = x.StoreId
                                };
                                return settingModel;
                            })
                .AsQueryable();

            var gridModel = new DataSourceResult
            {
                Data = settings.PagedForCommand(command).ToList(),
                Total = settings.Count()
            };

            return Json(gridModel);
        }
        [HttpPost]
        public virtual ActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");

            var storeId = model.StoreId;

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) ||
                setting.StoreId != storeId)
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            var storeId = model.StoreId;
            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _userActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _userActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return new NullJsonResult();
        }

        //action displaying notification (warning) to a store owner about a lot of traffic 
        //between the Redis server and the application when LoadAllLocaleRecordsOnStartup seetting is set
        [ValidateInput(false)]
        public ActionResult RedisCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
        {
            //LoadAllLocaleRecordsOnStartup is set and Redis cache is used, so display warning
            if (_config.RedisCachingEnabled && loadAllLocaleRecordsOnStartup)
                return Json(new { Result = _localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning") }, JsonRequestBehavior.AllowGet);

            return Json(new { Result = string.Empty }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
