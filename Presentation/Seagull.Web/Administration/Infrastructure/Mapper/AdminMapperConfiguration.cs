using System;
using AutoMapper;
using Seagull.Admin.Models.Cms;
using Seagull.Admin.Models.Common;
using Seagull.Admin.Models.Users;
using Seagull.Admin.Models.Directory;
using Seagull.Admin.Models.ExternalAuthentication;
using Seagull.Admin.Models.Localization;
using Seagull.Admin.Models.Logging;
using Seagull.Admin.Models.Messages;
using Seagull.Admin.Models.Plugins;
using Seagull.Admin.Models.Settings;
using Seagull.Admin.Models.Stores;
using Seagull.Admin.Models.Topics;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Directory;
using Seagull.Core.Domain.Localization;
using Seagull.Core.Domain.Logging;
using Seagull.Core.Domain.Media;
using Seagull.Core.Domain.Messages;
using Seagull.Core.Domain.Stores;
using Seagull.Core.Domain.Topics;
using Seagull.Core.Infrastructure.Mapper;
using Seagull.Core.Plugins;
using Seagull.Services.Authentication.External;
using Seagull.Services.Cms;
using Seagull.Services.Seo;
using Seagull.Web.Framework.Security.Captcha;

namespace Seagull.Admin.Infrastructure.Mapper
{
    /// <summary>
    /// AutoMapper configuration for admin area models
    /// </summary>
    public class AdminMapperConfiguration : IMapperConfiguration
    {
        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Mapper configuration action</returns>
        public Action<IMapperConfigurationExpression> GetConfiguration()
        {
            //TODO remove 'CreatedOnUtc' ignore mappings because now presentation layer models have 'CreatedOn' property and core entities have 'CreatedOnUtc' property (distinct names)

            Action<IMapperConfigurationExpression> action = cfg =>
            {
                //address
                cfg.CreateMap<Address, AddressModel>()
                    .ForMember(dest => dest.AddressHtml, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomAddressAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.FormattedCustomAddressAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCountries, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStates, mo => mo.Ignore())
                    .ForMember(dest => dest.FirstNameEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.FirstNameRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.LastNameEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.LastNameRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StateProvinceEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CityEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.CityRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddressEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddressRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddress2Enabled, mo => mo.Ignore())
                    .ForMember(dest => dest.StreetAddress2Required, mo => mo.Ignore())
                    .ForMember(dest => dest.ZipPostalCodeEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.ZipPostalCodeRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.PhoneEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.PhoneRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.FaxEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.FaxRequired, mo => mo.Ignore())
                    .ForMember(dest => dest.CountryName,
                        mo => mo.MapFrom(src => src.Country != null ? src.Country.Name : null))
                    .ForMember(dest => dest.StateProvinceName,
                        mo => mo.MapFrom(src => src.StateProvince != null ? src.StateProvince.Name : null))
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<AddressModel, Address>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.Country, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomAttributes, mo => mo.Ignore())
                    .ForMember(dest => dest.StateProvince, mo => mo.Ignore());

                //countries
                cfg.CreateMap<CountryModel, Country>()
                    .ForMember(dest => dest.StateProvinces, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                cfg.CreateMap<Country, CountryModel>()
                    .ForMember(dest => dest.NumberOfStates,
                        mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0))
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //state/provinces
                cfg.CreateMap<StateProvince, StateProvinceModel>()
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<StateProvinceModel, StateProvince>()
                    .ForMember(dest => dest.Country, mo => mo.Ignore());

                //language
                cfg.CreateMap<Language, LanguageModel>()
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableCurrencies, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.Search, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<LanguageModel, Language>()
                    .ForMember(dest => dest.LocaleStringResources, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //email account
                cfg.CreateMap<EmailAccount, EmailAccountModel>()
                    .ForMember(dest => dest.Password, mo => mo.Ignore())
                    .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                    .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<EmailAccountModel, EmailAccount>()
                    .ForMember(dest => dest.Password, mo => mo.Ignore());
                //message template
                cfg.CreateMap<MessageTemplate, MessageTemplateModel>()
                    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                    .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.SendImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<MessageTemplateModel, MessageTemplate>()
                    .ForMember(dest => dest.DelayPeriod, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());
                //queued email
                cfg.CreateMap<QueuedEmail, QueuedEmailModel>()
                    .ForMember(dest => dest.EmailAccountName,
                        mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.PriorityName, mo => mo.Ignore())
                    .ForMember(dest => dest.DontSendBeforeDate, mo => mo.Ignore())
                    .ForMember(dest => dest.SendImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<QueuedEmailModel, QueuedEmail>()
                    .ForMember(dest => dest.Priority, dt => dt.Ignore())
                    .ForMember(dest => dest.PriorityId, dt => dt.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, dt => dt.Ignore())
                    .ForMember(dest => dest.DontSendBeforeDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.SentOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachmentFilePath, mo => mo.Ignore())
                    .ForMember(dest => dest.AttachmentFileName, mo => mo.Ignore());
                //campaign
                cfg.CreateMap<Campaign, CampaignModel>()
                    .ForMember(dest => dest.DontSendBeforeDate, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore())
                    .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore())
                    .ForMember(dest => dest.TestEmail, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CampaignModel, Campaign>()
                    .ForMember(dest => dest.DontSendBeforeDateUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore());
                //topcis
                cfg.CreateMap<Topic, TopicModel>()
                    .ForMember(dest => dest.AvailableTopicTemplates, mo => mo.Ignore())
                    .ForMember(dest => dest.Url, mo => mo.Ignore())
                    .ForMember(dest => dest.SeName, mo => mo.MapFrom(src => src.GetSeName(0, true, false)))
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<TopicModel, Topic>()
                    .ForMember(dest => dest.SubjectToAcl, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore());

                ;

               

           
                //logs
                cfg.CreateMap<Log, LogModel>()
                    .ForMember(dest => dest.UserEmail, mo => mo.Ignore())
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<LogModel, Log>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                    .ForMember(dest => dest.User, mo => mo.Ignore());
                //ActivityLogType
                cfg.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                    .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore());
                cfg.CreateMap<ActivityLogType, ActivityLogTypeModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<ActivityLog, ActivityLogModel>()
                    .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                    .ForMember(dest => dest.UserEmail, mo => mo.MapFrom(src => src.User.Email))
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //currencies
                cfg.CreateMap<Currency, CurrencyModel>()
                    .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                    .ForMember(dest => dest.IsPrimaryExchangeRateCurrency, mo => mo.Ignore())
                    .ForMember(dest => dest.IsPrimaryStoreCurrency, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<CurrencyModel, Currency>()
                    .ForMember(dest => dest.CreatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.UpdatedOnUtc, mo => mo.Ignore())
                    .ForMember(dest => dest.LimitedToStores, mo => mo.Ignore())
                    .ForMember(dest => dest.RoundingType, mo => mo.Ignore());
                
        
                //external authentication methods
                cfg.CreateMap<IExternalAuthenticationMethod, AuthenticationMethodModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //widgets
                cfg.CreateMap<IWidgetPlugin, WidgetModel>()
                    .ForMember(dest => dest.FriendlyName, mo => mo.MapFrom(src => src.PluginDescriptor.FriendlyName))
                    .ForMember(dest => dest.SystemName, mo => mo.MapFrom(src => src.PluginDescriptor.SystemName))
                    .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.PluginDescriptor.DisplayOrder))
                    .ForMember(dest => dest.IsActive, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationActionName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationControllerName, mo => mo.Ignore())
                    .ForMember(dest => dest.ConfigurationRouteValues, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                //plugins
                cfg.CreateMap<PluginDescriptor, PluginModel>()
                    .ForMember(dest => dest.ConfigurationUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.CanChangeEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.IsEnabled, mo => mo.Ignore())
                    .ForMember(dest => dest.LogoUrl, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableUserRoles, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedUserRoleIds, mo => mo.Ignore())
                    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

                //user roles
                cfg.CreateMap<UserRole, UserRoleModel>()
                    .ForMember(dest => dest.PurchasedWithProductName, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<UserRoleModel, UserRole>()
                    .ForMember(dest => dest.PermissionRecords, mo => mo.Ignore());

               
                //user attributes
                cfg.CreateMap<UserAttribute, UserAttributeModel>()
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<UserAttributeModel, UserAttribute>()
                    .ForMember(dest => dest.UserAttributeValues, mo => mo.Ignore());
                //address attributes
                cfg.CreateMap<AddressAttribute, AddressAttributeModel>()
                    .ForMember(dest => dest.AttributeControlTypeName, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<AddressAttributeModel, AddressAttribute>()
                    .ForMember(dest => dest.AddressAttributeValues, mo => mo.Ignore());
                
                //stores
                cfg.CreateMap<Store, StoreModel>()
                    .ForMember(dest => dest.AvailableLanguages, mo => mo.Ignore())
                    .ForMember(dest => dest.Locales, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<StoreModel, Store>();

                //Settings
                cfg.CreateMap<CaptchaSettings, GeneralCommonSettingsModel.CaptchaSettingsModel>()
                    .ForMember(dest => dest.AvailableReCaptchaVersions, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<GeneralCommonSettingsModel.CaptchaSettingsModel, CaptchaSettings>()
                    .ForMember(dest => dest.ReCaptchaTheme, mo => mo.Ignore())
                    .ForMember(dest => dest.ReCaptchaLanguage, mo => mo.Ignore());

                

                cfg.CreateMap<RewardPointsSettings, RewardPointsSettingsModel>()
                    .ForMember(dest => dest.PrimaryStoreCurrencyCode, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.Enabled_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ExchangeRate_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MinimumRewardPointsToUse_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PointsForRegistration_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PointsForPurchases_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivationDelay_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ActivatePointsImmediately, mo => mo.Ignore())
                    .ForMember(dest => dest.DisplayHowMuchWillBeEarned_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.PageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<RewardPointsSettingsModel, RewardPointsSettings>();
               
               

               
                cfg.CreateMap<MediaSettings, MediaSettingsModel>()
                    .ForMember(dest => dest.PicturesStoredIntoDatabase, mo => mo.Ignore())
                    .ForMember(dest => dest.ActiveStoreScopeConfiguration, mo => mo.Ignore())
                    .ForMember(dest => dest.AvatarPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductDetailsPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore,
                        mo => mo.Ignore())
                    .ForMember(dest => dest.AssociatedProductPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CategoryThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.ManufacturerThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.VendorThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CartThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MiniCartThumbPictureSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MaximumImageSize_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.MultipleThumbDirectories_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultImageQuality_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore())
                    .ForMember(dest => dest.ImportProductImagesUsingHash_OverrideForStore, mo => mo.Ignore())
                    .ForMember(dest => dest.DefaultPictureZoomEnabled_OverrideForStore, mo => mo.Ignore());
                cfg.CreateMap<MediaSettingsModel, MediaSettings>()
                    .ForMember(dest => dest.ImageSquarePictureSize, mo => mo.Ignore())
                    .ForMember(dest => dest.AutoCompleteSearchThumbPictureSize, mo => mo.Ignore());
                cfg.CreateMap<UserSettings, UserUserSettingsModel.UserSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<UserUserSettingsModel.UserSettingsModel, UserSettings>()
                    .ForMember(dest => dest.HashedPasswordFormat, mo => mo.Ignore())
                    .ForMember(dest => dest.AvatarMaximumSizeBytes, mo => mo.Ignore())
                    .ForMember(dest => dest.OnlineUserMinutes, mo => mo.Ignore())
                    .ForMember(dest => dest.SuffixDeletedUsers, mo => mo.Ignore())
                    .ForMember(dest => dest.DeleteGuestTaskOlderThanMinutes, mo => mo.Ignore());
                cfg.CreateMap<AddressSettings, UserUserSettingsModel.AddressSettingsModel>()
                    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
                cfg.CreateMap<UserUserSettingsModel.AddressSettingsModel, AddressSettings>();



            };
            return action;
        }

        /// <summary>
        /// Order of this mapper implementation
        /// </summary>
        public int Order
        {
            get { return 0; }
        }
    }
}