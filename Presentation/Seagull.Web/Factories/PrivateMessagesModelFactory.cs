using System;
using System.Collections.Generic;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Users;
using Seagull.Services.Helpers;
using Seagull.Web.Models.Common;
using Seagull.Web.Models.PrivateMessages;

namespace Seagull.Web.Factories
{
    /// <summary>
    /// Represents the private message model factory
    /// </summary>
    public partial class PrivateMessagesModelFactory : IPrivateMessagesModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly UserSettings _userSettings;

        #endregion

        #region Constructors

        public PrivateMessagesModelFactory(
            IWorkContext workContext, 
            IStoreContext storeContext,
            IDateTimeHelper dateTimeHelper,
            UserSettings userSettings)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._dateTimeHelper = dateTimeHelper;
            this._userSettings = userSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the private message index model
        /// </summary>
        /// <param name="page">Number of items page; pass null to disable paging</param>
        /// <param name="tab">Tab name</param>
        /// <returns>Private message index model</returns>
        public virtual PrivateMessageIndexModel PreparePrivateMessageIndexModel(int? page, string tab)
        {
            int inboxPage = 0;
            int sentItemsPage = 0;
            bool sentItemsTabSelected = false;

            switch (tab)
            {
                case "inbox":
                    if (page.HasValue)
                    {
                        inboxPage = page.Value;
                    }
                    break;
                case "sent":
                    if (page.HasValue)
                    {
                        sentItemsPage = page.Value;
                    }
                    sentItemsTabSelected = true;
                    break;
                default:
                    break;
            }

            var model = new PrivateMessageIndexModel
            {
                InboxPage = inboxPage,
                SentItemsPage = sentItemsPage,
                SentItemsTabSelected = sentItemsTabSelected
            };

            return model;
        }


  

        #endregion
    }
}
