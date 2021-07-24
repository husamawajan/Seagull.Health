using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    public partial class UserUserSettingsModel : BaseSeagullModel
    {
        public UserUserSettingsModel()
        {
            UserSettings = new UserSettingsModel();
            AddressSettings = new AddressSettingsModel();
            DateTimeSettings = new DateTimeSettingsModel();
            ExternalAuthenticationSettings = new ExternalAuthenticationSettingsModel();
        }
        public UserSettingsModel UserSettings { get; set; }
        public AddressSettingsModel AddressSettings { get; set; }
        public DateTimeSettingsModel DateTimeSettings { get; set; }
        public ExternalAuthenticationSettingsModel ExternalAuthenticationSettings { get; set; }

        #region Nested classes

        public partial class UserSettingsModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.UsernamesEnabled")]
            public bool UsernamesEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToChangeUsernames")]
            public bool AllowUsersToChangeUsernames { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CheckUsernameAvailabilityEnabled")]
            public bool CheckUsernameAvailabilityEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.UserRegistrationType")]
            public int UserRegistrationType { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToUploadAvatars")]
            public bool AllowUsersToUploadAvatars { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultAvatarEnabled")]
            public bool DefaultAvatarEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.ShowUsersLocation")]
            public bool ShowUsersLocation { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.ShowUsersJoinDate")]
            public bool ShowUsersJoinDate { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowViewingProfiles")]
            public bool AllowViewingProfiles { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.NotifyNewUserRegistration")]
            public bool NotifyNewUserRegistration { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.RequireRegistrationForDownloadableProducts")]
            public bool RequireRegistrationForDownloadableProducts { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.HideDownloadableProductsTab")]
            public bool HideDownloadableProductsTab { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.HideBackInStockSubscriptionsTab")]
            public bool HideBackInStockSubscriptionsTab { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.UserNameFormat")]
            public int UserNameFormat { get; set; }
            
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordMinLength")]
            public int PasswordMinLength { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.UnduplicatedPasswordsNumber")]
            public int UnduplicatedPasswordsNumber { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordRecoveryLinkDaysValid")]
            public int PasswordRecoveryLinkDaysValid { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultPasswordFormat")]
            public int DefaultPasswordFormat { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.PasswordLifetime")]
            public int PasswordLifetime { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.FailedPasswordAllowedAttempts")]
            public int FailedPasswordAllowedAttempts { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.FailedPasswordLockoutMinutes")]
            public int FailedPasswordLockoutMinutes { get; set; }
            
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.NewsletterEnabled")]
            public bool NewsletterEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.NewsletterTickedByDefault")]
            public bool NewsletterTickedByDefault { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.HideNewsletterBlock")]
            public bool HideNewsletterBlock { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.NewsletterBlockAllowToUnsubscribe")]
            public bool NewsletterBlockAllowToUnsubscribe { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StoreLastVisitedPage")]
            public bool StoreLastVisitedPage { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.EnteringEmailTwice")]
            public bool EnteringEmailTwice { get; set; }


            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.GenderEnabled")]
            public bool GenderEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DateOfBirthEnabled")]
            public bool DateOfBirthEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DateOfBirthRequired")]
            public bool DateOfBirthRequired { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DateOfBirthMinimumAge")]
            [UIHint("Int32Nullable")]
            public int? DateOfBirthMinimumAge { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CityEnabled")]
            public bool CityEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CityRequired")]
            public bool CityRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CountryEnabled")]
            public bool CountryEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.CountryRequired")]
            public bool CountryRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.StateProvinceRequired")]
            public bool StateProvinceRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.FaxRequired")]
            public bool FaxRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AcceptPrivacyPolicyEnabled")]
            public bool AcceptPrivacyPolicyEnabled { get; set; }
        }

        public partial class AddressSettingsModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyEnabled")]
            public bool CompanyEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CompanyRequired")]
            public bool CompanyRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressEnabled")]
            public bool StreetAddressEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddressRequired")]
            public bool StreetAddressRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Enabled")]
            public bool StreetAddress2Enabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StreetAddress2Required")]
            public bool StreetAddress2Required { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeEnabled")]
            public bool ZipPostalCodeEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.ZipPostalCodeRequired")]
            public bool ZipPostalCodeRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityEnabled")]
            public bool CityEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CityRequired")]
            public bool CityRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.CountryEnabled")]
            public bool CountryEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.StateProvinceEnabled")]
            public bool StateProvinceEnabled { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneEnabled")]
            public bool PhoneEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.PhoneRequired")]
            public bool PhoneRequired { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxEnabled")]
            public bool FaxEnabled { get; set; }
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AddressFormFields.FaxRequired")]
            public bool FaxRequired { get; set; }
        }

        public partial class DateTimeSettingsModel : BaseSeagullModel
        {
            public DateTimeSettingsModel()
            {
                AvailableTimeZones = new List<SelectListItem>();
            }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.AllowUsersToSetTimeZone")]
            public bool AllowUsersToSetTimeZone { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultStoreTimeZone")]
            public string DefaultStoreTimeZoneId { get; set; }

            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.DefaultStoreTimeZone")]
            public IList<SelectListItem> AvailableTimeZones { get; set; }
        }

        public partial class ExternalAuthenticationSettingsModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.Configuration.Settings.UserUser.ExternalAuthenticationAutoRegisterEnabled")]
            public bool AutoRegisterEnabled { get; set; }
        }
        #endregion
    }
}