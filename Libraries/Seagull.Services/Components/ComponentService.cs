using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.Components;
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

namespace Seagull.Services.Components
{
    /// <summary>
    ///  Component service
    /// </summary>
    public partial class ComponentService : IComponentService
    {

         #region Fields

        private readonly IRepository<Component> _componentRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="componentRepository">Component repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ComponentService(IRepository<Component> componentRepository,
            IEventPublisher eventPublisher)
        {
            _componentRepository = componentRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an component by component identifier
        /// </summary>
        /// <param name="componentId">Component identifier</param>
        /// <returns>Component</returns>
        public virtual Component GetComponentById(decimal componentId)
        {
            if (componentId == 0)
                return null;

            return _componentRepository.GetById(componentId);
        }

        /// <summary>
        /// Marks component as deleted 
        /// </summary>
        /// <param name="component">Component</param>
        public virtual void DeleteComponent(Component component)
        {
            if (component == null)
                throw new ArgumentNullException("component");
            _componentRepository.Delete(component);
        }

        /// <summary>
        /// Gets all components
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Component collection</returns>
        public virtual IList<Component> GetAllComponents()
        {
            var query = from a in _componentRepository.Table
                        select a;
            var components = query.ToList();

            return components;
        }

        /// <summary>
        /// Gets all components
        /// </summary>
        /// <returns>Component collection</returns>
        public virtual IPagedList<Component> GetAllComponents(string description, int pageIndex, int pageSize)
        {
            var query = _componentRepository.Table;

            if (!String.IsNullOrWhiteSpace(description))
                query = query.Where(c => c.Name.ToLower().Contains(description.ToLower()));

            query = query.OrderBy(b => b.Id);

            var components = new PagedList<Component>(query, pageIndex, pageSize);
            return components;
        }


        /// <summary>
        /// Inserts an component
        /// </summary>
        /// <param name="component">Component</param>
        public virtual void InsertComponent(Component component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            _componentRepository.Insert(component);

            //event notification
            _eventPublisher.EntityInserted(component);
        }

        /// <summary>
        /// Updates the component
        /// </summary>
        /// <param name="component">Component</param>
        public virtual void UpdateComponent(Component component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            _componentRepository.Update(component);

            //event notification
            _eventPublisher.EntityUpdated(component);
        }
		/// <summary>
        /// Gets all components
        /// </summary>
        /// <returns>Component PagedList</returns>
        public virtual PagedList<Component> GetAllComponents(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<Component> query =  _componentRepository.Table.Where(a => showHidden).AsQueryable();
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    if (!Object.ReferenceEquals(null, _filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<Component>(
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
                        query = query.Where<Component>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                    }
                }
            }
            query = query.OrderBy<Component>(sort.predicate, !sort.reverse ? "asc" : "");
            return new PagedList<Component>(query, pagination.start / 10 , pagination.Count == 0 ? 10 : pagination.Count);
        }

        #endregion
    }
}
