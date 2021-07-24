using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Core.Domain.Media;
using Seagull.Core.Domain.Security;
using Seagull.Services.Authentication.External;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Media;
using Seagull.Services.Messages;
using Seagull.Services.Seo;
using Seagull.Services.Stores;
using Seagull.Web.Framework.Security.Captcha;
using Seagull.Web.Models.Common;
using Seagull.Web.Models.User;
using WebGrease.Css.Extensions;

namespace Seagull.Web.Factories
{
    /// <summary>
    /// Represents the user model factory
    /// </summary>
    public partial class UserModelFactory : IUserModelFactory
    {
        #region Fields

        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly UserSettings _userSettings;
        private readonly AddressSettings _addressSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPictureService _pictureService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly IDownloadService _downloadService;

        private readonly MediaSettings _mediaSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

        #endregion

        #region Ctor

        public UserModelFactory(IAddressModelFactory addressModelFactory, 
            IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings, 
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IGenericAttributeService genericAttributeService,
            RewardPointsSettings rewardPointsSettings,
            UserSettings userSettings,
            AddressSettings addressSettings, 
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IPictureService pictureService, 
            IOpenAuthenticationService openAuthenticationService,
            IDownloadService downloadService,
            MediaSettings mediaSettings,
            CaptchaSettings captchaSettings,
            SecuritySettings securitySettings,
            ExternalAuthenticationSettings externalAuthenticationSettings)
        {
            this._addressModelFactory = addressModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
            this._userAttributeParser = userAttributeParser;
            this._userAttributeService = userAttributeService;
            this._genericAttributeService = genericAttributeService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._userSettings = userSettings;
            this._addressSettings = addressSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._pictureService = pictureService;
            this._openAuthenticationService = openAuthenticationService;
            this._downloadService = downloadService;
            this._mediaSettings = mediaSettings;
            this._captchaSettings = captchaSettings;
            this._securitySettings = securitySettings;
            this._externalAuthenticationSettings = externalAuthenticationSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the custom user attribute models
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="overrideAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>List of the user attribute model</returns>
        public virtual IList<UserAttributeModel> PrepareCustomUserAttributes(User user, string overrideAttributesXml = "")
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var result = new List<UserAttributeModel>();

            var userAttributes = _userAttributeService.GetAllUserAttributes();
            foreach (var attribute in userAttributes)
            {
                var attributeModel = new UserAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    IsRequired = attribute.IsRequired,
                };

                //set already selected attributes
                var selectedAttributesXml = !String.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml : 
                    user.GetAttribute<string>(SystemUserAttributeNames.CustomUserAttributes, _genericAttributeService);

                result.Add(attributeModel);
            }


            return result;
        }

        /// <summary>
        /// Prepare the user info model
        /// </summary>
        /// <param name="model">User info model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>User info model</returns>
        public virtual UserInfoModel PrepareUserInfoModel(UserInfoModel model, User user, 
            bool excludeProperties, string overrideCustomUserAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (user == null)
                throw new ArgumentNullException("user");

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.FirstName = user.GetAttribute<string>(SystemUserAttributeNames.FirstName);
                model.LastName = user.GetAttribute<string>(SystemUserAttributeNames.LastName);
                model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
                var dateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
                model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
                model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
                model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
                model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                model.StateProvinceId = user.GetAttribute<int>(SystemUserAttributeNames.StateProvinceId);
                model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
                model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);


                model.Signature = user.GetAttribute<string>(SystemUserAttributeNames.Signature);

                model.Email = user.Email;
                model.Username = user.Username;
            }
            else
            {
                if (_userSettings.UsernamesEnabled && !_userSettings.AllowUsersToChangeUsernames)
                    model.Username = user.Username;
            }

            if (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = user.EmailToRevalidate;

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.CountryRequired = _userSettings.CountryRequired;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _userSettings.StateProvinceRequired;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;

            //external authentication
            model.NumberOfExternalAuthenticationProviders = _openAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_workContext.CurrentUser, _storeContext.CurrentStore.Id).Count;
            foreach (var ear in _openAuthenticationService.GetExternalIdentifiersFor(user))
            {
                var authMethod = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(ear.ProviderSystemName);
                if (authMethod == null || !authMethod.IsMethodActive(_externalAuthenticationSettings))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new UserInfoModel.AssociatedExternalAuthModel
                {
                    Id = ear.Id,
                    Email = ear.Email,
                    ExternalIdentifier = ear.ExternalIdentifier,
                    AuthMethodName = authMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id)
                });
            }

            //custom user attributes
            var customAttributes = PrepareCustomUserAttributes(user, overrideCustomUserAttributesXml);
            customAttributes.ForEach(model.UserAttributes.Add);

            return model;
        }

        /// <summary>
        /// Prepare the user register model
        /// </summary>
        /// <param name="model">User register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>User register model</returns>
        public virtual RegisterModel PrepareRegisterModel(RegisterModel model, bool excludeProperties, 
            string overrideCustomUserAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });
            
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _userSettings.DateOfBirthRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.CountryRequired = _userSettings.CountryRequired;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _userSettings.StateProvinceRequired;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.AcceptPrivacyPolicyEnabled = _userSettings.AcceptPrivacyPolicyEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = _userSettings.EnteringEmailTwice;
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });

                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_userSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"), 
                            Value = "0"
                        });
                    }

                }
            }

            //custom user attributes
            var customAttributes = PrepareCustomUserAttributes(_workContext.CurrentUser, overrideCustomUserAttributesXml);
            customAttributes.ForEach(model.UserAttributes.Add);

            return model;
        }

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        public virtual LoginModel PrepareLoginModel(bool? checkoutAsGuest)
        {
            var model = new LoginModel();
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault();
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return model;
        }

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <returns>Password recovery model</returns>
        public virtual PasswordRecoveryModel PreparePasswordRecoveryModel()
        {
            var model = new PasswordRecoveryModel();
            return model;
        }

        /// <summary>
        /// Prepare the password recovery confirm model
        /// </summary>
        /// <returns>Password recovery confirm model</returns>
        public virtual PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel()
        {
            var model = new PasswordRecoveryConfirmModel();
            return model;
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <returns>Register result model</returns>
        public virtual RegisterResultModel PrepareRegisterResultModel(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel
            {
                Result = resultText
            };
            return model;
        }

        /// <summary>
        /// Prepare the user navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>User navigation model</returns>
        public virtual UserNavigationModel PrepareUserNavigationModel(int selectedTabId = 0)
        {
            var model = new UserNavigationModel();

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserInfo",
                Title = _localizationService.GetResource("Account.UserInfo"),
                Tab = UserNavigationEnum.Info,
                ItemClass = "user-info"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserAddresses",
                Title = _localizationService.GetResource("Account.UserAddresses"),
                Tab = UserNavigationEnum.Addresses,
                ItemClass = "user-addresses"
            });

            model.UserNavigationItems.Add(new UserNavigationItemModel
            {
                RouteName = "UserChangePassword",
                Title = _localizationService.GetResource("Account.ChangePassword"),
                Tab = UserNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_userSettings.AllowUsersToUploadAvatars)
            {
                model.UserNavigationItems.Add(new UserNavigationItemModel
                {
                    RouteName = "UserAvatar",
                    Title = _localizationService.GetResource("Account.Avatar"),
                    Tab = UserNavigationEnum.Avatar,
                    ItemClass = "user-avatar"
                });
            }
           

            model.SelectedTab = (UserNavigationEnum)selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the user address list model
        /// </summary>
        /// <returns>User address list model</returns>
        public virtual UserAddressListModel PrepareUserAddressListModel()
        {
            var addresses = _workContext.CurrentUser.Addresses
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                .ToList();

            var model = new UserAddressListModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                _addressModelFactory.PrepareAddressModel(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));
                model.Addresses.Add(addressModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        public virtual ChangePasswordModel PrepareChangePasswordModel()
        {
            var model = new ChangePasswordModel();
            return model;
        }

        /// <summary>
        /// Prepare the user avatar model
        /// </summary>
        /// <param name="model">User avatar model</param>
        /// <returns>User avatar model</returns>
        public virtual UserAvatarModel PrepareUserAvatarModel(UserAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvatarUrl = _pictureService.GetPictureUrl(
                _workContext.CurrentUser.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        #endregion
    }
}
