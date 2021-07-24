using Seagull.Admin.GridBuild;
using Seagull.Admin.Helpers;
using Seagull.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seagull.Services.Localization;
using static Seagull.Admin.Helpers.ReportHelpers.ExportHelper;
using System.Data;
using System.Web.UI.WebControls;
using Seagull.Core;
using Seagull.Admin.Helpers.ReportHelpers;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp;
using DocumentFormat.OpenXml.Office.CustomUI;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using XMLWorkerTests;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.parser;
using System.Text;
using iTextSharp.text.html.simpleparser;
using static Seagull.Admin.Controllers.ReportController;

namespace Seagull.Admin.Controllers
{
    public class SystemReportController : BaseAdminController
    {
        #region Fields
        private readonly IDbContext _IDbContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Constructors
        public SystemReportController(
            IDbContext IDbContext,
            ILocalizationService localizationService,
            IWorkContext workContext
            )
        {
            this._IDbContext = IDbContext;
            _localizationService = localizationService;
            this._workContext = workContext;
        }
        #endregion

        #region

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        #endregion
    }
}