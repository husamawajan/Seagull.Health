using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.UserEntitys;
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
using Seagull.Core.Infrastructure;
using Seagull.Core.Domain.Users;

namespace Seagull.Services.UserEntitys
{
    /// <summary>
    ///  UserEntity service
    /// </summary>
    public partial class UserEntityService : IUserEntityService
    {

         #region Fields

        private readonly IRepository<UserEntity> _userentityRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userentityRepository">UserEntity repository</param>
        /// <param name="eventPublisher">Event published</param>
        public UserEntityService(IRepository<UserEntity> userentityRepository,
            IEventPublisher eventPublisher)
        {
            _userentityRepository = userentityRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an userentity by userentity identifier
        /// </summary>
        /// <param name="userentityId">UserEntity identifier</param>
        /// <returns>UserEntity</returns>
        public virtual UserEntity GetUserEntityById(int userentityId)
        {
            if (userentityId == 0)
                return null;

            return _userentityRepository.GetById(userentityId);
        }

        public virtual UserEntity GetUserEntityByName(string EntityName)
        {
            if (string.IsNullOrEmpty(EntityName))
                return null;

            UserEntity data = _userentityRepository.Table.Where(a => a.Name.Contains(EntityName.Trim())).FirstOrDefault();
            return data;
        }

       
        /// <summary>
        /// Marks userentity as deleted 
        /// </summary>
        /// <param name="userentity">UserEntity</param>
        public virtual void DeleteUserEntity(UserEntity userentity)
        {
            if (userentity == null)
                throw new ArgumentNullException("userentity");
            _userentityRepository.Delete(userentity);
        }

        /// <summary>
        /// Gets all userentitys
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>UserEntity collection</returns>
        public virtual IList<UserEntity> GetAllUserEntitys()
        {
            var query = from a in _userentityRepository.Table
                        select a;
            var userentitys = query.ToList();

            return userentitys;
        }

        public virtual IList<UserEntity> GetAllUserEntitysByUserTypeId(int UserTypeId)
        {
            var query = from a in _userentityRepository.Table.Where(a=>a.UserTypeId == UserTypeId)
                        select a;
            var userentitys = query.ToList();

            return userentitys;
        }
        /// <summary>
        /// Gets all userentitys
        /// </summary>
        /// <returns>UserEntity collection</returns>
        public virtual IPagedList<UserEntity> GetAllUserEntitys(string description, int pageIndex, int pageSize)
        {
            var query = _userentityRepository.Table;

            if (!String.IsNullOrWhiteSpace(description))
                query = query.Where(c => c.Name.ToLower().Contains(description.ToLower()));

            query = query.OrderBy(b => b.Id);

            var userentitys = new PagedList<UserEntity>(query, pageIndex, pageSize);
            return userentitys;
        }


        /// <summary>
        /// Inserts an userentity
        /// </summary>
        /// <param name="userentity">UserEntity</param>
        public virtual void InsertUserEntity(UserEntity userentity)
        {
            if (userentity == null)
                throw new ArgumentNullException("userentity");

            _userentityRepository.Insert(userentity);

            //event notification
            _eventPublisher.EntityInserted(userentity);
        }

        /// <summary>
        /// Updates the userentity
        /// </summary>
        /// <param name="userentity">UserEntity</param>
        public virtual void UpdateUserEntity(UserEntity userentity)
        {
            if (userentity == null)
                throw new ArgumentNullException("userentity");

            _userentityRepository.Update(userentity);

            //event notification
            _eventPublisher.EntityUpdated(userentity);
        }
		/// <summary>
        /// Gets all userentitys
        /// </summary>
        /// <returns>UserEntity PagedList</returns>
		public virtual PagedList<UserEntity> GetAllUserEntitys(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<UserEntity> query =  _userentityRepository.Table.AsQueryable();
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
					    query = query.Where<UserEntity>(
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
                                    var tempQuery = _userentityRepository.Table;
                                    query = count == 0 ? query.Where<UserEntity>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<UserEntity>(tempQuery.Where<UserEntity>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
                        {
                            string strFk = _search.Value.ToString();
                            switch ((string)_search.Key)
                            {
                                case "StrUserTypeId":
                                    query = query.Where(a => a.FK_UserType.Type.Contains(strFk));
                                    break;
                                
                                default:
                                    query = query.Where<UserEntity>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }
						
                    }
                }
            }
            if (showHidden)
            {
                query = query.Where(a => a.UserTypeId == 1);
                bool isAdmin = EngineContext.Current.Resolve<IWorkContext>().CurrentUser.UserRoles.Where(c => c.SystemName == SystemUserRoleNames.Administrators).Count() > 0;
                if (!isAdmin)
                    query = query.Where(a => a.Id == EngineContext.Current.Resolve<IWorkContext>().CurrentUser.EntityUserId);
            }
            query = query.OrderBy<UserEntity>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<UserEntity>(query, pagination.start / 10 , pagination.Count == 0 ? 10 : pagination.Count);
        }

        #endregion
    }
}
