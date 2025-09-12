using ClosedXML.Excel;
using scfs.Data;
using scfs_erp;
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

namespace scfs.Controllers.Search
{
    [SessionExpire]
    public class InvoiceHSNDetailsController : Controller
    {
        // GET: InvoiceHSNDetails
        SCFSERPContext context = new SCFSERPContext();
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
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
            return View();
        }

        public void showrpt()
        {

            int etype = Convert.ToInt32(Request.Form.Get("etype"));

            var afromDate = Request.Form.Get("from").Split('-');
            var fromDate = afromDate[2] + "-" + afromDate[1] + "-" + afromDate[0];

            var atoDate = Request.Form.Get("to").Split('-');
            var toDate = atoDate[2] + "-" + atoDate[1] + "-" + atoDate[0];

            var SDATE = afromDate[0] + "-" + afromDate[1] + "-" + afromDate[2];
            var EDATE = atoDate[0] + "-" + atoDate[1] + "-" + atoDate[2];

            var date1 = Convert.ToDateTime(SDATE).ToString("dd-MMM-yyyy");
            var date2 = Convert.ToDateTime(EDATE).ToString("dd-MMM-yyyy");


            string filename = "";
            string query = "";
            string spname = "";
            string tblname = "";
            string strhead = "";
            switch (etype)
            {
                case 0:
                    tblname = "Import";
                    spname = "pr_Import_HSN_Code_Wise_Invoice_Detail_Assgn";
                    strhead = "Import Invoice Details From " + date1 + " Till " + date2;
                    query = "exec [pr_Import_HSN_Code_Wise_Invoice_Detail_Assgn] @PSDPTID = 1, @PSDate = '" + date1 + "' , @PEDate = '" + date2 + "'";
                    filename = "Import_Invoice_Details_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    break;
                case 1:
                    tblname = "Export";
                    spname = "pr_Export_HSN_Code_Wise_Invoice_Detail_Assgn";
                    strhead = "Export Invoice Details From " + date1 + " Till " + date2;
                    query = "exec [pr_Export_HSN_Code_Wise_Invoice_Detail_Assgn] @PSDPTID = 2, @PSDate = '" + date1 + "' , @PEDate = '" + date2 + "'";
                    filename = "Export_Invoice_Details_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    break;
                case 2:
                    tblname = "Bond";
                    spname = "pr_Bond_HSN_Code_Wise_Invoice_Detail_Assgn";
                    strhead = "Bond Invoice Details From " + date1 + " Till " + date2;
                    query = "exec [pr_Bond_HSN_Code_Wise_Invoice_Detail_Assgn] @PSDPTID = 10, @PSDate = '" + date1 + "' , @PEDate = '" + date2 + "'";
                    filename = "Bond_Invoice_Details_" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    break;
            }

            filename = filename + ".xlsx";

            DataTable dt0 = new DataTable();
            DataTable dt1 = new DataTable();

            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection con = new SqlConnection(constring);

            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, spname);
            //da.Fill(dt);
            con.Close();
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(tblname);

                string compname = Session["COMPNAME"].ToString();
                
                ws.Cell(1, 1).Value = "" + compname.ToString();
                ws.Cell(2, 1).Value = "" + strhead.ToString();

                dt0 = ds.Tables[0];
                dt0.TableName = tblname;
                ws.Name = tblname;
                ws.Cell(4, 1).InsertTable(dt0);
                ws.Tables.Table(0).ShowAutoFilter = false;
                ws.Tables.Table(0).ShowRowStripes = false;
                //wb.Worksheets.Add(dt0);

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
            //return RedirectToAction("Index", "SalesInvoiceRpt");

        }

    }
}