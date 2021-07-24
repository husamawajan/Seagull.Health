using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Users;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    [Validator(typeof(UserValidator))]
    public partial class UserModel : BaseSeagullEntityModel
    {
        public UserModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.SendEmail = new SendEmailModel() { SendImmediately = true };
            this.SendPm = new SendPmModel();

            this.SelectedUserRoleIds= new List<int>();
            this.AvailableUserRoles = new List<SelectListItem>();

            this.AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            this.AvailableCountries = new List<SelectListItem>();
            this.AvailableStates = new List<SelectListItem>();
            this.UserAttributes = new List<UserAttributeModel>();
            this.AvailableUserTypes = new List<SelectListItem>();
            this.AvailableEntityUsers = new List<SelectListItem>();
        }
        public bool UsernamesEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.ShowDashboard")]
        [AllowHtml]
        public bool ShowDashboard { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Password")]
        [AllowHtml]
        [DataType(DataType.Password)]
        [NoTrim]
        public string Password { get; set; }

        

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Gender")]
        public string Gender { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.FullName")]
        public string FullName { get; set; }
        
        public bool DateOfBirthEnabled { get; set; }
        [UIHint("DateNullable")]
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }
        public bool StreetAddressEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.StreetAddress2")]
        [AllowHtml]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        public bool CountryEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Fax")]
        [AllowHtml]
        public string Fax { get; set; }

        public List<UserAttributeModel> UserAttributes { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.RegisteredInStore")]
        public string RegisteredInStore { get; set; }



        [SeagullResourceDisplayName("Admin.Users.Users.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Active")]
        public bool Active { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.Affiliate")]
        public string AffiliateName { get; set; }




        //time zone
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.TimeZoneId")]
        [AllowHtml]
        public string TimeZoneId { get; set; }

        public bool AllowUsersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //registration date
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        //IP adderss
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.IPAddress")]
        public string LastIpAddress { get; set; }


        [SeagullResourceDisplayName("Admin.Users.Users.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }


        //user roles
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        public string UserRoleNames { get; set; }
        public List<SelectListItem> AvailableUserRoles { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.UserRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedUserRoleIds { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.Fields.UserType")]
        public int? UserTypeId { get; set; }
        public string strUserTypeId { get; set; }
        public List<SelectListItem> AvailableUserTypes { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.Fields.EntityUser")]
        public int? EntityUserId { get; set; }
        public string strEntityUser { get; set; }
        public List<SelectListItem> AvailableEntityUsers { get; set; }

        public string EncId { get; set; } // Encrypt Url

        public int? UserOperatorId { get; set; }
        public string strUserOperatorId { get; set; }

        public int? UserMailId { get; set; }
        public string strUserMailId { get; set; }



        //send email model
        public SendEmailModel SendEmail { get; set; }
        //send PM model
        public SendPmModel SendPm { get; set; }
        //send the welcome message
        public bool AllowSendingOfWelcomeMessage { get; set; }
        //re-send the activation message
        public bool AllowReSendingOfActivationMessage { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }


        #region Nested classes

        public partial class StoreModel : BaseSeagullEntityModel
        {
            public string Name { get; set; }
        }

        public partial class AssociatedExternalAuthModel : BaseSeagullEntityModel
        {
            [SeagullResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.Email")]
            public string Email { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.ExternalIdentifier")]
            public string ExternalIdentifier { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.AuthMethodName")]
            public string AuthMethodName { get; set; }
        }

        public partial class RewardPointsHistoryModel : BaseSeagullEntityModel
        {
            [SeagullResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Store")]
            public string StoreName { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.PointsBalance")]
            public string PointsBalance { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Message")]
            [AllowHtml]
            public string Message { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class SendEmailModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.Users.Users.SendEmail.Subject")]
            [AllowHtml]
            public string Subject { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.SendEmail.Body")]
            [AllowHtml]
            public string Body { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.SendEmail.SendImmediately")]
            public bool SendImmediately { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.SendEmail.DontSendBeforeDate")]
            [UIHint("DateTimeNullable")]
            public DateTime? DontSendBeforeDate { get; set; }
        }

        public partial class SendPmModel : BaseSeagullModel
        {
            [SeagullResourceDisplayName("Admin.Users.Users.SendPM.Subject")]
            public string Subject { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial class OrderModel : BaseSeagullEntityModel
        {
            public override int Id { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.Orders.CustomOrderNumber")]
            public string CustomOrderNumber { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.Orders.OrderStatus")]
            public string OrderStatus { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.Orders.OrderStatus")]
            public int OrderStatusId { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.Orders.Store")]
            public string StoreName { get; set; }

            [SeagullResourceDisplayName("Admin.Users.Users.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class ActivityLogModel : BaseSeagullEntityModel
        {
            [SeagullResourceDisplayName("Admin.Users.Users.ActivityLog.ActivityLogType")]
            public string ActivityLogTypeName { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.ActivityLog.Comment")]
            public string Comment { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.ActivityLog.CreatedOn")]
            public DateTime CreatedOn { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.ActivityLog.IpAddress")]
            public string IpAddress { get; set; }
        }

        public partial class BackInStockSubscriptionModel : BaseSeagullEntityModel
        {
            [SeagullResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Store")]
            public string StoreName { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Product")]
            public int ProductId { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Product")]
            public string ProductName { get; set; }
            [SeagullResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class UserAttributeModel : BaseSeagullEntityModel
        {
            public UserAttributeModel()
            {
                Values = new List<UserAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }


            public IList<UserAttributeValueModel> Values { get; set; }

        }

        public partial class UserAttributeValueModel : BaseSeagullEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}