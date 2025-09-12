using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using scfs.Data;
using scfs_erp;

namespace scfs_erp.Controllers.ESeal
{
    [SessionExpire]
    public class ESealGateOutController : Controller
    {
        // GET: ESealGateOut

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Indexform
        [Authorize(Roles = "ESealGOIndex")]
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

        #region Getdata for index       
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ESeal_GateOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]),
                    Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));

                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.CHANAME.ToString(), d.VHLNO.ToString(), d.GDRVNAME.ToString(), d.GIDID.ToString(), d.GODID.ToString(), d.Edchk }).ToArray();
                //var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.CHANAME.ToString(), d.VHLNO.ToString(), d.GDRVNAME.ToString(), d.GODID.ToString() }).ToArray();
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
        
        #region Redirect  Form    
        [Authorize(Roles = "ESealGOEdit")]
        public void Edit(string id)
        {
            Response.Redirect("/ESealGateOut/ESGOForm/" + id);
        }
        #endregion

        #region ESeal Gate Out Form
        [Authorize(Roles = "ESealGOCreate")]
        public ActionResult ESGOForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            
            tab.GODID = 0;
            tab.GODATE = DateTime.Now;
            tab.GOTIME = DateTime.Now;
            //ViewBag.GIDID = new SelectList(context.VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN, "GIDID", "CONTNRNO");
            ViewBag.GIDID = new SelectList("");
            
            ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC");

            
            if (id != 0)//Edit Mode
            {

                //var query = context.Database.SqlQuery<VW_EXPORT_GATEOUT_MOD_ASSGN>("select * from VW_EXPORT_GATEOUT_MOD_ASSGN where GODID=" + id).ToList();
                tab = context.gateoutdetail.Find(id);

                //ViewBag.GOOTYPE = new SelectList(context.export_Gateout_Gootypes, "GOOTYPE", "GOOTYPEDESC", tab.GOOTYPE);

                ViewBag.GOOTYPE = new SelectList(context.Export_OperationTypeMaster.OrderBy(m => m.EOPTDESC), "EOPTID", "EOPTDESC", tab.GOOTYPE);

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();                

            }
            return View(tab);
        }
        #endregion

        #region Getdetails from  vt
        public JsonResult GetVehicleDetails(int id)
        {
            //var query = context.Database.SqlQuery<VW_ESEAL_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_ESEAL_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where GIDID=" + id).ToList();
            var query = context.Database.SqlQuery<VW_ESEAL_GATEOUT_CONTAINER_CBX_CHNG_ASSGN>("select * from VW_ESEAL_GATEOUT_CONTAINER_CBX_CHNG_ASSGN where VTNO=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetailEdit(int id)
        {
            var query = context.Database.SqlQuery<VW_ESEAL_GATEOUT_MOD_ASSGN>("select * from VW_ESEAL_GATEOUT_MOD_ASSGN where GODID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region E seal Gate out Save
        [HttpPost]
        public ActionResult EGOSaveData(GateOutDetail tab)
        {
            string status = "";
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    string todaydt = Convert.ToString(DateTime.Now);
                    string todayd = Convert.ToString(DateTime.Now.Date);

                    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                    tab.SDPTID = 11;
                    tab.REGSTRID = 1;
                    tab.TRANDID = 0;
                    //tab.GOTIME = DateTime.Now;
                    //tab.GODATE = tab.GOTIME.Date;

                    tab.LMUSRID = Session["CUSRID"].ToString();
                    tab.PRCSDATE = DateTime.Now;
                    tab.EHIDATE = DateTime.Now;
                    tab.EHITIME = DateTime.Now;

                    string indate = Convert.ToString(tab.GODATE);
                    if (indate != null || indate != "")
                    {
                        tab.GODATE = Convert.ToDateTime(indate).Date;
                    }
                    else { tab.GODATE = DateTime.Now.Date; }

                    if (tab.GODATE > Convert.ToDateTime(todayd))
                    {
                        tab.GODATE = Convert.ToDateTime(todayd);
                    }

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

                    if (tab.GOTIME > Convert.ToDateTime(todaydt))
                    {
                        tab.GOTIME = Convert.ToDateTime(todaydt);
                    }

                    if ((tab.GODID).ToString() != "0")
                    {
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        status = "Success";
                    }
                    else
                    {
                        tab.CUSRID = Session["CUSRID"].ToString();
                        tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=11").ToString());
                        int ano = tab.GONO;
                        string prfx = string.Format("{0:D5}", ano);
                        tab.GODNO = prfx.ToString();
                        //tab.GODNO = ano.ToString();
                        context.gateoutdetail.Add(tab);
                        context.SaveChanges();

                        status = "Success";
                    }

                    trans.Commit();
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
               
            }
            catch (Exception ex)
            {
                status = ex.Message.ToString();                
            }
            
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Printview     
        [Authorize(Roles = "ESealGOPrint")]
        public void PrintView(int id)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "ESEALGATEOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "ESeal_GateOut.RPT");
                cryRpt.RecordSelectionFormula = "{VW_ESEAL_GATEOUT_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_ESEAL_GATEOUT_PRINT_ASSGN.GODID} = " + id;

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

        #region Delete Record  
        [Authorize(Roles = "ESealGODelete")]
        public void Del()
        {

            String id = Request.Form.Get("id");

            String GIDID = "0";
            int GODID = 0;

            if (id.Contains(";"))
            {
                var ids = id.Split(';');
                GIDID = Convert.ToString(ids[0]);
                GODID = Convert.ToInt32(ids[1]);
            }
            

            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, GIDID);
            if (temp.Equals("PROCEED"))
            {
                GateOutDetail esealgateout = context.gateoutdetail.Find(Convert.ToInt32(GODID));
                context.gateoutdetail.Remove(esealgateout);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);

        }
        #endregion
    }
}