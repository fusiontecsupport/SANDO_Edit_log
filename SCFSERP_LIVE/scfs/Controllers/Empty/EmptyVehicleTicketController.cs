
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using scfs.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace scfs_erp.Controllers.Empty
{
    [SessionExpire]
    public class EmptyVehicleTicketController : Controller
    {
        // GET: EmptyVehicleTicket

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Form
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
            using (var e = new SCFSERPEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Empty_VT(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.STMRNAME, d.VHLNO, d.GIDID.ToString(), d.GODID.ToString() }).ToArray();
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

        #region Edit
        [Authorize(Roles = "EmptyVTEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/EmptyVehicleTicket/Form/" + id);
        }
        #endregion

        #region Form
        [Authorize(Roles = "EmptyVTCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            //ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_NEW_EMPTY_GATEOUT_CONTAINER_CBX_ASSGN>("select * from VW_NEW_EMPTY_GATEOUT_CONTAINER_CBX_ASSGN"), "GIDID", "CONTNRNO");
            if (id != 0)
            {
                tab = context.gateoutdetail.Find(id);

                var sql = (from r in context.gateindetails.Where(x => x.GIDID == tab.GIDID)
                           join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                           select new { r.CONTNRNO, r.GIDATE, r.STMRNAME, s.CONTNRSCODE }
                              ).ToList();

                if (sql.Count > 0)
                {
                    ViewBag.CONTNRNO = sql[0].CONTNRNO;
                    ViewBag.GIDATE = sql[0].GIDATE;
                    ViewBag.STMRNAME = sql[0].STMRNAME;
                    ViewBag.CONTNRSCODE = sql[0].CONTNRSCODE;
                }
            }
            return View(tab);
        }
        #endregion

        #region Auto complete for Container No
        public JsonResult AutoContainer(string term)
        {

            var result = context.Database.SqlQuery<VW_NEW_EMPTY_VT_CONTAINERNO_AUTOASSGN>("Select *From VW_NEW_EMPTY_VT_CONTAINERNO_AUTOASSGN  Where  CONTNRNO Like '%" + term + "%'").ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetContDetails
        public JsonResult GetContDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_NEW_EMPTY_GATEOUT_CONTAINER_CBX_ASSGN>("SELECT * FROM VW_NEW_EMPTY_GATEOUT_CONTAINER_CBX_ASSGN WHERE GIDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetCha
        public JsonResult GetCha(int id)
        {
            var sql = (from r in context.gateindetails.Where(x => x.GIDID == id)
                       join s in context.opensheetdetails on r.GIDID equals s.GIDID
                       join om in context.opensheetmasters on s.OSMID equals om.OSMID
                       join bd in context.billentrydetails on s.BILLEDID equals bd.BILLEDID
                       join m in context.billentrymasters on bd.BILLEMID equals m.BILLEMID
                       select new { m.CHAID, m.BILLEMNAME }
                              ).ToList();

            return Json(sql, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region AutoCha
        public JsonResult AutoCha(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region savedata        
        public void savedata(GateOutDetail tab)
        {
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 3;
            tab.REGSTRID = 1;
            tab.TRANDID = 0;

            //tab.GODATE = tab.GOTIME.Date;

            string indate = Convert.ToString(tab.GODATE);
            if (indate != null || indate != "")
            {
                tab.GODATE = Convert.ToDateTime(indate).Date;
            }
            else { tab.GODATE = DateTime.Now.Date; }

            string intime = Convert.ToString(tab.GOTIME);
            if ((intime != null || intime != "") && ((indate != null || indate != "")))
            {
                if ((intime.Contains(' ')) && (indate.Contains(' ')))
                {
                    var in_time = intime.Split(' ');
                    var in_date = indate.Split(' ');

                    if ((in_time[1].Contains(':')) && (in_date[0].Contains('/')))
                    {

                        var in_time1 = in_time[1].Split(':');
                        var in_date1 = in_date[0].Split('/');

                        string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                        tab.GOTIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GOTIME = DateTime.Now; }
                }
                else { tab.GOTIME = DateTime.Now; }
            }
            else { tab.GOTIME = DateTime.Now; }
                        
            tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = Session["CUSRID"].ToString(); 
            tab.PRCSDATE = DateTime.Now;
            tab.EHIDATE = DateTime.Now;
            tab.EHITIME = DateTime.Now;
            tab.GOBTYPE = 3;
            tab.SSEALNO = "-";
            tab.LSEALNO = "-";

            if ((tab.GODID).ToString() != "0")
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID = 3").ToString());
                int ano = tab.GONO;
                // string prfx = string.Format("{0:D5}", ano);
                tab.GODNO = ano.ToString();
                context.gateoutdetail.Add(tab);
                context.SaveChanges();


            }
            Response.Redirect("Index");
        }
        #endregion

        #region Printview
        [Authorize(Roles = "EmptyVTPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EMPTYGATEOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_Empty_GateOut.RPT");
                                
                cryRpt.RecordSelectionFormula = "{VW_IMPORT_EMPTY_GATEOUT_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_EMPTY_GATEOUT_PRINT_ASSGN.GODID} = " + id;

                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;
                crConnectionInfo.Password = stringbuilder.Password;

                CrTables = cryRpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table CrTable in CrTables)
                {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }

                cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "");
                cryRpt.Dispose();
                cryRpt.Close();
                GC.Collect();
                stringbuilder.Clear();
            }

        }
        #endregion
       
        #region Del ---
        [Authorize(Roles = "EmptyVTDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {

                GateOutDetail gateoutdetail = context.gateoutdetail.Find(Convert.ToInt32(id));
                context.gateoutdetail.Remove(gateoutdetail);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else

                Response.Write(temp);

        }
        #endregion
    }
}