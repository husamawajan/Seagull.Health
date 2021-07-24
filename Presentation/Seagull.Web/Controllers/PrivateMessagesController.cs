using System;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain.Users;
using Seagull.Services.Users;
using Seagull.Services.Localization;
using Seagull.Services.Logging;
using Seagull.Web.Factories;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Security;
using Seagull.Web.Models.PrivateMessages;

namespace Seagull.Web.Controllers
{
    [SeagullHttpsRequirement(SslRequirement.Yes)]
    public partial class PrivateMessagesController : BasePublicController
    {
        #region Fields

        private readonly IPrivateMessagesModelFactory _privateMessagesModelFactory;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Constructors

        public PrivateMessagesController(IPrivateMessagesModelFactory privateMessagesModelFactory,
            IUserService userService,
            IUserActivityService userActivityService,
            ILocalizationService localizationService,
            IWorkContext workContext, 
            IStoreContext storeContext)
        {
            this._privateMessagesModelFactory = privateMessagesModelFactory;
            this._userService = userService;
            this._userActivityService = userActivityService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._storeContext = storeContext;
        }

        #endregion
        
        #region Methods

        public virtual ActionResult Index(int? page, string tab)
        {
            //if (_workContext.CurrentUser.IsGuest())
            //{
            //    return new HttpUnauthorizedResult();
            //}

            var model = _privateMessagesModelFactory.PreparePrivateMessageIndexModel(page, tab);
            return View(model);
        }

        //inbox tab
        [ChildActionOnly]
        public virtual ActionResult Inbox(int page, string tab)
        {
            //var model = _privateMessagesModelFactory.PrepareInboxModel(page, tab);
            return PartialView();
        }

        //sent items tab
        [ChildActionOnly]
        public virtual ActionResult SentItems(int page, string tab)
        {
            //var model = _privateMessagesModelFactory.PrepareSentModel(page, tab);
            return PartialView();
        }

        [HttpPost, FormValueRequired("delete-inbox"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult DeleteInboxPM(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        [HttpPost, FormValueRequired("mark-unread"), ActionName("InboxUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult MarkUnread(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                }
            }
            return RedirectToRoute("PrivateMessages");
        }

        //updates sent items (deletes PrivateMessages)
        [HttpPost, FormValueRequired("delete-sent"), ActionName("SentUpdate")]
        [PublicAntiForgery]
        public virtual ActionResult DeleteSentPM(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("si", "").Trim();
                }

            }
            return RedirectToRoute("PrivateMessages", new {tab = "sent"});
        }



        #endregion
    }
}
