using Seagull.Core.Domain.UserTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Seagull.Core.Domain.UserEntitys
{
	/// <summary>
    /// Represents a UserEntity
    /// </summary>
    public partial class UserEntity : BaseEntity
    {
					public int Id { get; set; } // Id (Primary key)
					public string Name { get; set; } // Name
					public int? UserTypeId { get; set; } // UserTypeId
					public DateTime? CreatedDate { get; set; } // CreatedDate
					public DateTime? UpdatedDate { get; set; } // UpdatedDate
					public int? CreatedBy { get; set; } // CreatedBy
					public int? UpdatedBy { get; set; } // UpdatedBy
                    public virtual UserType FK_UserType { get; set; } // FK_UserType
                    public string Address { get; set; }
                    public string PhoneNumber { get; set; }
                    public string Fax { get; set; }
                    public string WebSite { get; set; }
                    public int? Sector { get; set; }
		    }
}
