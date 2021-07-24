using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.User;

namespace Seagull.Web.Models.User
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public partial class PasswordRecoveryModel : BaseSeagullModel
    {
        [AllowHtml]
        [SeagullResourceDisplayName("Account.PasswordRecovery.Email")]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}