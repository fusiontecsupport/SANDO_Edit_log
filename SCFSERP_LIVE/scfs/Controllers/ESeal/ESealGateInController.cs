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
    public class ESealGateInController : Controller
    {
        // GET: ESealGateIn

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        #endregion

        #region Indexform
        [Authorize(Roles = "ESealGateInIndex")]
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
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)/*model 22.edmx*/
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_ESeal_GateIn(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = data.Select(d => new string[] { d.GIDATE.Value.ToString("dd/MM/yyyy"), d.GITIME.Value.ToString("hh:mm tt"), d.GIDNO, d.CHANAME, d.EXPORTER, d.CONTNRNO, d.CONTNRSID, d.VHLNO, d.DRVNAME, d.DISPSTATUS, d.GIDID.ToString(), d.Edchk }).ToArray();
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

        #region Editpage
        [Authorize(Roles = "ESealGateInEdit")]
        public void Edit(int id)
        {
            Response.Redirect("~/ESealGateIn/ESGIForm/" + id);
           
        }
        #endregion

        #region Details Form
        [Authorize(Roles = "ESealGateInCreate")]
        public ActionResult ESGIForm(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInDetail tab = new GateInDetail();
            if (id == 0)
            {
                tab.GIDID = 0;
                tab.GIDATE = DateTime.Now.Date;
                tab.GITIME = DateTime.Now;
                tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);
                tab.ESBDATE = DateTime.Now;
                tab.GICCTLTIME = DateTime.Now;
                //--------------------Dropdown list------------------------------------//           

                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID != 1 && m.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC");
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC");

                //-------------------------------DISPSTATUS----

                List<SelectListItem> selectedDISPSTATUS = new List<SelectListItem>();
                SelectListItem selectedItemDSP = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = true };
                selectedDISPSTATUS.Add(selectedItemDSP);
                selectedItemDSP = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = false };
                selectedDISPSTATUS.Add(selectedItemDSP);
                ViewBag.DISPSTATUS = selectedDISPSTATUS;
            }
            else
            {
                tab = context.gateindetails.Find(id);

                ViewBag.CONTNRSID = new SelectList(context.containersizemasters.Where(m => m.CONTNRSID != 1 && m.DISPSTATUS == 0), "CONTNRSID", "CONTNRSDESC", tab.CONTNRSID);
                ViewBag.CONTNRTID = new SelectList(context.containertypemasters.Where(x => x.DISPSTATUS == 0).OrderBy(x => x.CONTNRTDESC), "CONTNRTID", "CONTNRTDESC", tab.CONTNRTID);
                //-------------------------DISPSTATUS----------------------------
                List<SelectListItem> selectedDISPSTATUS1 = new List<SelectListItem>();
                if (Convert.ToInt32(tab.DISPSTATUS) != 0)
                {
                    SelectListItem selectedItem31 = new SelectListItem { Text = "INBOOKS", Value = "0", Selected = false };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    selectedItem31 = new SelectListItem { Text = "CANCELLED", Value = "1", Selected = true };
                    selectedDISPSTATUS1.Add(selectedItem31);
                    
                }
                ViewBag.DISPSTATUS = selectedDISPSTATUS1;
            }


            //if (id != 0)
            //{
               
            //}
            return View(tab);
        }
        #endregion

        #region Savedata
        [HttpPost]
        public ActionResult Savedata(GateInDetail tab)
        {
            string status = "";
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    string todaydt = Convert.ToString(DateTime.Now);
                    string todayd = Convert.ToString(DateTime.Now.Date);

                    tab.COMPYID = Convert.ToInt32(Session["compyid"]);
                    tab.SDPTID = 11;
                    tab.PRCSDATE = DateTime.Now;

                    string indate = Convert.ToString(tab.GIDATE);
                    if (indate != null || indate != "")
                    {
                        tab.GIDATE = Convert.ToDateTime(indate).Date;
                    }
                    else { tab.GIDATE = DateTime.Now.Date; }

                    if (tab.GIDATE > Convert.ToDateTime(todayd))
                    {
                        tab.GIDATE = Convert.ToDateTime(todayd);
                    }

                    string intime = Convert.ToString(tab.GITIME);
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

                                tab.GITIME = Convert.ToDateTime(in_datetime);
                            }
                            else { tab.GITIME = DateTime.Now; }
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

                                tab.GITIME = Convert.ToDateTime(in_datetime);
                            }
                            else { tab.GITIME = DateTime.Now; }
                        }
                    }
                    else { tab.GITIME = DateTime.Now; }

                    if (tab.GITIME > Convert.ToDateTime(todaydt))
                    {
                        tab.GITIME = Convert.ToDateTime(todaydt);
                    }

                    //tab.GITIME = Convert.ToDateTime(tab.GITIME);
                    //tab.GITIME = new DateTime(tab.GIDATE.Year, tab.GIDATE.Month, tab.GIDATE.Day, tab.GITIME.Hour, tab.GITIME.Minute, tab.GITIME.Second);

                    tab.GICCTLDATE = null;
                    tab.GICCTLTIME = DateTime.Now;
                    tab.CONTNRID = 1;
                    tab.YRDID = 1;
                    tab.GIVHLTYPE = 0;
                    //tab.TRNSPRTID = 0;
                    //tab.TRNSPRTNAME = "-";
                    tab.GPREFNO = "-";
                    tab.IMPRTID = 0;
                    tab.IMPRTNAME = "-";
                    //tab.STMRID = 0;
                    //tab.STMRNAME = "-";
                    //tab.CONTNRTID = 0;
                    tab.YRDID = 0;
                    tab.VSLID = 0;
                    tab.VSLNAME = "-";
                    tab.VOYNO = "-";
                    tab.PRDTGID = 0;
                    tab.PRDTDESC = "-";

                    tab.UNITID = 0;
                    tab.GPPTYPE = 0;
                    tab.GPMODEID = 0;
                    tab.BCHAID = 0;
                    tab.BCHANAME = "-";


                    string VHLMID = tab.VHLMID.ToString();

                    if (VHLMID == "" || VHLMID == null)
                    { tab.VHLMID = 0; }
                    else { tab.VHLMID = Convert.ToInt32(VHLMID); }

                    tab.AVHLNO = tab.VHLNO;
                    tab.ESBDATE = DateTime.Now;
                    tab.BOEDATE = DateTime.Now.Date;
                    //tab.DISPSTATUS = 0; //tab.DISPSTATUS;

                    tab.LMUSRID = Session["CUSRID"].ToString();

                    if (tab.CUSRID == "" || tab.CUSRID == null)
                    {
                        if (Session["CUSRID"] != null)
                        {
                            tab.CUSRID = Session["CUSRID"].ToString();
                        }
                        else { tab.CUSRID = "0"; }
                    }

                    if (tab.GIDID.ToString() != "0")
                    {
                        context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        status = "Success";
                    }
                    else
                    {
                        //var sqr = context.Database.SqlQuery<GateInDetail>("select *from GATEINDETAIL where SDPTID=11 AND CONTNRNO='" + tab.CONTNRNO.ToString() + "'").ToList();
                        var sqr = context.Database.SqlQuery<PR_ES_GATEIN_CONTAINER_CHK_ASSGN_001_Result>("Exec PR_ES_GATEIN_CONTAINER_CHK_ASSGN_001 @PCONTNRNO='" + tab.CONTNRNO.ToString() + "'").ToList();

                        if (sqr.Count > 0)
                        {
                            status = "Exists";
                        }
                        else
                        {
                            tab.GINO = Convert.ToInt32(Autonumber.autonum("gateindetail", "GINO", "GINO <> 0 AND SDPTID = 11 and compyid = " + Convert.ToInt32(Session["compyid"]) + "").ToString());
                            int ano = tab.GINO;
                            string prfx = string.Format("{0:D5}", ano);
                            tab.GIDNO = prfx.ToString();

                            context.gateindetails.Add(tab);
                            context.SaveChanges();
                            status = "Success";
                        }
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

        #region Autocomplete Cha Name
        public JsonResult AutoChaName(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 4).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Expoter Autocomplete
        public JsonResult AutoExporter(string term)
        {
            var result = (from r in context.categorymasters.Where(m => m.CATETID == 2).Where(x => x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete transporter Name
        public JsonResult AutoTransporter(string term)
        {
            var result = (from r in context.categorymasters.Where(x => x.CATETID == 5 && x.DISPSTATUS == 0)
                          where r.CATENAME.ToLower().Contains(term.ToLower())
                          select new { r.CATENAME, r.CATEID }).OrderBy(x => x.CATENAME).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Steamer Name
        public JsonResult AutoSteamer(string term)
        {
            var result = (from category in context.categorymasters.Where(m => m.CATETID == 3).Where(x => x.DISPSTATUS == 0)
                          where category.CATENAME.ToLower().Contains(term.ToLower())
                          select new { category.CATENAME, category.CATEID }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete Vehicle PNR On  Id  
        public JsonResult AutoVehicleNo(string term)
        {
            string vno = ""; int Tid = 0;
            var Param = term.Split(';');

            if (Param[0] != "" || Param[0] != null) { vno = Convert.ToString(Param[0]); } else { vno = ""; }
            if (Param[1] != "" || Param[1] != null) { Tid = Convert.ToInt32(Param[1]); } else { Tid = 0; }


            var result = (from vehicle in context.vehiclemasters.Where(m => m.DISPSTATUS == 0 && m.TRNSPRTID == Tid)
                          where vehicle.VHLMDESC.ToLower().Contains(vno.ToLower())
                          select new { vehicle.VHLMDESC, vehicle.VHLMID }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Print Gate In
        [Authorize(Roles = "ESealGateInPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "ESEALGATEIN", Convert.ToInt32(id), Session["CUSRID"].ToString());

            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Eseal_GateIn.rpt");
                cryRpt.RecordSelectionFormula = "{VW_ESEAL_GATE_IN_PRINT_ASSGN.KUSRID} ='" + Session["CUSRID"].ToString() + "' and {VW_ESEAL_GATE_IN_PRINT_ASSGN.GIDID} =" + id;

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

        #region Delete E seal Gatein
        [Authorize(Roles = "ESealGateInDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {
                GateInDetail gateindetails = context.gateindetails.Find(Convert.ToInt32(id));
                context.gateindetails.Remove(gateindetails);
                context.SaveChanges();
                Response.Write("Deleted Successfully ...");
            }
            else
                Response.Write(temp);

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

            context.Database.CommandTimeout = 0;
            var result = context.Database.SqlQuery<PR_IMPORT_DASHBOARD_DETAILS_Result>("EXEC PR_ESEAL_DASHBOARD_DETAILS @PFDT='" + fdate + "',@PTDT='" + tdate + "',@PSDPTID=" + 11).ToList();

            foreach (var rslt in result)
            {
                if ((rslt.Sno == 1) && (rslt.Descriptn == "IMPORT - GATEIN"))
                {
                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["GI20"] = rslt.c_20;
                    Session["GI40"] = rslt.c_40;
                    Session["GI45"] = rslt.c_45;
                    Session["GITU"] = rslt.c_tues;
                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        public void CONT_Duplicate_Check(string CONTNRNO, string date)
        {
            CONTNRNO = Request.Form.Get("CONTNRNO");
            date = Request.Form.Get("GIDATE");

            string temp = ContainerNo_Check.esrecordCount(CONTNRNO);
            if (temp != "PROCEED")
            {
                Response.Write("Container number already exists");
            }
            else
            {
                var query = context.Database.SqlQuery<PR_ES_GATEIN_CONTAINER_CHK_ASSGN_001_Result>("Exec PR_ES_GATEIN_CONTAINER_CHK_ASSGN_001 @PCONTNRNO='" + CONTNRNO + "'").ToList();
                if (query.Count > 0)
                {
                    DateTime gedate = Convert.ToDateTime(query[0].GEDATE);
                    DateTime gidate = Convert.ToDateTime(date);
                    var s = (gedate - gidate).Days;
                    //   Response.Write(s);
                    //if (s != 0)
                    //{
                    //    Response.Write("DATE INCORRECT");
                    //}
                    if (gidate >= gedate)
                    {
                        Response.Write("PROCEED");
                    }
                    else
                    {
                        Response.Write("DATE INCORRECT");
                    }
                    //if (s < 10)
                    //{
                    //    Response.Write("DATE INCORRECT");
                    //}
                }
                else
                {
                    Response.Write("PROCEED");
                }
            }

        }

    }
}