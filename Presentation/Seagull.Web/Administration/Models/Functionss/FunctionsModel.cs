using System;
using System.Web.Mvc;
//using FluentValidation.Attributes;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;
using Seagull.Core.Domain.Functionss;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Seagull.Admin.Models.Functionss
{
    //[Validator(typeof(UserRoleValidator))]
    public class FunctionsModel : BaseSeagullEntityModel
    {
	// [Required]
// [StringLength(50)]
// [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.Id")]
		[AllowHtml]
		public int Id { get; set; } // Id (Primary key)
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.Name")]
		[AllowHtml]
		public string Name { get; set; } // Name
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.FunctionFormula")]
		[AllowHtml]
		public string FunctionFormula { get; set; } // FunctionFormula
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.CreatedDate")]
		[AllowHtml]
		public DateTime? CreatedDate { get; set; } // CreatedDate
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.UpdatedDate")]
		[AllowHtml]
		public DateTime? UpdatedDate { get; set; } // UpdatedDate
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.CreatedBy")]
		[AllowHtml]
		public int? CreatedBy { get; set; } // CreatedBy
		[SeagullResourceDisplayName("Admin.MWI.Functionss.Fields.UpdatedBy")]
		[AllowHtml]
		public int? UpdatedBy { get; set; } // UpdatedBy
		public Functions CurrentFunctions { get; set; }
    }
}
	
