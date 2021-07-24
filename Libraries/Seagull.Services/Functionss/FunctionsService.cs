using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.Functionss;
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

namespace Seagull.Services.Functionss
{
    /// <summary>
    ///  Functions service
    /// </summary>
    public partial class FunctionsService : IFunctionsService
    {

         #region Fields

        private readonly IRepository<Functions> _functionsRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="functionsRepository">Functions repository</param>
        /// <param name="eventPublisher">Event published</param>
        public FunctionsService(IRepository<Functions> functionsRepository,
            IEventPublisher eventPublisher)
        {
            _functionsRepository = functionsRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an functions by functions identifier
        /// </summary>
        /// <param name="functionsId">Functions identifier</param>
        /// <returns>Functions</returns>
        public virtual Functions GetFunctionsById(decimal functionsId)
        {
            if (functionsId == 0)
                return null;

            return _functionsRepository.GetById(functionsId);
        }

        /// <summary>
        /// Marks functions as deleted 
        /// </summary>
        /// <param name="functions">Functions</param>
        public virtual void DeleteFunctions(Functions functions)
        {
            if (functions == null)
                throw new ArgumentNullException("functions");
            _functionsRepository.Delete(functions);
        }

        /// <summary>
        /// Gets all functionss
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Functions collection</returns>
        public virtual IList<Functions> GetAllFunctionss()
        {
            var query = from a in _functionsRepository.Table
                        select a;
            var functionss = query.ToList();

            return functionss;
        }

        /// <summary>
        /// Gets all functionss
        /// </summary>
        /// <returns>Functions collection</returns>
        public virtual IPagedList<Functions> GetAllFunctionss(string description, int pageIndex, int pageSize)
        {
            var query = _functionsRepository.Table;

            if (!String.IsNullOrWhiteSpace(description))
                query = query.Where(c => c.Name.ToLower().Contains(description.ToLower()));

            query = query.OrderBy(b => b.Id);

            var functionss = new PagedList<Functions>(query, pageIndex, pageSize);
            return functionss;
        }


        /// <summary>
        /// Inserts an functions
        /// </summary>
        /// <param name="functions">Functions</param>
        public virtual void InsertFunctions(Functions functions)
        {
            if (functions == null)
                throw new ArgumentNullException("functions");

            _functionsRepository.Insert(functions);

            //event notification
            _eventPublisher.EntityInserted(functions);
        }

        /// <summary>
        /// Updates the functions
        /// </summary>
        /// <param name="functions">Functions</param>
        public virtual void UpdateFunctions(Functions functions)
        {
            if (functions == null)
                throw new ArgumentNullException("functions");

            _functionsRepository.Update(functions);

            //event notification
            _eventPublisher.EntityUpdated(functions);
        }
		/// <summary>
        /// Gets all functionss
        /// </summary>
        /// <returns>Functions PagedList</returns>
		public virtual PagedList<Functions> GetAllFunctionss(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<Functions> query =  _functionsRepository.Table.Where(a => showHidden).AsQueryable();
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!string.IsNullOrEmpty(_filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
					    query = query.Where<Functions>(
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
                                    var tempQuery = _functionsRepository.Table;
                                    query = count == 0 ? query.Where<Functions>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                        query.Concat<Functions>(tempQuery.Where<Functions>(
                                        (object)_search.Key, (object)_tempSearchKey,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                    count = count + 1;
                                }
                            }
                        }
                        else
						query = query.Where<Functions>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            query = query.OrderBy<Functions>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<Functions>(query, pagination.start / 10 , pagination.Count == 0 ? 10 : pagination.Count);
        }

        #endregion
    }
}
	
