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

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportGateOutController : Controller
    {
        // GET: ImportGateOut
        #region Context declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Page
        [Authorize(Roles = "ImportGateOutIndex")]
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


            var result = context.Database.SqlQuery<PR_IMPORT_DASHBOARD_DETAILS_Result>("EXEC PR_IMPORT_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 1).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 2) && (rslt.Descriptn == "IMPORT - GATEOUT"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["IGO20"] = rslt.c_20;
                    Session["IGO40"] = rslt.c_40;
                    Session["IGO45"] = rslt.c_45;
                    Session["IGOTU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region GetAjaxData
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Import_GateOut(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]),
                    Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));

                //var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.IGMNO, d.VSLNAME, d.STMRNAME, d.PRDTDESC.ToString(), d.GIDID.ToString(), d.GODID.ToString() }).ToArray();
                var aaData = data.Select(d => new string[] { d.GODATE.Value.ToString("dd/MM/yyyy"), d.GODNO.ToString(), d.CONTNRNO, d.IGMNO, d.GPLNO, d.CONTNRSCODE, d.ASLMDNO, d.VTTYPE.ToString(), d.CHANAME.ToString(), d.BOENO.ToString(), d.GODID.ToString() }).ToArray();
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

        #region Edit Page
        [Authorize(Roles = "ImportGateOutEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportGateOut/Form/" + id);
        }
        #endregion

        #region FORM 
        [Authorize(Roles = "ImportGateOutCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            tab.GODATE = DateTime.Now.Date;
            tab.GOTIME = DateTime.Now;
            tab.GODID = 0;
            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN>("select * from VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN").ToList(), "GIDID", "CONTNRNO");

            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.GOBTYPE = selectedType;


            if (id != 0)//Edit Mode
            {
                tab = context.gateoutdetail.Find(id);
                List<SelectListItem> selectedType_ = new List<SelectListItem>();
                if (tab.GOBTYPE == 2)
                {

                    SelectListItem selectedItem = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    selectedType_.Add(selectedItem);
                    selectedItem = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedType_.Add(selectedItem);
                    ViewBag.GOBTYPE = selectedType_;
                }

                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();

            }
            return View(tab);

        }


        [Authorize(Roles = "ImportGateOutCreate")]
        public ActionResult CForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateOutDetail tab = new GateOutDetail();
            tab.GODATE = DateTime.Now.Date;
            tab.GOTIME = DateTime.Now;
            tab.GODID = 0;
            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN>("select * from VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN").ToList(), "GIDID", "CONTNRNO");

            //-----------------------------type-----------
            List<SelectListItem> selectedType = new List<SelectListItem>();
            SelectListItem selectedItem1 = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedType.Add(selectedItem1);
            selectedItem1 = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = false };
            selectedType.Add(selectedItem1);
            ViewBag.GOBTYPE = selectedType;


            if (id != 0)//Edit Mode
            {
                tab = context.gateoutdetail.Find(id);
                List<SelectListItem> selectedType_ = new List<SelectListItem>();
                if (tab.GOBTYPE == 2)
                {

                    SelectListItem selectedItem = new SelectListItem { Text = "LOAD", Value = "1", Selected = false };
                    selectedType_.Add(selectedItem);
                    selectedItem = new SelectListItem { Text = "DESTUFF", Value = "2", Selected = true };
                    selectedType_.Add(selectedItem);
                    ViewBag.GOBTYPE = selectedType_;
                }

                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();

            }
            return View(tab);

        }
        #endregion

        #region  Insert/Modify data
        public void savedata(GateOutDetail tab)
        {
            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.SDPTID = 1;
            tab.REGSTRID = 1;
            tab.TRANDID = 0;

            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

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
                    //tab.GOTIME = DateTime.Now; 
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
            }
            else
            {
                tab.CUSRID = Session["CUSRID"].ToString();
                tab.GONO = Convert.ToInt32(Autonumber.autonum("GateOutDetail", "GONO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=1").ToString());
                int ano = tab.GONO;
                string prfx = string.Format("{0:D5}", ano);
                tab.GODNO = prfx.ToString();
                //tab.GODNO = ano.ToString();
                context.gateoutdetail.Add(tab);
                context.SaveChanges();
            }
            Response.Redirect("Index");
        }
        #endregion

        #region ContDetails
        public JsonResult GetVehicleDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN>("SELECT * FROM VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN WHERE isnull(GODID,0) = 0 and  VTSTYPE = 1 And VTNO = " + id).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContDetails(string id)
        {
            var data = context.Database.SqlQuery<VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN>("SELECT * FROM VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN WHERE isnull(GODID,0) = 0 and  VTSTYPE = 1 And CONTNRNO = '" + Convert.ToString(id) + "'").ToList();
            if (data != null)
            {
                if (data[0].OOCDATE != null && data[0].OOCDATE != "")
                    ViewBag.OOCDATE = Convert.ToDateTime(data[0].OOCDATE).ToString("dd/MM/yyyy");
            }
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContDetails_Mod(int id)
        {
            var data = context.Database.SqlQuery<VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN_001>("SELECT * FROM VW_IMPORT_GATEOUT_DETAIL_CTRL_ASSGN_001 WHERE GIDID=" + id + "").ToList();
            if (data.Count>0)
            {
                if (data[0].OOCDATE != null && data[0].OOCDATE != "")
                    ViewBag.OOCDATE = Convert.ToDateTime(data[0].OOCDATE).ToString("dd/MM/yyyy");

            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region VtMaxDate
        public JsonResult VtMaxDate(int id)
        {
            var data = (from q in context.vehicleticketdetail
                        where q.GIDID == id && q.SDPTID == 1
                        group q by q.VTDATE into g
                        select new { VTDATE = g.Max(t => t.VTDATE) }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Printview
        [Authorize(Roles = "ImportGateOutPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "IMPORTGATEOUT", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_GateOut.RPT");
                //cryRpt.Load("D:\\CFSReports\\Import_GateOut.RPT");

                cryRpt.RecordSelectionFormula = "{VW_IMPORT_GATE_Out_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_GATE_Out_PRINT_ASSGN.GODID} = " + id;

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

        #region delete
        [Authorize(Roles = "ImportGateOutDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            var param = id.Split(';');
            String temp = Delete_fun.delete_check1(fld, param[0]);

            if (temp.Equals("PROCEED"))
            {
                GateOutDetail gateoutdetail = context.gateoutdetail.Find(Convert.ToInt32(param[1]));
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