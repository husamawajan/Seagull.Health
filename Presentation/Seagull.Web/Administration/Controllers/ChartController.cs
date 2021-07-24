using Seagull.Admin.Models.charts;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Domain.Chart;
using Seagull.Data;
using Seagull.Services.chart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Seagull.Admin.Controllers
{
    public class ChartController : BaseAdminController
    {
        #region Fields...

        private readonly IDbContext _IDbContext;
        private readonly IDataProvider _dataProvider;
        private readonly IWorkContext _workContext;
        private readonly IChartService _ChartService;

        #endregion

        #region Cstor...

        public ChartController(IDbContext IDbContext, IDataProvider dataProvider, IWorkContext workContext, IChartService ChartService)
        {

            _IDbContext = IDbContext;
            _dataProvider = dataProvider;
            _workContext = workContext;
            _ChartService = ChartService;
        }

        #endregion
        // GET: Chart
        #region ActionResults...

        public ActionResult Index()
        {
            ChartsUInterfaceModel data = new ChartsUInterfaceModel();
            data._listofitems = _IDbContext.ExecuteStoredProcedureListObject<SelectListItem>("EXEC GetTableName").ToList();
            data._listofrelations.Add(new SelectListItem { Value = "inner join", Text = "inner join" });
            data._listofrelations.Add(new SelectListItem { Value = "left join", Text = "left join" });
            data._listofrelations.Add(new SelectListItem { Value = "right join", Text = "right join" });
            return View(data);
        }

        public ActionResult GetAllColoumnName(String TableName)
        {
            var pTableName = _dataProvider.GetParameter();
            pTableName.ParameterName = "TableName";
            pTableName.Value = TableName;
            pTableName.DbType = DbType.String;

            var Data = _IDbContext.ExecuteStoredProcedureListObject<SelectListItem>("EXEC [GetColoumnName]", pTableName).ToList();
            return Json(Data);
        }

        public ActionResult GetEntry(
            List<string> ArrayOfQuery,
            string AddCondition,
            string AggregationFunction,
            List<string> UpperDropDown,
            string UpperTable,
            int RadioType,
            string MeterTitle,
            string MeterLabel,
            string MyChartType)
        {
            #region Global Variable
            var item_1 = ArrayOfQuery.ElementAt(0);
            var row_1 = item_1.Split(',');
            string SQLCommand = string.Empty;
            int Type = 0; //Selected Cahrt Type
            string Model = null;
            #endregion
            switch (RadioType)
            {
                case 2:// Chart Selected
                    #region Chart
                    if (ArrayOfQuery.Count < 2)
                        return Json("Error, wrong number of tables!");
                    else
                    {
                        var item = ArrayOfQuery.ElementAt(1);
                        var row = item.Split(',');
                        string mytable = row[0];
                        string mycolumn = row[1];
                        string myrelation = row[2];
                        SQLCommand = "Select" + " " + "Count(*)" + " " + "as Total" + " " + "," + " " + UpperDropDown.ElementAt(0) + " " +
                                     "as Name from" + "[" + row_1[0] + "]" + " " + row_1[2] + " " + "[" + mytable + "]" + "on" +
                                     row_1[1] + " = " + mycolumn;
                        for (int i = 2; i < ArrayOfQuery.Count; i++)
                        {
                            string items = ArrayOfQuery.ElementAt(i);
                            string[] rows = items.Split(',');
                            SQLCommand = SQLCommand + " " + myrelation + "[" + rows[0] + "]" + " on " + mycolumn + " = " + rows[1];
                            myrelation = rows[2];
                            mycolumn = rows[1];
                            mytable = rows[0];

                        }
                        SQLCommand = SQLCommand + " " + "GROUP BY" + " " + UpperDropDown.ElementAt(0);
                        Type = int.Parse(MyChartType);
                    }
                    #endregion
                    break;
                case 1://Meters Selected
                    #region Meters Selected
                    if (ArrayOfQuery.Count < 2)
                        SQLCommand = "Select" + " " + AggregationFunction + "(" + UpperDropDown.ElementAt(0) + ")" + "from" + UpperTable;
                    else
                    {
                        var item_2 = ArrayOfQuery.ElementAt(1);
                        var row_2 = item_2.Split(',');
                        string table = row_2[0];
                        string column = row_2[1];
                        string relation = row_2[2];
                        SQLCommand = "Select" + " " + AggregationFunction + "(" + UpperDropDown.ElementAt(0) + ")" + " " + "from" + " " + "[" + row_1[0] + "]" + " " + row_1[2] + " " + "[" + table + "]" + " on "
                                    + row_1[1] + " = " + column;
                        for (int i = 2; i < ArrayOfQuery.Count; i++)
                        {
                            string items = ArrayOfQuery.ElementAt(i);
                            string[] rows = items.Split(',');
                            SQLCommand = SQLCommand + " " + relation + " " + "[" + rows[0] + "]" + " on " + column + " = " + rows[1];
                            relation = rows[2];
                            column = rows[1];
                            table = rows[0];
                        }
                    }
                    #endregion
                    break;
                case 3://Grids Selected
                    #region Grids Selected

                    if (ArrayOfQuery.Count < 2)
                        SQLCommand = "Select * from " + UpperTable;
                    else
                    {
                        var item_2 = ArrayOfQuery.ElementAt(1);
                        var row_2 = item_2.Split(',');
                        string table = row_2[0];
                        string column = row_2[1];
                        string relation = row_2[2];
                        SQLCommand = "Select * from [" + row_1[0] + "]" + " " + row_1[2] + " " + "[" + table + "]" + " on "
                                    + row_1[1] + " = " + column;
                        for (int i = 2; i < ArrayOfQuery.Count; i++)
                        {
                            string items = ArrayOfQuery.ElementAt(i);
                            string[] rows = items.Split(',');
                            SQLCommand = SQLCommand + " " + relation + " " + "[" + rows[0] + "]" + " on " + column + " = " + rows[1];
                            relation = rows[2];
                            column = rows[1];
                            table = rows[0];
                        }
                    }
                     Model = UpperDropDown.ElementAt(0).Split('.')[1];
                    
                        for(var i=1 ; i < UpperDropDown.Count ; i++)
                        {
                            Model += "," + UpperDropDown.ElementAt(i).Split('.')[1];
                        }
                        Model = Model.Replace("[", "").Replace("]", "");
                    #endregion
                    break;
            }
            //Where Statement
            SQLCommand += " " + AddCondition;
            //insert the Report 
            #region insert new Report
            UserChart MyChartData = new UserChart();
            MyChartData.UserId = _workContext.CurrentUser.Id;
            MyChartData.SQLstatement = SQLCommand;
            MyChartData.CreateDate = DateTime.Now;
            MyChartData.Type = RadioType;
            MyChartData.Title = MeterTitle;
            MyChartData.Label = MeterLabel;
            MyChartData.ChartType = Type;
            MyChartData.GridModel =Model; 
            _ChartService.InsertChart(MyChartData);
            #endregion
            return Json(true);
        }
       
        public ActionResult GetAllMeters()
        {

            List<Meters> _meters = new List<Meters>();
            foreach (var item in _ChartService.GetUserCharts(_workContext.CurrentUser.Id , 1))
            {
                var MyCommand = _dataProvider.GetParameter();
                MyCommand.ParameterName = "CommandSql";
                MyCommand.DbType = DbType.String;
                MyCommand.Value = item.SQLstatement;
                _meters.Add(new Meters
                {
                    id = item.Id,
                    result = _IDbContext.ExecuteStoredProcedureListObject<int>("EXEC [DynamicProcedureChart]", MyCommand).FirstOrDefault(),
                    MyTitle = item.Title,
                    MyLabel = item.Label
                });
            }


            return Json(_meters);
        }
        public ActionResult GridData(object data)
        {
            var json = new JavaScriptSerializer().Serialize(data);
            return Json(json);
        }
       
        public ActionResult GetAllGrids()
        {

            List<Grid> _Grid = new List<Grid>();
            
            foreach (var item in _ChartService.GetUserCharts(_workContext.CurrentUser.Id, 3))
            {
                var MyCommand = _dataProvider.GetParameter();
                MyCommand.ParameterName = "CommandSql";
                MyCommand.DbType = DbType.String;
                MyCommand.Value = item.SQLstatement;
                _Grid.Add(new Grid
                {
                    id = item.Id,
                    result = _IDbContext.ExecuteStoredProcedureListObject<object>("EXEC [DynamicProcedureChart]", MyCommand).ToList(),
                    MyTitle = item.Title,
                    MyLabel = item.Label,
                    Model = item.GridModel,
                });
            }

            return Json(_Grid);
        }
        public ActionResult GetAllCharts()
        {
            List<BarChart> Bars = new List<BarChart>();
            foreach (var item in _ChartService.GetUserCharts(_workContext.CurrentUser.Id , 2 ))
            {
                var MyCommand = _dataProvider.GetParameter();
                MyCommand.ParameterName = "CommandSql";
                MyCommand.DbType = DbType.String;
                MyCommand.Value = item.SQLstatement;
                Bars.Add(new BarChart
                {
                    id = item.Id,
                    data = _IDbContext.ExecuteStoredProcedureListObject<BarChartValues>("EXEC [DynamicProcedureChart]", MyCommand).ToList(),
                    MyTitle = item.Title,
                    MyLabel = item.Label,
                });
            }
            return Json(Bars);
        }
        #region Charts Classes
        public class Meters
        {
            public int id { get; set; }
            public int result { get; set; }
            public string MyTitle { get; set; }
            public string MyLabel { get; set; }
           
        }

        public class Grid
        {
            public int id { get; set; }
            public object result { get; set; }
            public string MyTitle { get; set; }
            public string MyLabel { get; set; }
            public string Model { get; set; }
        }
        public class BarChart
        {
            public BarChart()
            {
                data = new List<BarChartValues>();
            }
            public int id { get; set; }
            public string MyTitle { get; set; }
            public string MyLabel { get; set; }
            public List<BarChartValues> data { get; set; }
            
        }
        public class BarChartValues
        {
            public int Total { get; set; }
            public string Name { get; set; }
        }
        #endregion
        #endregion
    }
}