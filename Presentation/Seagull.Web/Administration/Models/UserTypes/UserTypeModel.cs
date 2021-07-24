using System;
using System.Web.Mvc;
//using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Core.Domain.UserTypes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seagull.Admin.Models.UserTypes
{
    //[Validator(typeof(UserRoleValidator))]
    public class UserTypeModel : BaseSeagullEntityModel
    {
	// [Required]
// [StringLength(50)]
// [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [SeagullResourceDisplayName("Admin.Seagull.UserTypes.Fields.Id")]
		[AllowHtml]
		public int Id { get; set; } // Id (Primary key)
        [SeagullResourceDisplayName("Admin.Seagull.UserTypes.Fields.Type")]
		[AllowHtml]
		public string Type { get; set; } // Type
        [SeagullResourceDisplayName("Admin.Seagull.UserTypes.Fields.CreatedDate")]
		[AllowHtml]
		public DateTime? CreatedDate { get; set; } // CreatedDate
        [SeagullResourceDisplayName("Admin.Seagull.UserTypes.Fields.UpdatedDate")]
		[AllowHtml]
		public DateTime? UpdatedDate { get; set; } // UpdatedDate
        [SeagullResourceDisplayName("Admin.Seagull.UserTypes.Fields.CreatedBy")]
		[AllowHtml]
		public int? CreatedBy { get; set; } // CreatedBy
        [SeagullResourceDisplayName("Admin.Seagull.UserTypes.Fields.UpdatedBy")]
		[AllowHtml]
		public int? UpdatedBy { get; set; } // UpdatedBy
		public UserType CurrentUserType { get; set; }
        public string EncId { get; set; } // Encrypt Url
    }
}
	
