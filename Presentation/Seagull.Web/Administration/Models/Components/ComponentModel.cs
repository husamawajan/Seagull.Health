using System;
using System.Web.Mvc;
//using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Core.Domain.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seagull.Admin.Models.Components
{
    //[Validator(typeof(UserRoleValidator))]
    public class ComponentModel : BaseSeagullEntityModel
    {
	// [Required]
// [StringLength(50)]
// [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
		[SeagullResourceDisplayName("Admin.Seagull.Components.Fields.Id")]
		[AllowHtml]
		public int Id { get; set; } // Id (Primary key)
		[SeagullResourceDisplayName("Admin.Seagull.Components.Fields.Name")]
		[AllowHtml]
		public string Name { get; set; } // Name
		[SeagullResourceDisplayName("Admin.Seagull.Components.Fields.CreatedBy")]
		[AllowHtml]
		public int? CreatedBy { get; set; } // CreatedBy
		[SeagullResourceDisplayName("Admin.Seagull.Components.Fields.CreatedDate")]
		[AllowHtml]
		public DateTime? CreatedDate { get; set; } // CreatedDate
		[SeagullResourceDisplayName("Admin.Seagull.Components.Fields.UpdatedBy")]
		[AllowHtml]
		public int? UpdatedBy { get; set; } // UpdatedBy
		[SeagullResourceDisplayName("Admin.Seagull.Components.Fields.UpdatedDate")]
		[AllowHtml]
		public DateTime? UpdatedDate { get; set; } // UpdatedDate
        public string EncId { get; set; } // Encrypt Url
		public Seagull.Core.Domain.Components.Component CurrentComponent { get; set; }


    }
}
