using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Caching;
using Seagull.Core.Data;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Data;
using Seagull.Services.Common;
using Seagull.Services.Events;
using Seagull.Services.Helpers;
using Newtonsoft.Json;
using CodeBureau;
using Seagull.Helpers.WhereOperation;
using ExtensionMethods;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using Seagull.Data.Mapping.Users;
using Seagull.Core.Infrastructure;
using Seagull.Services.Security;
using Seagull.Services.Localization;
using Seagull.Services.UserEntitys;
namespace Seagull.Services.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial class UserService : IUserService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string USERROLES_ALL_KEY = "Seagull.userrole.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        private const string USERROLES_BY_SYSTEMNAME_KEY = "Seagull.userrole.systemname-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string USERROLES_PATTERN_KEY = "Seagull.userrole.";

        #endregion

        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPassword> _userPasswordRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly UserSettings _userSettings;
        private readonly CommonSettings _commonSettings;

        #endregion

        #region Ctor

        public UserService(ICacheManager cacheManager,
            IRepository<User> userRepository,
            IRepository<UserPassword> userPasswordRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<GenericAttribute> gaRepository,
            IGenericAttributeService genericAttributeService,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            UserSettings userSettings,
            CommonSettings commonSettings)
        {
            this._cacheManager = cacheManager;
            this._userRepository = userRepository;
            this._userPasswordRepository = userPasswordRepository;
            this._userRoleRepository = userRoleRepository;
            this._gaRepository = gaRepository;
            this._genericAttributeService = genericAttributeService;
            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
            this._eventPublisher = eventPublisher;
            this._userSettings = userSettings;
            this._commonSettings = commonSettings;
        }

        #endregion

        #region Methods

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
        public virtual IPagedList<User> GetAllUsers(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int affiliateId = 0, int vendorId = 0,
            int[] userRoleIds = null, string email = null, string username = null,
            string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
             string phone = null, string zipPostalCode = null,
            string ipAddress = null, bool loadOnlyWithShoppingCart = false, int? sct = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _userRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            query = query.Where(c => !c.Deleted);
            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Intersect(userRoleIds).Any());
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            if (!String.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.FirstName &&
                        z.Attribute.Value.Contains(firstName)))
                    .Select(z => z.User);
            }
            if (!String.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.LastName &&
                        z.Attribute.Value.Contains(lastName)))
                    .Select(z => z.User);
            }
            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                string dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(5, 5) == dateOfBirthStr))
                    .Select(z => z.User);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                string dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);
                //EndsWith is not supported by SQL Server Compact
                //so let's use the following workaround http://social.msdn.microsoft.com/Forums/is/sqlce/thread/0f810be1-2132-4c59-b9ae-8f7013c0cc00

                //we also cannot use Length function in SQL Server Compact (not supported in this context)
                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Substring(8, 2) == dateOfBirthStr))
                    .Select(z => z.User);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                string dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.DateOfBirth &&
                        z.Attribute.Value.Contains(dateOfBirthStr)))
                    .Select(z => z.User);
            }

            //search by phone
            if (!String.IsNullOrWhiteSpace(phone))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.Phone &&
                        z.Attribute.Value.Contains(phone)))
                    .Select(z => z.User);
            }
            //search by zip
            if (!String.IsNullOrWhiteSpace(zipPostalCode))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where((z => z.Attribute.KeyGroup == "User" &&
                        z.Attribute.Key == SystemUserAttributeNames.ZipPostalCode &&
                        z.Attribute.Value.Contains(zipPostalCode)))
                    .Select(z => z.User);
            }

            //search by IpAddress
            if (!String.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            if (loadOnlyWithShoppingCart)
            {
                int? sctId = null;
                if (sct.HasValue)
                    sctId = (int)sct.Value;

            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var users = new PagedList<User>(query, pageIndex, 10000);
            return users;
        }

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        public virtual IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Intersect(userRoleIds).Any());

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (user.IsSystemAccount)
                throw new SeagullException(string.Format("System user account ({0}) could not be deleted", user.SystemName));

            user.Deleted = true;

            if (_userSettings.SuffixDeletedUsers)
            {
                if (!String.IsNullOrEmpty(user.Email))
                    user.Email += "-DELETED";
                if (!String.IsNullOrEmpty(user.Username))
                    user.Username += "-DELETED";
            }

            UpdateUser(user);

            //event notification
            _eventPublisher.EntityDeleted(user);
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>A user</returns>
        public virtual User GetUserById(int userId)
        {
            if (userId == 0)
                return null;

            return _userRepository.GetById(userId);
        }

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>Users</returns>
        public virtual IList<User> GetUsersByIds(int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new List<User>();

            var query = from c in _userRepository.Table
                        where userIds.Contains(c.Id) && !c.Deleted
                        select c;
            var users = query.ToList();
            //sort by passed identifiers
            var sortedUsers = new List<User>();
            foreach (int id in userIds)
            {
                var user = users.Find(x => x.Id == id);
                if (user != null)
                    sortedUsers.Add(user);
            }
            return sortedUsers;
        }

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>A user</returns>
        public virtual User GetUserByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var query = from c in _userRepository.Table
                        where c.UserGuid == userGuid
                        orderby c.Id
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        public virtual User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get user by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        public virtual User GetUserBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        public virtual User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Insert a guest user
        /// </summary>
        /// <returns>User</returns>
        public virtual User InsertGuestUser()
        {
            var user = new User
            {
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            ////add to 'Guests' role
            //var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
            //if (guestRole == null)
            //    throw new SeagullException("'Guests' role could not be loaded");
            //user.UserRoles.Add(guestRole);

            //_userRepository.Insert(user);

            return user;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void InsertUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Insert(user);

            //event notification
            _eventPublisher.EntityInserted(user);
        }

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        public virtual void UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRepository.Update(user);

            //event notification
            _eventPublisher.EntityUpdated(user);
        }

        ///// <summary>
        ///// Delete guest user records
        ///// </summary>
        ///// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        ///// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        ///// <param name="onlyWithoutShoppingCart">A value indicating whether to delete users only without shopping cart</param>
        ///// <returns>Number of deleted users</returns>
        //public virtual int DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart)
        //{
        //    if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
        //    {
        //        //stored procedures are enabled and supported by the database. 
        //        //It's much faster than the LINQ implementation below 

        //        #region Stored procedure

        //        //prepare parameters
        //        var pOnlyWithoutShoppingCart = _dataProvider.GetParameter();
        //        pOnlyWithoutShoppingCart.ParameterName = "OnlyWithoutShoppingCart";
        //        pOnlyWithoutShoppingCart.Value = onlyWithoutShoppingCart;
        //        pOnlyWithoutShoppingCart.DbType = DbType.Boolean;

        //        var pCreatedFromUtc = _dataProvider.GetParameter();
        //        pCreatedFromUtc.ParameterName = "CreatedFromUtc";
        //        pCreatedFromUtc.Value = createdFromUtc.HasValue ? (object)createdFromUtc.Value : DBNull.Value;
        //        pCreatedFromUtc.DbType = DbType.DateTime;

        //        var pCreatedToUtc = _dataProvider.GetParameter();
        //        pCreatedToUtc.ParameterName = "CreatedToUtc";
        //        pCreatedToUtc.Value = createdToUtc.HasValue ? (object)createdToUtc.Value : DBNull.Value;
        //        pCreatedToUtc.DbType = DbType.DateTime;

        //        var pTotalRecordsDeleted = _dataProvider.GetParameter();
        //        pTotalRecordsDeleted.ParameterName = "TotalRecordsDeleted";
        //        pTotalRecordsDeleted.Direction = ParameterDirection.Output;
        //        pTotalRecordsDeleted.DbType = DbType.Int32;

        //        //invoke stored procedure
        //        _dbContext.ExecuteSqlCommand(
        //            "EXEC [DeleteGuests] @OnlyWithoutShoppingCart, @CreatedFromUtc, @CreatedToUtc, @TotalRecordsDeleted OUTPUT",
        //            false, null,
        //            pOnlyWithoutShoppingCart,
        //            pCreatedFromUtc,
        //            pCreatedToUtc,
        //            pTotalRecordsDeleted);

        //        int totalRecordsDeleted = (pTotalRecordsDeleted.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
        //        return totalRecordsDeleted;

        //        #endregion
        //    }
        //    else
        //    {
        //        //stored procedures aren't supported. Use LINQ

        //        #region No stored procedure

        //        var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
        //        if (guestRole == null)
        //            throw new SeagullException("'Guests' role could not be loaded");

        //        var query = _userRepository.Table;
        //        if (createdFromUtc.HasValue)
        //            query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
        //        if (createdToUtc.HasValue)
        //            query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
        //        query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Contains(guestRole.Id));

        //        //don't delete system accounts
        //        query = query.Where(c => !c.IsSystemAccount);

        //        //only distinct users (group by ID)
        //        query = from c in query
        //                group c by c.Id
        //                    into cGroup
        //                    orderby cGroup.Key
        //                    select cGroup.FirstOrDefault();
        //        query = query.OrderBy(c => c.Id);
        //        var users = query.ToList();


        //        int totalRecordsDeleted = 0;
        //        foreach (var c in users)
        //        {
        //            try
        //            {
        //                //delete attributes
        //                var attributes = _genericAttributeService.GetAttributesForEntity(c.Id, "User");
        //                _genericAttributeService.DeleteAttributes(attributes);

        //                //delete from database
        //                _userRepository.Delete(c);
        //                totalRecordsDeleted++;
        //            }
        //            catch (Exception exc)
        //            {
        //                Debug.WriteLine(exc);
        //            }
        //        }
        //        return totalRecordsDeleted;

        //        #endregion
        //    }
        //}


        #endregion

        #region User roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public virtual void DeleteUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            if (userRole.IsSystemRole)
                throw new SeagullException("System role could not be deleted");

            _userRoleRepository.Delete(userRole);

            _cacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(userRole);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleById(int userRoleId)
        {
            if (userRoleId == 0)
                return null;

            return _userRoleRepository.GetById(userRoleId);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(USERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _userRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var userRole = query.FirstOrDefault();
                return userRole;
            });
        }

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User roles</returns>
        public virtual IList<UserRole> GetAllUserRoles(bool showHidden = false)
        {
            string key = string.Format(USERROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _userRoleRepository.Table
                            orderby cr.Name
                            where showHidden || cr.Active
                            select cr;
                var userRoles = query.ToList();
                return userRoles;
            });
        }
        public virtual IList<UserRole> GetAllUserRoleByUserTypeId(int UserTypeId, bool showHidden = false)
        {
            string key = string.Format(USERROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from cr in _userRoleRepository.Table
                            orderby cr.Name
                            where showHidden || cr.Active
                            select cr;
                var t = query.ToList();
                t.Add(_userRoleRepository.Table.Where(c => c.SystemName == SystemUserRoleNames.Registered).FirstOrDefault());
                var userRoles = t;
                return userRoles;
            });
        }
        public virtual PagedList<UserRole> GetAllUserRoles(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<UserRole> query = _userRoleRepository.Table;
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<UserRole>(
                                    (object)_filter.Key, (object)_filter.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = Json.Decode(search);
                foreach (var _search in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _search.Value) && !String.IsNullOrEmpty(_search.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int count = 0;
                            foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                            {
                                if (!string.IsNullOrEmpty(_tempSearchKey))
                                {
                                    var tempQuery = _userRoleRepository.Table;
                                    query = count == 0 ? query.Where<UserRole>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<UserRole>(tempQuery.Where<UserRole>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
                            query = query.Where<UserRole>(
                                        (object)_search.Key, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }

            query = query.Distinct<UserRole>().OrderBy<UserRole>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<UserRole>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }


        public virtual PagedList<User> GetAllUsers(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false, int EntityId = 0, bool withPremission = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<User> query;
            if (EntityId > 0)
                query = _userRepository.Table.Where(x => x.EntityUserId == EntityId);
            else
                query = _userRepository.Table;
            //Check Focal Piont User In Prepare Project
            if (withPremission)
                //query = query.Where(a => a.UserRoles.Where(usr => usr.PermissionRecords.Where(perm => perm.Name == StandardPermissionProvider.AddEntityReport.Name).Count() > 0).Count() > 0);
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))//!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<User>(
                                    (object)_filter.Key, (object)_filter.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = Json.Decode(search);
                foreach (var _search in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _search.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int count = 0;
                            foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                            {
                                if (!string.IsNullOrEmpty(_tempSearchKey))
                                {
                                    var tempQuery = _userRepository.Table;
                                    query = count == 0 ? query.Where<User>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<User>(tempQuery.Where<User>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
                            query = query.Where<User>(
                                        (object)_search.Key, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }

            bool isAdmin = EngineContext.Current.Resolve<IWorkContext>().CurrentUser.UserRoles.Where(c => c.SystemName == SystemUserRoleNames.Administrators).Count() > 0;
            if (!isAdmin)
                query = query.Where(a => a.EntityUserId == EngineContext.Current.Resolve<IWorkContext>().CurrentUser.EntityUserId);
            else
                query = query.Where(a => a.UserTypeId == 1);
            query = query.Distinct<User>().OrderBy<User>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<User>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }

        public virtual PagedList<User> GetAllLiaisonOfficerUsers(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false, int EntityId = 0, bool withPremission = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<User> query = _userRepository.Table.Where(a => a.UserRoles.Where(usr => usr.SystemName == "LiaisonOfficer").Count() > 0);

            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))//!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<User>(
                                    (object)_filter.Key, (object)_filter.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = Json.Decode(search);
                foreach (var _search in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _search.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int count = 0;
                            foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                            {
                                if (!string.IsNullOrEmpty(_tempSearchKey))
                                {
                                    var tempQuery = _userRepository.Table;
                                    query = count == 0 ? query.Where<User>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<User>(tempQuery.Where<User>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
                            query = query.Where<User>(
                                        (object)_search.Key, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            query = query.Distinct<User>().OrderBy<User>("Email", !sort.reverse ? "asc" : "");
            return new PagedList<User>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public virtual void InsertUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            _userRoleRepository.Insert(userRole);

            _cacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(userRole);
        }

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        public virtual void UpdateUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            _userRoleRepository.Update(userRole);

            _cacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(userRole);
        }


        public bool CheckCurrentUserByPermissionName(ICollection<UserRole> userRole, string permName)
        {
            return userRole.Where(s => s.PermissionRecords.Where(p => p.Name.Contains(permName)).Count() > 0).Count() > 0;
        }
        #endregion


        #region User passwords

        /// <summary>
        /// Gets user passwords
        /// </summary>
        /// <param name="userId">User identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>List of user passwords</returns>
        public virtual IList<UserPassword> GetUserPasswords(int? userId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
        {
            var query = _userPasswordRepository.Table;

            //filter by user
            if (userId.HasValue)
                query = query.Where(password => password.UserId == userId.Value);

            //filter by password format
            if (passwordFormat.HasValue)
                query = query.Where(password => password.PasswordFormatId == (int)(passwordFormat.Value));

            //get the latest passwords
            if (passwordsToReturn.HasValue)
                query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

            return query.ToList();
        }

        /// <summary>
        /// Get current user password
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>User password</returns>
        public virtual UserPassword GetCurrentPassword(int userId)
        {
            if (userId == 0)
                return null;

            //return the latest password
            return GetUserPasswords(userId, passwordsToReturn: 1).FirstOrDefault();
        }

        /// <summary>
        /// Insert a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        public virtual void InsertUserPassword(UserPassword userPassword)
        {
            if (userPassword == null)
                throw new ArgumentNullException("userPassword");

            _userPasswordRepository.Insert(userPassword);

            //event notification
            _eventPublisher.EntityInserted(userPassword);
        }

        /// <summary>
        /// Update a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        public virtual void UpdateUserPassword(UserPassword userPassword)
        {
            if (userPassword == null)
                throw new ArgumentNullException("userPassword");

            _userPasswordRepository.Update(userPassword);

            //event notification
            _eventPublisher.EntityUpdated(userPassword);

        }

        #endregion

        #endregion
    }

    public class CustomNotifyMsg
    {
        public string Msg { get; set; }
        public string URL { get; set; }

    }

    
}