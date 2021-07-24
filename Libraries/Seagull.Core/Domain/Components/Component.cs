using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Seagull.Core.Domain.Components
{
	/// <summary>
    /// Represents a Component
    /// </summary>
    public partial class Component : BaseEntity
    {
					public int Id { get; set; } // Id (Primary key)
					public string Name { get; set; } // Name
					public int? CreatedBy { get; set; } // CreatedBy
					public DateTime? CreatedDate { get; set; } // CreatedDate
					public int? UpdatedBy { get; set; } // UpdatedBy
					public DateTime? UpdatedDate { get; set; } // UpdatedDate
		    }
}
