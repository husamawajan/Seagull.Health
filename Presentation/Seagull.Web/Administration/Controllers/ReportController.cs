using System;
using Seagull.Core;
using System.Linq;
using System.Web.Mvc;
using Seagull.Core.Domain.Common;
using Seagull.Services.Localization;
using Seagull.Services.Security;
using Seagull.Web.Framework.Controllers;
using Seagull.Admin.Extensions;
using Seagull.Web.Framework.Kendoui;
using Newtonsoft.Json;
using Seagull.Services.Helpers;
using Seagull.Services.Logging;
using Seagull.Admin.Helpers;
using System.Data;
using Seagull.Core.Data;
using Seagull.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Seagull.Core.Domain.UserEntitys;
using Seagull.Services.UserEntitys;
using Seagull.Admin.GridBuild;
using System.Drawing;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using System.Web;
namespace Seagull.Admin.Controllers
{
	[AdminAuthorize]
    public class ReportController : BaseAdminController
    {
        #region Fields

		private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
	    private readonly IUserActivityService _userActivityService;
		private readonly IWorkContext _workContext;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _IDbContext;
        private readonly IRepository<UserEntity> _UserEntityRepository;
        private readonly IUserEntityService _UserEntityService;

		#endregion

		#region Constructors

        public ReportController( 
			ILocalizationService localizationService,
            IPermissionService permissionService,
            AdminAreaSettings adminAreaSettings,
			IUserActivityService userActivityService,
			IWorkContext workContext,
             IDataProvider dataProvider,
            IDbContext IDbContext,
            IRepository<UserEntity> UserEntityRepository,
            IUserEntityService UserEntityService)
		{
			this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
			this._userActivityService = userActivityService;
			this._workContext = workContext;
            this._dataProvider = dataProvider;
            this._IDbContext = IDbContext;
            this._UserEntityRepository = UserEntityRepository;
            this._UserEntityService = UserEntityService;
		}

		#endregion

		#region Report

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            // if (!_permissionService.Authorize(StandardPermissionProvider.ViewEdpKpiPlanReport))
            //  return AccessDeniedView();

            return View();
        }


        #region Export Helpers
        //private void ExportToExcell(DataTable query, string ReportName, List<ColumnName> ColumnName)
        //{
        //    string fileName = string.Format("{0}{1}_{2}.xls", ReportName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), GenerateRandomDigitCode(4));
        //    GridView grid = new GridView();
        //    BoundField column;
        //    string data = string.Empty;
        //    foreach (var item in ColumnName)
        //    {
        //        column = new BoundField();
        //        column.DataField = item.DataField;
        //        column.HeaderText = item.HeaderText;
        //        column.HeaderStyle.BackColor = item._color;
        //        column.HeaderStyle.ForeColor = System.Drawing.Color.White;
        //        grid.Columns.Add(column);
        //        //data += string.Format("<th scope='col' colspan='6' style='width:100px;background-color:red;color:white'>{0}</th>", item.HeaderText);
        //    }
        //    grid.AutoGenerateColumns = false;
        //    grid.DataSource = query;// ((IEnumerable<dynamic>)query).ToList().EnumToDataTable<object>();
        //    grid.DataBind();
        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    Response.ContentEncoding = System.Text.Encoding.Unicode;
        //    Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
        //    StringWriter s = new StringWriter();
        //    HtmlTextWriter HtmlWrite = new HtmlTextWriter(s);
        //    grid.RenderControl(HtmlWrite);
        //    //for big size of span use this 
        //    //string headerTable = @"<table class='GridTable'><thead><tr class='GridHeader'><th scope='col' colspan='22' style='width:100px;background-color:blue;'>{0}</th></tr><tr class='GridHeader'>{1}</tr></thead></table>";
        //    //string newheaderTable = string.Format(headerTable, ReportName , data);
        //    //Response.Output.Write(newheaderTable);
        //    Response.Output.Write(s.ToString());
        //    Response.End();
        //}
        //private void ExportToPDF(List<object> query, string ReportName, List<ColumnName> ColumnName)
        //{
        //    string fileName = string.Format("{0}{1}_{2}.pdf", ReportName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), GenerateRandomDigitCode(4));
        //    GridView GridView1 = new GridView();
        //    BoundField column;
        //    foreach (var item in ColumnName)
        //    {
        //        column = new BoundField();
        //        column.DataField = item.DataField;
        //        column.HeaderText = item.HeaderText;
        //        column.HeaderStyle.BackColor = System.Drawing.Color.LightBlue;
        //        column.HeaderStyle.ForeColor = System.Drawing.Color.Black;
        //        GridView1.Columns.Add(column);
        //        //data += string.Format("<th scope='col' colspan='6' style='width:100px;background-color:red;color:white'>{0}</th>", item.HeaderText);
        //    }
        //    using (StringWriter sw = new StringWriter())
        //    {
        //        using (HtmlTextWriter hw = new HtmlTextWriter(sw))
        //        {
        //            //To Export all pages
        //            GridView1.AllowPaging = false;
        //            GridView1.AutoGenerateColumns = false;
        //            GridView1.DataSource = query;
        //            GridView1.DataBind();
        //            GridView1.RenderControl(hw);
        //            StringReader sr = new StringReader(sw.ToString());
        //            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A2, 10f, 10f, 10f, 0f);
        //            iTextSharp.text.html.simpleparser.HTMLWorker htmlparser = new iTextSharp.text.html.simpleparser.HTMLWorker(pdfDoc);
        //            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //            pdfDoc.Open();
        //            htmlparser.Parse(sr);
        //            pdfDoc.Close();
        //            Response.ContentType = "application/pdf";
        //            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
        //            Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //            Response.Write(pdfDoc);
        //            Response.End();
        //        }
        //    }
        //}
       
        public class ColumnName
        {
            public string DataField { get; set; }
            public string HeaderText { get; set; }
            public System.Drawing.Color _color { get; set; }
        }
        private List<ColumnName> GetEconomyReportColumn()
        {
            var _column = new List<ColumnName>();
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "TaskName", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.Task") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Available", HeaderText = _localizationService.GetResource("admin.economyplan.availableamount") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "NotAvailable", HeaderText = _localizationService.GetResource("admin.economyplan.notavailableamount") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "EntityValue", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.ActualCost") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "FundRate", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.FundRate") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Target", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.Target") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Actual", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.Actual") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Progressrate", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.PrograssRate") });

            return _column;
        }
        private List<ColumnName> GetGeneralEconomyReportColumn()
        {
            var _column = new List<ColumnName>();
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Base", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.Base") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "ActionName", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.ActionId") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "ProjectClassification", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.ProjectClassification") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "ProjectType", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.ProjectType") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "ProjectName", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.PeojectName") });


            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyDone", HeaderText = _localizationService.GetResource("admin.economyplan.MonthlyDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyNotDone", HeaderText = _localizationService.GetResource("admin.economyplan.MonthlyNotDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyPercentageDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.MonthlyPercentageDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyPercentageNotDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.MonthlyPercentageNotDone") });

            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Done", HeaderText = _localizationService.GetResource("admin.economyplan.Done") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "NotDone", HeaderText = _localizationService.GetResource("admin.economyplan.NotDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "PercentageDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.PercentageDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "PercentageNotDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.PercentageNotDone") });

            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyFundTotalOfDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.MonthlyFundTotalOfDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyFundEntityTotalOfDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.MonthlyFundEntityTotalOfDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "MonthlyFundPercentage", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.MonthlyFundPercentage") });

            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "FundTotalOfDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.FundTotalOfDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "FundEntityTotalOfDone", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.FundEntityTotalOfDone") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "FundPercentage", HeaderText = _localizationService.GetResource("Admin.EconomyPlan.FundPercentage") });

            return _column;
        }
        private List<ColumnName> GetGovernmentReportColumn()
        {
            var _column = new List<ColumnName>();
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Pivot", HeaderText = _localizationService.GetResource("Admin.GovermentMainPlan.Pivot") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Program", HeaderText = _localizationService.GetResource("Admin.GovermentMainPlan.Program") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Step", HeaderText = _localizationService.GetResource("Admin.GovermentMainPlan.Steps") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "KPI", HeaderText = _localizationService.GetResource("Admin.GovermentMainPlan.kpi") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Year", HeaderText = _localizationService.GetResource("Admin.GovermentMainPlan.Year") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Q", HeaderText = _localizationService.GetResource("Admin.GovermentMainPlan.Q") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "QRate", HeaderText = _localizationService.GetResource("Admin.GovermentPlanReports.QRate") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Target", HeaderText = _localizationService.GetResource("Admin.GovermentPlanReports.totaltargetcumulative") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "Actual", HeaderText = _localizationService.GetResource("Admin.GovermentPlanReports.TotalActualCumulative") });
            _column.Add(new ColumnName { _color = System.Drawing.Color.Red, DataField = "TotalRate", HeaderText = _localizationService.GetResource("Admin.GovermentPlanReports.AccomplishmentRate") });

            return _column;
        }
        #endregion
        
		#endregion
    }
}
	
