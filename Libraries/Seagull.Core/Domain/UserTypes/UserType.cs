using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Seagull.Core.Domain.UserTypes
{
	/// <summary>
    /// Represents a UserType
    /// </summary>
    public partial class UserType : BaseEntity
    {
					public int Id { get; set; } // Id (Primary key)
					public string Type { get; set; } // Type
					public DateTime? CreatedDate { get; set; } // CreatedDate
					public DateTime? UpdatedDate { get; set; } // UpdatedDate
					public int? CreatedBy { get; set; } // CreatedBy
					public int? UpdatedBy { get; set; } // UpdatedBy
		    }
}
	
