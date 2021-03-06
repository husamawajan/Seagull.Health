using System;
using System.Linq;
using Seagull.Core.Domain.Common;

namespace Seagull.Core.Domain.Users
{
    public static class UserExtensions
    {
        #region User role

        /// <summary>
        /// Gets a value indicating whether user is in a certain user role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="userRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this User user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrEmpty(userRoleSystemName))
                throw new ArgumentNullException("userRoleSystemName");

            var result = user.UserRoles
                .FirstOrDefault(cr => (!onlyActiveUserRoles || cr.Active) && (cr.SystemName == userRoleSystemName)) != null;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether user a search engine
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.IsSystemAccount || String.IsNullOrEmpty(user.SystemName))
                return false;

            var result = user.SystemName.Equals(SystemUserNames.SearchEngine, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the user is a built-in record for background tasks
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        public static bool IsBackgroundTaskAccount(this User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.IsSystemAccount || String.IsNullOrEmpty(user.SystemName))
                return false;

            var result = user.SystemName.Equals(SystemUserNames.BackgroundTask, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsAdmin(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Administrators, onlyActiveUserRoles);
        }

        ///// <summary>
        ///// Gets a value indicating whether user is a forum moderator
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        ///// <returns>Result</returns>
        //public static bool IsForumModerator(this User user, bool onlyActiveUserRoles = true)
        //{
        //    return IsInUserRole(user, SystemUserRoleNames.ForumModerators, onlyActiveUserRoles);
        //}

        /// <summary>
        /// Gets a value indicating whether user is registered
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Registered, onlyActiveUserRoles);
        }

        ///// <summary>
        ///// Gets a value indicating whether user is guest
        ///// </summary>
        ///// <param name="user">User</param>
        ///// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        ///// <returns>Result</returns>
        //public static bool IsGuest(this User user, bool onlyActiveUserRoles = true)
        //{
        //    return IsInUserRole(user, SystemUserRoleNames.Guests, onlyActiveUserRoles);
        //}

       
        #endregion

        #region Addresses

        public static void RemoveAddress(this User user, Address address)
        {
            if (user.Addresses.Contains(address))
            {
                if (user.BillingAddress == address) user.BillingAddress = null;
                user.Addresses.Remove(address);
            }
        }

        #endregion
    }
}
