using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Validators.User;

namespace Seagull.Web.Models.User
{
    [Validator(typeof(ChangePasswordValidator))]
    public partial class ChangePasswordModel : BaseSeagullModel
    {
        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [SeagullResourceDisplayName("Account.ChangePassword.Fields.OldPassword")]
        public string OldPassword { get; set; }

        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [SeagullResourceDisplayName("Account.ChangePassword.Fields.NewPassword")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [SeagullResourceDisplayName("Account.ChangePassword.Fields.ConfirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        public string Result { get; set; }

    }
}