using ClosedXML.Excel;
using iTextSharp.text.pdf;
//using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp;
using iTextSharp.text;
using Microsoft.AspNet.SignalR.Hosting;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.Client.Http;
using Seagull.Services.Localization;
using OfficeOpenXml;
using static Seagull.Admin.Controllers.ReportController;

namespace Seagull.Admin.Helpers.ReportHelpers
{
    public static class ExportHelper
    {
        #region Methods
        public static void ExportToExcell(System.Data.DataTable query, string ReportName, List<ColumnName> ColumnName, HttpResponseBase Response)
        {
            string fileName = string.Format("{0}{1}_{2}.xls", ReportName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), GenerateRandomDigitCode(4));
            GridView grid = new GridView();
            BoundField column;
            string data = string.Empty;
            foreach (var item in ColumnName)
            {
                column = new BoundField();
                column.DataField = item.DataField;
                column.HeaderText = item.HeaderText;
                column.HeaderStyle.BackColor = item._color;
                column.HeaderStyle.ForeColor = System.Drawing.Color.White;
                grid.Columns.Add(column);
                //data += string.Format("<th scope='col' colspan='6' style='width:100px;background-color:red;color:white'>{0}</th>", item.HeaderText);
            }
            grid.AutoGenerateColumns = false;
            grid.DataSource = query;// ((IEnumerable<dynamic>)query).ToList().EnumToDataTable<object>();
            grid.DataBind();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            StringWriter s = new StringWriter();
            HtmlTextWriter HtmlWrite = new HtmlTextWriter(s);
            grid.RenderControl(HtmlWrite);
            //for big size of span use this 
            //string headerTable = @"<table class='GridTable'><thead><tr class='GridHeader'><th scope='col' colspan='22' style='width:100px;background-color:blue;'>{0}</th></tr><tr class='GridHeader'>{1}</tr></thead></table>";
            //string newheaderTable = string.Format(headerTable, ReportName , data);
            //Response.Output.Write(newheaderTable);
            Response.BufferOutput = true;
            Response.Output.Write(s.ToString());
            Response.End();
        }

        public static void ExportToExcellWithMultipleSheets(string ReportName, List<MultipleSheetExport> Sheets, HttpResponseBase Response, string Report = "")
        {
            string fileName = string.Format("{0}{1}_{2}.xls", ReportName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), GenerateRandomDigitCode(4));
            XLWorkbook wb = new XLWorkbook();
            wb.RightToLeft = true;
            wb.PageOptions.CenterVertically = true;
            wb.PageOptions.ShowGridlines = true;
            wb.Style.Font.Bold = true;
            wb.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            for (int i = 0; i < Sheets.Count; i++)
            {
                GridView gvExcel = Sheets[i].gvExcel;
                string SheetName = Sheets[i].SheetName;
                gvExcel.Visible = true;
                if (gvExcel.Visible)
                {
                    gvExcel.AllowPaging = false;
                    gvExcel.DataBind();
                    System.Data.DataTable dt = Sheets[i].Records;
                    if (dt != null)
                    {
                        //if DataTable
                        foreach (var item in Sheets[i].ColumnName)
                        {
                            try
                            {
                                dt.Columns[item.DataField].ColumnName = item.HeaderText;
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        //If Datatable 
                        foreach (GridViewRow row in gvExcel.Rows)
                        {
                            dt.Rows.Add();
                            for (int c = 0; c < row.Cells.Count; c++)
                            {
                                dt.Rows[dt.Rows.Count - 1][c] = row.Cells[c].Text;
                            }
                        }

                        // Let's play with the rows and columns
                        wb.Worksheets.Add(dt, SheetName);
                        wb.Worksheet(SheetName).Row(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.Red;
                        wb.Worksheet(SheetName).Column(1).CellsUsed().Style.Fill.BackgroundColor = XLColor.PowderBlue;
                        wb.Worksheet(SheetName).Column(1).CellsUsed().Style.Font.FontColor = XLColor.RedPigment;
                        switch (Report)
                        {
                            case "GetSectorFundAndProgressPerYear":
                                //Get Count For All Used Rows
                                var totalRows = wb.Worksheet(SheetName).RowsUsed().Count();
                                if (SheetName == "mainsheet")
                                {
                                    wb.Worksheet(SheetName).Column(10).ColumnUsed().Style.Fill.BackgroundColor = XLColor.BlueViolet;
                                    wb.Worksheet(SheetName).Row(2).CellsUsed().Style.Fill.BackgroundColor = XLColor.Yellow;
                                    wb.Worksheet(SheetName).Row(totalRows - 1).CellsUsed().Style.Fill.BackgroundColor = XLColor.Yellow;
                                    wb.Worksheet(SheetName).Row(totalRows).CellsUsed().Style.Fill.BackgroundColor = XLColor.Green;
                                }
                                break;
                            default:
                                break;
                        }
                        gvExcel.AllowPaging = true;
                    }
                }
            }
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
            using (MemoryStream MyMemoryStream = new MemoryStream())
            {
                wb.SaveAs(MyMemoryStream);
                MyMemoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public static void ExportToPDF(List<object> query, string ReportName, List<ColumnName> ColumnName, HttpResponseBase Response)
        {
            string fileName = string.Format("{0}{1}_{2}.pdf", ReportName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), GenerateRandomDigitCode(4));
            GridView GridView1 = new GridView();
            BoundField column;
            foreach (var item in ColumnName)
            {
                column = new BoundField();
                column.DataField = item.DataField;
                column.HeaderText = item.HeaderText;
                column.HeaderStyle.BackColor = System.Drawing.Color.LightBlue;
                column.HeaderStyle.ForeColor = System.Drawing.Color.Black;
                GridView1.Columns.Add(column);
                //data += string.Format("<th scope='col' colspan='6' style='width:100px;background-color:red;color:white'>{0}</th>", item.HeaderText);
            }
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    //To Export all pages
                    GridView1.AllowPaging = false;
                    GridView1.AutoGenerateColumns = false;
                    GridView1.DataSource = query;
                    GridView1.DataBind();
                    GridView1.RenderControl(hw);
                    StringReader sr = new StringReader(sw.ToString());
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A2, 10f, 10f, 10f, 0f);
                    iTextSharp.text.html.simpleparser.HTMLWorker htmlparser = new iTextSharp.text.html.simpleparser.HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
        }
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }
        #endregion

        #region Class
        //public class ColumnName
        //{
        //    public string DataField { get; set; }
        //    public string HeaderText { get; set; }
        //    public System.Drawing.Color _color { get; set; }
        //}

        public class MultipleSheetExport
        {
            public MultipleSheetExport()
            {
                gvExcel = new GridView();
                Records = new System.Data.DataTable();
                ColumnName = new List<ColumnName>();
                SheetName = string.Empty;
            }
            public System.Data.DataTable Records { get; set; }
            public List<ColumnName> ColumnName { get; set; }
            public GridView gvExcel { get; set; }
            public string SheetName { get; set; }
        }
        public class ExportColumns
        {
            public string ColumnName { get; set; }
            public string ColumnId { get; set; }
        }
        public static byte[] ExportModel(DataTable query, List<ExportColumns> ColumnName, string workSheetName)
        {
            byte[] result;

            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook

                var worksheet = package.Workbook.Worksheets.Add(workSheetName); //Worksheet name

                using (var cells = worksheet.Cells[1, 1, 1, 5]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                //First add the headers
                for (var i = 0; i < ColumnName.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = ColumnName[i].ColumnName;
                }

                for (var i = 1; i <= query.Rows.Count; i++)
                {
                    int count = 1;
                    // For each colum Id in ColumnName List 
                    foreach (ExportColumns columnId in ColumnName)
                    {
                        // For each row, print the values of each column.
                        DataRow row = query.Rows[i - 1];
                        worksheet.Cells[i + 1, count].Value = row[columnId.ColumnId].ToString();
                        count++;
                    }
                }
                result = package.GetAsByteArray();
            }
            return result;
        }

        #endregion
    }
}