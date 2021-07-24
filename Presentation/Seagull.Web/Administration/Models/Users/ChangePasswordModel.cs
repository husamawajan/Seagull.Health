using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class ChangePasswordModel : BaseSeagullModel
    {
        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [Required]
        public string OldPassword { get; set; }

        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [Required]
        public string NewPassword { get; set; }

        [AllowHtml]
        [NoTrim]
        [DataType(DataType.Password)]
        [Required]
        public string ConfirmNewPassword { get; set; }

    }
}