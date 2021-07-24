using System;
using System.Web.Mvc;
//using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Core.Domain.UserEntitys;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seagull.Admin.Models.UserEntitys
{
    //[Validator(typeof(UserRoleValidator))]
    public class UserEntityModel : BaseSeagullEntityModel
    {
	// [Required]
// [StringLength(50)]
// [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
		[SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.Id")]
		[AllowHtml]
		public int Id { get; set; } // Id (Primary key)
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.Name")]
		[AllowHtml]
		public string Name { get; set; } // Name
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.UserTypeId")]
		[AllowHtml]
		public int? UserTypeId { get; set; } // UserTypeId
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.CreatedDate")]
		[AllowHtml]
		public DateTime? CreatedDate { get; set; } // CreatedDate
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.UpdatedDate")]
		[AllowHtml]
		public DateTime? UpdatedDate { get; set; } // UpdatedDate
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.CreatedBy")]
		[AllowHtml]
		public int? CreatedBy { get; set; } // CreatedBy
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.UpdatedBy")]
		[AllowHtml]
        public int? UpdatedBy { get; set; } // UpdatedBy 
        [SeagullResourceDisplayName("Admin.Seagull.UserEntitys.Fields.UserTypeId")]
        [AllowHtml]
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string WebSite { get; set; }
        public int? Sector { get; set; }
        public string StrUserTypeId { get; set; } // StrUserTypeId
		public UserEntity CurrentUserEntity { get; set; }
        public string EncId { get; set; } // Encrypt Url
    }
}
