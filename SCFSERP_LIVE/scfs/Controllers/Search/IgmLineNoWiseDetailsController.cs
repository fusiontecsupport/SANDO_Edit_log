using ClosedXML.Excel;
using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Search
{
    [SessionExpire]
    public class IgmLineNoWiseDetailsController : Controller
    {
        // GET: IgmLineNoWiseDetails

        SCFSERPContext context = new SCFSERPContext();
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            ViewBag.GIDID = new SelectList("");
            return View();
        }

        public JsonResult GetContainerNo(string id)
        {
            string IGMNO = ""; string LineNo = "";

            if (id.Contains('~'))
            {
                var Param = id.Split('~');

                IGMNO = Param[0].ToString();
                LineNo = Param[1].ToString();
            }

            var query = context.Database.SqlQuery<pr_IGMNO_Search_ContainerNo_GridAssgn_Result>("EXEC pr_IGMNO_Search_ContainerNo_GridAssgn  @PIGNNo = '" + IGMNO.ToString() + "', @PGPLNo = '" + LineNo.ToString() + "'").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public void showrpt()
        {
            string IgmNo = ""; string LineNo = "";

            IgmNo = Request.Form.Get("textIgmNo").ToString();
            LineNo = Request.Form.Get("txtLineNo").ToString();
            GetIGMNoContainerGridXLRpt(IgmNo + "~" + LineNo);
        }
        public ActionResult GetIGMNoContainerGridXLRpt(int gentypeid)
        {

            String constring = ConfigurationManager.ConnectionStrings["KGKDefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);
            string sdate = "", edate = "";

            var afromDate = Request.Form.Get("from").Split('-');
            var fromDate = afromDate[2] + "-" + afromDate[1] + "-" + afromDate[0];

            var atoDate = Request.Form.Get("to").Split('-');
            var toDate = atoDate[2] + "-" + atoDate[1] + "-" + atoDate[0];

            var SDATE = afromDate[0] + "-" + afromDate[1] + "-" + afromDate[2];
            var EDATE = atoDate[0] + "-" + atoDate[1] + "-" + atoDate[2];

            var date1 = Convert.ToDateTime(SDATE).ToString("dd-MMM-yyyy");
            var date2 = Convert.ToDateTime(EDATE).ToString("dd-MMM-yyyy");

            int atype = Convert.ToInt32(Request.Form.Get("TMP_ACK"));
            int compyid = Convert.ToInt32(System.Web.HttpContext.Current.Session["compyid"]);

            string query = "";// "[dbo].[pr_Sales_Invoice_DOS_Detail_Report_01] @PSDATE = '" + date1 + "', @PEDATE = '" + date2 + "'";
            string tblname = "";
            string filename = "";
            switch (gentypeid)
            {
                case 0:
                    query = "[dbo].[pr_Sales_Invoice_DOS_Detail_Report_01] @PSDATE = '" + date1 + "', @PEDATE = '" + date2 + "'";
                    tblname = "DOS";
                    filename = "DOS.xlsx";
                    break;
                case 1:
                    break;
                case 2:
                    query = "[dbo].[pr_IBPO_DOS_Detail_Report_N01] @PSDATE = '" + date1 + "', @PEDATE = '" + date2 + "'";
                    tblname = "IBPO_DOS";
                    filename = "IBPO_DOS.xlsx";
                    break;
            }

            DataTable dt = new DataTable();
            dt.TableName = tblname;// "DOS";
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);
            con.Close();

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                ws.Table(0).ShowAutoFilter = true; // Disable AutoFilter.
                ws.Table(0).Theme = XLTableTheme.None; // Remove Theme.
                ws.Columns().AdjustToContents(); // Resize all columns.

                //
                //

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.Style.Border = null;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment;filename= DOS.xlsx");
                Response.AddHeader("content-disposition", "attachment;filename = " + filename);

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }

            return RedirectToAction("Index", "SalesInvoiceRpt");
        }

        public ActionResult GetIGMNoContainerGridXLRpt(string id)
        {
            //string IgmNo = Request.Form.Get("textIgmNo");
            //string LineNo = Request.Form.Get("txtLineNo");

            string IgmNo = ""; string LineNo = "";

            if (id.Contains('~'))
            {
                var Param = id.Split('~');

                IgmNo = Param[0].ToString();
                LineNo = Param[1].ToString();
            }


            string filename = "IGMNo_" +  DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            filename = filename + ".xlsx";
            string query = "";

            //var query = context.Database.SqlQuery<pr_IGMNO_Search_ContainerNo_GridAssgn_Result>("EXEC pr_IGMNO_Search_ContainerNo_GridAssgn  @PIGNNo = '" + IGMNO.ToString() + "', @PGPLNo = '" + LineNo.ToString() + "'").ToList();


            query = "exec [pr_IGMNO_Search_ContainerNo_GridAssgn_001] @PIGNNo = '" + IgmNo + "' , @PGPLNo = '" + LineNo + "'";


            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "pr_IGMNO_Search_ContainerNo_GridAssgn_001");
            //da.Fill(dt);
            con.Close();




            using (XLWorkbook wb = new XLWorkbook())
            {
                dt0 = ds.Tables[0];
                dt0.TableName = "FilterDetails";
                wb.Worksheets.Add(dt0);
                //dt1 = ds.Tables[1];
                //dt1.TableName = "pr_IGMNO_Search_ContainerNo_GridAssgn";
                //wb.Worksheets.Add(dt1);


                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.Style.Border = null;

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
            return RedirectToAction("Index", "SalesInvoiceRpt");
        }


    }
}