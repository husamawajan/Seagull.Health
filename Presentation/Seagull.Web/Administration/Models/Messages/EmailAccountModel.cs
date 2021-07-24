using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Messages;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Messages
{
    [Validator(typeof(EmailAccountValidator))]
    public partial class EmailAccountModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.DisplayName")]
        [AllowHtml]
        public string DisplayName { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Host")]
        [AllowHtml]
        public string Host { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Port")]
        public int Port { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.Password")]
        [AllowHtml]
        [DataType(DataType.Password)]
        [NoTrim]
        public string Password { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.EnableSsl")]
        public bool EnableSsl { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.IsDefaultEmailAccount")]
        public bool IsDefaultEmailAccount { get; set; }


        [SeagullResourceDisplayName("Admin.Configuration.EmailAccounts.Fields.SendTestEmailTo")]
        [AllowHtml]
        public string SendTestEmailTo { get; set; }

    }
}