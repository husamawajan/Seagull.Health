using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.UserTypes;
using Seagull.Services.Events;
using Seagull.Services.Common;
using Seagull.Data;
using Seagull.Core.Domain.Common;
using Seagull.Services.Helpers;
using Newtonsoft.Json.Linq;
using Seagull.Helpers.WhereOperation;
using CodeBureau;
using System.Web.Helpers;
using ExtensionMethods;
using Newtonsoft.Json;
using Seagull.Core.Domain.Users;

namespace Seagull.Services.UserTypes
{
    /// <summary>
    ///  UserType service
    /// </summary>
    public partial class UserTypeService : IUserTypeService
    {

         #region Fields

        private readonly IRepository<UserType> _usertypeRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWorkContext _iWorkContext;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="usertypeRepository">UserType repository</param>
        /// <param name="eventPublisher">Event published</param>
        public UserTypeService(IRepository<UserType> usertypeRepository,
            IEventPublisher eventPublisher,
            IWorkContext iWorkContext)
        {
            _usertypeRepository = usertypeRepository;
            _eventPublisher = eventPublisher;
            _iWorkContext = iWorkContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an usertype by usertype identifier
        /// </summary>
        /// <param name="usertypeId">UserType identifier</param>
        /// <returns>UserType</returns>
        public virtual UserType GetUserTypeById(decimal usertypeId)
        {
            if (usertypeId == 0)
                return null;

            return _usertypeRepository.GetById(usertypeId);
        }

        /// <summary>
        /// Marks usertype as deleted 
        /// </summary>
        /// <param name="usertype">UserType</param>
        public virtual void DeleteUserType(UserType usertype)
        {
            if (usertype == null)
                throw new ArgumentNullException("usertype");
            _usertypeRepository.Delete(usertype);
        }

        /// <summary>
        /// Gets all usertypes
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>UserType collection</returns>
        public virtual IList<UserType> GetAllUserTypes()
        {
            var query = from a in _usertypeRepository.Table
                        select a;
            var usertypes = query.ToList();

            return usertypes;
        }

        /// <summary>
        /// Gets all usertypes
        /// </summary>
        /// <returns>UserType collection</returns>
        public virtual IPagedList<UserType> GetAllUserTypes(string description, int pageIndex, int pageSize)
        {
            var query = _usertypeRepository.Table;

            if (!String.IsNullOrWhiteSpace(description))
                query = query.Where(c => c.Type.ToLower().Contains(description.ToLower()));

            query = query.OrderBy(b => b.Id);

            var usertypes = new PagedList<UserType>(query, pageIndex, pageSize);
            return usertypes;
        }


        /// <summary>
        /// Inserts an usertype
        /// </summary>
        /// <param name="usertype">UserType</param>
        public virtual void InsertUserType(UserType usertype)
        {
            if (usertype == null)
                throw new ArgumentNullException("usertype");

            _usertypeRepository.Insert(usertype);

            //event notification
            _eventPublisher.EntityInserted(usertype);
        }

        /// <summary>
        /// Updates the usertype
        /// </summary>
        /// <param name="usertype">UserType</param>
        public virtual void UpdateUserType(UserType usertype)
        {
            if (usertype == null)
                throw new ArgumentNullException("usertype");

            _usertypeRepository.Update(usertype);

            //event notification
            _eventPublisher.EntityUpdated(usertype);
        }
		/// <summary>
        /// Gets all usertypes
        /// </summary>
        /// <returns>UserType PagedList</returns>
		public virtual PagedList<UserType> GetAllUserTypes(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<UserType> query =  _usertypeRepository.Table.Where(a => showHidden).AsQueryable();
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
					    query = query.Where<UserType>(
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
                                    var tempQuery = _usertypeRepository.Table;
                                    query = count == 0 ? query.Where<UserType>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<UserType>(tempQuery.Where<UserType>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
						query = query.Where<UserType>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            query = query.OrderBy<UserType>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<UserType>(query, pagination.start / 10 , pagination.Count == 0 ? 10 : pagination.Count);
        }
        public virtual PagedList<UserType> GetAllUserTypesProject(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<UserType> query = _usertypeRepository.Table.Where(a => showHidden).AsQueryable();
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<UserType>(
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
                                    var tempQuery = _usertypeRepository.Table;
                                    query = count == 0 ? query.Where<UserType>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<UserType>(tempQuery.Where<UserType>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
                            query = query.Where<UserType>(
                                        (object)_search.Key, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            bool isAdmin = _iWorkContext.CurrentUser.UserRoles.Where(c => c.SystemName == SystemUserRoleNames.Administrators).Count() > 0;
            if (!isAdmin)
                query = query.Where(a => a.Id == _iWorkContext.CurrentUser.UserTypeId);
            query = query.OrderBy<UserType>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<UserType>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }
        #endregion
    }
}
	
