using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;
using Seagull.Admin.Infrastructure.Cache;
using Seagull.Admin.Models.Home;
using Seagull.Core;
using Seagull.Core.Caching;
using Seagull.Core.Domain.Common;
using Seagull.Core.Domain.Users;
using Seagull.Services.Configuration;
using Seagull.Services.Users;
using Seagull.Services.Security;
using Seagull.Core.Data;
using Seagull.Core.Infrastructure;
using System.Collections.Generic;
using Seagull.Services.Localization;
using Seagull.Admin.Helpers;
using Seagull.Core.Domain.UserEntitys;
using Seagull.Data;
using System.Data.SqlClient;
using Seagull.Web.Framework.Controllers;
using static Seagull.Admin.Controllers.ReportController;
using static Seagull.Admin.Helpers.ReportHelpers.ExportHelper;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Office.CustomUI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web.UI;
using iTextSharp.tool.xml.css;
using XMLWorkerTests;
using iTextSharp.tool.xml.pipeline.html;
using System.Text;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using System.Web;
using System.Dynamic;
using System.Threading;
using System.Net.Mail;
using Seagull.Core.Domain.Emails;
using Seagull.Services.Emails;
using System.Data;
using SmppIntegraion;
using Seagull.Admin.Models.Users;

namespace Seagull.Admin.Controllers
{
    [AdminAuthorize]
    public partial class HomeController : BaseAdminController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        //private readonly IRepository<Sector> _sectorRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserEntity> _userEntityRepository;
        private readonly IDbContext _dbContext;
        private readonly IDbContext _IDbContext;
        private readonly IEmailService _emailservice;
        private readonly IRepository<Email> _EmailRepository;
        private bool IsShowOrNot = false;
        #endregion

        #region Ctor

        public HomeController(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            IUserService userService,
            IWorkContext workContext,
            ICacheManager cacheManager,
            ILocalizationService localizationService,
            IRepository<User> userRepository,
            IDbContext IDbContext,
            IRepository<UserEntity> userEntityRepository,
            IDbContext dbContext,
            IEmailService emailservice,
            IRepository<Email> EmailRepository
            )
        {
            this._storeContext = storeContext;
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._userService = userService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            _localizationService = localizationService;
            _userRepository = userRepository;
            _userEntityRepository = userEntityRepository;
            _dbContext = dbContext;
            _IDbContext = IDbContext;
            this._emailservice = emailservice;
            this._EmailRepository = EmailRepository;
        }

        #endregion

        #region Methods

        //when entity loginIn he will redirect to Entity Dashboard then if he want to go to the main dashboard the parameter  (TransfareFromEntity) must be true
        public virtual ActionResult Index(bool TransfareFromEntity = false)
        {
            DashboardModel model = new DashboardModel();

            return View(model);
        }
        #endregion
    }
}