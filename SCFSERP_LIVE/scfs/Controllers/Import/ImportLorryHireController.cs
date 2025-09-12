using scfs.Data;
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

namespace scfs_erp.Controllers.Import
{
    [SessionExpire]
    public class ImportLorryHireController : Controller
    {
        //
        // GET: /ImportLorryHire/
        SCFSERPContext context = new SCFSERPContext();
        public static String constring = ConfigurationManager.ConnectionStrings["SCFSERP"].ConnectionString;
        SqlConnectionStringBuilder stringbuilder = new SqlConnectionStringBuilder(constring);
        [Authorize(Roles = "ImportLorryHireIndex")]
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
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {
            using (var e = new  CFSImportEntities())
            {
                var totalRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("TotalRowsCount", typeof(int));
                var filteredRowsCount = new System.Data.Entity.Core.Objects.ObjectParameter("FilteredRowsCount", typeof(int));
                                
                int trc = Convert.ToInt32(totalRowsCount.Value);
                int frc = Convert.ToInt32(filteredRowsCount.Value);
                int srn = Convert.ToInt32(param.iDisplayStart);
                int ern = Convert.ToInt32(param.iDisplayStart + param.iDisplayLength);

                DateTime sdate = Convert.ToDateTime(Session["SDATE"]).Date;
                DateTime edate = Convert.ToDateTime(Session["EDATE"]).Date;

                int Comyid = Convert.ToInt32(Session["compyid"]);

                string squery = "exec [pr_SearchImportLorryHire] @FilterTerm='" + param.sSearch + "',";
                squery += "@SortIndex=" + Convert.ToInt32(Request["iSortCol_0"]) + ",@SortDirection='" + Request["sSortDir_0"] + "',";
                squery += "@StartRowNum=" + srn + ",@EndRowNum=" + ern + ",";
                squery += "@TotalRowsCount=" + trc + ",@FilteredRowsCount=" + frc + ",";
                squery += "@PSDATE='" + sdate.ToString("yyyy-MM-dd") + "',@PEDATE='" + edate.ToString("yyyy-MM-dd") + "',@PCOMPYID=" + Comyid + "";
              

                var result = context.Database.SqlQuery<pr_SearchImportLorryHire_Result>(squery).ToList();

                //var data = e.pr_SearchImportLorryHire(param.sSearch, Convert.ToInt32(Request["iSortCol_0"]), Request["sSortDir_0"], param.iDisplayStart, param.iDisplayStart + param.iDisplayLength,
                //    totalRowsCount, filteredRowsCount, Convert.ToDateTime(Session["SDATE"]), Convert.ToDateTime(Session["EDATE"]), Convert.ToInt32(Session["compyid"]));
                var aaData = result.Select(d => new string[] { d.GILDATE.Value.ToString("dd/MM/yyyy"), d.GILDNO.ToString(), d.CONTNRNO, d.CONTNRSCODE, d.IGMNO, d.VSLNAME, d.STMRNAME, d.GILBILLNO, d.GIDID.ToString(), d.GILDID.ToString() }).ToArray();
                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount.Value),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount.Value)
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [Authorize(Roles = "ImportLorryHireEdit")]
        public void Edit(int id)
        {
            Response.Redirect("/ImportLorryHire/Form/" + id);
        }
        //----------------------Initializing Form--------------------------//
        [Authorize(Roles = "ImportLorryHireCreate")]
        public ActionResult Form(int? id = 0)
        {
            if (Convert.ToInt32(Session["compyid"]) == 0) { return RedirectToAction("Login", "Account"); }
            GateInLorryHireDetail tab = new GateInLorryHireDetail();
            tab.GILDATE = DateTime.Now.Date;
            tab.GILTIME = DateTime.Now; tab.GILBILLDATE = DateTime.Now.Date;
            tab.GILDID = 0;
            ViewBag.GIDID = new SelectList(context.Database.SqlQuery<VW_IMPORT_LORRYHIRE_DETAIL_CTRL_ASSGN>("select * from VW_IMPORT_LORRYHIRE_DETAIL_CTRL_ASSGN").ToList(), "GIDID", "CONTNRNO");



            if (id != 0)//Edit Mode
            {
                tab = context.GateInLorryHireDetail.Find(id);


                //-----------Getting Gate_In Details-----------------//

                var query = context.Database.SqlQuery<string>("select CONTNRNO from GATEINDETAIL where GIDID=" + tab.GIDID).ToList();
                ViewBag.CONTNRNO = query[0].ToString();

            }
            return View(tab);
        }


        public JsonResult AutoContnrno(string term)
        {
            CFSImportEntities db = new CFSImportEntities();
            var result = (from c in db.VW_IMPORT_LORRYHIRE_DETAIL_CTRL_ASSGN
                          where c.CONTNRNO.ToLower().Contains(term.ToLower())
                          select new { c.CONTNRNO, c.GIDID, c.CONTNRSDESC, c.GIDATE, c.PRDTDESC, c.TRNSPRTNAME, c.IGMNO, c.GPLNO, c.VSLNAME, c.VOYNO, c.IMPRTNAME, c.STMRNAME }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContDetails(int id)
        {
            var data = context.Database.SqlQuery<VW_IMPORT_LORRYHIRE_DETAIL_CTRL_ASSGN>("SELECT * FROM VW_IMPORT_LORRYHIRE_DETAIL_CTRL_ASSGN WHERE GIDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContDetails_Mod(int id)
        {
            var data = context.Database.SqlQuery<VW_IMPORT_LORRYHIRE_DETAIL_CTRL_MOD_ASSGN>("SELECT * FROM VW_IMPORT_LORRYHIRE_DETAIL_CTRL_MOD_ASSGN WHERE GIDID=" + id + "").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public void savedata(GateInLorryHireDetail tab)
        {
            string todaydt = Convert.ToString(DateTime.Now);
            string todayd = Convert.ToString(DateTime.Now.Date);

            tab.COMPYID = Convert.ToInt32(Session["compyid"]);
            tab.PRCSDATE = DateTime.Now;
            tab.SDPTID = 1;

            string indate = Convert.ToString(tab.GILDATE);
            if (indate != null || indate != "")
            {
                tab.GILDATE = Convert.ToDateTime(indate).Date;
            }
            else { tab.GILDATE = DateTime.Now.Date; }

            if (tab.GILDATE > Convert.ToDateTime(todayd))
            {
                tab.GILDATE = Convert.ToDateTime(todayd);
            }

            string intime = Convert.ToString(tab.GILTIME);
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

                        tab.GILTIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GILTIME = DateTime.Now; }
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

                        tab.GILTIME = Convert.ToDateTime(in_datetime);
                    }
                    else { tab.GILTIME = DateTime.Now; }
                }
            }
            else { tab.GILTIME = DateTime.Now; }

            if (tab.GILTIME > Convert.ToDateTime(todaydt))
            {
                tab.GILTIME = Convert.ToDateTime(todaydt);
            }

            //tab.GILDATE = tab.GILTIME.Date;

            if (tab.GILDID == 0)
                tab.CUSRID = Session["CUSRID"].ToString();
            tab.LMUSRID = Session["CUSRID"].ToString(); ;
            if (tab.GILDID != 0)
            {
                context.Entry(tab).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

            }
            else
            {
                tab.GILNO = Convert.ToInt32(Autonumber.autonum("GateInLorryHireDetail", "GILNO", "GILNO<>0 and COMPYID=" + Convert.ToInt32(Session["compyid"]) + " and SDPTID=1").ToString());
                int ano = tab.GILNO;
                tab.GILDNO = ano.ToString();

                context.GateInLorryHireDetail.Add(tab);
                context.SaveChanges();

                Session["GILTIME"] = tab.GILTIME;
                Session["GILBILLDATE"] = tab.GILBILLDATE;
                Session["GILBILLNO"] = tab.GILBILLNO; 
                Session["GILNARTN"] = tab.GILNARTN;

            }
            Response.Redirect("Index");
        }
        //..........................Printview...
        [Authorize(Roles = "ImportLorryHirePrint")]
        public void PrintView(int? id = 0)
        {

            //  ........delete TMPRPT...//
            context.Database.ExecuteSqlCommand("DELETE FROM TMPRPT_IDS WHERE KUSRID='" + Session["CUSRID"] + "'");
            var TMPRPT_IDS = TMP_InsertPrint.InsertToTMP("TMPRPT_IDS", "ImportLorryHire", Convert.ToInt32(id), Session["CUSRID"].ToString());
            if (TMPRPT_IDS == "Successfully Added")
            {
                ReportDocument cryRpt = new ReportDocument();
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;


                cryRpt.Load(ConfigurationManager.AppSettings["Reporturl"] + "Import_Lorryhire.RPT");

                cryRpt.RecordSelectionFormula = "{VW_IMPORT_LORRYHIRE_PRINT_ASSGN.KUSRID} = '" + Session["CUSRID"].ToString() + "' AND {VW_IMPORT_LORRYHIRE_PRINT_ASSGN.GILDID} = " + id;



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
                cryRpt.Close();
                cryRpt.Dispose();
                GC.Collect();
                stringbuilder.Clear();
            }

        }
        //end
        //...............Delete Row.............
        [Authorize(Roles = "ImportLorryHireDelete")]
        public void Del()
        {
            String id = Request.Form.Get("id");
            String fld = Request.Form.Get("fld");
            String temp = Delete_fun.delete_check1(fld, id);

            if (temp.Equals("PROCEED"))
            {
                GateInLorryHireDetail GateInLorryHireDetail = context.GateInLorryHireDetail.Find(Convert.ToInt32(id));
                context.GateInLorryHireDetail.Remove(GateInLorryHireDetail);
                context.SaveChanges();
                Response.Write("Deleted successfully...");
            }
            else
                Response.Write(temp);

        }//-----End of Delete Row
    }
}