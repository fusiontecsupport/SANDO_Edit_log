using scfs_erp.Context;
using scfs_erp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using static scfs_erp.Models.EInvoice;
using scfs.Data;
using System.Net;
using System.IO;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using scfs_erp.Helper;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Net.Mail;
using System.Web;

namespace scfs_erp.Controllers.ExportSCMTR
{

    [SessionExpire]   
    public class CHPOE07Controller : Controller
    {
        // GET: CHPOE07

        #region Context Declaration
        SCFSERPContext context = new SCFSERPContext();

        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

        #endregion

        #region Index 

        //[Authorize(Roles = "ExportStuffingBillIndex")]
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

            DateTime sd = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;

            DateTime ed = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;
            return View();
        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_XML_File_List(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, 1);
                var aaData = data.Select(d => new string[] { d.XMLFileName.ToString(), d.XMLPath.ToString(), d.LoadedDateTime.Value.ToString("dd/MM/yyyy hh:mm tt"), d.XMLId.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region ImportXMLFile
        public ActionResult ImportXMLFile(string ids = "")/*10rs.reminder*/
        {

            //SqlDataReader reader = null;
            //SqlDataReader Sreader = null;
            //string _connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //SqlConnection myConnection = new SqlConnection(_connStr);

            string rdesc = "";
            //var xmlid = id;

            var A_XMLIDs = ids.Split(';');
            //string[] A_XMLIDs = ids;
            var strimpPath = ConfigurationManager.AppSettings["FileImportedPath"];

            try
            {
                for (int count = 0; count < A_XMLIDs.Count(); count++)
                {
                    if (Convert.ToInt32(A_XMLIDs[count]) > 0)
                    {
                        CHPOE07_XMLFileInsert(Convert.ToInt32(A_XMLIDs[count]));
                        //

                        string xmlfilepathname = "";
                        string xmlfilename = "";
                        string strimpfilePath = "";
                        var result = context.Database.SqlQuery<XMLwithOpenXML>("Select * From XMLwithOpenXML where XMLId=" + Convert.ToInt32(A_XMLIDs[count])).ToList();
                        if (result.Count() != 0)
                        {
                            xmlfilepathname = result[0].XMLPath;
                            xmlfilename = result[0].XMLFileName;
                        }

                        //string xmlfilename = sctranmid.ToString() + "_SC.pdf";
                        //string path1 = Server.MapPath("~/SCFiles/" + sctranmid.ToString() + "/"); //Server.MapPath(Session["CUSRID"]+ "/Quotation/");
                        //string file1 = path1 + scfilename;

                        if (Directory.Exists(Path.GetDirectoryName(xmlfilepathname)))
                        {
                            if (!(Directory.Exists(strimpPath)))
                            {
                                Directory.CreateDirectory(strimpPath);
                            }
                            else
                            {
                                strimpfilePath = strimpPath + "\\" + xmlfilename;
                                System.IO.File.Copy(xmlfilepathname, strimpfilePath, true);
                                System.IO.File.Delete(xmlfilepathname);
                            }

                        }



                        //
                    }
                }
                rdesc = "Imported Succesfully";
            }
            catch (Exception ex)
            {
                rdesc = ex.Message;
                //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }

            //myConnection.Close();

            return Content(rdesc);


        }

        #endregion

        #region CHPOE07_XMLFileInsert
        private void CHPOE07_XMLFileInsert(int xmlid)
        {
            //string _connStr = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(constring);

            string strSQL = "";

            strSQL = "z_pr_CHPOE07_XML_Detail_Insert_Assgn";

            myConnection = new SqlConnection(constring);
            SqlCommand cmd = new SqlCommand(strSQL, myConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Pxmlid", xmlid);
            cmd.Parameters.AddWithValue("@PDHID", 0);
            myConnection.Open();
            cmd.ExecuteNonQuery();
            myConnection.Close();

        }

        #endregion
    }
}