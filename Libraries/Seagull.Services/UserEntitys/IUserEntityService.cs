using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Core.Domain.UserEntitys;
using Seagull.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace Seagull.Services.UserEntitys
{
    /// <summary>
    /// UserEntity Service interface
    /// </summary>
	public partial interface IUserEntityService
	{
		/// <summary>
        /// Deletes a userentity
        /// </summary>
        /// <param name="userentity">UserEntity</param>
        void DeleteUserEntity(UserEntity userentity);

        /// <summary>
        /// Gets all userentitys
        /// </summary>
        /// <returns>UserEntity collection</returns>
        IPagedList<UserEntity> GetAllUserEntitys(string description, int pageIndex, int pageSize);
        IList<UserEntity> GetAllUserEntitys();
        IList<UserEntity> GetAllUserEntitysByUserTypeId(int UserTypeId);
        /// <summary>
        /// Gets a userentity 
        /// </summary>
        /// <param name="userentityId">UserEntity identifier</param>
        /// <returns>UserEntity</returns>
        UserEntity GetUserEntityById(int userentityId);
        UserEntity GetUserEntityByName(string EntityName);
       

        /// <summary>
        /// Inserts a userentity
        /// </summary>
        /// <param name="userentity">UserEntity</param>
        void InsertUserEntity(UserEntity userentity);

        /// <summary>
        /// Updates the userentity
        /// </summary>
        /// <param name="userentity">UserEntity</param>
        void UpdateUserEntity(UserEntity userentity);

		/// <summary>
        /// Gets all userentitys
        /// </summary>
        /// <returns>UserEntity PagedList</returns>
		PagedList<UserEntity> GetAllUserEntitys(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false);
	}
}
 
