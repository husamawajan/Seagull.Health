using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Core.Domain.Components;
using Seagull.Services.Helpers;


namespace Seagull.Services.Components
{
    /// <summary>
    /// Component Service interface
    /// </summary>
	public partial interface IComponentService
	{
		/// <summary>
        /// Deletes a component
        /// </summary>
        /// <param name="component">Component</param>
        void DeleteComponent(Component component);

        /// <summary>
        /// Gets all components
        /// </summary>
        /// <returns>Component collection</returns>
        IPagedList<Component> GetAllComponents(string description, int pageIndex, int pageSize);
        IList<Component> GetAllComponents();
        /// <summary>
        /// Gets a component 
        /// </summary>
        /// <param name="componentId">Component identifier</param>
        /// <returns>Component</returns>
        Component GetComponentById(decimal componentId);

        /// <summary>
        /// Inserts a component
        /// </summary>
        /// <param name="component">Component</param>
        void InsertComponent(Component component);

        /// <summary>
        /// Updates the component
        /// </summary>
        /// <param name="component">Component</param>
        void UpdateComponent(Component component);

		/// <summary>
        /// Gets all components
        /// </summary>
        /// <returns>Component PagedList</returns>
        PagedList<Component> GetAllComponents(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false);
	}
}
 
