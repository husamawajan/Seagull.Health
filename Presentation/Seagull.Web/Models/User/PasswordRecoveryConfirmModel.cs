using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.User;

namespace Seagull.Web.Models.User
{
    [Validator(typeof(PasswordRecoveryConfirmValidator))]
    public partial class PasswordRecoveryConfirmModel : BaseSeagullModel
    {
        [AllowHtml]
        [DataType(DataType.Password)]
        [NoTrim]
        [SeagullResourceDisplayName("Account.PasswordRecovery.NewPassword")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [SeagullResourceDisplayName("Account.PasswordRecovery.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public bool DisablePasswordChanging { get; set; }
        public string Result { get; set; }
    }
}