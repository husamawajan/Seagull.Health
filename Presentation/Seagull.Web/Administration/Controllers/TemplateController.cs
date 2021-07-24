using System;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Extensions;
using Seagull.Core.Domain.Topics;
using Seagull.Services.Security;
using Seagull.Services.Topics;
using Seagull.Web.Framework.Kendoui;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Controllers
{
    public partial class TemplateController : BaseAdminController
    {
        #region Fields
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Constructors

        public TemplateController(
            ITopicTemplateService topicTemplateService,
            IPermissionService permissionService)
        {
            this._topicTemplateService = topicTemplateService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Category templates

        public virtual ActionResult CategoryTemplates()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            return View();
        }


        #endregion
       
    }
}
