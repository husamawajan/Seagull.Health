using System;
using System.Collections.Generic;
using System.Linq;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.Emails;
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
using Seagull.Services.Security;

namespace Seagull.Services.Emails
{
    /// <summary>                                                                                                                                                                                                                                  
    ///  Email service                                                                                                                                                                                                                
    /// </summary>                                                                                                                                                                                                                                 
    public partial class EmailService : IEmailService
    {

        #region Fields                                                                                                                                                                                                                             

        private readonly IRepository<Email> _EmailRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPermissionService _permissionService;
        #endregion

        #region Ctor                                                                                                                                                                                                                               

        /// <summary>                                                                                                                                                                                                                              
        /// Ctor                                                                                                                                                                                                                                   
        /// </summary>                                                                                                                                                                                                                             
        /// <param name="EmailRepository" > Email repository</param>                                                                                                                                                       
        /// <param name="eventPublisher">Event published</param>        
        /// 
        public EmailService()
        {

        }
        public EmailService(IRepository<Email> EmailRepository,
            IEventPublisher eventPublisher,
            IPermissionService permissionService)
        {
             System.Diagnostics.Debugger.Launch();
            _EmailRepository = EmailRepository;
            _eventPublisher = eventPublisher;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods                                                                                                                                                                                                                            

        /// <summary>                                                                                                                                                                                                                              
        /// Gets an Email by Email identifier                                                                                                                                                                            
        /// </summary>                                                                                                                                                                                                                             
        /// <param name="EmailId">Email identifier</param>                                                                                                                                                               
        /// <returns>Email</returns>                                                                                                                                                                                                  
        public virtual Email GetEmailById(int EmailId)
        {
            if (EmailId == 0)
                return null;

            return _EmailRepository.GetById(EmailId);
        }
        /// <summary>                                                                                                                                                                                                                              
        /// Marks Email as deleted                                                                                                                                                                                                    
        /// </summary>                                                                                                                                                                                                                             
        /// <param name="Email">Email</param>                                                                                                                                                                            
        public virtual void DeleteEmail(Email Email)
        {
            if (Email == null)
                throw new ArgumentNullException("Email");
            _EmailRepository.Delete(Email);
        }

        /// <summary>                                                                                                                                                                                                                              
        /// Gets all Emails                                                                                                                                                                                                           
        /// </summary>                                                                                                                                                                                                                             
        /// <param name="showHidden">A value indicating whether to show hidden records</param>                                                                                                                                                     
        /// <returns>Email collection</returns>                                                                                                                                                                                       
        public virtual IList<Email> GetAllEmails()
        {
            var query = from a in _EmailRepository.Table
                        select a;
            var Emails = query.ToList();

            return Emails;
        }

        /// <summary>                                                                                                                                                                                                                              
        /// Gets all Emails                                                                                                                                                                                                           
        /// </summary>                                                                                                                                                                                                                             
        /// <returns>Email collection</returns>                                                                                                                                                                                       
        public virtual IPagedList<Email> GetAllEmails(string description, int pageIndex, int pageSize)
        {
            var query = _EmailRepository.Table;

            if (!String.IsNullOrWhiteSpace(description))
                // query = query.Where(c => c.Name.ToLower().Contains(description.ToLower()));                                                                                                                                                 

                query = query.OrderBy(b => b.Id);

            var Emails = new PagedList<Email>(query, pageIndex, pageSize);
            return Emails;
        }


        /// <summary>                                                                                                                                                                                                                              
        /// Inserts an Email                                                                                                                                                                                                          
        /// </summary>                                                                                                                                                                                                                             
        /// <param name="Email">Email</param>                                                                                                                                                                            
        public virtual void InsertEmail(Email Email)
        {
            if (Email == null)
                throw new ArgumentNullException("Email");

            _EmailRepository.Insert(Email);

            //event notification                                                                                                                                                                                                                   
            _eventPublisher.EntityInserted(Email);
        }

        /// <summary>                                                                                                                                                                                                                              
        /// Updates the Email                                                                                                                                                                                                         
        /// </summary>                                                                                                                                                                                                                             
        /// <param name="Email">Email</param>                                                                                                                                                                            
        public virtual void UpdateEmail(Email Email)
        {
            if (Email == null)
                throw new ArgumentNullException("Email");

            _EmailRepository.Update(Email);

            //event notification                                                                                                                                                                                                                   
            _eventPublisher.EntityUpdated(Email);
        }
        /// <summary>                                                                                                                                                                                                                              
        /// Gets all Emails                                                                                                                                                                                                           
        /// </summary>                                                                                                                                                                                                                             
        /// <returns>Email PagedList</returns>                                                                                                                                                                                        
        public virtual IPagedList<Email> GetAllEmails(pagination pagination, sort sort, string search, string search_operator, string filter, int id = 0, bool showHidden = false)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? JObject.Parse("") : JObject.Parse(search_operator);
            IQueryable<Email> query = _EmailRepository.Table.AsQueryable();
            //int? currentEntityUserId = EngineContext.Current.Resolve<IWorkContext>().CurrentUser.EntityUserId;                                                                                                                                     
            bool isAdmin = false;//_permissionService.Authorize(StandardPermissionProvider.Admin, EngineContext.Current.Resolve<IWorkContext>().CurrentUser);                                                                                       
                                 //if (!isAdmin)                                                                                                                                                                                                                               
                                 //   query = query.Where(a => a.ProjectOwner == currentEntityUserId);                                                                                                                                                                       

            List<Email> EmailList = new List<Email>();
            if (!string.IsNullOrEmpty(filter) && filter.Length > 2)
            {
                searchFilter = Json.Decode(filter);
                foreach (var _filter in searchFilter)
                {
                    string fitlterstr = (string)_filter.Value;
                    if (fitlterstr.Contains(","))
                    {
                        fitlterstr.Split(',').Skip(1).ToList().ForEach(s =>
                        {
                            switch ((string)_filter.Key)
                            {
                                case "":
                                    break;
                            }
                        });

                        query = EmailList.AsQueryable();
                    }
                    else if (!string.IsNullOrEmpty(_filter.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _filter.Key).FirstOrDefault().Value : "eq";
                        query = query.Where<Email>(
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
                    if (!string.IsNullOrEmpty(_search.Value))
                    {
                        string op = operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).Count() > 0 ? (string)operater.Descendants().OfType<JProperty>().Where(p => p.Name == _search.Key).FirstOrDefault().Value : "eq";
                        string checkCurrentKey = Convert.ToString(_search.Value);
                        if (checkCurrentKey.Split(',').Count() > 1)
                        {
                            int i;
                            if (!(int.TryParse(checkCurrentKey.Split(',')[0].ToString(), out i)))
                                query = query.Where<Email>(
                                        (object)_search.Key, (object)_search.Value,
                                        (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                            else
                            {
                                int count = 0;
                                foreach (var _tempSearchKey in checkCurrentKey.Split(',').ToList())
                                {
                                    if (!string.IsNullOrEmpty(_tempSearchKey))
                                    {
                                        var tempQuery = _EmailRepository.Table;
                                        query = count == 0 ? query.Where<Email>(
                                            (object)_search.Key, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)) :
                                            query.Concat<Email>(tempQuery.Where<Email>(
                                            (object)_search.Key, (object)_tempSearchKey,
                                            (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op)));
                                        count = count + 1;
                                    }
                                }
                            }

                        }
                        else
                        {
                            string strFk = (string)_search.Value;
                            switch ((string)_search.Key)
                            {
                                case "":
                                    break;
                                default:
                                    query = query.Where<Email>(
                                    (object)_search.Key, (object)_search.Value,
                                    (WhereOperation)StringEnum.Parse(typeof(WhereOperation), op));
                                    break;
                            }
                        }
                    }
                }
            }
            query = query.OrderBy<Email>("Id", !sort.reverse ? "asc" : "");
            return new PagedList<Email>(query, pagination.start / 10, pagination.Count == 0 ? 10 : pagination.Count);
        }
        #endregion
    }
}