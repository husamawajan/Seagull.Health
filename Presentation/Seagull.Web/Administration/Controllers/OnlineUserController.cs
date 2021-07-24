using System;
using System.Linq;
using System.Web.Mvc;
using Seagull.Admin.Models.Users;
using Seagull.Core.Domain.Users;
using Seagull.Services.Common;
using Seagull.Services.Users;
using Seagull.Services.Directory;
using Seagull.Services.Helpers;
using Seagull.Services.Localization;
using Seagull.Services.Security;
using Seagull.Web.Framework.Kendoui;

namespace Seagull.Admin.Controllers
{
    public partial class OnlineUserController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly UserSettings _userSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public OnlineUserController(IUserService userService,
            IGeoLookupService geoLookupService, IDateTimeHelper dateTimeHelper,
            UserSettings userSettings,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            this._userService = userService;
            this._geoLookupService = geoLookupService;
            this._dateTimeHelper = dateTimeHelper;
            this._userSettings = userSettings;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
        }

        #endregion
        
        #region Methods

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            var users = _userService.GetOnlineUsers(DateTime.UtcNow.AddMinutes(-_userSettings.OnlineUserMinutes),
                null, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = users.Select(x => new OnlineUserModel
                {
                    Id = x.Id,
                    UserInfo = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Users.Guest"),
                    LastIpAddress = x.LastIpAddress,
                    Location = _geoLookupService.LookupCountryName(x.LastIpAddress),
                    LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                    LastVisitedPage = _userSettings.StoreLastVisitedPage ?
                        x.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage) :
                        _localizationService.GetResource("Admin.Users.OnlineUsers.Fields.LastVisitedPage.Disabled")
                }),
                Total = users.TotalCount
            };

            return Json(gridModel);
        }

        #endregion
    }
}
