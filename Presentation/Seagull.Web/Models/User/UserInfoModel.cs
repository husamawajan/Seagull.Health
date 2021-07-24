using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.User;

namespace Seagull.Web.Models.User
{
    [Validator(typeof(UserInfoValidator))]
    public partial class UserInfoModel : BaseSeagullModel
    {
        public UserInfoModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            this.UserAttributes = new List<UserAttributeModel>();
        }

        [SeagullResourceDisplayName("Account.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }
        [SeagullResourceDisplayName("Account.Fields.EmailToRevalidate")]
        public string EmailToRevalidate { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }
        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }
        [SeagullResourceDisplayName("Account.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [SeagullResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        [SeagullResourceDisplayName("Account.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [SeagullResourceDisplayName("Account.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }


        public bool DateOfBirthEnabled { get; set; }
        [SeagullResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [SeagullResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [SeagullResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }

       

        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [SeagullResourceDisplayName("Account.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [SeagullResourceDisplayName("Account.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }

        //preferences
        public bool SignatureEnabled { get; set; }
        [SeagullResourceDisplayName("Account.Fields.Signature")]
        [AllowHtml]
        public string Signature { get; set; }

        //time zone
        [SeagullResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowUsersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [SeagullResourceDisplayName("Account.Fields.VatNumber")]
        [AllowHtml]
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }

        //external authentication
        [SeagullResourceDisplayName("Account.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }
        public int NumberOfExternalAuthenticationProviders { get; set; }

        public IList<UserAttributeModel> UserAttributes { get; set; }

        #region Nested classes

        public partial class AssociatedExternalAuthModel : BaseSeagullEntityModel
        {
            public string Email { get; set; }

            public string ExternalIdentifier { get; set; }

            public string AuthMethodName { get; set; }
        }
        
        #endregion
    }
}