using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Caching;
using Seagull.Core.Domain;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Localization;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Localization;
using Seagull.Services.Media;
using Seagull.Services.Security;
using Seagull.Services.Seo;
using Seagull.Services.Topics;
using Seagull.Web.Framework.Security.Captcha;
using Seagull.Web.Framework.Themes;
using Seagull.Web.Framework.UI;
using Seagull.Web.Infrastructure.Cache;
using Seagull.Web.Models.Common;
using Seagull.Web.Models.Topics;

namespace Seagull.Web.Factories
{
    /// <summary>
    /// Represents the common models factory
    /// </summary>
    public partial class CommonModelFactory : ICommonModelFactory
    {
        #region Fields
        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ICacheManager _cacheManager;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPictureService _pictureService;
        private readonly HttpContextBase _httpContext;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CommonSettings _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Constructors

        public CommonModelFactory(
            ITopicService topicService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ISitemapGenerator sitemapGenerator,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            ICacheManager cacheManager,
            IPageHeadBuilder pageHeadBuilder,
            IPictureService pictureService,
            HttpContextBase httpContext,
            StoreInformationSettings storeInformationSettings,
            CommonSettings commonSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings
            )
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._cacheManager = cacheManager;
            this._pageHeadBuilder = pageHeadBuilder;
            this._pictureService = pictureService;
            this._httpContext = httpContext;
            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        public virtual LogoModel PrepareLogoModel()
        {
            var model = new LogoModel
            {
                StoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name)
            };

            var cacheKey = string.Format(ModelCacheEventConsumer.STORE_LOGO_PATH, _storeContext.CurrentStore.Id, _themeContext.WorkingThemeName, _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = _cacheManager.Get(cacheKey, () =>
            {
                var logo = "";
                var logoPictureId = _storeInformationSettings.LogoPictureId;
                if (logoPictureId > 0)
                {
                    logo = _pictureService.GetPictureUrl(logoPictureId, showDefaultPicture: false);
                }
                if (String.IsNullOrEmpty(logo))
                {
                    //use default logo
                    logo = string.Format("{0}Themes/{1}/Content/images/logo.png", _webHelper.GetStoreLocation(), _themeContext.WorkingThemeName);
                }
                return logo;
            });

            return model;
        }

        /// <summary>
        /// Prepare the language selector model
        /// </summary>
        /// <returns>Language selector model</returns>
        public virtual LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.CurrentStore.Id), () =>
            {
                var result = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(x => new LanguageModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return result;
            });

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };
            
            return model;
        }

        /// <summary>
        /// Prepare the currency selector model
        /// </summary>
        /// <returns>Currency selector model</returns>
        public virtual CurrencySelectorModel PrepareCurrencySelectorModel()
        {
            var availableCurrencies = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_CURRENCIES_MODEL_KEY, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id), () =>
            {
                var result = _currencyService
                    .GetAllCurrencies(storeId: _storeContext.CurrentStore.Id)
                    .Select(x =>
                    {
                        //currency char
                        var currencySymbol = "";
                        if (!string.IsNullOrEmpty(x.DisplayLocale))
                            currencySymbol = new RegionInfo(x.DisplayLocale).CurrencySymbol;
                        else
                            currencySymbol = x.CurrencyCode;
                        //model
                        var currencyModel = new CurrencyModel
                        {
                            Id = x.Id,
                            Name = x.GetLocalized(y => y.Name),
                            CurrencySymbol = currencySymbol
                        };
                        return currencyModel;
                    })
                    .ToList();
                return result;
            });

            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = availableCurrencies
            };

            return model;
        }

        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        public virtual HeaderLinksModel PrepareHeaderLinksModel()
        {
            var user = _workContext.CurrentUser;

            var unreadMessageCount = 0;
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;
            if (unreadMessageCount > 0)
            {
                unreadMessage = string.Format(_localizationService.GetResource("PrivateMessages.TotalUnread"), unreadMessageCount);

                //notifications here
                if (!user.GetAttribute<bool>(SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, _storeContext.CurrentStore.Id))
                {
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.NotifiedAboutNewPrivateMessages, true, _storeContext.CurrentStore.Id);
                    alertMessage = string.Format(_localizationService.GetResource("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
                }
            }

            var model = new HeaderLinksModel
            {
                IsAuthenticated = user.IsRegistered(),
                UserName = user.IsRegistered() ? user.FormatUserName() : "",
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };

            return model;
        }

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        public virtual AdminHeaderLinksModel PrepareAdminHeaderLinksModel()
        {
            var user = _workContext.CurrentUser;

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedUserName = user.IsRegistered() ? user.FormatUserName() : "",
                IsUserImpersonated = _workContext.OriginalUserIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = _pageHeadBuilder.GetEditPageUrl()
            };

            return model;
        }

        /// <summary>
        /// Prepare the social model
        /// </summary>
        /// <returns>Social model</returns>
        public virtual SocialModel PrepareSocialModel()
        {
            var model = new SocialModel
            {
                FacebookLink = _storeInformationSettings.FacebookLink,
                TwitterLink = _storeInformationSettings.TwitterLink,
                YoutubeLink = _storeInformationSettings.YoutubeLink,
                GooglePlusLink = _storeInformationSettings.GooglePlusLink,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
            };

            return model;
        }

        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        public virtual FooterModel PrepareFooterModel()
        {
            //footer topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_FOOTER_MODEL_KEY,
                _workContext.WorkingLanguage.Id,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentUser.GetUserRoleIds()));
            var cachedTopicModel = _cacheManager.Get(topicCacheKey, () =>
                _topicService.GetAllTopics(_storeContext.CurrentStore.Id)
                .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
                .Select(t => new FooterModel.FooterTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName(),
                    IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                    IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                    IncludeInFooterColumn3 = t.IncludeInFooterColumn3
                })
                .ToList()
            );

            //model
            var model = new FooterModel
            {
                StoreName = _storeContext.CurrentStore.GetLocalized(x => x.Name),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                SitemapEnabled = _commonSettings.SitemapEnabled,
                WorkingLanguageId = _workContext.WorkingLanguage.Id,
                HidePoweredBySeagullCommerce = _storeInformationSettings.HidePoweredBySeagullCommerce,
                Topics = cachedTopicModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the contact us model
        /// </summary>
        /// <param name="model">Contact us model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contact us model</returns>
        public virtual ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties)
            {
                model.Email = _workContext.CurrentUser.Email;
                model.FullName = _workContext.CurrentUser.GetFullName();
            }
            model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
            
            return model;
        }

        
        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="url">URL helper</param>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        public virtual string PrepareSitemapXml(UrlHelper url, int? id)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.SITEMAP_SEO_MODEL_KEY, id,
                _workContext.WorkingLanguage.Id,
                string.Join(",", _workContext.CurrentUser.GetUserRoleIds()),
                _storeContext.CurrentStore.Id);
            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(url, id));
            return siteMap;
        }

        /// <summary>
        /// Prepare the store theme selector model
        /// </summary>
        /// <returns>Store theme selector model</returns>
        public virtual StoreThemeSelectorModel PrepareStoreThemeSelectorModel()
        {
            var model = new StoreThemeSelectorModel();
            var currentTheme = _themeProvider.GetThemeConfiguration(_themeContext.WorkingThemeName);
            model.CurrentStoreTheme = new StoreThemeModel
            {
                Name = currentTheme.ThemeName,
                Title = currentTheme.ThemeTitle
            };
            model.AvailableStoreThemes = _themeProvider.GetThemeConfigurations()
                .Select(x => new StoreThemeModel
                {
                    Name = x.ThemeName,
                    Title = x.ThemeTitle
                })
                .ToList();
            return model;
        }

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        public virtual FaviconModel PrepareFaviconModel()
        {
            var model = new FaviconModel();

            //try loading a store specific favicon
            var faviconFileName = string.Format("favicon-{0}.ico", _storeContext.CurrentStore.Id);
            var localFaviconPath = System.IO.Path.Combine(_httpContext.Request.PhysicalApplicationPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = System.IO.Path.Combine(_httpContext.Request.PhysicalApplicationPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return model;
                }
            }

            model.FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName;
            return model;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            string robotsFilePath = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.custom.txt");
            if (System.IO.File.Exists(robotsFilePath))
            {
                //the robots.txt file exists
                string robotsFileContent = System.IO.File.ReadAllText(robotsFilePath);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/content/files/",
                    "/content/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness",
                };
                var localizableDisallowPaths = new List<string>
                {
                    "/addproducttocart/catalog/",
                    "/addproducttocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/compareproducts",
                    "/compareproducts/add/*",
                    "/user/avatar",
                    "/user/activation",
                    "/user/addresses",
                    "/user/changepassword",
                    "/user/checkusernameavailability",
                    "/user/downloadableproducts",
                    "/user/info",
                    "/deletepm",
                    "/emailwishlist",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfileproductattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist",
                };


                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                    {
                        sb.AppendFormat("Sitemap: {0}{1}/sitemap.xml", _storeContext.CurrentStore.Url, language.UniqueSeoCode);
                        sb.Append(newLine);
                    }
                }
                else
                {
                    //localizable paths (without SEO code)
                    sb.AppendFormat("Sitemap: {0}sitemap.xml", _storeContext.CurrentStore.Url);
                    sb.Append(newLine);
                }

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //URLs are localizable. Append SEO code
                    foreach (var language in _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id))
                    {
                        foreach (var path in localizableDisallowPaths)
                        {
                            sb.AppendFormat("Disallow: /{0}{1}", language.UniqueSeoCode, path);
                            sb.Append(newLine);
                        }
                    }
                }

                //load and add robots.txt additions to the end of file.
                string robotsAdditionsFile = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.additions.txt");
                if (System.IO.File.Exists(robotsAdditionsFile))
                {
                    string robotsFileContent = System.IO.File.ReadAllText(robotsAdditionsFile);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }
        
        #endregion
    }
}
