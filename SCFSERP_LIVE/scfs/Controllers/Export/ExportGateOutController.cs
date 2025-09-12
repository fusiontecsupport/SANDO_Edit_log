using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using scfs.Data;
using scfs_erp;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportGateOutController : Controller
    {
        // GET: ExportGateOut

        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Page
        [Authorize(Roles = "ExportGateOutIndex")]
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

            DateTime fromdate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["SDATE"]).Date;
            DateTime todate = Convert.ToDateTime(System.Web.HttpContext.Current.Session["EDATE"]).Date;

            TotalContainerDetails(fromdate, todate);

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

                var data = e.pr_Search_Export_GateOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]),
                    Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));

                //var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.IGMNO, d.VSLNAME, d.STMRNAME, d.PRDTDESC.ToString(), d.GIDID.ToString(), d.GODID.ToString() }).ToArray();
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.CHANAME.ToString(), d.VHLNO.ToString(), d.GDRVNAME.ToString(), d.GODID.ToString(), d.GIDID.ToString() }).ToArray();
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

        #region TotalContainerDetails
        public JsonResult TotalContainerDetails(DateTime fromdate, DateTime todate)
        {
            string fdate = ""; string tdate = ""; int sdptid = 1;
            if (fromdate == null)
            {
                fromdate = DateTime.Now.Date;
                fdate = Convert.ToString(fromdate);
            }
            else
            {
                string infdate = Convert.ToString(fromdate);
                var in_date = infdate.Split(' ');
                var in_date1 = in_date[0].Split('/');
                fdate = Convert.ToString(in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0]);
            }
            if (todate == null)
            {
                todate = DateTime.Now.Date;
                tdate = Convert.ToString(todate);
            }
            else
            {
                string intdate = Convert.ToString(todate);

                var in_date1 = intdate.Split(' ');
                var in_date2 = in_date1[0].Split('/');
                tdate = Convert.ToString(in_date2[2] + "-" + in_date2[1] + "-" + in_date2[0]);

            }


            var result = context.Database.SqlQuery<PR_EXPORT_LOADVEHICLEGATEOUT_DETAILS_Result>("EXEC PR_EXPORT_LOADVEHICLEGATEOUT_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=2").ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 3) && (rslt.Descriptn == "EXPORT - GATEOUT"))
                {

                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["EGO20"] = rslt.c_20;
                    Session["EGO40"] = rslt.c_40;
                    Session["EGO45"] = rslt.c_45;
                    Session["EGOTTU"] = rslt.c_tues;

                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Edit Page
        [Authorize(Roles = "ExportGateOutEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExportGateOut/Form/" + id);
        }
        #endregion

        #region FORM 
        [Authorize(Roles = "ExportGateOutCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            // VW_EXPORT_GATEOUT_MOD_ASSGN tab = new VW_EXPORT_GATEOUT_MOD_ASSGN();
            tab.GODID = 0;
            if (tab.GODID == 0)
            {
                tab.GODATE = DateTime.Now.Date;
                tab.GOTIME = DateTime.Now;
            }

            if (id != 0)
               
            
            //ViewBag.GIDID = new SelectList("");
           
            //-----------------------------Transport type-----------
          
            //end


            //-----------------------------CHA type-----------
           
            //end


            if (id != 0)//Edit Mode
            {

                //var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GODID=" + id).ToList();
                tab = context.gateoutdetail.Find(id);

                 var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN>("select * from VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN where GODID=" + tab.GODID).ToList();
                    ViewBag.FVTDNO = "";
                    ViewBag.CONTNRNO = "";
                    //var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                    if (query.Count > 0)
                    {
                        ViewBag.FVTDNO = query[0].ToString();
                        ViewBag.CONTNRNO = query[1].ToString();
                    }
                    
                             

            }
            return View(tab);
        }
        #endregion

        #region  Insert/Modify data
        public void savedata(GateOutDetail tab)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                    tab.SDPTID = 2;
                    tab.REGSTRID = 1;
                    tab.TRANDID = 0;

                    //tab.GOTIME = DateTime.Now;
                    //tab.GODATE = tab.GOTIME.Date;

                    tab.GOOTYPE = 0;tab.GOCTYPE = 0;tab.GOTTYPE = 0;

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
                        else
                        {
                            var in_time = intime;
                            var in_date = indate;

                            if ((in_time.Contains(':')) && (in_date.Contains('/')))
                            {
                                var in_time1 = in_time.Split(':');
                                var in_date1 = in_date.Split('/');

                                string in_datetime = in_date1[2] + "-" + in_date1[1] + "-" + in_date1[0] + "  " + in_time1[0] + ":" + in_time1[1] + ":" + in_time1[2];

                                tab.GOTIME = Convert.ToDateTime(in_datetime);
                            }
                            else { tab.GOTIME = DateTime.Now; }
                        }                       
                    }
                    else { tab.GOTIME = DateTime.Now; }

                    tab.LMUSRID = Session["CUSRID"].ToString();
                    tab.PRCSDATE = DateTime.Now;
                    tab.EHIDATE = DateTime.Now;
                    tab.EHITIME = DateTime.Now;
                    

                    if ((tab.GODID).ToString() != "0")
                    {
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        tab.CUSRID = Session["CUSRID"].ToString();
                        tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=2").ToString());
                        int ano = tab.GONO;
                        string prfx = string.Format("{0:D5}", ano);
                        tab.GODNO = prfx.ToString();
                        //tab.GODNO = ano.ToString();
                        context.gateoutdetail.Add(tab);
                        context.SaveChanges();
                    }

                    trans.Commit(); Response.Redirect("Index");
                }
            }
            catch (Exception ex)
            {
                var em = ex.Message;
                Response.Redirect("/Error/AccessDenied");
            }


        }
        #endregion

        #region Getdetails from  vt
        public JsonResult GetVehicleDetails(int id)
        {
            //var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where GIDID=" + id).ToList();
            var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where VTNO=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Detail(int id)
        {
            var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where GIDID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetailEdit(int id)
        {
            var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GIDID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region VtMaxDate
        public JsonResult VtMaxDate(string id)
        {
            int GIDID = 0;

            if (id != "" || id != null)
            {
                GIDID = Convert.ToInt32(id);
            }

            var data = (from q in context.vehicleticketdetail
                        where q.GIDID == GIDID && q.SDPTID == 2
                        group q by q.VTDATE into g
                        select new { VTDATE = g.Max(t => t.VTDATE) }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PrintView
        [Authorize(Roles = "ExportGateOutPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORTGATEOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_GateOut.RPT");
                //cryRpt.Load("D:\\CFSReports\\Export_GateOut.RPT");

                cryRpt.RecordSelectionFormula = "{VW_EXPORT_GATE_Out_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_GATE_Out_PRINT_ASSGN.GODID} = " + id;

                crConnectionInfo.ServerName = stringbuilder.DataSource;
                crConnectionInfo.DatabaseName = stringbuilder.InitialCatalog;
                crConnectionInfo.UserID = stringbuilder.UserID;// "ftec";
                crConnectionInfo.Password = stringbuilder.Password; //"ftec";

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

        #region delete
        [Authorize(Roles = "ExportGateOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split(';');
            String temp = Delete_fun.delete_check1(fld, param[1]);

            if (temp.Equals("PROCEED"))
            {
                GateOutDetail gateoutdetail = context.gateoutdetail.Find(Convert.ToInt32(param[0]));
                context.gateoutdetail.Remove(gateoutdetail);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}