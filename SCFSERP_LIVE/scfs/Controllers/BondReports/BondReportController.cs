using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using scfs_erp.Context;

namespace scfs.Controllers.BondReports
{
    public class BondReportController : Controller
    {
        // GET: BondReport
        #region contextdeclaration
        BondContext context = new BondContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        //[Authorize(Roles = "BondReportIndex")]
        public ActionResult Index()
        {
            if (Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            if (string.IsNullOrEmpty(Session["SDATE"] as string))
            {

                Session["SDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
                Session["EDATE"] = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                if (Request.Form.Get("from") != null)
                {
                    Session["SDATE"] = Request.Form.Get("from");
                    Session["EDATE"] = Request.Form.Get("to");
                }
            }
            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
        }

        public void GenerateReport()
        {
            string fromdt = Convert.ToDateTime(Request.Form.Get("from")).ToString("yyyy-MM-dd");
            string todt = Convert.ToDateTime(Request.Form.Get("to")).ToString("yyyy-MM-dd");

            string cusrid = Convert.ToString(Session["CUSRID"]);
            string crole = Convert.ToString(Session["Group"]);
            var rtype = Convert.ToInt32(Request.Form.Get("rtype"));
            if (crole == "SuperAdmin" || crole == "Admin")

            {
                //cusrid = "";
            }

            int compyid = Convert.ToInt32(Session["compyid"]);
            string filename =  "BondReport_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";

            switch (rtype)
            {
                case 0:
                    query = "exec [pr_Bond_FormA_Detail_Excel_Rpt]  @PSDATE = '" + fromdt + "', @PEDATE='" + todt + "'";
                    break;
                case 1:
                    query = "exec [pr_Bond_FormB_Detail_Excel_Rpt]  @PSDATE = '" + fromdt + "', @PEDATE='" + todt + "'";
                    break;
            }
            DataTable dt = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "test");
            //da.Fill(dt);
            con.Close();
            switch(rtype) 
            { 
                case 0:
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Form A");
                        ws.Cell(1, 5).Value = "Form - A";
                        ws.Cell(2, 5).Value = "Form to be maintained by the Warehouse licencee of the receipt, handling, storage and removal of the warehoused goods.";                //ws.Cell(2, 1).Value = compaddr.ToString();
                        ws.Cell(3, 5).Value = "Statement From the Date " + fromdt + " to " + todt;
                        ws.Cell(4, 5).Value = "SANCO BONDED WAREHOUSE 592,ENNORE EXPRESS HIGH ROAD, CHENNAI-600057   /   WH. CODE : MAA1S006     LIC. CODE : S-006";

                        dt = ds.Tables[0];
                        dt.TableName = " Reports";
                        ws.Name = "Form A Reports";
                        ws.Cell(5, 1).InsertTable(dt);
                        ws.Columns().AdjustToContents(); // Resize all columns.
                        ws.Tables.Table(0).ShowAutoFilter = false;
                        ws.Tables.Table(0).ShowRowStripes = false;                  

                        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = true;
                        wb.Style.Border = null;
                        wb.Style.Fill = null;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename= " + filename);// Collection_Details.xlsx");

                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            //Response.End();
                        }
                    }
                    break;
                case 1:
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Form B");
                        ws.Cell(1, 3).Value = "Form - B";
                        ws.Cell(2, 3).Value = "SANCO BONDED WAREHOUSE 592,ENNORE EXPRESS HIGH ROAD,CHENNAI-600057       WH. CODE : MAA1S006 /  LIC. CODE : S-006";
                        ws.Cell(3, 3).Value = "[See Para 3 of Circular No. 25/2016-Customs dated 08.06.2016] WH. CODE : MAA1S006 /  LIC. CODE : S-006";
                        ws.Cell(4, 3).Value = "Details of goods stored in the warehouse where the period for which they may remain warehoused under section 61 is expiring in the following month.  DEC - 2023";

                        dt = ds.Tables[0];
                        dt.TableName = " Reports";
                        ws.Name = "Form B Reports";
                        ws.Cell(5, 1).InsertTable(dt);
                        ws.Columns().AdjustToContents(); // Resize all columns.
                        ws.Tables.Table(0).ShowAutoFilter = false;
                        ws.Tables.Table(0).ShowRowStripes = false;

                        var count = dt.Rows.Count + 5;
                        int columnIndex = 12; // Assuming you want to sum values in the 10th column (index 9)
                        int columnIndex1 = 13;
                        double total = 0;
                        double total1 = 0;

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (row[columnIndex] != null)
                            {
                                total += Convert.ToDouble(row[columnIndex]);
                            }
                        }
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (row[columnIndex1] != null)
                            {
                                total1 += Convert.ToDouble(row[columnIndex]);
                            }
                        }
                        ws.Cell(count + 1, columnIndex - 1).Value = "Total";
                        ws.Cell(count + 1, columnIndex).Value = total;
                        ws.Cell(count + 1, columnIndex1).Value = total1;

                        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        wb.Style.Font.Bold = true;
                        wb.Style.Border = null;
                        wb.Style.Fill = null;

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename= " + filename);// Collection_Details.xlsx");

                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            //Response.End();
                        }
                    }

                    break;
            }
        }

    }
}