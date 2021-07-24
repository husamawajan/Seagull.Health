using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Core.Domain.Functionss;
using Seagull.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace Seagull.Services.Functionss
{
    /// <summary>
    /// Functions Service interface
    /// </summary>
	public partial interface IFunctionsService
	{
		/// <summary>
        /// Deletes a functions
        /// </summary>
        /// <param name="functions">Functions</param>
        void DeleteFunctions(Functions functions);

        /// <summary>
        /// Gets all functionss
        /// </summary>
        /// <returns>Functions collection</returns>
        IPagedList<Functions> GetAllFunctionss(string description, int pageIndex, int pageSize);
        IList<Functions> GetAllFunctionss();
        /// <summary>
        /// Gets a functions 
        /// </summary>
        /// <param name="functionsId">Functions identifier</param>
        /// <returns>Functions</returns>
        Functions GetFunctionsById(decimal functionsId);

        /// <summary>
        /// Inserts a functions
        /// </summary>
        /// <param name="functions">Functions</param>
        void InsertFunctions(Functions functions);

        /// <summary>
        /// Updates the functions
        /// </summary>
        /// <param name="functions">Functions</param>
        void UpdateFunctions(Functions functions);

		/// <summary>
        /// Gets all functionss
        /// </summary>
        /// <returns>Functions PagedList</returns>
		PagedList<Functions> GetAllFunctionss(pagination pagination, sort sort, string search, string search_operator, string filter, bool showHidden = false);
	}
}
	
 
 