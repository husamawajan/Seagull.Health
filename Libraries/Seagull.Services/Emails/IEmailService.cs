using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagull.Core;
using Seagull.Core.Domain.Emails;
using Seagull.Services.Helpers;
using Newtonsoft.Json.Linq;

namespace Seagull.Services.Emails
{
    /// <summary>                                                                                                                                                                                                             
    /// Email Service interface                                                                                                                                                                                  
    /// </summary>                                                                                                                                                                                                            
    public partial interface IEmailService
    {
        /// <summary>                                                                                                                                                                                                         
        /// Deletes a Email                                                                                                                                                                                      
        /// </summary>                                                                                                                                                                                                        
        /// <param name="Email">Email</param>                                                                                                                                                       
        void DeleteEmail(Email Email);

        /// <summary>                                                                                                                                                                                                         
        /// Gets all Emails                                                                                                                                                                                      
        /// </summary>                                                                                                                                                                                                        
        /// <returns>Email collection</returns>                                                                                                                                                                  
        IPagedList<Email> GetAllEmails(string description, int pageIndex, int pageSize);
        IList<Email> GetAllEmails();
        /// <summary>                                                                                                                                                                                                         
        /// Gets a Email                                                                                                                                                                                         
        /// </summary>                                                                                                                                                                                                        
        /// <param name="EmailId">Email identifier</param>                                                                                                                                          
        /// <returns>Email</returns>                                                                                                                                                                             
        Email GetEmailById(int EmailId);

        /// <summary>                                                                                                                                                                                                         
        /// Inserts a Email                                                                                                                                                                                      
        /// </summary>                                                                                                                                                                                                        
        /// <param name="Email">Email</param>                                                                                                                                                       
        void InsertEmail(Email Email);

        /// <summary>                                                                                                                                                                                                         
        /// Updates the Email                                                                                                                                                                                    
        /// </summary>                                                                                                                                                                                                        
        /// <param name="Email">Email</param>                                                                                                                                                       
        void UpdateEmail(Email Email);

        /// <summary>                                                                                                                                                                                                         
        /// Gets all Emails                                                                                                                                                                                      
        /// </summary>                                                                                                                                                                                                        
        /// <returns>Email PagedList</returns>                                                                                                                                                                   
        IPagedList<Email> GetAllEmails(pagination pagination, sort sort, string search, string search_operator, string filter, int id = 0, bool showHidden = false);

    }
}