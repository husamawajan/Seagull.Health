using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.User;

namespace Seagull.Web.Models.User
{
    [Validator(typeof(LoginValidator))]
    public partial class LoginModel : BaseSeagullModel
    {
        public bool CheckoutAsGuest { get; set; }

        [SeagullResourceDisplayName("Account.Login.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [SeagullResourceDisplayName("Account.Login.Fields.UserName")]
        [AllowHtml]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [SeagullResourceDisplayName("Account.Login.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [SeagullResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}