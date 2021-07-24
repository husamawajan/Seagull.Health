using System;
using System.Linq;
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
using Seagull.Admin.Models.charts;
using Seagull.Admin.Models.Components;
using Seagull.Admin.Models.UserEntitys;
using Seagull.Admin.Models.UserTypes;
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
using Seagull.Core.Domain.Chart;
using Seagull.Core.Domain.Components;
using Seagull.Core.Domain.UserEntitys;
using Seagull.Core.Domain.UserTypes;
using Seagull.Services.Authentication.External;
using Seagull.Services.Cms;
using Seagull.Services.Common;
using Seagull.Web.Framework.Security.Captcha;
using System.Collections.Generic;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Admin.Models.Functionss;
using Seagull.Core.Domain.Functionss;
using Seagull.Admin.Models.Emails;
using Seagull.Core.Domain.Emails;

namespace Seagull.Admin.Extensions
{
    public static class MappingExtensions
    {

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #region User attributes

        //attributes
        public static UserAttributeModel ToModel(this UserAttribute entity)
        {
            return entity.MapTo<UserAttribute, UserAttributeModel>();
        }

        public static UserAttribute ToEntity(this UserAttributeModel model)
        {
            return model.MapTo<UserAttributeModel, UserAttribute>();
        }

        public static UserAttribute ToEntity(this UserAttributeModel model, UserAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Address attributes

        //attributes
        public static AddressAttributeModel ToModel(this AddressAttribute entity)
        {
            return entity.MapTo<AddressAttribute, AddressAttributeModel>();
        }

        public static AddressAttribute ToEntity(this AddressAttributeModel model)
        {
            return model.MapTo<AddressAttributeModel, AddressAttribute>();
        }

        public static AddressAttribute ToEntity(this AddressAttributeModel model, AddressAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language entity)
        {
            var x = entity.MapTo<Language, LanguageModel>();
            x.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = x.Id }));
            return x;
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return model.MapTo<LanguageModel, Language>();
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return entity.MapTo<EmailAccount, EmailAccountModel>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return model.MapTo<EmailAccountModel, EmailAccount>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            var t = entity.MapTo<MessageTemplate, MessageTemplateModel>();
            t.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = t.Id }));
            return t;
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return model.MapTo<MessageTemplateModel, MessageTemplate>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return entity.MapTo<QueuedEmail, QueuedEmailModel>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return model.MapTo<QueuedEmailModel, QueuedEmail>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Campaigns

        public static CampaignModel ToModel(this Campaign entity)
        {
            return entity.MapTo<Campaign, CampaignModel>();
        }

        public static Campaign ToEntity(this CampaignModel model)
        {
            return model.MapTo<CampaignModel, Campaign>();
        }

        public static Campaign ToEntity(this CampaignModel model, Campaign destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Topics

        public static TopicModel ToModel(this Topic entity)
        {
            var t = entity.MapTo<Topic, TopicModel>();
            t.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = t.Id }));
            return t;
        }

        public static Topic ToEntity(this TopicModel model)
        {
            return model.MapTo<TopicModel, Topic>();
        }

        public static Topic ToEntity(this TopicModel model, Topic destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return entity.MapTo<Log, LogModel>();
        }

        public static Log ToEntity(this LogModel model)
        {
            return model.MapTo<LogModel, Log>();
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return model.MapTo(destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return entity.MapTo<ActivityLogType, ActivityLogTypeModel>();
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return entity.MapTo<ActivityLog, ActivityLogModel>();
        }

        #endregion

        #region Currencies

        public static CurrencyModel ToModel(this Currency entity)
        {
            return entity.MapTo<Currency, CurrencyModel>();
        }

        public static Currency ToEntity(this CurrencyModel model)
        {
            return model.MapTo<CurrencyModel, Currency>();
        }

        public static Currency ToEntity(this CurrencyModel model, Currency destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region External authentication methods

        public static AuthenticationMethodModel ToModel(this IExternalAuthenticationMethod entity)
        {
            return entity.MapTo<IExternalAuthenticationMethod, AuthenticationMethodModel>();
        }

        #endregion

        #region Widgets

        public static WidgetModel ToModel(this IWidgetPlugin entity)
        {
            return entity.MapTo<IWidgetPlugin, WidgetModel>();
        }

        #endregion

        #region Address

        public static AddressModel ToModel(this Address entity)
        {
            return entity.MapTo<Address, AddressModel>();
        }

        public static Address ToEntity(this AddressModel model)
        {
            return model.MapTo<AddressModel, Address>();
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return model.MapTo(destination);
        }

        public static void PrepareCustomAddressAttributes(this AddressModel model,
            Address address,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser)
        {
            //this method is very similar to the same one in Seagull.Web project
            if (addressAttributeService == null)
                throw new ArgumentNullException("addressAttributeService");

            if (addressAttributeParser == null)
                throw new ArgumentNullException("addressAttributeParser");

            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddressModel.AddressAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = addressAttributeService.GetAddressAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new AddressModel.AddressAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                var selectedAddressAttributes = address != null ? address.CustomAttributes : null;

                model.CustomAddressAttributes.Add(attributeModel);
            }
        }

        #endregion

        #region
        public static User ToModel(this User entity)
        {
            User model = new User();
            model.Id = entity.Id;
            model.Active = entity.Active;
            model.Username = entity.Username;
            model.Email = entity.Email;
            return model;
        }

        #endregion

        #region User roles

        //user roles
        public static UserRoleModel ToModel(this UserRole entity)
        {
            UserRoleModel model = new UserRoleModel();
            model.Id = entity.Id;
            model.Active = entity.Active;
            model.EnablePasswordLifetime = entity.EnablePasswordLifetime;
            model.IsSystemRole = entity.IsSystemRole;
            model.Name = entity.Name;
            model.SystemName = entity.SystemName;
            model.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = entity.Id }));
            return model;
        }

        public static UserRole ToEntity(this UserRoleModel model)
        {
            return model.MapTo<UserRoleModel, UserRole>();
        }

        public static UserRole ToEntity(this UserRoleModel model, UserRole destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Countries / states

        public static CountryModel ToModel(this Country entity)
        {
            return entity.MapTo<Country, CountryModel>();
        }

        public static Country ToEntity(this CountryModel model)
        {
            return model.MapTo<CountryModel, Country>();
        }

        public static Country ToEntity(this CountryModel model, Country destination)
        {
            return model.MapTo(destination);
        }

        public static StateProvinceModel ToModel(this StateProvince entity)
        {
            return entity.MapTo<StateProvince, StateProvinceModel>();
        }

        public static StateProvince ToEntity(this StateProvinceModel model)
        {
            return model.MapTo<StateProvinceModel, StateProvince>();
        }

        public static StateProvince ToEntity(this StateProvinceModel model, StateProvince destination)
        {
            return model.MapTo(destination);
        }


        #endregion

        #region Settings

        public static RewardPointsSettingsModel ToModel(this RewardPointsSettings entity)
        {
            return entity.MapTo<RewardPointsSettings, RewardPointsSettingsModel>();
        }
        public static RewardPointsSettings ToEntity(this RewardPointsSettingsModel model, RewardPointsSettings destination)
        {
            return model.MapTo(destination);
        }

        public static MediaSettingsModel ToModel(this MediaSettings entity)
        {
            return entity.MapTo<MediaSettings, MediaSettingsModel>();
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model, MediaSettings destination)
        {
            return model.MapTo(destination);
        }

        //user/user settings
        public static UserUserSettingsModel.UserSettingsModel ToModel(this UserSettings entity)
        {
            return entity.MapTo<UserSettings, UserUserSettingsModel.UserSettingsModel>();
        }
        public static UserSettings ToEntity(this UserUserSettingsModel.UserSettingsModel model, UserSettings destination)
        {
            return model.MapTo(destination);
        }
        public static UserUserSettingsModel.AddressSettingsModel ToModel(this AddressSettings entity)
        {
            return entity.MapTo<AddressSettings, UserUserSettingsModel.AddressSettingsModel>();
        }
        public static AddressSettings ToEntity(this UserUserSettingsModel.AddressSettingsModel model, AddressSettings destination)
        {
            return model.MapTo(destination);
        }



        //general (captcha) settings
        public static GeneralCommonSettingsModel.CaptchaSettingsModel ToModel(this CaptchaSettings entity)
        {
            return entity.MapTo<CaptchaSettings, GeneralCommonSettingsModel.CaptchaSettingsModel>();
        }
        public static CaptchaSettings ToEntity(this GeneralCommonSettingsModel.CaptchaSettingsModel model, CaptchaSettings destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return entity.MapTo<PluginDescriptor, PluginModel>();
        }

        #endregion

        #region Stores

        public static StoreModel ToModel(this Store entity)
        {
            return entity.MapTo<Store, StoreModel>();
        }

        public static Store ToEntity(this StoreModel model)
        {
            return model.MapTo<StoreModel, Store>();
        }

        public static Store ToEntity(this StoreModel model, Store destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region charts
        public static UserChartModel ToModel(this UserChart entity)
        {
            return entity.MapTo<UserChart, UserChartModel>();
        }

        public static UserChart ToEntity(this UserChartModel model)
        {
            return model.MapTo<UserChartModel, UserChart>();
        }

        public static UserChart ToEntity(this UserChartModel model, UserChart destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Component

        public static ComponentModel ToModel(this Component entity)
        {
            ComponentModel model = new ComponentModel();
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.CreatedBy = entity.CreatedBy;
            model.CreatedDate = entity.CreatedDate;
            model.UpdatedBy = entity.UpdatedBy;
            model.UpdatedDate = entity.UpdatedDate;
            model.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = entity.Id }));
            return model;
        }

        public static Component ToEntity(this ComponentModel model)
        {
            Component destination = new Component();
            destination.Id = model.Id;
            destination.Name = model.Name;
            destination.CreatedBy = model.CreatedBy;
            destination.CreatedDate = model.CreatedDate;
            destination.UpdatedBy = model.UpdatedBy;
            destination.UpdatedDate = model.UpdatedDate;
            return destination;
        }

        public static Component ToEntity(this ComponentModel model, Component destination)
        {
            destination.Id = model.Id;
            destination.Name = model.Name;
            destination.CreatedBy = model.CreatedBy;
            destination.CreatedDate = model.CreatedDate;
            destination.UpdatedBy = model.UpdatedBy;
            destination.UpdatedDate = model.UpdatedDate;
            return destination;
        }

        #endregion

        #region UserType

        public static UserTypeModel ToModel(this UserType entity)
        {
            UserTypeModel model = new UserTypeModel();
            model.Id = entity.Id;
            model.Type = entity.Type;
            model.CreatedDate = entity.CreatedDate;
            model.UpdatedDate = entity.UpdatedDate;
            model.CreatedBy = entity.CreatedBy;
            model.UpdatedBy = entity.UpdatedBy;
            model.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = entity.Id }));
            return model;
        }

        public static UserType ToEntity(this UserTypeModel model)
        {
            UserType destination = new UserType();
            destination.Id = model.Id;
            destination.Type = model.Type;
            destination.CreatedDate = model.CreatedDate;
            destination.UpdatedDate = model.UpdatedDate;
            destination.CreatedBy = model.CreatedBy;
            destination.UpdatedBy = model.UpdatedBy;
            return destination;
        }

        public static UserType ToEntity(this UserTypeModel model, UserType destination)
        {
            destination.Id = model.Id;
            destination.Type = model.Type;
            destination.CreatedDate = model.CreatedDate;
            destination.UpdatedDate = model.UpdatedDate;
            destination.CreatedBy = model.CreatedBy;
            destination.UpdatedBy = model.UpdatedBy;
            return destination;
        }

        #endregion

        #region UserEntity

        public static UserEntityModel ToModel(this UserEntity entity, bool DecoratedModel = true)
        {
            UserEntityModel model = new UserEntityModel();
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.UserTypeId = entity.UserTypeId;
            model.Address = entity.Address;
            model.PhoneNumber = entity.PhoneNumber;
            model.Fax = entity.Fax;
            model.WebSite = entity.WebSite;
            model.Sector = entity.Sector;
            model.CreatedDate = entity.CreatedDate;
            model.UpdatedDate = entity.UpdatedDate;
            model.CreatedBy = entity.CreatedBy;
            model.UpdatedBy = entity.UpdatedBy;
            model.StrUserTypeId = DecoratedModel ? entity.FK_UserType.Type : string.Empty;
            model.EncId = string.Format("seagull={0}", UrlHelperExtensions.ActionEncodedCustom(new { id = entity.Id }));
            return model;
        }

        public static UserEntity ToEntity(this UserEntityModel model)
        {
            UserEntity destination = new UserEntity();
            destination.Id = model.Id;
            destination.Name = model.Name;
            destination.UserTypeId = model.UserTypeId;
            destination.Address = model.Address;
            destination.PhoneNumber = model.PhoneNumber;
            destination.Fax = model.Fax;
            destination.WebSite = model.WebSite;
            destination.Sector = model.Sector;
            destination.CreatedDate = model.CreatedDate;
            destination.UpdatedDate = model.UpdatedDate;
            destination.CreatedBy = model.CreatedBy;
            destination.UpdatedBy = model.UpdatedBy;
            return destination;
        }

        public static UserEntity ToEntity(this UserEntityModel model, UserEntity destination)
        {
            destination.Id = model.Id;
            destination.Name = model.Name;
            destination.UserTypeId = model.UserTypeId;
            destination.Address = model.Address;
            destination.PhoneNumber = model.PhoneNumber;
            destination.Fax = model.Fax;
            destination.WebSite = model.WebSite;
            destination.Sector = model.Sector;
            destination.CreatedDate = model.CreatedDate;
            destination.UpdatedDate = model.UpdatedDate;
            destination.CreatedBy = model.CreatedBy;
            destination.UpdatedBy = model.UpdatedBy;
            return destination;
        }
        #endregion


        #region Email
        public static EmailModel ToModel(this Email entity)
        {
            EmailModel model = new EmailModel();
            model.Id = entity.Id;
            model.From = entity.From;
            model.To = entity.To;
            model.Subject = entity.Subject;
            model.Msg = entity.Msg;
            model.EmailDate = entity.EmailDate;
            model.OperatorId = entity.OperatorId;
            model.Type = entity.Type;

            return model;
        }

        public static Email ToEntity(this EmailModel model)
        {
            Email email = new Email();

            email.Id = model.Id;
            email.From = model.From;
            email.To = model.To;
            email.Subject = model.Subject;
            email.Msg = model.Msg;
            email.EmailDate = model.EmailDate;
            email.OperatorId = model.OperatorId;
            email.Type = model.Type;

            return email;
        }

        public static Email ToEntity(this EmailModel model, Email email)
        {
            email.Id = model.Id;
            email.From = model.From;
            email.To = model.To;
            email.Subject = model.Subject;
            email.Msg = model.Msg;
            email.EmailDate = model.EmailDate;
            email.OperatorId = model.OperatorId;
            email.Type = model.Type;

            return email;
        }
        #endregion

    }
}