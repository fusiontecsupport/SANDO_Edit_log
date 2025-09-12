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
    public class EmptySlipInController : Controller
    {
        // GET: EmptySlipIn

        #region contextdeclaration
        SCFSERPContext context = new SCFSERPContext();
        #endregion

        #region Index Form
        [Authorize(Roles = "EmptySlipInIndex")]
        public ActionResult  Index()
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

                var data = e.pr_Search_Empty_SlipIn(param.sSearch,
                                                Convert.ToInt32(Request["iSortCol_0"]),
                                                Request["sSortDir_0"],
                                                param.iDisplayStart,
                                                param.iDisplayStart + param.iDisplayLength,
                                                totalRowsCount,
                                                filteredRowsCount, Convert.ToInt32(Session["compyid"]), Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]));

                var aaData = data.Select(d => new string[] { d.ASLMDATE.Value.ToString("dd/MM/yyyy"), d.ASLMDNO, d.CONTNRNO, d.CONTNRSDESC, d.CHANAME, d.VHLNO, d.DISPSTATUS, d.ASLMID.ToString() }).ToArray();

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
        [Authorize(Roles = "EmptySlipInEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/EmptySlipIn/Form/" + id);
        }
        #endregion

        #region Form
        [Authorize(Roles = "EmptySlipInCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            AuthorizationSlipMaster tab = new AuthorizationSlipMaster();
            AuthorizationSlipMD vm = new AuthorizationSlipMD();
            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_ASL_EMPTY_CONTAINER_CBX_ASSGN>("select * from VW_ASL_EMPTY_CONTAINER_CBX_ASSGN"), "GIDID", "CONTNRNO");

            if (id != 0)
            {
                tab = context.authorizatioslipmaster.Find(id);

                vm.masterdata = context.authorizatioslipmaster.Where(x => x.ASLMID == id).ToList();
                vm.detaildata = context.authorizationslipdetail.Where(x => x.ASLMID == id).ToList();
                var gidid = Convert.ToInt32(vm.detaildata[0].OSDID);
                //  var sql = context.Database.SqlQuery<GateInDetail>("select * from Gateindetail where gidid=" + gidid).ToList();

                var sql = (from r in context.gateindetails.Where(x => x.GIDID == gidid)
                           join s in context.containersizemasters on r.CONTNRSID equals s.CONTNRSID
                           select new { r.CONTNRNO, r.GIDATE, r.STMRNAME, s.CONTNRSCODE, r.CHAID, r.CHANAME, r.STMRID }
                              ).ToList();

                if (sql.Count > 0)
                {
                    ViewBag.CONTNRNO = sql[0].CONTNRNO;
                    ViewBag.GIDATE = sql[0].GIDATE;
                    ViewBag.STMRNAME = sql[0].STMRNAME;
                    ViewBag.STMRID = sql[0].STMRID;
                    ViewBag.CHANAME = sql[0].CHANAME;
                    ViewBag.CHAID = sql[0].CHAID;
                    ViewBag.CONTNRSCODE = sql[0].CONTNRSCODE;
                }
            }
            return View(vm);
        }
        #endregion

        #region Auto complete for Container No
        public JsonResult AutoContainer(string term)
        {
          
            var result = context.Database.SqlQuery<VW_EMPTY_CONTAINERNO_AUTOASSGN>("Select *From VW_EMPTY_CONTAINERNO_AUTOASSGN  Where  CONTNRNO Like '%" + term + "%'").ToList();

            return Json(result, JsonRequestBehavior.AllowGet);           
        }
        #endregion

        #region GetContDetails
        public JsonResult GetContDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_ASL_EMPTY_CONTAINER_CBX_ASSGN>("SELECT * FROM VW_ASL_EMPTY_CONTAINER_CBX_ASSGN WHERE GIDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Getcont
        public JsonResult Getcont(string id)
        {
            var param = id.Split(';');
            var data = context.Database.SqlQuery<VW_ASL_EMPTY_CONTAINER_CBX_ASSGN>("SELECT * FROM VW_ASL_EMPTY_CONTAINER_CBX_ASSGN WHERE IGMNO='" + param[0] + "' and GPLNO='" + param[1] + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region savedata
        public void savedata(FormCollection F_Form)
        {
            using (context = new SCFSERPContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        AuthorizationSlipMaster authorizatioslipmaster = new AuthorizationSlipMaster();
                        AuthorizationSlipDetail authorizatioslipdetail = new AuthorizationSlipDetail();

                        //-------Getting Primarykey field--------
                        Int32 ASLMID = Convert.ToInt32(F_Form["masterdata[0].ASLMID"]);
                        Int32 ASLDID = Convert.ToInt32(F_Form["detaildata[0].ASLDID"]);
                        //string DELIDS = "";
                        //-----End

                        if (ASLMID != 0)
                        {
                            authorizatioslipmaster = context.authorizatioslipmaster.Find(ASLMID);
                        }

                        authorizatioslipmaster.COMPYID = Convert.ToInt32(Session["compyid"]);
                        authorizatioslipmaster.SDPTID = 1;
                        authorizatioslipmaster.ASLMTYPE = 3;
                        authorizatioslipmaster.CUSRID = Session["CUSRID"].ToString();                        
                        authorizatioslipmaster.LMUSRID = Session["CUSRID"].ToString();
                        authorizatioslipmaster.DISPSTATUS = Convert.ToInt16(F_Form["DISPSTATUS"]);
                        authorizatioslipmaster.PRCSDATE = DateTime.Now;

                        //authorizatioslipmaster.ASLMDATE = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]).Date;
                        //authorizatioslipmaster.ASLMTIME = Convert.ToDateTime(F_Form["masterdata[0].ASLMTIME"]);

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

                        if (ASLMID == 0)
                        {

                            authorizatioslipmaster.ASLMNO = Convert.ToInt32(Autonumber.autonum("AUTHORIZATIONSLIPMASTER", "ASLMNO", "COMPYID=" + Convert.ToInt32(Session["compyid"]) + " AND SDPTID=1 AND ASLMTYPE=3").ToString());

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

                        //-------------slip Details
                        if (ASLDID != 0)
                        {
                            authorizatioslipdetail = context.authorizationslipdetail.Find(ASLDID);
                        }

                        authorizatioslipdetail.ASLMID = authorizatioslipmaster.ASLMID;
                        authorizatioslipdetail.OSDID = Convert.ToInt32(F_Form["GIDID"]);
                        authorizatioslipdetail.LCATEID = 1;
                        authorizatioslipdetail.ASLDTYPE = 3;
                        authorizatioslipdetail.ASLLTYPE = 2;
                        authorizatioslipdetail.ASLOTYPE = 1;
                        authorizatioslipdetail.STFDID = 0;
                        authorizatioslipdetail.VHLNO = Convert.ToString(F_Form["detaildata[0].VHLNO"]);
                        authorizatioslipdetail.DRVNAME = Convert.ToString(F_Form["detaildata[0].DRVNAME"]);
                        authorizatioslipdetail.ASLFDESC = Convert.ToString(F_Form["detaildata[0].ASLFDESC"]);
                        authorizatioslipdetail.ASLTDESC = Convert.ToString(F_Form["detaildata[0].ASLTDESC"]);
                        authorizatioslipdetail.ASLDODATE = Convert.ToDateTime(F_Form["detaildata[0].ASLDODATE"]);
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

                        trans.Commit(); Response.Redirect("Index");

                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        //Response.Write("Sorry!!An Error Ocurred...");
                        Response.Redirect("/Error/AccessDenied");
                    }
                }
            }
        }
        #endregion

        #region PrintView
        [Authorize(Roles = "EmptySlipInPrint")]
        public void PrintView(int? id = 0)
        {
            String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
            SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "EMPTYSLIP", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;

                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_Empty_ADSLIP.RPT");

                cryRpt.RecordSelectionFormula = "{VW_IMPORT_EMPTY_ADSLIP_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_EMPTY_ADSLIP_PRINT_ASSGN.ASLMID} = " + id;

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

        #region Del
        [Authorize(Roles = "EmptySlipInDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);
            if (temp.Equals("PROCEED"))
            {

                AuthorizationSlipMaster authorizatioslipmaster = context.authorizatioslipmaster.Find(Convert.ToInt32(id));
                context.authorizatioslipmaster.Remove(authorizatioslipmaster);
                context.SaveChanges();

                Response.Write("Deleted Successfully ...");
            }
            else

                Response.Write(temp);

        }
        #endregion
    }
}