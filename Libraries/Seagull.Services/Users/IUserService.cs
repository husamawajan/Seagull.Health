using System;
using System.Collections.Generic;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.UserEntitys;
namespace Seagull.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService
    {
        #region Users

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="email">Email; null to load all users</param>
        /// <param name="username">Username; null to load all users</param>
        /// <param name="firstName">First name; null to load all users</param>
        /// <param name="lastName">Last name; null to load all users</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all users</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all users</param>
        /// <param name="phone">Phone; null to load all users</param>
        /// <param name="zipPostalCode">Phone; null to load all users</param>
        /// <param name="ipAddress">IP address; null to load all users</param>
        /// <param name="loadOnlyWithShoppingCart">Value indicating whether to load users only with shopping cart</param>
        /// <param name="sct">Value indicating what shopping cart type to filter; userd when 'loadOnlyWithShoppingCart' param is 'true'</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        IPagedList<User> GetAllUsers(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int affiliateId = 0, int vendorId = 0,
            int[] userRoleIds = null, string email = null, string username = null,
            string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string phone = null, string zipPostalCode = null,
            string ipAddress = null, bool loadOnlyWithShoppingCart = false, int? sct = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        void DeleteUser(User user);

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>A user</returns>
        User GetUserById(int userId);

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>Users</returns>
        IList<User> GetUsersByIds(int[] userIds);

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>A user</returns>
        User GetUserByGuid(Guid userGuid);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Get user by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        User GetUserBySystemName(string systemName);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Insert a guest user
        /// </summary>
        /// <returns>User</returns>
        User InsertGuestUser();

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        void InsertUser(User user);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        void UpdateUser(User user);



        ///// <summary>
        ///// Delete guest user records
        ///// </summary>
        ///// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        ///// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        ///// <param name="onlyWithoutShoppingCart">A value indicating whether to delete users only without shopping cart</param>
        ///// <returns>Number of deleted users</returns>
        //int DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart);

        #endregion

        #region User roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        void DeleteUserRole(UserRole userRole);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleById(int userRoleId);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleBySystemName(string systemName);

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User roles</returns>
        PagedList<UserRole> GetAllUserRoles(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false);
        IList<UserRole> GetAllUserRoles(bool showHidden = false);
        IList<UserRole> GetAllUserRoleByUserTypeId(int UserTypeId, bool showHidden = false);

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        void InsertUserRole(UserRole userRole);

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        void UpdateUserRole(UserRole userRole);
        PagedList<User> GetAllLiaisonOfficerUsers(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false, int EntityId = 0, bool withPremission = false);
        PagedList<User> GetAllUsers(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false, int EntityId = 0, bool withPremission = false);
        bool CheckCurrentUserByPermissionName(ICollection<UserRole> userRole, string permName);
        #endregion

        #region User passwords

        /// <summary>
        /// Gets user passwords
        /// </summary>
        /// <param name="userId">User identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>List of user passwords</returns>
        IList<UserPassword> GetUserPasswords(int? userId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null);

        /// <summary>
        /// Get current user password
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User password</returns>
        UserPassword GetCurrentPassword(int userId);

        /// <summary>
        /// Insert a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        void InsertUserPassword(UserPassword userPassword);

        /// <summary>
        /// Update a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        void UpdateUserPassword(UserPassword userPassword);

        #endregion

    }
}