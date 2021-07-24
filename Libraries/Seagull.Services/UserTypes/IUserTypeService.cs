using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Core.Domain.UserTypes;
using Seagull.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace Seagull.Services.UserTypes
{
    /// <summary>
    /// UserType Service interface
    /// </summary>
	public partial interface IUserTypeService
	{
		/// <summary>
        /// Deletes a usertype
        /// </summary>
        /// <param name="usertype">UserType</param>
        void DeleteUserType(UserType usertype);

        /// <summary>
        /// Gets all usertypes
        /// </summary>
        /// <returns>UserType collection</returns>
        IPagedList<UserType> GetAllUserTypes(string description, int pageIndex, int pageSize);
        IList<UserType> GetAllUserTypes();
        /// <summary>
        /// Gets a usertype 
        /// </summary>
        /// <param name="usertypeId">UserType identifier</param>
        /// <returns>UserType</returns>
        UserType GetUserTypeById(decimal usertypeId);

        /// <summary>
        /// Inserts a usertype
        /// </summary>
        /// <param name="usertype">UserType</param>
        void InsertUserType(UserType usertype);

        /// <summary>
        /// Updates the usertype
        /// </summary>
        /// <param name="usertype">UserType</param>
        void UpdateUserType(UserType usertype);

		/// <summary>
        /// Gets all usertypes
        /// </summary>
        /// <returns>UserType PagedList</returns>
		PagedList<UserType> GetAllUserTypes(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false);
        PagedList<UserType> GetAllUserTypesProject(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false);
	}
}
	
 
 