using scfs.Data;
using scfs_erp.Context;
using scfs_erp.Helper;
using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;

namespace scfs_erp.Controllers.Export
{
    [SessionExpire]
    public class ExportLoadSlipController : Controller
    {
        // GET: ExportLoadSlip
        #region Context Declaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Page
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

        #region GET AJAX DATA
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new CFSExportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));

                var data = e.pr_Search_Export_LoadSlip_Master(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                    totalRowsCount, filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));
                var aaData = data.Select(d => new string[] { d.ASLMDATE.ToString("dd/MM/yyyy"), d.ASLMDNO.ToString(), d.CONTNRNO, d.CONTNRSDESC, d.CHANAME, d.CSEALNO, d.ASEALNO, d.DISPSTATUS.ToString(), d.ASLMID.ToString(), d.ASLDID.ToString() }).ToArray();
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
                if ((rslt.Sno == 1) && (rslt.Descriptn == "EXPORT - LOADSLIP"))
                {

                    @ViewBag.Total20 = rslt.c_20;
                    @ViewBag.Total40 = rslt.c_40;
                    @ViewBag.Total45 = rslt.c_45;
                    @ViewBag.TotalTues = rslt.c_tues;

                    Session["EL20"] = rslt.c_20;
                    Session["EL40"] = rslt.c_40;
                    Session["EL45"] = rslt.c_45;
                    Session["ELTU"] = rslt.c_tues;

                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Redirect to Edit Form
        [Authorize(Roles = "ExportLoadSlipEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ExportLoadSlip/Form/" + id);
        }
        #endregion

        #region Create/Edit Form    
        [Authorize(Roles = "ExportLoadSlipCreate")]
        public ActionResult Form(int id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            AuthorizationSlipMaster tab = new AuthorizationSlipMaster();
            AuthorizationSlipMD vm = new AuthorizationSlipMD();


            List<SelectListItem> selectedTYPE = new List<SelectListItem>();
            SelectListItem selectedItemDSP = new SelectListItem { Text = "LOAD", Value = "1", Selected = true };
            selectedTYPE.Add(selectedItemDSP);
            ViewBag.ASLMTYPE = selectedTYPE;

            var sqlcn = context.Database.SqlQuery<VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEW>("select * from VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEW").ToList();

            ViewBag.SLTID = new SelectList(context.Export_SealTypeMasters, "SLTID", "SLTDESC",0);

            ViewBag.STFDID = sqlcn.Select(m => new SelectListItem()
            {
                Text = m.CONTNRNO,
                Value = m.STFDID.ToString()
            });

            if (id != 0)
            {
                tab = context.authorizatioslipmaster.Find(id);//find selected record

                vm.masterdata = context.authorizatioslipmaster.Where(det => det.ASLMID == id).ToList();
                vm.detaildata = context.authorizationslipdetail.Where(det => det.ASLMID == id).ToList();

                ViewBag.SLTID = new SelectList(context.Export_SealTypeMasters, "SLTID", "SLTDESC", 0);

                vm.viewdetail = context.Database.SqlQuery<VW_EXPORT_ASL_DETAIL_CTRL_ASSGN_MODS>("select *from  VW_EXPORT_ASL_DETAIL_CTRL_ASSGN_MODS  WHERE ASLMID =" + id).ToList();
                //vm.viewdetail = context.VW_EXPORT_ASL_DETAIL_CTRL_ASSGN_MOD.Where(det => det.ASLMID == id).ToList();
            }
            //ViewBag.STFDID = new SelectList(context.VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEWs, "STFDID", "CONTNRNO");
            return View(vm);
        }
        #endregion

        #region Savedata
        public void savedata(FormCollection F_Form)
        {
            using (SCFSERPContext context = new SCFSERPContext())
            {
                using (var trans1 = context.Database.BeginTransaction())
                {
                    try
                    {
                        AuthorizationSlipMaster authorizatioslipmaster = new AuthorizationSlipMaster();
                        AuthorizationSlipDetail authorizatioslipdetail = new AuthorizationSlipDetail();
                        //-------Getting Primarykey field--------
                        Int32 ASLMID = 0;

                        string aslmId = Convert.ToString(F_Form["masterdata[0].ASLMID"]);
                        if (aslmId == "" || aslmId == null)
                        {
                            ASLMID = 0;
                        }
                        else
                        {
                            ASLMID = Convert.ToInt32(aslmId);
                        }

                        Int32 ASLDID = 0;
                        string DELIDS = "";

                        if (ASLMID != 0)
                        {
                            authorizatioslipmaster = context.authorizatioslipmaster.Find(ASLMID);
                        }

                        authorizatioslipmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        authorizatioslipmaster.SDPTID = 2;
                        authorizatioslipmaster.ASLMTYPE = 1;
                        if (Session["CUSRID"] != null)
                            authorizatioslipmaster.CUSRID = Session["CUSRID"].ToString();
                        //   authorizatioslipmaster.CUSRID = "admin";
                        authorizatioslipmaster.LMUSRID = Session["CUSRID"].ToString();
                        authorizatioslipmaster.DISPSTATUS = 0;
                        authorizatioslipmaster.PRCSDATE = DateTime.Now;

                        string indate = Convert.ToString(F_Form["masterdata[0].ASLMDATE"]);
                        if (indate != null || indate != "")
                        {
                            authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(indate).Date;
                        }
                        else { authorizatioslipmaster.ASLMDATE = DateTime.Now.Date; }

                        string intime = Convert.ToString(F_Form["masterdata[0].ASLMTIME"]);
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

                                    authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(in_datetime);
                                }
                                else { authorizatioslipmaster.ASLMTIME = DateTime.Now; }
                            }
                            else { authorizatioslipmaster.ASLMTIME = DateTime.Now; }
                        }
                        else { authorizatioslipmaster.ASLMTIME = DateTime.Now; }

                        //authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(F_Form["masterdata[0].ASLMDATE"]).Date;
                        //authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]);
                        if (ASLMID == 0)
                        {
                            authorizatioslipmaster.ASLMNO = Convert.ToInt32(Autonumber.autonum("AUTHORIZATIONSLIPMASTER", "ASLMNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " AND SDPTID=2 AND ASLMTYPE=1").ToString());

                            int ano = authorizatioslipmaster.ASLMNO;
                            string prfx = string.Format("{0:D5}", ano);
                            authorizatioslipmaster.ASLMDNO = prfx.ToString();
                            context.authorizatioslipmaster.Add(authorizatioslipmaster);
                            context.SaveChanges();
                        }
                        else
                        {
                            context.Entry(authorizatioslipmaster).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                        }

                        //-------------Shipping Bill Details
                        string[] F_ASLDID = F_Form.GetValues("ASLDID");
                        string[] GIDID = F_Form.GetValues("GIDID");
                        string[] STFDID = F_Form.GetValues("STFDID");
                        string[] VHLNO = F_Form.GetValues("VHLNO");
                        string[] DRVNAME = F_Form.GetValues("DRVNAME");
                        string[] CSEALNO = F_Form.GetValues("CSEALNO");
                        string[] SLTID = F_Form.GetValues("SLTID");

                        string[] ASEALNO = F_Form.GetValues("ASEALNO");

                        for (int count = 0; count < F_ASLDID.Count(); count++)
                        {
                            ASLDID = Convert.ToInt32(F_ASLDID[count]);
                            if (ASLDID != 0)
                            {
                                authorizatioslipdetail = context.authorizationslipdetail.Find(ASLDID);
                            }

                            authorizatioslipdetail.ASLMID = authorizatioslipmaster.ASLMID;
                            authorizatioslipdetail.OSDID = 0;
                            authorizatioslipdetail.LCATEID = 0;
                            authorizatioslipdetail.ASLDTYPE = 3;
                            authorizatioslipdetail.ASLLTYPE = 2;
                            authorizatioslipdetail.ASLOTYPE = 1;
                            authorizatioslipdetail.STFDID = Convert.ToInt32(STFDID[count]);
                            authorizatioslipdetail.VHLNO = VHLNO[count].ToString();
                            authorizatioslipdetail.DRVNAME = DRVNAME[count].ToString();
                            authorizatioslipdetail.CSEALNO = CSEALNO[count].ToString();
                            authorizatioslipdetail.SLTID = Convert.ToInt32(SLTID[count]);
                            authorizatioslipdetail.ASEALNO = ASEALNO[count].ToString();
                            authorizatioslipdetail.ASLDODATE = DateTime.Now;
                            authorizatioslipdetail.DISPSTATUS = 0;
                            authorizatioslipdetail.PRCSDATE = DateTime.Now;

                            if (ASLDID == 0)
                            {
                                context.authorizationslipdetail.Add(authorizatioslipdetail);
                                context.SaveChanges();
                                ASLDID = authorizatioslipdetail.ASLDID;
                            }
                            else
                            {
                                context.Entry(authorizatioslipdetail).State = System.Data.Entity.EntityState.Modified;
                                context.SaveChanges();
                            }

                            DELIDS = DELIDS + "," + ASLDID.ToString();

                        }
                        context.Database.ExecuteSqlCommand("DELETE FROM authorizationslipdetail  WHERE ASLMID=" + ASLMID + " and  ASLDID NOT IN(" + DELIDS.Substring(1) + ")");
                        trans1.Commit();
                        Response.Redirect("Index");
                    }
                    catch (Exception ex)
                    {
                        var ermsg = ex.Message;
                        //Response.Write(ex.Message.ToString());
                        trans1.Rollback();
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }

            //Response.Redirect("Index");
        }
        #endregion

        #region Display Containers
        public JsonResult Container()
        {
            var query = context.Database.SqlQuery<VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEW>("select * from VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEW").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Display Details
        public JsonResult Slipdetails(int id)
        {
            //  var STFDID = Convert.ToInt32(id);
            var query = context.Database.SqlQuery<VW_STUFF_GATEIN_CTRL_ASSGN>("select * from VW_STUFF_GATEIN_CTRL_ASSGN where STFDID=" + id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region detaild edit
        public JsonResult det_edit(int id)
        {
            var sqlcn = context.Database.SqlQuery<VW_STUFF_GATEIN_CTRL_ASSGN>("select * from VW_STUFF_GATEIN_CTRL_ASSGN where STFDID=" + id).ToList();
            return Json(sqlcn[0].CONTNRNO, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region StuffMaxDate
        public ActionResult StuffMaxDate(string id)
        {
            int STFDID = 0;

            if (id != "" || id != null)
            {
                STFDID = Convert.ToInt32(id);
            }

            var result = (from a in context.stuffingdetails
                          join b in context.stuffingmasters on a.STFMID equals b.STFMID
                          where a.STFDID == STFDID
                          group b by b.STFMDNO into g
                          select new { STFMDATE = g.Max(t => t.STFMDATE) }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StuffEMaxDate(string id)
        {
            int STFDID = 0;

            if (id != "" || id != null)
            {
                STFDID = Convert.ToInt32(id);
            }

            var result = (from a in context.stuffingmasters
                          join b in context.stuffingdetails on a.STFMID equals b.STFMID 
                          join c in context.authorizationslipdetail on b.STFDID equals c.STFDID 
                          join d in context.authorizatioslipmaster on c.ASLMID equals d.ASLMID 
                          where d.ASLMID == STFDID
                          group a by a.STFMDNO into g
                          select new { STFMDATE = g.Max(t => t.STFMDATE) }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Containerno auto complete
        public JsonResult AutoContainer(string term)
        {           

            string sqry1 = "select distinct CONTNRNO,STFDID  from VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEW WHERE CONTNRNO LIKE '%" + term + "%'  ";

            var result = context.Database.SqlQuery<VW_EXPORT_ASL_STUFF_CONTAINER_CBX_ASSGN_NEW>(sqry1).ToList();
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Print view       
        [Authorize(Roles = "ExportLoadSlipPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EXPORTLOADSLIP", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Export_LoadSlip.RPT");
                cryRpt.RecordSelectionFormula = "{VW_EXPORT_LOADSLIP_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_EXPORT_LOADSLIP_PRINT_ASSGN.ASLMID} = " + id;

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

        #region Delete Records       
        [Authorize(Roles = "ExportLoadSlipDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = delet.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(id));
                context.authorizatioslipmaster.Remove(authorizatioslipmaster);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);
        }
        #endregion
    }
}
