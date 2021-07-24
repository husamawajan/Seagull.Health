using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Seagull.Core.Domain.Functionss
{
	/// <summary>
    /// Represents a Functions
    /// </summary>
    public partial class Functions : BaseEntity
    {
					public int Id { get; set; } // Id (Primary key)
					public string Name { get; set; } // Name
					public string FunctionFormula { get; set; } // FunctionFormula
					public DateTime? CreatedDate { get; set; } // CreatedDate
					public DateTime? UpdatedDate { get; set; } // UpdatedDate
					public int? CreatedBy { get; set; } // CreatedBy
					public int? UpdatedBy { get; set; } // UpdatedBy
		    }
}
	
